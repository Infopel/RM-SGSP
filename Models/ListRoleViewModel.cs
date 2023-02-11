using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Models
{
    public class ListRoleViewModel
    {
        public IList<IdentityRole> roles { get; set; }
    }
}
