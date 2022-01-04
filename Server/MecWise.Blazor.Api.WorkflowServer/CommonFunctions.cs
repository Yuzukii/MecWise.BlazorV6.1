using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MecWise.Blazor.Api.Services;
using MecWise.Blazor.Api.DataAccess;
using MecWise.Blazor.Common;
using MecWise.WorkflowCommon;
using System.Data;
using ePlatform.WebObjects;
using ePlatform.CommonClasses;
using epObjects;
using ePlatform.BusinessObject;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

namespace MecWise.Blazor.Api.WorkflowServer {
    public class CommonFunctions : ScreenService
    {
        public bool PrepareRouting(string cmd, string CompCode, string docType, string deptCode, string RunNo, string AppID, string EmpeID, string OrgUnitCode)
        {
            DBParameterCollection Params = new DBParameterCollection();
            Params.Add("RET_VAL", 0);
            Params.Add("CMD", cmd);
            Params.Add("COMP_CODE", CompCode);
            Params.Add("DOC_TYPE", docType);
            Params.Add("DEPT_CODE", deptCode);
            Params.Add("RUN_NO", RunNo);
            Params.Add("APP_ID", AppID);
            Params.Add("EMPE_ID", EmpeID);
            Params.Add("ORG_UNIT", OrgUnitCode);
            DB.ExecProcedure("HR_UP_PREP_ROUTE_LIST", Params);

            return true;
        }

        public bool StartRoutWithEpfObj(string objDataSource, string EmpeID, string FormText, string currloginuser)
        {
            Boolean isStartRouteOK = false;
            try
            {
                JObject objData = JObject.Parse(objDataSource);
                ePlatform.BusinessObject.CommonFuncs cbo;
                epScreen screen;
                RecipientCollection recipCollection = new RecipientCollection();

                //Modified by LaPW on 06-Aug-2021, Added application ID - LTA_DLE, LTA_MRE
                if (objData["APP_ID"].ToStr().In("LTA_REP","LTA_DLE","LTA_MRE"))
                {
                    recipCollection = GetABSRecpient(objData["COMP_CODE"].ToString(), objData["DOC_TYPE"].ToString(), objData["DEPT_CODE"].ToString(), objData["RUN_NO"].ToString(), objData["SRL_NO"].ToString(), objData["LINE_NO"].ToString(), objData["COVER_SEQ"].ToString(), objData["APP_ID"].ToString());
                }
                else if (objData["APP_ID"].ToStr().In("TMS_CIL", "TMS_COL", "TMS_CIE", "TMS_COE", "TMS_COEF", "TMS_CNS", "TMS_LOC_ERR", "TMS_CIW"))
                {
                    recipCollection = GetAttTakingRecpient(objData["COMP_CODE"].ToString(), objData["DOC_TYPE"].ToString(), objData["DEPT_CODE"].ToString(), objData["RUN_NO"].ToString(), objData["SRL_NO"].ToString(),objData["APP_ID"].ToString(), EmpeID);
                }
                else // default routing 
                {
                    recipCollection = GetDefRouting(objData["COMP_CODE"].ToString(), objData["DOC_TYPE"].ToString(), objData["DEPT_CODE"].ToString(), objData["RUN_NO"].ToString(), objData["APP_ID"].ToString(), EmpeID);
                }

                isStartRouteOK  = InitEpfBO(out cbo, out screen, DB.GetEPSession(objData["COMP_CODE"].ToString()).ConnectionString, currloginuser, objData["COMP_CODE"].ToString(), DB);

                if (isStartRouteOK)
                {
                    //var comp_code = new WebField();
                    //comp_code.Name = "COMP_CODE";
                    //comp_code.Value = objData["COMP_CODE"].ToString();
                    //screen.Fields.Add(comp_code);

                    //var doc_type = new WebField();
                    //doc_type.Name = "DOC_TYPE";
                    //doc_type.Value = objData["DOC_TYPE"].ToString();

                    //var dept_code = new WebField();
                    //dept_code.Name = "DEPT_CODE";
                    //dept_code.Value = objData["DEPT_CODE"].ToString();

                    //var run_no = new WebField();
                    //run_no.Name = "RUN_NO";
                    //run_no.Value = objData["RUN_NO"].ToString();

                    //var app_id = new WebField();
                    //app_id.Name = "APP_ID";
                    //app_id.Value = objData["APP_ID"].ToString();

                    //var empe_id = new WebField();
                    //empe_id.Name = "EMPE_ID";
                    //empe_id.Value = EmpeID;

                    //var wf_id = new WebField();
                    //wf_id.Name = "WF_ID";
                    //wf_id.Value = "";

                    string customFieldsRes = DB.GetAValue("SELECT PREFER_VAL FROM SV_WF_MF_PREFER WHERE COMP_CODE = %s AND PREFER_CODE = %s AND APP_ID = %s", objData["COMP_CODE"].ToString(), "WF-CUSTOM-APP-LINK", objData["APP_ID"].ToString()).ToStr();
                    customFieldsRes = "COMP_CODE,DOC_TYPE,DEPT_CODE,RUN_NO,EMPE_ID,APP_ID," + customFieldsRes;
                    string[] customFields = customFieldsRes.Split(',');
                    Writelog(customFieldsRes.ToStr());
                    foreach (string fieldID in customFields)
                    {
                        var webFieldVal = new WebField();
                        string value = string.Empty;
                        webFieldVal.Name = fieldID;
                        if(fieldID == "EMPE_ID")
                        {
                            value = EmpeID;
                        }
                        else 
                        {
                            value = objData[fieldID].ToString();
                        }

                        webFieldVal.Value = value;

                        screen.Fields.Add(webFieldVal);
                        webFieldVal.Dispose();

                        Writelog("in loop : " + fieldID + ", Data :" + value);
                    }

                    //screen.Fields.Add(doc_type);
                    //screen.Fields.Add(dept_code);
                    //screen.Fields.Add(run_no);
                    //screen.Fields.Add(app_id);
                    //screen.Fields.Add(empe_id);
                    //screen.Fields.Add(srl_no);
                    //screen.Fields.Add(wf_id);

                    isStartRouteOK = WorkflowFunctions.StartRout(cbo, screen, EmpeID, FormText, recipCollection);
                }
            }
            catch (Exception ex)
            {
                Writelog(ex.ToString());
            }
            return isStartRouteOK;
        }

        public bool StartRout(string CompCode, string docType, string deptCode, string RunNo, string AppID, string EmpeID, string FormText, string currloginuser)
        {
            Boolean isStartRouteOK = false;
            try
            {
                RecipientCollection recipCollection = new RecipientCollection();
                recipCollection = GetDefRouting(CompCode, docType, deptCode, RunNo, AppID, EmpeID);
                isStartRouteOK = WorkflowFunctions.StartRout(DB.GetEPSession(CompCode), CompCode, docType, deptCode, RunNo, AppID, EmpeID, FormText, recipCollection);
            }
            catch (Exception ex)
            {
                isStartRouteOK = false;
            }
            return isStartRouteOK;
        }

        private void Writelog(string log)
        {
            //[31/05/2021, PhyoZin] Commented.
            //DB.ExecQuery<object>("update MF_DEFA set TYPE_DESC= %s where comp_code = 'PUB' and FIELD_NAME = 'temp_upd'", log);
        }

        public bool StartWorkflow(string appID, string wfID, string currLoginUser, string copID)
        {
            bool isStartWorkflowOK = true;
            ePlatform.BusinessObject.CommonFuncs cbo;
            epScreen screen;
            string refmsg = string.Empty;
            bool bOK = InitEpfBO(out cbo, out screen, DB.GetEPSession(copID).ConnectionString, currLoginUser, copID, DB);
            isStartWorkflowOK = WorkflowFunctions.StartWorkflow(cbo, screen, appID, wfID, true, ref refmsg);

            return isStartWorkflowOK;
        }

        public string GetWFID(string CompCode, string docType, string deptCode, string RunNo)
        {
            String WF_ID = WorkflowFunctions.GetWFID(DB.GetEPSession(CompCode), CompCode, docType, deptCode, RunNo);
            return WF_ID;
        }

        public string GetWFIDWithAppLink(string CompCode, string appLink)
        {
            String WF_ID = WorkflowFunctions.GetWFID(DB.GetEPSession(CompCode), appLink);
            return WF_ID;
        }

        public bool Approve(string appID, string wfID, string wfRemark, string currLoginUser, string copID)
        {
            Boolean isWFApproveOK = true;
            ePlatform.BusinessObject.CommonFuncs cbo;
            epScreen screen;
            string refmsg = string.Empty;
            bool bOK = InitEpfBO(out cbo, out screen, DB.GetEPSession(copID).ConnectionString, currLoginUser, copID, DB);
            
            isWFApproveOK = WorkflowFunctions.Approve(cbo, screen, appID, wfID, wfRemark, ref refmsg);
            return isWFApproveOK;
        }

        public bool Deny(string appID, string wfID, string wfRemark, string currLoginUser, string copID)
        {
            Boolean isWFDenyOK = true;
            ePlatform.BusinessObject.CommonFuncs cbo;
            epScreen screen;
            string refmsg = string.Empty;
            bool bOK = InitEpfBO(out cbo, out screen, DB.GetEPSession(copID).ConnectionString, currLoginUser, copID, DB);

            isWFDenyOK = WorkflowFunctions.Deny(cbo, screen, appID, wfID, wfRemark, ref refmsg);
            return isWFDenyOK;
        }   

        public bool ApproveAuto(string appID, string wfID, bool autoApprove, string wfRemark, string currLoginUser, string copID)
        {
            Boolean isWFApproveOK = true;
            //ePlatform.BusinessObject.CommonFuncs cbo;
            //epScreen screen;
            string refmsg = string.Empty;
            //bool bOK = InitEpfBO(out cbo, out screen, DB.GetEPSession(copID).ConnectionString, currLoginUser, copID, DB);

            //isWFApproveOK = WorkflowFunctions.Approve(cbo, screen, appID, wfID, autoApprove, wfRemark, ref refmsg);

            EPSession sess = new EPSession(DB.EPSession.DBMS, DB.EPSession.DbOwner, DB.EPSession.ConnectionString, DB.EPSession.CoyID);
            isWFApproveOK = WorkflowFunctions.Approve(sess, appID, wfID, currLoginUser, wfRemark, ref refmsg, autoApprove, false);

            return isWFApproveOK;
        }

        public bool Clarify(string appID, string wfID, string wfRemark, string currLoginUser, string copID)
        {
            bool isClarifyOk = true;
            ePlatform.BusinessObject.CommonFuncs cbo;
            epScreen screen;
            string refmsg = string.Empty;
            bool bOK = InitEpfBO(out cbo, out screen, DB.GetEPSession(copID).ConnectionString, currLoginUser, copID, DB);

            isClarifyOk = WorkflowFunctions.Clarify(cbo, screen, appID, wfID, wfRemark, ref refmsg);
            return isClarifyOk;
        }

        public bool ClarifyAll(string appID, string wfID, string wfRemark, string currLoginUser, string copID)
        {
            bool isClarifyAllOk = true;
            ePlatform.BusinessObject.CommonFuncs cbo;
            epScreen screen;
            string refmsg = string.Empty;
            bool bOK = InitEpfBO(out cbo, out screen, DB.GetEPSession(copID).ConnectionString, currLoginUser, copID, DB);

            isClarifyAllOk = WorkflowFunctions.Clarify(cbo, screen, appID, wfID, wfRemark, ref refmsg);
            return isClarifyAllOk;
        }

        public bool ClarifySelect(string appID, string wfID, string recips, string wfRemark, string currLoginUser, string copID)
        {
            bool ClarifySelectOK = true;
            ePlatform.BusinessObject.CommonFuncs cbo;
            epScreen screen;
            string refmsg = string.Empty;
            bool bOK = InitEpfBO(out cbo, out screen, DB.GetEPSession(copID).ConnectionString, currLoginUser, copID, DB);

            ClarifySelectOK = WorkflowFunctions.ClarifySelect(cbo, screen, appID, wfID, recips, wfRemark, ref refmsg);
            return ClarifySelectOK;
        }
        public bool WorkFlowCancel(string appID, string wfID)
        {
            bool isWorkFlowCancelOk = true;
            try
            {
                isWorkFlowCancelOk = WorkflowFunctions.WorkFlowCancel(DB.GetEPSession(""), appID, wfID);
                return isWorkFlowCancelOk;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public bool ChkCurrRecip(string appId, string appLink, string loginEmpeId,string roleID)
        {
            bool isCurrRecip = false;

            string Result = DB.GetAValue("SELECT dbo.QF_CHK_CURRRECIP_ID(%s, %s,%s,%s)",
                appId, appLink, loginEmpeId, roleID).ToStr();
            if (Result == "YES")
            {
                isCurrRecip = true;
            }
            return isCurrRecip;
        }

        private static bool InitEpfBO(out ePlatform.BusinessObject.CommonFuncs cbo, out epScreen screen, string connectionString, string loginUserId, string copID, DBHelper db)
        {
            try
            {

                string empeId = db.GetAValue("SELECT EMPE_ID FROM SV_SC_USER_PROFILE WHERE USER_ID = %s", loginUserId).ToStr();
                
                BOParams bOParams = new BOParams();
                bOParams.Messages = new WebMsgs();

                ASPObject aSPObject = new ASPObject();
                aSPObject.Application = System.Web.HttpContext.Current.Application;
                aSPObject.Application.Add("S_CONNECT", connectionString);
                aSPObject.Application.Add("S_COYID", copID);
                aSPObject.Application.Add("S_DBMS", "MSSQL");
                aSPObject.Application.Add("S_DBOWNER", "dbo");
                aSPObject.Application.Add("S_UNICODE", 1);
                aSPObject.Application.Add("S_LANGID", 0);
                aSPObject.Application.Add("S_CHARSET", "");
                aSPObject.Application.Add("S_CULTUREINFO", "");
                aSPObject.Application.Add("S_DATEFORMAT", "");
                aSPObject.Application.Add("S_DATESEP", "");
                aSPObject.Application.Add("S_TIMEFORMAT", "");
                aSPObject.Application.Add("S_USER", loginUserId);
                aSPObject.Application.Add("S_EMPEID", empeId);

                aSPObject.Session = System.Web.HttpContext.Current.Session;
                aSPObject.Session.Add("S_CONNECT", connectionString);
                aSPObject.Session.Add("S_COYID", copID);
                aSPObject.Session.Add("S_DBMS", "MSSQL");
                aSPObject.Session.Add("S_DBOWNER", "dbo");
                aSPObject.Session.Add("S_UNICODE", 1);
                aSPObject.Session.Add("S_LANGID", 0);
                aSPObject.Session.Add("S_CHARSET", "");
                aSPObject.Session.Add("S_CULTUREINFO", "");
                aSPObject.Session.Add("S_DATEFORMAT", "");
                aSPObject.Session.Add("S_DATESEP", "");
                aSPObject.Session.Add("S_TIMEFORMAT", "");
                aSPObject.Session.Add("S_USER", loginUserId);
                aSPObject.Session.Add("S_EMPEID", empeId);

                aSPObject.Request = System.Web.HttpContext.Current.Request;

                WebState webState = new WebState(aSPObject);
                bOParams.Screen = new WebScrn(webState);
                bOParams.State = new WebState(aSPObject);

                screen = new epScreen(ref bOParams);
                cbo = new ePlatform.BusinessObject.CommonFuncs(ref bOParams);

                return true;
            }
            catch (Exception)
            {
                screen = null;
                cbo = null;
                return false;
            }
        }

        public RecipientCollection GetABSRecpient(string CompCode, string docType, string deptCode, string RunNo, string SrlNo, string LineNo, string CoverSeq, string AppID)
        {
            RecipientCollection recipCollection = new RecipientCollection();

            JArray routeList = DB.GetRows("SELECT * FROM SV_TMS_ABS_REQ_DETL_PLAN_REPLACE_RTING_LIST WHERE COMP_CODE = %s AND DOC_TYPE = %s AND DEPT_CODE = %s AND RUN_NO = %s AND SRL_NO = %s AND LINE_NO = %s AND COVER_SEQ = %s AND APP_ID = %s",
                CompCode, docType, deptCode, RunNo, SrlNo, LineNo, CoverSeq, AppID);

            foreach (JObject obj in routeList)
            {
                Recipient recip = AddABSRecipient(CompCode, obj);
                recipCollection.Add(recip);
            }
            return recipCollection;
        }
        public RecipientCollection GetAttTakingRecpient(string CompCode, string docType, string deptCode, string RunNo, string SrlNo, string AppID,string EmpeID)
        {
            RecipientCollection recipCollection = new RecipientCollection();

            JArray routeList = DB.GetRows("SELECT * FROM SV_TMS_TSH_INOUT_LOG_RTING_LIST WHERE COMP_CODE = %s AND DOC_TYPE = %s AND DEPT_CODE = %s AND RUN_NO = %s AND SRL_NO = %s AND APP_ID = %s AND EMPE_ID = %s",
                CompCode, docType, deptCode, RunNo, SrlNo, AppID, EmpeID);

            foreach (JObject obj in routeList)
            {
                Recipient recip = AddRecipient(CompCode, obj);
                recipCollection.Add(recip);
            }
            return recipCollection;
        }

        public RecipientCollection GetDefRouting(string CompCode, string docType, string deptCode, string RunNo, string AppID, string EmpeID)
        {
            RecipientCollection recipCollection = new RecipientCollection();

            string ownRouteView = DB.GetAValue("SELECT RSRV_FIELD_1 FROM SV_MF_DEFA WHERE COMP_CODE = %s AND FIELD_NAME = %s AND TYPE_CODE = %s", CompCode, "CUS_OWN_ROUT_VIEW", AppID).ToStr();

            if (string.IsNullOrEmpty(ownRouteView)) { ownRouteView = "SV_WF_TRNX_FORM_ROUTE"; }

            JArray routeList = DB.GetRows("SELECT * FROM " + ownRouteView + " WHERE COMP_CODE = %s AND DOC_TYPE = %s AND DEPT_CODE = %s AND RUN_NO = %s AND APP_ID = %s AND EMPE_ID = %s",
                CompCode, docType, deptCode, RunNo, AppID, EmpeID);

            foreach (JObject obj in routeList)
            {
                Recipient recip = AddRecipient(CompCode, obj);
                recipCollection.Add(recip);
            }
            return recipCollection;
        }

        public Recipient AddABSRecipient(string CompCode, JObject obj)
        {
            Recipient recip = new Recipient();
            recip.CompCode = CompCode;
            recip.ID = obj.GetValue("RECIP_ID").ToString();
            recip.Email = obj.GetValue("RECIP_EMAIL").ToString();

            if (obj.GetValue("RECIP_ROLE_ID").ToString() == "RA")
                recip.Role = RecipientRole.Approver;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "C1")
                recip.Role = RecipientRole.Certification;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "RN")
                recip.Role = RecipientRole.Notify;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R1")
                recip.Role = RecipientRole.Recommend;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R2")
                recip.Role = RecipientRole.Recommend2;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R3")
                recip.Role = RecipientRole.Recommend3;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R4")
                recip.Role = RecipientRole.Recommend4;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R5")
                recip.Role = RecipientRole.Recommend5;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R6")
                recip.Role = RecipientRole.Recommend6;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R7")
                recip.Role = RecipientRole.Recommend7;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R8")
                recip.Role = RecipientRole.Recommend8;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R9")
                recip.Role = RecipientRole.Recommend9;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R10")
                recip.Role = RecipientRole.Recommend10;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R11")
                recip.Role = RecipientRole.Recommend11;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R12")
                recip.Role = RecipientRole.Recommend12;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R13")
                recip.Role = RecipientRole.Recommend13;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R14")
                recip.Role = RecipientRole.Recommend14;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R15")
                recip.Role = RecipientRole.Recommend15;

            if (obj.GetValue("WF_TYPE").ToString() == "SERL")
                recip.WorkflowType = WorkflowType.Serial;
            else if (obj.GetValue("WF_TYPE").ToString() == "NOTY")
                recip.WorkflowType = WorkflowType.Notify;

            recip.DueDays = Decimal.ToInt32(obj.GetValue("DUE_DAYS").ToInt());

            recip.Sequence = Decimal.ToInt32(obj.GetValue("ROW_NO").ToInt());

            recip.DueHours = Decimal.ToInt32(obj.GetValue("DUE_HOURS").ToInt());
            return recip;
        }

        public Recipient AddRecipient(string CompCode, JObject obj)
        {
            Recipient recip = new Recipient();
            recip.CompCode = CompCode;
            recip.ID = obj.GetValue("RECIP_ID").ToString();
            recip.Email = obj.GetValue("RECIP_EMAIL").ToString();

            if (obj.GetValue("RECIP_ROLE_ID").ToString() == "RA")
                recip.Role = RecipientRole.Approver;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "C1")
                recip.Role = RecipientRole.Certification;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "RN")
                recip.Role = RecipientRole.Notify;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R1")
                recip.Role = RecipientRole.Recommend;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R2")
                recip.Role = RecipientRole.Recommend2;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R3")
                recip.Role = RecipientRole.Recommend3;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R4")
                recip.Role = RecipientRole.Recommend4;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R5")
                recip.Role = RecipientRole.Recommend5;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R6")
                recip.Role = RecipientRole.Recommend6;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R7")
                recip.Role = RecipientRole.Recommend7;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R8")
                recip.Role = RecipientRole.Recommend8;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R9")
                recip.Role = RecipientRole.Recommend9;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R10")
                recip.Role = RecipientRole.Recommend10;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R11")
                recip.Role = RecipientRole.Recommend11;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R12")
                recip.Role = RecipientRole.Recommend12;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R13")
                recip.Role = RecipientRole.Recommend13;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R14")
                recip.Role = RecipientRole.Recommend14;
            else if (obj.GetValue("RECIP_ROLE_ID").ToString() == "R15")
                recip.Role = RecipientRole.Recommend15;

            if (obj.GetValue("WF_TYPE").ToString() == "SERL")
                recip.WorkflowType = WorkflowType.Serial;
            else if (obj.GetValue("WF_TYPE").ToString() == "NOTY")
                recip.WorkflowType = WorkflowType.Notify;

            recip.DueDays = Decimal.ToInt32(obj.GetValue("DUE_DAYS").ToInt());

            recip.Sequence = Decimal.ToInt32(obj.GetValue("ROW_NO").ToInt());
            return recip;
        }

        public bool IsClarifyActionEnabled(string appID, string wfID, string copID, string currEmpeId) {
            return WorkflowFunctions.IsClarifyActionEnabled(DB.GetEPSession(copID), appID, wfID, currEmpeId);
        }

        public bool IsClarifyAllActionEnabled(string appID, string wfID, string copID, string currEmpeId) {
            return WorkflowFunctions.IsClarifyAllActionEnabled(DB.GetEPSession(copID), appID, wfID, currEmpeId);
        }

        public bool IsClarifyToSelectionEnabled(string appID, string wfID, string copID, string currEmpeId) {
            return WorkflowFunctions.IsClarifyToSelectionEnabled(DB.GetEPSession(copID), appID, wfID, currEmpeId);
        }

        public bool IsClarifySelectActionEnabled(string appID, string wfID, string copID, string currEmpeId) {
            return WorkflowFunctions.IsClarifySelectActionEnabled(DB.GetEPSession(copID), appID, wfID, currEmpeId);
        }
    }
}
