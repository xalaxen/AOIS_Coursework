using AOIS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AOIS
{
    /// <summary>
    /// Логика взаимодействия для FilmsWindow.xaml
    /// </summary>
    public partial class FilmsWindow : Window
    {
        private FilmsVM model;
        public FilmsWindow(string selectedGenre)
        {
            InitializeComponent();
            model = new FilmsVM(selectedGenre);
            DataContext = model;
        }

        private void FilmStaffDetailsBtn(object sender, RoutedEventArgs e)
        {
            StaffDetailsWindow staffDetailsWindow = new StaffDetailsWindow(model.SelectedFilm.Persons);
            staffDetailsWindow.Show();
            staffDetailsWindow.Focus();
        }
    }
}
