using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Dtos
{
    public record DataSetDto
    {
        public string CanalDesignacao { get; set; }
        public string SpotDesignacao { get; set; }
        public string Horas { get; set; }
        public string ClienteDesignaca { get; set; }
        public DateTime? DataPassagemConfrimacao { get; set; }
        public TimeSpan? Duracao { get; set; }
        public string Dias { get; set; }
        public string DuracaoSpot { get; set; }
        public string UserName {get; set; }
        public string Periodo {get; set; }
        public string PrintDay { get; set; }
        public int? Estado { get; set; }
    }
}
