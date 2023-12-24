using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AOIS.Model
{
    public class RootObject
    {
        [JsonProperty("docs")]
        public List<FilmJsonModel> Films { get; set; }
    }

    public class FilmJsonModel
    {
        public Fees Fees { get; set; }//
        public Rating Rating { get; set; }//
        public int MovieLength { get; set; }//
        public long Id { get; set; }//
        public string Name { get; set; }//
        public int Year { get; set; }//
        public Budget Budget { get; set; }//
        public List<Genre> Genres { get; set; }//
        public List<Country> Countries { get; set; }//
        public List<Person> Persons { get; set; }//
        public string AlternativeName { get; set; }//
        public string EnName { get; set; }//
        public int AgeRating { get; set; }//

        public FilmJsonModel() { }

        public FilmJsonModel(long film_id, string film_name, int release_year, double kpRating, int age_rating, long film_budget, string budget_currency, int film_lenght, long fees, List<Genre> film_genres, List<Country> film_countries, List<Person> film_people)
        {
            Id = film_id;
            Name = film_name;
            Year = release_year;
            Rating = new Rating { Kp = kpRating };
            AgeRating = age_rating;
            Budget = new Budget { Value = film_budget, Currency = budget_currency };
            MovieLength = film_lenght;
            Fees = new Fees { World = new WorldFee { Value = fees, Currency = "$" } };
            Genres = film_genres;
            Countries = film_countries;
            Persons = film_people;
        }
    }

    public class Fees
    {
        public WorldFee World { get; set; }
        public WorldFee Russia { get; set; }
        public WorldFee USA { get; set; }
    }

    public class WorldFee
    {
        public long Value { get; set; }
        public string Currency { get; set; }
    }

    public class Rating
    {
        public double Kp { get; set; }
        public double Imdb { get; set; }
        public double FilmCritics { get; set; }
        public double RussianFilmCritics { get; set; }
        public object Await { get; set; }
    }

    public class Budget
    {
        public long Value { get; set; }
        public string Currency { get; set; }
    }

    public class Genres
    {
        public string Name { get; set; }
    }

    public class Countries
    {
        public string Name { get; set; }
    }

    public class Person
    {
        public long Id { get; set; }
        public string Photo { get; set; }
        public string Name { get; set; }
        public string EnName { get; set; }
        public string Description { get; set; }
        public string Profession { get; set; }
        public string EnProfession { get; set; }
    }
}
