//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AOIS.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Film
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Film()
        {
            this.Staff_in_film = new HashSet<Staff_in_film>();
            this.Countries = new HashSet<Country>();
            this.Genres = new HashSet<Genre>();
        }
    
        public long film_id { get; set; }
        public string name { get; set; }
        public int year { get; set; }
        public double raiting { get; set; }
        public int ageRating { get; set; }
        public long budget { get; set; }
        public int movieLenght { get; set; }
        public long fees { get; set; }
        public string budget_currency { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Staff_in_film> Staff_in_film { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Country> Countries { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Genre> Genres { get; set; }
    }
}
