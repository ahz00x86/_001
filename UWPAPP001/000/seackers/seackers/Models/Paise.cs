using System;
using System.Collections.Generic;

namespace seackers.Models;

public partial class Paise
{
    public Guid Id { get; set; }

    public int Code { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Direccione> Direcciones { get; set; } = new List<Direccione>();
}
