using LibPro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibPro.Controllers
{
    public class BibliosController : Controller
    {
        private readonly LibproContext _context;

        public BibliosController(LibproContext context)
        {
            _context = context;
        }

        // GET: Biblios
        public async Task<IActionResult> Index(byte isDeleted = 0)
        {
            var biblios = await _context.Biblios.Where(b =>
                b.isDeleted == isDeleted).Include(b => b.Category).ToListAsync();

            return View(biblios);
        }

        // GET: Biblios/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var biblios = await _context.Biblios
                .FirstOrDefaultAsync(m => m.BibID == id);
            if (biblios == null)
            {
                return NotFound();
            }

            return View(biblios);
        }

        // GET: Biblios/Create
        public IActionResult Create()
        {
            ViewData["CatID"] = new SelectList(_context.Categories, "CatID", "CatName");
            ViewData["PubID"] = new SelectList(_context.Publishers, "PubID", "PubName");
            return View();
        }

        // POST: Biblios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Biblios biblios, IFormFile? newimg)
        {
            if (ModelState.IsValid)
            {

                if (newimg != null && newimg.Length != 0)
                {


                    //檢查上傳的檔案格式
                    if (newimg.ContentType != "image/jpeg" && newimg.ContentType != "image/png")
                    {
                        ModelState.AddModelError("Image", "只允許上傳jpg或png格式的圖片檔案");
                        return View(biblios);
                    }

                    string FileName = Guid.NewGuid().ToString() + Path.GetExtension(newimg.FileName);
                    string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImgPath", FileName);


                    using (FileStream stream = new FileStream(uploads, FileMode.Create))
                    {
                        newimg.CopyTo(stream);
                        biblios.ImgPath = FileName;
                    }


                }




                _context.Add(biblios);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            return View(biblios);
        }

        // GET: Biblios/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var biblios = await _context.Biblios.FindAsync(id);
            if (biblios == null)
            {
                return NotFound();
            }
            ViewData["CatID"] = new SelectList(_context.Categories, "CatID", "CatName", biblios.CatID);
            ViewData["PubID"] = new SelectList(_context.Publishers, "PubID", "PubName", biblios.PubID);
            return View(biblios);
        }

        // POST: Biblios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id,  Biblios biblios, IFormFile? newimg)
        {
            if (id != biblios.BibID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var oldData = await _context.Biblios.AsNoTracking().FirstOrDefaultAsync(b => b.BibID == biblios.BibID);
                var oldImg = oldData.ImgPath;

                if (newimg != null && newimg.Length != 0)
                {

                    //檢查上傳的檔案格式
                    if (newimg.ContentType != "image/jpeg" && newimg.ContentType != "image/png")
                    {
                        ModelState.AddModelError("Image", "只允許上傳jpg或png格式的圖片檔案");
                        return View(biblios);
                    }



                  


                    if (oldImg != null)
                    {
                        string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImgPath", oldImg);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }

                    }

                  

                    string FileName = Guid.NewGuid().ToString() + Path.GetExtension(newimg.FileName);
                    string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ImgPath", FileName);


                    using (FileStream stream = new FileStream(uploads, FileMode.Create))
                    {
                        newimg.CopyTo(stream);
                        biblios.ImgPath = FileName;
                    }


                }

                if(newimg == null && oldImg != null)
                {
                    biblios.ImgPath = oldImg;
                }


                try
                {
                    _context.Update(biblios);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BibliosExists(biblios.BibID))
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
            return View(biblios);
        }

      
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var biblios = await _context.Biblios.FindAsync(id);
            if (biblios != null)
            {
                biblios.isDeleted = 1;
                _context.Biblios.Update(biblios);
                await _context.SaveChangesAsync();            
            
            }

         
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(long id)
        {
            var biblios = await _context.Biblios.FindAsync(id);



            if (biblios != null)
            {
                biblios.isDeleted = 0;
                _context.Biblios.Update(biblios);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BibliosExists(long id)
        {
            return _context.Biblios.Any(e => e.BibID == id);
        }
    }
}
