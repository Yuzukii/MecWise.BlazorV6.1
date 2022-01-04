using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using ePlatform.CommonClasses;

namespace MecWise.Blazor.Api.DataAccess
{
    public class DBHelperV2
    {
        private EPSession _session;

        public DBHelperV2(string connectionString) {
            _session = new EPSession("MSSQL", "dbo", connectionString);
        }
                
        
        public IEnumerable<T> ExecQuery<T>(string sqlTemplate, params object[] paramArray)
        {
            string sql = _session.SqlExpr(sqlTemplate, paramArray);
            using (var connection = _session.Connection)
            {
                var result = connection.Query<T>(sql).ToList();
                if (result != null)
                {
                    return result;
                }
                else
                {
                    return new List<T>();
                }

            }
            
        }

        public int ExecProcedure(string procedureName, DynamicParameters param) {
            int affectedRows;
            using (var connection = _session.Connection) {
                affectedRows = connection.Execute(procedureName, param, commandType: CommandType.StoredProcedure);

            }

            return affectedRows;
        }
    }
}
