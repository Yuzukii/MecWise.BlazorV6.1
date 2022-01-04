using System;
using System.Collections.Generic;
using System.Linq;
using MecWise.Blazor.Entities;
using MecWise.Blazor.Api.DataAccess;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using MecWise.Blazor.Common;
using ePlatform.CommonClasses;

namespace MecWise.Blazor.Api.Repositories
{
    public class EPF_SCRN_Repository : GenericRepository<EPF_SCRN>
    {
        DBHelper _db;

        public EPF_SCRN_Repository(DBHelper db) : base(db)
        {
            _db = db;
        }

        public string GetRights(string scrnId) {
            JArray rows = _db.GetRows("exec %<:DBOWNER>UP_SC_CHK_MDULE_ACCESS @USER_ID=%s,@OBJ_ID=%s", _db.LogonUser, scrnId);
            if (rows.Count > 0) {
                return rows[0]["RIGHTS"].ToStr();
            }
            return string.Empty;
        }

        public List<EPF_SCRN> GetScreen(string scrnId, bool includeFields = true) {
            List<EPF_SCRN> result = this.GetByCriteria(new { SCRN_ID = scrnId }).ToList();
            if (includeFields) {
                GenericRepository<EPF_SCRN_FIELDS> _scrnFieldRepo = new GenericRepository<EPF_SCRN_FIELDS>(_db);
                GenericRepository<EPF_SCRN_EVENTS> _scrnEventRepo = new GenericRepository<EPF_SCRN_EVENTS>(_db);
                GenericRepository<EPF_BRWS> _scrnBrwsRepo = new GenericRepository<EPF_BRWS>(_db);
                foreach (EPF_SCRN screen in result) {
                    screen.FIELDS = _scrnFieldRepo.GetByCriteria(new { SCRN_ID = scrnId }).ToList();
                    screen.EVENTS = _scrnEventRepo.GetByCriteria(new { SCRN_ID = scrnId }).ToList();

                    screen.BRWS = new List<EPF_BRWS>();
                    foreach (EPF_SCRN_FIELDS field in screen.FIELDS.FindAll(x => !string.IsNullOrEmpty(x.BRWS_ID))) {
                        foreach (EPF_BRWS brws in _scrnBrwsRepo.GetByCriteria(new { BRWS_ID = field.BRWS_ID })) {
                            Dictionary<string, EPF_BRWS_COL> baseViewColInfos = GetColumnInfos(screen.BASE_VIEW);
                            string sql = ParseBrwsSql(new Dictionary<string, object>(), baseViewColInfos, brws.BRWS_SQL, true);
                            brws.BRWS_COLUMNS = GetColumnInfos(sql);
                            screen.BRWS.Add(brws);
                        }
                        
                    }

                }

            }
            return result;
        }

        public JObject GetBrws(string brwsId, JObject dataSource, string baseView) {
            JObject result = new JObject();

            GenericRepository<EPF_BRWS> _brwsRepo = new GenericRepository<EPF_BRWS>(_db);
            EPF_BRWS brws = _brwsRepo.GetByCriteria(new { BRWS_ID = brwsId }).ToList().First();

            Dictionary<string, EPF_BRWS_COL> dataColInfos = GetColumnInfos(baseView);

            if (brws != null) {
                result.Add("brws", JToken.FromObject(brws));
                Dictionary<string, object> data = new Dictionary<string, object>(dataSource.ToObject<IDictionary<string, object>>(), StringComparer.CurrentCultureIgnoreCase);
                string sql = ParseBrwsSql(data, dataColInfos, brws.BRWS_SQL, true);
                JObject colInfos = JObject.FromObject(GetColumnInfos(sql));
                result.Add("cols", colInfos);
            }
            return result;
        }

        public object GetCodeDesc(string sqlDesc, JObject dataSource, Dictionary<string, EPF_BRWS_COL> dataColInfos) {
            object result = "";
            Dictionary<string, object> data = new Dictionary<string, object>(dataSource.ToObject<IDictionary<string, object>>(), StringComparer.CurrentCultureIgnoreCase);
            string sql = ParseBrwsSql(data, dataColInfos, sqlDesc, false);
            
            IEnumerable<dynamic> brwsData = _db.ExecQuery<dynamic>(sql);
            if (brwsData.Count() > 0) {
                JObject row = JObject.FromObject(brwsData.First());
                result = row[row.Properties().Select(p => p.Name).First()];
            }
            return result;
        }

        public JArray FetchData(string viewName, string fieldKeys, Dictionary<string, JObject> dataColInfos, Dictionary<string, object> param) {
            string sql = string.Format("SELECT * FROM {0}", viewName);
            string[] keys = fieldKeys.Split(',');

            if (keys.Length > 0) {
                sql = string.Format("{0} WHERE ", sql);
                for (int i = 0; i <= keys.Length - 1; i++) {
                    if (i != 0) {
                        sql = string.Format("{0} AND ", sql);
                    }

                    if (dataColInfos.ContainsKey(keys[i])) {

                        if (!param.ContainsKey(keys[i])) {
                            sql = string.Format("{0} {1} IS NULL", sql, keys[i]);
                        }
                        else {
                            if (string.IsNullOrEmpty(param[keys[i]].ToStr())) {
                                sql = string.Format("{0} {1} IS NULL", sql, keys[i]);
                            }
                            else {
                                if (dataColInfos[keys[i]]["col_type"].ToStr().In("date", "datetime")) {
                                    DateTime val = param[keys[i]].ToDate();
                                    sql = string.Format("{0} {1} = '{2}'", sql, keys[i], val.ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                                else if (dataColInfos[keys[i]]["col_type"].ToStr() == "number") {
                                    sql = string.Format("{0} {1} = {2}", sql, keys[i], param[keys[i]]);
                                }
                                else {
                                    sql = string.Format("{0} {1} = '{2}'", sql, keys[i], param[keys[i]]);
                                }
                            }
                                
                        }

                    }



                }
            }

            IEnumerable<dynamic> rows = _db.ExecQuery<dynamic>(sql);
            if (rows.Count() > 0) {
                return JArray.FromObject(rows);
            }

            return new JArray();
        }

        public JObject GetNavigateRecordKeys(string brwsId, JObject dataSource, string baseView, string fieldKeys, int offsetFromCurrent) {

            // get brwsData based on screen's brws id
            JObject brwsData =  this.GetBrwsData(brwsId, dataSource, baseView);

            if (brwsData.ContainsKey("rows")) {
                
                // get current row index base on screen key fields
                int currIndex = brwsData["rows"].ToList().FindIndex(row => this.IsRecExists(JObject.FromObject(row), fieldKeys, dataSource));

                if (currIndex > -1) {

                    int naviIndex = currIndex + offsetFromCurrent;

                    if (naviIndex < 0 || naviIndex > brwsData["rows"].Count() - 1) {
                        return null;
                    }

                    JToken naviRecKeys = brwsData["rows"].ToList()[naviIndex];
                    return JObject.FromObject(naviRecKeys);

                }
            }
            
            return null;
        }

        private bool IsRecExists(JObject row, string fieldKeys, JObject dataSource) {
            string[] keys = fieldKeys.Split(',');
            foreach (string key in keys) {
                if (row.ContainsKey(key)) {
                    if (row[key].IsDate()) {
                        if (row[key].ToDate() != dataSource[key].ToDate()) {
                            return false;
                        }
                    }
                    else if (row[key].IsNumeric()) {
                        if (row[key].ToDec() != dataSource[key].ToDec()) {
                            return false;
                        }
                    }
                    else {
                        if (row[key].ToStr() != dataSource[key].ToStr()) {
                            return false;
                        }
                    }
                }
                else {
                    return false;
                }
            }

            return true;
        }

        public JObject GetBrwsData(string brwsId, JObject dataSource, string baseView) {
            return GetBrwsData(brwsId, dataSource, baseView, null);
        }

        public JObject GetBrwsData(string brwsId, JObject dataSource, string baseView, JObject pagingInfo) {
            JObject result = new JObject();
            
            GenericRepository<EPF_BRWS> _brwsRepo = new GenericRepository<EPF_BRWS>(_db);
            EPF_BRWS brws = _brwsRepo.GetByCriteria(new { BRWS_ID = brwsId }).ToList().First();
            
            Dictionary<string, EPF_BRWS_COL> dataColInfos = GetColumnInfos(baseView);

            if (brws != null) {
                result.Add("brws", JToken.FromObject(brws));
                Dictionary<string, object> data = new Dictionary<string, object>(dataSource.ToObject<IDictionary<string, object>>(), StringComparer.CurrentCultureIgnoreCase);
                string sql = ParseBrwsSql(data, dataColInfos, brws.BRWS_SQL, true);
                JObject colInfos = JObject.FromObject(GetColumnInfos(sql));
                result.Add("cols", colInfos);

                JArray brwsData;
                if (pagingInfo != null) {
                    GetRowsWithPagination(sql, pagingInfo, result);
                }
                else {
                    brwsData = _db.GetRows(sql);
                    if (brwsData.Count() > 0) {
                        result.Add("rows", JArray.FromObject(brwsData));
                    }
                }
                
            }
            return result;
        }

        private double GetRowCount(string sql) {
            // replace columns with COUNT(*)
            sql = string.Format("SELECT COUNT(*) FROM ({0}) TMP", sql);
            
            // remove ORDER BY
            MatchCollection matchCollection = Regex.Matches(sql, @"ORDER\s+BY.*", RegexOptions.IgnoreCase);
            foreach (Match match in matchCollection) {
                sql = sql.Replace(match.Value, "");
            }

            return _db.GetAValue(sql).ToDbl();
        }

        private void GetRowsWithPagination(string sql, JObject pagingInfo, JObject result) {
            double skip = pagingInfo["skip"].ToDbl();
            double take = pagingInfo["take"].ToDbl();
            double totalCount = 0;
            JArray rows;

            // remove top keyword for server-side paging
            MatchCollection matchCollection = Regex.Matches(sql, @"(?<=^\s*SELECT\s+(|DISTINCT\s+))TOP\s+\d+", RegexOptions.IgnoreCase);
            foreach (Match match in matchCollection) {
                sql = sql.Replace(match.Value, "");
            }

            string filter = "";
            if (pagingInfo.ContainsKey("filter")) {
                filter = GetWhereClause(JArray.Parse(pagingInfo["filter"].ToStr()));
                if (!string.IsNullOrEmpty(filter)) {
                    filter = "WHERE " + filter;
                }
            }

            // put OFFSET/FETCH
            if (sql.ToUpper().Contains("ORDER BY")) {
                string orderBy = sql.Substring(sql.IndexOf("ORDER BY"));
                string selectFrom = sql.Substring(0, sql.IndexOf("ORDER BY"));

                sql = string.Format("SELECT * FROM ({0}) A {1} ", selectFrom, filter);
                totalCount = GetRowCount(sql);

                if (take == 0) {
                    take = totalCount;
                }

                sql = string.Format("SELECT * FROM ({0}) A {1} {2} OFFSET {3} ROWS FETCH NEXT {4} ROWS ONLY", selectFrom, filter, orderBy, skip, take);
                rows = _db.GetRows(sql);
                //sql = sql + string.Format(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", skip, take);
            }
            else {
                sql = string.Format("SELECT * FROM ({0}) A {1} ", sql, filter);
                totalCount = GetRowCount(sql);

                if (take == 0) {
                    take = totalCount;
                }

                sql = sql + string.Format(" ORDER BY 1 OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", skip, take);
                rows = _db.GetRows(sql);
            }

            result.Add("totalCount", totalCount);
            if (rows.Count() > 0) {
                result.Add("rows", rows);
            }

            return;
        }

        private string GetWhereClause(JArray filter) {
            string whereClause = "";

            if (filter[1].ToStr() == "and") {
                foreach (var item in filter) {
                    if (item.ToStr() == "and") {
                        whereClause += " and ";
                    }
                    else {
                        JArray subItems = JArray.Parse(item.ToStr());
                        if (subItems[1].ToStr() == "and") {
                            whereClause += GetWhereClause(subItems);
                        }
                        else {
                            if (subItems[1].ToStr() == "contains") {
                                whereClause += string.Format(" {0} like '%{1}%'", subItems[0], subItems[2]);
                            }
                            else {
                                whereClause += string.Format(" {0} {1} '{2}'", subItems[0], subItems[1], subItems[2]);
                            }
                        }
                    }
                }
            }
            else {
                JArray subItems = JArray.Parse(filter.ToStr());
                if (subItems[1].ToStr() == "contains") {
                    whereClause += string.Format(" {0} like '%{1}%'", subItems[0], subItems[2]);
                }
                else {
                    whereClause += string.Format(" {0} {1} '{2}'", subItems[0], subItems[1], subItems[2]);
                }
            }

            return whereClause;
        }


        



        public JObject GetQueryData(string brwsSql, JObject dataSource, string baseView) {
            JObject result = new JObject();
            Dictionary<string, EPF_BRWS_COL> dataColInfos = GetColumnInfos(baseView);

            if (!string.IsNullOrEmpty(brwsSql)) {
                Dictionary<string, object> data = new Dictionary<string, object>(dataSource.ToObject<IDictionary<string, object>>(), StringComparer.CurrentCultureIgnoreCase);
                string sql = ParseBrwsSql(data, dataColInfos, brwsSql);
                JObject colInfos = JObject.FromObject(GetColumnInfos(sql));
                result.Add("cols", colInfos);

                IEnumerable<dynamic> brwsData = _db.ExecQuery<dynamic>(sql);
                if (brwsData.Count() > 0) {
                    result.Add("rows", JArray.FromObject(brwsData));
                }
            }
            return result;
        }

        public void Action(string baseProcName, string cmd, JObject postData) {
            JObject dataSource = JObject.FromObject(postData["DataSource"]);
            SessionState session = postData["Session"].ToObject<SessionState>();

            List<JObject> parameters = GetParamInfos(baseProcName).Values.ToList();
            DBParameterCollection paramList = new DBParameterCollection();
            foreach (var param in parameters) {
                int paramType = Convert.ToInt32(param["PARAMETER_TYPE"]);
                string paramName = param["PARAMETER_NAME"].ToString().Replace("@", "");

                if (paramType == 1) {
                    if (paramName == "CMD") {
                        paramList.Add(new SqlParameter(paramName, cmd));
                    }
                    else {
                        object paramValue = DBNull.Value;
                        if (dataSource.ContainsKey(paramName))
                            paramValue = dataSource[paramName];
                        paramList.Add(new SqlParameter(paramName, paramValue));
                    }
                }
                else if (paramType == 2 && paramName == "RET_VAL") {
                    paramList.Add(new SqlParameter(paramName, 0));
                }

            }
            _db.ExecProcedure(baseProcName, paramList);


        }


        public void ExecMultiPickSelection(JObject postData) {
            string cmd = "U";
            JObject dataSource = JObject.FromObject(postData["dataSource"]);
            JArray selectedRecords = JArray.FromObject(postData["selectedRecords"]);
            string procName = postData["selectedGroup"]["UPD_PROCE"].ToStr();
            List<string> sqlStat = postData["selectedGroup"]["SQL_STAT"].ToStr().Replace("%<", "").Replace(":v>%s", "").Split(',').ToList();
            List<string> returnFields = postData["selectedGroup"]["PKLT_RTN_FLDS"].ToStr().Split(',').ToList();

            foreach (var record in selectedRecords) {
                DBParameterCollection paramList = new DBParameterCollection();
                List<JObject> parameters = GetParamInfos(procName).Values.ToList();

                paramList.Add(new SqlParameter("RET_VAL", 0));
                paramList.Add(new SqlParameter("CMD", cmd));
                for (int i = 3; i < parameters.Count; i++) {
                    int paramType = Convert.ToInt32(parameters[i]["PARAMETER_TYPE"]);
                    string paramName = parameters[i]["PARAMETER_NAME"].ToString().Replace("@", "");

                    if (paramType == 1 || paramType == 2) {
                        int statIndex = i - 3;
                        object paramValue = DBNull.Value;
                        if (statIndex < sqlStat.Count) {
                            string key = sqlStat[statIndex];
                            if (dataSource.ContainsKey(key)) {
                                paramValue = dataSource[key];
                            }
                        }
                        else {
                            int retIndex = i - (3 + sqlStat.Count);
                            if (retIndex < returnFields.Count) {
                                string key = returnFields[retIndex];
                                paramValue = record[key];
                            }
                        }

                        paramList.Add(new SqlParameter(paramName, paramValue));
                    }

                }

                _db.ExecProcedure(procName, paramList);


            }
            
        }

        private Dictionary<string, JObject> GetParamInfos(string procedureName) {
            return _db.GetParamInfos(procedureName);
        }

        public Dictionary<string, EPF_BRWS_COL> GetColumnInfos(string viewName) {
            return _db.GetColumnInfos(viewName);
        }

        private string ParseBrwsSql(Dictionary<string, object> dataSource, Dictionary<string, EPF_BRWS_COL> baseViewColInfos, string sql)
        {
            return ParseBrwsSql(dataSource, baseViewColInfos, sql, true);
        }

        private string ParseBrwsSql(Dictionary<string, object> data, Dictionary<string, EPF_BRWS_COL> baseViewColInfos, string sql, bool top1000) {
            sql = sql.ToUpper(); 
            sql = sql.Replace("%<:DBOWNER>", "dbo.");

            sql = sql.Replace("%<=:COMP_CODE>", string.Format("'{0}'", _db.EPSession.CoyID));
            sql = sql.Replace("%<:COMP_CODE>", string.Format("'{0}'", _db.EPSession.CoyID));

            sql = sql.Replace("%<=:COYID>", string.Format("'{0}'", _db.EPSession.CoyID));
            sql = sql.Replace("%<:COYID>", string.Format("'{0}'", _db.EPSession.CoyID));

            sql = sql.Replace("%<=:EMPE_ID>", string.Format("'{0}'", _db.EPSession.EmployeeID));
            sql = sql.Replace("%<:EMPE_ID>", string.Format("'{0}'", _db.EPSession.EmployeeID));

            sql = sql.Replace("%<=:USER_ID>", string.Format("'{0}'", _db.EPSession.LogonUser));
            sql = sql.Replace("%<:USER_ID>", string.Format("'{0}'", _db.EPSession.LogonUser));

            sql = sql.Replace("%<=:LANG_ID>", string.Format("{0}", _db.EPSession.DefaultLanguage));
            sql = sql.Replace("%<:LANG_ID>", string.Format("{0}", _db.EPSession.DefaultLanguage));
            sql = sql.Replace("<LANG_ID>", _db.EPSession.DefaultLanguage.ToStr());

            MatchCollection matchCollection = Regex.Matches(sql, @"%<=([a-zA-Z0-9_\-\.]+)>");
            foreach (Match match in matchCollection) {
                string colName = match.Value.Replace("%<=", "").Replace(">", "");

                if (data.ContainsKey(colName) && !string.IsNullOrEmpty(data[colName].ToStr())) {
                    if (baseViewColInfos.ContainsKey(colName)) {
                        if (baseViewColInfos[colName].col_type.ToStr().In("date", "datetime")) {
                            sql = sql.Replace(match.Value, string.Format("'{0}'", Convert.ToDateTime(data[colName]).ToString("yyyy-MM-dd HH:mm:ss")));
                        }
                        else if (baseViewColInfos[colName].col_type.ToStr() == "number") {
                            sql = sql.Replace(match.Value, data[colName].ToStr());
                        }
                        else {
                            sql = sql.Replace(match.Value, string.Format("'{0}'", data[colName].ToStr()));
                        }
                    }
                    else {
                        if (data[colName].IsDate()) {
                            sql = sql.Replace(match.Value, string.Format("'{0}'", Convert.ToDateTime(data[colName]).ToString("yyyy-MM-dd HH:mm:ss")));
                        }
                        else {
                            sql = sql.Replace(match.Value, string.Format("'{0}'", data[colName].ToStr()));
                        }
                    }

                }
                else {
                    sql = sql.Replace(match.Value, "null");
                }

            }


            matchCollection = Regex.Matches(sql, @"%<[^ ]*>");
            foreach (Match match in matchCollection) {
                string result = "";
                string[] colNames = match.Value.Replace("%<", "").Replace(">", "").Replace(" ", "").Split(',');
                foreach (string colName in colNames) {
                    if (result != "") {
                        result = string.Format("{0} AND", result);
                    }

                    string keyName = "";
                    string valName = "";

                    if (colName.Contains("=")) {
                        keyName = colName.Split('=')[0];
                        valName = colName.Split('=')[1];
                    }
                    else {
                        keyName = colName;
                        valName = colName;
                    }

                    if (data.ContainsKey(valName)) {
                        object value = data[valName];
                        if (baseViewColInfos.ContainsKey(valName)) {
                            if (baseViewColInfos[valName].col_type.ToStr().In("date", "datetime")) {
                                result = string.Format("{0} {1} = '{2}'", result, keyName, Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                            else if (baseViewColInfos[valName].col_type.ToStr() == "number") {
                                if (string.IsNullOrEmpty(value.ToStr())) {
                                    result = string.Format("{0} {1} = null", result, keyName);
                                }
                                else {
                                    result = string.Format("{0} {1} = {2}", result, keyName, value.ToStr());
                                }
                            }
                            else {
                                result = string.Format("{0} {1} = '{2}'", result, keyName, value.ToStr());
                            }
                        }
                        else {
                            if (value.IsDate()) {
                                result = string.Format("{0} {1} = '{2}'", result, keyName, Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                            else {
                                result = string.Format("{0} {1} = '{2}'", result, keyName, value.ToStr());
                            }
                        }
                    }
                    else {
                        if (valName.Substring(0, 1) == ":") {
                            object value = "";
                            if (valName.Replace(":", "").In("COMP_CODE", "COYID")) {
                                value = _db.EPSession.CoyID;
                            }
                            else if (valName.Replace(":", "") == "EMPE_ID") {
                                value = _db.EPSession.EmployeeID;
                            }
                            else if (valName.Replace(":", "") == "USER_ID") {
                                value = _db.EPSession.LogonUser;
                            }
                            result = string.Format("{0} {1} = '{2}'", result, keyName, value.ToStr());
                        }
                        else {
                            result = string.Format("{0} {1} = null", result, keyName);
                        }
                    }
                }

                if (result != "") {
                    sql = sql.Replace(match.Value, result);
                }
            }

            // Add in TOP 1000 for performance
            if (top1000) {
                if (!Regex.IsMatch(sql, @"^SELECT[ \t\n]+(DISTINCT[ \t\n]*)?TOP[ \t\n]", RegexOptions.IgnoreCase)) {
                    Match match = Regex.Match(sql, @"^SELECT[ \t\n]+(DISTINCT[ \t\n]*)?");
                    if (match != null) {
                        sql = Regex.Replace(sql, @"^SELECT[ \t\n]+(DISTINCT[ \t\n]*)?", match.Value.TrimEnd(' ') + " TOP 1000 ");
                    }
                }
            }

            
            return sql;
        }

        public string GetScreenModuleId(string scrnId) {
            return _db.GetAValue("SELECT MDULE_ID FROM SV_EPF_SCRN WHERE SCRN_ID = %s", scrnId).ToStr();
        }
    }
}
