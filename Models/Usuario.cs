using System;
using System.Collections.Generic;

#nullable disable

namespace SGSP.Models
{
    public partial class Usuario
    {
        public int Id { get; set; }
        public string IdAspNetUser { get; set; }
        public int? IdCanal { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? RemovedOn { get; set; }
        public int? IdJanela { get; set; }
        public int? IdCliente { get; set; }
        public string Role { get; set; }

        public virtual Canal IdCanalNavigation { get; set; }
        public virtual Janela IdJanelaNavigation { get; set; }
        public virtual Cliente IdClienteNavigation { get; set; }
    }
}
