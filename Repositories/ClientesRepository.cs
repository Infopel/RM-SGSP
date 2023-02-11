using Microsoft.EntityFrameworkCore;
using SGSP.Data;
using SGSP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Repositories
{
    public class ClientesRepository : IClientesRepository
    {
        private readonly dbSpotsContext _context;
        public ClientesRepository(dbSpotsContext context)
        {
            _context = context;
        }
        public async Task<Cliente> Create(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return cliente;
        }

        public void CreateCliente(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
        }

        public async Task Delete(int id)
        {
            var clienteToDelete = await _context.Clientes.FindAsync(id);
            _context.Clientes.Remove(clienteToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Cliente>> Get()
        {
            return await _context.Clientes.Where(p => p.IsActive == true).ToListAsync();
        }

        public async Task<Cliente> Get(int id)
        {
            return await _context.Clientes
                .FindAsync(id);
        }

        public async Task<Cliente> GetClienteBy(string nuit)
        {
            return await _context.Clientes.Where(c => c.Nuit == nuit && c.IsActive == true).FirstOrDefaultAsync();
        }
        public IEnumerable<Cliente> GetAllClientes()
        {
            var cliente = _context.Clientes
                .Include(c => c.Spots)
                .Where(c => c.IsActive == true)
                .ToList();
            return cliente;
        }

        public Cliente GetClienteBy(int id)
        {
            throw new NotImplementedException();
        }

        public List<Cliente> GetClientes()
        {
            var clientes = _context.Clientes
                .Where(p => p.IsActive == true).ToList();
            return clientes;
        }

        public async Task<IEnumerable<Cliente>> Search()
        {
            IQueryable<Cliente> query = _context.Clientes
                .Where(c => c.IsActive == true);

            //if(!string.IsNullOrEmpty(any))
            //{
            //    query = query.Where(c => c.Designacao.ToLower() == any.ToLower() || c.Nuit.ToLower() == any.ToLower());
            //}

            return await query.ToListAsync();
        }

        public async Task Update(Cliente cliente)
        {
            _context.Entry(cliente).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCliente(int clienteId, Cliente cliente)
        {
            var c = await _context.Clientes.FindAsync(clienteId);
            if (c != null)
            {
                c.Designacao = cliente.Designacao;
                c.Nuit = cliente.Nuit;
                c.EMail = cliente.EMail;
                c.Telefone = cliente.Telefone;

                await _context.SaveChangesAsync();
            }
        }

    }
}
