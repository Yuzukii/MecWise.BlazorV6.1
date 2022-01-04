using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Newtonsoft.Json.Linq;
using MecWise.Blazor.Api.Services;
using System.Threading.Tasks;
using MecWise.Blazor.Entities;
using MecWise.Blazor.Common;
using MecWise.Blazor.Api.Filters;

namespace MecWise.Blazor.Api.Controllers
{
    [Authorize]
    public class EpfScreenController : ApiController
    {
        EpfScreenService _screenService;
        string _activeScrnId;

        public EpfScreenController() {
            string[] sessionHeader = System.Web.HttpContext.Current.Request.Headers.GetValues("Session");
            if (sessionHeader != null) {
                string jsonStr = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(sessionHeader[0]));
                SessionState session = JObject.Parse(jsonStr).ToObject<SessionState>();
                _activeScrnId = session.ActiveScrnId;
                _screenService = new EpfScreenService(session);
            }
            else {
                _screenService = new EpfScreenService();
            }
        }

        [GzipCompression]
        [HttpGet]
        [Route("EpfScreen/GetScreen/{scrnId}")]
        public async Task<HttpResponseMessage> GetScreen(string scrnId) {
            
            //Call microservice to handle request
            var microServiceResponse = await CallMicroServiceAsync(string.Format("EpfScreen/GetScreen/{0}", scrnId), HttpMethod.Get, scrnId);
            if (microServiceResponse != null) {
                return microServiceResponse;
            }

            //fallback if no response from microservice, handle request by this API Serivce
            try {
                return Request.CreateResponse(HttpStatusCode.OK, JArray.FromObject(_screenService.GetScreen(scrnId)));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [GzipCompression]
        [HttpGet]
        [Route("EpfScreen/GetRights/{mduleId}")]
        public async Task<HttpResponseMessage> GetRights(string mduleId) {

            //Call microservice to handle request
            var microServiceResponse = await CallMicroServiceAsync(string.Format("EpfScreen/GetRights/{0}", mduleId), HttpMethod.Get);
            if (microServiceResponse != null) {
                return microServiceResponse;
            }

            //fallback if no response from microservice, handle request by this API Serivce
            try {
                return Request.CreateResponse(HttpStatusCode.OK, _screenService.GetRights(mduleId));
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [GzipCompression]
        [HttpPost]
        [Route("EpfScreen/GetBrws")]
        public async Task<HttpResponseMessage> GetBrws()
        {

            //Call microservice to handle request
            var microServiceResponse = await CallMicroServiceAsync("EpfScreen/GetBrws", HttpMethod.Post);
            if (microServiceResponse != null) {
                return microServiceResponse;
            }

            //fallback if no response from microservice, handle request by this API Serivce
            string value = await Request.Content.ReadAsStringAsync();
            try {
                JObject postData = JObject.Parse(value);
                string baseView = postData["baseView"].ToString();
                string brwsId = postData["brwsId"].ToString();
                JObject data = JObject.FromObject(postData["dataSource"]);

                return Request.CreateResponse(HttpStatusCode.OK, _screenService.GetBrws(brwsId, data, baseView));
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [GzipCompression]
        [HttpGet]
        [Route("EpfScreen/GetColumnInfos/{viewName}")]
        public async Task<HttpResponseMessage> GetColumnInfos(string viewName) {

            //Call microservice to handle request
            var microServiceResponse = await CallMicroServiceAsync(string.Format("EpfScreen/GetColumnInfos/{0}", viewName), HttpMethod.Get);
            if (microServiceResponse != null) {
                return microServiceResponse;
            }

            //fallback if no response from microservice, handle request by this API Serivce
            try {
                return Request.CreateResponse(HttpStatusCode.OK, JObject.FromObject(_screenService.GetColumnInfos(viewName)));
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [GzipCompression]
        [HttpPost]
        [Route("EpfScreen/GetCodeDesc")]
        public async Task<HttpResponseMessage> GetCodeDesc() {

            //Call microservice to handle request
            var microServiceResponse = await CallMicroServiceAsync("EpfScreen/GetCodeDesc", HttpMethod.Post);
            if (microServiceResponse != null) {
                return microServiceResponse;
            }

            //fallback if no response from microservice, handle request by this API Serivce
            string value = await Request.Content.ReadAsStringAsync();
            try {
                JObject postData = JObject.Parse(value);
                string sqlDesc = postData["sqlDesc"].ToString();
                JObject data = JObject.FromObject(postData["dataSource"]);
                Dictionary<string, EPF_BRWS_COL> cols = postData["dataColInfos"].ToObject<Dictionary<string, EPF_BRWS_COL>>();

                return Request.CreateResponse(HttpStatusCode.OK, _screenService.GetCodeDesc(sqlDesc, data, cols));
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [GzipCompression]
        [HttpPost]
        [Route("EpfScreen/FetchData")]
        public async Task<HttpResponseMessage> FetchData() {

            //Call microservice to handle request
            var microServiceResponse = await CallMicroServiceAsync("EpfScreen/FetchData", HttpMethod.Post);
            if (microServiceResponse != null) {
                return microServiceResponse;
            }

            //fallback if no response from microservice, handle request by this API Serivce
            string value = await Request.Content.ReadAsStringAsync();
            try {
                JObject postData = JObject.Parse(value);
                string viewName = postData["viewName"].ToString();
                string fieldKeys = postData["fieldKeys"].ToString();
                Dictionary<string, JObject> dataColInfos = postData["dataColInfos"].ToObject<Dictionary<string, JObject>>();
                Dictionary<string, object> param = postData["param"].ToObject<Dictionary<string, object>>();

                return Request.CreateResponse(HttpStatusCode.OK, _screenService.FetchData(viewName, fieldKeys, dataColInfos, param));
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [GzipCompression]
        [HttpPost]
        [Route("EpfScreen/GetNavigateRecordKeys")]
        public async Task<HttpResponseMessage> GetNavigateRecordKeys() {

            //Call microservice to handle request
            var microServiceResponse = await CallMicroServiceAsync("EpfScreen/GetNavigateRecordKeys", HttpMethod.Post);
            if (microServiceResponse != null) {
                return microServiceResponse;
            }

            //fallback if no response from microservice, handle request by this API Serivce
            string value = await Request.Content.ReadAsStringAsync();
            try {
                JObject postData = JObject.Parse(value);
                string baseView = postData["baseView"].ToString();
                string brwsId = postData["brwsId"].ToString();
                JObject data = JObject.FromObject(postData["dataSource"]);
                string fieldKeys = postData["fieldKeys"].ToStr();
                int offsetFromCurrent = postData["offsetFromCurrent"].ToInt();

                return Request.CreateResponse(HttpStatusCode.OK, _screenService.GetNavigateRecordKeys(brwsId, data, baseView, fieldKeys, offsetFromCurrent));
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [GzipCompression]
        [HttpPost]
        [Route("EpfScreen/GetBrwsData")]
        public async Task<HttpResponseMessage> GetBrwsData() {

            //Call microservice to handle request
            var microServiceResponse = await CallMicroServiceAsync("EpfScreen/GetBrwsData", HttpMethod.Post);
            if (microServiceResponse != null) {
                return microServiceResponse;
            }

            //fallback if no response from microservice, handle request by this API Serivce
            string value = await Request.Content.ReadAsStringAsync();
            try {
                JObject postData = JObject.Parse(value);
                string baseView = postData["baseView"].ToString();
                string brwsId = postData["brwsId"].ToString();
                JObject data = JObject.FromObject(postData["dataSource"]);

                string navigateScrnId = "";
                if (postData.ContainsKey("navigateScrnId")) {
                    navigateScrnId = postData["navigateScrnId"].ToStr();
                }

                return Request.CreateResponse(HttpStatusCode.OK, _screenService.GetBrwsData(brwsId, data, baseView, navigateScrnId));
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [GzipCompression]
        [HttpPost]
        [Route("EpfScreen/GetBrwsDataServerPaging")]
        public async Task<HttpResponseMessage> GetBrwsDataServerPaging() {

            //Call microservice to handle request
            var microServiceResponse = await CallMicroServiceAsync("EpfScreen/GetBrwsDataServerPaging", HttpMethod.Post);
            if (microServiceResponse != null) {
                return microServiceResponse;
            }

            //fallback if no response from microservice, handle request by this API Serivce
            string value = await Request.Content.ReadAsStringAsync();
            try {
                JObject postData = JObject.Parse(value);
                string baseView = postData["baseView"].ToString();
                string brwsId = postData["brwsId"].ToString();
                JObject data = JObject.FromObject(postData["dataSource"]);
                JObject pagingInfo = JObject.FromObject(postData["args"]);

                JObject brwsData = _screenService.GetBrwsData(brwsId, data, baseView, pagingInfo);

                JObject retData = new JObject();
                retData.Add("data", brwsData["rows"]);
                retData.Add("totalCount", brwsData["totalCount"].ToDbl());
                retData.Add("summary", "");
                retData.Add("groupCount", 0);


                return Request.CreateResponse(HttpStatusCode.OK, retData);
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [GzipCompression]
        [HttpPost]
        [Route("EpfScreen/GetQueryData")]
        public async Task<HttpResponseMessage> GetQueryData() {

            //Call microservice to handle request
            var microServiceResponse = await CallMicroServiceAsync("EpfScreen/GetQueryData", HttpMethod.Post);
            if (microServiceResponse != null) {
                return microServiceResponse;
            }

            //fallback if no response from microservice, handle request by this API Serivce
            string value = await Request.Content.ReadAsStringAsync();
            try {
                JObject postData = JObject.Parse(value);
                string baseView = postData["baseView"].ToString();
                string brwsSql = postData["brwsSql"].ToString();
                JObject data = JObject.FromObject(postData["dataSource"]);
                //Dictionary<string, JObject> cols = postData["dataColInfos"].ToObject<Dictionary<string, JObject>>();

                return Request.CreateResponse(HttpStatusCode.OK, _screenService.GetQueryData(brwsSql, data, baseView));
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("EpfScreen/Action/{baseProcName}/{cmd}")]
        public async Task<HttpResponseMessage> Action(string baseProcName, string cmd) {

            //Call microservice to handle request
            var microServiceResponse = await CallMicroServiceAsync(string.Format("EpfScreen/Action/{0}/{1}", baseProcName, cmd), HttpMethod.Post);
            if (microServiceResponse != null) {
                return microServiceResponse;
            }

            //fallback if no response from microservice, handle request by this API Serivce
            string value = await Request.Content.ReadAsStringAsync();
            try {
                JObject postData = JObject.Parse(value);
                
                _screenService.Action(baseProcName, cmd, postData);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [GzipCompression]
        [HttpPost]
        [Route("EpfScreen/ExecServerFunction")]
        public async Task<HttpResponseMessage> ExecServerFunction() {

            //Call microservice to handle request
            var microServiceResponse = await CallMicroServiceAsync("EpfScreen/ExecServerFunction", HttpMethod.Post);
            if (microServiceResponse != null) {
                return microServiceResponse;
            }

            //fallback if no response from microservice, handle request by this API Serivce
            string value = await Request.Content.ReadAsStringAsync();
            try {
                 ServerFunction func = JObject.Parse(value).ToObject<ServerFunction>();
                object result = _screenService.ExecFunction(func.AssemblyName, func.ClassName, func.MethodName, func.Parameters);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [GzipCompression]
        [HttpPost]
        [Route("EpfScreen/ExecMultiPickSelection")]
        public async Task<HttpResponseMessage> ExecMultiPickSelection() {
            
            //Call microservice to handle request
            var microServiceResponse = await CallMicroServiceAsync("EpfScreen/ExecMultiPickSelection", HttpMethod.Post);
            if (microServiceResponse != null) {
                return microServiceResponse;
            }

            //fallback if no response from microservice, handle request by this API Serivce
            string value = await Request.Content.ReadAsStringAsync();
            try {
                JObject postData = JObject.Parse(value);
                _screenService.ExecMultiPickSelection(postData);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private async Task<HttpResponseMessage> CallMicroServiceAsync(string route, HttpMethod httpMethod) {
            return await CallMicroServiceAsync(route, httpMethod, string.Empty);
        }

        private async Task<HttpResponseMessage> CallMicroServiceAsync(string route, HttpMethod httpMethod, string scrnId) {

            if (string.IsNullOrEmpty(scrnId)) {
                scrnId = _activeScrnId;
            }

            string microserviceUrl = GetMicroServiceUrl(scrnId);
            
            if (string.IsNullOrEmpty(microserviceUrl)) {
                return null;
            }

            return await RedirectRequestToMicroServiceAsync(microserviceUrl, route, httpMethod);
        }

        private string GetMicroServiceUrl(string scrnId) {
            
            if (scrnId != "") {

                string moduleId = _screenService.GetScreenModuleId(scrnId).ToUpper();
                if (moduleId != "") {
                    foreach (var microservice in ApiSetting.MicroServiceForModules) {
                        string key = microservice.Key.ToUpper();
                        string keyWithoutWildCast = key.Replace("%", "");

                        if (key == "%") {
                            return microservice.Value;
                        }

                        if (key.Contains("%")) {
                            if (key.StartsWith("%")) {
                                if (moduleId.EndsWith(keyWithoutWildCast)) {
                                    return microservice.Value;
                                }
                            }

                            if (key.EndsWith("%")) {
                                if (moduleId.StartsWith(keyWithoutWildCast)) {
                                    return microservice.Value;
                                }
                            }
                        }
                        else {
                            if (moduleId == keyWithoutWildCast) {
                                return microservice.Value;
                            }
                        }

                    }
                }
            }
            return string.Empty;
        }

        private async Task<HttpResponseMessage> RedirectRequestToMicroServiceAsync(string microServiceBaseUrl, string route, HttpMethod httpMethod) {
            
            microServiceBaseUrl = microServiceBaseUrl.EndsWith("/") ? microServiceBaseUrl : microServiceBaseUrl + "/";
            Uri microServiceBaseUri = new Uri(microServiceBaseUrl);
            Uri newUri = new Uri(microServiceBaseUri, route);
            string forwardUri = newUri.ToStr();

            Request.Headers.Remove("Host");
            Request.RequestUri = new Uri(forwardUri);
            Request.Method = httpMethod;
            if (Request.Method == HttpMethod.Get) {
                Request.Content = null;
            }

            using (var client = new HttpClient()) {
                string[] sessionHeader = System.Web.HttpContext.Current.Request.Headers.GetValues("Session");
                string[] authorization = System.Web.HttpContext.Current.Request.Headers.GetValues("Authorization");
                string[] acceptEncoding = System.Web.HttpContext.Current.Request.Headers.GetValues("Accept-Encoding");

                client.DefaultRequestHeaders.Add("Authorization", authorization);
                client.DefaultRequestHeaders.Add("Session", sessionHeader);
                if (acceptEncoding != null) {
                    client.DefaultRequestHeaders.Add("Accept-Encoding", acceptEncoding);
                }

                if (Request.Method == HttpMethod.Get) {
                    var response = await client.GetAsync(forwardUri, HttpCompletionOption.ResponseHeadersRead);
                    response.Content.Headers.Add("RequestHandler", "microservice");
                    return response;
                }
                else if (Request.Method == HttpMethod.Post) {
                    var response = await client.PostAsync(forwardUri, Request.Content);
                    response.Content.Headers.Add("RequestHandler", "microservice");
                    return response;
                }
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Error in redirecting request!");
        }
    }
}
