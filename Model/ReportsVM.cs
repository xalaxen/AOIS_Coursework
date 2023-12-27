using AOIS.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AOIS.Model
{
    public class ReportsVM : INotifyPropertyChanged
    {
        private PersonJsonModel selectedPerson;
        private ComboBox myComboBox;

        private Command topTenFilmsByAllCmd;
        private Command topTenFilmsByGenreCmd;
        private Command topTenFilmsByFeesCmd;
        private Command filmCountByYearCmd;

        public ReportsVM(PersonJsonModel person, ref ComboBox comboBox) 
        { 
            selectedPerson = person;
            myComboBox = comboBox;
            comboBox.ItemsSource = loadGenres();
        }

        private List<Genre> loadGenres()
        {
            List<Genre> genres = new List<Genre>();
            using (var context = new KinoPoistEntities())
            {
                foreach (var genre in context.Genres)
                {
                    genres.Add(genre);
                }
            }

            return genres;
        }
        
        public Command TopTenFilmsByAllCmd
        {
            get
            {
                return topTenFilmsByAllCmd ?? (topTenFilmsByAllCmd = new Command(obj =>
                {
                    List<Film> filmsWithHighestRating;
                    using (var context = new KinoPoistEntities())
                    {
                        filmsWithHighestRating = context.Staff_in_film
                        .Where(sf => sf.person_id == selectedPerson.id)
                        .OrderByDescending(sf => sf.Film.raiting)
                        .Select(sf => sf.Film)
                        .Take(10)
                        .ToList();
                    }

                    StringBuilder filmsListBuilder = new StringBuilder();
                    foreach (var f in filmsWithHighestRating)
                    {
                        filmsListBuilder.AppendLine($"• {f.name} — {f.raiting}");
                    }

                    string filmsList = filmsListBuilder.ToString().Trim();


                    MakeReports makeReports = new MakeReports("ReportTemplates\\TopTenFilmsByAllTemplate.doc");

                    makeReports.ReplaceBookmark("personName", selectedPerson.name);
                    makeReports.ReplaceBookmark("filmsList", filmsList);
                    makeReports.TrySave();
                    makeReports.Close();
                    MessageBox.Show("Новый отчет сохранен на рабочий стол.");
                }));
            }
        }
        
        public Command TopTenFilmsByGenreCmd
        {
            get
            {
                return topTenFilmsByGenreCmd ?? (topTenFilmsByGenreCmd = new Command(obj =>
                {
                    if(myComboBox.SelectedValue == null)
                    {
                        MessageBox.Show("Сначала выберите жанр!");
                        return;
                    }

                    List<Film> films;
                    string selectedGenreName = myComboBox.SelectedValue.ToString();

                    using (var context = new KinoPoistEntities())
                    {
                        films = context.Staff_in_film
                        .Where(sf => sf.person_id == selectedPerson.id && sf.Film.Genres.Any(g => g.name == selectedGenreName))
                        .OrderByDescending(sf => sf.Film.raiting)
                        .Select(sf => sf.Film)
                        .Take(10)
                        .ToList();
                    }

                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (var film in films)
                    {
                        stringBuilder.AppendLine($"• {film.name} — {film.raiting}");
                    }

                    string filmsList = stringBuilder.ToString().Trim();
                    
                    MakeReports makeReports = new MakeReports("ReportTemplates\\TopTenFilmsByGenreTemplate.doc");
                    makeReports.ReplaceBookmark("personName", selectedPerson.name);
                    makeReports.Replace("{film_genre}", selectedGenreName);
                    makeReports.Replace("{film_genre}", selectedGenreName);
                    makeReports.ReplaceBookmark("filmsList", filmsList);
                    makeReports.TrySave();
                    makeReports.Close();
                    MessageBox.Show("Новый отчет сохранен на рабочий стол.");
                }));
            }
        }

        public Command TopTenFilmsByFeesCmd
        {
            get
            {
                return topTenFilmsByFeesCmd ?? (topTenFilmsByFeesCmd = new Command(obj =>
                {
                    List<Film> films;

                    using (var context = new KinoPoistEntities())
                    {
                        films = context.Staff_in_film
                        .Where(sf => sf.person_id == selectedPerson.id)
                        .OrderByDescending(sf => sf.Film.fees)
                        .Select(sf => sf.Film)
                        .Take(10)
                        .ToList();
                    }

                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (var film in films)
                    {
                        stringBuilder.AppendLine($"• {film.name} — {film.fees}$");
                    }

                    string filmsList = stringBuilder.ToString().Trim();

                    MakeReports makeReports = new MakeReports("ReportTemplates\\TopTenFilmsByFeesTemplate.doc");
                    makeReports.Replace("{person_name}", selectedPerson.name);
                    makeReports.ReplaceBookmark("filmsList", filmsList);
                    makeReports.TrySave();
                    makeReports.Close();
                    MessageBox.Show("Новый отчет сохранен на рабочий стол.");
                }));
            }
        }

        public Command FilmCountByYearCmd
        {
            get
            {
                return filmCountByYearCmd ?? (filmCountByYearCmd = new Command(obj =>
                {
                    MakeReports makeReports = new MakeReports("ReportTemplates\\FilmsCountByYearTemplate.doc");
                    Dictionary<int, int> filmsByYear;

                    using (var context = new KinoPoistEntities())
                    {
                        filmsByYear = context.Staff_in_film
                            .Where(sf => sf.person_id == selectedPerson.id)
                            .GroupBy(sf => sf.Film.year)
                            .Select(group => new { Year = group.Key, FilmCount = group.Count() })
                            .ToDictionary(item => item.Year, item => item.FilmCount);
                    }

                    // Создаем график и сохраняем изображение
                    Microsoft.Office.Interop.Excel.Chart chart = makeReports.GenerateFilmCountByYearChart(filmsByYear);
                    string chartImagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ChartImage.png");
                    makeReports.SaveChartImage(chartImagePath, chart);

                    // Заменяем переменные в документе
                    makeReports.Replace("{person_name}", selectedPerson.name);
                    makeReports.Replace("{person_name}", selectedPerson.name);

                    // Вставляем график в Word
                    makeReports.InsertChartIntoWord(chartImagePath);

                    // Сохраняем и закрываем
                    makeReports.TrySave();
                    makeReports.Close();

                    MessageBox.Show("Новый отчет сохранен на рабочий стол.");
                }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
