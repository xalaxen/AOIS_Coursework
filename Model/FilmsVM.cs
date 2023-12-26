using AOIS.Controller;
using Lab_3.Classes;
using Newtonsoft.Json;
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
        int pageNumber = 1;
        string selectedGenre;
        ObservableCollection<FilmJsonModel> films = new ObservableCollection<FilmJsonModel>();

        public int PageNumber
        {
            get { return pageNumber; }
            set
            {
                if(pageNumber != value)
                {
                    pageNumber = Convert.ToInt32(value);
                    OnPropertyChanged(nameof(PageNumber));
                }
            }
        }

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
            // загружаем данные из бд
            FillFilmsList();
            // если ничего там нет, загружаем из интернета
            if (Films.Count == 0)
            {
                InitAsync();
            }
        }

        private async void InitAsync()
        {
            await UpdateFilmsList(pageNumber);
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
                            fees = film.Fees != null ? (film.Fees.World != null ? film.Fees.World.Value : ( film.Fees.Russia != null ? film.Fees.Russia.Value : 0 )) : 0,
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

                        // добавляем новый или обновляем старый, если что-то изменилось
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
                    // подгружаем все фильмы с указанным жанром
                    var films = context.Films
                                       .Where(film => film.Genres.Any(g => g.name == selectedGenre))
                                       .ToList();

                    // формирование списка для вывода в представление
                    foreach (var film in films)
                    {
                        var countries = film.Countries.ToList();
                        var genres = film.Genres.ToList();
                        var people = film.Staff_in_film.Select(person => new Person
                        {
                            Id = person.person_id,
                            Profession = person.role_name
                        }).ToList();

                        var newfilm = new FilmJsonModel(
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

            OnPropertyChanged(nameof(Films));
        }

        public async Task UpdateFilmsList(int page = 1)
        {
            List<FilmJsonModel> allFilmsList = new List<FilmJsonModel>();

            try
            {
                // загрузка всех фильмов до какой-то страницы (по умолчанию только 1)
                string response = await requests.GetFilms(TOKEN, selectedGenre, page);
                RootObject rootObj = JsonConvert.DeserializeObject<RootObject>(response);
                List<FilmJsonModel> filmsList = rootObj.Films;

                // если фильмов почему-то нет (дада вроде и такое может быть)
                if (filmsList.Count == 0)
                {
                    return;
                }

                allFilmsList.AddRange(filmsList);
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
