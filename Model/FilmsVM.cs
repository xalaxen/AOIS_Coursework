using AOIS.Controller;
using Lab_3.Classes;
using Newtonsoft.Json;
using ProductsDelliverySystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AOIS.Model
{
    class FilmsVM : INotifyPropertyChanged
    {
        string TOKEN = SaveAndLoad.LoadFromFile<string>("D:/Задания и записи/Архитура ИС/coursework/token.json")[0];
        public event PropertyChangedEventHandler PropertyChanged;
        MakeRequests requests = new MakeRequests();

        FilmJsonModel selectedFilm;
        string selectedGenre;
        ObservableCollection<FilmJsonModel> films = new ObservableCollection<FilmJsonModel>();

        public FilmJsonModel SelectedFilm
        {
            get { return selectedFilm; }
            set
            {
                if (value != selectedFilm)
                {
                    selectedFilm = value;
                    OnPropertyChanged(nameof(SelectedFilm));
                }
            }
        }

        public ObservableCollection<FilmJsonModel> Films
        {
            get { return films; }
            set
            {
                if(films != value)
                {
                    films = value;
                    OnPropertyChanged(nameof(Films));
                }

            }
        }

        public string SelectedGenre
        {
            get { return SelectedGenre; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    selectedGenre = value;
                    OnPropertyChanged(nameof(SelectedGenre));
                }
            }
        }

        public FilmsVM(string selectedGenre)
        {
            SelectedGenre = selectedGenre;
            FillFilmsList();
            if (Films.Count == 0)
            {
                InitAsync();
            }
        }

        private async void InitAsync()
        {
            await UpdateFilmsList(1);
        }

        public void FillFilmsList(List<FilmJsonModel> filmsList = null)
        {
            if(filmsList != null) // случай с загрузкой новых данных в бд
            {
                using(var context = new KinoPoistEntities())
                {
                    foreach(var film in filmsList)
                    {
                        Film newFilm = new Film
                        {
                            film_id = film.Id,
                            name = film.Name,
                            year = film.Year,
                            raiting = film.Rating.Kp,
                            ageRating = film.AgeRating,
                            budget = film.Budget != null ? film.Budget.Value : 0,
                            budget_currency = film.Budget?.Currency ?? "#",
                            movieLenght = film.MovieLength,
                            fees = film.Fees.World != null ? film.Fees.World.Value : ( film.Fees.Russia != null ? film.Fees.Russia.Value : 0 ),
                        };

                        // чтобы точно все было хорошо
                        if (film.Budget != null && film.Budget.Currency is string currency)
                        {
                            newFilm.budget_currency = currency.Length > 0 ? currency.Substring(0, 1) : "#";
                        }
                        else
                        {
                            newFilm.budget_currency = "#";
                        }

                        // Проверка существования стран и добавление их в контекст
                        foreach (var country in film.Countries)
                        {
                            var existingCountry = context.Countries.FirstOrDefault(c => c.name == country.name);
                            if (existingCountry == null)
                            {
                                context.Countries.Add(country);
                            }
                            else
                            {
                                newFilm.Countries.Add(existingCountry);
                            }
                        }

                        // Проверка существования жанров и добавление их в контекст
                        foreach (var genre in film.Genres)
                        {
                            var existingGenre = context.Genres.FirstOrDefault(g => g.name == genre.name);
                            if (existingGenre == null)
                            {
                                context.Genres.Add(genre);
                            }
                            else
                            {
                                newFilm.Genres.Add(existingGenre);
                            }
                        }

                        context.Films.AddOrUpdate(f => f.film_id, newFilm);
                        Films.Add(film);
                        context.SaveChanges();                       
                    }
                }
            }
            else // случай с загрузкой данных из бд
            {
                using (var context = new KinoPoistEntities())
                {
                    if (context.Films.Any() == true)
                    {
                        foreach (var film in context.Films)
                        {
                            List<Country> countries = film.Countries.ToList();
                            List<Genre> genres = film.Genres.ToList();
                            List<Person> people = new List<Person>();

                            foreach (var person in film.Staff_in_film)
                            {
                                people.Add(new Person
                                {
                                    Id = person.person_id,
                                    Profession = person.role_name
                                });
                            }

                            FilmJsonModel newfilm = new FilmJsonModel(
                                film.film_id,
                                film.name,
                                film.year,
                                film.raiting,
                                film.ageRating,
                                film.budget,
                                film.budget_currency,
                                film.movieLenght,
                                film.fees,
                                genres,
                                countries,
                                people
                                );

                            Films.Add(newfilm);
                        }
                    }
                }
            }

            OnPropertyChanged(nameof(Films));
        }

        public async Task UpdateFilmsList(int page = 1)
        {
            int currentPage = 1;
            List<FilmJsonModel> allFilmsList = new List<FilmJsonModel>();

            try
            {
                while (currentPage <= page)
                {
                    string response = await requests.GetFilms(TOKEN, selectedGenre, currentPage);
                    RootObject rootObj = JsonConvert.DeserializeObject<RootObject>(response);
                    List<FilmJsonModel> filmsList = rootObj.Films;
                    
                    if (filmsList.Count == 0)
                    {
                        break;
                    }

                    allFilmsList.AddRange(filmsList);
                    currentPage++;
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Не удалось получить данные!");
                return;
            }
            catch (JsonException ex)
            {
                MessageBox.Show("Не удалось разобрать json строку.");
                return;
            }

            if (allFilmsList.Count != 0)
            {
                FillFilmsList(allFilmsList);
            }
            else
            {
                MessageBox.Show("Нет фильмов в этой категории!");
            }

            OnPropertyChanged(nameof(Films));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
