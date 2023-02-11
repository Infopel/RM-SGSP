using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Dtos
{
    public record ClientesDto
    {
        public int Id { get; set; }
        public string Designacao { get; set; }
        public string Nuit { get; set; }
        public string Telefone { get; set; }
        public string EMail { get; set; }
        public DateTime? DataCreate { get; set; }
        public int NSpots { get; set; }
        public string Action { get; set; }


    }
}
