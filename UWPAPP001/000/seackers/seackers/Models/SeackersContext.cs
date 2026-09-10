using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace seackers.Models;

public partial class SeackersContext : DbContext
{
    public SeackersContext()
    {
    }

    public SeackersContext(DbContextOptions<SeackersContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Archivo> Archivos { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<CenterLike> CenterLikes { get; set; }

    public virtual DbSet<CenterUser> CenterUsers { get; set; }

    public virtual DbSet<Contenido> Contenidos { get; set; }

    public virtual DbSet<DataUser> DataUsers { get; set; }

    public virtual DbSet<Direccione> Direcciones { get; set; }

    public virtual DbSet<Envio> Envios { get; set; }

    public virtual DbSet<Paise> Paises { get; set; }

    public virtual DbSet<PedidosC> PedidosCs { get; set; }

    public virtual DbSet<ProdCat> ProdCats { get; set; }

    public virtual DbSet<ProductosB> ProductosBs { get; set; }

    public virtual DbSet<Quest> Quests { get; set; }

    public virtual DbSet<UserQuestSelect> UserQuestSelects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Archivo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Archivos__3214EC07ED3715DE");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.FileName).HasMaxLength(120);
            entity.Property(e => e.Titulo).HasMaxLength(120);
            entity.Property(e => e.Url).HasMaxLength(2048);
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC074C3E3B44");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Titulo).HasMaxLength(120);

            entity.HasOne(d => d.IdPadreNavigation).WithMany(p => p.InverseIdPadreNavigation)
                .HasForeignKey(d => d.IdPadre)
                .HasConstraintName("FK_Categorias_Padre");

            entity.HasOne(d => d.IdSeccionNavigation).WithMany(p => p.InverseIdSeccionNavigation)
                .HasForeignKey(d => d.IdSeccion)
                .HasConstraintName("FK_Categorias_Seccion");

            entity.HasOne(d => d.IdTemaNavigation).WithMany(p => p.InverseIdTemaNavigation)
                .HasForeignKey(d => d.IdTema)
                .HasConstraintName("FK_Categorias_Tema");

            entity.HasOne(d => d.IdTematicaNavigation).WithMany(p => p.InverseIdTematicaNavigation)
                .HasForeignKey(d => d.IdTematica)
                .HasConstraintName("FK_Categorias_Tematica");
        });

        modelBuilder.Entity<CenterLike>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.CenterId }).HasName("PK__CenterLi__74103033447973BE");

            entity.HasOne(d => d.Center).WithMany(p => p.CenterLikeCenters)
                .HasForeignKey(d => d.CenterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("centers_usersLikes");

            entity.HasOne(d => d.User).WithMany(p => p.CenterLikeUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_usersLikes");
        });

        modelBuilder.Entity<CenterUser>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.CenterId }).HasName("PK__CenterUs__74103033FDA0E504");

            entity.Property(e => e.AceeptD).HasColumnType("datetime");
            entity.Property(e => e.Started).HasColumnType("datetime");

            entity.HasOne(d => d.Center).WithMany(p => p.CenterUserCenters)
                .HasForeignKey(d => d.CenterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("centers_users");

            entity.HasOne(d => d.User).WithMany(p => p.CenterUserUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_users");
        });

        modelBuilder.Entity<Contenido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Contenid__3214EC0733A8DF24");

            entity.ToTable("Contenido");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Contenido1).HasColumnName("Contenido");
            entity.Property(e => e.Titulo).HasMaxLength(120);

            entity.HasOne(d => d.IdPadreNavigation).WithMany(p => p.InverseIdPadreNavigation)
                .HasForeignKey(d => d.IdPadre)
                .HasConstraintName("FK_Contenido_Padre");

            entity.HasOne(d => d.IdTemaNavigation).WithMany(p => p.InverseIdTemaNavigation)
                .HasForeignKey(d => d.IdTema)
                .HasConstraintName("FK_Contenido_Tema");

            entity.HasOne(d => d.IdTematicaNavigation).WithMany(p => p.InverseIdTematicaNavigation)
                .HasForeignKey(d => d.IdTematica)
                .HasConstraintName("FK_Contenido_Tematica");
        });

        modelBuilder.Entity<DataUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__tmp_ms_x__1788CC4CE59E74DB");

            entity.Property(e => e.ImgHeadUrl).HasMaxLength(2048);
            entity.Property(e => e.ImgLogoUrl).HasMaxLength(2048);
            entity.Property(e => e.Latitud).HasColumnType("decimal(10, 8)");
            entity.Property(e => e.Longitud).HasColumnType("decimal(11, 8)");
            entity.Property(e => e.Name).HasMaxLength(120);
            entity.Property(e => e.Phone).HasMaxLength(16);
            entity.Property(e => e.Url).HasMaxLength(2048);

            entity.HasOne(d => d.User).WithOne(p => p.DataUser)
                .HasForeignKey<DataUser>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("users_data");
        });

        modelBuilder.Entity<Direccione>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Direccio__1788CC4C82DDBDB6");

            entity.Property(e => e.Cp)
                .HasMaxLength(12)
                .HasColumnName("CP");
            entity.Property(e => e.Direccion).HasMaxLength(256);
            entity.Property(e => e.Pais).HasColumnName("PAIS");

            entity.HasOne(d => d.PaisNavigation).WithMany(p => p.Direcciones)
                .HasForeignKey(d => d.Pais)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Direccion_Pais");

            entity.HasOne(d => d.User).WithOne(p => p.Direccione)
                .HasForeignKey<Direccione>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Direccion_user");
        });

        modelBuilder.Entity<Envio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Envios__3214EC0720722D9F");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Nombre).HasMaxLength(64);
            entity.Property(e => e.Pvp)
                .HasColumnType("decimal(11, 2)")
                .HasColumnName("PVP");
            entity.Property(e => e.UrlImgMain).HasMaxLength(256);
        });

        modelBuilder.Entity<Paise>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Paises__3214EC074131F8E0");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Code).HasColumnName("CODE");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<PedidosC>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.Id }).HasName("PK__PedidosC__74A9828CAFAF9DC8");

            entity.ToTable("PedidosC");

            entity.Property(e => e.AceeptD).HasColumnType("datetime");
            entity.Property(e => e.Cliente).HasMaxLength(120);
            entity.Property(e => e.Cp)
                .HasMaxLength(12)
                .HasColumnName("CP");
            entity.Property(e => e.Direccion).HasMaxLength(256);
            entity.Property(e => e.EnvioId).HasColumnName("ENVIO_ID");
            entity.Property(e => e.IdBussiness).HasMaxLength(450);
            entity.Property(e => e.Nombre).HasMaxLength(64);
            entity.Property(e => e.Pais).HasColumnName("PAIS");
            entity.Property(e => e.Pvp)
                .HasColumnType("decimal(11, 2)")
                .HasColumnName("PVP");
            entity.Property(e => e.Started).HasColumnType("datetime");
            entity.Property(e => e.UrlImgMain).HasMaxLength(256);

            entity.HasOne(d => d.Envio).WithMany(p => p.PedidosCs)
                .HasForeignKey(d => d.EnvioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PedidosC__ENVIO___51300E55");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.PedidosCs)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Categoria_Pedido");

            entity.HasOne(d => d.User).WithMany(p => p.PedidosCs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserId_Pedido");

            entity.HasOne(d => d.IdNavigation).WithMany(p => p.PedidosCs)
                .HasForeignKey(d => new { d.IdBussiness, d.IdProducto })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Producto_Pedido");
        });

        modelBuilder.Entity<ProdCat>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ProductId, e.CategoriaId }).HasName("PK__ProdCat__383B53E16FCCBAB0");

            entity.ToTable("ProdCat");

            entity.Property(e => e.Ok).HasColumnName("ok");

            entity.HasOne(d => d.Categoria).WithMany(p => p.ProdCats)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_cat_ProdCat");

            entity.HasOne(d => d.ProductosB).WithMany(p => p.ProdCats)
                .HasForeignKey(d => new { d.UserId, d.ProductId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_prod_ProdCat");
        });

        modelBuilder.Entity<ProductosB>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.Id }).HasName("PK__Producto__74A9828C51514AAD");

            entity.ToTable("ProductosB");

            entity.Property(e => e.AceeptD).HasColumnType("datetime");
            entity.Property(e => e.Nombre).HasMaxLength(64);
            entity.Property(e => e.Pvp)
                .HasColumnType("decimal(11, 2)")
                .HasColumnName("PVP");
            entity.Property(e => e.Started).HasColumnType("datetime");
            entity.Property(e => e.UrlImgMain).HasMaxLength(256);

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.ProductosBs)
                .HasForeignKey(d => d.IdCategoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Categoria_Producto");

            entity.HasOne(d => d.User).WithMany(p => p.ProductosBs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserId_Producto");
        });

        modelBuilder.Entity<Quest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Quest__3214EC07ADF3DF77");

            entity.ToTable("Quest");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdQuestionNavigation).WithMany(p => p.InverseIdQuestionNavigation)
                .HasForeignKey(d => d.IdQuestion)
                .HasConstraintName("FK_Quest_IdQuestion");

            entity.HasOne(d => d.IdTemaNavigation).WithMany(p => p.Quests)
                .HasForeignKey(d => d.IdTema)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Quest_Tema");
        });

        modelBuilder.Entity<UserQuestSelect>(entity =>
        {
            entity.HasKey(e => new { e.IdUser, e.IdQuestion, e.IdSelect }).HasName("PK__tmp_ms_x__D5CA032FE51D0FD7");

            entity.ToTable("UserQuestSelect");

            entity.Property(e => e.Time).HasColumnType("datetime");

            entity.HasOne(d => d.IdQuestionNavigation).WithMany(p => p.UserQuestSelectIdQuestionNavigations)
                .HasForeignKey(d => d.IdQuestion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserQuest_IdQuestion");

            entity.HasOne(d => d.IdSelectNavigation).WithMany(p => p.UserQuestSelectIdSelectNavigations)
                .HasForeignKey(d => d.IdSelect)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserQuest_IdSelect");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserQuestSelects)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserQuest_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
