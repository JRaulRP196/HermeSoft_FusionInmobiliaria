using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;

namespace HermeSoft_Fusion.Business
{
    public class BancoBusiness
    {

        private BancoRepository _bancoRepository;
        private EndeudamientoMaximoRepository _endeudamientoMaximoRepository;
        private SeguroBancoRepository _seguroBancoRepository;
        private EscenarioTasaInteresRepository _escenarioTasaInteresRepository;
        private PlazosEscenariosRepository _plazosEscenariosRepository;
        private AppDbContext _context;

        public BancoBusiness(BancoRepository bancoRepository, EndeudamientoMaximoRepository endeudamientoMaximoRepository, 
            SeguroBancoRepository seguroBancoRepository, EscenarioTasaInteresRepository escenarioTasaInteresRepository, 
            PlazosEscenariosRepository plazosEscenariosRepository, AppDbContext context)
        {
            _bancoRepository = bancoRepository;
            _endeudamientoMaximoRepository = endeudamientoMaximoRepository;
            _seguroBancoRepository = seguroBancoRepository;
            _escenarioTasaInteresRepository = escenarioTasaInteresRepository;
            _plazosEscenariosRepository = plazosEscenariosRepository;
            _context = context;
        }

        public async Task<Banco> Agregar(Banco banco, IFormFile LogoFile)
        {
            banco.Logo = await GuardarLogo(LogoFile);
            return await _bancoRepository.Agregar(banco);
        }

        public async Task<Banco> Editar(Banco banco, IFormFile LogoFile)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Banco bancoRespuesta = await _bancoRepository.ObtenerPorId(banco.IdBanco);
                if (LogoFile != null)
                {
                    bancoRespuesta.Logo = await GuardarLogo(LogoFile);
                }

                bancoRespuesta.Comision = banco.Comision;
                bancoRespuesta.Enlace = banco.Enlace;
                bancoRespuesta.HonorarioAbogado = banco.HonorarioAbogado;
                bancoRespuesta.MaxCredito = banco.MaxCredito;
                bancoRespuesta.Nombre = banco.Nombre;
                bancoRespuesta.TipoCambio = banco.TipoCambio;
                
                await ActualizarEscenarios(banco, bancoRespuesta);
                ActualizarSeguros(banco, bancoRespuesta);
                ActualizarEndeudamientos(banco, bancoRespuesta);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return banco;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

        public async Task<IEnumerable<Banco>> ObtenerTodos()
        {
            return await _bancoRepository.ObtenerTodos();
        }

        public async Task<Banco?> ObtenerPorId(int id)
        {
            return await _bancoRepository.ObtenerPorId(id);
        }

        #region Helpers

        private async Task<string> GuardarLogo(IFormFile LogoFile)
        {
            string carpetaLogos = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/logos");
            if (!Directory.Exists(carpetaLogos))
                Directory.CreateDirectory(carpetaLogos);

            string extension = Path.GetExtension(LogoFile.FileName);
            string nombreArchivo = Guid.NewGuid().ToString() + extension;
            string rutaFisica = Path.Combine(carpetaLogos, nombreArchivo);

            using (var stream = new FileStream(rutaFisica, FileMode.Create))
            {
                await LogoFile.CopyToAsync(stream);
            }

            string rutaBD = $"/uploads/logos/{nombreArchivo}";
            return rutaBD;
        }

        private async Task ActualizarEscenarios(Banco banco, Banco bancoRespuesta)
        {

            List<EscenarioTasaInteres> nuevos = banco.EscenariosTasaInteres;
            var viejos = bancoRespuesta?.EscenariosTasaInteres ?? new List<EscenarioTasaInteres>();

            foreach(EscenarioTasaInteres escenario in nuevos)
            {
                if(escenario.IdEscenario == 0)
                {
                    await _escenarioTasaInteresRepository.Agregar(escenario);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var escenarioBD = viejos.Find(x => x.IdEscenario == escenario.IdEscenario);
                    escenarioBD.Nombre = escenario.Nombre;
                    escenarioBD.IdTasaInteres = escenario.IdTasaInteres;
                    await ActualizarPlazos(escenario, escenarioBD);
                }
            }

            var escenariosBorrar = viejos
                .Where(v => !nuevos.Any(n => n.IdEscenario == v.IdEscenario))
                .ToList();

            foreach (EscenarioTasaInteres escenario in escenariosBorrar)
            {
                _escenarioTasaInteresRepository.Eliminar(escenario);
            }

        }

        private async Task ActualizarPlazos(EscenarioTasaInteres escenario, EscenarioTasaInteres escenarioBD)
        {
            List<PlazosEscenarios> nuevos = escenario.PlazosEscenarios;
            var viejos = escenarioBD?.PlazosEscenarios ?? new List<PlazosEscenarios>();

            foreach (PlazosEscenarios plazo in nuevos)
            {
                if (plazo.IdPlazoEscenario == 0)
                {
                    plazo.IdEscenario = escenario.IdEscenario;
                    await _plazosEscenariosRepository.Agregar(plazo);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var plazoBD = viejos.Find(x => x.IdPlazoEscenario == plazo.IdPlazoEscenario);
                    plazoBD.Plazo = plazo.Plazo;
                    plazoBD.PorcAdicional = plazo.PorcAdicional;
                    plazoBD.IdIndicador = plazo.IdIndicador;
                }
            }

            var plazosBorrar = viejos
                .Where(v => !nuevos.Any(n => n.IdPlazoEscenario == v.IdPlazoEscenario))
                .ToList();

            foreach(PlazosEscenarios plazo in plazosBorrar)
            {
                _plazosEscenariosRepository.Eliminar(plazo);
            }
        }

        private void ActualizarSeguros(Banco banco, Banco bancoBD)
        {
            foreach (SeguroBanco seguroBanco in banco.SeguroBancos)
            {
                var seguroBD = bancoBD.SeguroBancos.Find(x => x.IdSeguroBanco == seguroBanco.IdSeguroBanco);
                seguroBD.PorcSeguro = seguroBanco.PorcSeguro;
            }
        }

        private void ActualizarEndeudamientos(Banco banco, Banco bancoBD)
        {
            foreach (EndeudamientoMaximo endeudamiento in banco.EndeudamientoMaximos)
            {
                var endeudamientoBD = bancoBD.EndeudamientoMaximos.Find(x => x.IdEndeudamiento == endeudamiento.IdEndeudamiento);
                endeudamientoBD.PorcEndeudamiento = endeudamiento.PorcEndeudamiento;
            }
        }

        #endregion

    }
}
