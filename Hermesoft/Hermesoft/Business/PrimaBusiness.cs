using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Business
{
    public class PrimaBusiness
    {

        private readonly PrimaRepository _primaRepository;
        private readonly AppDbContext _context;

        public PrimaBusiness(PrimaRepository primaRepository, AppDbContext context)
        {
            _primaRepository = primaRepository;
            _context = context;
        }

        #region Utilidades

        public async Task<Primas> Agregar2(Primas prima) // Borrar
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Primas primaBD = await Obtener(prima.CorreoCliente);
                if (primaBD != null)
                {
                    prima = await Editar(primaBD, prima);
                }
                else
                {
                    await _primaRepository.Agregar(prima);
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return prima;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al agregar la prima: {ex.Message}");
            }
        }

        public async Task<Primas> Agregar(Primas prima)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _primaRepository.Agregar(prima);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return prima;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al agregar la prima: {ex.Message}");
            }
        }

        public async Task<Primas> Editar(Primas primaBD, Primas prima)
        {
            try
            {
                primaBD.FechaInicio = prima.FechaInicio;
                primaBD.FechaCierre = prima.FechaCierre;
                primaBD.Porcentaje = prima.Porcentaje;
                primaBD.Total = prima.Total;
                _context.DESGLOSES_PRIMAS.RemoveRange(primaBD.DesglosesPrimas);
                primaBD.DesglosesPrimas.Clear();

                foreach (var desglose in prima.DesglosesPrimas)
                {
                    desglose.Prima = primaBD;
                    primaBD.DesglosesPrimas.Add(desglose);
                    
                }

                await _context.SaveChangesAsync();
                return primaBD;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al editar la prima: {ex.Message}");
            }
        }

        public async Task<Primas> Obtener(string correoCliente)
        {
            return await _primaRepository.Obtener(correoCliente);
        }

        public async Task<List<Primas>> ObtenerPorCorreo(string correoCliente) //Q
        {
            return await _primaRepository.ObtenerPorCorreo(correoCliente);
        }

        public async Task<Primas> ObtenerPorId(int idPrima) //Q
        {
            return await _primaRepository.ObtenerPorId(idPrima);
        }
        #endregion

    }
}
