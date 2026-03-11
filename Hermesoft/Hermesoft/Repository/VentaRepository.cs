using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;

namespace HermeSoft_Fusion.Repository
{
    public class VentaRepository
    {

        private readonly AppDbContext _context;

        public VentaRepository(AppDbContext context)
        {
            _context = context;
        }

        #region Utilidades

        public async Task Agregar(Venta venta)
        {
            _context.VENTAS.Add(venta);
        }

        public async Task<IEnumerable<Venta>> Obtener()
        {
            return _context.VENTAS.ToList();
        }

        #endregion

    }
}
