using System;
using System.Collections.Generic;
using System.Linq;
using MecWise.Blazor.Entities;
using MecWise.Blazor.Api.DataAccess;
using Newtonsoft.Json.Linq;
using MecWise.Blazor.Common;
using System.Net.Http.Headers;
using System.Data.SqlClient;

namespace MecWise.Blazor.Api.Repositories {
    public class JobRepository : GenericRepository<SV_REPORT_DEFN> {
        DBHelper _db;

        public JobRepository(DBHelper db) : base(db) {
            _db = db;
        }

        public JObject GetReport(string reportId) {
            string sql = _db.SqlExpr("SELECT * FROM SV_REPORT_DEFN WHERE REPORT_ID = %s", reportId);
            JObject report = _db.GetARow(sql);

            sql = _db.SqlExpr("SELECT * FROM SV_REPORT_DEFN_DETAILS WHERE REPORT_ID = %s AND FORMULA_TYPE = 'PARAM'", reportId);
            JArray reportDetails = _db.GetRows(sql);
            report.Add("_DETAILS", reportDetails);

            sql = _db.SqlExpr("SELECT * FROM SV_REPORT_DEFN_DETAILS WHERE REPORT_ID = %s AND FORMULA_TYPE = 'FIELD'", reportId);
            JArray reportFields = _db.GetRows(sql);
            report.Add("_FIELDS", reportFields);


            return report;
        }

        public JObject GetReportJobStatus(string compCode, string jobId) {
            string sql = "Select A.JOB_TYPE, JOB_STATUS, ERR_DESC, JOB_DESC, OUTPUT_FILE_NAME, JOB_SUBMIT_TIME From %<:DbOwner>SV_MF_JOB A Left Outer Join %<:DbOwner>SV_MF_JOB_ERROR B On A.JOB_NUMBER = B.JOB_NUMBER Where A.COMP_CODE = %s And A.JOB_NUMBER = %s ";
            //sql = _db.SqlExpr(sql, compCode, jobId, "ACTIVE_REPORT", "ACTIVE_REPORT_DOTNET", "CRYSTAL_REPORT", "EXCEL_REPORT", "RS_REPORT", "XTRA_REPORT");
            sql = _db.SqlExpr(sql, compCode, jobId);
            return _db.GetARow(sql);
        }

        public JArray FetchReportPreferences(string reportId)
        {
            try
            {
                string sql = "SELECT REPORT_ID,PREFER_CODE, PREFER_VAL FROM SV_REPORT_MF_PREFER WHERE COMP_CODE = %s AND REPORT_ID = %s "
                + " UNION "
                + " SELECT REPORT_ID,PREFER_CODE, PREFER_VAL FROM SV_REPORT_MF_PREFER WHERE COMP_CODE = %s "
                + " AND NOT EXISTS (SELECT 1 FROM SV_REPORT_MF_PREFER WHERE COMP_CODE = %s AND REPORT_ID = %s) ";

                sql = _db.SqlExpr(sql, _db.CoyID, reportId, _db.CoyID, _db.CoyID, reportId);
                return _db.GetRows(sql);
            }
            catch (Exception)
            {
                return new JArray();
            }
            
        }

    }
}
