using MecWise.Blazor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MecWise.Blazor.Common;
using System.ComponentModel;
using System.Runtime.InteropServices.ComTypes;

namespace MecWise.Blazor.Workflow {
    public class WorkflowClient 
    {
        SessionState _session;
        const string _serverSideAssemblyName = "MecWise.Blazor.Api.WorkflowServer";
        const string _serverSideClassName = "CommonFunctions";

        string _docType = "";
        string _deptCode = "";
        string _appId = "";

        public WorkflowClient(SessionState session, string appId, string docType, string deptCode) {
            _session = session;
            _appId = appId;
            _docType = docType;
            _deptCode = deptCode;
        }

        public async Task<bool> PrepareRoutingListAsync(string cmd,  string runNo, string empeId, string orgUnit)
        {
            Console.WriteLine("WF Function - PrepareRoutingList");
            return Convert.ToBoolean(await _session.ExecServerFuncAsync(_serverSideAssemblyName, _serverSideClassName, 
                "PrepareRouting", cmd, _session.CompCode, _docType, _deptCode, runNo, _appId, empeId, orgUnit));
        }

        public async Task<bool> StartRoutAsync(string runNo, string empeId, string formtext)
        {
            Console.WriteLine("WF Function - StartRout");
            return Convert.ToBoolean(await _session.ExecServerFuncAsync(_serverSideAssemblyName, _serverSideClassName,
                "StartRout", _session.CompCode, _docType, _deptCode, runNo, _appId, empeId, formtext, _session.UserID));
        }

        public async Task<bool> EpfStartRoutAsync(string objDataSource, string empeId, string formtext)
        {
            return Convert.ToBoolean(await _session.ExecServerFuncAsync(_serverSideAssemblyName, _serverSideClassName,
                "StartRoutWithEpfObj", objDataSource, empeId, formtext, _session.UserID));
        }

        public async Task<bool> StartWorkflowAsync(string appLink)
        {
            Console.WriteLine("WF Function - StartRout");
            string wfID = await GetWFIDWithAppLinkAsync(appLink);
            return Convert.ToBoolean(await _session.ExecServerFuncAsync(_serverSideAssemblyName, _serverSideClassName,
                "StartWorkflow", _appId, wfID, _session.UserID, _session.CompCode));
        }

        public async Task<string> GetWFIDAsync(string runNo)
        {
            return Convert.ToString(await _session.ExecServerFuncAsync(_serverSideAssemblyName, _serverSideClassName, 
                "GetWFID", _session.CompCode, _docType, _deptCode, runNo));
        }

        public async Task<string> GetWFIDWithAppLinkAsync(string appLink)
        {
            Console.WriteLine(appLink);
            return Convert.ToString(await _session.ExecServerFuncAsync(_serverSideAssemblyName, _serverSideClassName,
                "GetWFIDWithAppLink", _session.CompCode, appLink));
        }

        public async Task<bool> ApproveAutoAsync(string appLink, bool autoApprove, string wfRemark)
        {
            Console.WriteLine("WF Function - ApproveAuto");
            string wfID = await GetWFIDWithAppLinkAsync(appLink);
            return Convert.ToBoolean(await _session.ExecServerFuncAsync(_serverSideAssemblyName, _serverSideClassName, 
                "ApproveAuto", _appId, wfID, autoApprove, wfRemark, _session.UserID, _session.CompCode));
        }

        public async Task<bool> ApproveAsync(string appLink, string wfRemark)
        {
            Console.WriteLine("WF Function - Approve");
            string wfID = await GetWFIDWithAppLinkAsync(appLink);
            return Convert.ToBoolean(await _session.ExecServerFuncAsync(_serverSideAssemblyName, _serverSideClassName, 
                "Approve", _appId, wfID, wfRemark, _session.UserID, _session.CompCode));
        }

        public async Task<bool> ApproveWithWFIDAsync(string wfID, string wfRemark) {
            Console.WriteLine("WF Function - ApproveWithWFID");
            return Convert.ToBoolean(await _session.ExecServerFuncAsync(_serverSideAssemblyName, _serverSideClassName,
                "Approve", _appId, wfID, wfRemark, _session.UserID, _session.CompCode));
        }

        public async Task<bool> DenyAsync(string appLink, string wfRemark)
        {
            Console.WriteLine("WF Function - Deny");
            string wfID = await GetWFIDWithAppLinkAsync(appLink);
            return Convert.ToBoolean(await _session.ExecServerFuncAsync(_serverSideAssemblyName, _serverSideClassName, 
                "Deny", _appId, wfID, wfRemark, _session.UserID, _session.CompCode));
        }

        public async Task<bool> ClarifyAsync(string appLink, string wfRemark)
        {
            Console.WriteLine("WF Function - Clarify");
            string wfID = await GetWFIDWithAppLinkAsync(appLink);
            return Convert.ToBoolean(await _session.ExecServerFuncAsync(_serverSideAssemblyName, _serverSideClassName,
                "Clarify", _appId, wfID, wfRemark, _session.UserID, _session.CompCode));
        }

        public async Task<bool> ClarifyAllAsync(string appLink, string wfRemark)
        {
            Console.WriteLine("WF Function - Clarify");
            string wfID = await GetWFIDWithAppLinkAsync(appLink);
            return Convert.ToBoolean(await _session.ExecServerFuncAsync(_serverSideAssemblyName, _serverSideClassName,
                "ClarifyAll", _appId, wfID, wfRemark, _session.UserID, _session.CompCode));
        }

        public async Task<bool> WorkFlowCancelAsync(string appLink)
        {
            Console.WriteLine("WF Function - WorkFlowCancel");
            string wfID = await GetWFIDWithAppLinkAsync(appLink);
            return Convert.ToBoolean(await _session.ExecServerFuncAsync(_serverSideAssemblyName, _serverSideClassName,
                "WorkFlowCancel", _appId, wfID));
        }

        public async Task<bool> ChkCurrRecipAsyn(string appLink, string roleID)
        {
            Console.WriteLine("WF Function - ChkCurrRecip");
            //string appLink = _session.CompCode + ":" + _docType + ":" + _deptCode + ":" + runNo;
            return Convert.ToBoolean(await _session.ExecServerFuncAsync(_serverSideAssemblyName, _serverSideClassName,
                "ChkCurrRecip", _appId, appLink, _session.EmpeID, roleID));
        }

        public async Task<bool> ClarifySelectAsync(string appLink, string recips, string wfRemark)
        {
            Console.WriteLine("WF Function - Clarify Select");
            string wfID = await GetWFIDWithAppLinkAsync(appLink);
            return Convert.ToBoolean(await _session.ExecServerFuncAsync(_serverSideAssemblyName, _serverSideClassName,
                "ClarifySelect", _appId, wfID, recips, wfRemark, _session.UserID, _session.CompCode));
        }

        public async Task<bool> IsClarifyActionEnabled(string wfID) {
            var result = await _session.ExecServerFuncAsync(_serverSideAssemblyName, _serverSideClassName,
                "IsClarifyActionEnabled", _appId, wfID, _session.CompCode, _session.EmpeID);
            return result.ToBool();
        }

        public async Task<bool> IsClarifyAllActionEnabled(string wfID) {
            var result = await _session.ExecServerFuncAsync(_serverSideAssemblyName, _serverSideClassName,
                "IsClarifyAllActionEnabled", _appId, wfID, _session.CompCode, _session.EmpeID);
            return result.ToBool();
        }

        public async Task<bool> IsClarifyToSelectionEnabled(string wfID) {
            var result = await _session.ExecServerFuncAsync(_serverSideAssemblyName, _serverSideClassName,
                "IsClarifyToSelectionEnabled", _appId, wfID, _session.CompCode, _session.EmpeID);
            return result.ToBool();
        }

        public async Task<bool> IsClarifySelectActionEnabled(string wfID) {
            var result = await _session.ExecServerFuncAsync(_serverSideAssemblyName, _serverSideClassName,
                "IsClarifySelectActionEnabled", _appId, wfID, _session.CompCode, _session.EmpeID);
            return result.ToBool();
        }

        public async Task<bool> OTStartWorkflowAsync(string wfID)
        {
            Console.WriteLine("WF Function - OTStartWorkflowAsync");
            return Convert.ToBoolean(await _session.ExecServerFuncAsync(_serverSideAssemblyName, _serverSideClassName,
                "StartWorkflow", _appId, wfID, _session.UserID, _session.CompCode));
        }

        public async Task<bool> OTApproveAutoAsync(string wfID, bool autoApprove, string wfRemark)
        {
            Console.WriteLine("WF Function - OTApproveAutoAsync");
            return Convert.ToBoolean(await _session.ExecServerFuncAsync(_serverSideAssemblyName, _serverSideClassName,
                "ApproveAuto", _appId, wfID, autoApprove, wfRemark, _session.UserID, _session.CompCode));
        }

        public async Task<bool> OTDenyAsync(string wfID, string wfRemark)
        {
            Console.WriteLine("WF Function - OTDenyAsync");
            return Convert.ToBoolean(await _session.ExecServerFuncAsync(_serverSideAssemblyName, _serverSideClassName,
                "Deny", _appId, wfID, wfRemark, _session.UserID, _session.CompCode));
        }

        public async Task<bool> OTCancel(string wfID)
        {
            Console.WriteLine("WF Function - OTCancel");

            return Convert.ToBoolean(await _session.ExecServerFuncAsync(_serverSideAssemblyName, _serverSideClassName,
                "WorkFlowCancel", _appId, wfID));
        }

         
    }
}
