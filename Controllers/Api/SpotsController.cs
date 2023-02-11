using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SGSP.Dtos;
using SGSP.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SGSP.Models;
using System.Security.Claims;

namespace SGSP.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotsController : ControllerBase
    {
        private readonly ISpotsRepository _spotsRepository;
        private readonly IPlanificacaoRepository _planificacaoRepository;
        private readonly IJanelasRepository _janelasRepository;
        private readonly IUsuariosRepository _usuariosRepository;
        private readonly IClientesRepository _clientesRepository;
        private readonly EmailController _emailController;

        public SpotsController(ISpotsRepository spotsRepository, IPlanificacaoRepository planificacaoRepository,
            IJanelasRepository janelasRepository, IUsuariosRepository usuariosRepository, IClientesRepository clientesRepository,
            EmailController emailController)
        {
            _spotsRepository = spotsRepository;
            _planificacaoRepository = planificacaoRepository;
            _janelasRepository = janelasRepository;
            _usuariosRepository = usuariosRepository;
            _clientesRepository = clientesRepository;
            _emailController = emailController;
        }

        [HttpGet]
        public async Task<IEnumerable<Spot>> GetSpots()
        {
            return await _spotsRepository.Get();
        }

        [HttpGet("getAll")]
        public IEnumerable<Spot> Get()
        {
            return _spotsRepository.GetAllSpots();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Spot>> GetSpotsBy(int id)
        {
            return await _spotsRepository.Get(id);
        }

        [HttpGet("getBy/{code}")]
        public async Task<ActionResult<Spot>> GetSpotBy(string code)
        {

            var spot = await _spotsRepository.GetSpotBy(code);

            if (spot == null)
                return Ok(new { d = "-1" });

            return spot;
        }

        public async Task<Spot> UpdateEstadoSpot()
        {
            var spots = _spotsRepository.GetAllSpotsEstado();
            var planificacoes = _planificacaoRepository.GetAllPlanificacaoEstado();

            if (spots != null)
                foreach (var spot in spots)
                    await _spotsRepository.UpdateSpotEstado(spot);

            if (planificacoes != null)
                foreach (var planificacao in planificacoes)
                    await _planificacaoRepository.UpdatePlanificacaoEstado(planificacao);
            return spots.FirstOrDefault();
        }

        private async Task<Spot> SendEmailCredenciaisAcesso(string to, string name)
        {
            EmailRequest email = new EmailRequest()
            {
                ToEmail = to,
                Body = $@"<div style='font-family: Roboto-Regular,Helvetica,Arial,sans-serif;'>
                    <p>PREZADA(O) <b>{name}</b></p>
                    <p>
                        A sua conta foi activada com sucesso. Pode aceder ao Sistema de Gestão de Spots Publicitarios da Rádio de Moçambique pelo link&nbsp; 
                        <a href='http://sgrm.dev.lotuscomputer.co.mz/' target='_blank'>
                            http://sgrm.dev.lotuscomputer.co.mz/ 
                        </a>
                        <span>
                            &nbsp;,e de seguida introduzir as seguintes credenciais:
                        </span>
                    </p>
                    <ul>
                        <li>
                            <b>EMAIL: {to}</b>
                        </li>
                        <li>
                            <b>SENHA: SGSP@2022</b>
                        </li>
                    </ul>
                    <br/><br/><br/>
                    <div style='font - family:Roboto-Regular,Helvetica,Arial,sans-serif; color: rgba(0, 0, 0, 0.54); font-size:11px; line-height:18px; padding - top:12px;'>
                        <div>
                             Recebeu este email para o(a) informar acerca de alterações importantes efetuadas aos serviços.
                        </div>
                        <div style ='direction:ltr'>
                            © {DateTime.Now.Year} SGSP - Rádio de Moçambique
                        </div>
                    </div>
                </div>
                ",
                Subject = "RÁDIO DE MOÇAMBIQUE: CREDENCIAIS DE ACESSO!"
            };

            await _emailController.Send(email);

            return null;

        }

        [HttpPost]
        public async Task<ActionResult<Spot>> PostSpots([FromForm] Spot spots, [FromForm] SpotPlanificacao[] list)
        {
            DateTime dataPlan, dataPrioridade;
            var existCliente = await _spotsRepository.GetSpotClienteBy(spots.IdCliente.Value);

            if (existCliente == null)
            {
                var getCliente = await _clientesRepository.Get(spots.IdCliente.Value);
                if (getCliente.EMail != null)
                    await SendEmailCredenciaisAcesso(getCliente.EMail, getCliente.Designacao);
            }


            spots.IsActive = true;
                spots.DataCreate = DateTime.Now;
                spots.IdTipoSpot = 1;
                spots.Estado = 1;

            //Generate Code Spot
            var getSpotsCode = await _spotsRepository.GetlastProcessoBy(1);
            if (getSpotsCode != null)
                spots.Code = CodeGeneratorAsync(1, getSpotsCode.Code);
            else
                spots.Code = CodeGeneratorAsync(1, "");

            var newSpot = await _spotsRepository.Create(spots);

                //await _planificacaoRepository.CreateList(list);

                foreach (var item in list)
                {
                    int n = 0;

                    do
                    {
                        var getLastProcesso = await _planificacaoRepository.GetLastBy(item.IdCanal.Value, item.IdJanela.Value, item.DataPlanificacao.Value);

                        if (getLastProcesso == null)
                        {
                            var getJanela = await _janelasRepository.GetBy(item.IdJanela.Value);
                            dataPlan = TimeCombined(item.DataPlanificacao.Value, getJanela.HoraInicio.Value, spots.Duracao.Value);
                            dataPrioridade = TimeCombined(item.DataPlanificacao.Value, getJanela.HoraInicio.Value, spots.Duracao.Value);
                        }
                        else
                        {
                            dataPlan = TimeCombined(item.DataPlanificacao.Value, getLastProcesso.DataPlanificacao.Value.TimeOfDay, spots.Duracao.Value);
                            dataPrioridade = TimeCombined(item.DataPlanificacao.Value, getLastProcesso.Prioridade.Value.TimeOfDay, spots.Duracao.Value);
                        }

                        SpotPlanificacao spotPlanificacao = new SpotPlanificacao()
                        {
                            IdSpot = newSpot.Id,
                            IdJanela = item.IdJanela,
                            IdCanal = item.IdCanal,
                            DataPlanificacao = dataPlan,
                            Estado = 1,
                            IsReagendamento = false,
                            Prioridade = Time30sCombined(dataPlan, dataPlan.TimeOfDay)
                        };

                        await _planificacaoRepository.Create(spotPlanificacao);

                        n++;

                    } while (n < item.Estado);
                }

                return CreatedAtAction(nameof(GetSpots), new { idSpot = newSpot.Id }, newSpot);

        }



        [HttpPost("anuncio/{id}")]
        public async Task<ActionResult<Spot>> PostAnuncio([FromForm] Spot anuncio, [FromForm] SpotPlanificacao[] list)
        {
            DateTime dataPlan, dataPrioridade;

            var existCliente = await _spotsRepository.GetSpotClienteBy(anuncio.IdCliente.Value);

            if (existCliente == null)
            {
                var getCliente = await _clientesRepository.Get(anuncio.IdCliente.Value);
                if (getCliente.EMail != null)
                    await SendEmailCredenciaisAcesso(getCliente.EMail, getCliente.Designacao);
            }

            anuncio.IsActive = true;
                anuncio.DataCreate = DateTime.Now;
                anuncio.IdTipoSpot = 2;
                anuncio.DataInicio = anuncio.DataFim;
                anuncio.Estado = 1;

            //Generate Code Anúncio
            var getAnuncioCode = await _spotsRepository.GetlastProcessoBy(2);
            if(getAnuncioCode != null)
                anuncio.Code = CodeGeneratorAsync(2, getAnuncioCode.Code);
            else
                anuncio.Code = CodeGeneratorAsync(2, "");

            var newAnuncio = await _spotsRepository.Create(anuncio);
                //await _planificacaoRepository.CreateList(list);

                foreach (var item in list)
                {
                    int n = 0;

                    do
                    {
                        var getLastProcesso = await _planificacaoRepository.GetLastBy(item.IdCanal.Value, item.IdJanela.Value, item.DataPlanificacao.Value);

                        if (getLastProcesso == null)
                        {
                            var getJanela = await _janelasRepository.GetBy(item.IdJanela.Value);
                            dataPlan = TimeCombined(item.DataPlanificacao.Value, getJanela.HoraInicio.Value, anuncio.Duracao.Value);
                            dataPrioridade = TimeCombined(item.DataPlanificacao.Value, getJanela.HoraInicio.Value, anuncio.Duracao.Value);
                        }
                        else
                        {
                            dataPlan = TimeCombined(item.DataPlanificacao.Value, getLastProcesso.DataPlanificacao.Value.TimeOfDay, anuncio.Duracao.Value);
                            dataPrioridade = TimeCombined(item.DataPlanificacao.Value, getLastProcesso.Prioridade.Value.TimeOfDay, anuncio.Duracao.Value);
                        }

                        SpotPlanificacao anuncioPlanificacao = new SpotPlanificacao()
                        {
                            IdSpot = newAnuncio.Id,
                            IdJanela = item.IdJanela,
                            IdCanal = item.IdCanal,
                            DataPlanificacao = dataPlan,
                            Estado = 1,
                            IsReagendamento = false,
                            Prioridade = Time30sCombined(dataPrioridade, dataPrioridade.TimeOfDay)
                        };

                        await _planificacaoRepository.Create(anuncioPlanificacao);

                        n++;

                    } while (n < item.Estado);
                }

                return CreatedAtAction(nameof(GetSpots), new { idAnuncio = newAnuncio.Id }, newAnuncio);

        }

        [HttpPost("locutorAnuncio")]
        public async Task<ActionResult<Spot>> PostSingleAnuncio([FromForm] Spot anuncio, [FromForm] SpotPlanificacao[] list)
        {
            DateTime dataPlan, dataPrioridade;

                anuncio.IsActive = true;
                anuncio.DataCreate = DateTime.Now;
                anuncio.IdTipoSpot = 2;
                anuncio.IdCanal = 1;

            //Generate Code Anúncio
            var getAnuncioCode = await _spotsRepository.GetlastProcessoBy(2);
            if (getAnuncioCode != null)
                anuncio.Code = CodeGeneratorAsync(2, getAnuncioCode.Code);
            else
                anuncio.Code = CodeGeneratorAsync(2, "");

            var newAnuncio = await _spotsRepository.Create(anuncio);

                foreach (var item in list)
                {
                    int n = 0;

                    do
                    {
                        var getLastProcesso = await _planificacaoRepository.GetLastBy(item.IdCanal.Value, item.IdJanela.Value, item.DataPlanificacao.Value);

                        if (getLastProcesso == null)
                        {
                            var getJanela = await _janelasRepository.GetBy(item.IdJanela.Value);
                            dataPlan = TimeCombined(item.DataPlanificacao.Value, getJanela.HoraInicio.Value, anuncio.Duracao.Value);
                            dataPrioridade = TimeCombined(item.DataPlanificacao.Value, getJanela.HoraInicio.Value, anuncio.Duracao.Value);
                        }
                        else
                        {
                            dataPlan = TimeCombined(item.DataPlanificacao.Value, getLastProcesso.DataPlanificacao.Value.TimeOfDay, anuncio.Duracao.Value);
                            dataPrioridade = TimeCombined(item.DataPlanificacao.Value, getLastProcesso.Prioridade.Value.TimeOfDay, anuncio.Duracao.Value);
                        }

                        SpotPlanificacao anuncioPlanificacao = new SpotPlanificacao()
                        {
                            IdSpot = newAnuncio.Id,
                            IdJanela = item.IdJanela,
                            IdCanal = item.IdCanal,
                            DataPlanificacao = dataPlan,
                            Estado = 4,
                            IsReagendamento = false,
                            Prioridade = Time30sCombined(dataPrioridade, dataPrioridade.TimeOfDay)
                        };

                        await _planificacaoRepository.Create(anuncioPlanificacao);

                        n++;

                    } while (n < item.Estado);

                }

            return CreatedAtAction(nameof(GetSpots), new { idAnuncio = newAnuncio.Id }, newAnuncio);

        }

        [HttpPut]
        public async Task<ActionResult<Spot>> PutSpots(int id, [FromForm] Spot spots)
        {
            if (id != spots.Id)
            {
                return BadRequest();
            }

            await _spotsRepository.Update(spots);

            return NoContent();
        }

        [Route("editarSpot/{id}")]
        [HttpPost]
        public async Task<ActionResult<Spot>> EditarSpot([FromRoute] int id, [FromBody] Spot spot)
        {

            if(spot.IsActive == true)
            {
                var oldSpot = await _spotsRepository.Get(id);
                Spot spotHistory = new Spot()
                {
                    Designacao = oldSpot.Designacao,
                    Code = oldSpot.Code,
                    DataFim = oldSpot.DataFim,
                    DataInicio = oldSpot.DataInicio,
                    Duracao = oldSpot.Duracao,
                    IdCanal = oldSpot.IdCanal,
                    Periodicidade = oldSpot.Periodicidade,
                    IsActive = false,
                    IdSpotPrincipal = id,
                    IdTipoSpot = oldSpot.IdTipoSpot,
                    DataCreate = DateTime.Now,
                    Estado = 2
                };

                await _spotsRepository.Create(spotHistory);

                await _spotsRepository.EditarSpot(id, spot);
            }


            foreach (var item in spot.SpotPlanificacaos)
            {
                    if(item.IsReagendamento == true)
                    {
                        var getPlan = await _planificacaoRepository.Get(item.Id);

                        SpotPlanificacao editarPlanificacao = new SpotPlanificacao()
                        {
                            IdJanela = item.IdJanela,
                            DataPlanificacao = item.DataPlanificacao
                        };

                        await _planificacaoRepository.EditPlanificacao(item.Id, editarPlanificacao);

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
                    }

            }

            return NoContent();

        }

        [Route("updateSpot/{id}")]
        [HttpPost]
        public async Task<ActionResult<Spot>> PutSpot([FromRoute]int id, [FromBody] Spot spot)
        {
            if (id != spot.Id)
            {
                return BadRequest();
            }

            var oldSpot = await _spotsRepository.Get(id);

            Spot spotHistory = new Spot()
            {
                Designacao = oldSpot.Designacao,
                Code = oldSpot.Code,
                DataFim = oldSpot.DataFim,
                DataInicio = oldSpot.DataInicio,
                Duracao = oldSpot.Duracao,
                IdCanal = oldSpot.IdCanal,
                Periodicidade = oldSpot.Periodicidade,
                IsActive = false,
                IdSpotPrincipal = id,
                IdTipoSpot = oldSpot.IdTipoSpot,
                DataCreate = DateTime.Now,
                Estado = 2

            };

            await _spotsRepository.Create(spotHistory);

            await _spotsRepository.UpdateSpot(id, spot);

            return NoContent();
        }

        [Route("updateAnuncio/{id}")]
        [HttpPost]
        public async Task<ActionResult<Spot>> PutAnuncio([FromRoute]int id, [FromBody] Spot anuncio)
        {
            if (id != anuncio.Id)
            {
                return BadRequest();
            }

            var oldAnuncio = await _spotsRepository.Get(id);

            Spot anuncioHistory = new Spot()
            {
                Designacao = oldAnuncio.Designacao,
                Code = oldAnuncio.Code,
                DataFim = oldAnuncio.DataFim,
                Duracao = oldAnuncio.Duracao,
                IdCanal = oldAnuncio.IdCanal,
                Periodicidade = oldAnuncio.Periodicidade,
                IsActive = false,
                IdSpotPrincipal = id,
                IdTipoSpot = oldAnuncio.IdTipoSpot,
                DataCreate = DateTime.Now,
                Estado = 2

            };

            await _spotsRepository.Create(anuncioHistory);

            await _spotsRepository.UpdateSpot(id, anuncio);

            return NoContent();
        }

        [Route("disableSpot/{id}")]
        [HttpPost]
        public async Task<ActionResult<Spot>> PutSpotDisable([FromRoute]int id)
        {
            var spotToDisable = await _spotsRepository.Get(id);

            if (spotToDisable == null)
                return NotFound();

            spotToDisable.IsActive = false;
            await _spotsRepository.Update(spotToDisable);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var spotToDelete = await _spotsRepository.Get(id);

            if (spotToDelete == null)
                return NotFound($"Spot não encontrado: {id}");

            await _spotsRepository.Delete(spotToDelete.Id);
            return NoContent();
        }

        [HttpGet("listSpots")]
        public IActionResult GetAllSpotsList()
        {
            IEnumerable<Spot> spot = _spotsRepository.GetAllSpots();
            var result = MapSpotToSpotsDto(spot);

            return Ok(new { data = result });
        }

        [HttpGet("listSpotsBy/{idCanal}")]
        public IActionResult GetAllSpotsListBy(int idCanal)
        {
            IEnumerable<Spot> spot = _spotsRepository.GetAllSpotsBy(idCanal);
            var result = MapSpotToSpotsDto(spot);

            return Ok(new { data = result });
        }

        [HttpGet("listSpotsByIdCanalUser")]
        public IActionResult GetAllSpotsListByIdCanalUser()
        {
            var idCanal = int.Parse(User.FindFirst("IdCanal").Value);

            IEnumerable<Spot> spot = _spotsRepository.GetAllSpotsBy(idCanal);
            var result = MapSpotToSpotsDto(spot);

            return Ok(new { data = result });
        }

        [HttpGet("listAnuncios")]
        public IActionResult GetAllAnunciosList()
        {
            IEnumerable<Spot> anuncios = _spotsRepository.GetAllAnuncios();
            var result = MapSpotToSpotsDto(anuncios);

            return Ok(new { data = result });
        }

        [HttpGet("listAnunciosBy/{idCanal}")]
        public IActionResult GetAllAnunciosListBy([FromRoute] int idCanal)
        {
            IEnumerable<Spot> anuncios = _spotsRepository.GetAllAnunciosBy(idCanal);
            var result = MapSpotToSpotsDto(anuncios);

            return Ok(new { data = result });
        }

        [HttpGet("listAnunciosByIdCanalUser")]
        public IActionResult GetAllAnunciosListByIdCanalUser()
        {
            var idCanal = int.Parse(User.FindFirst("IdCanal").Value);

            IEnumerable<Spot> anuncios = _spotsRepository.GetAllAnunciosBy(idCanal);
            var result = MapSpotToSpotsDto(anuncios);

            return Ok(new { data = result });
        }

        [HttpGet("getSpotsCliente/{idCliente}")]
        public IActionResult GetSpotsClienteBy(string idCliente)
        {
            var cliente = _usuariosRepository.GetUsurioBy(idCliente);
            if (cliente.Result == null)
                return Ok(new { d = "-1" });

            IEnumerable<Spot> spots = _spotsRepository.GetAllSpotsClienteBy(cliente.Result.IdCliente.Value);
            return Ok(spots);
        }

        private IEnumerable<SpotsDto> MapSpotToSpotsDto(IEnumerable<Spot> spots)
        {
            IEnumerable<SpotsDto> result;
            result = spots.Select(s => new SpotsDto()
            {
                Id = s.Id,
                Designacao = s.Designacao,
                Code = s.Code,
                Cliente = s.IdClienteNavigation.Designacao,
                DataInicio = s.DataInicio,
                DataFim = s.DataFim,
                Periodo = s.SpotPlanificacaos.FirstOrDefault().IdJanelaNavigation.HoraInicio.ToString() + "-" + s.SpotPlanificacaos.FirstOrDefault().IdJanelaNavigation.HoraFim.ToString(),
                Periodicidade = s.Periodicidade,
                Estado = getEstadoString(s.Estado),
                Duracao = s.Duracao,
                Action = getAction(s.Id, s.Code, s.Designacao, s.DataInicio, s.DataFim, s.Periodicidade, s.Duracao, s.IdCanal, s.IdTipoSpot, s.Estado),
                IdCanal = s.IdCanal,
                idTipoSpot = s.IdTipoSpot,

            });

            return result;
        }

        private string getEstadoString(int? estado)
        {
            if (estado == 1)
            {
                string aberto = "<span class='badge bg-primary'>ABERTO</span>";
                return aberto;
            }
            else if(estado == 2)
            {
                string fechado = "<span class='badge badge-danger'>FECHADO</span>";
                return fechado;
            }
            else
            {
                string wow = "<span class='badge badge-danger'>FECHADO</span>";
                return wow;
            }
        }
        private string getAction(int id, string code, string designacao, DateTime? dataInicio, DateTime? dataFim, int? periodicidade, TimeSpan? duracao, int? idCanal, int? tipoSpot, int? estado)
        {
            var result = "";

            if(tipoSpot == 1)
                if(estado == 1)
                    result = "<div class='options-action'><a href='/Spots/Edit?code=" + code+ "' class='edit-click'><i class='fas fa-pencil-alt'></i></a> <i data-id='" + id + "' data-codigo='" + code + "' data-duracao='" + duracao + "' data-designacao='" + designacao + "' data-dataI='" + dataInicio + "' data-dataF='" + dataFim + "' data-periodicidade='" + periodicidade + "' class='destivarr fas fa-trash-alt'></i></div>";
                else
                    result = "<div class='options-action'><a class='edit-click-disabled'><i class='fas fa-pencil-alt'></i></a> <i data-id='" + id + "' data-codigo='" + code + "' data-duracao='" + duracao + "' data-designacao='" + designacao + "' data-dataI='" + dataInicio + "' data-dataF='" + dataFim + "' data-periodicidade='" + periodicidade + "' class='fas fa-trash-alt destivarr-disabled'></i></div>";
            else
                if (estado == 1)
                    result = "<div class='options-action'><a href='/Anuncios/Edit?code=" + code + "' class='edit-click'><i class='fas fa-pencil-alt'></i></a> <i data-id='" + id + "' data-codigo='" + code + "' data-duracao='" + duracao + "' data-designacao='" + designacao + "' data-dataI='" + dataInicio + "' data-dataF='" + dataFim + "' data-periodicidade='" + periodicidade + "' class='destivarr fas fa-trash-alt'></i></div>";
                else
                    result = "<div class='options-action'><a class='edit-click-disabled'><i class='fas fa-pencil-alt'></i></a> <i data-id='" + id + "' data-codigo='" + code + "' data-duracao='" + duracao + "' data-designacao='" + designacao + "' data-dataI='" + dataInicio + "' data-dataF='" + dataFim + "' data-periodicidade='" + periodicidade + "' class='fas fa-trash-alt destivarr-disabled'></i></div>";



            return result;
        }

        private DateTime Datecombined (DateTime dateV)
        {
            DateTime dateNow = dateV;
            TimeSpan time1 = new TimeSpan(7,0,0);
            //TimeSpan time2 = new TimeSpan(7,0,0);
            //TimeSpan time = time1 + time2;
            //DateTime combined = dateNow.Add(time);

            DateTime combined = dateNow.Add(time1);

            return combined;
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
        private string CodeGeneratorAsync(int idTipoProcesso, string codeProcesso)
        {        
            DateTime today = DateTime.Now;
            string result = "";
            if(idTipoProcesso == 1)
            {

                if (codeProcesso.Length > 10) {
                    if (codeProcesso.Contains("S"))
                    {
                        string[] code = codeProcesso.Split('/');
                        string todaycode = today.ToString("yy") + today.ToString("MM") + today.ToString("dd");
                        if (code[1] != todaycode)
                        {
                            result = "S/" + today.ToString("yy") + today.ToString("MM") + today.ToString("dd") + "/01";
                        }
                        else
                        {
                            int newCode = Int32.Parse(code[2]) + 1;
                            if (newCode < 10)
                                result = "S/" + today.ToString("yy") + today.ToString("MM") + today.ToString("dd") + "/0" + newCode.ToString();
                            else
                                result = "S/" + today.ToString("yy") + today.ToString("MM") + today.ToString("dd") + "/" + newCode.ToString();
                        }

                    }
                    else
                        result = "S/" + today.ToString("yy") + today.ToString("MM") + today.ToString("dd") + "/01";
                }
                else
                    result = "S/" + today.ToString("yy") + today.ToString("MM") + today.ToString("dd") + "/01";
            }

            else
            {
                if (codeProcesso.Length > 10)
                {
                    if (codeProcesso.Contains("A"))
                    {
                        string[] code = codeProcesso.Split('/');
                        string todaycode = today.ToString("yy") + today.ToString("MM") + today.ToString("dd");
                        if (code[1] != todaycode)
                        {
                            result = "A/" + today.ToString("yy") + today.ToString("MM") + today.ToString("dd") + "/01";
                        }
                        else
                        {
                            int newCode = Int32.Parse(code[2]) + 1;
                            if (newCode < 10)
                                result = "A/" + today.ToString("yy") + today.ToString("MM") + today.ToString("dd") + "/0" + newCode.ToString();
                            else
                                result = "A/" + today.ToString("yy") + today.ToString("MM") + today.ToString("dd") + "/" + newCode.ToString();
                        }
                    }
                    else
                        result = "A/" + today.ToString("yy") + today.ToString("MM") + today.ToString("dd") + "/01";
                }
                else
                    result = "A/" + today.ToString("yy") + today.ToString("MM") + today.ToString("dd") + "/01";
            }

            return result;
        }
    }
}
