using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SGSP.Dtos;
using SGSP.Models;
using SGSP.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanificacoesController : ControllerBase
    {
        private readonly IPlanificacaoRepository _planificacaoRepository;
        private readonly IJanelasRepository _janelasRepository;
        private readonly ISpotsRepository _spotsRepository;
        private readonly IUsuariosRepository _usuariosRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly EmailController _emailController;

        public PlanificacoesController(IPlanificacaoRepository planificacaoRepository, IJanelasRepository janelasRepository, ISpotsRepository spotsRepository,
            UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IUsuariosRepository usuariosRepository,
            EmailController emailController)
        {
            _planificacaoRepository = planificacaoRepository;
            _janelasRepository = janelasRepository;
            _spotsRepository = spotsRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _usuariosRepository = usuariosRepository;
            _emailController = emailController;
        }

        [HttpGet]
        public IEnumerable<SpotPlanificacao> GetPlanificacao()
        {
            return _planificacaoRepository.GetAllPlanificacao();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SpotPlanificacao>> GetPlanificacaoBy(int id)
        {
            return await _planificacaoRepository.Get(id);
        }

        [HttpPost]
        public async Task<ActionResult<SpotPlanificacao>> PostPlanificacao([FromBody] SpotPlanificacao planificacao)
        {
            var newPlanificacao = await _planificacaoRepository.Create(planificacao);
            return CreatedAtAction(nameof(GetPlanificacao), new { id = newPlanificacao.Id }, newPlanificacao);
        }

        [HttpPut]
        public async Task<ActionResult<SpotPlanificacao>> Putplanificacao(int id, [FromBody] SpotPlanificacao planificacao)
        {
            if (id != planificacao.Id)
            {
                return BadRequest();
            }

            await _planificacaoRepository.Update(planificacao);

            return NoContent();
        }

        [Route("parecerLocutor/{id}/{estado}")]
        [HttpPost]
        public async Task<ActionResult<SpotPlanificacao>> PutplanificacaoLocutorBy([FromRoute] int id, [FromRoute] int estado, [FromBody] SpotPlanificacao planificacao)
        {
            //Estados 1 - Pendente; 2 - Skip; 3 - Transmitido; 4 - Anuncio a Espera de parecer Coordenador; 5 - Inactivo

            var getProcesso = await _planificacaoRepository.Get(id);
            var userLog = await _userManager.FindByEmailAsync(User.FindFirst("Email").Value);
            var roles = await _userManager.GetRolesAsync(userLog);

            if (getProcesso == null)
                return NotFound();

            if (estado == 2)
            {
                planificacao.DataSkip = planificacao.DataPassagem;
                planificacao.Estado = estado;
                planificacao.DataPassagem = null;
            }
            else
            {
                planificacao.Estado = 3;
            }

            if (planificacao.SkipMotivo == "N/A")
                planificacao.SkipMotivo = null;

            planificacao.UserLocutor = roles.FirstOrDefault() + " - " + User.FindFirst("Nome").Value + " " + User.FindFirst("Apelido").Value;

            IEnumerable<Usuario> existeCoordenador = _usuariosRepository.GetAllUserCanalsBy(getProcesso.IdCanal.Value);
            if (existeCoordenador.Count() < 1)
            {
                planificacao.UserCoordenador = roles.FirstOrDefault() + " - " + User.FindFirst("Nome").Value + " " + User.FindFirst("Apelido").Value;

                if (planificacao.SkipMotivo == "N/A")
                    planificacao.ParecerCoordenador = null;
                else
                    planificacao.ParecerCoordenador = planificacao.SkipMotivo;

                planificacao.DataPassagemConfirmacao = DateTime.Now;

            }
            await _planificacaoRepository.UpdatePlanificacao(id, planificacao);

            IEnumerable<SpotPlanificacao> pl = _planificacaoRepository.GetAllPlanificacaoByToday(getProcesso.IdCanal.Value);
            var result = MapPlanificacaoToPlanificacaoesDto(pl);

            return Ok(result);
        }

        [Route("parecerLocutorNew/{id}/{estado}")]
        [HttpPost]
        public async Task<ActionResult<SpotPlanificacao>> PutplanificacaoLocutorNewBy([FromRoute] int id, [FromRoute] int estado, [FromBody] SpotPlanificacao planificacao)
        {
            //Estados 1 - Pendente; 2 - Skip; 3 - Transmitido; 4 - Anuncio a Espera de parecer Coordenador; 5 - Inactivo

            var getProcesso = await _planificacaoRepository.Get(id);
            var userLog = await _userManager.FindByEmailAsync(User.FindFirst("Email").Value);
            var roles = await _userManager.GetRolesAsync(userLog);

            if (getProcesso == null)
                return NotFound();

            if (estado == 2)
            {
                planificacao.DataSkip = planificacao.DataPassagem;
                planificacao.Estado = estado;
                planificacao.DataPassagem = null;
            }
            else
            {
                planificacao.Estado = 4;
            }

            if (planificacao.SkipMotivo == "N/A")
                planificacao.SkipMotivo = null;
            

            planificacao.UserLocutor = roles.FirstOrDefault() + " - " + User.FindFirst("Nome").Value +" "+ User.FindFirst("Apelido").Value;

            IEnumerable<Usuario> existeCoordenador = _usuariosRepository.GetAllUserCanalsBy(getProcesso.IdCanal.Value);
            if (existeCoordenador.Count() < 1)
            {
                planificacao.UserCoordenador = roles.FirstOrDefault() + " - " + User.FindFirst("Nome").Value + " " + User.FindFirst("Apelido").Value;

                if (planificacao.SkipMotivo == "N/A")
                    planificacao.ParecerCoordenador = null;
                else
                    planificacao.ParecerCoordenador = planificacao.SkipMotivo;
                
                planificacao.DataPassagemConfirmacao = DateTime.Now;

            }
            await _planificacaoRepository.UpdatePlanificacao(id, planificacao);

            IEnumerable<SpotPlanificacao> pl = _planificacaoRepository.GetAllPlanificacaoByToday(getProcesso.IdCanal.Value);
            var result = MapPlanificacaoToPlanificacaoesDto(pl);

            return Ok(result);
        }

        [Route("parecerCoordenador/{id}/{estado}")]
        [HttpPost]
        public async Task<ActionResult<SpotPlanificacao>> PutplanificacaoCoordenadorBy([FromRoute] int id, [FromRoute] int estado, [FromBody] SpotPlanificacao planificacao)
        {
            //Estados 1 - Pendente; 2 - Skip; 3 - Transmitido; 4 - Anuncio a Espera de parecer Coordenador; 5 - Inactivo

            var getProcesso = await _planificacaoRepository.Get(id);
            var userLog = await _userManager.FindByEmailAsync(User.FindFirst("Email").Value);
            var roles = await _userManager.GetRolesAsync(userLog);

            if (getProcesso == null)
                return NotFound();

            if (estado == 2)
            {
                planificacao.DataSkip = DateTime.Now;
                planificacao.Estado = estado;

                if (getProcesso.IdSpotNavigation.IdClienteNavigation.EMail != null)
                {
                    EmailRequest email = new EmailRequest()
                    {
                        ToEmail = getProcesso.IdSpotNavigation.IdClienteNavigation.EMail,
                        Body = $@"<div style='font-family:Roboto-Regular,Helvetica,Arial,sans-serif;'>
                    <p>PREZADA(O) <b>{getProcesso.IdSpotNavigation.IdClienteNavigation.Designacao}</b></p>
                    <p>{planificacao.ParecerCoordenador}</p>
                    <br/><br/><br/>
                    <div style='color: rgba(0, 0, 0, 0.54); font-size:11px; line-height:18px; padding-top:12px;'>
                        <div>
                             Recebeu este email para o(a) informar acerca de alterações importantes efetuadas aos serviços.
                        </div>
                        <div style ='direction:ltr'>
                            © {DateTime.Now.Year} SGSP - Rádio de Moçambique
                        </div>
                    </div>
                </div>
                ",
                        Subject = "ALERTA DE NÃO TRANSMISSÃO PARA A CAMPANHA " + getProcesso.IdSpotNavigation.Designacao.ToUpper()
                    };

                    await _emailController.Send(email);
                }

            }
            else
            {
                planificacao.Estado = estado;
                planificacao.DataPassagemConfirmacao = DateTime.Now;
            }


            if (planificacao.ParecerCoordenador == "N/A")
                planificacao.ParecerCoordenador = null;

            planificacao.UserCoordenador = roles.FirstOrDefault() + " - " + User.FindFirst("Nome").Value +" "+ User.FindFirst("Apelido").Value;
            planificacao.UserLocutor = getProcesso.UserLocutor;
            planificacao.SkipMotivo = getProcesso.SkipMotivo;
            planificacao.DataPassagem = getProcesso.DataPassagem;

            await _planificacaoRepository.UpdatePlanificacao(id, planificacao);

            if (getProcesso.DataPlanificacao.Value.Date == getProcesso.IdSpotNavigation.DataFim.Value.Date)
                await _spotsRepository.UpdateEstado(getProcesso.IdSpot.Value);

            IEnumerable<SpotPlanificacao> pl = _planificacaoRepository.GetAllPlanificacaoByToday(getProcesso.IdCanal.Value);
            var result = MapPlanificacaoToPlanificacaoesDto(pl);

            return Ok(result);
        }

        [Route("parecerCoordenadorNotificacao/{id}")]
        [HttpPost]
        public async Task<ActionResult<SpotPlanificacao>> PutplanificacaoCoordenadorNotificacaoBy([FromRoute] int id, [FromBody] SpotPlanificacao planificacao)
        {
            //Estados 1 - Pendente; 2 - Skip; 3 - Transmitido; 4 - Anuncio a Espera de parecer Coordenador; 5 - Inactivo

            var getProcesso = await _planificacaoRepository.Get(id);
            var userLog = await _userManager.FindByEmailAsync(User.FindFirst("Email").Value);
            var roles = await _userManager.GetRolesAsync(userLog);

            if (getProcesso == null)
                return NotFound();


            if (getProcesso.IdSpotNavigation.IdClienteNavigation.EMail != null)
            {
                EmailRequest email = new EmailRequest()
                {
                    ToEmail = getProcesso.IdSpotNavigation.IdClienteNavigation.EMail,
                    Body = $@"<div style='font-family:Roboto-Regular,Helvetica,Arial,sans-serif;'>
                    <p>PREZADA(O) <b>{getProcesso.IdSpotNavigation.IdClienteNavigation.Designacao}</b></p>
                    <p>{planificacao.ParecerCoordenador}</p>
                    <br/><br/><br/>
                    <div style='color: rgba(0, 0, 0, 0.54); font-size:11px; line-height:18px; padding-top:12px;'>
                        <div>
                             Recebeu este email para o(a) informar acerca de alterações importantes efetuadas aos serviços.
                        </div>
                        <div style ='direction:ltr'>
                            © {DateTime.Now.Year} SGSP - Rádio de Moçambique
                        </div>
                    </div>
                </div>
                ",
                    Subject = "ALERTA DE NÃO TRANSMISSÃO PARA A CAMPANHA " + getProcesso.IdSpotNavigation.Designacao.ToUpper()
                };

                await _emailController.Send(email);
            }

            planificacao.UserCoordenador = roles.FirstOrDefault() + " - " + User.FindFirst("Nome").Value +" "+ User.FindFirst("Apelido").Value;
            planificacao.UserLocutor = getProcesso.UserLocutor;
            planificacao.SkipMotivo = getProcesso.SkipMotivo;
            planificacao.DataPassagem = getProcesso.DataPassagem;
            planificacao.DataSkip = getProcesso.DataSkip;
            planificacao.Estado = getProcesso.Estado;
            planificacao.DataPassagemConfirmacao = getProcesso.DataPassagemConfirmacao;

            await _planificacaoRepository.UpdatePlanificacao(id, planificacao);

            if (getProcesso.DataPlanificacao.Value.Date == getProcesso.IdSpotNavigation.DataFim.Value.Date)
                await _spotsRepository.UpdateEstado(getProcesso.IdSpot.Value);

            IEnumerable<SpotPlanificacao> pl = _planificacaoRepository.GetAllPlanificacaoByToday(getProcesso.IdCanal.Value);
            var result = MapPlanificacaoToPlanificacaoesDto(pl);

            return Ok(result);
        }

        [Route("parecerDep/{id}/{estado}")]
        [HttpPost]
        public async Task<ActionResult<SpotPlanificacao>> PutplanificacaoDepBy([FromRoute] int id, [FromRoute] int estado, [FromBody] SpotPlanificacao planificacao)
        {
            //Estados 1 - Pendente; 2 - Skip; 3 - Transmitido; 4 - Anuncio a Espera de parecer Coordenador; 5 - Inactivo

            var getProcesso = await _planificacaoRepository.Get(id);
            var userLog = await _userManager.FindByEmailAsync(User.FindFirst("Email").Value);
            var roles = await _userManager.GetRolesAsync(userLog);

            if (getProcesso == null)
                return NotFound();

            if (estado == 2)
            {
                planificacao.DataSkip = DateTime.Now;
                planificacao.Estado = estado;
            }
            else
            {
                planificacao.Estado = estado;
                planificacao.DataPassagemConfirmacao = DateTime.Now;
            }


            if (planificacao.ParecerCoordenador == "N/A")
                planificacao.ParecerCoordenador = null;

            planificacao.UserCoordenador = roles.FirstOrDefault() + " - " + User.FindFirst("Nome").Value + " " + User.FindFirst("Apelido").Value;
            planificacao.UserLocutor = getProcesso.UserLocutor;
            planificacao.SkipMotivo = getProcesso.SkipMotivo;
            planificacao.DataPassagem = getProcesso.DataPassagem;
            planificacao.DataPassagemConfirmacao = DateTime.Now;

            await _planificacaoRepository.UpdatePlanificacao(id, planificacao);

            if(getProcesso.DataPlanificacao.Value.Date ==  getProcesso.IdSpotNavigation.DataFim.Value.Date)
                await _spotsRepository.UpdateEstado(getProcesso.IdSpot.Value);

            IEnumerable<SpotPlanificacao> pl = _planificacaoRepository.GetAllPlanificacaoBy(getProcesso.IdCanal.Value);
            var result = MapPlanificacaoToPlanificacaoesDto(pl);

            return Ok(result);
        }

        [Route("reagendamento/{id}")]
        [HttpPost]
        public async Task<ActionResult<SpotPlanificacao>> PutChangeEstadoBy([FromRoute] int id, [FromBody] SpotPlanificacao processo)
        {
            //Estados 1 - Pendente; 2 - Skip; 3 - Transmitido; 4 - Anuncio a Espera de parecer Coordenador; 5 - Inactivo

            DateTime dataPlan, dataPrioridade;

            var getPlan = await _planificacaoRepository.Get(id);

            if (getPlan == null)
                return NotFound();

            await _planificacaoRepository.UpdatePlanificacaoEstado(id);

            var getLastProcesso = await _planificacaoRepository.GetLastBy(getPlan.IdCanal.Value, getPlan.IdJanela.Value, processo.DataPlanificacao);

            if (getLastProcesso == null)
            {
                var getJanela = await _janelasRepository.GetBy(getPlan.IdJanela.Value);
                dataPlan = TimeCombined(processo.DataPlanificacao.Value, getJanela.HoraInicio.Value, getPlan.IdSpotNavigation.Duracao.Value);
                dataPrioridade = TimeCombined(processo.DataPlanificacao.Value, getJanela.HoraInicio.Value, getPlan.IdSpotNavigation.Duracao.Value);
            }
            else
            {
                dataPlan = TimeCombined(processo.DataPlanificacao.Value, getLastProcesso.DataPlanificacao.Value.TimeOfDay, getPlan.IdSpotNavigation.Duracao.Value);
                dataPrioridade = TimeCombined(processo.DataPlanificacao.Value, getLastProcesso.Prioridade.Value.TimeOfDay, getPlan.IdSpotNavigation.Duracao.Value);
            }

            SpotPlanificacao newPlanificacao = new SpotPlanificacao()
            {
                IdSpot = getPlan.IdSpot,
                IdJanela = getPlan.IdJanela,
                IdCanal = getPlan.IdCanal,
                DataPlanificacao = dataPlan,
                Estado = 1,
                IsReagendamento = true,
                Prioridade = Time30sCombined(dataPrioridade, dataPrioridade.TimeOfDay),
                IdSpotPlanificacaoPrincipal = getPlan.Id
            };

            await _planificacaoRepository.Create(newPlanificacao);

            IEnumerable<SpotPlanificacao> pl = _planificacaoRepository.GetAllPlanificacaoBy(getPlan.IdCanal.Value);
            var result = MapPlanificacaoToPlanificacaoesDto(pl);

            return Ok(result);
        }

        [Route("orderPlanificacao/{id}")]
        [HttpPost]
        public async Task<ActionResult<SpotPlanificacao>> PutPlanificacaoOrder([FromRoute] int id, [FromBody] SpotPlanificacao processo)
        {
            var getPlan = await _planificacaoRepository.Get(id);

            if (getPlan == null)
                return NotFound();

            await _planificacaoRepository.OrderPlanificacao(getPlan.Id, processo.DataPlanificacao.Value);

            return Ok();
        }

        [Route("orderPlanificacaoMonth/{id}")]
        [HttpPost]
        public async Task<ActionResult<SpotPlanificacao>> PutPlanificacaoOrderMonth([FromRoute] int id, [FromBody] SpotPlanificacao processo)
        {
            //Estados 1 - Pendente; 2 - Skip; 3 - Transmitido; 4 - Anuncio a Espera de parecer Coordenador; 5 - Inactivo

            var getPlan = await _planificacaoRepository.Get(id);

            if (getPlan == null)
                return NotFound();

            DateTime newDateP = processo.Prioridade.Value;
            DateTime newDateD = processo.DataPlanificacao.Value;

            newDateD = newDateD.Add(getPlan.DataPlanificacao.Value.TimeOfDay);
            newDateP = newDateP.Add(getPlan.Prioridade.Value.TimeOfDay);

            await _planificacaoRepository.OrderPlanificacaoDrag(getPlan.Id, newDateP, newDateD);

            SpotPlanificacao historyPlan = new SpotPlanificacao()
            {
                IdSpot = getPlan.IdSpot,
                IdJanela = getPlan.IdJanela,
                DataPlanificacao = getPlan.DataPlanificacao,
                Estado = 5,
                IdCanal = getPlan.IdCanal,
                Prioridade = getPlan.Prioridade,
                IsReagendamento = false,
                IdSpotPlanificacaoPrincipal = getPlan.Id
            };

            await _planificacaoRepository.Create(historyPlan);

            var getProcesso = await _spotsRepository.Get(getPlan.IdSpot);

            if (getProcesso.DataFim.Value.Date < newDateD.Date)
            {
                await _spotsRepository.UpdateSpotDate(getProcesso.Id, newDateD.Date);

                Spot spotHistory = new Spot()
                {
                    Designacao = getProcesso.Designacao,
                    Code = getProcesso.Code,
                    DataFim = getProcesso.DataFim,
                    DataInicio = getProcesso.DataInicio,
                    Duracao = getProcesso.Duracao,
                    IdCanal = getProcesso.IdCanal,
                    Periodicidade = getProcesso.Periodicidade,
                    IsActive = false,
                    IdSpotPrincipal = getProcesso.Id,
                    IdTipoSpot = getProcesso.IdTipoSpot,
                    DataCreate = DateTime.Now,
                    Estado = 2

                };

                await _spotsRepository.Create(spotHistory);
            }



            return Ok();
        }

        //[HttpPut("{id}")]
        //public async Task<ActionResult> PatchplanificacaoBy(int id, [FromBody] SpotPlanificacao planificacao)
        //{
        //    await _planificacaoRepository.UpdatePlanificacao(id, planificacao);
        //    return Ok();
        //}

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var planificacaoToDelete = await _planificacaoRepository.Get(id);

            if (planificacaoToDelete == null)
                return NotFound();

            await _planificacaoRepository.Delete(planificacaoToDelete.Id);
            return NoContent();
        }

        [HttpGet("listPlanificacao")]
        public IActionResult GetAllPlanificacaoList()
        {
            IEnumerable<SpotPlanificacao> planificacao = _planificacaoRepository.GetAllPlanificacao();
            var result = MapPlanificacaoToPlanificacaoesDto(planificacao);

            return Ok(result);
        }

        [HttpGet("listPlanificacaoByIdCanalUSer")]
        public IActionResult GetAllPlanificacaoListByIdCanalUser()
        {
            var idCanal = int.Parse(User.FindFirst("IdCanal").Value);
            IEnumerable<SpotPlanificacao> planificacao = _planificacaoRepository.GetAllPlanificacaoBy(idCanal);
            var result = MapPlanificacaoToPlanificacaoesDto(planificacao);

            return Ok(result);
        }

        [HttpGet("listPlanificacaoBy/{idCanal}")]
        public IActionResult GetAllPlanificacaoListBy(int idCanal)
        {
            IEnumerable<SpotPlanificacao> planificacao = _planificacaoRepository.GetAllPlanificacaoBy(idCanal);
            var result = MapPlanificacaoToPlanificacaoesDto(planificacao);

            return Ok(result);
        }

        [HttpGet("listPlanificacaoToday")]
        public IActionResult GetAllPlanificacaoListToday()
        {
            var idCanal = int.Parse(User.FindFirst("IdCanal").Value);

            IEnumerable<SpotPlanificacao> planificacao = _planificacaoRepository.GetAllPlanificacaoByToday(idCanal);
            var result = MapPlanificacaoToPlanificacaoesDto(planificacao);

            return Ok(result);
        }

        [HttpGet("listPlanificacaoTodayBy/{idCanal}")]
        public IActionResult GetAllPlanificacaoListTodayBy(int idCanal)
        {
            IEnumerable<SpotPlanificacao> planificacao = _planificacaoRepository.GetAllPlanificacaoByToday(idCanal);
            var result = MapPlanificacaoToPlanificacaoesDto(planificacao);

            return Ok(result);
        }

        [HttpGet("listPlanificacaoLocutorToday")]
        public IActionResult GetAllPlanificacaoLocutorListToday()
        {
            var idCanal = int.Parse(User.FindFirst("IdCanal").Value);

            IEnumerable<SpotPlanificacao> planificacao = _planificacaoRepository.GetAllPlanificacaoLocutorByToday(idCanal);
            var result = MapPlanificacaoToPlanificacaoesDto(planificacao);

            return Ok(result);
        }

        [HttpGet("listPlanificacaoLocutorTodayBy/{idCanal}")]
        public IActionResult GetAllPlanificacaoLocutorListTodayBy(int idCanal)
        {
            IEnumerable<SpotPlanificacao> planificacao = _planificacaoRepository.GetAllPlanificacaoLocutorByToday(idCanal);
            var result = MapPlanificacaoToPlanificacaoesDto(planificacao);

            return Ok(result);
        }

        [HttpGet("listPlanificacaoCoordenadorToday")]
        public IActionResult GetAllPlanificacaoCoordenadorListToday()
        {
            var idCanal = int.Parse(User.FindFirst("IdCanal").Value);

            IEnumerable<SpotPlanificacao> planificacao = _planificacaoRepository.GetAllPlanificacaoCoordenadasByToday(idCanal);
            var result = MapPlanificacaoToPlanificacaoesDto(planificacao);

            return Ok(result);
        }

        [HttpGet("listPlanificacaoCoordenadorTodayBy/{idCanal}")]
        public IActionResult GetAllPlanificacaoCoordenadorListTodayBy(int idCanal)
        {
            IEnumerable<SpotPlanificacao> planificacao = _planificacaoRepository.GetAllPlanificacaoCoordenadasByToday(idCanal);
            var result = MapPlanificacaoToPlanificacaoesDto(planificacao);

            return Ok(result);
        }

        [HttpGet("listPlanificacaoClienteBy/{idCliente}")]
        public IActionResult GetAllPlanificacaoClienteBy(string idCliente)
        {
            var cliente = _usuariosRepository.GetUsurioBy(idCliente);
            if (cliente.Result == null)
            {
                var resulted = MapPlanificacaoToPlanificacaoesDto(new SpotPlanificacao());
                return Ok(new { data = resulted });
            }


            IEnumerable<SpotPlanificacao> planificacao = _planificacaoRepository.GetAllPlanificacaoClienteBy(cliente.Result.IdCliente.Value);
            var result = MapPlanificacaoToPlanificacaoesDtoCliente(planificacao);

            return Ok(new { data = result });
        }

        [HttpGet("listPlanificacaoClienteFilterBy/{idCliente}/{idSpot}/{dataI}/{dataF}")]
        public IActionResult GetAllPlanificacaoClienteFiltreBy(int idCliente, int idSpot, DateTime? dataI, DateTime? dataF)
        {
            Filter filter = new Filter()
            {
                IdCliente = idCliente,
                IdSpot = idSpot,
                DataF = dataF,
                DataI = dataI
            };

            IEnumerable<SpotPlanificacao> planificacao = _planificacaoRepository.GetAllPlanificacaoClienteFilterBy(filter);
            var result = MapPlanificacaoToPlanificacaoesDtoCliente(planificacao);

            return Ok(new { data = result });
        }
        private IEnumerable<PlanificacaoDto> MapPlanificacaoToPlanificacaoesDto(IEnumerable<SpotPlanificacao> planificacaos)
        {

            IEnumerable<PlanificacaoDto> result;
            result = planificacaos.Select(p => new PlanificacaoDto()
            {
                Id = p.Id,
                Code = p.IdSpotNavigation.Code,
                Designacao = p.IdSpotNavigation.Designacao,
                DataInicio = p.IdSpotNavigation.DataInicio,
                DataFim = p.IdSpotNavigation.DataFim,
                DataPassagem = p.DataPassagem,
                DataPassagemConfrimacao = p.DataPassagemConfirmacao,
                DataPlanificacao = p.DataPlanificacao,
                DataSkip = p.DataSkip,
                SkipMotivo = p.SkipMotivo,
                Cliente = p.IdSpotNavigation.IdClienteNavigation.Designacao,
                Duracao = p.IdSpotNavigation.Duracao,
                IsReagendamento = p.IsReagendamento,
                IdTipo = p.IdSpotNavigation.IdTipoSpot,
                TipoProcesso = p.IdSpotNavigation.IdTipoSpotNavigation.Designacao,
                Prioridade = p.Prioridade,
                IdPlanificacaoPrincipal = p.IdSpotPlanificacaoPrincipal,
                Estado = p.Estado,
                UserCoordenador = p.UserCoordenador,
                UserLocutor = p.UserLocutor,
                DataCreateProcesso = p.IdSpotNavigation.DataCreate,
                CoordenadorParecer = p.ParecerCoordenador,
                IdCanal = p.IdCanal,
                CanalDesignacao = p.IdCanalNavigation.Designacao,
                EstadoHTML = EstadoHTML(p.Estado.Value, p.ParecerCoordenador, ProcessoTipo(p.IdSpotNavigation.IdTipoSpotNavigation.Designacao, p.IdSpotNavigation.Designacao)),
                DesignacaoProcesso = ProcessoTipo(p.IdSpotNavigation.IdTipoSpotNavigation.Designacao, p.IdSpotNavigation.Designacao),
                LastDate = null
            });

            return result;
        }
        private IEnumerable<PlanificacaoDto> MapPlanificacaoToPlanificacaoesDtoCliente(IEnumerable<SpotPlanificacao> planificacaos)
        {

            IEnumerable<PlanificacaoDto> result;
            result = planificacaos.Select(p => new PlanificacaoDto()
            {
                Id = p.Id,
                Code = p.IdSpotNavigation.Code,
                Designacao = p.IdSpotNavigation.Designacao,
                DataInicio = p.IdSpotNavigation.DataInicio,
                DataFim = p.IdSpotNavigation.DataFim,
                DataPassagem = p.DataPassagem,
                DataPassagemConfrimacao = p.DataPassagemConfirmacao,
                DataPlanificacao = p.DataPlanificacao,
                DataSkip = p.DataSkip,
                SkipMotivo = p.SkipMotivo,
                Cliente = p.IdSpotNavigation.IdClienteNavigation.Designacao,
                Duracao = p.IdSpotNavigation.Duracao,
                IsReagendamento = p.IsReagendamento,
                IdTipo = p.IdSpotNavigation.IdTipoSpot,
                TipoProcesso = p.IdSpotNavigation.IdTipoSpotNavigation.Designacao,
                Prioridade = p.Prioridade,
                IdPlanificacaoPrincipal = p.IdSpotPlanificacaoPrincipal,
                Estado = p.Estado,
                UserCoordenador = p.UserCoordenador,
                UserLocutor = p.UserLocutor,
                DataCreateProcesso = p.IdSpotNavigation.DataCreate,
                CoordenadorParecer = p.ParecerCoordenador,
                IdCanal = p.IdCanal,
                CanalDesignacao = p.IdCanalNavigation.Designacao,
                EstadoHTML = EstadoHTML(p.Estado.Value, p.ParecerCoordenador, ProcessoTipo(p.IdSpotNavigation.IdTipoSpotNavigation.Designacao, p.IdSpotNavigation.Designacao)),
                DesignacaoProcesso = ProcessoTipo(p.IdSpotNavigation.IdTipoSpotNavigation.Designacao, p.IdSpotNavigation.Designacao),
                LastDate = DataPassagem(p.DataSkip, p.DataPassagemConfirmacao)
            });

            return result;
        }

        private DateTime DataPassagem(DateTime? dataSkip, DateTime? dataPassagem)
        {
            if (dataSkip != null)
                return dataSkip.Value;
            else if (dataPassagem != null)
                return dataPassagem.Value;
            else
                return DateTime.Now;
        }
        private ActionResult<SpotPlanificacao> MapPlanificacaoToPlanificacaoesDto(SpotPlanificacao p)
        {
            PlanificacaoTimeDto result = new PlanificacaoTimeDto()
            {
                DateProcesso = p.DataPlanificacao,
                JanelaOpen = p.IdJanelaNavigation.HoraInicio,
                JanelaEnd = p.IdJanelaNavigation.HoraFim
            };

            return Ok(result);
        }

        private ActionResult<SpotPlanificacao> MapJanelasToJanelasDto(Janela j)
        {
            JanelasDto result = new JanelasDto()
            {
                Id = j.Id,
                Designacao = j.Designacao,
                HoraInicio = j.HoraInicio,
                HoraFim = j.HoraFim,
                DiasSemana = j.DiasSemana,
                IdCanal = j.IdCanal,
                IsActive = j.IsActive
            };

            return Ok(result);
        }

        private string ProcessoTipo(string tipo, string designacao)
        {
            return tipo + " - " + designacao;
        }
        private string EstadoHTML(int estado, string parecer, string designacao)
        {
            string result;
            if (estado == 2)
                 result = "<a class='view-parecer' data-designacao='" + designacao + "' data-parecer='" + parecer + "'><span class='badge bg-danger'>Não Transmitido</span></a>";
            else if (estado == 3)
                 result = "<span class='badge bg-success'>Transmitido</span>";
            else if (estado == 1)
                 result = "<span class='badge bg-primary'>Pendente</span>";
            else
                 result = "";

            return result;
        }
        private DateTime TimeCombined(DateTime dataProcesso, TimeSpan lastProcessoTime, TimeSpan duracaoNewProcesso)
        {

            string[] lastProcesso = lastProcessoTime.ToString().Split(':');
            string[] duracaoProcesso = duracaoNewProcesso.ToString().Split(':');

            TimeSpan time1 = new TimeSpan(Int32.Parse(lastProcesso[0]), Int32.Parse(lastProcesso[1]), Int32.Parse(lastProcesso[2]));
            TimeSpan time2 = new TimeSpan(Int32.Parse(duracaoProcesso[0]), Int32.Parse(duracaoProcesso[1]), Int32.Parse(duracaoProcesso[2]));

            TimeSpan time = time1 + time2;

            DateTime combined = dataProcesso.Add(time);

            return combined;
        }

        private DateTime Time30sCombined(DateTime dataProcesso, TimeSpan lastProcessoTime)
        {

            TimeSpan time = new TimeSpan(0, 30, 0);

            DateTime combined = dataProcesso.Add(time);

            return combined;
        }

        private int CompareDates (DateTime dataProcesso, DateTime dataPlanificacao)
        {
            int result = DateTime.Compare(dataProcesso, dataPlanificacao);
            int state;

            if (result < 0)
                state = 0;
            else if (result == 0)
                state = 1;
            else
                state = 2;

            return state;
        }
    }
}
