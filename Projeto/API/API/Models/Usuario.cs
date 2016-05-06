using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models
{
    public class Usuario:  MongoBase<Usuario>
    {
        public Usuario() { }
        public Usuario(String ID) : base(ID) { }

        public String Foto { get; set; }

        public String Nome { get; set; }

        public String Email { get; set; }

        public String Senha { get; set; }

        public String ChaveNotificacao { get; set; }

        [DBIgnoreMongo]
        public long QuantidadePosts {
            get
            {
                Post P = new Post();
                return P.QuantidadeUsuario(ID);
            }
        }

        public bool Login(String Email, String Senha)
        {
            var Query = StartLINQ().Where(uu => uu.Email == Email && uu.Senha == Senha);

            if (Query.Count() > 0)
            {
                IncorporateData(Query.First());
                EndDB();
                return true;
            }
            else
            {
                EndDB();
                return false;
            }

        }
    }
}
