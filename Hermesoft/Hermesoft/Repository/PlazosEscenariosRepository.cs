using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;

namespace HermeSoft_Fusion.Repository
{
    public class PlazosEscenariosRepository
    {

        private AppDbContext _context;

        public PlazosEscenariosRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task<PlazosEscenarios> Agregar(PlazosEscenarios plazo)
        {
            await _context.PLAZOS_ESCENARIOS.AddAsync(plazo);
            if(await _context.SaveChangesAsync() > 0)
            {
                return plazo;
            }
            return null;
        }

    }
}
