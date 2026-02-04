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
        public DbSet<IndicadoresBancarios> INDICADORES_BANCARIOS { get; set; }
        public DbSet<IndicadoresBancos> INDICADORES_BANCOS { get; set; }
        public DbSet<TasaInteres> TASAS_INTERES { get; set; }
        public DbSet<EscenarioTasaInteres> ESCENARIOS_TASAS_INTERES { get; set; }              
        public DbSet<HistoricoCambioBancario> HISTORICO_CAMBIOS_BANCARIOS { get; set; }



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
                entity.HasKey(e => e.IdEndeudamiento);
                entity.HasOne(e => e.Banco)
                      .WithMany()
                      .HasForeignKey(e => e.IdBanco)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.TipoAsalariado)
                      .WithMany()
                      .HasForeignKey(e => e.IdTipoAsalariado);
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
                entity.HasKey(e => e.IdSeguroBanco);
                entity.HasOne(e => e.Banco)
                      .WithMany()
                      .HasForeignKey(e => e.IdBanco)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Seguro)
                      .WithMany()
                      .HasForeignKey(e => e.IdSeguro);
            });

            // Configuraciones de IndicadoresBancarios
            modelBuilder.Entity<IndicadoresBancarios>(entity =>
            {
                entity.HasKey(e => e.IdIndicador);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
            });

            // Configuraciones de IndicadoresBancos
            modelBuilder.Entity<IndicadoresBancos>(entity =>
            {
                entity.HasKey(e => e.IdIndicadorBanco);
                entity.HasOne(e => e.Banco)
                      .WithMany()
                      .HasForeignKey(e => e.IdBanco)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Indicador)
                      .WithMany()
                      .HasForeignKey(e => e.IdIndicador);
            });

            // Configuraciones de TasaInteres
            modelBuilder.Entity<TasaInteres>(entity =>
            {
                entity.HasKey(e => e.IdTasaInteres);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
            });

            // Configuraciones de EscenarioTasaInteres
            modelBuilder.Entity<EscenarioTasaInteres>(entity =>
            {
                entity.HasKey(e => e.IdEscenario);
                entity.HasOne(e => e.Banco)
                      .WithMany()
                      .HasForeignKey(e => e.IdBanco)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.TasaInteres)
                      .WithMany()
                      .HasForeignKey(e => e.IdTasaInteres);
            });

            modelBuilder.Entity<HistoricoCambioBancario>(entity =>
            {
                entity.HasKey(e => e.IdHistorico);
            });

        }
    }
}
