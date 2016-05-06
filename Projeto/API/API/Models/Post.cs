using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models
{
    public class Post :  MongoBase<Post>
    {
        public Post() { }
        public Post(String ID) : base(ID) { }

        [DBIndex]
        public String UsuarioID { get; set; }

        [DBIgnoreMongo]
        public Usuario UsuarioDados {
            get {
                Usuario U = new Usuario(UsuarioID);
                U.Senha = "";
                return U;
            }
        }


        public String Imagem { get; set; }
        public String Descricao { get; set; }

        [DBIndex]
        public DateTime Data { get; set; }

        public List<Post> Lista(int Pagina)
        {
            var Query = StartLINQ().OrderByDescending(oo => oo.Data).Skip((Pagina - 1) * 10).Take(10);
            return Query.ToList();
            EndDB();
        }
        public List<Post> ListaUsuario(String UsuarioID)
        {
            var Query = StartLINQ().Where(pp=>pp.UsuarioID == UsuarioID).OrderByDescending(oo => oo.Data);
            return Query.ToList();
            EndDB();
        }

        public long QuantidadeUsuario(String UsuarioID)
        {
            return StartLINQ().Count(pp => pp.UsuarioID == UsuarioID);
            EndDB();
        }
    }
}
