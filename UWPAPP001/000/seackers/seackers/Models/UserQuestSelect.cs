using System;
using System.Collections.Generic;

namespace seackers.Models;

public partial class UserQuestSelect
{
    public string IdUser { get; set; } = null!;

    public Guid IdQuestion { get; set; }

    public Guid IdSelect { get; set; }

    public DateTime Time { get; set; }

    public virtual Quest IdQuestionNavigation { get; set; } = null!;

    public virtual Quest IdSelectNavigation { get; set; } = null!;

    public virtual AspNetUser IdUserNavigation { get; set; } = null!;
}
