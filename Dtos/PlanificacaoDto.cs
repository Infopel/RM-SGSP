using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Dtos
{
    public record PlanificacaoDto
    {
        public int Id { get; set; }
        public string Designacao { get; set; }
        public string Cliente { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public DateTime? DataCreateProcesso { get; set; }
        public DateTime? DataPassagem { get; set; }
        public DateTime? DataPassagemConfrimacao { get; set; }
        public DateTime? DataPlanificacao { get; set; }
        public DateTime? DataSkip { get; set; }
        public string SkipMotivo { get; set; }
        public TimeSpan? Duracao { get; set; }
        public string? CanalDesignacao { get; set; }
        public Boolean? IsReagendamento { get; set; }
        public int? IdTipo { get; set; }
        public string TipoProcesso { get; set; }
        public string Code { get; set; }
        public DateTime? Prioridade { get; set; }
        public int? IdPlanificacaoPrincipal { get; set; }
        public int? Estado { get; set; }
        public string UserCoordenador { get; set; }
        public string UserLocutor { get; set; }
        public string CoordenadorParecer { get; set; }
        public int? IdCanal { get; set; }
        public string DesignacaoCanal { get; set; }
        public string EstadoHTML { get; set; }
        public string DesignacaoProcesso { get; set; }
        public DateTime? LastDate { get; set; }
    }
}
