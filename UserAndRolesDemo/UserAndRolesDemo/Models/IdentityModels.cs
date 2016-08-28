using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace UserAndRolesDemo.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class User : IdentityUser<string, IdentityUserLogin, UserRoles, IdentityUserClaim>
    {
        public User()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User, string> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public int Age { get; set; }

        public string Description { get; set; }
    }

    public class UserRoles : IdentityUserRole
    {
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }


    public class Role : IdentityRole<string, UserRoles>
    {
        public Role()
        {
            this.Id = Guid.NewGuid().ToString();
        }
    }

    public class ApplicationDbContext : IdentityDbContext<User, Role, string, IdentityUserLogin, UserRoles, IdentityUserClaim>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}