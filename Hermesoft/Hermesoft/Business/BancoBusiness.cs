using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;

namespace HermeSoft_Fusion.Business
{
    public class BancoBusiness
    {

        private BancoRepository _bancoRepository;
        private EscenarioTasaInteresRepository _escenarioTasaInteresRepository;
        private PlazosEscenariosRepository _plazosEscenariosRepository;
        private SeguroRepository _seguroRepository;
        private TipoAsalariadoRepository _tipoAsalariadoRepository;
        private AppDbContext _context;

        public BancoBusiness(BancoRepository bancoRepository, EscenarioTasaInteresRepository escenarioTasaInteresRepository, 
            PlazosEscenariosRepository plazosEscenariosRepository, SeguroRepository seguroRepository, 
            TipoAsalariadoRepository tipoAsalariadoRepository, AppDbContext context)
        {
            _bancoRepository = bancoRepository;
            _escenarioTasaInteresRepository = escenarioTasaInteresRepository;
            _plazosEscenariosRepository = plazosEscenariosRepository;
            _seguroRepository = seguroRepository;
            _tipoAsalariadoRepository = tipoAsalariadoRepository;
            _context = context;
        }

        public async Task<Banco> Agregar(Banco banco, IFormFile LogoFile)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                banco.Logo = await GuardarLogo(LogoFile);
                await _bancoRepository.Agregar(banco);
                await _context.SaveChangesAsync();
                transaction.Commit();
                return banco;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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

        public async Task<Banco> IniciarBanco()
        {
            Banco banco = new Banco();
            await IniciarSeguros(banco);
            await IniciarEndeudamientos(banco);
            return banco;
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

        private async Task IniciarEndeudamientos(Banco banco)
        {
            var tiposAsalariados = await _tipoAsalariadoRepository.Obtener();
            foreach (TipoAsalariado tipoAsalariado in tiposAsalariados)
            {
                var endeudamientoMaximo = new EndeudamientoMaximo();
                endeudamientoMaximo.IdTipoAsalariado = tipoAsalariado.IdTipoAsalariado;
                endeudamientoMaximo.TipoAsalariado = tipoAsalariado;
                banco.EndeudamientoMaximos.Add(endeudamientoMaximo);
            }
        }

        private async Task IniciarSeguros(Banco banco)
        {
            var seguros = await _seguroRepository.Obtener();
            foreach (Seguro seguro in seguros)
            {
                var seguroBanco = new SeguroBanco();
                seguroBanco.IdSeguro = seguro.IdSeguro;
                seguroBanco.Seguro = seguro;
                banco.SeguroBancos.Add(seguroBanco);
            }
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
