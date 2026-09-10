using System;
using System.Collections.Generic;

namespace seackers.Models;

public partial class Quest
{
    public Guid Id { get; set; }

    public Guid IdTema { get; set; }

    public Guid? IdQuestion { get; set; }

    public string Text { get; set; } = null!;

    public bool? IsCorrect { get; set; }

    public bool? Public { get; set; }

    public virtual Quest? IdQuestionNavigation { get; set; }

    public virtual Contenido IdTemaNavigation { get; set; } = null!;

    public virtual ICollection<Quest> InverseIdQuestionNavigation { get; set; } = new List<Quest>();

    public virtual ICollection<UserQuestSelect> UserQuestSelectIdQuestionNavigations { get; set; } = new List<UserQuestSelect>();

    public virtual ICollection<UserQuestSelect> UserQuestSelectIdSelectNavigations { get; set; } = new List<UserQuestSelect>();
}
