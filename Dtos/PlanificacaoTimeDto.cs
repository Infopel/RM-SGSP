using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Dtos
{
    public record PlanificacaoTimeDto
    {
        public DateTime? DateProcesso { get; set; }
        public TimeSpan? JanelaOpen { get; set; }
        public TimeSpan? JanelaEnd { get; set; }

    }
}
