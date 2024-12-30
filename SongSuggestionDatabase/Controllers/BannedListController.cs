using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SongSuggestionDatabase.Data;
using SongSuggestionDatabase.Models;

namespace SongSuggestionDatabase.Controllers
{
    public class BannedListController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BannedListController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BannedList
        public async Task<IActionResult> Index()
        {
              return _context.BannedList != null ? 
                          View(await _context.BannedList.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.BannedArtist'  is null.");
        }

        // GET: BannedList/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BannedList/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,IgnoreInChecks,IsPermanentlyBanned,Comments")] BannedArtist bannedArtist)
        {
            if (ModelState.IsValid)
            {
                bannedArtist.Id = Guid.NewGuid();
                _context.Add(bannedArtist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bannedArtist);
        }

        // GET: BannedList/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.BannedList == null)
            {
                return NotFound();
            }

            var bannedArtist = await _context.BannedList.FindAsync(id);
            if (bannedArtist == null)
            {
                return NotFound();
            }
            return View(bannedArtist);
        }

        // POST: BannedList/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,IgnoreInChecks,IsPermanentlyBanned,Comments")] BannedArtist bannedArtist)
        {
            if (id != bannedArtist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bannedArtist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BannedArtistExists(bannedArtist.Id))
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
            return View(bannedArtist);
        }

        // GET: BannedList/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.BannedList == null)
            {
                return NotFound();
            }

            var bannedArtist = await _context.BannedList
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bannedArtist == null)
            {
                return NotFound();
            }

            return View(bannedArtist);
        }

        // POST: BannedList/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.BannedList == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BannedArtist'  is null.");
            }
            var bannedArtist = await _context.BannedList.FindAsync(id);
            if (bannedArtist != null)
            {
                _context.BannedList.Remove(bannedArtist);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BannedArtistExists(Guid id)
        {
          return (_context.BannedList?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
