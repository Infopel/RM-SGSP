using System;
using System.Collections.Generic;

#nullable disable

namespace SGSP.Models
{
    public partial class Cliente
    {
        public Cliente()
        {
            InverseIdClientePrincipalNavigation = new HashSet<Cliente>();
            Spots = new HashSet<Spot>();
            Usuarios = new HashSet<Usuario>();
        }

        public int Id { get; set; }
        public string Designacao { get; set; }
        public string Nuit { get; set; }
        public string Telefone { get; set; }
        public string EMail { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? DataCreate { get; set; }
        public int? IdClientePrincipal { get; set; }

        public virtual Cliente IdClientePrincipalNavigation { get; set; }
        public virtual ICollection<Cliente> InverseIdClientePrincipalNavigation { get; set; }
        public virtual ICollection<Spot> Spots { get; set; }
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}
