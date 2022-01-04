using MecWise.Blazor.Common;
using MecWise.Blazor.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MecWise.Blazor.Workflow.Client
{
    public class WFAction : Screen
    {

        #region Private Declarations
        private string _logonKey;
        private string _prefName = string.Empty;
        private string _reqName = string.Empty;
        //private EPSession _sess;
        private string[] _keys;                     //[30/07/10, Rajeeb]
        private string _userId = string.Empty;      //[27/01/11, Rajeeb]
        private bool _isUserAuthenticationEnabled = false;
        #endregion


        #region Properties

        ///// <summary>
        ///// gets the logon key to validate the request
        ///// </summary>
        //private string LogonKey {
        //    get {
        //        if (string.IsNullOrEmpty(_logonKey)) {
        //            if (_keys != null)
        //                _logonKey = this.GetKeyValue("lk");       //[30/07/10, Rajeeb]
        //            else
        //                _logonKey = this.GetQueryParamValue("lk");


        //            if (IsEncoded) _logonKey = this.DecryptString(_logonKey);
        //        }
        //        return _logonKey;
        //    }
        //}

        private string UserID {
            get {
                return _userId;
            }
        }

        /// <summary>
        /// gets the logon key to validate the request
        /// </summary>
        private string WorkflowID {
            get {
                if (_keys != null) return this.GetKeyValue("wfid");         //[30/07/10, Rajeeb]
                return this.GetQueryStringValue("wfid");
            }
        }

        /// <summary>
        /// gets the application id
        /// </summary>
        private string AppID {
            get {
                if (_keys != null) return this.GetKeyValue("appid");        //[30/07/10, Rajeeb]
                return this.GetQueryStringValue("appid");
            }
        }

        /// <summary>
        /// Gets the Action to be performed
        /// </summary>
        private int Action {
            get {
                if (_keys != null) return Convert.ToInt32(this.GetKeyValue("act"));     //[30/07/10, Rajeeb]
                return Convert.ToInt32(this.GetQueryStringValue("act"));
            }
        }

        private async Task<string> RecipientIDAsync() {
            if (IsEncoded) {
                if (_keys != null) {
                    return await Session.ExecServerFuncAsync<string>("MecWise.Blazor.Api.WorkflowServer", "WFAction", "DecryptString", this.GetKeyValue("recip"));
                }

                return await Session.ExecServerFuncAsync<string>("MecWise.Blazor.Api.WorkflowServer", "WFAction", "DecryptString", this.GetQueryStringValue("recip"));
            }
            else {
                if (_keys != null) return this.GetKeyValue("recip");        //[30/07/10, Rajeeb]
                return this.GetQueryStringValue("recip");
            }
        }


        private bool IsEncoded {
            get {
                try {
                    // 0 - Do Not Encode. 1  - Keys are encoded.
                    if (_keys != null) return this.GetKeyValue("enc").Equals("1");  //[30/07/10, Rajeeb]
                    return this.GetQueryStringValue("enc").Equals("1");
                }
                catch { return true; }
            }
        }

        //[141106, Rajeeb]
        private string DelegateId {
            get {
                try {
                    if (_keys != null) return this.GetKeyValue("dgid");
                    return this.GetQueryStringValue("dgid");
                }
                catch { return string.Empty; }
            }
        }

        // Added by Rajeeb on 07/12/2006
        private bool IsNotify {
            get {
                int action = this.Action;
                if (action == 3 || action == 6) return true;
                return false;
            }
        }

        private bool DisplayUserComments {
            get {
                return (this.Action == 5 || this.Action == 4 || this.Action == 6);
            }
        }

        //[30/07/10, Rajeeb]
        private bool IsUserAuthenticationEnabled {
            get {
                return _isUserAuthenticationEnabled;
            }
        }

        //[27/01/11, Rajeeb]
        private async Task<string> RecipientRoleIDAsync() {
            return await Session.ExecServerFuncAsync<string>("MecWise.Blazor.Api.WorkflowServer", "WFAction", "GetRecipientRoleID",
                    this.AppID, this.WorkflowID, await this.RecipientIDAsync());
        }
        #endregion

        #region "Screen Events"
        protected override async Task<object> LoadAsync()
        {
            SetTitleBarMenuVisible(false);

            await this.GetQueryStringValue();     //[30/07/10, Rajeeb]

            if (!this.IsUserAuthenticationEnabled) {
                SetFieldVisible("txtUserAttr", false);
            }

            bool ok = false;
            if (await this.ValidWorkflowAsync()) {
                
                ok = this.DisplayUserComments;
                if (!ok) // Direct approval / deny
                    await this.DoWfActionAsync(string.Empty);
                else
                    this.SetDisplayData();
            }
            this.DisplayPanels(ok);



            return await base.LoadAsync();
        }

        protected override async Task<object> ShowRecAsync()
        {
            
            if (ScrnMode == ScreenMode.Add)
            {
                await InitFieldsAsync();
            }
            return await Task.FromResult<object>(null);
        }

        protected override async Task<object> SavedRecAsync()
        {
            return await Task.FromResult<object>(null);
        }

        protected override async Task<bool> ValidEntryAsync()
        {
            return await Task.FromResult<bool>(true);
        }

        private async Task<object> InitFieldsAsync()
        {
            return await Task.FromResult<object>(null);
        }
        #endregion

        #region "Control Events"

        public async void btnWfAction_OnClick() {
            if (this.IsUserAuthenticationEnabled)           //[30/07/10, Rajeeb]
            {
                bool validUser = await Session.ExecServerFuncAsync<bool>("MecWise.Blazor.Api.WorkflowServer", "WFAction", "ValidateUser", 
                    this.AppID, this.WorkflowID, await this.RecipientIDAsync(), await this.RecipientRoleIDAsync());

                if (!validUser) {
                    this.SetMsg(this.GetMessage(130));
                    this.DisplayPanels(false);
                    return;
                }
            }

            Console.WriteLine("DoWfAction");
            await this.DoWfActionAsync(this.GetFieldValue<string>("txtUserComments"));
            this.DisplayPanels(false);

            Console.WriteLine("Refresh");
            Refresh();
        }

        public void btnClose_OnClick() {
            this.Close();
        }

        #endregion


        #region Private/Helper Methods
        //[30/07/10, Rajeeb]
        private async Task GetQueryStringValue() {
            string key = base.GetQueryStringValue("wk");
            JObject settings = await Session.ExecServerFuncAsync<JObject>("MecWise.Blazor.Api.WorkflowServer", "WFAction", "GetQueryStringValue", key);
            Console.WriteLine(settings.ToString());
            if (!string.IsNullOrEmpty(settings["keys"].ToStr())) {
                _keys = settings["keys"].ToStr().Split(char.Parse("&"));
            }
            else {
                _keys = null;
            }


            if (!string.IsNullOrEmpty(settings["isUserAuthenticationEnabled"].ToStr())) {
                _isUserAuthenticationEnabled = settings["isUserAuthenticationEnabled"].ToBool();
            }
            else {
                _isUserAuthenticationEnabled = false;
            }


            if (!string.IsNullOrEmpty(settings["userId"].ToStr())) {
                _userId = settings["userId"].ToStr();
            }
            else {
                _userId = string.Empty;
            }

        }

        //[30/07/10, Rajeeb]
        private string GetKeyValue(string key) {
            if (_keys != null) {
                foreach (string keyVal in _keys) {
                    if (keyVal.ToLower().StartsWith(key + "="))
                        return keyVal.Substring(keyVal.IndexOf("=") + 1);
                }
            }

            return string.Empty;
        }

        //    private string DecryptString(string cipherText) {
        //        EPEncrypt enc = new EPEncrypt();

        //        try { return enc.DecryptString(cipherText); }
        //        finally { enc = null; }
        //    }

        private async Task DoWfActionAsync(string comments) {
            JObject retObj = await Session.ExecServerFuncAsync<JObject>("MecWise.Blazor.Api.WorkflowServer", "WFAction", "DoWfAction", 
                this.AppID, this.WorkflowID, this.UserID, await RecipientIDAsync(), this.DelegateId, this.Action, comments);

            this.SetMsg(this.GetMessage(retObj["msgCode"].ToInt()));
        }



        //    

        private void DisplayPanels(bool display) {

            SetFieldVisible("panelMsg", !display);
            SetFieldVisible("panelComments", display);

            if (display) {
                switch (this.Action) {
                    case 1:
                    case 4:
                        SetFieldDescription("btnWfAction", "Approve/Recommend");
                        break;
                    case 2:
                    case 5:
                        SetFieldDescription("btnWfAction", "Deny");
                        break;
                    case 3:
                    case 6:
                        SetFieldDescription("btnWfAction", "Noted");
                        break;
                    default:
                        SetFieldVisible("btnWfAction", false);
                        break;
                }
            }
        }

        /// <summary>
        /// Displays success / error messages to the user
        /// </summary>
        /// <param name="msg">the message to be displayed</param>
        private void SetMsg(string msg) {
            this.SetFieldDescription("lblMsg", msg);
        }

        private string GetMessage(int msgCode) {
            switch (msgCode) {
                case 1:
                case 4:
                    return "Application has been recommended / approved successfully";
                case 2:
                case 5:
                    return "Application has been denied";
                case 3:
                case 6:
                    return "Application has been noted";
                case -1: return "Error while recommending / approving application";
                case -2: return "Error while denying application";
                case 33: return "Error while noted";
                case -3:
                case 100: return "Application has already been acted upon";
                case 110: return "Invalid Recipient";
                case 120: return "Invalid WF Application";
                case 130: return "Invalid user / password";
                case 140: return "You do NOT have rights to act on your application";
                default: return "Invalid action specified";
            }
        }

        private async Task<bool> ValidWorkflowAsync() {
            int errCode = this.Action;
            if (errCode < 1 || errCode > 5) {
                this.SetMsg(this.GetMessage(-100));
                return false;
            }


            JObject retVal = await Session.ExecServerFuncAsync<JObject>("MecWise.Blazor.Api.WorkflowServer", "WFAction", "IsValidApplication",
                this.AppID, this.WorkflowID, Session.CompCode, this.IsNotify, await RecipientIDAsync(), this.DelegateId);

            
            bool valid = retVal["valid"].ToBool();
            _prefName = retVal["_prefName"].ToStr();
            _reqName = retVal["_reqName"].ToStr();
            errCode = retVal["errCode"].ToInt();


            string info = string.Format("{0} of {1}", _prefName, _reqName); ;
            SetFieldDescription("lblAppTitle", info);

            if (!valid) {
                this.SetMsg(this.GetMessage(errCode));
                return false;
            }

            return true;
        }

        



        private async void SetDisplayData() {

            string displayData = await Session.ExecServerFuncAsync<string>("MecWise.Blazor.Api.WorkflowServer", "WFAction", "GetDisplayData",
                this.AppID, this.WorkflowID, this.UserID);

            SetFieldValue("txtPrevComments", displayData);

        }

        //    private void SetInfoLabel(string requesterId) {
        //        try {
        //            string sql = "Select FAM_NAME From %<:DbOwner>SV_WF_EMPE_lIST Where COMP_CODE = %s And EMPE_ID = %s";
        //            sql = this.DBSession.SqlExpr(sql, this.DBSession.CoyID, requesterId);
        //            _reqName = Convert.ToString(this.DBSession.GetAVal(sql, string.Empty));

        //            if (!WebFuncs.cmIsValid(_reqName)) {
        //                sql = "Select FAM_NAME From %<:DbOwner>SV_WF_EMPE_lIST Where RECIP_COMP_CODE = %s And EMPE_ID = %s";
        //                sql = this.DBSession.SqlExpr(sql, this.DBSession.CoyID, requesterId);
        //                _reqName = Convert.ToString(this.DBSession.GetAVal(sql, string.Empty));
        //            }
        //        }
        //        catch { _reqName = string.Empty; }

        //        lblInfo.Text = string.Format("{0} of {1}", _prefName, _reqName);
        //    }



        #endregion
    }
}
