using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Dtos
{
    public record CanalDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Guid IdUserAsp { get; set; }
        public int [] NewIdCanais { get; set; }
        public int [] OldIdCanais { get; set; }
    }
}
