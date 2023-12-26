using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AOIS.Controller
{
    public class MakeRequests
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<string> GetGenres(string TOKEN)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.kinopoisk.dev/v1/movie/possible-values-by-field?field=genres.name");
            request.Headers.Add("X-API-KEY", TOKEN);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string genrs_list = await response.Content.ReadAsStringAsync();

            return genrs_list;
        }

        public async Task<string> GetCountries(string TOKEN)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.kinopoisk.dev/v1/movie/possible-values-by-field?field=countries.name");
            request.Headers.Add("X-API-KEY", TOKEN);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string countries_list = await response.Content.ReadAsStringAsync();

            return countries_list;
        }

        public async Task<string> GetFilms(string TOKEN, string genre, int page)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.kinopoisk.dev/v1.4/movie?"+ $"page={page}" + "&limit=100&selectFields=id&selectFields=name&selectFields=enName&selectFields=alternativeName&selectFields=year&selectFields=rating&selectFields=ageRating&selectFields=budget&selectFields=movieLength&selectFields=genres&selectFields=countries&selectFields=persons&selectFields=fees&isSeries=false" + $"&genres.name={genre}" + "&notNullFields=ageRating");
            request.Headers.Add("X-API-KEY", TOKEN);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string films_list = await response.Content.ReadAsStringAsync();

            return films_list;
        }

        public async Task<string> GetFilmStaff(string TOKEN, long film_id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.kinopoisk.dev/v1.4/movie?page=1&limit=250&selectFields=persons&id=" + $"{film_id}");
            request.Headers.Add("X-API-KEY", TOKEN);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string film_staff = await response.Content.ReadAsStringAsync();

            return film_staff;
        }

        public async Task<string> GetPersonsInfo(string TOKEN, long ID)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.kinopoisk.dev/v1.4/person?page=1&limit=10&selectFields=id&selectFields=name&selectFields=sex&selectFields=birthday&selectFields=birthPlace&" + $"id={ID}");
            request.Headers.Add("X-API-KEY", TOKEN);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string person_info = await response.Content.ReadAsStringAsync();

            return person_info;
        }
    }
}
