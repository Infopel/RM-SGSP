using SGSP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Repositories
{
    public interface IUsuariosRepository
    {
        Task<Usuario> Create(Usuario user);
        Task UpdateUsuario(string idUser);
        Task UpdateUsuarioCanal(Usuario usuario);
        Task<Usuario> GetUsurioBy(string idAsp);
        Task<Usuario> GetUsurioCanalBy(string idAsp, int idCanal);
        IEnumerable<Usuario> GetAllCanalsBy(string idAsp);
        IEnumerable<Usuario> GetAllUserCanalsBy(int idCanal);
    }
}
