using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;

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
            if (await _context.SaveChangesAsync() > 0)
            {
                return escenarioTasaInteres;
            }
            return null;
        }

    }
}
