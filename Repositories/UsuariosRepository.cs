using Microsoft.EntityFrameworkCore;
using SGSP.Data;
using SGSP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Repositories
{
    public class UsuariosRepository : IUsuariosRepository
    {

        private readonly dbSpotsContext _context;
        public UsuariosRepository(dbSpotsContext context)
        {
            _context = context;
        }

        public async Task<Usuario> Create(Usuario user)
        {
            _context.Usuarios.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public IEnumerable<Usuario> GetAllCanalsBy(string idAsp)
        {
            return _context.Usuarios
                .Where(u => u.IdAspNetUser == idAsp && u.IsActive == true)
                .Include(u => u.IdCanalNavigation)
                .ToList();
        }
        public IEnumerable<Usuario> GetAllUserCanalsBy(int idCanal)
        {
            return _context.Usuarios
                .Where(u => u.IdCanal == idCanal && u.IsActive == true && u.Role.ToLower().Contains("coordenador"))
                .Include(u => u.IdCanalNavigation)
                .ToList();
        }
        public async Task<Usuario> GetUsurioBy(string idAsp)
        {
            var usurario = await _context.Usuarios
                .Where(u => u.IdAspNetUser == idAsp && u.IsActive == true)
                .FirstOrDefaultAsync();

            return usurario;
        }
        public async Task<Usuario> GetUsurioCanalBy(string idAsp, int idCanal)
        {
            var usurario = await _context.Usuarios
                .Where(u => u.IdAspNetUser == idAsp && u.IsActive == true && u.IdCanal == idCanal)
                .FirstOrDefaultAsync();

            return usurario;
        }

        public async Task UpdateUsuario(string idUser)
        {
            var user = await _context.Usuarios
                .Where(u => u.IdAspNetUser == idUser && u.IsActive == true)
                .FirstOrDefaultAsync();

            if (user != null)
            {
                user.IsActive = false;
                user.RemovedOn = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateUsuarioCanal(Usuario usuario)
        {
            var user = await _context.Usuarios
                .Where(u => u.IdAspNetUser == usuario.IdAspNetUser && u.IdCanal == usuario.IdCanal && u.IsActive == true)
                .FirstAsync();

            if(user != null)
            {
                user.IsActive = false;
                user.RemovedOn = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}
