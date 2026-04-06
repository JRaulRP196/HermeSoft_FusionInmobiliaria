using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Business
{
    public class MapaBusiness
    {

        private readonly AppDbContext _context;
        private readonly MapaRepository _mapaRepository;

        public MapaBusiness(AppDbContext context, MapaRepository mapaRepository)
        {
            _context = context;
            _mapaRepository = mapaRepository;
        }


        public async Task<IEnumerable<Mapa>> GetMapas()
        {
            return await _mapaRepository.GetMapas();
        }

        public async Task<Mapa> GetMapaPorCondominio(string condominio)
        {
            return await _mapaRepository.GetMapaPorCondominio(condominio);
        }

        public async Task<Mapa> Agregar(Mapa mapa, IFormFile ImagenMapa)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/mapas");

                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                var nombreArchivo = Guid.NewGuid() + Path.GetExtension(ImagenMapa.FileName);
                var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await ImagenMapa.CopyToAsync(stream);
                }

                // guardar la ruta en la BD
                mapa.Direccion = "/mapas/" + nombreArchivo;

                var mapaBD = await _context.MAPAS.FirstOrDefaultAsync(m => m.condominio == mapa.condominio);

                if (mapaBD != null)
                {
                    mapaBD.Direccion = "/mapas/" + nombreArchivo;
                    _context.MAPAS.Update(mapaBD);
                    if (await _context.SaveChangesAsync() <= 0)
                    {
                        await transaction.RollbackAsync();
                        return null;
                    }
                    await transaction.CommitAsync();
                    return mapaBD;
                }

                await _mapaRepository.Agregar(mapa);
                if (await _context.SaveChangesAsync() <= 0)
                {
                    await transaction.RollbackAsync();
                    return null;
                }
                await transaction.CommitAsync();
                return mapa;
            }
            catch
            {
                await transaction.RollbackAsync();
                return null;
            }

        }

    }
}
