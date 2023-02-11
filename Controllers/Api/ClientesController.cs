using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGSP.Dtos;
using SGSP.Models;
using SGSP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly IClientesRepository _clientesRepository;
        private readonly ISpotsRepository _spotsRepository;
        private readonly AdministrationController _administrationController;
        private readonly IUsuariosRepository _usuariosRepository;
        public ClientesController(IClientesRepository clientesRepository, ISpotsRepository spotsRepository, AdministrationController administrationController, IUsuariosRepository usuariosRepository)
        {
            _clientesRepository = clientesRepository;
            _spotsRepository = spotsRepository;
            _administrationController = administrationController;
            _usuariosRepository = usuariosRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<Cliente>> GetClientes()
        {

            return await _clientesRepository.Get();
        }
        [HttpGet("search")]
        public async Task<ActionResult<Cliente>> Search(string any)
        {
            try
            {
                if (!string.IsNullOrEmpty(any))
                {
                    var states = await _clientesRepository.Search();
                    var data = states.Where(a => a.Designacao.Contains(any, StringComparison.OrdinalIgnoreCase)
                    || a.Nuit.Contains(any, StringComparison.OrdinalIgnoreCase)).ToList().AsReadOnly();

                    return Ok(data);
                }
                else
                {
                    return Ok(new { d = "-1" });
                }
            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetClientes(int id)
        {
            return await _clientesRepository.Get(id);
        }

        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente([FromForm] Cliente cliente)
        {

            var verfCliente = await _clientesRepository.GetClienteBy(cliente.Nuit);

            if(verfCliente == null)
            {
                cliente.DataCreate = DateTime.Today;
                cliente.IsActive = true;

                var newCliente = await _clientesRepository.Create(cliente);

                string[] nomeCliente = cliente.Designacao.ToString().Split(' ');

                CreateUserWithRoleViewModel newUser = new CreateUserWithRoleViewModel()
                {
                    Nome = nomeCliente.First(),
                    Apelido = nomeCliente.Last(),
                    Email = cliente.EMail,
                    idRole = "1e2cdaa6-b863-4c78-b19d-0e23c5f1214d",
                    UserName = cliente.EMail,
                    IdCanal = new string[] {""},
                    IdCliente = newCliente.Id
            };

                var getUserId = await _administrationController.CreateUserWithRole(newUser);

                return CreatedAtAction(nameof(GetClientes), new { id = newCliente.Id }, newCliente);
            }

            return Ok(new { d = "-1" });
        }
        [Route("updateCliente/{id}")]
        [HttpPost]
        public async Task<ActionResult<Cliente>> PutClientes([FromRoute] int id, [FromBody] Cliente cliente)
        {
            try {
                if (id != cliente.Id)
                {
                    return BadRequest();
                }

                var oldCliente = await _clientesRepository.Get(id);

                Cliente clienteHistory = new Cliente()
                {
                    Designacao = oldCliente.Designacao,
                    Nuit = oldCliente.Nuit,
                    Telefone = oldCliente.Telefone,
                    EMail = oldCliente.EMail,
                    IsActive = false,
                    DataCreate = DateTime.Now,
                    IdClientePrincipal = id
                };

                await _clientesRepository.Create(clienteHistory);

                await _clientesRepository.UpdateCliente(id, cliente);

                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

        }
        [Route("disableCliente/{id}")]
        [HttpPost]
        public async Task<ActionResult<Cliente>> PutClienteDisable([FromRoute] int id)
        {
            var clienteToDisable = await _clientesRepository.Get(id);

            if (clienteToDisable == null)
                return NotFound();

            clienteToDisable.IsActive = false;
            await _clientesRepository.Update(clienteToDisable);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete (int id)
        {
            var clienteToDelete = await _clientesRepository.Get(id);
           
            if (clienteToDelete == null)
                return NotFound();

            await _clientesRepository.Delete(clienteToDelete.Id);
            return NoContent();
        }
        [HttpGet ("listCliente")]
        public IActionResult GetAllClienteList()
        {

            IEnumerable<Cliente> cliente = _clientesRepository.GetAllClientes();

            var result = MapClienteToClientesDto(cliente);

            return Ok(new{data = result });
        }

        //[HttpGet("listaAllClientes")]
        //public IEnumerable<ClientesDto> GetAllClientes()
        //{
        //    var clientes = _clientesRepository.GetAllClientes()
        //        .Select(cliente => cliente.AsDto());

        //    return clientes;
        //}

        private ClientesDto MapClienteToClienteDto(Cliente cliente)
        {
                return new ClientesDto
                {
                    Id = cliente.Id,
                    Designacao = cliente.Designacao,
                    Nuit = cliente.Nuit,
                    Telefone = cliente.Telefone,
                    EMail = cliente.EMail,
                    DataCreate = cliente.DataCreate,
                    NSpots = cliente.Spots.Count()
                };
        }

        private IEnumerable<ClientesDto> MapClienteToClientesDto(IEnumerable<Cliente> clientes)
        {
            IEnumerable<ClientesDto> result;
            result = clientes.Select(c => new ClientesDto()
            {
                Id = c.Id,
                Designacao = c.Designacao,
                Nuit = c.Nuit,
                Telefone = c.Telefone,
                EMail = c.EMail,
                DataCreate = c.DataCreate,
                NSpots = c.Spots.Count(),
                Action = getAction(c.Id, c.Designacao, c.Nuit, c.Telefone, c.EMail)
            });

            return result;
        }

        private string getAction(int id, string designacao, string nuit, string telefone, string email)
        {

            var result = "<div class='options-action'><i  data-id='"+id+"' data-designacao='"+designacao+"' data-nuit='"+nuit+"' data-telefone='"+telefone+"' data-email='"+email+"' class='editarr fas fa-pencil-alt'></i> <i data-id='"+id+"' data-designacao='"+designacao+"' data-nuit='"+nuit+"' data-telefone='"+telefone+"' data-email='" +email+ "' class='destivarr fas fa-trash-alt'></i></div>";

            return result;
        }
    }
}
