using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models.Banco;

namespace HermeSoft_Fusion.Repository
{
    public class HistoricoCambiosBancariosRepository
    {

        private readonly AppDbContext _context;

        public HistoricoCambiosBancariosRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Agregar(HistoricoCambiosBancarios historicoCambiosBancarios)
        {
            await _context.HISTORICO_CAMBIOS_BANCARIOS.AddAsync(historicoCambiosBancarios);
        }

    }
}
