using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;
using HermeSoft_Fusion.Repository.Servicios;

namespace HermeSoft_Fusion.Business
{
    public class TipoCambioBusiness
    {

        private readonly TipoCambioAPIRepository _tipoCambioApi;
        private readonly TipoCambioRepository _tipoCambioRepository;
        private readonly AppDbContext _context;

        public TipoCambioBusiness(TipoCambioRepository tipoCambioRepository, TipoCambioAPIRepository tipoCambioApi, AppDbContext context)
        {
            _tipoCambioApi = tipoCambioApi;
            _tipoCambioRepository = tipoCambioRepository;
            _context = context;
        }

        public async Task<TipoCambio> Obtener()
        {
            return await _tipoCambioRepository.Obtener();
        }

        private async Task Agregar()
        {
            var serie = await _tipoCambioApi.Obtener();
            TipoCambio tipoCambio = new TipoCambio {
                Cambio = serie.valorDatoPorPeriodo
            };
            await _tipoCambioRepository.Agregar(tipoCambio);
        }

        public async Task Editar()
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                TipoCambio tipoCambio = await Obtener();
                if (tipoCambio == null)
                {
                    await Agregar();
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return;
                }
                var serie = await _tipoCambioApi.Obtener();
                tipoCambio.Cambio = serie.valorDatoPorPeriodo;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}
