namespace seackers.Models.InApp
{
    public class IndexViewModel
    {
        public List<SeackerIndexCenter> Centers { get; set; } 
    }

    public class SeackerIndexCenter
    {
        public String Name { get; set; }
        public String Logo { get; set; }
        public String Phone { get; set; }
        public String WebSite { get; set; }
        public String Latitud { get; set; }
        public String Longitud { get; set; }
        public Int64 Likes { get; set; }
    }
}
