using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace MecWise.Blazor.Api.Controllers
{
    public class TestController : ApiController
    {
        [Route(""), HttpGet]
        public HttpResponseMessage Index()
        {
            
            return Request.CreateResponse(HttpStatusCode.OK, "Service is running normally...");
        }

    }
}
