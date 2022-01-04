using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using MecWise.Blazor.Api.Services;

using Newtonsoft.Json.Linq;
using System.Web;
using ePlatform.Security;
using MecWise.Blazor.Common;
using System.Globalization;
using MecWise.Blazor.Api.Filters;

namespace MecWise.Blazor.Api.Controllers
{

    public class LoginController : ApiController
    {
        LoginService _loginService;

        public LoginController() {
            string[] sessionHeader = System.Web.HttpContext.Current.Request.Headers.GetValues("Session");
            if (sessionHeader != null) {
                string jsonStr = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(sessionHeader[0]));
                SessionState session = JObject.Parse(jsonStr).ToObject<SessionState>();
                _loginService = new LoginService(session);
            }
            else {
                _loginService = new LoginService();
            }
        }

        [Authorize]
        [GzipCompression]
        [HttpGet]
        [Route("Login/GetUserProfile/{userId}")]
        public HttpResponseMessage GetUserProfile(string userId)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, JArray.FromObject(_loginService.GetUserProfiles(userId)));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Authorize]
        [GzipCompression]
        [HttpGet]
        [Route("Login/GetCompProfiles")]
        public HttpResponseMessage GetCompProfiles()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, JArray.FromObject(_loginService.GetCompProfiles()));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Authorize]
        [GzipCompression]
        [HttpPost]
        [Route("Login/GeneratePushNotiReg")]
        public async Task<HttpResponseMessage> GeneratePushNotiReg()
        {
            string value = await Request.Content.ReadAsStringAsync();
            try
            {
                JObject postData = JObject.Parse(value);
                return Request.CreateResponse(HttpStatusCode.OK, _loginService.GeneratePushNotiReg(postData));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Authorize]
        [GzipCompression]
        [HttpGet]
        [Route("Login/GetPushNotiReg/{compCode}/{empeId}")]
        public HttpResponseMessage GetPushNotiReg(string compCode, string empeId)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _loginService.GetPushNotiReg(compCode, empeId));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Authorize]
        [GzipCompression]
        [HttpPost]
        [Route("Login/UpdatePushNotiReg")]
        public async Task<HttpResponseMessage> UpdatePushNotiReg()
        {
            string value = await Request.Content.ReadAsStringAsync();
            try
            {
                JObject postData = JObject.Parse(value);
                return Request.CreateResponse(HttpStatusCode.OK, _loginService.UpdatePushNotiReg(postData));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Authorize]
        [GzipCompression]
        [HttpPost]
        [Route("Login/CheckToken")]
        public async Task<HttpResponseMessage> CheckToken()
        {
            string contentStr = await Request.Content.ReadAsStringAsync();
            try
            {
                JObject postData = JObject.Parse(contentStr);
                return Request.CreateResponse(HttpStatusCode.OK, _loginService.CheckToken(postData["token"].ToStr(), (LoginType)postData["tokenType"].ToInt()));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [Authorize]
        [GzipCompression]
        [HttpPost]
        [Route("Login/ChangePassword")]
        public async Task<HttpResponseMessage> ChangePassword() {
            string contentStr = await Request.Content.ReadAsStringAsync();
            try {
                JObject postData = JObject.Parse(contentStr);
                string newPwd = postData["newPwd"].ToStr();
                return Request.CreateResponse(HttpStatusCode.OK, _loginService.ChangePassword(newPwd));
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [GzipCompression]
        [HttpPost]
        [Route("Login/ResetPassword")]
        public async Task<HttpResponseMessage> ResetPassword() {
            string contentStr = await Request.Content.ReadAsStringAsync();
            try {
                JObject postData = JObject.Parse(contentStr);
                string userId = postData["userId"].ToStr();
                return Request.CreateResponse(HttpStatusCode.OK, _loginService.ResetPassword(userId));
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

    }
}
