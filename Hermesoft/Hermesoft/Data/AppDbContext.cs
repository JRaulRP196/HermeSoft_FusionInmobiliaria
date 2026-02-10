using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Models.Banco;
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
        public DbSet<IndicadoresBancarios> INDICADORES_BANCARIOS { get; set; }
        public DbSet<TasaInteres> TASAS_INTERES { get; set; }
        public DbSet<EscenarioTasaInteres> ESCENARIOS_TASAS_INTERES { get; set; }
        public DbSet<PlazosEscenarios> PLAZOS_ESCENARIOS { get; set; }
        public DbSet<HistoricoCambiosBancarios> HISTORICO_CAMBIOS_BANCARIOS { get; set; }
        public DbSet<TipoCambio> TIPO_CAMBIO {  get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuraciones de Banco
            modelBuilder.Entity<Banco>(entity =>
            {
                entity.HasKey(e => e.IdBanco);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Enlace).HasMaxLength(500);
            });

            // Configuraciones de TipoAsalariado
            modelBuilder.Entity<TipoAsalariado>(entity =>
            {
                entity.HasKey(e => e.IdTipoAsalariado);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
            });

            // Configuraciones de EndeudamientoMaximo
            modelBuilder.Entity<EndeudamientoMaximo>(entity =>
            {
                entity.HasOne(e => e.Banco)
                      .WithMany(b => b.EndeudamientoMaximos)
                      .HasForeignKey(e => e.IdBanco)
                      .OnDelete(DeleteBehavior.Cascade);

            });

            // Configuraciones de Seguro
            modelBuilder.Entity<Seguro>(entity =>
            {
                entity.HasKey(e => e.IdSeguro);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
            });

            // Configuraciones de SeguroBanco
            modelBuilder.Entity<SeguroBanco>(entity =>
            {
                entity.HasOne(e => e.Banco)
                      .WithMany(b => b.SeguroBancos)
                      .HasForeignKey(e => e.IdBanco)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuraciones de IndicadoresBancarios
            modelBuilder.Entity<IndicadoresBancarios>(entity =>
            {
                entity.HasKey(e => e.IdIndicador);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
            });

            // Configuraciones de TasaInteres
            modelBuilder.Entity<TasaInteres>(entity =>
            {
                entity.HasKey(e => e.IdTasaInteres);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<EscenarioTasaInteres>(entity =>
            {
                entity.HasKey(e => e.IdEscenario);

                entity.HasMany(e => e.PlazosEscenarios)
                      .WithOne(p => p.EscenarioTasaInteres)
                      .HasForeignKey(p => p.IdEscenario)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TipoCambio>(entity =>
            {
                entity.HasKey(e=>e.IdTipoCambio);

                entity.HasMany(e => e.Bancos)
                .WithOne(b => b.TipoCambio)
                .HasForeignKey(b => b.IdTipoCambio)
                .OnDelete(DeleteBehavior.Restrict);
  
            });

        }
    }
}
