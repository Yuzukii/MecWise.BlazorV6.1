using System;
using MecWise.Blazor;
using MecWise.Blazor.Common;
using MecWise.Blazor.Components;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Timers;

namespace MecWise.Blazor.Utilities.Client {
    public class EPF_REPORT_STS_BLZ : Screen {

        System.Timers.Timer _timer;
        string _jobId;
        bool _autoClose = false;
        bool _previewEnable;

        #region "Screen Events"

        protected override async Task<object> LoadAsync() {
            SetTitleBarMenuVisible(false);
            
            _jobId = GetQueryStringValue("JOB_ID");
            _autoClose = GetQueryStringValue("AUTO_CLOSE").ToBool();
            _previewEnable = GetQueryStringValue("PREVIEW").ToBool();
            if (!string.IsNullOrEmpty(_jobId)) {
                bool fetched;
                try {
                    fetched = await FetchReportFileAsync();
                }
                catch (Exception ex) {
                    Console.WriteLine("Error while fetching report file. " + ex.Message);
                    fetched = true;
                }
                if (!fetched) {
                    SetTimer();
                }
            }

            return await base.LoadAsync();
        }

        protected override async Task<object> ShowRecAsync() {
            if (ScrnMode == ScreenMode.Add) {
                await InitFieldsAsync();
            }

            return await Task.FromResult<object>(null);
        }

        protected override async Task<object> SavedRecAsync() {
            return await Task.FromResult<object>(null);
        }

        protected override async Task<bool> ValidEntryAsync() {
            if (ScrnMode == ScreenMode.Add) {
            }
            return await Task.FromResult<bool>(true);
        }

        private async Task<object> InitFieldsAsync() {
            
            return await Task.FromResult<object>(null);
        }

        public override void Dispose() {
            base.Dispose();
            _timer.Enabled = false;
            _timer = null;
        }

        #endregion



        #region "Contorl Events"

        public async void btnClose_OnClick() {
            await Session.ExecJSAsync("CloseWindow");
        }

        public async void btnDownload_OnClick() {
            JObject result = JObject.Parse( GetFieldValue<string>("JOB_RESULT"));
            if (result["jobStatus"].ToStr() == "COM") {
                if (result["jobType"].ToStr().In("ACTIVE_REPORT", "ACTIVE_REPORT_DOTNET", "CRYSTAL_REPORT", "EXCEL_REPORT", "RS_REPORT", "XTRA_REPORT")) {
                    await Session.ExecJSAsync("_downloadReport", result["reportFile"].ToString(),
                        result["reportFileName"].ToString(), result["reportFileExtension"].ToString(), _previewEnable.ToInt());
                }
            }
        }

        #endregion



        #region "Helper Functions"

        private void SetTimer() {
            // Create a timer with a five second interval.
            _timer = new System.Timers.Timer(5000);
            // Hook up the Elapsed event for the timer. 
            _timer.Elapsed += OnTimedEvent;
            _timer.Enabled = true;
        }

        private async void OnTimedEvent(Object source, ElapsedEventArgs e) {
            bool fetched;
            try {
                fetched = await FetchReportFileAsync();
            }
            catch (Exception ex) {
                Console.WriteLine("Error while fetching report file. " + ex.Message);
                fetched = true;
            }

            if (fetched) {
                _timer.Enabled = false;
            }
        }


        private async Task<bool> FetchReportFileAsync() {
            JObject result = await CommonFunctions.FetchReportFileAsync(Session, _jobId);
            if (result != null) {

                // save the result on the screen
                if (FieldExists("JOB_RESULT")) {
                    SetFieldValue("JOB_RESULT", result.ToStr());
                }


                // Display Job Status
                if (SCRN_ID == "EPF_REPORT_STS_BLZ") {
                    if (result["jobType"].ToStr().In("ACTIVE_REPORT", "ACTIVE_REPORT_DOTNET", "CRYSTAL_REPORT", "EXCEL_REPORT", "RS_REPORT", "XTRA_REPORT")) {
                        SetFieldValue("lblJobType", "Report Status");
                    }
                    else {
                        SetFieldValue("lblJobType", "Job Status");
                    }

                    if (result["jobType"].ToStr().In("ACTIVE_REPORT", "ACTIVE_REPORT_DOTNET", "CRYSTAL_REPORT", "EXCEL_REPORT", "RS_REPORT", "XTRA_REPORT")) {
                        SetFieldValue("lblMessage5", "Status of the report :");
                    }
                    else {
                        SetFieldValue("lblMessage5", "Status of the job :");
                    }

                    SetFieldValue("lblJobId", _jobId);
                    SetFieldValue("lblJobDesc", result["jobDesc"].ToStr());
                    SetFieldValue("lblJobStatus", result["jobStatusDesc"].ToStr());
                    SetFieldValue("lblSubmittedTime", result["jobSubmitTime"].ToDate().ToString("dd/MM/yyyy hh:mm:ss tt"));
                    SetFieldValue("lblMessage7", "Last Updated: " + result["jobStatusTime"].ToDate().ToString("dd/MM/yyyy hh:mm:ss tt"));
                    SetFieldValue("lblErrorDesc", "");
                }
                else if (SCRN_ID == "EPF_REPORT_STS2_BLZ") {
                    if (result["jobType"].ToStr().In("ACTIVE_REPORT", "ACTIVE_REPORT_DOTNET", "CRYSTAL_REPORT", "EXCEL_REPORT", "RS_REPORT", "XTRA_REPORT")) {
                        SetFieldValue("lblJobType", "Report Status");
                        SetFieldVisible("btnDownload", true);
                    }
                    else {
                        SetFieldValue("lblJobType", "Job Status");
                        SetFieldVisible("btnDownload", false);
                    }

                    if (result["jobType"].ToStr().In("ACTIVE_REPORT", "ACTIVE_REPORT_DOTNET", "CRYSTAL_REPORT", "EXCEL_REPORT", "RS_REPORT", "XTRA_REPORT")) {
                        SetFieldValue("lblMessage5", "Status of the report");
                    }
                    else {
                        SetFieldValue("lblMessage5", "Status of the job");
                    }

                    SetFieldValue("lblJobId", _jobId);
                    SetFieldValue("lblJobDesc", result["jobDesc"].ToStr());
                    SetFieldValue("lblJobStatus", result["jobStatusDesc"].ToStr());
                    SetFieldValue("lblSubmittedTime", result["jobSubmitTime"].ToDate().ToString("dd/MM/yyyy hh:mm:ss tt"));
                    SetFieldValue("lblMessage7", result["jobStatusTime"].ToDate().ToString("dd/MM/yyyy hh:mm:ss tt"));
                    SetFieldValue("lblErrorDesc", "");
                    SetFieldDisable("btnDownload", true);

                    SetFieldCssClass("lblJobStatus", "text-warning");
                }
                    

                //Process Job Status
                if (result["jobStatus"].ToStr() == "IP") {
                    Refresh();
                }
                else if (result["jobStatus"].ToStr() == "COM") {
                    if (SCRN_ID == "EPF_REPORT_STS_BLZ") {
                        if (result["jobType"].ToStr().In("ACTIVE_REPORT", "ACTIVE_REPORT_DOTNET", "CRYSTAL_REPORT", "EXCEL_REPORT", "RS_REPORT", "XTRA_REPORT")) {
                            await Session.ExecJSAsync("_downloadReport", result["reportFile"].ToString(),
                                result["reportFileName"].ToString(), result["reportFileExtension"].ToString(), _previewEnable.ToInt());
                        }

                        Refresh();

                        if (_autoClose) {
                            await Session.ExecJSAsync("CloseWindow");
                        }
                    }
                    else {
                        SetFieldCssClass("lblJobStatus", "text-success");
                        SetFieldDisable("btnDownload", false);
                    }

                    Refresh();

                    return true;
                }
                else if (result["jobStatus"].ToStr().In("ERR", "CAN", "ABT")) {
                    if (SCRN_ID == "EPF_REPORT_STS2_BLZ") {
                        SetFieldCssClass("lblJobStatus", "text-danger");
                        SetFieldCssClass("lblErrorDesc", "text-danger");
                    }

                    SetFieldValue("lblErrorDesc", result["errDesc"].ToStr());
                    if (FieldExists("EpfSubContainer9")) {
                        SetFieldVisible("EpfSubContainer9", true);
                    }
                    
                    Refresh();
                    return true;
                }
            }

            return false;
        }

        #endregion

    }
}
