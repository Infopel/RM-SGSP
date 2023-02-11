using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SGSP.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Controllers
{
    //[Authorize]                                       // bloqueia totalmente
    //[AllowAnonymous]                                // desbloqueia totalmente
    //[Authorize(Roles = "Admin, locutor, Etc...,")] // desbloqueia baseando em uma role!
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public HomeController(ILogger<HomeController> logger,
            UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize]
        public IActionResult Index()
        {
            //if (User.Identity.IsAuthenticated && (User.IsInRole("Administrador") || User.IsInRole("Operador Comercial") || User.IsInRole("Chefe de Publicidade e Vendas")))
            //    return RedirectToAction("Index", "Planificacao");
            if (User.Identity.IsAuthenticated && User.IsInRole("Locutor"))
                return RedirectToAction("LocutorAdmin", "Planificacao");
            else if (User.Identity.IsAuthenticated && User.IsInRole("Sonorizador"))
                return RedirectToAction("SonorizadorAdmin", "Planificacao");
            else if (User.Identity.IsAuthenticated && User.IsInRole("Coordenador"))
                return RedirectToAction("CoordenadorAdmin", "Planificacao");
            else if (User.Identity.IsAuthenticated && User.IsInRole("Cliente"))
                return RedirectToAction("Cliente", "Home");
            else
                return RedirectToAction("Index", "Planificacao");
            //else if (User.Identity.IsAuthenticated && (User.IsInRole("Operador Comercial") || User.IsInRole("Chefe de Publicidade e Vendas")))
            //    return RedirectToAction("Comercial", "Planificacao");

            return RedirectToAction("Index", "Planificacao"); ;
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Roles = "Cliente")]
        public IActionResult Cliente()
        {
            return View();
        }
    }
}
