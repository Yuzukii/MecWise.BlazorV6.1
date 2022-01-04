using System;
using System.Collections.Generic;
using System.Linq;
using MecWise.Blazor.Api.DataAccess;
using MecWise.Blazor.Entities;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection;
using System.Data.SqlClient;
using Dapper;

namespace MecWise.Blazor.Api.Repositories
{
    public class GenericRepository<T> : iRepository<T> where T : class
    {
        DBHelper _db;
        EntitySettingAttribute _entitySetting;

        public string BaseViewName { get { return _entitySetting.BaseViewName; }  }
        public string BaseProcedureName { get { return _entitySetting.BaseProcedureName; } }
        public string[] PrimaryKeyNames { get { return _entitySetting.PrimaryKeys; } }

        public GenericRepository(DBHelper db) {
            _db = db;
            _entitySetting = GetAttribute(typeof(T));
        }
        

        public IEnumerable<T> GetByCriteria(params object[] paramList)
        {
            if (_entitySetting.PrimaryKeys.Length == 0)
                return null;

            string sql = string.Format("SELECT * FROM {0}", _entitySetting.BaseViewName);

            DynamicParameters parameters = new DynamicParameters();
            int pCount = 0;

            string criteria = "";
            foreach (object param in paramList)
            {
                string subCriteria = "";
                foreach (PropertyInfo propInfo in param.GetType().GetProperties())
                {
                    string paramName = string.Format("@p{0}", pCount);
                    if (subCriteria == "")
                    {
                        subCriteria = string.Format("{0} = {1}", propInfo.Name, paramName);
                    }
                    else
                    {
                        subCriteria = string.Format("{0} AND {1} = {2}", subCriteria, propInfo.Name, paramName);
                    }

                    parameters.Add(paramName, param.GetType().GetProperty(propInfo.Name).GetValue(param));
                    pCount += 1;
                }

                if (criteria == "")
                {
                    criteria = string.Format("({0})", subCriteria);
                }
                else
                {
                    criteria = string.Format("{0} OR ({1}) ", criteria, subCriteria);
                }

            }

            sql = string.Format("{0} WHERE {1}", sql, criteria);

            return _db.ExecQueryDapper<T>(sql, parameters);
        }      

        public IEnumerable<T> GetAll()
        {
            string sql = string.Format("SELECT * FROM {0}", _entitySetting.BaseViewName);
            return _db.ExecQueryDapper<T>(sql).ToList<T>();
        }

        public IEnumerable<T> GetAllByComp(string compCode)
        {
            if (typeof(T).GetProperty("COMP_CODE") == null)
                return new List<T>();

            string sql = string.Format("SELECT * FROM {0} WHERE COMP_CODE = @COMP_CODE", _entitySetting.BaseViewName);
            return _db.ExecQueryDapper<T>(sql, new { COMP_CODE = compCode }).ToList<T>();
        }

        public void DeleteByKeys(object param = null) {
            List<T> entities = this.GetByCriteria(param).ToList<T>();
            if(entities.Count > 0)
                this.Delete(entities[0]);
        }

        public void Delete(T entity)
        {
            this.ExecBaseProcedure("D", entity);
        }

        public void Insert(T entity)
        {
            this.ExecBaseProcedure("I", entity);
        }

        public void Update(T entity)
        {
            this.ExecBaseProcedure("U", entity);
        }

        private IEnumerable<SPParameter> GetParamNames() {
            List<SPParameter> parameters = _db.ExecQueryDapper<SPParameter>("sp_procedure_params_100_managed @procedure_name", 
                new { procedure_name = _entitySetting.BaseProcedureName }).ToList();
            return parameters;
        }

        private void ExecBaseProcedure(string cmd, T entity) {
            var param = new DynamicParameters();
            foreach (SPParameter spParam in this.GetParamNames())
            {
                if (spParam.PARAMETER_TYPE == 1)
                {
                    if (spParam.PARAMETER_NAME == "@CMD")
                    {
                        param.Add(spParam.PARAMETER_NAME, cmd);
                    }
                    else
                    {
                        object paramValue = DBNull.Value;
                        if (HasProperty(entity, spParam.PARAMETER_NAME.Replace("@", "")))
                            paramValue = GetPropertyValue(entity, spParam.PARAMETER_NAME.Replace("@", ""));
                        param.Add(spParam.PARAMETER_NAME, paramValue);
                    }
                }
                else if (spParam.PARAMETER_TYPE == 2 && spParam.PARAMETER_NAME == "@RET_VAL") 
                {
                    param.Add(spParam.PARAMETER_NAME, 0);
                }
            }

            _db.ExecProcedureDapper(_entitySetting.BaseProcedureName, param);
        }

        private bool HasProperty(T src, string propName) {
            if (src.GetType().GetProperty(propName) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private object GetPropertyValue(T src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        private EntitySettingAttribute GetAttribute(Type t)
        {
            // Get instance of the attribute.
            EntitySettingAttribute EntitySetting =
                (EntitySettingAttribute)Attribute.GetCustomAttribute(t, typeof(EntitySettingAttribute));

            if (EntitySetting == null)
            {
                return new EntitySettingAttribute();
            }
            else
            {
                return EntitySetting;
            }
        }

    }

    public class SPParameter {

        public string PARAMETER_NAME { get; set; }
        public int PARAMETER_TYPE { get; set; }
        public string TYPE_NAME { get; set; }
        
    }
}
