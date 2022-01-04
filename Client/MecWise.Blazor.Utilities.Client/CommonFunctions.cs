using MecWise.Blazor.Common;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace MecWise.Blazor.Utilities.Client {
    public class CommonFunctions {

        public static async Task GenerateDirectReportAsync(SessionState session, string reportId, string reportFileName, ReportFormats format, Dictionary<string, object> reportParams) {
            string paramStr = GetReportParamsString(reportParams);

            JObject postData = new JObject();
            postData.Add("COMP_CODE", session.CompCode);
            postData.Add("REPORT_ID", reportId);
            postData.Add("REPORT_FILE_NAME", reportFileName);
            postData.Add("REPORT_TYPE", format.ToString("F"));
            postData.Add("PARAM", paramStr);
            

            string url = "Job/GenerateDirectReport";
            JObject result = await session.PostJsonAsync<JObject>(url, postData);

            JArray reportPrefer = JArray.FromObject(result["reportPrefer"]);

            //[23/Feb/2021, PhyoZin] Added to check report preview setting
            JToken preferSetting = reportPrefer.ToList<JToken>().Find(x => x["PREFER_CODE"].ToStr() == "RPT-OUTPUT-PDF-VIEWER-ENABLED");
            int previewEnable = 0;
            if (preferSetting != null)
            {
                previewEnable = preferSetting["PREFER_VAL"].ToBool().ToInt();
            }

            await session.ExecJSAsync("_downloadReport",
                result["reportFile"].ToString(), result["reportFileName"].ToString(), result["reportFileExtension"].ToString(), previewEnable);
        }

        public static async Task<JObject> GenerateBatchReportAsync(SessionState session, string reportId, ReportFormats format, Dictionary<string, object> reportParams,
            string selectCriteria = "") {
            JObject postData = new JObject();
            postData.Add("REPORT_ID", reportId);
            postData.Add("SESSION", JObject.FromObject(session).ToString());
            postData.Add("PARAM", JObject.FromObject(reportParams).ToString());
            postData.Add("FORMAT", format.ToInt());
            postData.Add("SELECT_CRITERIA", selectCriteria);

            string url = "Job/GenerateBatchReport";
            JObject result = await session.PostJsonAsync<JObject>(url, postData);
            return result;
        }

        public static async Task<JObject> FetchReportFileAsync(SessionState session, string jobId) {
            JObject result;
            string url = string.Format("Job/FetchReportFile/{0}/{1}", session.CompCode, jobId);
            try {
                result = await session.GetJsonAsync<JObject>(url);
            }
            catch (Exception ex) {
                result = new JObject();
                result["jobStatus"] = "ERR";
                result["jobStatusDesc"] = "Error";
                result["errDesc"] = ex.Message;
            }
            
            return result;
        }

        public static async Task<JArray> FetchReportPreferences(SessionState session, string reportId)
        {
            JArray result;
            string url = string.Format("Job/FetchReportPreferences/{0}", reportId);
            try
            {
                result = await session.GetJsonAsync<JArray>(url);
            }
            catch
            {
                result = new JArray();
            }

            return result;
        }

        private static string GetReportParamsString(Dictionary<string, object> reportParams) {
            string paramStr = "";
            foreach (var param in reportParams) {
                if (!string.IsNullOrEmpty(paramStr)) {
                    paramStr += ";";
                }
                if (param.Value.IsDate()) {
                    paramStr += string.Format("{0}={1}", param.Key, Convert.ToDateTime(param.Value).ToString("dd/MM/yyyy HH:mm:ss"));
                }
                else {
                    paramStr += string.Format("{0}={1}", param.Key, param.Value);
                }
            }

            return paramStr;
        }

        public static async Task<JObject> GetReportPreferences(SessionState session)
        {
            JObject result;
            string url = string.Format("Job/GetReportPreferences");
            try
            {
                result = await session.GetJsonAsync<JObject>(url);
            }
            catch (Exception ex)
            {
                result = new JObject();
                result["jobStatus"] = "ERR";
                result["jobStatusDesc"] = "Error";
                result["errDesc"] = ex.Message;
            }

            return result;
        }

        public static async Task<JObject> LaunchJobAsync(SessionState session, string jobType, string jobRefId, string jobDesc, Dictionary<string, object> jobParams) {
            JObject postData = new JObject();
            postData.Add("SESSION", JObject.FromObject(session).ToString());
            postData.Add("JOB_TYPE", jobType);
            postData.Add("JOB_REF_ID", jobRefId);
            postData.Add("JOB_DESC", jobDesc);
            postData.Add("PARAM", JObject.FromObject(jobParams).ToString());

            string url = "Job/LaunchJob";
            JObject result = await session.PostJsonAsync<JObject>(url, postData);
            return result;
        }

        public static async Task<bool> DownloadFileAsync(SessionState session, string fileName) {
           
            JObject postData = new JObject();
            postData.Add("UTILITY_DOWNLOAD", true);
            postData.Add("FILE_NAME", fileName);
            postData.Add("FILE_TYPE", "");

            string apiUrl = session.GetAbsoluteApiUrl("File/Download/");
            return await session.ExecJSAsync<bool>("_downloadFile", apiUrl, session.AccessToken.token_type, session.AccessToken.access_token, postData.ToStr());
        }

    }
}
