using AOIS.Controller;
using Lab_3.Classes;
using Newtonsoft.Json;
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

        public async void FillGenresList()
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

            foreach (var genre in genreList)
            {
                genres.Add(genre);
            }

            OnPropertyChanged(nameof(Genres));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
