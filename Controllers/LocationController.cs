using ERHuntCV.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ERHuntCV.Controllers
{
    public class LocationController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LocationController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        // =========================================================
        // COUNTRIES
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> GetCountries()
        {
            try
            {
                string url =
                    "https://api.geocoded.me/v2/countries" +
                    "?fields=id,iso2,name&limit=300";

                using HttpClient client =
                    _httpClientFactory.CreateClient();

                HttpResponseMessage response =
                    await client.GetAsync(url);

                string json =
                    await response.Content.ReadAsStringAsync();

                Console.WriteLine(
                    "COUNTRY STATUS: " +
                    response.StatusCode);

                Console.WriteLine(
                    "COUNTRY RESPONSE: " +
                    json);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(
                        (int)response.StatusCode,
                        json);
                }

                var result =
                    JsonSerializer.Deserialize<CountryApiResponse>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                if (result?.Data == null)
                {
                    return Json(new List<Country>());
                }

                var countries =
                    result.Data
                        .OrderBy(x => x.Name)
                        .ToList();

                return Json(countries);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "COUNTRY ERROR: " +
                    ex.ToString());

                return StatusCode(
                    500,
                    "Country API Error: " +
                    ex.Message);
            }
        }


        // =========================================================
        // STATES
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> GetStates(
            string countryCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(countryCode))
                {
                    return BadRequest(
                        "Country code is required.");
                }

                countryCode =
                    countryCode
                        .Trim()
                        .ToUpperInvariant();

                string url =
                    "https://api.geocoded.me/v2/states" +
                    "?filter[country]=" +
                    Uri.EscapeDataString(countryCode) +
                    "&fields=id,name,countryCode,stateCode" +
                    "&limit=5000";

                Console.WriteLine(
                    "STATE URL: " + url);

                using HttpClient client =
                    _httpClientFactory.CreateClient();

                HttpResponseMessage response =
                    await client.GetAsync(url);

                string json =
                    await response.Content.ReadAsStringAsync();

                Console.WriteLine(
                    "STATE STATUS: " +
                    response.StatusCode);

                Console.WriteLine(
                    "STATE RESPONSE: " +
                    json);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(
                        (int)response.StatusCode,
                        json);
                }

                var result =
                    JsonSerializer.Deserialize<StateApiResponse>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                if (result?.Data == null)
                {
                    return Json(new List<State>());
                }

                var states =
                    result.Data
                        .OrderBy(x => x.Name)
                        .ToList();

                return Json(states);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "STATE ERROR: " +
                    ex.ToString());

                return StatusCode(
                    500,
                    "State API Error: " +
                    ex.Message);
            }
        }


        // =========================================================
        // CITIES
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> GetCities(
            string countryCode,
            string stateCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(countryCode))
                {
                    return BadRequest(
                        "Country code is required.");
                }

                if (string.IsNullOrWhiteSpace(stateCode))
                {
                    return BadRequest(
                        "State code is required.");
                }

                countryCode =
                    countryCode
                        .Trim()
                        .ToUpperInvariant();

                stateCode =
                    stateCode
                        .Trim()
                        .ToUpperInvariant();

                string url =
                    "https://api.geocoded.me/v2/cities" +
                    "?filter[country]=" +
                    Uri.EscapeDataString(countryCode) +
                    "&filter[state]=" +
                    Uri.EscapeDataString(stateCode) +
                    "&fields=id,name,countryCode,stateCode" +
                    "&limit=5000";

                Console.WriteLine(
                    "CITY URL: " + url);

                using HttpClient client =
                    _httpClientFactory.CreateClient();

                HttpResponseMessage response =
                    await client.GetAsync(url);

                string json =
                    await response.Content.ReadAsStringAsync();

                Console.WriteLine(
                    "CITY STATUS: " +
                    response.StatusCode);

                Console.WriteLine(
                    "CITY RESPONSE: " +
                    json);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(
                        (int)response.StatusCode,
                        json);
                }

                var result =
                    JsonSerializer.Deserialize<CityApiResponse>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                if (result?.Data == null)
                {
                    return Json(new List<City>());
                }

                var cities =
                    result.Data
                        .OrderBy(x => x.Name)
                        .ToList();

                return Json(cities);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "CITY ERROR: " +
                    ex.ToString());

                return StatusCode(
                    500,
                    "City API Error: " +
                    ex.Message);
            }
        }


        // =========================================================
        // API RESPONSE CLASSES
        // =========================================================

        public class CountryApiResponse
        {
            public List<Country> Data { get; set; }
                = new List<Country>();
        }

        public class StateApiResponse
        {
            public List<State> Data { get; set; }
                = new List<State>();
        }

        public class CityApiResponse
        {
            public List<City> Data { get; set; }
                = new List<City>();
        }
    }
}