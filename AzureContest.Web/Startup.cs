using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AzureContest.Web.Startup))]
namespace AzureContest.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
