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
        public DbSet<Banco> BANCOS { get; set; }
        public DbSet<TipoAsalariado> TIPOS_ASALARIADOS { get; set; }
        public DbSet<EndeudamientoMaximo> ENDEUDAMIENTOS_MAXIMOS { get; set; }
        public DbSet<Seguro> SEGUROS { get; set; }
        public DbSet<SeguroBanco> SEGUROS_BANCOS { get; set; }
        public DbSet<TasaInteres> TASAS_INTERES { get; set; }
        public DbSet<EscenarioTasaInteres> ESCENARIOS_TASAS_INTERES { get; set; }
    }
}
