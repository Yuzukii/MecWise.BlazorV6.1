using System;
using System.Collections.Generic;
using System.Linq;
using MecWise.Blazor.Entities;
using MecWise.Blazor.Api.DataAccess;
using Newtonsoft.Json.Linq;
using MecWise.Blazor.Common;

namespace MecWise.Blazor.Api.Repositories
{
    public class SC_USER_PROFILE_Repository :  GenericRepository<SV_SC_USER_PROFILE>
    {
        DBHelper _db;
        public SC_USER_PROFILE_Repository(DBHelper db) : base(db)
        {
            _db = db;
        }

        public String GetCompAces(string userId)
        {
            string sql = "SELECT ISNULL(COMP_ACES,'%') AS COMP_ACES FROM SV_SC_USER_PROFILE WHERE USER_ID= %s AND AC_FLAG NOT LIKE '1%' AND AC_EFF_DATE <= dbo.GetLocDate() AND ISNULL(AC_EXPY_DATE,'20990101') >= dbo.GetLocDate()";
            IEnumerable<SV_SC_USER_PROFILE> objUsers = _db.ExecQuery<SV_SC_USER_PROFILE>(sql, userId);
            if(objUsers.Count() > 0)
            {
                SV_SC_USER_PROFILE objUser = objUsers.First();
                return objUser.COMP_ACES;
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetOpenIdConnectUserID(string emailAddr) {
            string sql = "SELECT TOP 1 USER_ID FROM SV_BLZ_OIDC_USER_PROFILE WHERE " +
                "EMAIL_ADDR = %s AND AC_FLAG NOT LIKE '1%' AND AC_EFF_DATE <= dbo.GetLocDate() " +
                "AND ISNULL(AC_EXPY_DATE,'20990101') >= dbo.GetLocDate() ORDER BY CREA_DATE DESC";

            return _db.GetAValue(sql, emailAddr).ToStr();
        }

        public String CheckByPassLogin(string userId)
        {
            string sql = "SELECT (SELECT  t2.KEY_VAL + ',' FROM SV_CM_APP t2 WHERE t2.KEY_SECT = t1.KEY_SECT AND t2.KEY_NAME IN('SC.USER.LOGON.SCRN.ID', 'SC.USER.LOGON.SCRN.PARAMS') FOR XML PATH('')) AS KEY_VAL ";
                   sql+=" FROM SV_CM_APP t1 WHERE t1.KEY_SECT = 'EPF.FEATURES' AND t1.KEY_NAME = 'SC.USER.LOGON.USER.ID' AND t1.KEY_VAL=%s";
            return _db.GetAValue(sql,userId).ToString();            
        }

        public Int32 validTokenTime(string keySect, string keyName)
        {
            string sql = "SELECT KEY_VAL FROM SV_CM_APP WHERE KEY_SECT=%s AND KEY_NAME=%s ";
            int minutes = _db.GetAValue(sql, keySect, keyName).ToInt(); //_db.ExecQuery<SV_CM_APP>(sql, keySect, keyName);
            return minutes;
        }

        public JObject GetPushNotiApiSetting()
        {
            JObject setting = new JObject();
            string sql = "SELECT KEY_VAL FROM SV_CM_APP WHERE KEY_SECT LIKE 'PUSH%' AND KEY_NAME = 'MecwiseMessengerApiUrl'";
            setting.Add("ApiUrl", _db.GetAValue(sql).ToString());

            sql = "SELECT KEY_VAL FROM SV_CM_APP WHERE KEY_SECT LIKE 'PUSH%' AND KEY_NAME = 'MecwiseMessengerApiKey'";
            setting.Add("ApiKey", _db.GetAValue(sql).ToString());

            sql = "SELECT KEY_VAL FROM SV_CM_APP WHERE KEY_SECT LIKE 'PUSH%' AND KEY_NAME = 'WebPushServerPublicKey'";
            setting.Add("WebPushServerPublicKey", _db.GetAValue(sql).ToString());

            return setting;
        }

        public string GetUserEmail(string userId) {
            return _db.GetAValue("SELECT B.EMAIL_ADDR FROM %<:DBOWNER>SV_SC_USER_PROFILE A INNER JOIN %<:DBOWNER>SV_EMPE_EMAIL B " +
                    " ON A.EMPE_ID = B.EMPE_ID WHERE A.USER_ID = %s", userId).ToStr();
        }

        public void ParseEmailUrl(ref string link, string coyId) {
            if (string.IsNullOrEmpty(link)) {
                return;
            }

            string routeUrl = string.Empty;
            try {
                string sql = "Select ROUTE_ID From %<:DbOwner>SV_SC_CUST_COMP Where COMP_CODE = %s";
                routeUrl = _db.GetAValue(sql, coyId).ToStr();
            }
            catch (Exception) {
                routeUrl = string.Empty;
            }

            if (string.IsNullOrEmpty(routeUrl)) {
                link = link.Replace("{ROUTE_ID}/", string.Empty);
            }
            else {
                if (routeUrl.ToUpper().Contains("{ROUTE_ID}")) {
                    link = link.Replace("{ROUTE_ID}", routeUrl);
                }
                else {
                    link = link.Replace("/Ess", string.Format("/{0}/Ess", routeUrl));
                }
            }

        }

    }
}
