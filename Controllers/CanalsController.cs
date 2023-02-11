using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGSP.Controllers
{
    public class CanalsController : Controller
    {

        //[Authorize(Roles = "Administrador")]
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        //[Authorize(Roles = "Administrador")]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        //[Authorize(Roles = "Administrador")]
        [Authorize]
        public IActionResult CreateJanelas()
        {
            return View();
        }
    }
}
