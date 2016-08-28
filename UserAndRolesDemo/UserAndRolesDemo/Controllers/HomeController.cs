using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using UserAndRolesDemo.Models;

namespace UserAndRolesDemo.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {




            User currentUser = new ApplicationDbContext().Users.Find(this.User.Identity.GetUserId());
            var model = new UserViewModel()
            {
                Age = currentUser.Age,
                UserName = currentUser.UserName,
            };

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [ChildActionOnly]
        public ActionResult GetRoles()
        {
            IEnumerable<string> roles = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>()).Roles.Select(c => c.Name).ToList();

            return this.PartialView("Roles", roles);
        }

        [ChildActionOnly]
        public ActionResult GetUsersAndTheirRoles()
        {
            ICollection<UserViewModel> model = new List<UserViewModel>();
            
            // Many queries to database.
            /*
            var userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
            IEnumerable<IdentityUser> dbUsers = userManager.Users.ToList();

            foreach (var user in dbUsers)
            {
                model.Add(new UserViewModel()
                {
                    UserName = user.UserName,
                    Roles = userManager.GetRoles(user.Id)
                });
            }
            */

            // a single query with same data.
            model = new ApplicationDbContext().Users.Include(u => u.Roles).Select(u => new UserViewModel()
            {
                Age = u.Age,
                UserName = u.UserName,
                Roles = u.Roles.Select(r => r.Role.Name)
            })
            .ToList();

            return this.PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult GetRolesAndUsersIn()
        {
            ICollection<RoleViewModel> model = new List<RoleViewModel>();
            model = new ApplicationDbContext().Roles.Include(r => r.Users).Select(r => new RoleViewModel()
            {
                Name = r.Name,
                UsersIn = r.Users.Select(u => u.User.UserName)
            })
            .ToList();

            return this.PartialView(model);
        }
        
        [HttpPost]
        public ActionResult AddCurrentUserToRole(string roleName)
        {
            // var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>());
            // var userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
            // userManager.AddToRole(this.User.Identity.GetUserId(), roleName);

            var context = new ApplicationDbContext();
            User userToUpdate = context.Users.Find(this.User.Identity.GetUserId());
            Role roleToAdd = context.Roles.FirstOrDefault(r => r.Name == roleName);

            if (roleToAdd != null && userToUpdate != null && userToUpdate.Roles.All(c => c.Role.Name != roleName))
            {
                userToUpdate.Roles.Add(new UserRoles() { Role = roleToAdd, User = userToUpdate });
                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}