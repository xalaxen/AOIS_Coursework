using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOIS.Model
{
    public class RootObjectStaff
    {
        [JsonProperty("docs")]
        public List<PersonJsonModel> PersonJsonModel { get; set; }
    }

    public class PersonJsonModel
    {
        public long id { get; set; }
        public string name { get; set; }
        public List<BPlace> birthPlace { get; set; }
        public DateTime birthDay { get; set; }
        public string sex { get; set; }
        public string profession { get; set; }
    }

    public class BPlace
    {
        public string Value { get; set; }
    }
}
