using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Newtonsoft.Json.Linq;
using MecWise.Blazor.Api.Services;
using MecWise.Blazor.Common;
using MecWise.Blazor.Api.Filters;

namespace MecWise.Blazor.Api.Controllers
{
    [Authorize]
    public class HomeController : ApiController
    {
        HomeService _homeService;

        public HomeController() {
            string[] sessionHeader = System.Web.HttpContext.Current.Request.Headers.GetValues("Session");
            if (sessionHeader != null) {
                string jsonStr = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(sessionHeader[0]));
                SessionState session = JObject.Parse(jsonStr).ToObject<SessionState>();
                _homeService = new HomeService(session);
            }
            else {
                _homeService = new HomeService();
            }
        }

        [GzipCompression]
        [HttpGet]
        [Route("Home/GetEmpeName/{compCode}/{empeId}")]
        public HttpResponseMessage GetEmpeName(string compCode, string empeId)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _homeService.GetEmpeName(compCode, empeId));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
