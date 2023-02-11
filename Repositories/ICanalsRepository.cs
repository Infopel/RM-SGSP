using SGSP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Repositories
{
    public interface ICanalsRepository
    {
        IEnumerable<Canal> Get();
        Task<Canal> Create(Canal canal);
        Task Update(Canal canal);
        Task UpdateCanal(int idCanal, Canal canal);
        Task<Canal> GetBy(string code);
    }
}
