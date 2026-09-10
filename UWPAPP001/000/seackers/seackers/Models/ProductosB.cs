using System;
using System.Collections.Generic;

namespace seackers.Models;

public partial class ProductosB
{
    public string UserId { get; set; } = null!;

    public Guid Id { get; set; }

    public Guid IdCategoria { get; set; }

    public DateTime Started { get; set; }

    public bool Accepted { get; set; }

    public DateTime? AceeptD { get; set; }

    public string Nombre { get; set; } = null!;

    public string Contenido { get; set; } = null!;

    public decimal? Pvp { get; set; }

    public int? Stock { get; set; }

    public string? UrlImgMain { get; set; }

    public virtual Categoria IdCategoriaNavigation { get; set; } = null!;

    public virtual ICollection<PedidosC> PedidosCs { get; set; } = new List<PedidosC>();

    public virtual ICollection<ProdCat> ProdCats { get; set; } = new List<ProdCat>();

    public virtual AspNetUser User { get; set; } = null!;
}
