using MecWise.Blazor.Common;
using MecWise.Blazor.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MecWise.Blazor.Workflow.Client
{
    public class WF_APRV_OS_BLZ : Screen
    {
        string _compCode;
        string _docType;
        string _deptCode;
        string _runNo;
        string _appId;
        string _wfId;
        string sAppLink;
        WorkflowClient _wfClient;
        JObject _wfObject;


        #region "Screen Events"

        protected override async Task<object> LoadAsync()
        {
            string[] appLink = GetFieldValue<string>("APP_LINK").Split(':');

            _compCode = appLink[0];
            _docType = appLink[1];
            _deptCode = appLink[2];
            _runNo = appLink[3];
            _appId = GetFieldValue<string>("APP_ID");
            _wfId = GetFieldValue<string>("WF_ID");
            sAppLink = GetFieldValue<string>("APP_LINK");
            _wfClient = new WorkflowClient(Session, _appId, _docType, _deptCode);

            var result = await Session.ExecServerFuncAsync("MecWise.Blazor.Api.WorkflowServer", "WF_APRV_OS_BLZ", "GetWorkflowObject",
                _appId, _wfId, Session.CompCode, Session.UserID, Session.EmpeID, Session.LangId);
            _wfObject = JObject.FromObject(result);
            Console.WriteLine(_wfObject.ToString());

            SetTitleBarMenuVisible(false);
            
                return await base.LoadAsync();
        }

        protected override async Task<object> ShowRecAsync()
        {
            string sFormType = GetQueryStringValue("WF_FORM_TYPE");


            if (RecordFetched)
            {


                SetFieldValue("WF_PREV_COMMENTS", _wfObject["msPrevComments"].ToStr());
                SetFieldValue("WF_LBL_ACTION", _wfObject["msActionDesc"].ToStr());

                if (_wfObject["msApprButtonDesc"].ToStr() != "NONE")
                {
                    SetFieldDescription("WF_BTN_APPROVE", _wfObject["msApprButtonDesc"].ToStr());
                }
                if (_wfObject["msDenyButtonDesc"].ToStr() != "NONE")
                {
                    SetFieldDescription("WF_BTN_DISAPPROVE", _wfObject["msDenyButtonDesc"].ToStr());
                }


                Console.WriteLine("WF_PREV_COMMENTS.Visible");
                Console.WriteLine(!Fields.Search("WF_PREV_COMMENTS").Visible);

                if (sFormType == "H")
                {
                    SetFieldVisible("WF_PREV_COMMENTS", false);
                    SetFieldVisible("WF_COMMENTS", false);
                    SetFieldVisible("WF_BTN_APPROVE", false);
                    SetFieldVisible("WF_BTN_DISAPPROVE", false);
                    SetFieldVisible("WF_BTN_CLARIFY", false);
                    SetFieldVisible("WF_BTN_CLARIFY_ALL", false);
                    SetFieldVisible("CLARIFY_TO", false);
                    SetFieldVisible("WF_BTN_CLARIFY_SELECT", false);
                }
                else
                {
                    Console.WriteLine("_wfObject[msRights]");
                    Console.WriteLine(_wfObject["msRights"].ToStr());

                    if (_wfObject["msRights"].ToStr().In("NONE", "VIEWER", "OWNER"))
                    {


                        SetFieldVisible("WF_PREV_COMMENTS", false);
                        SetFieldVisible("WF_COMMENTS", false);
                        SetFieldVisible("WF_BTN_APPROVE", false);
                        SetFieldVisible("WF_BTN_DISAPPROVE", false);
                        SetFieldVisible("WF_BTN_CLARIFY", false);
                        SetFieldVisible("WF_BTN_CLARIFY_ALL", false);
                        SetFieldVisible("CLARIFY_TO", false);
                        SetFieldVisible("WF_BTN_CLARIFY_SELECT", false);
                    }
                    else if (_wfObject["msRights"].ToStr() == "4INFO")
                    {
                        SetFieldVisible("WF_BTN_APPROVE", true);
                        SetFieldVisible("WF_BTN_DISAPPROVE", false);
                        SetFieldVisible("WF_COMMENTS", false);
                        SetFieldVisible("WF_BTN_CLARIFY", false);
                        SetFieldVisible("WF_BTN_CLARIFY_ALL", false);
                        SetFieldVisible("CLARIFY_TO", false);
                        SetFieldVisible("WF_BTN_CLARIFY_SELECT", false);
                    }
                    else
                    {
                        SetFieldVisible("WF_PREV_COMMENTS", true);
                        SetFieldVisible("WF_COMMENTS", true);
                        SetFieldVisible("WF_BTN_APPROVE", true);
                        SetFieldVisible("WF_BTN_DISAPPROVE", true);
                        SetFieldVisible("WF_PREV_COMMENTS", true);

                        SetFieldVisible("WF_BTN_CLARIFY", _wfObject["isClarifyActionEnabled"].ToBool());
                        SetFieldVisible("WF_BTN_CLARIFY_ALL", _wfObject["isClarifyAllActionEnabled"].ToBool());
                        SetFieldVisible("CLARIFY_TO", _wfObject["isClarifyToSelectionEnabled"].ToBool());
                        SetFieldVisible("WF_BTN_CLARIFY_SELECT", _wfObject["isClarifySelectActionEnabled"].ToBool());
                    }
                }

                SetFieldVisible("RECIPS_LIST", _wfObject["isClarifySelectActionEnabled"].ToBool());
                SetFieldVisible("WF_BTN_UP", _wfObject["isClarifySelectActionEnabled"].ToBool());
                SetFieldVisible("WF_BTN_DOWN", _wfObject["isClarifySelectActionEnabled"].ToBool());
                SetFieldVisible("WF_BTN_SELECT", _wfObject["isClarifySelectActionEnabled"].ToBool());


                SetFieldDisable("WF_CC_NOTE", !Fields.Search("WF_BTN_DISAPPROVE").Visible);
                SetFieldDisable("WF_BTN_CC", !Fields.Search("WF_BTN_DISAPPROVE").Visible);

                if (!Fields.Search("WF_PREV_COMMENTS").Visible)
                {
                    SetFieldVisible("WF_PREV_COMMENTS", true);
                    SetFieldDisable("WF_PREV_COMMENTS", true);
                    SetFieldDisable("WF_CC_NOTE", true);
                    SetFieldDisable("WF_BTN_CC", true);
                }

                if (!Fields.Search("WF_COMMENTS").Visible)
                {
                    SetFieldVisible("WF_COMMENTS", true);
                    SetFieldDisable("WF_COMMENTS", true);
                    SetFieldDisable("WF_CC_NOTE", true);
                    SetFieldDisable("WF_BTN_CC", true);
                }


            }
            else
            {
                SetFieldDisable("WF_BTN_APPROVE", true);
                SetFieldDisable("WF_BTN_DISAPPROVE", true);
                SetFieldDisable("WF_BTN_CLARIFY", true);
                SetFieldDisable("WF_BTN_CLARIFY_ALL", true);
                SetFieldDisable("CLARIFY_TO", true);
                SetFieldDisable("WF_BTN_CLARIFY_SELECT", true);
            }

            if (Session.LangId == 0)
            {
                SetFieldValue("PRF_NAME", GetFieldValue("PRF_NAME_0"));
            }
            else if (Session.LangId == 1)
            {
                SetFieldValue("PRF_NAME", GetFieldValue("PRF_NAME_1"));
            }


            if (!string.IsNullOrEmpty(GetFieldValue<string>("WF_ADD_RECIP_ID")))
            {
                //CCOfficer()
            }

            await NavigateScrn();
            return await Task.FromResult<object>(null);
        }

        protected override async Task<object> SavedRecAsync()
        {
            return await Task.FromResult<object>(null);
        }

        protected override async Task<bool> ValidEntryAsync()
        {
            if (ScrnMode == ScreenMode.Add)
            {
            }
            return await Task.FromResult<bool>(true);
        }

        private async Task<object> InitFieldsAsync()
        {
            
            
            return await Task.FromResult<object>(null);
        }
        #endregion


        #region "Contorl Events"

        public async void WF_BTN_APPROVE_OnClick() {
            string wfRemark = GetFieldValue<string>("WF_COMMENTS");
            await _wfClient.ApproveAsync(_runNo, wfRemark);
            SetFieldDisable("WF_BTN_APPROVE", true);
            SetFieldDisable("WF_BTN_DISAPPROVE", true);
            SetFieldDisable("WF_BTN_CLARIFY", true);
            SetFieldDisable("WF_BTN_CLARIFY_ALL", true);
            SetFieldDisable("CLARIFY_TO", true);
            SetFieldDisable("WF_BTN_CLARIFY_SELECT", true);
            Session.ShowMessage(sAppLink + " has been Approved");

            this.Close();
        }

        public async void WF_BTN_DISAPPROVE_OnClick() {
            string wfRemark = GetFieldValue<string>("WF_COMMENTS");
            await _wfClient.DenyAsync(_runNo, wfRemark);
            SetFieldDisable("WF_BTN_APPROVE", true);
            SetFieldDisable("WF_BTN_DISAPPROVE", true);
            SetFieldDisable("WF_BTN_CLARIFY", true);
            SetFieldDisable("WF_BTN_CLARIFY_ALL", true);
            SetFieldDisable("CLARIFY_TO", true);
            SetFieldDisable("WF_BTN_CLARIFY_SELECT", true);
            Session.ShowMessage(sAppLink + "has been Rejected");
            this.Close();
        }

        public async void WF_BTN_CLARIFY_OnClick() {
            string wfRemark = GetFieldValue<string>("WF_COMMENTS");
            await _wfClient.ClarifyAsync(_runNo, wfRemark);
            SetFieldDisable("WF_BTN_APPROVE", true);
            SetFieldDisable("WF_BTN_DISAPPROVE", true);
            SetFieldDisable("WF_BTN_CLARIFY", true);
            SetFieldDisable("WF_BTN_CLARIFY_ALL", true);
            SetFieldDisable("CLARIFY_TO", true);
            SetFieldDisable("WF_BTN_CLARIFY_SELECT", true);
            Session.ShowMessage(sAppLink + " has been Clarified");
            this.Close();
        }

        public async void WF_BTN_CLARIFY_ALL_OnClick() {
            string wfRemark = GetFieldValue<string>("WF_COMMENTS");
            await _wfClient.ClarifyAllAsync(_runNo, wfRemark);
            SetFieldDisable("WF_BTN_APPROVE", true);
            SetFieldDisable("WF_BTN_DISAPPROVE", true);
            SetFieldDisable("WF_BTN_CLARIFY", true);
            SetFieldDisable("WF_BTN_CLARIFY_ALL", true);
            SetFieldDisable("CLARIFY_TO", true);
            SetFieldDisable("WF_BTN_CLARIFY_SELECT", true);
            Session.ShowMessage(sAppLink + " has been Clarified");
            this.Close();
        }

        public async void WF_BTN_CLARIFY_SELECT_OnClick() {
            string wfRemark = GetFieldValue<string>("WF_COMMENTS");
            string recips = GetFieldValue<string>("RECIP_IDS");
            await _wfClient.ClarifySelectAsync(_runNo, recips, wfRemark);
            SetFieldDisable("WF_BTN_APPROVE", true);
            SetFieldDisable("WF_BTN_DISAPPROVE", true);
            SetFieldDisable("WF_BTN_CLARIFY", true);
            SetFieldDisable("WF_BTN_CLARIFY_ALL", true);
            SetFieldDisable("CLARIFY_TO", true);
            SetFieldDisable("WF_BTN_CLARIFY_SELECT", true);
            Session.ShowMessage(sAppLink + " has been Clarified");
            this.Close();
        }
        public async void BTN_VIEW_DOC_OnClick()
        {
            
            string sScrnID = GetFieldValue<string>("PRF_SCRN_ID");
            string paramkey = _compCode + "," + _docType + "," + _deptCode + "," + _runNo;
            if (sScrnID != "" && sAppLink != "")
            {
                sScrnID = await get_ScrnIDAsync(_compCode, sScrnID, _appId, _wfId, sAppLink);

                string sKeys = Convert.ToString(await Session.ExecServerFuncAsync("MecWise.Blazor.Api.WorkflowServer",
                "WF_APRV_OS_BLZ", "GetFieldsID", sScrnID));
                 NavigateScreen(sScrnID, ScreenMode.Update, sKeys, sKeys, paramkey);
                await OpenPopupScreenAsync(sScrnID, ScreenMode.Update, sKeys, sKeys, paramkey);
            }

        }
        public async Task<bool> NavigateScrn()
        {
            
            string sScrnID = GetFieldValue<string>("PRF_SCRN_ID");
            string paramkey = _compCode + "," + _docType + "," + _deptCode + "," + _runNo;
            
                sScrnID = await get_ScrnIDAsync(_compCode, sScrnID, _appId, _wfId, sAppLink);

                string sKeys = Convert.ToString(await Session.ExecServerFuncAsync("MecWise.Blazor.Api.WorkflowServer",
                "WF_APRV_OS_BLZ", "GetFieldsID", sScrnID));
                NavigateScreen(sScrnID, ScreenMode.Update, sKeys, sKeys, paramkey, false);
            
           
            return true;
        }
        #endregion


        #region "Helper Functions"
        public async Task<string> get_ScrnIDAsync(string sCompcode,string sScrnID, string sAppId, string sWfId, string sAppLink)
        {
            return Convert.ToString(await Session.ExecServerFuncAsync("MecWise.Blazor.Api.WorkflowServer",
                    "WF_APRV_OS_BLZ", "GetScrnID", sCompcode, sScrnID, sAppId, sWfId, sAppLink));
        }

        #endregion

    }
}
