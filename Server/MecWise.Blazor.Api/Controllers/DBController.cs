using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Newtonsoft.Json.Linq;
using MecWise.Blazor.Api.Services;
using MecWise.Blazor.Api.DataAccess;
using MecWise.Blazor.Common;
using System.Threading.Tasks;
using ePlatform.CommonClasses;
using System.Data;
using System.Data.SqlClient;

namespace MecWise.Blazor.Api.Controllers
{
    [Authorize]
    public class DBController : ApiController
    {
        DBHelper _db;
        
        public DBController() {
            _db = new DBHelper(ApiSetting.ConnectionString);
        }

        [HttpPost]
        [Route("DB/ExecQuery")]
        public async Task<HttpResponseMessage> ExecQuery() {
            string value = await Request.Content.ReadAsStringAsync();

            try {
                QueryTemplate template = JObject.Parse(value).ToObject<QueryTemplate>();
                IEnumerable<dynamic> result = _db.ExecQuery<dynamic>(template.SqlTemplate, template.Parameters);

                return Request.CreateResponse(HttpStatusCode.OK, JArray.FromObject(result));
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //[HttpPost]
        //[Route("DB/ExecProc")]
        //public async Task<HttpResponseMessage> ExecProc() {
        //    string value = await Request.Content.ReadAsStringAsync();

        //    try {
        //        ProcedureTemplate template = JObject.Parse(value).ToObject<ProcedureTemplate>();
        //        List<IDataParameter> sqlParams = new List<IDataParameter>();
        //        foreach (var param in template.Params) {
        //            SqlParameter sqlParam = new SqlParameter(param.Name, param.Value);
        //            sqlParam.Direction = param.Direction;
        //            sqlParams.Add(sqlParam);
        //        }

        //        object retObj = _db.EpfExecProcedure(template.ProcedureName, sqlParams);
        //        JArray retRows = new JArray();
        //        retRows.Add(retObj);
        //        return Request.CreateResponse(HttpStatusCode.OK, retRows);
        //    }
        //    catch (Exception ex) {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
        //    }
        //}
    }
}
