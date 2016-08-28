using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UserAndRolesDemo.Models
{
    public class UserViewModel
    {
        public int Age { get; set; }

        public string RoleName { get; set; }

        public string UserName { get; set; }

        /// <summary>
        /// Role names for current user.
        /// </summary>
        /// <value>
        /// The roles.
        /// </value>
        public IEnumerable<string> Roles { get; set; }
    }
}