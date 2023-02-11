using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    public class CanalsController : ControllerBase
    {

        private readonly ICanalsRepository _canalsRepository;
        private readonly IJanelasRepository _janelasRepository;
        private readonly EmailController _emailController;
        private readonly IUsuariosRepository _usuariosRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public CanalsController(ICanalsRepository canalsRepository, 
            IJanelasRepository janelasRepository, EmailController emailController,
            IUsuariosRepository usuariosRepository, UserManager<IdentityUser> userManager)
        {
            _canalsRepository = canalsRepository;
            _janelasRepository = janelasRepository;
            _emailController = emailController;
            _usuariosRepository = usuariosRepository;
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult GetCanals()
        {
            IEnumerable<Canal> canals = _canalsRepository.Get();
            return Ok(canals);
        }

        [HttpPost]
        public async Task<ActionResult<Canal>> PostCanal([FromForm] Canal canal, [FromForm] Janela[] list)
        {
            var verfCanal = await _canalsRepository.GetBy(canal.Designacao);

            if (verfCanal == null)
            {
                canal.IsActive = true;
                canal.DataCreate = DateTime.Now;
                var newCanal = await _canalsRepository.Create(canal);

                foreach(var j in list)
                {
                    Janela newJanela = new Janela()
                    {
                        Designacao = j.Designacao,
                        HoraInicio = j.HoraInicio,
                        HoraFim = j.HoraFim,
                        IdCanal = newCanal.Id,
                        DiasSemana = j.DiasSemana,
                        IsActive = true
                    };

                    await _janelasRepository.Create(newJanela);
                }

                return CreatedAtAction(nameof(GetCanals), new { id = newCanal.Id }, newCanal);

            }

            return Ok(new { d = "-1" });
        }

        [Route("updateCanal/{idCanal}/{idJanela}")]
        [HttpPost]
        public async Task<ActionResult<Canal>> PutCanal([FromRoute] int idCanal, [FromRoute] int idJanela, [FromBody] Canal canal)
        {

            var getJanela = await _janelasRepository.GetBy(canal.Janelas.FirstOrDefault().Id);

            await _canalsRepository.UpdateCanal(idCanal, canal);
            await _janelasRepository.UpdateJanela(idJanela, canal.Janelas.FirstOrDefault());

            return NoContent();

        }
        [HttpGet("getCanaisUserBy/{idAsp}")]
        public IActionResult GetAllCanalUserListBy([FromRoute] Guid idAsp)
        {
            IEnumerable<Usuario> userCanal = _usuariosRepository.GetAllCanalsBy(idAsp.ToString());
            var result = MapUserToCanalDto(userCanal);

            return Ok(new { result });
        }

        private IEnumerable<CanalDto> MapUserToCanalDto(IEnumerable<Usuario> usuario)
        {
            IEnumerable<CanalDto> result;
            result = usuario.Select(s => new CanalDto()
            {
                Id = s.IdCanal.Value,
                Name = s.IdCanalNavigation.Designacao

            });

            return result;
        }

        [HttpPost("usuarioCanalUpdate")]
        public async Task<ActionResult<Usuario>> UpdateUsuario([FromForm] CanalDto usuario)
        {
            var getUsuario = await _usuariosRepository.GetUsurioBy(usuario.IdUserAsp.ToString());
            var user = await _userManager.FindByIdAsync(usuario.IdUserAsp.ToString());
            var roles = await _userManager.GetRolesAsync(user);

            if (getUsuario != null)
            {
                if (usuario.OldIdCanais != null)
                    foreach (var oldUserCanl in usuario.OldIdCanais)
                    {
                        Usuario oldUser = new Usuario()
                        {
                            IdAspNetUser = usuario.IdUserAsp.ToString(),
                            IdCanal = oldUserCanl
                        };

                        await _usuariosRepository.UpdateUsuarioCanal(oldUser);
                    }

                foreach (var newUserCanal in usuario.NewIdCanais)
                {
                    Usuario newUser = new Usuario()
                    {
                        IdAspNetUser = usuario.IdUserAsp.ToString(),
                        IdCanal = newUserCanal,
                        IsActive = true,
                        CreatedOn = DateTime.Now,
                        Role = roles.FirstOrDefault()
                    };

                    await _usuariosRepository.Create(newUser);
                }

                return Ok(new { data = "1" });
            }

            return Ok(new { data = "-1" });
        }
    }
}
