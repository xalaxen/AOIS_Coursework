using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AOIS.Controller
{
    public class MakeRequests
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly string TOKEN = Environment.GetEnvironmentVariable("TOKEN_KP");
        private static MakeRequests instance;

        private MakeRequests() { }

        public static MakeRequests Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MakeRequests();
                }
                return instance;
            }
        }

        private HttpRequestMessage CreateRequest(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-API-KEY", TOKEN);
            return request;
        }

        public async Task<string> GetGenres()
        {
            var request = CreateRequest("https://api.kinopoisk.dev/v1/movie/possible-values-by-field?field=genres.name");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string genrsList = await response.Content.ReadAsStringAsync();

            return genrsList;
        }

        public async Task<string> GetCountries()
        {
            var request = CreateRequest("https://api.kinopoisk.dev/v1/movie/possible-values-by-field?field=countries.name");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string countriesList = await response.Content.ReadAsStringAsync();

            return countriesList;
        }

        public async Task<string> GetFilms(string genre, int page)
        {
            var request = CreateRequest("https://api.kinopoisk.dev/v1.4/movie?" + $"page={page}" + "&limit=100&selectFields=id&selectFields=name&selectFields=enName&selectFields=alternativeName&selectFields=year&selectFields=rating&selectFields=ageRating&selectFields=budget&selectFields=movieLength&selectFields=genres&selectFields=countries&selectFields=persons&selectFields=fees&isSeries=false" + $"&genres.name={genre}" + "&notNullFields=ageRating");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string filmsList = await response.Content.ReadAsStringAsync();

            return filmsList;
        }

        public async Task<string> GetFilmStaff(long film_id)
        {
            var request = CreateRequest("https://api.kinopoisk.dev/v1.4/movie?page=1&limit=250&selectFields=persons&id=" + $"{film_id}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string filmStaff = await response.Content.ReadAsStringAsync();

            return filmStaff;
        }

        public async Task<string> GetPersonsInfo(long ID)
        {
            var request = CreateRequest("https://api.kinopoisk.dev/v1.4/person?page=1&limit=10&selectFields=id&selectFields=name&selectFields=sex&selectFields=birthday&selectFields=birthPlace&" + $"id={ID}");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string person_info = await response.Content.ReadAsStringAsync();

            return person_info;
        }
    }
}
