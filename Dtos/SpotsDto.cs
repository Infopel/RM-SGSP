using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Dtos
{
    public record SpotsDto
    {
        public int Id { get; set; }
        public string Designacao { get; set; }
        public TimeSpan? Duracao { get; set; }
        public string Code { get; set; }
        public string Cliente { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string Periodo { get; set; }
        public int? Periodicidade { get; set; }
        public string Estado { get; set; }
        public string Action { get; set; }
        public int? IdCanal { get; set; }
        public int? idTipoSpot { get; set; }
    }
}
