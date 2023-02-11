using Microsoft.EntityFrameworkCore;
using SGSP.Data;
using SGSP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Repositories
{
    public class CanalsRepository: ICanalsRepository
    {
        private readonly dbSpotsContext _context;
        public CanalsRepository(dbSpotsContext context)
        {
            _context = context;
        }

        public async Task<Canal> Create(Canal canal)
        {
            _context.Canals.Add(canal);
            await _context.SaveChangesAsync();

            return canal;
        }
        public IEnumerable<Canal> Get()
        {
            var canal = _context.Canals
                .Where(c => c.IsActive == true)
                .Include(c => c.Janelas)
                .ToList();

            return canal;
        }

        public async Task<Canal> GetBy(string code)
        {
            return await _context.Canals.Where(c => c.Designacao.ToLower() == code.ToLower() && c.IsActive == true).FirstOrDefaultAsync();
        }

        public async Task Update(Canal canal)
        {
            _context.Entry(canal).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCanal(int idCanal, Canal canal)
        {
            var c = await _context.Canals.FindAsync(idCanal);
            if(c != null)
            {
                c.Designacao = canal.Designacao;

                await _context.SaveChangesAsync();
            }
        }
    }
}
