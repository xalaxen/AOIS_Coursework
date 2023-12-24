using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public Fees Fees { get; set; }
        public Rating Rating { get; set; }
        public int MovieLength { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public Budget Budget { get; set; }
        public List<Genres> Genres { get; set; }
        public List<Countries> Countries { get; set; }
        public List<Person> Persons { get; set; }
        public string AlternativeName { get; set; }
        public string EnName { get; set; }
        public int AgeRating { get; set; }
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
        public float Kp { get; set; }
        public float Imdb { get; set; }
        public float FilmCritics { get; set; }
        public float RussianFilmCritics { get; set; }
        public object Await { get; set; }
    }

    public class Budget
    {
        public int Value { get; set; }
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
