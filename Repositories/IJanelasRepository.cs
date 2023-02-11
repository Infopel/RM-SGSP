using SGSP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Repositories
{
    public interface IJanelasRepository
    {
        IEnumerable<Janela> Get();
        Task<Janela> Create(Janela janela);
        IEnumerable<Janela> GetAllBy(int id);
        Task<Janela> GetBy(int idJanela);
        Task Update(Janela janela);
        Task UpdateJanela(int idJanela, Janela janela);
    }
}
