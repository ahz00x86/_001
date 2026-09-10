using System;
using System.Collections.Generic;

namespace seackers.Models;

public partial class Contenido
{
    public Guid Id { get; set; }

    public Guid? IdTematica { get; set; }

    public Guid? IdTema { get; set; }

    public Guid? IdPadre { get; set; }

    public bool? Publico { get; set; }

    public bool? Borrado { get; set; }

    public string Titulo { get; set; } = null!;

    public string Contenido1 { get; set; } = null!;

    public virtual Contenido? IdPadreNavigation { get; set; }

    public virtual Contenido? IdTemaNavigation { get; set; }

    public virtual Contenido? IdTematicaNavigation { get; set; }

    public virtual ICollection<Contenido> InverseIdPadreNavigation { get; set; } = new List<Contenido>();

    public virtual ICollection<Contenido> InverseIdTemaNavigation { get; set; } = new List<Contenido>();

    public virtual ICollection<Contenido> InverseIdTematicaNavigation { get; set; } = new List<Contenido>();

    public virtual ICollection<Quest> Quests { get; set; } = new List<Quest>();
}
