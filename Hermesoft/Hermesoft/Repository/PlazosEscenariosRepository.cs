using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using Microsoft.EntityFrameworkCore;

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
            return plazo;
        }

        public void Editar(PlazosEscenarios plazo)
        {
            _context.PLAZOS_ESCENARIOS.Update(plazo);
        }

        public void Eliminar(PlazosEscenarios plazo)
        {
            _context.PLAZOS_ESCENARIOS.Remove(plazo);
        }

    }
}
