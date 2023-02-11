using AspNetCore.Reporting;
using SGSP.Data;
using SGSP.Dtos;
using SGSP.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SGSP.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly dbSpotsContext _context;
        private readonly IPlanificacaoRepository _planificacaoRepository;
        public ReportRepository(dbSpotsContext context, IPlanificacaoRepository planificacaoRepository)
        {
            _context = context;
            _planificacaoRepository = planificacaoRepository;
        }
        public byte[] GenerateReportAsync(List<DataSetDto> dataSetDtos, string reportName, string nomeUser, string periodo, DateTime dataI, DateTime dataF)
        {

            string fileDirPath = Assembly.GetExecutingAssembly().Location.Replace("SGSP.dll", string.Empty);
            string path = string.Format("{0}Reports\\{1}.rdlc", fileDirPath, reportName);
            string [] duracaoProcesso = dataSetDtos.FirstOrDefault().DuracaoSpot.ToString().Split(':');

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            if(Int32.Parse(duracaoProcesso[1]) != 0)
                parameters.Add("duracao", duracaoProcesso[1]+"' "+duracaoProcesso[2]+'"');
            else
                parameters.Add("duracao", duracaoProcesso[2]+'"');

            parameters.Add("responsavel", nomeUser);
            parameters.Add("periodo", periodo);
            parameters.Add("local", "MAPUTO, " + DateTime.Today.ToString("dd 'de' MMMMM 'de' yyyy", System.Globalization.CultureInfo.GetCultureInfo("PT-pt")).ToUpper());
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding.GetEncoding("windows-1252");
            LocalReport report = new LocalReport(path);

            report.AddDataSource("DataSetCertificado", dataSetDtos);
            
            var result = report.Execute(GetRenderType("pdf"), 1, parameters, "");

            return result.MainStream;
        }

        private RenderType GetRenderType(string reportType)
        {
            var renderType = RenderType.Pdf;
            switch (reportType.ToLower())
            {
                default:
                case "pdf":
                    renderType = RenderType.Pdf;
                    break;
                case "word":
                    renderType = RenderType.Word;
                    break;
                case "excel":
                    renderType = RenderType.Excel;
                    break;
            }

            return renderType;
        }

    }
}
