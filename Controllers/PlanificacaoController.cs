using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Controllers
{
    public class PlanificacaoController : Controller
    {
        //[Authorize(Roles = "Administrador")]
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        //[Authorize(Roles = "Locutor")]
        [Authorize]
        public IActionResult Locutor()
        {
            return View();
        }

        [Route("LocutorAdmin")]
        //[Authorize(Roles = "Administrador")]
        [Authorize]
        public IActionResult LocutorAdmin()
        {
            return View();
        }

        //[Authorize(Roles = "Coordenador")]
        [Authorize]
        public IActionResult Coordenador()
        {
            return View();
        }

        [Route("CoordenadorAdmin")]
        //[Authorize(Roles = "Administrador")]
        [Authorize]
        public IActionResult CoordenadorAdmin()
        {
            return View();
        }

        //[Authorize(Roles = "Sonorizador")]
        [Authorize]
        public IActionResult Sonorizador()
        {
            return View();
        }

        //[Authorize(Roles = " Administrador")]
        [Authorize]
        public IActionResult SonorizadorAdmin()
        {
            return View();
        }

        //[Authorize(Roles = "Operador Comercial, Chefe de Publicidade e Vendas")]
        [Authorize]
        public IActionResult Comercial()
        {
            return View();
        }
    }
}
