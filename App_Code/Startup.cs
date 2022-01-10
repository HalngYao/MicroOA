using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MicroOA.Startup))]
namespace MicroOA
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
