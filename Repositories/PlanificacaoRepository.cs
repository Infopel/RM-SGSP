using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using SGSP.Data;
using SGSP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Repositories
{
    public class PlanificacaoRepository : IPlanificacaoRepository
    {
        private readonly dbSpotsContext _context;
        public PlanificacaoRepository(dbSpotsContext context)
        {
            _context = context;
        }

        public async Task<SpotPlanificacao> Create(SpotPlanificacao planificacao)
        {
            _context.SpotPlanificacaos.Add(planificacao);
            await _context.SaveChangesAsync();

            return planificacao;
        }

        public async Task<SpotPlanificacao> CreateList(SpotPlanificacao[] list)
        {

            foreach (SpotPlanificacao c in list)
            {
                _context.SpotPlanificacaos.Add(c);
            }

            await _context.SaveChangesAsync();

            return list.FirstOrDefault();
        }

        public async Task Delete(int id)
        {
            var planificacaoToDelete = await _context.SpotPlanificacaos.FindAsync(id);
            _context.SpotPlanificacaos.Remove(planificacaoToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<SpotPlanificacao>> Get()
        {
            return await _context.SpotPlanificacaos.ToListAsync();
        }

        public async Task<SpotPlanificacao> Get(int id)
        {
            var planificacao = await _context.SpotPlanificacaos
                .Include(p => p.IdSpotNavigation)
                .Include(p => p.IdSpotNavigation.IdClienteNavigation)
                .Where(p => p.Id == id).ToListAsync();

            return planificacao.FirstOrDefault();
        }

        public async Task<SpotPlanificacao> GetLastBy(int idCanal, int idJanela, DateTime? data)
        {
            var lastPlanificacao = await _context.SpotPlanificacaos
                .Include(p => p.IdSpotNavigation)
                .Where(p => p.IdSpotNavigation.IsActive == true && p.IdCanal == idCanal && p.IdJanela == idJanela && p.DataPlanificacao.Value.Date == data.Value.Date && p.Estado != 6)
                .Include(p => p.IdJanelaNavigation)
                .OrderBy(p => p.Id)
                .LastOrDefaultAsync();

            return lastPlanificacao;
        }

        public IEnumerable<SpotPlanificacao> GetAllPlanificacao()
        {
            var planificacao = _context.SpotPlanificacaos
                .Include(p => p.IdSpotNavigation)
                    .ThenInclude(spot => spot.IdClienteNavigation)
                    .Include(spot => spot.IdSpotNavigation.IdTipoSpotNavigation)
                    .Include(s => s.IdCanalNavigation)
                    .Where(s => s.Estado < 6 && s.IdSpotNavigation.IsActive == true)
                .ToList();
            return planificacao;
        }
        public IEnumerable<SpotPlanificacao> GetAllPlanificacaoBy(int idCanal)
        {
            var planificacao = _context.SpotPlanificacaos
                .Include(p => p.IdSpotNavigation)
                    .ThenInclude(spot => spot.IdClienteNavigation)
                    .Include(spot => spot.IdSpotNavigation.IdTipoSpotNavigation)
                    .Include(s => s.IdCanalNavigation)
                    .Where(s => s.Estado < 6 && s.IdSpotNavigation.IsActive == true && s.IdCanal == idCanal)
                .ToList();
            return planificacao;
        }

        public IEnumerable<SpotPlanificacao> GetAllPlanificacaoByToday(int idCanal)
        {
            var planificacao = _context.SpotPlanificacaos
                .Include(p => p.IdSpotNavigation)
                    .ThenInclude(spot => spot.IdClienteNavigation)
                    .Include(spot => spot.IdSpotNavigation.IdTipoSpotNavigation)
                    .Include(s => s.IdCanalNavigation)
                .Where(s => s.Estado < 5 && s.DataPlanificacao.Value.Date == DateTime.Now.Date && s.IdSpotNavigation.IsActive == true && s.IdCanal == idCanal)
                .ToList();
            return planificacao;
        }

        public IEnumerable<SpotPlanificacao> GetAllPlanificacaoLocutorByToday(int idCanal)
        {
            //Estados 1 - Pendente; 2 - Skip; 3 - Transmitido; 4 - Anuncio a Espera de parecer Coordenador; 5 - Inactivo
            var planificacao = _context.SpotPlanificacaos
                .Include(p => p.IdSpotNavigation)
                    .ThenInclude(spot => spot.IdClienteNavigation)
                    .Include(s => s.IdCanalNavigation)
                    .Include(spot => spot.IdSpotNavigation.IdTipoSpotNavigation)
                .Where(s => s.Estado < 5  && s.DataPlanificacao.Value.Date == DateTime.Now.Date && s.IdSpotNavigation.IsActive == true && s.IdCanal == idCanal)
                .ToList();
            return planificacao;
        }

        public IEnumerable<SpotPlanificacao> GetAllPlanificacaoCoordenadasByToday(int idCanal)
        {
            var planificacao = _context.SpotPlanificacaos
                .Include(p => p.IdSpotNavigation)
                    .ThenInclude(spot => spot.IdClienteNavigation)
                    .Include(spot => spot.IdSpotNavigation.IdTipoSpotNavigation)
                    .Include(s => s.IdCanalNavigation)
                .Where(s => ((s.Estado == 2 && s.ParecerCoordenador == null) || (s.Estado == 3 && s.DataPassagemConfirmacao == null) || (s.Estado == 4 && s.DataPassagem != null) ) && (s.DataPlanificacao.Value.Date == DateTime.Now.Date && s.IdSpotNavigation.IsActive == true && s.IdCanal == idCanal))
                .ToList();
            return planificacao;
        }

        public IEnumerable<SpotPlanificacao> GetAllPlanificacaoClienteBy(int idCliente)
        {
            var planificacao = _context.SpotPlanificacaos
                    .Include(p => p.IdSpotNavigation)
                    .ThenInclude(spot => spot.IdClienteNavigation)
                    .Include(spot => spot.IdSpotNavigation.IdTipoSpotNavigation)
                    .Include(s => s.IdCanalNavigation)
                .Where(p => p.IdSpotNavigation.IdClienteNavigation.Id == idCliente && (p.Estado == 2 || p.Estado == 3))
                .ToList();

            return planificacao;
        }

            public async Task Update(SpotPlanificacao planificacao)
        {
            _context.Entry(planificacao).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePlanificacaoPatchAsync(int planificacaoId, JsonPatchDocument planidicacaoModel)
        {
            var planificacao = await _context.SpotPlanificacaos.FindAsync(planificacaoId);

            if(planificacao != null)
            {
                planidicacaoModel.ApplyTo(planificacao);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdatePlanificacao(int planificacaoId, SpotPlanificacao planificacao)
        {
            var plan = await _context.SpotPlanificacaos.FindAsync(planificacaoId);
            if(plan != null)
            {
                plan.DataSkip = planificacao.DataSkip;
                plan.DataPassagem = planificacao.DataPassagem;
                plan.DataPassagemConfirmacao = planificacao.DataPassagemConfirmacao;
                plan.SkipMotivo = planificacao.SkipMotivo;
                plan.UserLocutor = planificacao.UserLocutor;
                plan.UserCoordenador = planificacao.UserCoordenador;
                plan.ParecerCoordenador = planificacao.ParecerCoordenador;
                plan.Estado = planificacao.Estado;

                await _context.SaveChangesAsync();
            }
        }

        public async Task EditPlanificacao(int planificacaoId, SpotPlanificacao planificacao)
        {
            var plan = await _context.SpotPlanificacaos.FindAsync(planificacaoId);
            if (plan != null)
            {
                plan.DataPlanificacao = planificacao.DataPlanificacao;
                plan.IdJanela = planificacao.IdJanela;

                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdatePlanificacaoEstado(SpotPlanificacao planificacao)
        {
            var plan = await _context.SpotPlanificacaos.FindAsync(planificacao.Id);
            if (plan != null)
            {
                plan.Estado = 2;
                plan.DataSkip = DateTime.Now;
                plan.ParecerCoordenador = "Server Service Runned!!";
                plan.UserCoordenador = "Server Service";
                if(planificacao.UserLocutor == null)
                {
                    plan.SkipMotivo = "Server Service Runned!!";
                    plan.UserLocutor = "Server Service";
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdatePlanificacaoEstado(int planificacaoId)
        {
            //Estados 1 - Pendente; 2 - Skip; 3 - Transmitido; 4 - Anuncio a Espera de parecer Coordenador; 5 - Inactivo

            var plan = await _context.SpotPlanificacaos.FindAsync(planificacaoId);
            if (plan != null)
            {
                plan.Estado = 5;
                await _context.SaveChangesAsync();
            }
        }

        public async Task OrderPlanificacao(int planificacaoId, DateTime? newDate)
        {
            var plan = await _context.SpotPlanificacaos.FindAsync(planificacaoId);
            if (plan != null)
            {
                plan.Prioridade = newDate;
                await _context.SaveChangesAsync();
            }
        }

        public async Task OrderPlanificacaoDrag(int planificacaoId, DateTime? newDateP, DateTime? newDateD)
        {
            var plan = await _context.SpotPlanificacaos.FindAsync(planificacaoId);
            if (plan != null)
            {
                plan.Prioridade = newDateP;
                plan.DataPlanificacao = newDateD;

                await _context.SaveChangesAsync();
            }
        }

        public List<SpotPlanificacao> getPlanificacaoby(int? idCliente, int? tipoSpot, DateTime? dataI, DateTime? dataF, int? idCanal)
        {
            var planificacao = _context.SpotPlanificacaos
                .Where(s => s.IdSpotNavigation.IdTipoSpot == tipoSpot && s.IdSpotNavigation.IdCliente == idCliente && s.IdCanal == idCanal && s.Estado == 3 && s.DataPassagemConfirmacao.Value.Date >= dataI && s.DataPassagemConfirmacao.Value.Date <= dataF)
                .Include(s => s.IdSpotNavigation)
                .Include(s => s.IdSpotNavigation.IdClienteNavigation)
                .Include(s => s.IdCanalNavigation)
                .ToList();

            return planificacao;
        }

        public IEnumerable<SpotPlanificacao> GetAllPlanificacaoClienteFilterBy(Filter filter)
        {
            var planificacao = _context.SpotPlanificacaos
                .Where(s => (s.IdSpotNavigation.IdCliente == filter.IdCliente && s.IdSpotNavigation.Id == filter.IdSpot) &&
                (s.Estado == 2 && s.DataPassagemConfirmacao.Value.Date >= filter.DataI.Value.Date) &&
                (s.Estado == 2 && s.DataPassagemConfirmacao.Value.Date <= filter.DataF.Value.Date) ||
                (s.Estado == 3 && s.DataSkip.Value.Date >= filter.DataI.Value.Date) &&
                (s.Estado == 3 && s.DataSkip.Value.Date <= filter.DataF.Value.Date))
                .Include(s => s.IdSpotNavigation)
                .Include(s => s.IdSpotNavigation.IdClienteNavigation)
                .Include(spot => spot.IdSpotNavigation.IdTipoSpotNavigation)
                .Include(s => s.IdCanalNavigation)
                .OrderByDescending(s => s.Id)
                .ToList();

            return planificacao;
        }

        public IEnumerable<SpotPlanificacao> GetAllPlanificacaoEstado()
        {
            var planificacao = _context.SpotPlanificacaos
                    .Where(s => s.DataPlanificacao.Value.Date < DateTime.Now.Date && (s.Estado == 1 || s.Estado == 4)  && s.IdSpotNavigation.IsActive == true)
                .ToList();
            return planificacao;
        }
    }
}
