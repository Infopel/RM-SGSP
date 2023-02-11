using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Hubs
{
    public class PlanificacaoHub : Hub
    {
        public async Task SendPlanificacao(string planificacao, int idCanal)
        {
            await Clients.All.SendAsync("ReceiveAllPlanificacao", planificacao, idCanal);
        }
        
        public async Task SendPlanificacaoChange(string planificacao, int idCanal)
        {
            await Clients.All.SendAsync("ReceiveThePlanificacaoChange", planificacao, idCanal);
        }    
        
        public async Task SendNewPlanificacao(string planificacao, int idCanal, bool isAll)
        {
            await Clients.All.SendAsync("ReceiveNewPlanificacao", planificacao, idCanal, isAll);
        }

        public async Task PlanificacaoChange(int idCanal)
        {
            await Clients.All.SendAsync("ReceivePlanificacaoChange", idCanal);
        }

    }
}
