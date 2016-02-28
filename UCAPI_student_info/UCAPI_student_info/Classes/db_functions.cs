using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCAPI_student_info.Classes
{
    class db_functions
    {
        public List<db_document> users { get; set; }
        
        public async Task setup_users()
        {
            users = new List<db_document>();
            var client = new MongoClient("mongodb://localhost:27017");
            var db = client.GetDatabase("db");
            var users_from_db = db.GetCollection<BsonDocument>("users");
            var documents = await users_from_db.Find(new BsonDocument()).ToListAsync();
            foreach (BsonDocument doc in documents)
            {
                var dbdoc = new db_document((doc.GetElement("_id").Value).ToString());
                
                var i = 0;
                while(i<doc.Values.ToArray().Length)
                {
                    string var_name = doc.Names.ToList().ToArray()[i].ToString();
                    string var_val = doc.Values.ToList().ToArray()[i].ToString();
                    //var val = new db_values((doc.Names.ToArray()[i].ToBson()).ToString(), (doc.Values.ToArray()[i].ToBson()).ToString());
                    var val = new db_values(var_name, var_val);
                    dbdoc.variables.Add(val);
                    i++;
                }
                dbdoc = set_variables(dbdoc);
                users.Add(dbdoc);
            }
        }

        public db_document set_variables(db_document doc)
        {
            var variables = doc.variables;
            foreach (db_values val in variables)
            {
                switch (val.name)
                {
                    case "username":
                        doc.username = val.value;
                        break;
                    case "password":
                        doc.password = val.value;
                        break;
                    case "_id":
                        doc.id = val.value;
                        break;
                    default:
                        break;
                }
            }
            return doc;
        }

    }
}
