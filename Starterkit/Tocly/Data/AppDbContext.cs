using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using HermeSoft_Fusion.Models;

namespace HermeSoft_Fusion.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Mapa> MAPAS { get; set; }
        public DbSet<Coordenadas> COORDENADAS { get; set; }

        public DbSet<Banco> BANCOS { get; set; }
        public DbSet<TipoAsalariado> TIPOS_ASALARIADOS { get; set; }
        public DbSet<EndeudamientoMaximo> ENDEUDAMIENTOS_MAXIMOS { get; set; }
        public DbSet<Seguro> SEGUROS { get; set; }
        public DbSet<SeguroBanco> SEGUROS_BANCOS { get; set; }
        public DbSet<TasaInteres> TASAS_INTERES { get; set; }
        public DbSet<EscenarioTasaInteres> ESCENARIOS_TASAS_INTERES { get; set; }
        public DbSet<HistoricoCambioBancario> HISTORICO_CAMBIOS_BANCARIOS { get; set; }
    }
}
