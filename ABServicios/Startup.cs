using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ABServicios.Startup))]
namespace ABServicios
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
