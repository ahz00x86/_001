using System;
using System.Collections.Generic;

namespace seackers.Models;

public partial class ProdCat
{
    public string UserId { get; set; } = null!;

    public Guid ProductId { get; set; }

    public Guid CategoriaId { get; set; }

    public bool? Ok { get; set; }

    public virtual Categoria Categoria { get; set; } = null!;

    public virtual ProductosB ProductosB { get; set; } = null!;
}
