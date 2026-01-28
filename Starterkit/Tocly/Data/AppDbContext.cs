using HermeSoft_Fusion.Models;
using Microsoft.EntityFrameworkCore;
namespace HermeSoft_Fusion.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Mapa> MAPAS { get; set; }
        public DbSet<Coordenadas> COORDENADAS { get; set; }
        public DbSet<Primas> PRIMAS { get; set; }
        public DbSet<DesglosesPrimas> DESGLOSES_PRIMAS { get; set; }
    }
}
