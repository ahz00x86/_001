using System;
using System.Collections.Generic;

namespace seackers.Models;

public partial class Categoria
{
    public Guid Id { get; set; }

    public Guid? IdTematica { get; set; }

    public Guid? IdTema { get; set; }

    public Guid? IdSeccion { get; set; }

    public Guid? IdPadre { get; set; }

    public bool? Publico { get; set; }

    public bool? Borrado { get; set; }

    public string Titulo { get; set; } = null!;

    public string Contenido { get; set; } = null!;

    public virtual Categoria? IdPadreNavigation { get; set; }

    public virtual Categoria? IdSeccionNavigation { get; set; }

    public virtual Categoria? IdTemaNavigation { get; set; }

    public virtual Categoria? IdTematicaNavigation { get; set; }

    public virtual ICollection<Categoria> InverseIdPadreNavigation { get; set; } = new List<Categoria>();

    public virtual ICollection<Categoria> InverseIdSeccionNavigation { get; set; } = new List<Categoria>();

    public virtual ICollection<Categoria> InverseIdTemaNavigation { get; set; } = new List<Categoria>();

    public virtual ICollection<Categoria> InverseIdTematicaNavigation { get; set; } = new List<Categoria>();

    public virtual ICollection<PedidosC> PedidosCs { get; set; } = new List<PedidosC>();

    public virtual ICollection<ProdCat> ProdCats { get; set; } = new List<ProdCat>();

    public virtual ICollection<ProductosB> ProductosBs { get; set; } = new List<ProductosB>();
}
