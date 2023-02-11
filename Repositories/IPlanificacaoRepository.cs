using Microsoft.AspNetCore.JsonPatch;
using SGSP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Repositories
{
    public interface IPlanificacaoRepository
    {
        Task<IEnumerable<SpotPlanificacao>> Get();
        Task<SpotPlanificacao> Get(int id);
        Task<SpotPlanificacao> Create(SpotPlanificacao planificacao);
        Task Update(SpotPlanificacao planificacao);
        Task Delete(int id);
        IEnumerable<SpotPlanificacao> GetAllPlanificacao();
        IEnumerable<SpotPlanificacao> GetAllPlanificacaoEstado();
        IEnumerable<SpotPlanificacao> GetAllPlanificacaoBy(int idCanal);
        IEnumerable<SpotPlanificacao> GetAllPlanificacaoByToday(int idCanal);
        IEnumerable<SpotPlanificacao> GetAllPlanificacaoLocutorByToday(int idCanal);
        IEnumerable<SpotPlanificacao> GetAllPlanificacaoCoordenadasByToday(int idCanal);
        IEnumerable<SpotPlanificacao> GetAllPlanificacaoClienteBy(int idCliente);
        IEnumerable<SpotPlanificacao> GetAllPlanificacaoClienteFilterBy(Filter filter);
        Task<SpotPlanificacao> CreateList(SpotPlanificacao[] list);
        Task UpdatePlanificacaoPatchAsync(int planificacaoId, JsonPatchDocument planidicacaoModel);
        Task UpdatePlanificacao(int planificacaoId, SpotPlanificacao planificacao);
        Task UpdatePlanificacaoEstado(SpotPlanificacao planificacao);
        Task UpdatePlanificacaoEstado(int planificacaoId);
        Task OrderPlanificacao(int planificacaoId, DateTime? newDate);
        Task OrderPlanificacaoDrag(int planificacaoId, DateTime? newDateP, DateTime? newDateD);
        Task<SpotPlanificacao> GetLastBy(int idCanal, int idJanela, DateTime? data);
        Task EditPlanificacao(int planificacaoId, SpotPlanificacao planificacao);
        List<SpotPlanificacao> getPlanificacaoby(int? idCliente, int? tipoSpot, DateTime? dataI, DateTime? dataF, int? idCanal);
    }
}
