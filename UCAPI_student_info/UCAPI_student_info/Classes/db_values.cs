using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCAPI_student_info.Classes
{
    class db_values
    {
        public string name { get; set; }
        public string value { get; set; }
        public db_values(string name, string value)
        {
            this.name = name;
            this.value = value;
        }
        public db_values() { }
    }
}
