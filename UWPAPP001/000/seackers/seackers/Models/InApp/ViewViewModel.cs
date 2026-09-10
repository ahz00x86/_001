namespace seackers.Models.InApp
{
    public class ViewViewModel
    {
        public ViewSeackerCenter Center { get; set; } 
    }

    public class ViewSeackerCenter
    {
        public String Id { get; set; }
        public String Name { get; set; }
        public String Logo { get; set; }
        public String Head { get; set; }
        public String Phone { get; set; }
        public String WebSite { get; set; }
        public String Latitud { get; set; }
        public String Longitud { get; set; }
        public Int64 Likes { get; set; }
        public Boolean? MyLike { get; set; }

    }

}
