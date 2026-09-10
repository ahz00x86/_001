using System;
using System.Collections.Generic;

namespace seackers.Models;

public partial class AspNetUser
{
    public string Id { get; set; } = null!;

    public string? UserName { get; set; }

    public string? NormalizedUserName { get; set; }

    public string? Email { get; set; }

    public string? NormalizedEmail { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; } = new List<AspNetUserClaim>();

    public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; } = new List<AspNetUserLogin>();

    public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; set; } = new List<AspNetUserToken>();

    public virtual ICollection<CenterLike> CenterLikeCenters { get; set; } = new List<CenterLike>();

    public virtual ICollection<CenterLike> CenterLikeUsers { get; set; } = new List<CenterLike>();

    public virtual ICollection<CenterUser> CenterUserCenters { get; set; } = new List<CenterUser>();

    public virtual ICollection<CenterUser> CenterUserUsers { get; set; } = new List<CenterUser>();

    public virtual DataUser? DataUser { get; set; }

    public virtual Direccione? Direccione { get; set; }

    public virtual ICollection<PedidosC> PedidosCs { get; set; } = new List<PedidosC>();

    public virtual ICollection<ProductosB> ProductosBs { get; set; } = new List<ProductosB>();

    public virtual ICollection<UserQuestSelect> UserQuestSelects { get; set; } = new List<UserQuestSelect>();

    public virtual ICollection<AspNetRole> Roles { get; set; } = new List<AspNetRole>();
}
