using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Models
{
    public partial class Filter
    {
        public int IdCliente { get; set; }
        public int IdSpot { get; set; }
        public DateTime? DataI { get; set; }
        public DateTime? DataF { get; set; }
    }
}
