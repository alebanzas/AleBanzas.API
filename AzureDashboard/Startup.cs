using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AzureDashboard.Startup))]
namespace AzureDashboard
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
