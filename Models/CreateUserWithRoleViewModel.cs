using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Models
{
    public class CreateUserWithRoleViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required] 
        public string Nome { get; set; }
        [Required] 
        public string Apelido { get; set; }
        [Required]
        public string idRole { get; set; }

        [Display(Name = "Canal")]
        public string [] IdCanal { get; set; }
        public int? IdCliente { get; set; }
    }
}
