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

namespace AOIS.View
{
    /// <summary>
    /// Логика взаимодействия для CreateReports.xaml
    /// </summary>
    public partial class CreateReports : Window
    {
        private ReportsVM model;
        public CreateReports(PersonJsonModel selectedPerson)
        {
            InitializeComponent();
            model = new ReportsVM(selectedPerson, ref comboBox);
            DataContext = model;
        }
    }
}
