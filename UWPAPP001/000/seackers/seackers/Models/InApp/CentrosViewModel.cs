namespace seackers.Models.InApp
{
    public class CentrosViewModel
    {
        public List<CentroSeacker> CentrosSeacker { get; set; }

        public String NewCenterName { get; set; }
        public String NewCenterEmail { get; set; }
        public Boolean Notificar { get; set; }
    }

    public class CentroSeacker
    {
        public String Id { get; set; }
        public String? Name { get; set; }
        public String? Phone { get; set; }
        public String? Email { get; set; }
        public String UrlLogo { get; set; }
        public String Url { get; set; }
        public Boolean? Lock { get; set; }
        // Agrega otras propiedades según tus necesidades
    }
}
