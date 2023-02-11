using SGSP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Repositories
{
    public interface ISpotsRepository
    {
        Task<IEnumerable<Spot>> Get();
        Task<Spot> Get(int? id);
        Task<Spot> Create(Spot spot);
        Task Update(Spot spot);
        Task Delete(int id);
        Task<Spot> GetlastProcessoBy(int tipoProcesso);
        List<Spot> GetSpots();
        IEnumerable<Spot> GetAllSpots();
        IEnumerable<Spot> GetAllSpotsEstado();
        IEnumerable<Spot> GetAllSpotsClienteBy(int IdCliente);
        IEnumerable<Spot> GetAllSpotsBy(int idCanal);
        IEnumerable<Spot> GetAllAnuncios();
        IEnumerable<Spot> GetAllAnunciosBy(int idCanal);
        Task UpdateSpot(int spotId, Spot spot);
        Task UpdateSpotEstado(Spot spot);
        Task UpdateSpotDate(int spotId, DateTime newDate);
        Task UpdateAnuncio(int anuncioId, Spot anuncio);
        Task EditarSpot(int spotId, Spot spot);
        Task UpdateEstado(int idSpot);
        Task<Spot> GetSpotBy(string code);
        Task<Spot> GetSpotClienteBy(int idCliente);
    }
}
