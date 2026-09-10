using System;
using System.Collections.Generic;

namespace seackers.Models;

public partial class PedidosC
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = null!;

    public string IdBussiness { get; set; } = null!;

    public Guid IdProducto { get; set; }

    public Guid IdCategoria { get; set; }

    public DateTime Started { get; set; }

    public bool Accepted { get; set; }

    public DateTime? AceeptD { get; set; }

    public string Nombre { get; set; } = null!;

    public string Contenido { get; set; } = null!;

    public decimal? Pvp { get; set; }

    public int? Cantidad { get; set; }

    public string? UrlImgMain { get; set; }

    public string Cliente { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public string Cp { get; set; } = null!;

    public Guid Pais { get; set; }

    public Guid EnvioId { get; set; }

    public virtual Envio Envio { get; set; } = null!;

    public virtual Categoria IdCategoriaNavigation { get; set; } = null!;

    public virtual ProductosB IdNavigation { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
