using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MecWise.Blazor.Entities;
using MecWise.Blazor.Api.DataAccess;
using MecWise.Blazor.Api.Repositories;
using Newtonsoft.Json.Linq;
using MecWise.Blazor.Common;
using System.Data;

namespace MecWise.Blazor.Api.Services
{
    public class EpfScreenService
    {
        DBHelper _db;
        EPF_SCRN_Repository _scrnRepo;
        
        public EpfScreenService() {
            _db = new DBHelper(ApiSetting.ConnectionString);
            _scrnRepo = new EPF_SCRN_Repository(_db);
        }

        public EpfScreenService(SessionState session) { 
            _db = new DBHelper(session.UserID, session.CompCode, session.LangId, ApiSetting.ConnectionString);
            _scrnRepo = new EPF_SCRN_Repository(_db);
        }

        public List<EPF_SCRN> GetScreen(string scrnId, bool includeFields = true) {
            return _scrnRepo.GetScreen(scrnId, includeFields);
        }

        public string GetScreenModuleId(string scrnId) {
            return _scrnRepo.GetScreenModuleId(scrnId);
        }

        public string GetRights(string scrnId) {
            return _scrnRepo.GetRights(scrnId);
        }

        public JObject GetBrws(string brwsId, JObject dataSource, string baseView)
        {
            return _scrnRepo.GetBrws(brwsId, dataSource, baseView);
        }

        public object GetCodeDesc(string sqlDesc, JObject dataSource, Dictionary<string, EPF_BRWS_COL> dataColInfos) {
            return _scrnRepo.GetCodeDesc(sqlDesc, dataSource, dataColInfos);
        }

        public Dictionary<string, EPF_BRWS_COL> GetColumnInfos(string viewName) {
            return _scrnRepo.GetColumnInfos(viewName);
        }

        public JObject GetBrwsData(string brwsId, JObject dataSource, string baseView, string NavigateScrnId) {

            JObject result = _scrnRepo.GetBrwsData(brwsId, dataSource, baseView);
            if (!string.IsNullOrEmpty(NavigateScrnId)) {
                var scrnResult = this.GetScreen(NavigateScrnId);
                if (scrnResult.Count > 0) {
                    result.Add("navigateScrn", JObject.FromObject(scrnResult.First()));
                }
            }

            return result;
        }

        public JObject GetBrwsData(string brwsId, JObject dataSource, string baseView, JObject pagingInfo) {
            return _scrnRepo.GetBrwsData(brwsId, dataSource, baseView, pagingInfo);
        }

        public JObject GetQueryData(string brwsSql, JObject dataSource, string baseView) {
            return _scrnRepo.GetQueryData(brwsSql, dataSource, baseView);
        }

        public JArray FetchData(string viewName, string fieldKeys, Dictionary<string, JObject> dataColInfos, Dictionary<string, object> param) {
            return _scrnRepo.FetchData(viewName, fieldKeys, dataColInfos, param);
        }

        public JObject GetNavigateRecordKeys(string brwsId, JObject dataSource, string baseView, string fieldKeys, int offsetFromCurrent) {
            return _scrnRepo.GetNavigateRecordKeys(brwsId, dataSource, baseView, fieldKeys, offsetFromCurrent);
        }

        public void Action(string baseProcName, string cmd, JObject postData) {
            _scrnRepo.Action(baseProcName, cmd, postData);
        }

        public object ExecFunction(string assemblyName, string className, string methodName, params object[] parameters) {

            try {
                //load assembly
                var assembly = System.Reflection.Assembly.Load(assemblyName);

                // get type/method info
                var type = assembly.GetType(string.Format("{0}.{1}", assemblyName, className));
                var method = type.GetMethod(methodName);

                // instantiate object and run method
                ScreenService service = (ScreenService)Activator.CreateInstance(type, null);
                service.DB = _db;
                var parameterInfos = method.GetParameters();
                for (int i = 0; i <= parameterInfos.Length - 1; i++)
                {
                    if (parameterInfos[i].ParameterType.BaseType.Name == "Enum")
                    {
                        parameters[i] = Enum.ToObject(parameterInfos[i].ParameterType, parameters[i]);
                    }
                    else {
                        parameters[i] = Convert.ChangeType(parameters[i], parameterInfos[i].ParameterType);
                    }
                    
                }
                object result = method.Invoke(service, parameters);
                return result;
            }
            catch (Exception ex) {
                if (ex is System.Reflection.TargetInvocationException) {
                    throw ex.InnerException;
                }
                else {
                    throw ex;
                }
            }
            
        }

        public void ExecMultiPickSelection(JObject postData) {
            _scrnRepo.ExecMultiPickSelection(postData);
        }

        

    }
}
