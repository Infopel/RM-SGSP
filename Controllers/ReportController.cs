using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Reporting;
using System.Reflection;
using SGSP.Repositories;
using SGSP.Models;
using SGSP.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace SGSP.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportRepository _reportRepository;
        private readonly IPlanificacaoRepository _planificacaoRepository;
        public ReportController(IReportRepository reportRepository, IPlanificacaoRepository planificacaoRepository)
        {
            _planificacaoRepository = planificacaoRepository;
            _reportRepository = reportRepository;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult print(ReportViewModel model)
        {
            var planificacao = _planificacaoRepository.getPlanificacaoby(model.IdCliente.Value, model.TipoSpot.Value, model.DataI.Value, model.DataF.Value, model.IdCanal.Value);
            if (planificacao.Count == 0)
                return Ok(new { d = "-1" });

            return Ok(new { d = "1" });
        }

        [HttpGet]
        [Authorize]
        public IActionResult PrintCertificado(ReportViewModel model)
        {
            var planificacao = _planificacaoRepository.getPlanificacaoby(model.IdCliente.Value, model.TipoSpot.Value, model.DataI.Value, model.DataF.Value, model.IdCanal.Value);
            
            if (planificacao.Count == 0)
                return NotFound();
            
            List<PlanificacaoDto> planDto = new List<PlanificacaoDto>();

            var dataSetDtos = new List<DataSetDto>();
            var listDtos = new List<DataSetDto>();

            DateTime dataAdd = model.DataI.Value.Date;
            string horas = "";
            dataAdd = dataAdd.AddDays(-1);

            do
            {
                dataAdd = dataAdd.AddDays(1);

                foreach (var item in planificacao.GroupBy(p => p.DataPassagemConfirmacao.Value.Date))
                {

                    horas = "";

                    foreach (var group in item)
                    {
                        horas += " "+group.DataPassagemConfirmacao.Value.TimeOfDay.ToString("hh\\:mm") + ",";
                    }

                    if (item.FirstOrDefault().DataPassagemConfirmacao.Value.Date == dataAdd.Date)
                    {
                        listDtos.Add(new DataSetDto
                            {
                                CanalDesignacao = item.FirstOrDefault().IdCanalNavigation.Designacao,
                                DuracaoSpot = item.FirstOrDefault().IdSpotNavigation.Duracao.Value.ToString(),
                                SpotDesignacao = item.FirstOrDefault().IdSpotNavigation.Designacao,
                                ClienteDesignaca = item.FirstOrDefault().IdSpotNavigation.IdClienteNavigation.Designacao,
                                DataPassagemConfrimacao = item.FirstOrDefault().DataPassagemConfirmacao.Value.Date,
                                Dias = item.FirstOrDefault().DataPassagemConfirmacao.Value.Day.ToString(),
                                Horas = horas.TrimEnd(new char[] { ',' })
                            });
                    }
                    else
                    {
                        var isExist = listDtos.Where(d => d.DataPassagemConfrimacao == dataAdd.Date).ToList();

                        if (isExist.Count == 0)
                            listDtos.Add(new DataSetDto
                            {
                                CanalDesignacao = item.FirstOrDefault().IdCanalNavigation.Designacao,
                                DuracaoSpot = item.FirstOrDefault().IdSpotNavigation.Duracao.Value.ToString(),
                                SpotDesignacao = item.FirstOrDefault().IdSpotNavigation.Designacao,
                                ClienteDesignaca = item.FirstOrDefault().IdSpotNavigation.IdClienteNavigation.Designacao,
                                DataPassagemConfrimacao = dataAdd.Date,
                                Dias = dataAdd.Day.ToString(),
                                Horas = " -"
                            });    
                    }
                }
            } while (dataAdd.Date <= model.DataF.Value.Date.AddDays(-1));



            foreach (var lastDto in listDtos.GroupBy(d => d.DataPassagemConfrimacao))
            {
                dataSetDtos.Add(new DataSetDto
                {
                    CanalDesignacao = lastDto.FirstOrDefault().CanalDesignacao,
                    DuracaoSpot = lastDto.FirstOrDefault().DuracaoSpot,
                    SpotDesignacao = lastDto.FirstOrDefault().SpotDesignacao,
                    ClienteDesignaca = lastDto.FirstOrDefault().ClienteDesignaca,
                    Dias = lastDto.FirstOrDefault().Dias,
                    Horas = lastDto.LastOrDefault().Horas.Remove(0, 1)
                });
            }


            string userName = User.FindFirst("Nome").Value + " " + User.FindFirst("Apelido").Value;

            string periodo = model.DataI.Value.ToString("MMMMM", System.Globalization.CultureInfo.GetCultureInfo("PT-pt")).ToUpper() 
                            + " - "+ model.DataF.Value.ToString("MMMMM", System.Globalization.CultureInfo.GetCultureInfo("PT-pt")).ToUpper() 
                            + "/"+ model.DataF.Value.ToString("yyyy");

            dataSetDtos.FirstOrDefault().UserName = userName;
            dataSetDtos.FirstOrDefault().Periodo = periodo;
            dataSetDtos.FirstOrDefault().PrintDay = "MAPUTO, " + DateTime.Today.ToString("dd 'de' MMMMM 'de' yyyy", System.Globalization.CultureInfo.GetCultureInfo("PT-pt")).ToUpper();

            return Ok(new { data = dataSetDtos });
            //var returnString = _reportRepository.GenerateReportAsync(dataSetDtos, model.ReportName, userName, periodo, model.DataI.Value, model.DataF.Value);

            //return File(returnString, System.Net.Mime.MediaTypeNames.Application.Octet, model.ReportName+"_"+ model.IdCliente.Value + model.DataF.Value.Day+".pdf");
        }

    }
}
