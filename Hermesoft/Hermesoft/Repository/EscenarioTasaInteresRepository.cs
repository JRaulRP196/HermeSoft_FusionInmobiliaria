using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models.Banco;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Repository
{
    public class EscenarioTasaInteresRepository
    {

        private AppDbContext _context;

        public EscenarioTasaInteresRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EscenarioTasaInteres> Agregar(EscenarioTasaInteres escenarioTasaInteres)
        {
            await _context.ESCENARIOS_TASAS_INTERES.AddAsync(escenarioTasaInteres);
            return escenarioTasaInteres;
        }

        public void Editar(EscenarioTasaInteres escenarioTasaInteres)
        {
            _context.ESCENARIOS_TASAS_INTERES.Update(escenarioTasaInteres);
        }

        public void Eliminar(EscenarioTasaInteres escenarioTasa)
        {
            _context.ESCENARIOS_TASAS_INTERES.Remove(escenarioTasa);
        }

        public async Task<EscenarioTasaInteres> Obtener(int id)
        {
            return await _context.ESCENARIOS_TASAS_INTERES
                .Include(e => e.PlazosEscenarios)
                    .ThenInclude(ip => ip.Indicador)
                .FirstOrDefaultAsync(e => e.IdEscenario == id);
        }

    }
}
