namespace seackers.Models.InApp
{
    [Serializable]
    public class ContentViewModel
    {
        public List<TemaS_AV> Contenidos { get; set; }
    }

    [Serializable]
    public class TemaS_AV
    {
        public Guid Id { get; set; }

        public Guid? IdTema { get; set; }

        public Guid? IdPadre { get; set; }

        public string Titulo { get; set; }
        public string Contendio { get; set; }

        public List<TemaS_AV> Secciones { get; set; }

        public Int32 Nivel { get; set; }

    }

}
