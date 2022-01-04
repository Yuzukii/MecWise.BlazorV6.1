using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using System.Net;

namespace MecWise.Blazor.Api.MicroService.Controllers
{
    public class TestController : ApiController
    {
        [Route(""), HttpGet]
        public HttpResponseMessage Index() {

            return Request.CreateResponse(HttpStatusCode.OK, "Service is running normally...");
        }
    }
}
