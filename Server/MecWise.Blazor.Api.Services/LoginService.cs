using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MecWise.Blazor.Entities;
using MecWise.Blazor.Api.DataAccess;
using MecWise.Blazor.Api.Repositories;
using Newtonsoft.Json.Linq;
using ePlatform.Security;
using MecWise.Security;
using System.Globalization;
using MecWise.Blazor.Common;
using Mecwise.Messenger.CommonClasses;
using Mecwise.Messenger.Entities;
using ePlatform.Mail;

namespace MecWise.Blazor.Api.Services
{
    public class LoginService 
    {
        //GenericRepository<SV_SC_USER_PROFILE> _userProfileRepo;
        GenericRepository<SV_MF_COMP> _compProfileRepo;
        SC_USER_PROFILE_Repository _userProfileRepo;
        DBHelper _db;

        public LoginService() {
            _db = new DBHelper(ApiSetting.ConnectionString);
            _userProfileRepo = new SC_USER_PROFILE_Repository(_db);
            _compProfileRepo = new GenericRepository<SV_MF_COMP>(_db);
        }

        public LoginService(SessionState session) {
            _db = new DBHelper(session.UserID, session.CompCode, session.LangId, ApiSetting.ConnectionString);
            _userProfileRepo = new SC_USER_PROFILE_Repository(_db);
            _compProfileRepo = new GenericRepository<SV_MF_COMP>(_db);
        }

        public List<SV_SC_USER_PROFILE> GetUserProfiles(string userId)
        {
            return _userProfileRepo.GetByCriteria(new { USER_ID = userId }).ToList();
        }

        public IEnumerable<SV_MF_COMP> GetCompProfiles()
        {
            return _compProfileRepo.GetAll();
        }

        public JObject GetPushNotiReg(string compCode, string empeId)
        {
            try
            {
                JObject apiSetting = _userProfileRepo.GetPushNotiApiSetting();
                Mecwise.Messenger.CommonClasses.WebAPIUtility msgAPI = new Mecwise.Messenger.CommonClasses.WebAPIUtility();
                msgAPI.ApiUrl = apiSetting["ApiUrl"].ToString();
                msgAPI.ApiKey = apiSetting["ApiKey"].ToString();
                UserRegistrationBase reg = msgAPI.GetMecWiseRegistration(compCode, empeId);

                JObject data = new JObject();
                data.Add("apiSetting", apiSetting);
                data.Add("registration", JObject.FromObject(reg));

                return data; 
            }
            catch (Exception ex)
            {
                return new JObject();
            }
            
        }

        public bool UpdatePushNotiReg(JObject reg)
        {
            try
            {
                JObject apiSetting = _userProfileRepo.GetPushNotiApiSetting();
                Mecwise.Messenger.CommonClasses.WebAPIUtility msgAPI = new Mecwise.Messenger.CommonClasses.WebAPIUtility();
                msgAPI.ApiUrl = apiSetting["ApiUrl"].ToString();
                msgAPI.ApiKey = apiSetting["ApiKey"].ToString();
                msgAPI.UpdateWebPushSubscription(reg["MecwiseRegisterID"].ToStr(),reg["WebPushSubscription"].ToStr());
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool IsValidTokenUser(string encryptVal) {
            EPEncrypt enc = new EPEncrypt();
            string decryptStr = enc.DecryptString(encryptVal);
            string[] tokenVal = decryptStr.Split(';');
            string userId = tokenVal[0];
            string chkUserActive = _userProfileRepo.GetCompAces(userId);
            return (!String.IsNullOrEmpty(chkUserActive) ? true : false);
        }

        public bool IsValidOpenIdConnectUser(string encryptVal)
        {
            EPEncrypt enc = new EPEncrypt();
            string decryptStr = enc.DecryptString(encryptVal);
            string[] tokenVal = decryptStr.Split(';');
            string emailAddr = tokenVal[0];
            string userId = _userProfileRepo.GetOpenIdConnectUserID(emailAddr);
            if (!string.IsNullOrEmpty(userId)) {
                return true;
            }

            return false;
        }

        public JObject CheckToken(string token, LoginType tokenType)
        {
            JObject retObj = new JObject();
            retObj.Add("retSts", false);
            retObj.Add("UserID", "");
            retObj.Add("CompID", "");
            retObj.Add("ClientID", "");
            retObj.Add("LaunchURL", "");
            retObj.Add("UserType", "");
            retObj.Add("errMsg", "");

            try {

                EPEncrypt enc = new EPEncrypt();
                string tokenStr = enc.DecryptString(token);
                string[] tokenVal = tokenStr.Split(';');
                string userId = tokenVal[0];


                /*
                
                Check for byPass login
                - Only userId is encrypted in token
                - Get CompCode and LaunchUrl from CM_APP based on userId
                
                Format: [UserID]
                e.g. mwdAuto

                */
                if (tokenVal.Length == 1) {
                    retObj["UserID"] = userId;
                    string chkByPassLogin = _userProfileRepo.CheckByPassLogin(userId);
                    if (!String.IsNullOrEmpty(chkByPassLogin)) {
                        string[] scrnInfo = chkByPassLogin.Split(',');
                        if (scrnInfo.Length >= 2) {
                            retObj["retSts"] = true;
                            retObj["LaunchURL"] = "EpfScreen/" + scrnInfo[0];
                            string CompCode = scrnInfo[1].ToStr();
                            if (CompCode.Contains("=") && CompCode.Contains(";")) CompCode = CompCode.Substring(CompCode.IndexOf("=") + 1, CompCode.IndexOf(";") - CompCode.IndexOf("=") - 1);
                            retObj["CompID"] = CompCode;
                            retObj["UserType"] = LoginType.byPassTokenAccess.ToInt();
                            return retObj;
                        }
                    }
                }


                
                /*
                
                Check for normal/oidc token access
                
                Format-1: [UserID];[clientID];[yyMMdd HH:mm:ss];[launchUrl]
                e.g. eplatform;mecwise.blazor;200101 00:00:00;EpfScreen/MF_CARD_G_BLZ

                Format-2: [UserID];[clientID];[yyMMdd HH:mm:ss];[launchUrl];[CompCode]
                e.g. eplatform;mecwise.blazor;200101 00:00:00;EpfScreen/MF_CARD_G_BLZ;PUB

                Format-3: [UserID];[clientID];EXPIRE_TIME=[yyyyMMdd HH:mm:ss];[launchUrl];[CompCode]
                e.g. eplatform;mecwise.blazor;EXPIRE_TIME=20990101 00:00:00;;PUB

                */
                DateTime startTime = DateTime.MinValue;
                DateTime expireTime = DateTime.MinValue;
                string tokenTimeType = "";
                string tokentime = tokenVal[2].ToString();
                if (tokentime.IsDate("yyMMdd HH:mm:ss")) {
                    tokenTimeType = "START_TIME";
                    startTime = DateTime.ParseExact(tokentime, "yyMMdd HH:mm:ss", CultureInfo.InvariantCulture);
                }
                else {
                    tokenTimeType = tokentime.Split('=')[0];
                    tokentime = tokentime.Split('=')[1];
                    expireTime = DateTime.ParseExact(tokentime, "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture);
                }

                // oidc use same token format but put email address in user id place
                // must replace email address with user id to go futher
                if (tokenType == LoginType.oidcTokenAccess) {
                    userId = _userProfileRepo.GetOpenIdConnectUserID(userId);
                }

                retObj["UserID"] = userId;
                retObj["ClientID"] = tokenVal[1];
                retObj["LaunchURL"] = tokenVal[3];

                string tokenCompCode = "";
                if (tokenVal.Length == 5) {
                    tokenCompCode = tokenVal[4];
                }

                //check USERID
                string compAces = _userProfileRepo.GetCompAces(userId);
                if (!String.IsNullOrEmpty(compAces)) {
                    Int32 validtime = _userProfileRepo.validTokenTime("DEFAULT", "VALID_TOKEN_DURATION");

                    // Check Token alive time
                    if ((tokenTimeType == "START_TIME" && startTime.AddSeconds(validtime) >= DateTime.Now)
                        || (tokenTimeType == "EXPIRE_TIME" && expireTime > DateTime.Now)
                        ) {

                        retObj["retSts"] = true;
                        if (!string.IsNullOrEmpty(tokenCompCode) && (compAces == "%" || compAces.Contains(tokenCompCode))) {
                            retObj["CompID"] = tokenCompCode;
                        }
                        else {
                            retObj["CompID"] = compAces;
                        }

                        retObj["UserType"] = tokenType.ToInt();
                    }
                    else {
                        retObj["errMsg"] = "Request Time Out";
                    }
                }
                else {
                    retObj["errMsg"] = "Invalid User";
                }

            }
            catch (Exception) {
                //retObj["errMsg"] = "Invalid Request";
                retObj["errMsg"] = token;
            }
            
            return retObj;
        }

        public string GeneratePushNotiReg(JObject postData) {
            JObject apiSetting = _userProfileRepo.GetPushNotiApiSetting();
            WebAPIUtility messengerUtility = new WebAPIUtility();
            messengerUtility.ApiUrl = apiSetting["ApiUrl"].ToString();
            messengerUtility.ApiKey = apiSetting["ApiKey"].ToString();

            string compCode = postData["compCode"].ToStr();
            string empeId = postData["empeId"].ToStr();
            DateTime effDate = postData["effDate"].ToDate();
            DateTime expyDate = postData["expyDate"].ToDate();

            string regId = messengerUtility.GenerateMecwiseRegisterID(compCode, empeId, effDate, expyDate);
            return regId;
        }


        public string ChangePassword(string newPwd) {
            string message = string.Empty;

            UserProfile userProfile = new UserProfile(_db.EPSession);

            if (userProfile.Fetch(_db.EPSession.LogonUser)) {
                if (userProfile.ResetPassword(newPwd)) {
                    if (userProfile.Update() != 0) {
                        message = "Error updating password record";
                    }
                }
                else {
                    message = userProfile.LastErrorMessage;
                }
            }
            else {
                message = "User name not found";
            }

            return message;
        }

        public string ResetPassword(string userId) {
            string message = string.Empty;

            UserProfile userProfile = new UserProfile(_db.EPSession);

            if (userProfile.Fetch(userId)) {

                int pwdLen = userProfile.AccountProfile.MinPasswordLength;
                pwdLen = (pwdLen == 0 ? 8 : pwdLen);

                string newPwd = userProfile.GetRandomPassword(pwdLen, true);

                if (userProfile.ResetPassword(newPwd)) {
                    if (userProfile.Update() != 0) {
                        message = "Error updating password record";
                    }
                    else {
                        message = NotifyUser(userId, newPwd);
                    }
                }
                else {
                    message = userProfile.LastErrorMessage;
                }
            }
            else {
                message = "User name not found";
            }

            return message;
        }

        private string NotifyUser(string userId, string newPwd) {

            string errMsg = "Password reset sucessfully. ";

            try {
                string empeMail = _userProfileRepo.GetUserEmail(userId);

                if (!string.IsNullOrEmpty(empeMail)) {
                    errMsg += SendMail(userId, empeMail, newPwd);
                }
                else {
                    errMsg += "Notification cannot be sent to user as e-mail address not found";
                }
            }
            catch (Exception ex) {
                errMsg += "Error in notifing user : " + ex.Message;
            }

            return errMsg;
        }

        private string SendMail(string userId, string empeMail, string newPwd) {
            string errMsg = "", smtpServer, from, body, subject, bodyTemplate, url, SMTPAuthAccessKey, SMTPAuthAccessDomain;
            bool SMTPAuthEnabled, SMTPCustomPortEnabled;
            int SMTPCustomPortNumber;

            EpfMail userMail = new EpfMail();

            try {

                smtpServer = System.Configuration.ConfigurationManager.AppSettings["SC_SMTP_SERVER"].ToStr();
                from = System.Configuration.ConfigurationManager.AppSettings["SC_MAIL_FROM"].ToStr();
                subject = System.Configuration.ConfigurationManager.AppSettings["SC_MAIL_SUBJECT"].ToStr();
                bodyTemplate = System.Configuration.ConfigurationManager.AppSettings["SC_MAIL_TEMPLATE"].ToStr();
                url = System.Configuration.ConfigurationManager.AppSettings["SC_MAIL_ESSURL"].ToStr();
                SMTPAuthEnabled = System.Configuration.ConfigurationManager.AppSettings["SMTPAuthEnabled"].ToBool();
                SMTPAuthAccessKey = System.Configuration.ConfigurationManager.AppSettings["SMTPAuthAccessKey"].ToStr();
                SMTPAuthAccessDomain = System.Configuration.ConfigurationManager.AppSettings["SMTPAuthAccessDomain"].ToStr();
                SMTPCustomPortEnabled = System.Configuration.ConfigurationManager.AppSettings["SMTPCustomPortEnabled"].ToBool();
                SMTPCustomPortNumber = System.Configuration.ConfigurationManager.AppSettings["SMTPCustomPortNumber"].ToInt();

                _userProfileRepo.ParseEmailUrl(ref url, _db.EPSession.CoyID);

                body = "<p>This is to inform you that your password to the <a href=\"{URL}\">Employee Self Service System</a> is created/reset by the administrator.</p>"
                    + "<BR/><BR/>"
                    + "<p>Login Id : <b>{USER_ID}</b></p>"
                    + "<p>Password : <b>{NEW_PWD}</b></p>"
                    + "<BR/><BR/>"
                    + "<p>Please change your password immediately for security reasons.</p>"
                    + "<BR/><BR/><BR/>"
                    + "<p>This email is automatically generated by user administration. Please do NOT reply.</p>";


                body = body.Replace("{URL}", url);
                body = body.Replace("{USER_ID}", userId);
                body = body.Replace("{NEW_PWD}", newPwd);

                userMail.SmtpServer = smtpServer;
                userMail.To = empeMail;
                userMail.From = from;
                userMail.Subject = subject;
                userMail.Body = body;

                userMail.AuthenticationEnabled = (SMTPAuthEnabled && !String.IsNullOrEmpty(SMTPAuthAccessKey));
                if (userMail.AuthenticationEnabled) {
                    try {
                        string key = SMTPAuthAccessKey.Trim(new Char[] { ';', ' ' });
                        if (key.Contains(";")) {
                            string[] parts = key.Split(new Char[] { Char.Parse(";") }, StringSplitOptions.RemoveEmptyEntries);
                            userMail.AuthorizedUserId = parts[0];
                            userMail.AuthorizedPassword = parts[1];
                        }
                        else {
                            if (!key.ToLower().StartsWith("encid=")) {
                                key = "EncID=" + key;
                            }
                            key = ePlatform.CommonClasses.EPSession.DecodeConnectString(key);

                            foreach (string part in key.Split(Char.Parse(";"))) {
                                if (part.ToLower().StartsWith("user id=")) {
                                    userMail.AuthorizedUserId = part.Substring(part.IndexOf("=") + 1);
                                }
                                if (part.ToLower().StartsWith("password=")) {
                                    userMail.AuthorizedPassword = part.Substring(part.IndexOf("=") + 1);
                                }
                            }
                        }
                        userMail.AuthorizedDomain = SMTPAuthAccessDomain;
                    }
                    catch (Exception) {
                        userMail.AuthenticationEnabled = false;
                    }
                }

                //smtpcustomport
                userMail.SmtpCustomPortEnabled = (SMTPCustomPortEnabled && SMTPCustomPortNumber != 0);
                if (userMail.SmtpCustomPortEnabled) {
                    userMail.SmtpPort = SMTPCustomPortNumber;
                }

                if (!userMail.Send()) {
                    errMsg = "Error in sending mail to " + empeMail + ".";
                }
            }
            catch (Exception ex) {
                errMsg = "Error in sending mail to " + empeMail + " : " + ex.Message;
            }

            return errMsg;
        }




    }
}
