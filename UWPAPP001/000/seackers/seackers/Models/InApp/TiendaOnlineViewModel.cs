namespace seackers.Models.InApp
{
    public class TiendaOnlineViewModel
    {
        public List<Categoria> Categorias { get; set; }

        public List<Producto> ProductosINIT { get; set; }

        public List<Producto> MINEProductos { get; set; }

    }


    public class Categoria
    {
        public Categoria SuperC { get; set; }
        public Guid? IdPadre { get; set; }
        public Guid? IdTema { get; set; }

        public Guid? IdTematica { get; set; }
        public Guid Id { get; set; }

        public String Nombre { get; set; }

        public Boolean Selecionada { get; set; }

        public List<Categoria> CategoriasHIJAS { get; set; }

        public Int32 Nivel { get; set; }

        public Boolean? Publico { get; set; }
        public Boolean Checked { get; set; }
    }

    public class Producto
    {
        public Guid Id { get; set; }

        public String IdB { get; set; }

        public Guid? IdCategoria { get; set; }

        public String Nombre { get; set; }

        public String Contenido { get; set; }

        public String ImageUrl { get; set; }

        public Decimal? Precio { get; set; }

        public Int32 Stock { get; set; }

        public Int32 Cantidad { get; set; }

        public List<Categoria> CategoriasProd { get; set; }

    }

}
