using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SocialMediaLogin.Web.Startup))]
namespace SocialMediaLogin.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
