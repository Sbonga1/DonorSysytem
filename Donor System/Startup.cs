using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Donor_System.Startup))]
namespace Donor_System
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
