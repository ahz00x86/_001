namespace seackers.Models.InApp
{    public class UsersViewModel
    {
        public List<UsuarioSeacker> UsuariosSeacker { get; set; }
                
        public String NewUserEmail { get; set; }
        public Boolean Notificar { get; set; }
    }

    public class UsuarioSeacker
    {
        public String Id { get; set; }
        public String? Name { get; set; }
        public String? Email { get; set; }
        public String Phone { get; set; }
        public String UrlLogo { get; set; }
        public Boolean Accepted { get; set; }
        public Boolean? Lock { get; set; }
        // Agrega otras propiedades según tus necesidades
    }
}
