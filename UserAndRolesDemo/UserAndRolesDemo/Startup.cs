using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UserAndRolesDemo.Startup))]
namespace UserAndRolesDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
