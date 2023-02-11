using System;
using System.Collections.Generic;

#nullable disable

namespace SGSP.Models
{
    public partial class TipoSpot
    {
        public TipoSpot()
        {
            Spots = new HashSet<Spot>();
        }

        public int Id { get; set; }
        public string Designacao { get; set; }

        public virtual ICollection<Spot> Spots { get; set; }
    }
}
