using Microsoft.AspNetCore.Mvc;
using SGSP.Dtos;
using SGSP.Models;
using SGSP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SGSP.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class JanelasController : ControllerBase
    {
        private readonly IJanelasRepository _janelaRepository;

        public JanelasController(IJanelasRepository janelasRepository)
        {
            _janelaRepository = janelasRepository;
        }

        // GET: api/<JanelasController>
        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<Janela> janelas = _janelaRepository.Get();
            var result = MapJanelasToJanelasDto(janelas);

            return Ok(new { data = result });
        }

        [HttpGet("{idCanal}")]
        public IActionResult GetJanelasBy(int idCanal)
        {
            IEnumerable<Janela> janelas = _janelaRepository.GetAllBy(idCanal);

            return Ok(janelas);
        }

        // POST api/<JanelasController>
        [HttpPost]
        public async Task<ActionResult<Janela>> PostJanela([FromForm] Janela [] janelas)
        {
            
            foreach(var j in janelas)
            {
                Janela newJanela = new Janela()
                {
                    Designacao = j.Designacao,
                    HoraInicio = j.HoraInicio,
                    HoraFim = j.HoraFim,
                    IdCanal = j.IdCanal,
                    DiasSemana = j.DiasSemana,
                    IsActive = true
                };

                await _janelaRepository.Create(newJanela);
            }

            return Ok(new { d = "1" });
        }

        // PUT api/<JanelasController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [Route("disableJanela/{id}")]
        [HttpPost]
        public async Task<ActionResult<Janela>> PutJanelaDisable([FromRoute] int id)
        {
            var janelaToDisable = await _janelaRepository.GetBy(id);

            if (janelaToDisable == null)
                return NotFound();

            janelaToDisable.IsActive = false;
            await _janelaRepository.Update(janelaToDisable);

            return NoContent();
        }

        // DELETE api/<JanelasController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private IEnumerable<JanelasDto> MapJanelasToJanelasDto(IEnumerable<Janela> janelas)
        {
            IEnumerable<JanelasDto> result;
            result = janelas.Select(j => new JanelasDto()
            {
                Id = j.Id,
                Designacao = j.Designacao,
                HoraInicio = j.HoraInicio,
                HoraFim = j.HoraFim,
                DiasSemana = j.DiasSemana,
                Canal = j.IdCanalNavigation.Designacao,
                CodeCanal = j.IdCanalNavigation.Code,
                IdCanal = j.IdCanalNavigation.Id,
                IsActive = j.IsActive,
                Action = getAction(j.Id, j.IdCanalNavigation.Id, j.Designacao, j.IdCanalNavigation.Designacao, j.IdCanalNavigation.Code, j.HoraInicio.ToString(), j.HoraFim.ToString(), j.DiasSemana)
            }); ;

            return result;
        }

        private string getAction(int idJanela, int idCanal, string designacaoJanela, string designacaoCanal, string codeCanal, string horaI, string horaF, string diasSemana)
        {

            var result = "<div class='options-action'><i  data-idjanela='" + idJanela + "' data-idcanal='" + idCanal + "' data-designacaojanela='" + designacaoJanela + "' data-codeCanal='" + codeCanal + "' data-designacaocanal='" + designacaoCanal + "' data-horai='" + horaI + "' data-horaf='" + horaF + "' data-dias='" + diasSemana + "' class='editarr fas fa-pencil-alt'></i> <i data-idjanela='" + idJanela + "' data-idcanal='" + idCanal + "' data-designacaojanela='" + designacaoJanela + "' data-designacaocanal='" + designacaoCanal + "' data-horai='" + horaI + "' data-horaf='" + horaF + "' data-codeCanal='" + codeCanal + "' data-dias='" + diasSemana + "' class='destivarr fas fa-trash-alt'></i></div>";

            return result;
        }
    }
}
