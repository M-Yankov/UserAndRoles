using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UserAndRolesDemo.Models
{
    public class RoleViewModel
    {
        public string Name { get; set; }

        public IEnumerable<string> UsersIn { get; set; }
    }
}