using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Dtos
{
    public record JanelasDto
    {
        public int Id { get; set; }
        public string Designacao { get; set; }
        public TimeSpan? HoraInicio { get; set; }
        public TimeSpan? HoraFim { get; set; }
        public string DiasSemana { get; set; }
        public string Canal { get; set; }
        public int? IdCanal { get; set; }
        public string CodeCanal { get; set; }
        public bool? IsActive { get; set; }
        public string Action { get; set; }

    }
}
