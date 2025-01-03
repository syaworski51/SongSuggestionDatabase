using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using SongSuggestionDatabase.Data;
using SongSuggestionDatabase.Models;
using SongSuggestionDatabase.Models.Output;

namespace SongSuggestionDatabase.Controllers
{
    [Authorize(Roles = "Moderator")]
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
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? search, string sortBy = "date", string order = "desc")
        {
            ViewBag.SearchString = search;

            var sortOptions = _context.CatalogSortOptions.OrderBy(s => s.Id);
            ViewBag.CatalogSortOptions = new SelectList(sortOptions, "Value", "Name");

            bool orderIsAscending = order == "asc";

            IQueryable<Request> requests = _context.Requests
                .Include(r => r.Currency)
                .Include(r => r.Episode);

            if (!search.IsNullOrEmpty())
                requests = requests.Where(r => r.Title.Contains(search!) ||
                                               r.Artist.Contains(search!));
            
            switch (sortBy)
            {
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

                default:
                    requests = orderIsAscending ?
                        requests.OrderBy(r => r.Episode!.Date)
                            .ThenBy(r => r.Artist) :
                        requests.OrderByDescending(r => r.Episode!.Date)
                            .ThenBy(r => r.Artist);

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
                request.Episode = _context.Episodes.FirstOrDefault(e => e.Id == request.EpisodeId);
                request.USDAmount = request.Currency!.Code == "USD" ?
                    request.Amount :
                    await ConvertToUSDAsync(request.Currency!, (decimal)request.Amount!);

                // Request amounts cannot be less than $10 USD
                if (request.USDAmount < 10)
                {
                    ViewBag.Message = new DangerMessage("Request amount cannot be less than $10 USD.");
                    return View();
                }

                // Banned artists cannot be played if banned list checks are enabled
                bool bannedListChecksEnabled = request.Episode!.BannedListChecksEnabled == "Yes";
                bool artistIsBanned = await CheckBannedListAsync(request.Artist);
                if (bannedListChecksEnabled && artistIsBanned)
                {
                    ViewBag.Message = new DangerMessage($"{request.Artist} is banned.");
                    return View();
                }

                // Songs that have already been done cannot be played if catalog checks are enabled
                bool catalogChecksEnabled = request.Episode!.CatalogChecksEnabled == "Yes";
                bool songExists = await CheckCatalogAsync(request.Title, request.Artist);
                if (catalogChecksEnabled && songExists)
                {
                    ViewBag.Message = new DangerMessage($"{request.Title} by {request.Artist} has already been done.");
                    return View();
                }

                // If we have reached this point, this request has passed the validation checks!
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
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Amount,USDAmount,Title,Artist,RatingId,DetailedRating,Quote")] Request request)
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
        private async Task<bool> CheckCatalogAsync(string title, string artist)
        {
            return await _context.Requests.AnyAsync(r => r.Title == title && 
                                                         r.Artist == artist);
        }

        /// <summary>
        ///     Check the banned list to see if an artist has been banned from the livestreams.
        /// </summary>
        /// <param name="artist">The artist to check for.</param>
        /// <returns>True if the artist has been banned, False if not.</returns>
        private async Task<bool> CheckBannedListAsync(string artist)
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

        private async Task<decimal> ConvertToUSDAsync(Currency currency, decimal amount)
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
