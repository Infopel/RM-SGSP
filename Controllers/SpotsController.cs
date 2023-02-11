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

namespace SGSP.Views
{
    public class SpotsController : Controller
    {
        private readonly dbSpotsContext _context;

        public SpotsController(dbSpotsContext context)
        {
            _context = context;
        }

        // GET: Spots
        //[Authorize(Roles = "Administrador")]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var dbSpotsContext = _context.Spots.Include(s => s.IdCanalNavigation).Include(s => s.IdClienteNavigation).Include(s => s.IdSpotPrincipalNavigation);
            return View(await dbSpotsContext.ToListAsync());
        }

        //// GET: Spots/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var spot = await _context.Spots
        //        .Include(s => s.IdCanalNavigation)
        //        .Include(s => s.IdClienteNavigation)
        //        .Include(s => s.IdSpotPrincipalNavigation)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (spot == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(spot);
        //}

        // GET: Spots/Create
        //[Authorize(Roles = "Administrador, Operador Comercial, Chefe de Publicidade e Vendas")]
        [Authorize]
        public IActionResult Create()
        {
            ViewData["IdCanal"] = new SelectList(_context.Canals, "Id", "Designacao");
            ViewData["IdCliente"] = new SelectList(_context.Clientes, "Id", "Id");
            ViewData["IdSpotPrincipal"] = new SelectList(_context.Spots, "Id", "Id");
            return View();
        }

        //[Authorize(Roles = "Administrador, Operador Comercial, Chefe de Publicidade e Vendas")]
        [Authorize]
        public IActionResult Edit()
        {
            return View();
        }

        [Authorize(Roles = "Operador Comercial, Chefe de Publicidade e Vendas")]
        public IActionResult ListSpots()
        {
            return View();
        } 

        // POST: Spots/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Designacao,Code,IdCliente,DataInicio,DataFim,Duracao,IdCanal,Periodicidade,IsActive,IdSpotPrincipal")] Spot spot)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(spot);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["IdCanal"] = new SelectList(_context.Canals, "Id", "Id", spot.IdCanal);
        //    ViewData["IdCliente"] = new SelectList(_context.Clientes, "Id", "Id", spot.IdCliente);
        //    ViewData["IdSpotPrincipal"] = new SelectList(_context.Spots, "Id", "Id", spot.IdSpotPrincipal);
        //    return View(spot);
        //}

        //// GET: Spots/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var spot = await _context.Spots.FindAsync(id);
        //    if (spot == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["IdCanal"] = new SelectList(_context.Canals, "Id", "Id", spot.IdCanal);
        //    ViewData["IdCliente"] = new SelectList(_context.Clientes, "Id", "Id", spot.IdCliente);
        //    ViewData["IdSpotPrincipal"] = new SelectList(_context.Spots, "Id", "Id", spot.IdSpotPrincipal);
        //    return View(spot);
        //}

        // POST: Spots/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Designacao,Code,IdCliente,DataInicio,DataFim,Duracao,IdCanal,Periodicidade,IsActive,IdSpotPrincipal")] Spot spot)
        //{
        //    if (id != spot.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(spot);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!SpotExists(spot.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["IdCanal"] = new SelectList(_context.Canals, "Id", "Id", spot.IdCanal);
        //    ViewData["IdCliente"] = new SelectList(_context.Clientes, "Id", "Id", spot.IdCliente);
        //    ViewData["IdSpotPrincipal"] = new SelectList(_context.Spots, "Id", "Id", spot.IdSpotPrincipal);
        //    return View(spot);
        //}

        //// GET: Spots/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var spot = await _context.Spots
        //        .Include(s => s.IdCanalNavigation)
        //        .Include(s => s.IdClienteNavigation)
        //        .Include(s => s.IdSpotPrincipalNavigation)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (spot == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(spot);
        //}

        //// POST: Spots/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var spot = await _context.Spots.FindAsync(id);
        //    _context.Spots.Remove(spot);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool SpotExists(int id)
        //{
        //    return _context.Spots.Any(e => e.Id == id);
        //}
    }
}
