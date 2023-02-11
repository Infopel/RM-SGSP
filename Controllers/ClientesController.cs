using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SGSP;

namespace SGSP.Views
{
    public class ClientesController : Controller
    {
        [Authorize(Roles = "Administrador, Operador Comercial, Chefe de Publicidade e Vendas")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
