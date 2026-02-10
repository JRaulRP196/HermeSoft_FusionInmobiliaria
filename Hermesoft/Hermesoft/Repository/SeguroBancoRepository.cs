using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models.Banco;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Repository
{
    public class SeguroBancoRepository
    {

        private AppDbContext _context;

        public SeguroBancoRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        #region Utilidades

        public async Task<SeguroBanco> Agregar(SeguroBanco seguro)
        {
            await _context.SEGUROS_BANCOS.AddAsync(seguro);
            if(await _context.SaveChangesAsync() > 0)
            {
                return seguro;
            }
            return null;
        }

        public void Editar(SeguroBanco seguro)
        {
            _context.SEGUROS_BANCOS.Update(seguro);
        }

        #endregion

    }
}
