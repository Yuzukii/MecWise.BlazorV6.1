using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MecWise.Blazor.Entities;
using MecWise.Blazor.Api.DataAccess;
using MecWise.Blazor.Api.Repositories;
using MecWise.Blazor.Common;
using System.IO;
using Newtonsoft.Json.Linq;
using MecWise.XtraReportCommon;
using ePlatform.CommonClasses;
using System.Runtime.InteropServices;
using System.Xml;
using MecWise.ReportBase;
using System.Runtime.CompilerServices;
using ePlatform.Security;
using System.Configuration;
using System.Threading;
using MecWise.ReportWeb;

namespace MecWise.Blazor.Api.Services
{
    public class JobService {
        enum ReportType {
            ePreview = 0,
            ePrinter = 1,
            eExport = 2,
            eFax = 4,
            eEmail = 8
        }

        DBHelper _db;
        EPSession _epSession;
        JobRepository _jobRepository;

        public JobService() {
            _db = new DBHelper(ApiSetting.ConnectionString);
            _jobRepository = new JobRepository(_db);
        }

        public JobService(SessionState session) {
            _db = new DBHelper(session.UserID, session.CompCode, session.LangId, ApiSetting.ConnectionString);
            _jobRepository = new JobRepository(_db);
        }

        public async Task<JObject> GenerateDirectReportAsync(string compCode, string reportId, string reportfileName, string param, string format) {
            _epSession = _db.GetEPSession(compCode);

            ReportLauncher launcher = new ReportLauncher();
            ReportFormats reportFormat = ParseEnum<ReportFormats>(format);

            launcher.Dbms = _epSession.DBMS;
            launcher.DbOwner = _epSession.DbOwner;
            launcher.ConnectString = EPSession.EncodeConnectString(_epSession.ConnectionString);
            launcher.CoyId = _epSession.CoyID;
            launcher.ReportId = reportId;
            launcher.ReportFolderPath = ConfigurationManager.AppSettings["XTRA_REPORT_FOLDER"];
            launcher.ReportFileName = reportfileName;
            launcher.ReportFormat = reportFormat;
            launcher.IsDirectReport = true;
            
            launcher.ReportParams = MecWise.XtraReportCommon.CommonFunctions.BuildReportParameters(param);
            launcher.LaunchReport(true);

            MemoryStream mem = new MemoryStream();
            
            launcher.ExportReport(reportFormat, mem);
            
            string reportFileName = string.Format("{0}_XTRA_REPORT_{1}", compCode, DateTime.Now.ToString("yyMMddHHmm"));
            string reportFileExtension = "";
            if (reportFormat == ReportFormats.Pdf) {
                reportFileExtension = "pdf";
            }
            else if (reportFormat == ReportFormats.Excel) {
                reportFileExtension = "xls";
            }
            else if (reportFormat == ReportFormats.ExcelX) {
                reportFileExtension = "xlsx";
            }
            else if (reportFormat == ReportFormats.Rtf) {
                reportFileExtension = "rtf";
            }

            JObject result = new JObject();
            result.Add("reportFileName", reportFileName);
            result.Add("reportFileExtension", reportFileExtension);
            result.Add("reportFile", Convert.ToBase64String(mem.ToArray()));
            result.Add("reportPrefer", await FetchReportPreferences(reportId));

            return result;

        }

        public async Task<JObject> GenerateBatchReportAsync(SessionState session, string reportId, Dictionary<string, object> parameters, int outputFormat, string selectCriteria) {
            string sJobXML = string.Empty;
            JObject reportRec = await GetReportAsync(reportId);
            JObject result = new JObject();
            result.Add("error", false);
            result.Add("jobId", string.Empty);

            try {
                if (!GenReportJobXMLString(session, reportId, reportRec, ref sJobXML, parameters, (ReportFormats)outputFormat)) {
                    return null;
                }

                XmlRptReader oRead;
                oRead = new XmlRptReader(sJobXML);

                LaunchReport launchReport = new LaunchReport();
                ReportJob reportJob = new ReportJob();

                
                reportJob.Dbms = _db.EPSession.DBMS;
                reportJob.DbOwner = _db.EPSession.DbOwner;
                reportJob.CoyId = _db.EPSession.CoyID;
                reportJob.ConnectString = Encrypt(_db.EPSession.ConnectionString);
                reportJob.JobDesc = oRead.JobDesc;
                reportJob.JobOutputFormat = oRead.OutFileName;
                reportJob.JobOwner = oRead.OwnerID;
                reportJob.JobStatus = "IP";
                reportJob.JobType = oRead.JobType;
                reportJob.LangId = session.LangId.ToString();

                string msReportDsn = string.Empty;
                reportJob.ReportDsn = msReportDsn;
                reportJob.ReportId = oRead.RefID;

                reportJob.JobCriteria = oRead.JobCriteria;

                string msSelectCriteria = selectCriteria;
                reportJob.ReportSelectCriteria = System.Security.SecurityElement.Escape(msSelectCriteria);

                string msJobConnect;
                if (IsDoNetReport(oRead.JobType)) {
                    msJobConnect = _db.EPSession.ConnectionString;
                }
                else {
                    msJobConnect = _db.EPSession.ConnectionString;
                    msJobConnect = msJobConnect.TrimEnd(';') + ";" + "Provider=SQLOLEDB";
                    msJobConnect = msJobConnect.Replace("initial catalog", "Database");
                }
                reportJob.JobConnect = Encrypt(msJobConnect);
                reportJob.JobParams = "<PARAM>" + oRead.JobParameters + "</PARAM>";

                if (IsSecurityProtocolEnabled()) {
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                }

                string jobId = launchReport.AddReportJob(reportJob, null);
                if (!string.IsNullOrEmpty(jobId)) {
                    if (launchReport.Execute(jobId, reportJob)) {
                        result["error"] = false;
                        result["jobId"] = jobId;
                    }
                }
            }
            catch (Exception ex) {
                result["error"] = true;
                result["errorMessage"] = ex.Message;
            }
            
            return result;

        }

        public async Task<JObject> LaunchJob(SessionState session, string jobType, string jobRefId, string jobDesc, Dictionary<string, object> parameters) {
            string sJobXML = string.Empty;
            
            JObject result = new JObject();
            result.Add("error", false);
            result.Add("jobId", string.Empty);

            try {
                if (!GenJobXMLString(session, jobType, jobRefId, jobDesc, parameters, ref sJobXML)) {
                    return null;
                }

                XmlRptReader oRead;
                oRead = new XmlRptReader(sJobXML);

                LaunchReport launchReport = new LaunchReport();
                ReportJob reportJob = new ReportJob();


                reportJob.Dbms = _db.EPSession.DBMS;
                reportJob.DbOwner = _db.EPSession.DbOwner;
                reportJob.CoyId = _db.EPSession.CoyID;
                reportJob.ConnectString = Encrypt(_db.EPSession.ConnectionString);
                reportJob.JobDesc = oRead.JobDesc;
                reportJob.JobOwner = oRead.OwnerID;
                reportJob.JobStatus = "IP";
                reportJob.JobType = oRead.JobType;
                reportJob.LangId = session.LangId.ToString();

                string msReportDsn = string.Empty;
                reportJob.ReportDsn = msReportDsn;
                reportJob.ReportId = oRead.RefID;

                string msJobConnect;
                if (IsDoNetReport(oRead.JobType)) {
                    msJobConnect = _db.EPSession.ConnectionString;
                }
                else {
                    msJobConnect = _db.EPSession.ConnectionString;
                    msJobConnect = msJobConnect.TrimEnd(';') + ";" + "Provider=SQLOLEDB";
                    msJobConnect = msJobConnect.Replace("initial catalog", "Database");
                }
                reportJob.JobConnect = Encrypt(msJobConnect);
                reportJob.JobParams = "<PARAM>" + oRead.JobParameters + "</PARAM>";

                if (IsSecurityProtocolEnabled()) {
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                }

                string jobId = launchReport.AddReportJob(reportJob, null);
                if (!string.IsNullOrEmpty(jobId)) {
                    if (launchReport.Execute(jobId, reportJob)) {
                        result["error"] = false;
                        result["jobId"] = jobId;
                    }
                }
            }
            catch (Exception ex) {
                result["error"] = true;
                result["errorMessage"] = ex.Message;
            }

            return result;

        }

        public Task<JObject> FetchReportFileAsync(string compCode, string jobId) {
           
            JObject result = new JObject();
            result.Add("jobStatus", string.Empty);
            result.Add("jobStatusDesc", string.Empty);
            result.Add("jobSubmitTime", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
            result.Add("jobStatusTime", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
            result.Add("jobDesc", string.Empty);
            result.Add("errDesc", string.Empty);
            result.Add("reportFileName", string.Empty);
            result.Add("reportFileExtension", string.Empty);
            result.Add("reportFile", string.Empty);
            
            string jobStatus;

            try {
                JObject jobStatusRec = _jobRepository.GetReportJobStatus(compCode, jobId);
                if (jobStatusRec != null) {
                    jobStatus = jobStatusRec["JOB_STATUS"].ToString();
                    result["jobStatus"] = jobStatus;
                    result["jobSubmitTime"] = jobStatusRec["JOB_SUBMIT_TIME"].ToDate();
                    result["jobDesc"] = jobStatusRec["JOB_DESC"].ToStr();
                    result["jobType"] = jobStatusRec["JOB_TYPE"].ToStr();
                    if (jobStatus == "COM") {
                        if (jobStatusRec["JOB_TYPE"].ToStr().In("ACTIVE_REPORT", "ACTIVE_REPORT_DOTNET", "CRYSTAL_REPORT", "EXCEL_REPORT", "RS_REPORT", "XTRA_REPORT")) {
                            // fetch report file for reports
                            EpfAzureFileProvider fileProvider = new EpfAzureFileProvider();
                            string url = ConfigurationManager.AppSettings["MRS_BATCH_REPORTS_URL"];
                            string path = BatchReportsFolderPath();
                            string file = jobStatusRec["OUTPUT_FILE_NAME"].ToString();
                            string fileExtension = file.Split('.')[file.Split('.').Length - 1];
                            byte[] byteArray = fileProvider.ReadFile(url, path, file);

                            if (byteArray != null) {
                                result["jobStatusDesc"] = "Completed";
                                result["errDesc"] = string.Empty;
                                result["reportFileName"] = file;
                                result["reportFileExtension"] = fileExtension;
                                result["reportFile"] = Convert.ToBase64String(byteArray);
                            }
                            else {
                                result["jobStatus"] = "ERR";
                                result["jobStatusDesc"] = "Error";
                                result["errDesc"] = "There are no records matching your selection criteria. Please modify and try again.";
                            }
                        }
                        else {
                            result["jobStatusDesc"] = "Completed";
                            result["errDesc"] = string.Empty;
                        }

                    }
                    else {
                        result["errDesc"] = string.Empty;
                        switch (jobStatus) {
                            case "NEW":
                                result["jobStatusDesc"] = "Submitted";
                                break;
                            case "QUD":
                                result["jobStatusDesc"] = "Queued";
                                break;
                            case "CAN":
                                result["jobStatusDesc"] = "Cancelled";
                                break;
                            case "IP":
                                result["jobStatusDesc"] = "In Progress";
                                break;
                            case "ABT":
                                result["jobStatusDesc"] = "Aborted";
                                break;
                            case "ERR":
                                result["jobStatusDesc"] = "Error";
                                result["errDesc"] = jobStatusRec["ERR_DESC"].ToString();
                                break;
                            default:
                                break;
                        }
                    }

                    return Task.FromResult<JObject>(result);
                }
                else {

                    result["jobStatus"] = "ERR";
                    result["jobStatusDesc"] = "Error";
                    result["errDesc"] = "Unable to find job.";
                }
            }
            catch (Exception ex) {
                result["jobStatus"] = "ERR";
                result["jobStatusDesc"] = "Error";
                result["errDesc"] = ex.Message;
            }
            
            return Task.FromResult<JObject>(result);

        }

        public async Task<JArray> FetchReportPreferences(string reportId)
        {
            JArray result = _jobRepository.FetchReportPreferences(reportId);
            return await Task.FromResult<JArray>(result);
        }

        private bool IsDoNetReport(string jobType) {
            switch (jobType) {
                case "ACTIVE_REPORT_DOTNET":
                    return true;
                case "RS_REPORT":
                    return true;
                case "XTRA_REPORT":
                    return true;
            }
            return false;
        }

        private bool IsSecurityProtocolEnabled() {
            string enabled = "false";
            try {
                enabled = Convert.ToString(ConfigurationManager.AppSettings["EPF_SCPROTOCOL_ENABLED"]);
            }
            catch (Exception) {
                enabled = "false";
            }
            return (enabled == "true");
        }

        private bool GenJobXMLString(SessionState session, string jobType, string jobRefId, string jobDec, Dictionary<string, object> parameters, ref string strJobXML) {
            XmlRptWriter oWrite;
            EPEncrypt encrypt = new EPEncrypt();

            // Function which calls the Public Function to generate XML File
            oWrite = new XmlRptWriter();

            //string rptFileName = reportDefn["REPORT_FILE_NAME"].ToString();
            //strFileExt = rptFileName.Split('.')[rptFileName.Split('.').Length - 1];

            // Company Code
            oWrite.AddCompCode(session.CompCode);
            // Job/Report Type
            oWrite.AddJobType(jobType);
            // Report Type
            oWrite.AddRefID(jobRefId);
            // Report Description
            oWrite.AddJobDesc(jobDec);
            // Current Logged in user
            oWrite.AddOwnerID(session.UserID);
            // Priority of the Report
            oWrite.AddJobPriority("0");
            // Schedule Time of the Report
            oWrite.AddSchedule(System.DateTime.Now);
            // Next Job ID to Process after this Report
            oWrite.AddNextJobID(String.Empty);

            // Add the Parameters details
            foreach (var param in parameters) {
                oWrite.AddJobParams(param.Key, "TEXT", param.Value);
            }


            // Add the Application EPSession Settings for Web Service to Use
            oWrite.AddApplnDBSettings("CONNECTION_STRING", _db.EPSession.ConnectionString);
            oWrite.AddApplnDBSettings("DBMS", _db.EPSession.DBMS);
            oWrite.AddApplnDBSettings("DBOWNER", _db.EPSession.DbOwner);
            oWrite.AddApplnDBSettings("COYID", _db.EPSession.CoyID);

            oWrite.AddJobProperty("DB_CONNECT", encrypt.EncryptString(_db.EPSession.ConnectionString));

            // Processing is finished. Time to Return back to the Caller
            strJobXML = oWrite.Xml;
            oWrite = null;

            return true;

        }

        private bool GenReportJobXMLString(SessionState session, string reportId, JObject reportDefn,
            ref string strJobXML, Dictionary<string, object> parameters, ReportFormats outputFormat) {
            XmlRptWriter oWrite;
            
            // Function which calls the Public Function to generate XML File
            oWrite = new XmlRptWriter();

            //string rptFileName = reportDefn["REPORT_FILE_NAME"].ToString();
            //strFileExt = rptFileName.Split('.')[rptFileName.Split('.').Length - 1];

            // Company Code
            oWrite.AddCompCode(session.CompCode);
            // Job/Report Type
            oWrite.AddJobType(reportDefn["REPORT_TYPE"].ToString());
            // Report Type
            oWrite.AddRefID(reportId);
            // Report Description
            oWrite.AddJobDesc(reportDefn["REPORT_TITLE_0"].ToString());
            // Report Extention
            oWrite.AddOutFileName(ReportExtension(outputFormat));
            // Current Logged in user
            oWrite.AddOwnerID(session.UserID);
            // Priority of the Report
            oWrite.AddJobPriority("0");
            // Schedule Time of the Report
            oWrite.AddSchedule(System.DateTime.Now);
            // Next Job ID to Process after this Report
            oWrite.AddNextJobID(String.Empty);
            // Job Criteria in case of Form Printing Reports
            /*
            If msReportParams <> "" Then
                varRepParams = Split(msReportParams, SV_REPORT_PARAMS_SEPERATOR)
                For intLoop = 0 To varRepParams.Length - 1
                    varParam = Split(varRepParams.GetValue(intLoop), "=")
                    oWrite.AddJobCriteria(varParam.GetValue(0), varParam.GetValue(1))
                Next
            End If
            */


            

            // Add the Parameter Values
            AddJobParameterValues(session, reportDefn, ref oWrite, parameters);

            // Add the Property Values
            AddJobPropertyValues(reportDefn, ref oWrite);

            // Add the Application EPSession Settings for Web Service to Use
            oWrite.AddApplnDBSettings("CONNECTION_STRING", _db.EPSession.ConnectionString);
            oWrite.AddApplnDBSettings("DBMS", _db.EPSession.DBMS);
            oWrite.AddApplnDBSettings("DBOWNER", _db.EPSession.DbOwner);
            oWrite.AddApplnDBSettings("COYID", _db.EPSession.CoyID);

            // Processing is finished. Time to Return back to the Caller
            strJobXML = oWrite.Xml;
            oWrite = null;

            return true;

        }

        private void AddJobPropertyValues(JObject reportDefn, ref XmlRptWriter oWrite) {
            string strConnectionString = _db.EPSession.ConnectionString;

            //------------ Reports & BatchReports folder paths ---------------
            oWrite.AddJobProperty("ACTIVE_REPORT_FOLDER", ReportsFolderPath());
            oWrite.AddJobProperty("BATCH_REPORT_FOLDER", BatchReportsFolderPath());
            oWrite.AddJobProperty("REPORT_FILE_NAME", ReportFileName(reportDefn["REPORT_FILE_NAME"].ToStr()));

            //------------ Connection string to App DB ---------------
            oWrite.AddJobProperty("JOB_CONNECT", Encrypt(strConnectionString));

            //------------ Select Criteria ---------------
            string strCriteria = string.Empty;
            oWrite.AddJobProperty("SELECT_CRITERIA", strCriteria);

            //------------ Others ---------------
            oWrite.AddJobProperty("REPORT_ACTION", ReportType.ePreview);
            oWrite.AddJobProperty("NO_OF_COPIES", 1);
            oWrite.AddJobProperty("DEF_PRINTER_NAME", _db.EPSession.DefaultLanguage);
            oWrite.AddJobProperty("CURR_LANG", _db.EPSession.DefaultLanguage);
            oWrite.AddJobProperty("SERVER_SIDE_PRINT", "False");
            oWrite.AddJobProperty("CLIENT_SIDE_PRINT", "False");
        }

        private string ReportExtension(ReportFormats outputFormat) {
            string strFileExt = "pdf";
            switch (outputFormat) {
                case ReportFormats.Pdf:
                    strFileExt = "pdf";
                    break;
                case ReportFormats.Excel:
                    strFileExt = "xls";
                    break;
                case ReportFormats.ExcelX:
                    strFileExt = "xlsx";
                    break;
                case ReportFormats.Csv:
                    strFileExt = "csv";
                    break;
                case ReportFormats.Rtf:
                    strFileExt = "rtf";
                    break;
                case ReportFormats.Mht:
                    strFileExt = "mht";
                    break;
                case ReportFormats.Html:
                    strFileExt = "html";
                    break;
                case ReportFormats.Text:
                    strFileExt = "txt";
                    break;
                case ReportFormats.Image:
                    strFileExt = "ttf";
                    break;
                default:
                    strFileExt = "pdf";
                    break;
            }
            return strFileExt;
        }

        private int ReturnReportType(string strReportType) {
            if (strReportType == ReportType.ePreview.ToString("G")) {
                return 0;
            }
            else if (strReportType == ReportType.ePrinter.ToString("G")) {
                return 1;
            }
            else if (strReportType == ReportType.eExport.ToString("G")) {
                return 2;
            }
            else if(strReportType == ReportType.eFax.ToString("G")) {
                return 4;
            }
            else if (strReportType == ReportType.eEmail.ToString("G")) {
                return 8;
            }
            return 0;
        }

        private string ReportFileName(string fileName) {
            if (string.IsNullOrEmpty(fileName)) {
                return string.Empty;
            }

            string suffix = ReportFileSuffix();
            if (string.IsNullOrEmpty(suffix)) {
                return fileName;
            }

            string[] parts = fileName.Split('.');
            return parts[0] + "-" + suffix + "." + parts[1];
        }

        private string ReportFileSuffix() {
            string key = "REPORT_FILE_SUFFIX_" + _db.EPSession.DefaultLanguage.ToStr();
            return ConfigurationManager.AppSettings[key];
        }
        private string BatchReportsFolderPath() {
            return GetFolderFullPath("BATCH_REPORT_FOLDER");
        }

        private string ReportsFolderPath() {
            return GetFolderFullPath("ACTIVE_REPORT_FOLDER");
        }

        private string GetFolderFullPath(string folder) {
            string path = string.Empty;
            path = ConfigurationManager.AppSettings[folder];
            if (string.IsNullOrEmpty(path)) { 
                return path; 
            }
            if (path.StartsWith("~")) { 
                path = path.Replace("~", ToolkitFolderPath()); 
            }
            return path;
        }

        private string ToolkitFolderPath() {
            return ConfigurationManager.AppSettings["TOOLKIT_FOLDER"];
        }

        private void AddJobParameterValues(SessionState session, JObject reportDefn, ref XmlRptWriter oWrite, Dictionary<string, object> parameters) {
            int intCount = 0;

            // get parameters...
            List<JToken> details = reportDefn["_DETAILS"].ToList().FindAll(x => x["FORMULA_TYPE"].ToStr() == "PARAM");
            foreach (var detailRec in details) {
                if (detailRec["PARAM_VISIBLE"].ToStr() == "N") {
                    if (detailRec["PARAM_NAME"].ToStr().ToLower() == "currempeid") {
                        oWrite.AddJobParams("txtParam" + intCount, "TEXT", session.EmpeID.ToUpper());
                    }
                    else {
                        // Added else block to cater to hidden parameters with default key/values specified.
                        string defaultKey = detailRec["PARAM_DEF_VALUE_0"].ToStr();
                        if (!string.IsNullOrEmpty(defaultKey)) {
                            if (detailRec["PARAM_TYPE"].ToStr() == "D") {
                                oWrite.AddJobParams("txtParam" + intCount, "DATE", GetDefaultValue(session, defaultKey));
                            }
                            else {
                                // for hidden parameters, added checking for P type parameters
                                if (IsExportAttrControl(reportDefn, detailRec["PARAM_NAME"].ToStr())) {
                                    oWrite.AddJobParams("txtParam" + intCount, "TEXT", Encrypt(GetDefaultValue(session, defaultKey).ToStr()));
                                }
                                else {
                                    oWrite.AddJobParams("txtParam" + intCount, "TEXT", GetDefaultValue(session, defaultKey));
                                }
                            }
                        }
                    }
                }
                else if (detailRec["PARAM_VISIBLE"].ToStr() == "R") {
                    object val = detailRec["PARAM_DEF_VALUE_0"].ToString();
                    if (!string.IsNullOrEmpty(val.ToStr())) {
                        if (val.ToStr().StartsWith("SQL:")) {
                            val = _db.GetAValue(val.ToStr().Substring(4));
                        }
                        else if (val.ToStr().StartsWith(":")) {
                            val = GetDefaultValue(session, val.ToStr()).ToString();
                        }

                        if (detailRec["PARAM_TYPE"].ToStr() == "D") {
                            val = val.ToDate().ToString("dd/MM/yyyy");
                        }
                    }

                    if (detailRec["PARAM_TYPE"].ToStr() == "D") {
                        oWrite.AddJobParams("txtParam" + intCount, "DATE", val);
                    }
                    else {
                        // for hidden parameters, added checking for P type parameters
                        if (IsExportAttrControl(reportDefn, detailRec["PARAM_NAME"].ToStr())) {
                            oWrite.AddJobParams("txtParam" + intCount, "TEXT", Encrypt(GetDefaultValue(session, val.ToStr()).ToStr()));
                        }
                        else {
                            if (val.ToStr().Contains("&")) {
                                val = val.ToStr().Replace("&", "&amp;");
                            }
                            oWrite.AddJobParams("txtParam" + intCount, "TEXT", val);
                        }
                    }
                }
                else {
                    // Purposely didnt add _D or _C ahead of the param name as ARWebReport does not handle it
                    // Get the correct field on screen for parameters are hidden parameter values also added. 
                    if (detailRec["PARAM_TYPE"].ToStr() == "D") {
                        oWrite.AddJobParams("txtParam" + intCount, "DATE", parameters[detailRec["PARAM_NAME"].ToStr()].ToDate().ToString("dd/MM/yyyy"));
                    }
                    else {
                        // for hidden parameters, added checking for P type parameters
                        if (IsExportAttrControl(reportDefn, detailRec["PARAM_NAME"].ToStr())) {
                            string val = parameters[detailRec["PARAM_NAME"].ToStr()].ToStr();
                            oWrite.AddJobParams("txtParam" + intCount, "TEXT", Encrypt(GetDefaultValue(session, val).ToStr()));
                        }
                        else {
                            string val = parameters[detailRec["PARAM_NAME"].ToStr()].ToStr();
                            if (val.Contains("&")) {
                                val = val.Replace("&", "&amp;");
                            }
                            oWrite.AddJobParams("txtParam" + intCount, "TEXT", val);
                        }
                    }
                }

                intCount += 1;
            }
        }

        private string Encrypt(string clearText) {
            EPEncrypt enc;
            try {
                enc = new EPEncrypt();
                return enc.EncryptString(clearText);
            }
            catch {
                return clearText;
            }
            finally {
                enc = null;
            }
        }

        private bool IsExportAttrControl(JObject reportDefn, string paramName) {

            if (reportDefn["REPORT_TYPE"].ToStr() != "XTRA_REPORT") { return false;  }

            if (string.IsNullOrEmpty(paramName)) { return false; }

            if (paramName.ToLower() == "exportattr" || paramName.ToLower() == "exportattrconfirm") { return true; }

            return false;
        }

        private object GetDefaultValue(SessionState session, string key) {
            string defaultValue = key;
            switch (key) {
                case ":COMP_CODE":
                    return session.CompCode;
                case ":EMPE_ID":
                    return session.EmpeID;
                case ":DATE":
                    return DateTime.Today;
                case ":TIME":
                    return (DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString());
                case ":NOW":
                    return DateTime.Now;
                case ":USER_ID":
                    return session.UserID;
                default:
                    if (key.StartsWith(":")) {
                        defaultValue = key.Substring(1);
                    }
                    break;
            }
            return defaultValue;
        }

        public Task<JObject> GetReportAsync(string reportId) {
            return Task.FromResult<JObject>(_jobRepository.GetReport(reportId));
        }

        private static T ParseEnum<T>(string value) {
            return (T)Enum.Parse(typeof(T), value, true);
        }

    }
}
