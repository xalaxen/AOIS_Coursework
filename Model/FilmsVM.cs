using AOIS.Controller;
using Lab_3.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        }

        public async void FillFilmsList()
        {
            string response;
            RootObject rootObj;
            List<FilmJsonModel> filmsList;

            try
            {
                response = await requests.GetFilms(TOKEN, selectedGenre);
                rootObj = JsonConvert.DeserializeObject<RootObject>(response);
                filmsList = rootObj.Films;
            }
            catch
            {
                MessageBox.Show("Не удалось получить данные!");
                return;
            }

            if(filmsList.Count != 0)
            {
                foreach (var film in filmsList)
                {
                    Films.Add(film);
                }
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
