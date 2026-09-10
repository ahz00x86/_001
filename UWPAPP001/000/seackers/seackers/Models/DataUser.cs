using System;
using System.Collections.Generic;

namespace seackers.Models;

public partial class DataUser
{
    public string UserId { get; set; } = null!;

    public string? Name { get; set; }

    public string? ImgLogoUrl { get; set; }

    public string? ImgHeadUrl { get; set; }

    public decimal? Latitud { get; set; }

    public decimal? Longitud { get; set; }

    public string? Phone { get; set; }

    public string? Url { get; set; }

    public bool? Lock { get; set; }

    public virtual AspNetUser User { get; set; } = null!;
}
