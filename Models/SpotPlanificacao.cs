using System;
using System.Collections.Generic;

#nullable disable

namespace SGSP.Models
{
    public partial class SpotPlanificacao
    {
        public SpotPlanificacao()
        {
            InverseIdSpotPlanificacaoPrincipalNavigation = new HashSet<SpotPlanificacao>();
        }

        public int Id { get; set; }
        public int? IdSpot { get; set; }
        public int? IdJanela { get; set; }
        public DateTime? DataPlanificacao { get; set; }
        public DateTime? DataPassagem { get; set; }
        public DateTime? DataPassagemConfirmacao { get; set; }
        public DateTime? DataSkip { get; set; }
        public int? Estado { get; set; }
        public int? IdCanal { get; set; }
        public string SkipMotivo { get; set; }
        public string ParecerCoordenador { get; set; }
        public string UserLocutor { get; set; }
        public string UserCoordenador { get; set; }
        public DateTime? Prioridade { get; set; }
        public bool? IsReagendamento { get; set; }
        public int? IdSpotPlanificacaoPrincipal { get; set; }

        public virtual Canal IdCanalNavigation { get; set; }
        public virtual Janela IdJanelaNavigation { get; set; }
        public virtual Spot IdSpotNavigation { get; set; }
        public virtual SpotPlanificacao IdSpotPlanificacaoPrincipalNavigation { get; set; }
        public virtual ICollection<SpotPlanificacao> InverseIdSpotPlanificacaoPrincipalNavigation { get; set; }
    }
}
