using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SongSuggestionDatabase.Data;
using SongSuggestionDatabase.Models;

namespace SongSuggestionDatabase.Controllers
{
    public class EpisodesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<LiveDataHub> _hubContext;

        public EpisodesController(ApplicationDbContext context, IHubContext<LiveDataHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: Episodes
        public async Task<IActionResult> Index(int year)
        {
            var episodes = _context.Episodes.OrderByDescending(e => e.Date);

            List<int> years = episodes
                .OrderByDescending(e => e.Date.Year)
                .Select(e => e.Date.Year)
                .Distinct()
                .ToList();
            ViewBag.Years = new SelectList(years);

            Dictionary<Episode, int> songCounts = new();
            foreach (var episode in episodes)
            {
                int count = await _context.Requests.CountAsync(r => r.Episode == episode);
                songCounts.Add(episode, count);
            }
            ViewBag.SongCounts = songCounts;

            var currencies = _context.Currencies.OrderBy(c => c.Code);
            ViewBag.Currencies = new SelectList(currencies, "Code", "Symbol");

            return View(episodes);
        }

        // GET: Episodes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Episodes == null)
            {
                return NotFound();
            }

            var episode = await _context.Episodes.FirstOrDefaultAsync(m => m.Id == id);
            if (episode == null)
            {
                return NotFound();
            }

            IQueryable<Request> songs = _context.Requests
                .Where(r => r.Episode == episode)
                .OrderBy(r => r.Artist);
            ViewBag.Songs = songs;

            ViewBag.RatingCounts = await GetRatingCounts(songs);

            return View(episode);
        }

        // GET: Episodes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Episodes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,CatalogChecksEnabled,BannedListChecksEnabled")] Episode episode)
        {
            episode.Id = Guid.NewGuid();
            episode.Date = DateTime.Now;
            episode.ShortName = $"LSSF {episode.Date.ToShortDateString()}";
            episode.Format = "Livestream";

            _context.Add(episode);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Episodes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Episodes == null)
            {
                return NotFound();
            }

            var episode = await _context.Episodes.FindAsync(id);
            if (episode == null)
            {
                return NotFound();
            }
            return View(episode);
        }

        // POST: Episodes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Date,Title,Format,CatalogChecksEnabled,BannedListChecksEnabled")] Episode episode)
        {
            if (id != episode.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(episode);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EpisodeExists(episode.Id))
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
            return View(episode);
        }

        // GET: Episodes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Episodes == null)
            {
                return NotFound();
            }

            var episode = await _context.Episodes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (episode == null)
            {
                return NotFound();
            }

            return View(episode);
        }

        // POST: Episodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Episodes == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Episode'  is null.");
            }
            var episode = await _context.Episodes.FindAsync(id);
            if (episode != null)
            {
                _context.Episodes.Remove(episode);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EpisodeExists(Guid id)
        {
          return (_context.Episodes?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private async Task<Dictionary<Rating, int>> GetRatingCounts(IQueryable<Request> songs)
        {
            Dictionary<Rating, int> ratingCounts = new();
            var ratings = _context.Ratings.OrderByDescending(r => r.Index);

            foreach (var rating in ratings)
            {
                int count = await songs
                    .Where(s => s.Rating == rating)
                    .CountAsync();

                ratingCounts.Add(rating, count);
            }

            return ratingCounts;
        }

        public async Task<IActionResult> StartSong(Request song, Request? previousSong)
        {
            song.Status = "Now Playing";

            if (previousSong != null)
                previousSong.Status = "Completed";

            _context.Update(song);
            _context.Update(previousSong);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveUpdate");

            return Ok();
        }
    }
}
