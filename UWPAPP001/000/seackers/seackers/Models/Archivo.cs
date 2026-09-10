using System;
using System.Collections.Generic;

namespace seackers.Models;

public partial class Archivo
{
    public Guid Id { get; set; }

    public string Titulo { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public string Url { get; set; } = null!;
}
