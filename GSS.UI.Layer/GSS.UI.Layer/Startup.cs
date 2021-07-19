using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GSS.UI.Layer.Startup))]
namespace GSS.UI.Layer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
