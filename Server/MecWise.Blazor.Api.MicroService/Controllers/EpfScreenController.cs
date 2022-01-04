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
using MecWise.Blazor.Api.MicroService.Filters;

namespace MecWise.Blazor.Api.MicroService.Controllers {
    
    [MicroServiceAuthorize]
    public class EpfScreenController : ApiController
    {
        EpfScreenService _screenService;

        public EpfScreenController() {
            string[] sessionHeader = System.Web.HttpContext.Current.Request.Headers.GetValues("Session");
            if (sessionHeader != null) {
                string jsonStr = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(sessionHeader[0]));
                SessionState session = JObject.Parse(jsonStr).ToObject<SessionState>();
                _screenService = new EpfScreenService(session);
            }
            else {
                _screenService = new EpfScreenService();
            }
        }

        [GzipCompression]
        [HttpGet]
        [Route("EpfScreen/GetScreen/{scrnId}")]
        public HttpResponseMessage GetScreen(string scrnId)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, JArray.FromObject(_screenService.GetScreen(scrnId)));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [GzipCompression]
        [HttpGet]
        [Route("EpfScreen/GetRights/{scrnId}")]
        public HttpResponseMessage GetRights(string scrnId) {
            try {
                return Request.CreateResponse(HttpStatusCode.OK, _screenService.GetRights(scrnId));
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
        public HttpResponseMessage GetColumnInfos(string viewName) {
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

        
    }
}
