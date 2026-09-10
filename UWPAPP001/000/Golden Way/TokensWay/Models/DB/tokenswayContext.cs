using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TokensWay
{
    public partial class tokenswayContext : DbContext
    {
        private IConfiguration _configuration;

        public tokenswayContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public tokenswayContext(DbContextOptions<tokenswayContext> options)
            : base(options)
        {
        }

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
