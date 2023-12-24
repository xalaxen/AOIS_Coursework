using AOIS.Controller;
using Lab_3.Classes;
using Newtonsoft.Json;
using ProductsDelliverySystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AOIS.Model
{
    class GenresVM : INotifyPropertyChanged
    {
        string TOKEN = SaveAndLoad.LoadFromFile<string>("D:/Задания и записи/Архитура ИС/coursework/token.json")[0];
        public event PropertyChangedEventHandler PropertyChanged;
        MakeRequests requests = new MakeRequests();

        Genre selectedGenre;
        ObservableCollection<Genre> genres = new ObservableCollection<Genre>();

        public ObservableCollection<Genre> Genres 
        {  
            get { return genres; }
            set 
            { 
                if(genres != value)
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
                if(selectedGenre != value)
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
        public void FillGenresList(List<Genre> genreList = null)
        {
            if(genreList != null)  // случай с загрузкой новых данных в бд
            {
                using (var context = new KinoPoistEntities())
                {
                    foreach (var genre in genreList)
                    {
                        genres.Add(genre);

                        if (!context.Genres.Contains(genre))
                        {
                            context.Genres.Add(genre);
                        }
                    }
                    context.SaveChanges();
                }
            }
            else // случай с загрузкой данных из бд
            {
                using(var context = new KinoPoistEntities())
                {
                    foreach(var genre in context.Genres)
                    {
                        genres.Add(genre);
                    }
                }
            }

            OnPropertyChanged(nameof(Genres));
        }

        // Обновление списка жанров полученных из API запроса и добавление их в БД через FillGenresList
        public async void UpdateGenresList()
        {
            string response;
            List<Genre> genreList;

            try
            {
                response = await requests.GetGenres(TOKEN);
                genreList = JsonConvert.DeserializeObject<List<Genre>>(response);
            }
            catch
            {
                MessageBox.Show("Не удалось получить данные!");
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
