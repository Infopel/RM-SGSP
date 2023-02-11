using SGSP.Dtos;
using SGSP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Repositories
{
    public interface IReportRepository
    {
        byte[] GenerateReportAsync(List<DataSetDto> Planificacao, string reportName, string nomeUser, string periodo, DateTime dataI, DateTime dataF);
    }
}
