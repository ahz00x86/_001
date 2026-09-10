using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(textoamp3.Startup))]
namespace textoamp3
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
