using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace API.Controllers
{
    [EnableCors("*", "*", "*")]
    public class UsuariosController : ApiController
    {
        [HttpPost]
        [Route("api/usuarios/login")]
        public async Task<HttpResponseMessage> Login(String Email, String Senha)
        {
            Usuario U = new Usuario();
            if (U.Login(Email, Senha)) {
                U.Senha = "";
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.OK, U));
            }

            return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.NotAcceptable));
        }

        [HttpPost]
        [Route("api/usuarios")]
        public async Task<HttpResponseMessage> Salva([FromBody]Usuario U)
        {
            if (U.HasData && String.IsNullOrEmpty(U.Senha))
            {
                //Recupera a senha
                Usuario U_ATUAL = new Usuario(U.ID);
                U.Senha = U_ATUAL.Senha;
            }

            await U.Save();
            U.Senha = "";

            return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.OK, U));
        }
    }
}
