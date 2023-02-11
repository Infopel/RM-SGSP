using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SGSP;
using SGSP.Data;

namespace SGSP.Controllers
{
    public class AnunciosController : Controller
    {
        private readonly dbSpotsContext _context;

        public AnunciosController(dbSpotsContext context)
        {
            _context = context;
        }

        // GET: Spots
        //[Authorize(Roles = "Administrador")]
        [Authorize]
        public IActionResult Index()
        {
            ViewData["IdCanal"] = new SelectList(_context.Canals, "Id", "Designacao");
            return View();
        }

        //[Authorize(Roles = "Administrador, Chefe de Publicidade e Vendas")]
        [Authorize]
        public IActionResult Create()
        {
            ViewData["IdCanal"] = new SelectList(_context.Canals, "Id", "Designacao");
            ViewBag.Janela = _context.Janelas.ToList();

            return View();
        }

        //[Authorize(Roles = "Administrador, Chefe de Publicidade e Vendas")]
        [Authorize]
        public IActionResult Edit()
        {
            return View();
        }

        //[Authorize(Roles = "Chefe de Publicidade e Vendas")]
        [Authorize]
        public IActionResult ListAnuncios()
        {
            return View();
        }
    }
}
