using System;
using System.Collections.Generic;

namespace seackers.Models;

public partial class Envio
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Contenido { get; set; } = null!;

    public int Horas { get; set; }

    public decimal? Pvp { get; set; }

    public string? UrlImgMain { get; set; }

    public virtual ICollection<PedidosC> PedidosCs { get; set; } = new List<PedidosC>();
}
