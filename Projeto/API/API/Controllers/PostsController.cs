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
    public class PostsController : ApiController
    {
        [HttpPost]
        [Route("api/posts")]
        public async Task<HttpResponseMessage> Salva([FromBody]Post POST)
        {
            if (!POST.HasData)
                POST.Data = DateTime.Now;

            await POST.Save();
            return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.OK, POST));
        }

        [HttpGet]
        [Route("api/posts")]
        public async Task<HttpResponseMessage> Listar(int Pagina) // FEED
        {
            Post P = new Post();
            return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.OK, P.Lista(Pagina)));
        }

        [HttpGet]
        [Route("api/posts/usuario/{UsuarioID}")]
        public async Task<HttpResponseMessage> Listar(string UsuarioID) 
        {
            Post P = new Post();
            return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.OK, P.ListaUsuario(UsuarioID)));
        }

        [HttpDelete]
        [Route("api/posts/{id}")]
        public async Task<HttpResponseMessage> Excluir(string ID)
        {
            Post P = new Post(ID);
            await P.Delete();
            return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.NoContent));
        }

    }
}
