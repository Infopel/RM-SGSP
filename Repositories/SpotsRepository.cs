using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SGSP.Data;
using SGSP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Repositories
{
    public class SpotsRepository : ISpotsRepository
    {
        private readonly dbSpotsContext _context;
        public SpotsRepository(dbSpotsContext context)
        {
            _context = context;
        }
        public async Task<Spot> Create(Spot spot)
        {
            _context.Spots.Add(spot);
            await _context.SaveChangesAsync();

            return spot;
        }

        public async Task Delete(int id)
        {
            var spotToDelete = await _context.Spots.FindAsync(id);
            _context.Spots.Remove(spotToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Spot>> Get()
        {
            return await _context.Spots.ToListAsync();
        }

        public async Task<Spot> Get(int? id)
        {
            return await _context.Spots.FindAsync(id);
        }

        public IEnumerable<Spot> GetAllSpots()
        {
            var spots = _context.Spots
                .Include(s => s.IdClienteNavigation)
                .Include(s => s.IdTipoSpotNavigation)
                .Include(s => s.SpotPlanificacaos)
                    .ThenInclude(spotP => spotP.IdJanelaNavigation)
                .Where(s => s.IsActive == true && s.IdTipoSpot == 1 && s.IdClienteNavigation.IsActive == true)
                .ToList();
            return spots;
        }
        public IEnumerable<Spot> GetAllSpotsBy(int idCanal)
        {
            var spots = _context.Spots
                .Include(s => s.IdClienteNavigation)
                .Include(s => s.IdTipoSpotNavigation)
                .Include(s => s.SpotPlanificacaos)
                    .ThenInclude(spotP => spotP.IdJanelaNavigation)
                .Where(s => s.IsActive == true && s.IdTipoSpot == 1 && s.IdClienteNavigation.IsActive == true && s.IdCanal == idCanal)
                .ToList();
            return spots;
        }
        
        public IEnumerable<Spot> GetAllAnuncios()
        {
            var anuncios = _context.Spots
                .Include(s => s.IdClienteNavigation)
                .Include(s => s.IdTipoSpotNavigation)
                .Include(s => s.SpotPlanificacaos)
                    .ThenInclude(spotP => spotP.IdJanelaNavigation)
                .Where(s => s.IsActive == true && s.IdTipoSpot == 2 && s.IdClienteNavigation.IsActive == true)
                .ToList();
            return anuncios;
        }

        public IEnumerable<Spot> GetAllAnunciosBy(int idCanal)
        {
            var anuncios = _context.Spots
                .Include(s => s.IdClienteNavigation)
                .Include(s => s.IdTipoSpotNavigation)
                .Include(s => s.SpotPlanificacaos)
                    .ThenInclude(spotP => spotP.IdJanelaNavigation)
                .Where(s => s.IsActive == true && s.IdTipoSpot == 2 && s.IdClienteNavigation.IsActive == true && s.IdCanal == idCanal)
                .ToList();
            return anuncios;
        }

        public async Task<Spot> GetSpotBy(string code)
        {
            var newCode = code.Replace("-", "/");
            var spot =  await _context.Spots
                .Include(s => s.IdCanalNavigation)
                .Include(s => s.IdClienteNavigation)
                .Include(s => s.IdCanalNavigation.Janelas)
                .Where(s => s.Code == newCode && s.IsActive == true && s.IdClienteNavigation.IsActive == true)
                .Include(s => s.SpotPlanificacaos)
                .FirstOrDefaultAsync();

            return spot;
        }

        public async Task<Spot> GetSpotClienteBy(int idCliente)
        {
            var spot = await _context.Spots
                .Include(s => s.IdClienteNavigation)
                .Where(s => s.IdCliente == idCliente && s.IsActive == true && s.IdClienteNavigation.IsActive == true)
                .FirstOrDefaultAsync();

            return spot;
        }

        public List<Spot> GetSpots()
        {
            var spots = _context.Spots
                .Where(s => s.IsActive == true).ToList();
            return spots;
        }

        public async Task Update(Spot spot)
        {
            _context.Entry(spot).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSpot(int spotId, Spot spot)
        {
            var s = await _context.Spots.FindAsync(spotId);
            if (s != null)
            {
                s.Designacao = spot.Designacao;
                s.Code = spot.Code;
                s.Duracao = spot.Duracao;

                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateSpotDate(int spotId, DateTime newdate)
        {
            var s = await _context.Spots.FindAsync(spotId);
            if (s != null)
            {
                s.DataFim = newdate;

                await _context.SaveChangesAsync();
            }
        }

        public async Task EditarSpot(int spotId, Spot spot)
        {
            var s = await _context.Spots.FindAsync(spotId);
            if (s != null)
            {
                s.Designacao = spot.Designacao;
                s.Duracao = spot.Duracao;
                s.DataFim = spot.DataFim;

                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateEstado(int idSpot)
        {
            var s = await _context.Spots.FindAsync(idSpot);
            if (s != null)
                s.Estado = 2;

            await _context.SaveChangesAsync();
        }
        public async Task UpdateAnuncio(int anuncioId, Spot anuncio)
        {
            var a = await _context.Spots.FindAsync(anuncioId);
            if (a != null)
            {
                a.Designacao = anuncio.Designacao;
                a.Code = anuncio.Code;
                a.DataFim = anuncio.DataFim;
                a.Periodicidade = anuncio.Periodicidade;
                a.Duracao = anuncio.Duracao;

                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateSpotEstado(Spot spot)
        {
            var a = await _context.Spots.FindAsync(spot.Id);
            if (a != null)
            {
                a.Estado = 2;
                await _context.SaveChangesAsync();
            }
        }

        public IEnumerable<Spot> GetAllSpotsClienteBy(int IdCliente)
        {
            var spots = _context.Spots
                .Where(s => s.IdCliente == IdCliente && s.Estado == 1)
                .OrderByDescending(s => s.Id)
                .ToList();

            return spots;
        }

        public IEnumerable<Spot> GetAllSpotsEstado()
        {
            var spots = _context.Spots
                .Where(s => s.IsActive == true && s.DataFim.Value.Date < DateTime.Now.Date && s.IdClienteNavigation.IsActive == true)
                .ToList();
            return spots;
        }

        public async Task<Spot> GetlastProcessoBy(int tipoProcesso)
        {
            try
            {
                var processo = await _context.Spots.Where(s => s.IdTipoSpot == tipoProcesso && s.IsActive == true && s.IdClienteNavigation.IsActive == true)
                    .OrderByDescending(s => s.Id)
                    .FirstAsync();

                return processo;

            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
