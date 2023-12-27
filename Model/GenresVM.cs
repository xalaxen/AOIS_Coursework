using AOIS.Controller;
using Lab_3.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AOIS.Model
{
    class GenresVM : INotifyPropertyChanged
    {
        private MakeRequests requests = ((App)Application.Current).Requests;
        private Genre selectedGenre;
        private ObservableCollection<Genre> genres = new ObservableCollection<Genre>();

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Genre> Genres
        {
            get { return genres; }
            set
            {
                if (genres != value)
                {
                    genres = value;
                    OnPropertyChanged(nameof(Genres));
                }
            }
        }

        public Genre SelectedGenre
        {
            get { return selectedGenre; }
            set
            {
                if (selectedGenre != value)
                {
                    selectedGenre = value;
                    OnPropertyChanged(nameof(SelectedGenre));
                }
            }
        }

        public GenresVM()
        {
            FillGenresList();
        }

        // Заполняет массив жанрами из БД, если на вход не подан другой массив жанров, иначе же обновляет данные
        private void FillGenresList(List<Genre> genreList = null)
        {
            if (genreList != null)  // случай с загрузкой новых данных в бд
            {
                MessageBox.Show("Загрузка данных началась!");
                fillDataBase(genreList);
                MessageBox.Show("Загрузка данных завершилась!");
            }
            else // случай с загрузкой данных из бд
            {
                loadFromDataBase();
            }

            OnPropertyChanged(nameof(Genres));
        }

        private int loadFromDataBase()
        {
            using (var context = new KinoPoistEntities())
            {
                foreach (var genre in context.Genres)
                {
                    genres.Add(genre);
                }
            }

            return 0;
        }

        private int fillDataBase(List<Genre> genreList)
        {
            using (var context = new KinoPoistEntities())
            {
                foreach (var genre in genreList)
                {
                    genres.Add(genre);

                    if (!context.Genres.Any(g=>g.name == genre.name))
                    {
                        context.Genres.Add(genre);
                    }
                }
                context.SaveChanges();
            }

            return 0;
        }

        // Обновление списка жанров полученных из API запроса и добавление их в БД через FillGenresList
        public async void UpdateGenresList()
        {
            List<Genre> genreList;

            try
            {
                string response = await requests.GetGenres();
                genreList = JsonConvert.DeserializeObject<List<Genre>>(response);
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Не удалось получить данные!\n{ex.Message}");
                return;
            }
            catch (JsonException ex)
            {
                MessageBox.Show($"Не удалось разобрать json строку.\n{ex.Message}");
                return;
            }

            FillGenresList(genreList);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
