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
using MecWise.Blazor.Api.Filters;

namespace MecWise.Blazor.Api.Controllers
{
    [Authorize]
    public class JobController : ApiController
    {

        JobService _jobService;

        public JobController() {
            string[] sessionHeader = System.Web.HttpContext.Current.Request.Headers.GetValues("Session");
            if (sessionHeader != null) {
                string jsonStr = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(sessionHeader[0]));
                SessionState session = JObject.Parse(jsonStr).ToObject<SessionState>();
                _jobService = new JobService(session);
            }
            else {
                _jobService = new JobService();
            }
        }

        [HttpPost]
        [Route("Job/GenerateDirectReport")]
        public async Task<HttpResponseMessage> GenerateDirectReport() {
            string value = await Request.Content.ReadAsStringAsync();

            try {
                
                JObject postData = JObject.Parse(value);
                string compCode = postData["COMP_CODE"].ToString();
                string reportId = postData["REPORT_ID"].ToString();
                string param = postData["PARAM"].ToString();
                string format = postData["REPORT_TYPE"].ToString();
                string reportFileName = postData["REPORT_FILE_NAME"].ToString();
                JObject result = await _jobService.GenerateDirectReportAsync(compCode, reportId, reportFileName, param, format);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        

        [HttpPost]
        [Route("Job/GenerateBatchReport")]
        public async Task<HttpResponseMessage> GenerateBatchReport() {
            string value = await Request.Content.ReadAsStringAsync();

            try {

                JObject postData = JObject.Parse(value);
                string reportId = postData["REPORT_ID"].ToStr();
                SessionState session = JObject.Parse(postData["SESSION"].ToStr()).ToObject<SessionState>();
                Dictionary<string, object> parameters = JObject.Parse(postData["PARAM"].ToStr()).ToObject<Dictionary<string, object>>();
                int format = postData["FORMAT"].ToInt();
                string selectCriteria = postData["SELECT_CRITERIA"].ToStr();
                JObject result = await _jobService.GenerateBatchReportAsync(session, reportId, parameters, format, selectCriteria);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [GzipCompression]
        [HttpGet]
        [Route("Job/FetchReportPreferences/{reportId}")]
        public async Task<HttpResponseMessage> FetchReportPreferences(string reportId)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, await _jobService.FetchReportPreferences(reportId));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [GzipCompression]
        [HttpGet]
        [Route("Job/FetchReportFile/{compCode}/{jobId}")]
        public async Task<HttpResponseMessage> FetchReportFile(string compCode, string jobId) {
            
            try {
                return Request.CreateResponse(HttpStatusCode.OK, await _jobService.FetchReportFileAsync(compCode, jobId));
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("Job/LaunchJob")]
        public async Task<HttpResponseMessage> LaunchJob() {
            string value = await Request.Content.ReadAsStringAsync();

            try {

                JObject postData = JObject.Parse(value);
                
                SessionState session = JObject.Parse(postData["SESSION"].ToStr()).ToObject<SessionState>();
                string jobType = postData["JOB_TYPE"].ToStr();
                string jobRefId = postData["JOB_REF_ID"].ToStr();
                string jobDesc = postData["JOB_DESC"].ToStr();
                Dictionary<string, object> parameters = JObject.Parse(postData["PARAM"].ToStr()).ToObject<Dictionary<string, object>>();
                
                JObject result = await _jobService.LaunchJob(session, jobType, jobRefId, jobDesc, parameters);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [GzipCompression]
        [HttpGet]
        [Route("Job/GetReport/{reportId}")]
        public async Task<HttpResponseMessage> GetMessengerId(string reportId) {
            try {
                return Request.CreateResponse(HttpStatusCode.OK, await _jobService.GetReportAsync(reportId));
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

    }
}
