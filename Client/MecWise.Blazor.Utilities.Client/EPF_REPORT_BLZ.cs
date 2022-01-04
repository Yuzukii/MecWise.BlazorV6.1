using System;
using MecWise.Blazor;
using MecWise.Blazor.Common;
using MecWise.Blazor.Components;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using MecWise.Blazor.Entities;

namespace MecWise.Blazor.Utilities.Client {
    public class EPF_REPORT_BLZ : Screen {

        JObject _report;
        JArray _prefer;

        #region "Screen Events"

        protected override async Task<object> LoadAsync() {
            
            SetTitleBarMenuVisible(FieldTitleBarMenuItemType.New, false);

            await InitReportDataAsync();

            RenderParameterFields();
            GenerateReportQueryFields();

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

        #endregion



        #region "Contorl Events"

        public async void btnSelectCriteria_OnClick() {
            await OpenQueryBuilderAsync("QueryFieldsData", "QueryResult");
        }

        public async void btnGenerate_OnClick() {

            List<JToken> mandatoryParams = _report["_DETAILS"].ToList().FindAll(x => x["PARAM_CHECK"].ToString() == "Yes");
            if (mandatoryParams != null) {
                foreach (var param in mandatoryParams) {
                    if (string.IsNullOrEmpty(GetFieldValue<string>(param["PARAM_NAME"].ToString()))) {
                        Session.ToastMessage(param["PARAM_PROMPT_0"].ToString() + " cannot be empty!", ToastMessageType.warning);
                        return;
                    }
                }
            }

            string reportId = _report["REPORT_ID"].ToString();
            Dictionary<string, object> reportParams = GetParameters();
            string selectCriteria = GetSelectCriteria();
            ReportFormats format = ParseEnum<ReportFormats>(GetFieldValue<string>("REPORT_TYPE"));
            JObject result = await CommonFunctions.GenerateBatchReportAsync(Session, reportId, format, reportParams, selectCriteria);
            string jobId = result["jobId"].ToStr();

            bool previewEnable = false;
            JToken prefer = _prefer.ToList<JToken>().Find(x => x["PREFER_CODE"].ToStr() == "RPT-OUTPUT-PDF-VIEWER-ENABLED");
            if (prefer != null) {
                previewEnable = prefer["PREFER_VAL"].ToBool();
            }

            if (!result["error"].ToBool() && !string.IsNullOrEmpty(jobId)) {
                string stautsPageId = "EPF_REPORT_STS_BLZ";
                prefer = _prefer.ToList<JToken>().Find(x => x["PREFER_CODE"].ToStr() == "BLZ-RPT-STS-PAGE-ID");
                if (prefer != null) {
                    stautsPageId = prefer["PREFER_VAL"].ToStr();
                }

                // call job status page;
                string url = BaseUri + "EpfScreen/"+ stautsPageId + "?JOB_ID=" + jobId + "&PREVIEW=" + previewEnable.ToInt();
                //NaviManager.NavigateTo(url);
                await Session.ExecJSAsync("NewWindow", url, false, false);
            }
            else {
                Console.WriteLine("Error while generating report: " + _report["REPORT_ID"].ToString());
                Console.WriteLine("Error message: " + result["errorMessage"].ToStr());
                Session.ToastMessage("Error while generating report. Please contact administrator.", ToastMessageType.error);
            }


        }


        #endregion




        #region "Helper Functions"

        private string GetSelectCriteria() {
            string selectCriteria = string.Empty;
            string queryResultStr = GetFieldValue<string>("QueryResult");
            if (!string.IsNullOrEmpty(queryResultStr)) {
                JObject queryResult = JObject.Parse(queryResultStr);
                JArray ExpressionData = JArray.FromObject(queryResult["ExpressionData"]);
                foreach (var item in ExpressionData) {
                    selectCriteria += (selectCriteria == "" ? "" : " ") + item["TEXT"].ToStr();
                }
            }

            return selectCriteria;
        }

        private static T ParseEnum<T>(string value) {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        private Dictionary<string, object> GetParameters() {
            Dictionary<string, object> result = new Dictionary<string, object>();

            foreach (var detail in _report["_DETAILS"]) {

                string value;
                if (detail["PARAM_TYPE"].ToString() == "D") {
                    value = GetFieldValue<DateTime>(detail["PARAM_NAME"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                }
                else {
                    value = GetFieldValue<string>(detail["PARAM_NAME"].ToString());
                }

                result.Add(detail["PARAM_NAME"].ToString(), value);

            }

            return result;
        }

        private void GenerateReportQueryFields() {
            JArray QueryFieldsData = new JArray();


            foreach (var item in _report["_FIELDS"]) {
                QueryField field = new QueryField() {
                    FieldName = item["PARAM_NAME"].ToStr(),
                    FieldDescription = item["PARAM_PROMPT_0"].ToStr(),
                    FieldType = item["PARAM_TYPE"].ToStr()
                };
                QueryFieldsData.Add(JObject.FromObject(field));
            }

            SetFieldValue("QueryFieldsData", QueryFieldsData.ToStr());
        }

        private void RenderParameterFields() {
            if (_report != null) {
                SetTitleBarText(_report["REPORT_TITLE_0"].ToString());
                FieldButton btnGenerate = (FieldButton)Fields.Search("btnGenerate");
                btnGenerate.IconCssClass = "oi oi-cog mr-2";

                FieldContainer parameterContainer = (FieldContainer)Fields.Search("D");

                foreach (var detail in _report["_DETAILS"]) {
                    FieldContainer paramFieldContainer = new FieldContainer("row");
                    Field paramField;

                    // populate realted field base on report parameter setting
                    if (detail["PARAM_TYPE"].ToString() == "N") {
                        // NumberBox
                        paramField = new FieldNumber(detail["PARAM_NAME"].ToString(), detail["PARAM_PROMPT_0"].ToString(), "col-md-6 col-sm-6 col-12");
                    }
                    else if (detail["PARAM_TYPE"].ToString() == "D") {
                        // NumberBox
                        paramField = new FieldDate(detail["PARAM_NAME"].ToString(), detail["PARAM_PROMPT_0"].ToString(), "col-md-6 col-sm-6 col-12");
                    }
                    else if (!string.IsNullOrEmpty(detail["PARAM_SQL_STMT"].ToString())) {
                        // PickList
                        FieldPickList pickListField = new FieldPickList() {
                            ID = detail["PARAM_NAME"].ToString(),
                            Description = detail["PARAM_PROMPT_0"].ToString(),
                            StyleClassWidth = "col-md-6 col-sm-6 col-12",
                            BrwsId = "",
                            BrwsKeys = "",
                            PickOnly = false,
                            BrwsTarget = detail["PARAM_NAME"].ToString(),
                            BrwsAssign = detail["PARAM_NAME"].ToString()
                        };

                        pickListField.OnPickListSelectionChanged += picklist_OnPickListSelectionChanged;
                        pickListField.OnPickListSelection += picklist_OnPickListSelection;

                        paramField = pickListField;
                    }
                    else {
                        // Default TextBox
                        paramField = new FieldText(detail["PARAM_NAME"].ToString(), detail["PARAM_PROMPT_0"].ToString(), "col-md-6 col-sm-6 col-12");
                    }
                    paramField.ParentScreen = this;

                    // assign default value
                    object defValue = detail["PARAM_DEF_VALUE_0"].ToStr();
                    switch (defValue) {
                        case ":COMP_CODE":
                            defValue = Session.CompCode;
                            break;
                        case ":EMPE_ID":
                            defValue = Session.EmpeID;
                            break;
                        case ":USER_ID":
                            defValue = Session.UserID;
                            break;
                        case ":LANG_ID":
                            defValue = Session.LangId;
                            break;
                        case ":DATE":
                            defValue = DateTime.Now;
                            break;
                        default:
                            break;
                    }
                    SetFieldValue(detail["PARAM_NAME"].ToString(), defValue);


                    // visible
                    if (detail["PARAM_VISIBLE"].ToString() == "N") {
                        paramField.Visible = false;
                    }

                    paramFieldContainer.Fields.Add(paramField);
                    parameterContainer.Fields.Add(paramFieldContainer);
                }

                

                Refresh();

            }
        }

        private async void picklist_OnPickListSelection(object sender, FieldClickEventArgs e) {
            FieldPickList picklist = (FieldPickList)sender;

            JToken param = _report["_DETAILS"].ToList().Find(x => x["PARAM_NAME"].ToString() == picklist.ID);
            if (param != null) {

                string url = "EpfScreen/GetQueryData";
                JObject postData = new JObject();
                postData.Add("baseView", "");
                postData.Add("brwsSql", param["PARAM_SQL_STMT"]);
                postData.Add("session", JObject.FromObject(Session));
                postData.Add("dataSource", this.DataSource);
                postData.Add("dataColInfos", JObject.FromObject(this.DataColumnInfos));

                JObject QueryData = await Session.PostJsonAsync<JObject>(url, postData);

                Dictionary<string, EPF_BRWS_COL> colInfos = QueryData["cols"].ToObject<Dictionary<string, EPF_BRWS_COL>>();
                picklist.BrwsGrid = new FieldGridView();
                picklist.BrwsGrid.ParentScreen = this;
                picklist.BrwsGrid.BRWS_ID = picklist.BrwsId;
                picklist.BrwsGrid.ShowTitleBar = false;
                if (QueryData.ContainsKey("rows")) {
                    picklist.BrwsGrid.DataSource = JArray.FromObject(QueryData["rows"]);
                }

                string colWidths = "";
                if (string.IsNullOrEmpty(param["SQL_COL_WIDTH_0"].ToString())) {
                    foreach (var colInfo in colInfos) {
                        if (!string.IsNullOrEmpty(colWidths)) {
                            colWidths += ",";
                        }
                        colWidths += "1000";
                    }
                }
                else {
                    colWidths = param["SQL_COL_WIDTH_0"].ToString();
                }
                picklist.BrwsGrid.InitColumns(colInfos, colWidths, param["SQL_COL_0"].ToString(), "", "");

                Refresh();
            }

        }

        private void picklist_OnPickListSelectionChanged(object sender, FieldChangeEventArgs e) {

            FieldPickList picklist = (FieldPickList)sender;
            JObject selectedRow = e.DataSource;
            if (selectedRow != null) {
                string firstKey = selectedRow.Properties().Select(p => p.Name).FirstOrDefault();
                SetFieldValue(picklist.ID, selectedRow[firstKey].ToString());
            }

            Refresh();
        }


        private void picklistAssignFields(JObject selectedRow, string brwsTarget, string brwsAssign) {

            string[] brwsTargets = brwsTarget.Replace(" ", "").Split(',');
            string[] brwsAssigns = brwsAssign.Replace(" ", "").Split(',');

            for (int i = 0; i <= brwsTargets.Length - 1; i++) {
                if (this.DataSource.ContainsKey(brwsTargets[i]) && selectedRow.ContainsKey(brwsAssigns[i])) {
                    this.DataSource[brwsTargets[i]] = selectedRow[brwsAssigns[i]];
                }
            }

        }

        private async Task InitReportDataAsync() {
            string reportId = GetQueryStringValue("REPORT_ID");
            string url = string.Format("Job/GetReport/{0}", reportId);
            _report = await Session.GetJsonAsync<JObject>(url);
            _prefer = await CommonFunctions.FetchReportPreferences(Session, reportId);
        }

        #endregion

    }
}
