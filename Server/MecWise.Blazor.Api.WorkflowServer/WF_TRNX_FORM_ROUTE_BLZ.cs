using MecWise.Blazor.Api.Services;
using MecWise.Blazor.Common;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MecWise.Blazor.Api.WorkflowServer
{
    class WF_TRNX_FORM_ROUTE_BLZ : ScreenService
    {
        public int getMaxRowNo(string compCode, string docType, string deptCode, string runNo, string appId, string empeId)
        {
            int MaxRowNo = DB.GetAValue("SELECT IsNULL(Max(ROW_NO), 0) ROW_NO " +
                "FROM SV_WF_TRNX_FORM_ROUTE " +
                "WHERE COMP_CODE = %s AND DOC_TYPE = %s AND DEPT_CODE = %s AND RUN_NO = %s AND APP_ID = %s AND EMPE_ID = %s",
                compCode, docType, deptCode, runNo, appId, empeId).ToInt();

            return MaxRowNo;
        }

        public string isMultiRecipAllw(string appId, string recipRoleId)
        {
            string result = DB.GetAValue("SELECT ISNULL(ROLE_MULTIPLE,'') ROLE_MULTIPLE " +
                "FROM SV_WF_MF_PROFILE_ROLE " +
                "WHERE APP_ID = %s AND ROLE_ID = %s",
                appId, recipRoleId).ToString();

            return result;
        }

        public string isRAExisted(string compCode, string docType, string deptCode, string runNo, string appId, string empeId, string recipRoleID)
        {
            string result = DB.GetAValue("SELECT RECIP_ROLE_ID " +
                "FROM SV_WF_TRNX_FORM_ROUTE " +
                "WHERE COMP_CODE = %s AND DOC_TYPE = %s AND DEPT_CODE = %s AND RUN_NO = %s AND APP_ID = %s AND EMPE_ID = %s AND RECIP_ROLE_ID = %s",
                compCode, docType, deptCode, runNo, appId, empeId, recipRoleID).ToString();

            return result;
        }
    }
}
