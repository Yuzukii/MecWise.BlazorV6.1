using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;
using MecWise.Blazor.Api.Services;
using System.IO;
using MecWise.Blazor.Common;

namespace MecWise.Blazor.Api.Controllers
{
    [Authorize]
    public class CommonController : ApiController
    {
        CommonService _commonService;

        public CommonController() {
            string[] sessionHeader = System.Web.HttpContext.Current.Request.Headers.GetValues("Session");
            if (sessionHeader != null) {
                string jsonStr = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(sessionHeader[0]));
                SessionState session = JObject.Parse(jsonStr).ToObject<SessionState>();
                _commonService = new CommonService(session);
            }
            else {
                _commonService = new CommonService();
            }
        }

        [HttpPost]
        [Route("Common/GetV5NavigationKey")]
        public async Task<HttpResponseMessage> GetNavigationKey() {
            string value = await Request.Content.ReadAsStringAsync();
            try {
                JObject postData = JObject.Parse(value);
                return Request.CreateResponse(HttpStatusCode.OK, _commonService.GetV5NavigationKey(postData));
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpPost]
        [Route("Common/RememberUserToken")]
        public async Task<HttpResponseMessage> RememberUserToken()
        {
            string value = await Request.Content.ReadAsStringAsync();
            try
            {
                JObject postData = JObject.Parse(value);
                return Request.CreateResponse(HttpStatusCode.OK, _commonService.RememberUserToken(postData));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
