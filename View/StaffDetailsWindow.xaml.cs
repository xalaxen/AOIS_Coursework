using AOIS.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для StaffDetailsWindow.xaml
    /// </summary>
    public partial class StaffDetailsWindow : Window
    {
        private StaffVM model;
        public StaffDetailsWindow(List<Person> people, long film_id)
        {
            InitializeComponent();
            model = new StaffVM(people, film_id);
            DataContext = model;
        }

        private void UdpateStaffDetailsInfoBtn(object sender, RoutedEventArgs e)
        {
            model.UpdatePersonsInfo();
        }
    }
}
