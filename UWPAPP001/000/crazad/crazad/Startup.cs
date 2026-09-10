using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(crazad.Startup))]
namespace crazad
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
