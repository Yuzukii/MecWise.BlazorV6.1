using MecWise.Blazor.Api.Services;
using MecWise.Blazor.Api.DataAccess;
using MecWise.Blazor.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MecWise.WorkflowCommon;
using MecWise.WorkflowEngine;
using MecWise.WorkflowBase;
using ePlatform.Security;

namespace MecWise.Blazor.Api.WorkflowServer
{
    public class WFAction : ScreenService
    {
        public bool IsUserAuthenticationEnabled() {
            return (Convert.ToString(MecWise.Framework.cmFuncs.AppValue(
                    "WF_QCKAPRV_AUTH_ENABLED", "false")).ToLower() == "true");
        }

        public string GetRecipientRoleID(string appId, string workflowId, string recipientId) {
            try {
                string sql =
                    "Select A.RECIP_ROLE_ID From %<:DbOwner>SV_WF_REQUEST_RECIPS A " +
                    "Join %<:DbOwner>SV_WF_REQUEST B On A.APP_ID = B.APP_ID And A.WF_ID = B.WF_ID And A.RECIP_ID = B.CURR_RECIP_ID " +
                    "Where A.APP_ID = %s And A.WF_ID = %s And A.RECIP_ID = %s";
                return DB.GetAValue(sql, appId, workflowId, recipientId).ToStr();
            }
            catch { return string.Empty; }
        }


        public JObject GetQueryStringValue(string key) {
            JObject retValue = new JObject();
            string keys = string.Empty;
            if (!string.IsNullOrEmpty(key)) {
                key = key.Replace("~~", "+");
                keys = this.DecryptString(key);
            }

            bool isUserAuthenticationEnabled = IsUserAuthenticationEnabled();
            
            retValue.Add("keys", keys);
            retValue.Add("isUserAuthenticationEnabled", isUserAuthenticationEnabled);
            retValue.Add("userId", this.GetUser(string.Empty, keys.Split(char.Parse("&"))));


            

            return retValue;
        }

        
        public string DecryptString(string cipherText) {
            EPEncrypt enc = new EPEncrypt();
            
            try { 
                return enc.DecryptString(cipherText);
            }
            finally { 
                enc = null; 
            }
        }

        
        public JObject IsValidApplication(string appId, string workflowId, string compCode, bool isNotify, string recipientId, string delegateId) {
            JObject retVal = new JObject();
            retVal.Add("valid", false);
            retVal.Add("_prefName", string.Empty);
            retVal.Add("_reqName", string.Empty);
            retVal.Add("errCode", 1);


            string sql =
                "Select A.CURR_RECIP_ID, A.WF_STATE, B.PRF_NAME_0, C.FAM_NAME, A.REQ_ID " +
                "From %<:DbOwner>SV_WF_REQUEST A, %<:DbOwner>SV_WF_MF_PROFILE B, %<:DbOwner>SV_WF_EMPE_PROFILE_WEB C " +
                "Where A.APP_ID = %s And A.WF_ID = %s And B.APP_ID = A.APP_ID " +
                "And C.EMPE_ID = A.CURR_RECIP_ID And A.COMP_CODE = %s";
            
            JObject rowArr = DB.GetARow(sql, appId, workflowId, compCode);

            // Added by Rajeeb, 12-Oct-2006
            // Check for Action taken by cross-company approver.
            if (rowArr == null) {
                //object[] rowArray = null;
                sql =
                    "Select A.CURR_RECIP_ID, A.WF_STATE, B.PRF_NAME_0, A.REQ_ID " +
                    "From %<:DbOwner>SV_WF_REQUEST A, %<:DbOwner>SV_WF_MF_PROFILE B " +
                    "Where A.APP_ID = %s And A.WF_ID = %s And B.APP_ID = A.APP_ID And A.COMP_CODE = %s";
                //sql = this.DBSession.SqlExpr(sql, this.AppID, this.WorkflowID, this.DBSession.CoyID);
                JObject rowArray = DB.GetARow(sql, appId, workflowId, compCode);

                if (rowArray == null) {
                    retVal["valid"] = false;
                    return retVal;
                }
                if (!isNotify) {
                    if (!rowArray["WF_STATE"].ToStr().ToUpper().Equals("PEND")) {
                        retVal["valid"] = false;
                        retVal["errCode"] = 100;
                        return retVal;
                    }

                    if (!rowArray["CURR_RECIP_ID"].ToStr().ToUpper().Equals(recipientId.ToUpper())) {
                        retVal["valid"] = false;
                        retVal["errCode"] = 110;
                        return retVal;
                    }

                    //[141106, Rajeeb]
                    if (rowArray["REQ_ID"].ToStr().ToUpper().Equals(delegateId.ToUpper())) {
                        retVal["valid"] = false;
                        retVal["errCode"] = 140;
                        return retVal;
                    }
                }

                if (string.IsNullOrEmpty(rowArray["PRF_NAME_0"].ToStr()))
                    retVal["_prefName"] = rowArray["PRF_NAME_0"].ToStr();
                

                //if (WebFuncs.cmIsValid(rowArray.GetValue(3)))
                //	_reqName = rowArray.GetValue(3).ToString();
            }
            else {
                if (!isNotify) {
                    if (!rowArr["CURR_RECIP_ID"].ToString().ToUpper().Equals(recipientId.ToUpper())) {
                        retVal["valid"] = false;
                        retVal["errCode"] = 110;
                        return retVal;
                    }

                    if (!rowArr["WF_STATE"].ToString().ToUpper().Equals("PEND")) {
                        retVal["valid"] = false;
                        retVal["errCode"] = 100;
                        return retVal;
                    }

                    //[141106, Rajeeb]
                    if (rowArr["REQ_ID"].ToString().ToUpper().Equals(delegateId.ToUpper())) {
                        retVal["valid"] = false;
                        retVal["errCode"] = 140;
                        return retVal;
                    }
                }

                if (!string.IsNullOrEmpty(rowArr["PRF_NAME_0"].ToStr()))
                    retVal["_prefName"] = rowArr["PRF_NAME_0"].ToStr();

                if (!string.IsNullOrEmpty(rowArr["FAM_NAME"].ToStr()))
                    retVal["_reqName"] = rowArr["FAM_NAME"].ToStr();
            }

            retVal["valid"] = true;
            retVal["errCode"] = 0;
            return retVal;
        }

        private string GetLogFilePath() {
            string filePath = System.Configuration.ConfigurationManager.AppSettings["LogPath"];
            if (filePath == string.Empty || filePath == "" || filePath == null) filePath = @"~\..\Log\";
            if (filePath.IndexOf("~") >= 0) filePath = filePath.Replace(
                "~", System.AppDomain.CurrentDomain.BaseDirectory);
            return System.IO.Path.GetFullPath(filePath);
        }

        //[131121, Rajeeb]
        //removed hard-coded log file name for workflow log file
        private string GetLogFilePrefix() {
            string prefix = string.Empty;
            try { prefix = System.Configuration.ConfigurationManager.AppSettings["LogPrefix"]; }
            catch { prefix = string.Empty; }
            return (string.IsNullOrEmpty(prefix) ? "MecWiseWorkflow" : prefix);
        }


        public string GetDisplayData(string appId, string workflowId, string userId) {
            string comments = string.Empty;
            WorkflowManager wf = new WorkflowManager(this.DB.EPSession);
            WorkflowErrorCollection coll = new WorkflowErrorCollection();
            WorkflowRequest req = null;

            try {
                coll.LogToType = ErrorDefn.LogType.LogToFile;
                wf.Errors = coll;
                wf.LanguageID = "0"; // Always English
                req = wf.GetARequest(appId, workflowId);
                req.UserId = userId;

                comments = req.Comments();

                return comments;
            }
            finally {
                if (wf != null) wf.Dispose();
                wf = null;
                coll = null;
            }
        }

        public JObject DoWfAction(string appId, string workflowId, string userId, string recipientId, string delegateId, int action, string comments) {
            // TODO : Do WF Action Here
            /* Check Currrent Status of WF
            * If Approved or Cancelled or Withdrawn.. Display Error
            * Else proceeed with action
            * Display appropriate info to user
            * */

            JObject retVal = new JObject();
            retVal.Add("status", "");
            retVal.Add("msgCode", "");
            retVal.Add("errMsg", "");

            WorkflowErrorCollection coll = new WorkflowErrorCollection();
            coll.DBConnect = DB.EPSession;
            coll.LogToType = ErrorDefn.LogType.LogToFile;
            coll.LogFilePath = this.GetLogFilePath();
            coll.LogFilePrefix = this.GetLogFilePrefix();       //[131121, Rajeeb]
                                                                //coll.Add("Before action taken by Routing officer.");

            WorkflowManager wf = new WorkflowManager(DB.EPSession);
            wf.Errors = coll;
            wf.LanguageID = "0"; // Always English
            WorkflowRequest req = wf.GetARequest(appId, workflowId);

            try {
                bool ok = false;
                req.UserId = this.GetUser(userId, recipientId, delegateId);
                switch (action) {
                    case 1:
                    case 4:
                        ok = req.Approve(comments);
                        break;
                    case 2:
                    case 5:
                        ok = req.Deny(comments);
                        break;
                    case 3:
                    case 6:
                        ok = req.Noted(comments);
                        break;
                }

                //coll.Add("After action taken by Routing officer.");
                coll.Show();

                retVal["status"] = true;
                retVal["msgCode"] = action * (ok ? 1 : -1);
                retVal["errMsg"] = "";

            }
            catch (Exception ex) {
                retVal["status"] = false;
                retVal["msgCode"] = 0;
                retVal["errMsg"] = ex.Message;
            }
            finally {
                if (wf != null) wf.Dispose();
                wf = null;
                coll = null;
            }

            return retVal;
        }

        public string GetUser(string userId, string[] keys) {
            string recipientId = this.GetKeyValue("recip", keys);
            string delegateId = this.GetKeyValue("dgid", keys);

            return this.GetUser(userId, recipientId, delegateId);
        }

        public string GetUser(string userId, string recipientId, string delegateId) {

            if (string.IsNullOrEmpty(userId))  //[27/01/11, Rajeeb]
            {
                //[150604, Rajeeb]
                string id = recipientId;
                if (!string.IsNullOrEmpty(delegateId))
                    id = delegateId;

                string sql = "Select [USER_ID] From %<:DbOwner>SV_SC_USER_PROFILE Where EMPE_ID = %s";
                userId = this.DB.GetAValue(sql, id).ToStr();
            }

            return userId;
        }

        //[30/07/10, Rajeeb]
        private string GetKeyValue(string key, string[] keys) {
            if (keys != null) {
                foreach (string keyVal in keys) {
                    if (keyVal.ToLower().StartsWith(key + "="))
                        return keyVal.Substring(keyVal.IndexOf("=") + 1);
                }
            }

            return string.Empty;
        }


        //[30/07/10, Rajeeb]
        private bool ValidateUser(string attr, string appId, string workflowId, string recipientId, string recipientRoleId) {
            if (string.IsNullOrEmpty(attr)) return false;

           
            //[141106, Rajeeb]
            string sql =
                "Select [USER_ID] From %<:DbOwner>CFT_WF_TASK_OS_RECIPS() Y " +
                "Join %<:DbOwner>SV_SC_USER_PROFILE S On Y.APRV_ID = S.EMPE_ID  " +
                "Where APP_ID = %s And WF_ID = %s And RECIP_ID = %s And RECIP_ROLE_ID = %s";
            sql = this.DB.SqlExpr(sql, appId, workflowId, recipientId, recipientRoleId);

            
            JArray rows = DB.GetRows(sql);
            if (rows.Count > 0) {
                foreach (var row in rows) {
                    try {
                        string name = row["USER_ID"].ToStr();
                        MecWise.Security.UserPasswordProfile profile = new
                            MecWise.Security.UserPasswordProfile(this.DB.EPSession);
                        if (profile.Fetch(name)) {
                            profile.UserID = name;
                            if (profile.ValidatePassword(attr)) {
                                //_userId = name;
                                return true;
                            }
                        }
                    }
                    catch { return false; }
                }
            }
            return false;
        }
    }
}
