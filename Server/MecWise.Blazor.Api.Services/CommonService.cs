using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MecWise.Blazor.Entities;
using MecWise.Blazor.Api.DataAccess;
using MecWise.Blazor.Api.Repositories;
using MecWise.Blazor.Common;
using System.IO;
using Newtonsoft.Json.Linq;
using ePlatform.Security;
using System.Runtime.InteropServices;

namespace MecWise.Blazor.Api.Services
{
    public class CommonService {

        CommonRepository _commonRepository;

        public CommonService() {
            DBHelper db = new DBHelper(ApiSetting.ConnectionString);
            _commonRepository = new CommonRepository(db);
        }

        public CommonService(SessionState session) 
        {
            DBHelper db = new DBHelper(session.UserID, session.CompCode, session.LangId, ApiSetting.ConnectionString);
            _commonRepository = new CommonRepository(db);
        }

        public JObject GetV5NavigationKey(JObject postData) {
            JObject retObj = new JObject();
            string connectId = postData["connectId"].ToStr();
            string connectKey = postData["connectKey"].ToStr();
            SessionState sess = postData["session"].ToObject<SessionState>();
            String relativeURL = postData["relativeURL"].ToString();
            int userRole = postData["userRole"].ToInt();
            String key = @"{0}||{1}||{2}||{3}||{4}||{5}||{6}";
            key = string.Format(key, connectId, connectKey, DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt"), sess.UserID, userRole, sess.CompCode, relativeURL);
            EPEncrypt enc = new EPEncrypt();
            String encryptedKey = enc.EncryptString(key).Replace("+", "~~");

            retObj.Add("encryptedKey", encryptedKey);
            retObj.Add("key", key);
            return retObj;
        }

        public string RememberUserToken(JObject postData)
        {
            EPEncrypt enc = new EPEncrypt();
            DateTime expiredTime = DateTime.Now.AddMinutes(ApiSetting.SessionExpireTime);

            // never expire
            if (ApiSetting.SessionExpireTime == 0) {
                expiredTime = new DateTime(2099, 1, 1);
            }
            
            string launchUrl = "";
            string clearText = string.Format("{0};{1};EXPIRE_TIME={2};{3};{4}", postData["userId"], postData["clientId"], expiredTime.ToString("yyyyMMdd HH:mm:ss"), launchUrl, postData["compCode"]);
            return enc.EncryptString(clearText);
        }
    }
}
