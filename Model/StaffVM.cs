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
            if(Persons.Count == 0)
            {
                UpdatePersonsInfo();
            }
        }

        public void FillPersonsInfo(List<PersonJsonModel> persons = null)
        {
            if(persons != null) // случай с загрузкой новых данных в бд
            {
                using (var context = new KinoPoistEntities())
                {
                    foreach(var person in persons)
                    {
                        if(context.Film_Staff.Find(person.id) == null)
                        {
                            // заполнение словаря ролей
                            if (context.Roles.Find(person.profession) == null)
                            {
                                context.Roles.Add(new Role { name = person.profession });
                            }

                            // если там в джсоне намесили чего непонятного
                            if (context.Countries.Find(person.birthPlace[0].Value) == null)
                            {
                                context.Countries.Add(new Country { name = person.birthPlace[0].Value });
                            }

                            context.Film_Staff.Add(new Film_Staff
                            {
                                person_id = person.id,
                                name = person.name,
                                birthday = person.birthDay,
                                birthPlace = person.birthPlace[0].Value,
                                sex = person.sex
                            });

                            Persons.Add(person);
                            context.SaveChanges();
                        }
                    }
                }
            }
            else // случай с загрузкой данных из бд
            {
                using(var context = new KinoPoistEntities())
                {
                    foreach(var person in context.Film_Staff)
                    {
                        Persons.Add(new PersonJsonModel
                        {
                            id = person.person_id,
                            name = person.name,
                            birthDay = person.birthday,
                            birthPlace = new List<BPlace> { new BPlace { Value = person.birthPlace } },
                            sex = person.sex
                        });
                    }
                }
            }

            OnPropertyChanged(nameof(Persons));
        }

        public async void UpdatePersonsInfo()
        {
            List<PersonJsonModel> people = new List<PersonJsonModel>();
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
                        people.Add(rootObjectStaff.PersonJsonModel[0]);
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
            FillPersonsInfo(people);
            OnPropertyChanged(nameof(Persons));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
