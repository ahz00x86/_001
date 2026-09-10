namespace seackers.Models.InApp
{
    [Serializable]
    public class FormationViewModel
    {
        public List<TemaS> Temas { get; set; }
    }

    [Serializable]
    public class TemaS
    {        
        public Guid Id { get; set; }
                
        public Guid? IdTematica { get; set; }
        public Guid? IdTema { get; set; }

        public Guid? IdPadre { get; set; }

        public string Titulo { get; set; }
        public string Contendio { get; set; }

        public List<TemaS> Secciones { get; set; }

        public Int32 Nivel { get; set; }

        public Boolean? Publico { get; set; }

        public TemaS SuperT { get; set; }

    }
}
