using LibPro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibPro.Controllers
{
    [Authorize(Roles = "Staff")]
    public class LocationsController : Controller
    {
        private readonly LibproContext _context;

        public LocationsController(LibproContext context)
        {
            _context = context;
        }

        // GET: Locations
        public async Task<IActionResult> Index()
        {
            var libproContext = _context.Locations.Include(l => l.ParentLocation);
            return View(await libproContext.ToListAsync());
        }

       
        // GET: Locations/Create
        public IActionResult Create()
        {
            ViewData["ParentID"] = new SelectList(_context.Locations, "LocationID", "LocationName");
            return PartialView("_LocCreate"); ;
        }

        // POST: Locations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LocationID,LocationName,Depth,SortOrder,ParentID")] Locations locations)
        {
            if (ModelState.IsValid)
            {
                _context.Add(locations);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentID"] = new SelectList(_context.Locations, "LocationID", "LocationName", locations.ParentID);
            return View(locations);
        }

        // GET: Locations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locations = await _context.Locations.FindAsync(id);
            if (locations == null)
            {
                return NotFound();
            }
            ViewData["ParentID"] = new SelectList(_context.Locations, "LocationID", "LocationName", locations.ParentID);
            return PartialView("_LocEdit", locations);
        }

        // POST: Locations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Locations locations)
        {
            if (id != locations.LocationID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(locations);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationsExists(locations.LocationID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentID"] = new SelectList(_context.Locations, "LocationID", "LocationName", locations.ParentID);
            return View(locations);
        }


        private bool LocationsExists(int id)
        {
            return _context.Locations.Any(e => e.LocationID == id);
        }
    }
}
