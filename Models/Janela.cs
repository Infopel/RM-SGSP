using System;
using System.Collections.Generic;

#nullable disable

namespace SGSP.Models
{
    public partial class Janela
    {
        public Janela()
        {
            SpotPlanificacaos = new HashSet<SpotPlanificacao>();
            Usuarios = new HashSet<Usuario>();
        }

        public int Id { get; set; }
        public string Designacao { get; set; }
        public TimeSpan? HoraInicio { get; set; }
        public TimeSpan? HoraFim { get; set; }
        public string DiasSemana { get; set; }
        public int? IdCanal { get; set; }
        public bool? IsActive { get; set; }

        public virtual Canal IdCanalNavigation { get; set; }
        public virtual ICollection<SpotPlanificacao> SpotPlanificacaos { get; set; }
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}
