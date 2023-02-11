using SGSP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Repositories
{
    public interface IClientesRepository
    {
        Task<IEnumerable<Cliente>> Get();
        Task<IEnumerable<Cliente>> Search();
        Task<Cliente> Get(int id);
        Task<Cliente> Create(Cliente cliente);
        Task Update(Cliente cliente);
        Task Delete(int id);
        List<Cliente> GetClientes();
        IEnumerable<Cliente> GetAllClientes();
        void CreateCliente(Cliente cliente);
        Task UpdateCliente(int clienteId, Cliente cliente);
        Task<Cliente> GetClienteBy(string nuit);
    }
}
