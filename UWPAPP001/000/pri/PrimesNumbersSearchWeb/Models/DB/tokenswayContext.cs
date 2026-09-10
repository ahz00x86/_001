using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PrimesNumbersSearchWeb.Models.DB
{
    public partial class tokenswayContext : DbContext
    {
        public tokenswayContext()
        {
        }

        public tokenswayContext(DbContextOptions<tokenswayContext> options)
            : base(options)
        {
        }

        public virtual DbSet<LpnsProcesosActivo> LpnsProcesosActivos { get; set; } = null!;
        public virtual DbSet<Proceso> Procesos { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var ccnn = String.Empty;
                #if (DEBUG)
                var builder = WebApplication.CreateBuilder();
                ccnn = builder.Configuration["apzyxGames"];
                #else
                ccnn = _configuration.GetConnectionString("apzyxGames");
                #endif

                optionsBuilder.UseSqlServer(ccnn);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LpnsProcesosActivo>(entity =>
            {
                entity.HasKey(e => new { e.Numero, e.Colision })
                    .HasName("LPNS_ProcesosActivos_PK");

                entity.ToTable("LPNS_ProcesosActivos");

                entity.Property(e => e.Numero)
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasColumnName("numero");

                entity.Property(e => e.Colision).HasColumnName("colision");
            });

            modelBuilder.Entity<Proceso>(entity =>
            {
                entity.HasKey(e => new { e.Fecha, e.Id });

                entity.Property(e => e.Fecha)
                    .HasColumnType("datetime")
                    .HasColumnName("FECHA");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Cuenta)
                    .HasMaxLength(255)
                    .HasColumnName("CUENTA");

                entity.Property(e => e.Estado).HasColumnName("ESTADO");

                entity.Property(e => e.Pagado).HasColumnName("PAGADO");

                entity.Property(e => e.Pe).HasColumnName("PE");

                entity.Property(e => e.Pr).HasColumnName("PR");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
