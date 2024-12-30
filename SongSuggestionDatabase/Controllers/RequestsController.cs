using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using SongSuggestionDatabase.Data;
using SongSuggestionDatabase.Models;

namespace SongSuggestionDatabase.Controllers
{
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private HttpClient _client = new HttpClient();

        public RequestsController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // GET: Requests
        public async Task<IActionResult> Index(string? search, string sortOrder = "date_desc")
        {
            ViewBag.SearchString = search;

            var sortOptions = _context.CatalogSortOptions.OrderBy(s => s.Id);
            ViewBag.CatalogSortOptions = new SelectList(sortOptions, "Value", "Name");

            string[] sortParams = sortOrder.Split("_");
            string sortBy = sortParams[0], order = sortParams[1];

            bool orderIsAscending = order == "asc";

            IQueryable<Request> requests = _context.Requests
                .Include(r => r.Currency)
                .Include(r => r.Episode);

            if (!search.IsNullOrEmpty())
                requests = requests.Where(r => r.Title.Contains(search!) ||
                                               r.Artist.Contains(search!));
            
            switch (sortBy)
            {
                case "date":
                    requests = orderIsAscending ?
                        requests.OrderBy(r => r.Episode!.Date)
                            .ThenBy(r => r.Artist) :
                        requests.OrderByDescending(r => r.Episode!.Date)
                            .ThenBy(r => r.Artist);

                    break;

                case "artist":
                    requests = orderIsAscending ?
                        requests.OrderBy(r => r.Artist)
                            .ThenBy(r => r.Title)
                            .ThenBy(r => r.Episode!.Date) :
                        requests.OrderByDescending(r => r.Artist)
                            .ThenBy(r => r.Title)
                            .ThenBy(r => r.Episode!.Date);

                    break;

                case "title":
                    requests = orderIsAscending ?
                        requests.OrderBy(r => r.Title)
                            .ThenBy(r => r.Artist)
                            .ThenBy(r => r.Episode!.Date) :
                        requests.OrderByDescending(r => r.Title)
                            .ThenBy(r => r.Artist)
                            .ThenBy(r => r.Episode!.Date);

                    break;
            }

            ViewBag.RatingCounts = await GetRatingCounts(requests);

            return View(await requests.ToListAsync());
        }

        // GET: Requests/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Requests == null)
            {
                return NotFound();
            }

            var request = await _context.Requests
                .FirstOrDefaultAsync(m => m.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // GET: Requests/Create
        public IActionResult Create()
        {
            var currencies = _context.Currencies.OrderBy(c => c.Code);
            ViewBag.Currencies = new SelectList(currencies, "Code", "Symbol");
            
            return View();
        }

        // POST: Requests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RequestedBy,CurrencyCode,Amount,Title,Artist,EpisodeId")] Request request)
        {
            if (ModelState.IsValid)
            {
                request.Id = Guid.NewGuid();
                request.TimeRequested = DateTime.Now;
                request.Status = "In Queue";
                request.USDAmount = request.Currency!.Code == "USD" ?
                    request.Amount :
                    await ConvertToUSD(request.Currency!, (int)request.Amount!);

                _context.Add(request);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var currencies = _context.Currencies.OrderBy(c => c.Code);
            ViewBag.Currencies = new SelectList(currencies, "Code", "Symbol");

            return View(request);
        }

        // GET: Requests/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Requests == null)
            {
                return NotFound();
            }

            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }
            return View(request);
        }

        // POST: Requests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,TimeRequested,Amount,USDAmount,Title,Artist,DetailedRating,Quote")] Request request)
        {
            if (id != request.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(request);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestExists(request.Id))
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
            return View(request);
        }

        // GET: Requests/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Requests == null)
            {
                return NotFound();
            }

            var request = await _context.Requests
                .FirstOrDefaultAsync(m => m.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // POST: Requests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Requests == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Request'  is null.");
            }
            var request = await _context.Requests.FindAsync(id);
            if (request != null)
            {
                _context.Requests.Remove(request);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequestExists(Guid id)
        {
            return (_context.Requests?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private void ResetDefaultRequestHeaders(HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        ///     Check the catalog to see if a song has been done before.
        /// </summary>
        /// <param name="title">The title of the song.</param>
        /// <param name="artist">The artist of the song.</param>
        /// <returns>True if the song has been done before, False if not.</returns>
        private async Task<bool> CheckCatalog(string title, string artist)
        {
            return await _context.Requests.AnyAsync(r => r.Title == title && 
                                                         r.Artist == artist);
        }

        /// <summary>
        ///     Check the banned list to see if an artist has been banned from the livestreams.
        /// </summary>
        /// <param name="artist">The artist to check for.</param>
        /// <returns>True if the artist has been banned, False if not.</returns>
        private async Task<bool> CheckBannedList(string artist)
        {
            return await _context.BannedList.AnyAsync(a => a.Name == artist);
        }

        private async Task<Dictionary<Rating, int>> GetRatingCounts(IQueryable<Request> songs)
        {
            Dictionary<Rating, int> ratingCounts = new();
            var ratings = _context.Ratings.OrderByDescending(r => r.Index);

            foreach (var rating in ratings)
            {
                int count = await songs.CountAsync(s => s.Rating == rating);
                ratingCounts.Add(rating, count);
            }

            return ratingCounts;
        }

        private async Task<decimal> ConvertToUSD(Currency currency, decimal amount)
        {
            string key = _config.GetValue<string>("Secrets:CurrencyConverter:APIKey")!;
            string endpoint = $"https://v6.exchangerate-api.com/v6/{key}/pair/{currency.Code}/USD/{amount}";
            
            using (_client)
            {
                ResetDefaultRequestHeaders(_client);
                var response = await _client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    JObject result = await ParseJSON(response);
                    return result["conversion_result"]!.ToObject<decimal>();
                }
            }

            throw new Exception($"There was an error converting {currency.Code} {amount} to USD.");
        }

        private async Task<JObject> ParseJSON(HttpResponseMessage response)
        {
            string jsonContent = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(jsonContent);

            return result;
        }
    }
}
