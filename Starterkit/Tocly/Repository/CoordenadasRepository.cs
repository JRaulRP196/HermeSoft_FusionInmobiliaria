using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Repository
{
    public class CoordenadasRepository
    {

        private readonly AppDbContext _context;

        public CoordenadasRepository(AppDbContext context)
        {
            _context = context;
        }

        #region Utilidades

        public async Task<Coordenadas> Agregar(string lote, string X, string Y, int idMapa)
        {
            if (X == null || Y == null || lote == null)
            {
                return null;
            }

            Coordenadas coordenadas = new Coordenadas();
            coordenadas.IdMapa = idMapa;
            coordenadas.Lote = lote;
            coordenadas.X = X;
            coordenadas.Y = Y;
            _context.COORDENADAS.Add(coordenadas);
            if (await _context.SaveChangesAsync() > 0)
            {
                return coordenadas;
            }
            return null;
        }

        public async Task<IEnumerable<Coordenadas>> GetCoordenadasPorMapa(int id)
        {
            IEnumerable<Coordenadas> coordenadas = await _context.COORDENADAS.Where(c => c.IdMapa == id).ToListAsync();
            return coordenadas;
        }

        #endregion

    }
}
