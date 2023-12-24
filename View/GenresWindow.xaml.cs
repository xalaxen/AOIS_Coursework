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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AOIS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GenresWindow : Window
    {
        private Model.GenresVM model;
        public GenresWindow()
        {
            InitializeComponent();
            model = new Model.GenresVM();
            DataContext = model;
        }

        private void OpenFilmsCategoryBtn(object sender, RoutedEventArgs e)
        {
            FilmsWindow filmsWindow = new FilmsWindow(model.SelectedGenre.name);
            filmsWindow.Show();
            filmsWindow.Focus();
        }
    }
}
