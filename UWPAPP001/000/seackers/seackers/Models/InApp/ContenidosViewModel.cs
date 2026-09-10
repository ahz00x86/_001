namespace seackers.Models.InApp
{
    public class ContenidosViewModel
    {
        public String MainContent { get; set; }
        public List<Contenido> ContenidosUsuario { get; set; }

        public Boolean HaveQuest { get; set; }

        public Boolean HaveResponse { get; set; }

    }

    public class Contenido
    {
        public Guid Id { get; set; }

        public Guid? IdTema { get; set; }

        public Guid? IdPadre { get; set; }

        public string Title { get; set; }

        public List<Contenido> Secciones { get; set; }

        public Int32 Nivel { get; set; }

    }
}
