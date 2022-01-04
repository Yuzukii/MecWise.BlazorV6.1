using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using ePlatform.CommonClasses;
using Newtonsoft.Json.Linq;
using System.Data.Common;
using System.Runtime.InteropServices.WindowsRuntime;
using MecWise.Blazor.Common;
using MecWise.Blazor.Entities;

namespace MecWise.Blazor.Api.DataAccess
{
    public class DBHelper
    {
        private EPSession _session;

        public EPSession EPSession { get { return _session; } }

        public string LogonUser { get {
                if (_session != null) {
                    return _session.LogonUser;
                }
                return ""; 
            } 
        }

        public string CoyID {
            get {
                if (_session != null) {
                    return _session.CoyID;
                }
                return "";
            }
        }

        public string EmployeeID {
            get {
                if (_session != null) {
                    return _session.EmployeeID;
                }
                return "";
            }
        }

        public int DefaultLanguage {
            get {
                if (_session != null) {
                    return _session.DefaultLanguage;
                }
                return 0;
            }
        }

        public string ConnectionString {
            get {
                if (_session != null) {
                    return _session.ConnectionString;
                }
                return "";
            }
        }

        public DBHelper(string connectionString) {
            _session = new EPSession("MSSQL", "dbo", connectionString);
        }

        public DBHelper(string userId, string compCode, int langId, string connectionString) {
            _session = new EPSession("MSSQL", "dbo", connectionString);
            _session.LogonUser = userId;
            //_session.EmployeeID = "";
            _session.CoyID = compCode;
            _session.DefaultLanguage = langId;
        }

        public EPSession GetEPSession(string compCode) {
            return GetEPSession(compCode, string.Empty);
        }

        public string SqlExpr(string sqlTemplate, params object[] parameterArray) {
            return _session.SqlExpr(sqlTemplate, parameterArray);
        }

        public EPSession GetEPSession(string compCode, string logonUserId)
        {
            EPSession epSession = new EPSession(_session.DBMS, _session.DbOwner, _session.ConnectionString);
            if (compCode != string.Empty) {
                epSession.CoyID = compCode;
            }
            
            if (logonUserId != string.Empty) {
                epSession.LogonUser = logonUserId;
            }

            return epSession;
        }

        public Dictionary<string, JObject> GetParamInfos(string procedureName)
        {
            Dictionary<string, JObject> paramInfos = new Dictionary<string, JObject>();
            JArray rows = GetRows("sp_procedure_params_100_managed %s", procedureName);
            foreach (JToken row in rows) {
                string key = row["PARAMETER_NAME"].ToStr().TrimStart('@');
                JObject val = JObject.FromObject(row);
                paramInfos.Add(key, val);
            }

            return paramInfos;
        }

        public EPParameterCollection GetEPParameterCollection(string procedureName) {
            EPRecordSet proc = new EPRecordSet(_session, "", procedureName);
            return proc.Params;
        }

        public EPRecordSet GetEPRecordSet(string viewName, string procedureName) {
            EPRecordSet proc = new EPRecordSet(_session, viewName, procedureName);
            return proc;
        }

        public Dictionary<string, EPF_BRWS_COL> GetColumnInfos(string viewName)
        {
            Dictionary<string, EPF_BRWS_COL> dataColumnInfos = new Dictionary<string, EPF_BRWS_COL>();

            if (string.IsNullOrEmpty(viewName))
            {
                return dataColumnInfos;
            }

            string sql = viewName;
            if (!sql.Contains(" "))
            {
                sql = string.Format("SELECT * FROM {0}", sql);
            }
            sql = sql.Replace("'", "''");
            sql = string.Format("DECLARE @query nvarchar(max) = '{0}'; EXEC sp_describe_first_result_set @query, null, 0;", sql);

            IEnumerable<dynamic> result = ExecQuery<dynamic>(sql);
            result = result.Select(x => new { x.column_ordinal, x.name, x.system_type_id });
            foreach (var row in result)
            {
                string name = row.name;
                EPF_BRWS_COL brwsCol = new EPF_BRWS_COL();
                
                brwsCol.name = name;
                brwsCol.column_ordinal = (int)row.column_ordinal;
                brwsCol.system_type_id = (int)row.system_type_id;

                if (brwsCol.system_type_id.ToString().In("40"))
                {
                    brwsCol.col_type = "date";
                }
                else if (brwsCol.system_type_id.ToString().In("41", "42", "43", "58", "61"))
                {
                    brwsCol.col_type = "datetime";
                }
                else if (brwsCol.system_type_id.ToString().In("48", "52", "56", "59", "60", "62", "106", "108", "122", "127"))
                {
                    brwsCol.col_type = "number";
                }
                else if (brwsCol.system_type_id.ToString().In("104")) {
                    brwsCol.col_type = "boolean";
                }
                else if (brwsCol.system_type_id.ToString().In("35", "36", "99", "167", "175", "231", "239"))
                {
                    brwsCol.col_type = "string";
                }


                if (!dataColumnInfos.ContainsKey(name))
                {
                    dataColumnInfos.Add(name, brwsCol);
                }
            }
            return dataColumnInfos;
        }

        public object ExecProcedure(string procName, DBParameterCollection parameters) {
            JObject retObj = new JObject();

            EPRecordSet proc = new EPRecordSet(_session, "", procName);
            foreach (IDataParameter param in parameters) {
                if (param.Value == null) {
                    param.Value = DBNull.Value;
                }
                else if (string.IsNullOrEmpty(param.Value.ToString())) {
                    param.Value = DBNull.Value;
                }
                else {
                    if (proc.Params[param.ParameterName].DbType == DbType.DateTime || proc.Params[param.ParameterName].DbType == DbType.Date) {
                        //param.Value = Convert.ToDateTime(param.Value);
                        param.Value = param.Value.ToDate();
                    }
                }

                proc.Params[param.ParameterName].Value = param.Value;
                proc.Params[param.ParameterName].Direction = param.Direction;

                if (param.Direction == ParameterDirection.InputOutput || param.Direction == ParameterDirection.Output)
                    retObj.Add(param.ParameterName, null);
            }
            proc.Execute();

            foreach (var item in retObj) {
                retObj[item.Key] = JToken.FromObject(proc.Params[item.Key].Value);
                parameters[item.Key].Value = proc.Params[item.Key].Value;
            }

            return retObj;
        }

        public IEnumerable<T> ExecQuery<T>(string sqlTemplate, params object[] paramArray) {
            string sql = _session.SqlExpr(sqlTemplate, paramArray);
            using (var connection = new SqlConnection(_session.ConnectionString)) {
                var result = connection.Query<T>(sql).ToList();
                if (result != null) {
                    return result;
                }
                else {
                    return new List<T>();
                }

            }

        }

        public int ExecProcedureDapper(string procedureName, DynamicParameters param) {
            int affectedRows;
            using (var connection = new SqlConnection(_session.ConnectionString)) {
                affectedRows = connection.Execute(procedureName, param, commandType: CommandType.StoredProcedure);

            }

            return affectedRows;
        }
        
        public IEnumerable<T> ExecQueryDapper<T>(string sql, object param = null) {
            using (var connection = new SqlConnection(_session.ConnectionString)) {
                var result = connection.Query<T>(sql, param).ToList();
                if (result != null) {
                    return result;
                }
                else {
                    return new List<T>();
                }

            }

        }

        public JArray GetRows(string sqlTemplate, params object[] parameters) {
            IEnumerable<dynamic> result = ExecQuery<dynamic>(sqlTemplate, parameters);
            return JArray.FromObject(result);
        }

        public JObject GetARow(string sqlTemplate, params object[] parameters) {
            JArray result = GetRows(sqlTemplate, parameters);
            if (result.Count > 0) {
                return JObject.FromObject(result.First());
            }
            else {
                return null;
            }
        }

        public object GetAValue(string sqlTemplate, params object[] parameters) {
            JObject result = GetARow(sqlTemplate, parameters);
            if (result != null) {
                return result[result.Properties().Select(p => p.Name).First()];
            }
            else {
                return null;
            }
        }
    }
}
