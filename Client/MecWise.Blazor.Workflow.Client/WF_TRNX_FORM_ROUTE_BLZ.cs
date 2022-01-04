using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using MecWise.Blazor.Components;
using MecWise.Blazor.Common;


namespace MecWise.Blazor.Workflow.Client
{
    class WF_TRNX_FORM_ROUTE_BLZ : Screen
    {
        #region "Screen Events"
        protected override async Task<object> LoadAsync()
        {
            Console.WriteLine("WF_TRNX_FORM_ROUTE_BLZ - LoadAsync");

            if (string.IsNullOrEmpty(GetQueryParamValue("RUN_NO"))) {
                string appId = GetQueryParamValue("APP_ID");
                string docType = GetQueryParamValue("DOC_TYPE");
                string deptCode = GetQueryParamValue("DEPT_CODE");
                string rowNo = GetQueryParamValue("ROW_NO");
                string srlNo = GetQueryParamValue("SRL_NO");

                string runNo = GetParentFieldValue<string>("RUN_NO");
                string empeId = GetParentFieldValue<string>("EMPE_ID");

                //string orgUnit = ""; //GetParentFieldValue("ORG_UNIT").ToString();
                //WorkflowClient wfClient = new WorkflowClient(Session, appId, docType, deptCode);
                //await wfClient.PrepareRoutingListAsync("U", runNo, empeId, orgUnit);

                string param = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                    Session.CompCode, docType, deptCode, runNo, appId, empeId, rowNo, srlNo);

                NavigateScreen("WF_TRNX_FORM_ROUTE_BLZ", ScrnMode,
                    "COMP_CODE,DOC_TYPE,DEPT_CODE,RUN_NO,APP_ID,EMPE_ID,ROW_NO,SRL_NO",
                    "COMP_CODE,DOC_TYPE,DEPT_CODE,RUN_NO,APP_ID,EMPE_ID", param, false);

            }

            return await base.LoadAsync();
        }

        protected override async Task<object> ShowRecAsync()
        {
            Console.WriteLine("WF_TRNX_FORM_ROUTE_BLZ - ShowRecAsync");
            
            if (ScrnMode == ScreenMode.Add)
            {
                await InitFieldsAsync();
            }
            return await Task.FromResult<object>(null);
        }

        protected override async Task<object> SavedRecAsync()
        {
            Console.WriteLine("WF_TRNX_FORM_ROUTE_BLZ - SavedRecAsync");
            return await Task.FromResult<object>(null);
        }

        protected override async Task<bool> ValidEntryAsync()
        {
            Console.WriteLine("WF_TRNX_FORM_ROUTE_BLZ - ValidEntryAsync");
            bool isValidCheckOk = true;
            if (GetFieldValue<string>("RECIP_ROLE_ID") == "RA")
            {
                string isMultiRoleAllw = await isMultiRecipAllw();
                if (isMultiRoleAllw == "N")
                {
                    string isRARoleExisted = await isRAExisted();
                    if (isRARoleExisted == "RA")
                    {
                        isValidCheckOk = false;
                        Session.ShowMessage("System cannot set more than one approval.");
                    }
                }
            }

            if (ScrnMode == ScreenMode.Add && isValidCheckOk)
            {
                int maxRowNo = await GetMaxRowNo() + 10;
                Console.WriteLine(maxRowNo.ToString());

                SetFieldValue("RSRV_CHAR_1", "EDITED");
                SetFieldValue("ROW_NO", maxRowNo.ToString());
                SetFieldValue("USER_LOG", Session.UserID);
                SetFieldValue("TIME_LOG", DateTime.Now);
            }
            return await Task.FromResult<bool>(isValidCheckOk);
        }

        private async Task<object> InitFieldsAsync()
        {
            Console.WriteLine("WF_TRNX_FORM_ROUTE_BLZ - InitFieldsAsync");

            return await Task.FromResult<object>(null);
        }
        #endregion

        #region "Helper Functions"
        private async Task<int> GetMaxRowNo()
        {
            return Convert.ToInt32(await Session.ExecServerFuncAsync("MecWise.Blazor.Api.WorkflowServer", "WF_TRNX_FORM_ROUTE_BLZ", "getMaxRowNo",
                Session.CompCode, GetFieldValue("DOC_TYPE"), GetFieldValue("DEPT_CODE"), GetFieldValue("RUN_NO"), GetFieldValue("APP_ID"), GetFieldValue("EMPE_ID")));
        }

        private async Task<string> isMultiRecipAllw()
        {
            return Convert.ToString(await Session.ExecServerFuncAsync("MecWise.Blazor.Api.WorkflowServer", "WF_TRNX_FORM_ROUTE_BLZ", "isMultiRecipAllw",
                 GetFieldValue("APP_ID"), GetFieldValue("RECIP_ROLE_ID")));
        }

        private async Task<string> isRAExisted()
        {
            return Convert.ToString(await Session.ExecServerFuncAsync("MecWise.Blazor.Api.WorkflowServer", "WF_TRNX_FORM_ROUTE_BLZ", "isRAExisted",
                Session.CompCode, GetFieldValue("DOC_TYPE"), GetFieldValue("DEPT_CODE"), GetFieldValue("RUN_NO"), GetFieldValue("APP_ID"), GetFieldValue("EMPE_ID"), GetFieldValue("RECIP_ROLE_ID")));
        }
        #endregion
    }
}
