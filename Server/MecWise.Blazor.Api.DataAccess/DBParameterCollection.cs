using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MecWise.Blazor.Api.DataAccess {
    public class DBParameterCollection : ICollection<SqlParameter> {
        List<SqlParameter> _params;

        public DBParameterCollection() {
            _params = new List<SqlParameter>();
        }
        public SqlParameter this[string key] { 
            get {
                return _params.Find(x => x.ParameterName == key);
            }
            set {
                _params[_params.FindIndex(x => x.ParameterName == key)] = value;
            } 
        }

        public int Count => _params.Count;

        public bool IsReadOnly => false;

        public void Add(SqlParameter item) {
            _params.Add(item);
        }

        public void Add(string paramName, object value) {
            _params.Add(new SqlParameter() { ParameterName = paramName, Value = value });
        }

        public void Add(string paramName, object value, ParameterDirection direction) {
            _params.Add(new SqlParameter() { ParameterName = paramName, Value = value, Direction = direction });
        }

        public void Clear() {
            _params.Clear();
        }

        public bool Contains(SqlParameter item) {
            return _params.Contains(item);
        }

        public void CopyTo(SqlParameter[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public IEnumerator<SqlParameter> GetEnumerator() {
            return _params.GetEnumerator();
        }

        public bool Remove(SqlParameter item) {
            return _params.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _params.GetEnumerator();
        }
    }


    
}
