using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models.Banco;
using HermeSoft_Fusion.Models.Servicios;
using HermeSoft_Fusion.Repository;
using HermeSoft_Fusion.Repository.Servicios;

namespace HermeSoft_Fusion.Business
{
    public class IndicadoresBancariosBusiness
    {

        private readonly IndicadoresBancariosRepository _indicadorBancarioRepository;
        private readonly IndicadorEconomicoRepository _economicoRepository;
        private readonly AppDbContext _context;

        public IndicadoresBancariosBusiness(IndicadoresBancariosRepository indicadoresBancariosRepository,
            IndicadorEconomicoRepository economicoRepository, AppDbContext context)
        {
            _indicadorBancarioRepository = indicadoresBancariosRepository;
            _economicoRepository = economicoRepository;
            _context = context;
        }

        public async Task<IEnumerable<IndicadoresBancarios>> Obtener()
        {
            return await _indicadorBancarioRepository.Obtener();
        }

        private async Task Agregar()
        {
                IEnumerable<IndicadoresBancarios> indicadores = await ObtenerDelBCCR();
                foreach (IndicadoresBancarios indicador in indicadores)
                {
                    await _indicadorBancarioRepository.Agregar(indicador);
                }
        }

        public async Task Editar()
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                IEnumerable<IndicadoresBancarios> indicadoresBD = await Obtener();
                if(indicadoresBD.Count() <= 0)
                {
                    await Agregar();
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return;
                }

                IEnumerable<IndicadoresBancarios> indicadoresBCCR = await ObtenerDelBCCR();
                foreach (var indicadorBCCR in indicadoresBCCR)
                {
                    var indicadorBD = indicadoresBD
                        .FirstOrDefault(x => x.Nombre == indicadorBCCR.Nombre);
                    indicadorBD.FechaVigente = indicadorBCCR.FechaVigente;
                    indicadorBD.PorcSeguro = indicadorBCCR.PorcSeguro;
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<IEnumerable<IndicadoresBancarios>> ObtenerDelBCCR()
        {
            Serie TBP = await _economicoRepository.Obtener("ObtenerTBP");
            Serie prime = await _economicoRepository.Obtener("ObtenerPrime");
            Serie sorf1Mes = await _economicoRepository.Obtener("ObtenerSOFR_1mes");
            Serie sorf3Mes = await _economicoRepository.Obtener("ObtenerSOFR_3meses");
            Serie sorf6Mes = await _economicoRepository.Obtener("ObtenerSOFR_6meses");
            List<IndicadoresBancarios> indicadores = new List<IndicadoresBancarios>();
            indicadores.Add(new IndicadoresBancarios { FechaVigente = DateTime.Now, Nombre = "N/A",
                PorcSeguro = 0 });
            indicadores.Add(new IndicadoresBancarios { FechaVigente = DateTime.Parse(TBP.fecha), Nombre = "TBP",
                PorcSeguro = TBP.valorDatoPorPeriodo });
            indicadores.Add(new IndicadoresBancarios { FechaVigente = DateTime.Parse(prime.fecha), Nombre = "PRIME",
                PorcSeguro = prime.valorDatoPorPeriodo });
            indicadores.Add(new IndicadoresBancarios { FechaVigente = DateTime.Parse(sorf1Mes.fecha), Nombre = "SOFT 1 MES",
                PorcSeguro = sorf1Mes.valorDatoPorPeriodo });
            indicadores.Add(new IndicadoresBancarios { FechaVigente = DateTime.Parse(sorf3Mes.fecha), Nombre = "SOFT 3 MESES",
                PorcSeguro = sorf3Mes.valorDatoPorPeriodo});
            indicadores.Add(new IndicadoresBancarios { FechaVigente = DateTime.Parse(sorf6Mes.fecha), Nombre = "SOFT 6 MESES",
                PorcSeguro = sorf6Mes.valorDatoPorPeriodo});
            return indicadores;
        }

    }
}
