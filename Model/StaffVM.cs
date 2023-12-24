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

namespace AOIS.Model
{
    class StaffVM : INotifyPropertyChanged
    {
        string TOKEN = SaveAndLoad.LoadFromFile<string>("D:/Задания и записи/Архитура ИС/coursework/token.json")[0];
        MakeRequests requests = new MakeRequests();
        public event PropertyChangedEventHandler PropertyChanged;

        // этот массив нужен для того, чтобы получить полную нужную информацию о человеке, тк в этом классе храниться не вся нужная информация
        List<Person> rawPersons = new List<Person>();

        PersonJsonModel selectedPerson;
        ObservableCollection<PersonJsonModel> persons = new ObservableCollection<PersonJsonModel>();

        public ObservableCollection<PersonJsonModel> Persons
        {
            get { return persons; }
            set
            {
                if (persons != value)
                {
                    persons = value;
                    OnPropertyChanged(nameof(Persons));
                }
            }
        }

        public PersonJsonModel SelectedPerson
        {
            get { return selectedPerson; }
            set
            {
                if (selectedPerson != value)
                {
                    selectedPerson = value;
                    OnPropertyChanged(nameof (SelectedPerson));
                }
            }
        }

        public StaffVM(List<Person> people)
        {
            rawPersons = people;
            FillPersonsInfo();
        }

        public async void FillPersonsInfo()
        {
            foreach (Person person in rawPersons)
            {
                try
                {
                    string response = await requests.GetPersonsInfo(TOKEN, person.Id);
                    // Если у человека нет обязательных полей, по типу даты или места рождения, пропускаем его
                    try
                    {
                        RootObjectStaff rootObjectStaff = JsonConvert.DeserializeObject<RootObjectStaff>(response);
                        rootObjectStaff.PersonJsonModel[0].profession = person.Profession;
                        Persons.Add(rootObjectStaff.PersonJsonModel[0]);
                    }
                    catch
                    {
                        continue;
                    }
                }
                catch
                {
                    continue;
                }
            }
            OnPropertyChanged(nameof(Persons));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
