using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using SGSP.Data;
using SGSP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Repositories
{
    public class JanelasRepository: IJanelasRepository
    {
        private readonly dbSpotsContext _context;
        public JanelasRepository(dbSpotsContext context)
        {
            _context = context;
        }

        public async Task<Janela> Create(Janela janela)
        {
            _context.Janelas.Add(janela);
            await _context.SaveChangesAsync();

            return janela;
        }

        public IEnumerable<Janela> GetAllBy(int idCanal)
        {
            var janelas = _context.Janelas
                .Where(j => j.IsActive == true && j.IdCanal == idCanal && j.IdCanalNavigation.IsActive == true)
                .ToList();

            return janelas;
        }

        public async Task<Janela> GetBy(int idJanela)
        {
            return await _context.Janelas
                .Where(j => j.IsActive == true && j.Id == idJanela)
                .Include(j => j.IdCanalNavigation)
                .FirstOrDefaultAsync();
        }

        public IEnumerable<Janela> Get()
        {
            var janelas = _context.Janelas
                .Include(j => j.IdCanalNavigation)
                .Where(j => j.IsActive == true && j.IdCanalNavigation.IsActive == true)
                .ToList();
            return janelas;
        }

        public async Task Update(Janela janela)
        {
            _context.Entry(janela).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateJanela(int idJanela, Janela janela)
        {
            var j = await _context.Janelas.FindAsync(idJanela);
            if(j != null)
            {
                j.Designacao = janela.Designacao;
                j.DiasSemana = janela.DiasSemana;
                j.HoraFim = janela.HoraFim;
                j.HoraInicio = janela.HoraInicio;

                await _context.SaveChangesAsync();
            }
        }
    }
}
