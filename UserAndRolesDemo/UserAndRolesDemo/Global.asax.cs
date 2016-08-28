using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using UserAndRolesDemo.Models;
using Microsoft.Owin.Security;
using UserAndRolesDemo.Controllers;

namespace UserAndRolesDemo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            var context = new ApplicationDbContext();
            context.Database.Initialize(true);

            this.SeedUsersAndRoles(context);
        }

        private void SeedUsersAndRoles(ApplicationDbContext context)
        {
            if (context.Roles.Count() == 0)
            {
                Role adminRole = new Role() { Name = "Admin" };
                Role managerRole = new Role() { Name = "Manager" };
                Role editorRole = new Role() { Name = "Editor" };
                Role devRole = new Role() { Name = "Developer" };
                Role testerRole = new Role() { Name = "Tester" };

                context.Roles.Add(adminRole);
                context.Roles.Add(managerRole);
                context.Roles.Add(editorRole);
                context.Roles.Add(devRole);
                context.Roles.Add(testerRole);

                context.SaveChanges();
                
                var userStore = new UserStore<User, Role, string, IdentityUserLogin, UserRoles, IdentityUserClaim>(context);
                var userManager = new UserManager<User, string>(userStore);

                User admin = new Models.User() { Age = DateTime.Now.Second, Email = "admin@test.com", UserName = "Administrator" };
                admin.Roles.Add(new UserRoles() { Role = adminRole, User = admin });
                admin.Roles.Add(new UserRoles() { Role = managerRole, User = admin });

                string password = "123456";
                var res = userManager.Create(admin, password);
            }
        }

    }
}
