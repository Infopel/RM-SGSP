using System;
using System.Collections.Generic;

#nullable disable

namespace SGSP.Models
{
    public partial class Canal
    {
        public Canal()
        {
            Janelas = new HashSet<Janela>();
            SpotPlanificacaos = new HashSet<SpotPlanificacao>();
            Spots = new HashSet<Spot>();
            Usuarios = new HashSet<Usuario>();
        }

        public int Id { get; set; }
        public string Designacao { get; set; }
        public DateTime? DataCreate { get; set; }
        public string Code { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<Janela> Janelas { get; set; }
        public virtual ICollection<SpotPlanificacao> SpotPlanificacaos { get; set; }
        public virtual ICollection<Spot> Spots { get; set; }
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}
