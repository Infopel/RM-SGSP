using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Models
{
    public class ListUserViewModel
    {
        public IList<IdentityUser> users { get; set; }
        public IList<string> roles { get; set; }
    }
}
