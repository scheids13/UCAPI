using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCAPI_student_info.Classes
{
    class db_document
    {
        public List<db_values> variables { set; get; }
        public string id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public db_document(string name, List<db_values> variables)
        {
            this.id = name;
            this.variables = variables;
        }
        public db_document(string name)
        {
            this.id = name;
            this.variables = new List<db_values>();
        }
        public db_document()
        {
            this.id = "";
            this.variables = new List<db_values>();
        }
    }
}
