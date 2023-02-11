using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Models
{
    public class ReportViewModel
    {
        public int? IdCanal { get; set; }
        public int? TipoSpot { get; set; }
        public int? IdCliente { get; set; }
        public DateTime? DataI { get; set; }
        public DateTime? DataF { get; set; }
        public string ReportName { get; set; }
    }
}
