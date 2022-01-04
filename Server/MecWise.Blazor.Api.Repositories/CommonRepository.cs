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
    public class CommonRepository : GenericRepository<SV_CM_APP> {
        DBHelper _db;
        public CommonRepository(DBHelper db) : base(db) {
            _db = db;
        }

        public int GetNewLineNo(JObject postData) {
            return _db.GetAValue("SELECT ISNULL(MAX(LINE_NO),0) + 1 FROM EPF_UPLD_FILE WHERE KEY_CODE = %s", postData["KEY_CODE"].ToStr()).ToInt();
        }

        public void SaveFileRecord(JObject postData) {

            DBParameterCollection Params = new DBParameterCollection();
            Params.Add("RET_VAL", 0);
            Params.Add("CMD", "I");
            Params.Add("KEY_CODE", postData["KEY_CODE"]);
            Params.Add("LINE_NO", postData["LINE_NO"]);
            Params.Add("UPLD_FILENAME", postData["UPLD_FILENAME"]);
            Params.Add("ACT_FILENAME", postData["ACT_FILENAME"]);
            if (string.IsNullOrEmpty(postData["FRIENDLY_NAME"].ToStr())) {
                Params.Add("FRIENDLY_NAME", postData["SEL_FILE"]);
            }
            else {
                Params.Add("FRIENDLY_NAME", postData["FRIENDLY_NAME"]);
            }
            
            Params.Add("FILE_REMARKS", postData["FILE_REMARKS"]);
            Params.Add("FILE_TYPE", postData["FILE_TYPE"]);
            Params.Add("FILE_SIZE", postData["FILE_SIZE"]);
            Params.Add("FILE_EXT", postData["FILE_EXT"]);
            _db.ExecProcedure("UP_EPF_UPLD_FILE", Params);

        }

        public void DeleteFileRecord(JObject postData) {

            DBParameterCollection Params = new DBParameterCollection();
            Params.Add("RET_VAL", 0);
            Params.Add("CMD", "D");
            Params.Add("KEY_CODE", postData["KEY_CODE"]);
            Params.Add("LINE_NO", postData["LINE_NO"]);
            Params.Add("UPLD_FILENAME", postData["UPLD_FILENAME"]);
            Params.Add("ACT_FILENAME", postData["ACT_FILENAME"]);
            if (string.IsNullOrEmpty(postData["FRIENDLY_NAME"].ToStr())) {
                Params.Add("FRIENDLY_NAME", postData["SEL_FILE"]);
            }
            else {
                Params.Add("FRIENDLY_NAME", postData["FRIENDLY_NAME"]);
            }

            Params.Add("FILE_REMARKS", postData["FILE_REMARKS"]);
            Params.Add("FILE_TYPE", postData["FILE_TYPE"]);
            Params.Add("FILE_SIZE", postData["FILE_SIZE"]);
            Params.Add("FILE_EXT", postData["FILE_EXT"]);
            _db.ExecProcedure("UP_EPF_UPLD_FILE", Params);

        }
    }
}
