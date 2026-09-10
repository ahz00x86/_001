using System;
using System.Collections.Generic;

namespace seackers.Models;

public partial class CenterLike
{
    public string UserId { get; set; } = null!;

    public string CenterId { get; set; } = null!;

    public bool? Like { get; set; }

    public virtual AspNetUser Center { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
