﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Models
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            users = new List<string>();
        }

        public string Id { get; set; }
        [Required(ErrorMessage = "Role Name is required")]
        public string RoleName { get; set; }
        public List<string> users { get; set; }
    }
}
