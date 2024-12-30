using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using SongSuggestionDatabase.Data;
using SongSuggestionDatabase.Models;
using System.Net.Http.Headers;

namespace SongSuggestionDatabase.Controllers
{
    public class CurrenciesController : Controller
    {
        private readonly ILogger<CurrenciesController> _logger;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly string AppJson;

        public CurrenciesController(ILogger<CurrenciesController> logger,
                                    IConfiguration config,
                                    ApplicationDbContext context)
        {
            _logger = logger;
            _config = config;
            _context = context;

            AppJson = _config.GetValue<string>("RequestHeaders:AppJson")!;
        }
        
        public async Task<IActionResult> Index(string? search)
        {
            ViewBag.CurrencySearch = search;

            Dictionary<string, decimal> conversionRates = await GetConversionRates();
            IQueryable<Currency> currencies = _context.Currencies.OrderBy(c => c.Code);

            if (!search.IsNullOrEmpty())
            {
                currencies = currencies.Where(c => c.Symbol.Contains(search!) ||
                                                   c.Code.Contains(search!) ||
                                                   c.Name.Contains(search!));

                conversionRates = FilterCurrencyDictionary(conversionRates, currencies.ToList());
            }

            ViewBag.Currencies = currencies;
            
            return View(conversionRates);
        }

        private string GetCurrencyConverterAPIKey()
        {
            return _config.GetValue<string>("Secrets:CurrencyConverter:APIKey")!;
        }

        private async Task<Dictionary<string, decimal>> GetConversionRates()
        {
            Dictionary<string, decimal>? conversionRates = null;
            
            string key = GetCurrencyConverterAPIKey();
            string endpoint = $"https://v6.exchangerate-api.com/v6/{key}/latest/USD";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(AppJson));
                var response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var result = await ParseJSON(response);
                    conversionRates = result["conversion_rates"]!.ToObject<Dictionary<string, decimal>>()!;

                    conversionRates.Remove("USD");
                }
            }

            return conversionRates!;
        }

        private async Task<JObject> ParseJSON(HttpResponseMessage response)
        {
            string json = await response.Content.ReadAsStringAsync();
            JObject result = JObject.Parse(json);

            return result;
        }

        /// <summary>
        ///     Filter out a list of currencies from the currency dictionary.
        /// </summary>
        /// <param name="fullDictionary">The full dictionary of currencies.</param>
        /// <param name="filteringList">The list of currencies to filter out.</param>
        /// <returns>The new dictionary of currencies.</returns>
        private Dictionary<string, decimal> FilterCurrencyDictionary(Dictionary<string, decimal> fullDictionary,
                                                                     List<Currency> filteringList)
        {
            // Store the new dictionary here
            Dictionary<string, decimal> filteredCurrencies = new();
            
            // For each currency to be filtered...
            foreach (var currency in filteringList)
            {
                // If the currency's code is a key in the dictionary, add it and its conversion rate to the new dictionary
                if (fullDictionary.TryGetValue(currency.Code, out decimal rate))
                    filteredCurrencies.Add(currency.Code, rate);
            }

            // Return the filtered currency dictionary
            return filteredCurrencies;
        }
    }
}
