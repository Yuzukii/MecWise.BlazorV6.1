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

namespace MecWise.Blazor.Api.WorkflowServer
{
    public class WF_APRV_OS_BLZ : ScreenService
    {
        public JObject GetWorkflowObject(string sAppId, string sWfId, string sCompCode, string sUserId, string sEmpeId, int langId) {
            
            JObject retObj = new JObject();
            retObj.Add("msUserId", null);
            retObj.Add("msRights", null);
            retObj.Add("msPrevComments", null);
            retObj.Add("msActionDesc", null);
            retObj.Add("msAppLink", null);
            retObj.Add("msCurrRecipRole", null);
            retObj.Add("miCurrRecipLineNo", null);
            retObj.Add("msApprButtonDesc", null);
            retObj.Add("msDenyButtonDesc", null);

            retObj.Add("isClarifyActionEnabled", null);
            retObj.Add("isClarifyAllActionEnabled", null);
            retObj.Add("isClarifyToSelectionEnabled", null);
            retObj.Add("isClarifySelectActionEnabled", null);

            WorkflowManager moWorkflow = new WorkflowManager(DB.GetEPSession(sCompCode));
            WorkflowRequest moRequest = moWorkflow.GetARequest(sAppId, sWfId);

            if (moRequest == null) {
                retObj["msRights"] = "NONE";
                retObj["msPrevComments"] = "";
                retObj["msActionDesc"] = "COULD NOT INITIATE REQUEST....APP_ID : " + sAppId + " WF_ID : " + sWfId;
                return retObj;
            }

            retObj["msPrevComments"] = moRequest.Comments();
            moRequest.UserId = sUserId;
            retObj["msRights"] = moRequest.GetRights();
            retObj["msAppLink"] = moRequest.AppLink;

            WorkflowRecipient moRecipient = moRequest.Recips.ItemByUserId(sUserId);
            if (moRecipient == null) {
                moRecipient = moRequest.Recips.ItemByLineNo(moRequest.CurrRecipLNo);
            }

            retObj["msCurrRecipRole"] = moRecipient.RoleId;
            retObj["miCurrRecipLineNo"] = moRequest.CurrRecipLNo;

            if (moRequest.RequestCommand == WorkflowDefn.RequestCommand.RC_START || moRequest.RequestCommand == WorkflowDefn.RequestCommand.RC_RESUME) {
                retObj["msActionDesc"] = moRecipient.RoleDescription;
            }
            else {
                retObj["msActionDesc"] = moRecipient.GetRoleTerm("ACT_DESC", sAppId);
            }
            retObj["msApprButtonDesc"] = moRecipient.GetRoleTerm("BTN_APPR", sAppId);
            retObj["msDenyButtonDesc"] = moRecipient.GetRoleTerm("BTN_DENY", sAppId);

            string sNoted = string.Empty;
            string sSql = string.Empty;
            string sAPP_IDs = string.Empty;
            JObject sRow = null;

            sSql = "Select TERM_DESC_" + langId.ToStr() + " From %<:DbOwner>SV_WF_MF_TERMS " +
                "Where TERM_TYPE = %s And TERM_CODE = %s And TERM_SUB_CODE = %s";

            sSql = DB.SqlExpr(sSql, "BTN_APPR", "DEFAULT", "RN");
            sNoted = DB.GetAValue(sSql).ToStr();
            if (string.IsNullOrEmpty(sNoted)) {
                return retObj;
            }

            try {
                if (string.IsNullOrEmpty(retObj["msApprButtonDesc"].ToStr())) {
                    return retObj;
                }
                sSql = "Select PREFER_VAL, RSRV_CHAR_1, RSRV_CHAR_2, RSRV_CHAR_3 From %<:DbOwner>SV_WF_MF_PREFER " +
                        "WHERE COMP_CODE = %s And APP_ID = %s And PREFER_CODE = %s";
                sSql = DB.SqlExpr(sSql, sCompCode, "$SYS", "WF-ACTN-NOTED-CAPTION");
                sRow = DB.GetARow(sSql);
                if (sRow != null) {
                    string enabled = sRow["PREFER_VAL"].ToStr();
                    enabled = string.IsNullOrEmpty(enabled) ? "N" : enabled;
                    if (enabled.ToUpper() == "Y") {
                        sAPP_IDs = sRow["RSRV_CHAR_1"].ToStr() + "," + sRow["RSRV_CHAR_2"].ToStr();
                        sAPP_IDs = sAPP_IDs.Replace(" ", String.Empty);
                        if (!sAPP_IDs.Equals(",") && !string.IsNullOrEmpty(sAppId)) {
                            if (sAPP_IDs.Contains(sAppId + ",") || sAPP_IDs.Contains("," + sAppId)) {
                                retObj["msApprButtonDesc"] = sRow["RSRV_CHAR_3"].ToStr();
                            }
                        }
                    }
                }
            }
            catch (Exception) {
            }


            retObj["isClarifyActionEnabled"] = WorkflowFunctions.IsClarifyActionEnabled(DB.GetEPSession(sCompCode), sAppId, sWfId, sEmpeId);
            retObj["isClarifyAllActionEnabled"] = WorkflowFunctions.IsClarifyAllActionEnabled(DB.GetEPSession(sCompCode), sAppId, sWfId, sEmpeId);
            retObj["isClarifyToSelectionEnabled"] = WorkflowFunctions.IsClarifyToSelectionEnabled(DB.GetEPSession(sCompCode), sAppId, sWfId, sEmpeId);
            retObj["isClarifySelectActionEnabled"] = WorkflowFunctions.IsClarifySelectActionEnabled(DB.GetEPSession(sCompCode), sAppId, sWfId, sEmpeId);

            

            return retObj;
        }
        public string GetScrnID(string sCompcode, string sScrnID,string sAppId, string sWfId, string sAppLink)
        {
            string scrn_id = DB.GetAValue("Select SCRN_ID From %<:DbOwner>CF_WF_VIEW_DOC_SCRN_ID (%s, %s, %s, %s, %s)", sCompcode, sScrnID, sAppId, sWfId, sAppLink).ToStr();

            return scrn_id;

        }
        public string GetFieldsID(string sScrnID)
        {
            string FieldsID = DB.GetAValue("SELECT FIELD_KEYS FROM %<:DBOWNER>SV_EPF_SCRN WHERE SCRN_ID= %s", sScrnID).ToStr();

            return FieldsID;

        }
        
    }
}
