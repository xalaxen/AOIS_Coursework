using AOIS.Controller;
using Lab_3.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AOIS.Model
{
    class StaffVM : INotifyPropertyChanged
    {
        string TOKEN = SaveAndLoad.LoadFromFile<string>("D:/Задания и записи/Архитура ИС/coursework/token.json")[0];
        MakeRequests requests = new MakeRequests();
        public event PropertyChangedEventHandler PropertyChanged;

        // этот массив нужен для того, чтобы получить полную нужную информацию о человеке, тк в этом классе храниться не вся нужная информация
        List<Person> rawPersons = new List<Person>();
        long film_id;
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
        public long Film_id
        {
            get { return film_id; }
            set
            {
                film_id= value;
            }
        }

        public StaffVM(List<Person> people, long nfilm_id)
        {
            //rawPersons = people;
            film_id = nfilm_id;
            FillPersonsInfo(film_id);
            if(Persons.Count == 0)
            {
                UpdatePersonsInfo();
            }
        }

        public void FillPersonsInfo(long film_id, List<PersonJsonModel> persons = null)
        {
            if(persons != null) // случай с загрузкой новых данных в бд
            {
                using (var context = new KinoPoistEntities())
                {
                    foreach(var person in persons)
                    {
                        Film_Staff film_Staff = new Film_Staff
                        {
                            person_id = person.id,
                            name = person.name,
                            birthday = person.birthDay,
                            birthPlace = person.birthPlace != null && person.birthPlace.Any() ? person.birthPlace.Last().Value : "н/д",
                            sex = person.sex != null ? person.sex : "н/д"
                        };

                        // добавление стран, если чегото не хватает
                        if (person.birthPlace != null)
                        {
                            foreach (var country in person.birthPlace)
                            {
                                var existingCountry = context.Countries.FirstOrDefault(c => c.name == country.Value);
                                if (existingCountry == null)
                                {
                                    context.Countries.Add(new Country { name = country.Value });
                                }
                            }
                        }

                        // заполнение словаря ролей
                        var existingRole = context.Roles.FirstOrDefault(r => r.name == person.profession);
                        if(existingRole == null) { context.Roles.Add(new Role { name = person.profession }); }

                        // добавление связи с фильмом
                        context.Staff_in_film.AddOrUpdate(new Staff_in_film
                        {
                            film_id = film_id,
                            person_id = film_Staff.person_id,
                            role_name = person.profession
                        });

                        context.Film_Staff.AddOrUpdate(film_Staff);
                        Persons.Add(person);
                        context.SaveChanges();
                    }
                }
            }
            else // случай с загрузкой данных из бд
            {
                using(var context = new KinoPoistEntities())
                {
                    // получаем состав фильма
                    var personsInFilm = context.Film_Staff 
                        .Where(person => person.Staff_in_film.Any(pf => pf.film_id == film_id))
                        .ToList();

                    foreach(var person in personsInFilm)
                    {
                        // получаем роль в фильме
                        var staffRolesInFilm = person.Staff_in_film.FirstOrDefault(pf => pf.film_id == film_id);
                        // заполняем сущьность
                        if(staffRolesInFilm != null)
                        {
                            Persons.Add(new PersonJsonModel
                            {
                                id = person.person_id,
                                name = person.name,
                                birthDay = person.birthday,
                                birthPlace = new List<BPlace> { new BPlace { Value = person.birthPlace } },
                                sex = person.sex,
                                profession = staffRolesInFilm.role_name
                            });
                        }
                    }
                }
            }

            OnPropertyChanged(nameof(Persons));
        }

        public async void UpdatePersonsInfo()
        {
            List<PersonJsonModel> people = new List<PersonJsonModel>();
            await GetSelectedFilmStaff();
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
            FillPersonsInfo(film_id, people);
            OnPropertyChanged(nameof(Persons));
        }

        // получаем съемочную группу фильма, для передачи их айдишников в другой VM
        public async Task GetSelectedFilmStaff()
        {
            try
            {
                string response = await requests.GetFilmStaff(TOKEN, film_id);
                RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(response);
                List<FilmJsonModel> root = rootObject.Films;
                rawPersons = root[0].Persons;
            }
            catch
            {
                MessageBox.Show("Не удалось получить команду фильма");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
