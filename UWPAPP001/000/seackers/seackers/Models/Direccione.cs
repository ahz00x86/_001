using System;
using System.Collections.Generic;

namespace seackers.Models;

public partial class Direccione
{
    public string UserId { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public string Cp { get; set; } = null!;

    public Guid Pais { get; set; }

    public virtual Paise PaisNavigation { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
