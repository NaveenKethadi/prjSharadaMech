using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(prjSharadaMech.Startup))]
namespace prjSharadaMech
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
