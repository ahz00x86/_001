using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(universalsemiotics.Startup))]
namespace universalsemiotics
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
