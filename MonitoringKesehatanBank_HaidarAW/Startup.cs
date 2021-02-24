using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MonitoringKesehatanBank_HaidarAW.Startup))]
namespace MonitoringKesehatanBank_HaidarAW
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
