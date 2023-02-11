using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Controllers
{
    public class RelatoriosController : Controller
    {
        //[Authorize(Roles = "Administrador, Operador Comercial, Chefe de Publicidade e Vendas")]
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        //[Authorize(Roles = "Administrador, Operador Comercial, Chefe de Publicidade e Vendas")]
        [Authorize]
        public IActionResult Print()
        {
            return View();
        }
    }
}
