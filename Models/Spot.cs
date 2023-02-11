using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace SGSP.Models
{
    public partial class Spot
    {
        public Spot()
        {
            InverseIdSpotPrincipalNavigation = new HashSet<Spot>();
            SpotPlanificacaos = new HashSet<SpotPlanificacao>();
        }

        public int Id { get; set; }
        [Display(Name = "Designação")]
        public string Designacao { get; set; }
        [Display(Name = "Código")]
        public string Code { get; set; }
        public int? IdCliente { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public TimeSpan? Duracao { get; set; }
        public int? IdCanal { get; set; }
        public int? Periodicidade { get; set; }
        public bool? IsActive { get; set; }
        public int? IdSpotPrincipal { get; set; }
        public int? IdTipoSpot { get; set; }
        public DateTime? DataCreate { get; set; }
        public int? Estado { get; set; }

        public virtual Canal IdCanalNavigation { get; set; }
        public virtual Cliente IdClienteNavigation { get; set; }
        public virtual Spot IdSpotPrincipalNavigation { get; set; }
        public virtual TipoSpot IdTipoSpotNavigation { get; set; }
        public virtual ICollection<Spot> InverseIdSpotPrincipalNavigation { get; set; }
        public virtual ICollection<SpotPlanificacao> SpotPlanificacaos { get; set; }
    }
}
