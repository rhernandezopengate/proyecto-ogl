using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebAppOrdenesCerradas.Startup))]
namespace WebAppOrdenesCerradas
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
