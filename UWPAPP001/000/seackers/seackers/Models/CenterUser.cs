using System;
using System.Collections.Generic;

namespace seackers.Models;

public partial class CenterUser
{
    public string UserId { get; set; } = null!;

    public string CenterId { get; set; } = null!;

    public DateTime Started { get; set; }

    public Guid Key { get; set; }

    public bool? Accepted { get; set; }

    public DateTime? AceeptD { get; set; }

    public virtual AspNetUser Center { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
