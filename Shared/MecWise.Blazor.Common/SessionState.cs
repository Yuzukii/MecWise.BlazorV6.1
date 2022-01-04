using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using System.Linq;
using System.Collections;
using System.Data;
using System.Web;

namespace MecWise.Blazor.Common {

    public enum LoginType {
        normal,
        tokenAccess,
        byPassTokenAccess,
        oidcTokenAccess    //for Azure AD SSO
    }

    public enum ToastMessageType {
        error,
        info,
        success,
        warning
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ToastMessageLocation
    {
        top,
        left,
        bottom,
        right
    }

    public class ToastMessagePosition {
        public ToastMessageLocation at { get; set; } = ToastMessageLocation.bottom;
        public int offsetLeft { get; set; } = 0;
        public int offsetTop { get; set; } = -30;
        public string offset {
            get {
                return string.Format("{0} {1}", offsetLeft, offsetTop);
            }
        }
    }

    public class ToastMessageOption {
        public string message { get; set; } = "";
        public string width { get; set; } = "95%";
        public bool shading { get; set; } = false;
        public bool closeOnOutsideClick { get; set; } = true;
        public bool closeOnClick { get; set; } = true;
        public ToastMessagePosition position { get; set; } = new ToastMessagePosition();
    }

    public class SessionState {

        private const string _showLoading = "MecWise.Blazor.Components.EpfScreen.ShowLoading";

        private string _userId = "";
        private string _empeId = "";
        private string _compCode = "";
        private string _activeScrnId = "";
        private int _langId = 0;
        private bool _isAdmin = false;
        private bool _rememberUser = false;

        private Token _token = null;

        public JObject ScreenSize;
        public Config AppConfig;

        public Token AccessToken { get { return _token; } set { _token = value; } }
        public string ActiveScrnId { get { return _activeScrnId; } 
            set {
                _activeScrnId = value;
                if (this.JSRuntime != null) {
                    this.ExecJS("_setActiveScreenId", _activeScrnId);
                }
            } 
        }
        public string UserID { get {
                return this._userId;
            }
            set {
                this._userId = value.ToStr();
            }
        }
        public string EmpeID { 
            get {
                return this._empeId;
            } 
            set {
                this._empeId = value.ToStr();
            } 
        }
        public string CompCode {
            get {
                return this._compCode;
            }
            set {
                this._compCode = value.ToStr();
            }
        }
        public bool IsAdmin {
            get {
                return this._isAdmin;
            }
            set {
                this._isAdmin = value;
            }
        }
        public bool RememberUser {
            get {
                return this._rememberUser;
            }
            set {
                this._rememberUser = value;
            }
        }
        public int LangId {
            get {
                return this._langId;
            }
            set {
                this._langId = value;
            }
        }
        public Dictionary<string, object> Items { get; set; } = new Dictionary<string, object>();

        [JsonProperty]
        private Stack<string> NavigationHistory = new Stack<string>();

        public bool Loading {
            get {
                if (this.Items.Keys.Contains(_showLoading)) {
                    return this.Items[_showLoading].ToBool();
                }
                else {
                    return true; // return true as default
                }
            }
            set {
                if (this.Items.Keys.Contains(_showLoading)) {
                    this.Items[_showLoading] = value;
                }
                else {
                    this.Items.Add(_showLoading, value);
                }
            }
        }

        [JsonIgnore]
        public IJSRuntime JSRuntime;

        public SessionState() { }

        public void ClearNavigationHistory() {
            this.NavigationHistory.Clear();
            this.SaveSession();
        }

        public string PeekNavigationHistory() {
            return this.NavigationHistory.Peek();
        }

        public int GetNavigationHistoryCount() {
            return this.NavigationHistory.Count;
        }

        public string PopNavigationHistory() {
            string popValue = this.NavigationHistory.Pop();
            this.SaveSession();

            return popValue;
        }

        public void PushNavigationHistory(string baseUri, string uri) {
            uri = uri.Replace(baseUri, "");

            if (this.NavigationHistory.Count > 0) {
                if (uri == this.NavigationHistory.Peek()) {
                    return;
                }

            }

            this.NavigationHistory.Push(uri);
            this.SaveSession();

        }

        public async void InitConfig(IJSRuntime jsRunTime, Config config) {
            this.JSRuntime = jsRunTime;
            
            // handle apiUrl without '/' at the end.
            config.apiUrl = config.apiUrl.TrimEnd('/') + "/";
            this.AppConfig = config;
            this.ScreenSize = JObject.Parse((await JSRuntime.InvokeAsync<object>("GetScreenSize")).ToString());

        }

        public async Task<bool> InitSavedSession() {
            string sessionKey = await GetSessionKeyAsync();
            string savedSession = await this.GetLocalStorageItem(sessionKey);
            if (string.IsNullOrEmpty(savedSession)) {
                return false;
            }

            JObject jSession = JObject.Parse(savedSession);
            if (jSession["AccessToken"]["ExpiredTime"].ToDate() < DateTime.UtcNow) {
                return false;
            }

            this.AppConfig.MobileNavButtons.Clear();
            JsonConvert.PopulateObject(savedSession, this);

            IEnumerable<string> reverseList = new List<string>(this.NavigationHistory);
            reverseList.Reverse();

            this.NavigationHistory = new Stack<string>(reverseList); // reverse the NavigationHistory stack

            return true;
        }

        public async Task<bool> ConfirmMessageAsync(string title, string confirmText, string btnOkText, string btnCancelText)
        {
            return await JSRuntime.InvokeAsync<bool>("ModalDialogConfirm", title, confirmText, btnOkText, btnCancelText);
        }
        public async Task<bool> ConfirmMessageAsync(string title, string confirmText)
        {
            return await ConfirmMessageAsync(title, confirmText, "Ok", "Cancel");
        }
        public async Task<bool> ConfirmMessageAsync(string confirmText) {
            return await ConfirmMessageAsync("Confirm", confirmText, "Ok", "Cancel");
        }

        public async void ShowMessage(string title, string messageText)
        {
            await JSRuntime.InvokeAsync<object>("ModalDialogAlert", title, messageText);
        }
        public void ShowMessage(string messageText)
        {
            ShowMessage("Info", messageText);
        }


        public async Task<string> PromptMessageAsync(string promtText) {
            return await JSRuntime.InvokeAsync<string>("PromtMessage", promtText, "");
        }

        public async Task<string> PromptMessageAsync(string promtText, string defaultValue)
        {
            return await JSRuntime.InvokeAsync<string>("PromtMessage", promtText, defaultValue);
        }

        public async Task<object> ExecJSAsync(string functionName, params object[] parameters) {
            return await JSRuntime.InvokeAsync<object>(functionName, parameters);
        }
         
        public async Task<T> ExecJSAsync<T>(string functionName, params object[] parameters)
        {
            return await JSRuntime.InvokeAsync<T>(functionName, parameters);
        }

        public object ExecJS(string functionName, params object[] parameters)
        {
            //return ((IJSInProcessRuntime)JSRuntime).Invoke<object>(functionName, parameters); 
            return JSRuntime.InvokeAsync<object>(functionName, parameters);
        }

        public async void ToastMessage(string messageText, ToastMessageType type) {

            ToastMessageOption option = new ToastMessageOption();
            option.position.at = AppConfig.toastMessagePosition;
            string browser = await this.ExecJSAsync<string>("GetBrowserType");
            if (option.position.at == ToastMessageLocation.top) {
                bool InIframe = await this.ExecJSAsync<bool>("iniFrame");
                if (browser == "mobile-chrome") {
                    if (InIframe) {
                        option.position.offsetTop = 60;
                    }
                    else {
                        option.position.offsetTop = 145;
                    }
                }
                else {
                    if (InIframe) {
                        option.position.offsetTop = 60;
                    }
                    else {
                        option.position.offsetTop = 100;
                    }
                }
                
            }

            if (option.position.at == ToastMessageLocation.left) {
                option.position.offsetLeft = 410;
            }

            if (option.position.at == ToastMessageLocation.right) {
                option.position.offsetLeft = -410;
            }

            option.message = messageText;
            this.ToastMessage(option, type);
        }

        public async void ToastMessage(ToastMessageOption option, ToastMessageType type)
        {
            int displayTime = AppConfig.toastMessageDelay * 1000; // calculate for milliseconds
            await JSRuntime.InvokeAsync<object>("ToastMessage", JObject.FromObject(option).ToStr(), type.ToStr(), displayTime);
        }

        public async Task SetStorageItemAsync(string key, string data) {
            await JSRuntime.InvokeAsync<object>("SetStorageItem", key, data);
        }

        public async Task<string> GetStorageItemAsync(string key) {
            return await JSRuntime.InvokeAsync<string>("GetStorageItem", key);
        }

        public async Task RemoveStorageItemAsync(string key)
        {
            await JSRuntime.InvokeAsync<object>("RemoveStorageItem", key);
        }

        public async Task<string> GetLocalStorageItem(string key)
        {
            return await JSRuntime.InvokeAsync<string>("GetLocalStorageItem", key);
        }
        public async Task SetLocalStorageItem(string key, string data)
        {
            await JSRuntime.InvokeAsync<object>("SetLocalStorageItem", key, data);
        }

        public async Task RemoveLocalStorageItem(string key)
        {
            await JSRuntime.InvokeAsync<object>("RemoveLocalStorageItem", key);
        }

        public async void NavigateWithoutHistory(string url) {
            await JSRuntime.InvokeAsync<object>("NavigateWithoutHistory", url);
        }

        public async Task<string> GetSessionKeyAsync() {
            return await JSRuntime.InvokeAsync<string>("GetSessionKey");
        }

        public string GetAbsoluteApiUrl(string url) {
            return string.Format("{0}/{1}", this.AppConfig.apiUrl, url);
        }

        public async Task<string> GetGeoLocationAsync()
        {
            return await JSRuntime.InvokeAsync<string>("GetGeoLocation");
        }

        public async Task<string> GetGeoCoordinatesAsync() {
            return await JSRuntime.InvokeAsync<string>("GetGeoCoordinates");
        }

        public async void InitAPIService() {
            var client = new HttpClient();
            var response = await client.GetAsync(this.AppConfig.apiUrl);
            if (response.StatusCode == HttpStatusCode.OK) {
                string content = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(content);
            }
            else {
                Console.WriteLine("Unable to contact service...");
            }
        }

        public async Task<bool> LoginAsync(string userId, string password) {
            _token = new Token();
            _token.LastRequestTime = DateTime.UtcNow;
            var client = new HttpClient();

            var authorizationHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", this.AppConfig.clientId, this.AppConfig.clientSecret)));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationHeader);
            var form = new Dictionary<string, string>
                {
                        {"grant_type", "password"},
                        {"login_type", LoginType.normal.ToStr() },
                        {"username", userId},
                        {"password", password},
                    };

            string accessTokenUrl = new Uri(new Uri(this.AppConfig.apiUrl), "accesstoken").ToString();
            var tokenResponse = await client.PostAsync(accessTokenUrl, new FormUrlEncodedContent(form));
            if (tokenResponse.StatusCode == HttpStatusCode.OK) {
                var contentString = await tokenResponse.Content.ReadAsStringAsync();
                JsonConvert.PopulateObject(contentString, _token);
            }

            if (!string.IsNullOrEmpty(_token.access_token))
                return true;
            else
                return false;
        }

        //private async Task<bool> RequestAccessTokenAsync(string userId, string password) {
        //    if (_token == null || _token.access_token == string.Empty) {
        //        _token = new Token();
        //        _token.LastRequestTime = DateTime.UtcNow;
        //        var client = new HttpClient();

        //        var authorizationHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", this.AppConfig.clientId, this.AppConfig.clientSecret)));
                
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationHeader);
        //        //client.DefaultRequestHeaders.Add("origin", "http://localhost:50341/");
        //        var form = new Dictionary<string, string>
        //            {
        //                {"grant_type", "password"},
        //                {"login_type", LoginType.normal.ToStr() },
        //                {"username", userId},
        //                {"password", password},
        //            };

        //        string accessTokenUrl = new Uri(new Uri(this.AppConfig.apiUrl), "accesstoken").ToString();
        //        var tokenResponse = await client.PostAsync(accessTokenUrl, new FormUrlEncodedContent(form));
        //        if (tokenResponse.StatusCode == HttpStatusCode.OK) {
        //            var contentString = await tokenResponse.Content.ReadAsStringAsync();
        //            JsonConvert.PopulateObject(contentString, _token);
        //        }
        //    }

        //    if (!string.IsNullOrEmpty(_token.access_token))
        //        return true;
        //    else
        //        return false;
        //}

        //public Task<bool> AutoLoginAsync(string tokenStr)
        //{
        //    _token = null;
        //    return RequestAcessTokenAsync(tokenStr);
        //}

        //private async Task<bool> RequestAcessTokenAsync(string tokenStr)
        //{
        //    if (_token == null || _token.access_token == string.Empty)
        //    {
        //        _token = new Token();
        //        _token.LastRequestTime = DateTime.UtcNow;
        //        var client = new HttpClient();

        //        var authorizationHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", this.AppConfig.clientId, this.AppConfig.clientSecret)));

        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationHeader);
        //        //client.DefaultRequestHeaders.Add("origin", "http://localhost:50341/");
        //        var form = new Dictionary<string, string>
        //            {
        //                {"grant_type", "password"},
        //                {"login_type", "auto" },
        //                {"username", tokenStr},
        //            };

        //        string accessTokenUrl = new Uri(new Uri(this.AppConfig.apiUrl), "accesstoken").ToString();
        //        var tokenResponse = await client.PostAsync(accessTokenUrl, new FormUrlEncodedContent(form));
        //        if (tokenResponse.StatusCode == HttpStatusCode.OK) {
        //            var contentString = await tokenResponse.Content.ReadAsStringAsync();
        //            JsonConvert.PopulateObject(contentString, _token);
        //        }
        //    }

        //    if (!string.IsNullOrEmpty(_token.access_token))
        //        return true;
        //    else
        //        return false;
        //}

        public async Task<JObject> LoginWithTokenAsync(string tokenStr, LoginType tokenType) {
            _token = new Token();
            _token.LastRequestTime = DateTime.UtcNow;
            var client = new HttpClient();

            var authorizationHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", this.AppConfig.clientId, this.AppConfig.clientSecret)));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationHeader);
            var form = new Dictionary<string, string>
                {
                        {"grant_type", "password"},
                        {"login_type", tokenType.ToStr()},
                        {"username", tokenStr},
                    };

            string accessTokenUrl = new Uri(new Uri(this.AppConfig.apiUrl), "accesstoken").ToString();
            var tokenResponse = await client.PostAsync(accessTokenUrl, new FormUrlEncodedContent(form));
            if (tokenResponse.StatusCode == HttpStatusCode.OK) {
                var contentString = await tokenResponse.Content.ReadAsStringAsync();
                JsonConvert.PopulateObject(contentString, _token);
            }

            if (!string.IsNullOrEmpty(_token.access_token)) {
                string url = string.Format("Login/CheckToken");
                JObject postData = new JObject();
                postData.Add("token", tokenStr);
                postData.Add("tokenType", tokenType.ToInt());
                HttpResponseMessage response = new HttpResponseMessage();
                response = await this.PostAsync(this.GetAbsoluteApiUrl(url), postData);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                    string retVal = await response.Content.ReadAsStringAsync();
                    JObject retObj = JObject.Parse(retVal);
                    return retObj;
                }
            }

            return null;
        }


        private async Task<bool> RefreshAccessToken() {
            bool isiniFrame = await JSRuntime.InvokeAsync<bool>("iniFrame");

            if (_token == null) {
                return false;
            }

            if (this.AppConfig.clientId != string.Empty && _token.refresh_token != string.Empty && _token.ExpiredTime.AddMinutes(-5) <= DateTime.UtcNow) {
                var client = new HttpClient();

                var authorizationHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", this.AppConfig.clientId, this.AppConfig.clientSecret)));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationHeader);
                var form = new Dictionary<string, string>
                {
                    {"grant_type", "refresh_token"},
                    {"refresh_token", _token.refresh_token}
                };

                string accessTokenUrl = new Uri(new Uri(this.AppConfig.apiUrl), "accesstoken").ToString();
                var tokenResponse = await client.PostAsync(accessTokenUrl, new FormUrlEncodedContent(form));
                if (tokenResponse.StatusCode == HttpStatusCode.OK) {
                    var contentString = await tokenResponse.Content.ReadAsStringAsync();
                    JsonConvert.PopulateObject(contentString, _token);
                    _token.LastRequestTime = DateTime.UtcNow;
                    SaveSession();
                }
            }

            return true;
        }

        private async void SaveSession() {
            string sessionKey = await GetSessionKeyAsync();
            await this.SetLocalStorageItem(sessionKey, JObject.FromObject(this).ToString());
        }

        public string GetRequestHeaderSession() {
            JObject result = new JObject();

            result.Add("UserId", _userId);
            result.Add("CompCode", _compCode);
            result.Add("EmpeId", _empeId);
            result.Add("LangId", _langId);
            result.Add("ActiveScrnId", _activeScrnId);

            string formattedString = result.ToString().Replace("\n", " ").Replace(" ", "");
            string base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes(formattedString));

            return base64String;
        }

        public async Task<T> PostJsonAsync<T>(string url, object data) {
            // Request Access Token 
            await RefreshAccessToken();

            string resultContent;

            using (var client = new HttpClient()) {
                string apiUrl = new Uri(new Uri(this.AppConfig.apiUrl), url).OriginalString;
                string jsonString = JsonConvert.SerializeObject(data);

                if (_token != null) client.DefaultRequestHeaders.Add("Authorization", String.Format("{0} {1}", _token.token_type, _token.access_token));
                client.DefaultRequestHeaders.Add("Session", GetRequestHeaderSession());

                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                using (var result = await client.PostAsync(apiUrl, content)) {
                    resultContent = await result.Content.ReadAsStringAsync();
                    if (result.StatusCode != HttpStatusCode.OK) {
                        Console.WriteLine(resultContent);
                        if (result.StatusCode == HttpStatusCode.Unauthorized) {
                            await this.ExecJSAsync("NavigateWithoutHistory", "");
                        }
                    }

                }
            }

            return JsonConvert.DeserializeObject<T>(resultContent);
        }

        public async Task<HttpResponseMessage> PostAsync(string url, object data) {
            // Request Access Token 
            await RefreshAccessToken();

            HttpResponseMessage result;
            string resultContent;
            using (var client = new HttpClient()) {
                string apiUrl = new Uri(new Uri(this.AppConfig.apiUrl), url).OriginalString;
                string jsonString = JsonConvert.SerializeObject(data);
                if (_token != null) client.DefaultRequestHeaders.Add("Authorization", String.Format("{0} {1}", _token.token_type, _token.access_token));
                client.DefaultRequestHeaders.Add("Session", GetRequestHeaderSession());
                
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                result = await client.PostAsync(apiUrl, content);
                resultContent = await result.Content.ReadAsStringAsync();
                if (result.StatusCode != HttpStatusCode.OK) {
                    Console.WriteLine(resultContent);
                    if (result.StatusCode == HttpStatusCode.Unauthorized) {
                        await this.ExecJSAsync("NavigateWithoutHistory", "");
                    }
                }
            }
                
            return result;

        }

        public async Task<T> PutJsonAsync<T>(string url, object dataObject) {
            // Request Access Token 
            await RefreshAccessToken();

            string resultContent;
            using (var client = new HttpClient()) {
                string apiUrl = new Uri(new Uri(this.AppConfig.apiUrl), url).OriginalString;
                string jsonString = JsonConvert.SerializeObject(dataObject);

                if (_token != null) client.DefaultRequestHeaders.Add("Authorization", String.Format("{0} {1}", _token.token_type, _token.access_token));
                client.DefaultRequestHeaders.Add("Session", GetRequestHeaderSession());
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var result = await client.PutAsync(apiUrl, content);
                resultContent = await result.Content.ReadAsStringAsync();
                if (result.StatusCode != HttpStatusCode.OK) {
                    Console.WriteLine(resultContent);
                    if (result.StatusCode == HttpStatusCode.Unauthorized) {
                        await this.ExecJSAsync("NavigateWithoutHistory", "");
                    }
                }
            }


            return JsonConvert.DeserializeObject<T>(resultContent);
        }

        public async Task<HttpResponseMessage> PutAsync(string url, object dataObject) {
            // Request Access Token 
            await RefreshAccessToken();

            HttpResponseMessage result;
            string resultContent;
            using (var client = new HttpClient()) {
                string apiUrl = new Uri(new Uri(this.AppConfig.apiUrl), url).OriginalString;
                string jsonString = JsonConvert.SerializeObject(dataObject);

                if (_token != null) client.DefaultRequestHeaders.Add("Authorization", String.Format("{0} {1}", _token.token_type, _token.access_token));
                client.DefaultRequestHeaders.Add("Session", GetRequestHeaderSession());
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                result = await client.PutAsync(apiUrl, content);
                resultContent = await result.Content.ReadAsStringAsync();
                if (result.StatusCode != HttpStatusCode.OK) {
                    Console.WriteLine(resultContent);
                    if (result.StatusCode == HttpStatusCode.Unauthorized) {
                        await this.ExecJSAsync("NavigateWithoutHistory", "");
                    }
                }
            }
            return result;

        }

        public async Task<T> GetJsonAsync<T>(string url) {
            // Request Access Token 
            await RefreshAccessToken();
            
            string retVal = "";
            using (var client = new HttpClient()) {
                string apiUrl = new Uri(new Uri(this.AppConfig.apiUrl), url).OriginalString;
                if (_token != null) client.DefaultRequestHeaders.Add("Authorization", String.Format("{0} {1}", _token.token_type, _token.access_token));
                client.DefaultRequestHeaders.Add("Session", GetRequestHeaderSession());
                var result = await client.GetAsync(apiUrl);
                
                string resultContent = await result.Content.ReadAsStringAsync();
                retVal = resultContent;
                if (result.StatusCode != HttpStatusCode.OK) {
                    Console.WriteLine(resultContent);
                    if (result.StatusCode == HttpStatusCode.Unauthorized) {
                        await this.ExecJSAsync("NavigateWithoutHistory", "");
                    }
                }
            }
           
            return JsonConvert.DeserializeObject<T>(retVal);
        }

        public async Task<HttpResponseMessage> GetAsync(string url) {
            // Request Access Token 
            await RefreshAccessToken();

            HttpResponseMessage result;
            using (var client = new HttpClient()) {
                string apiUrl = new Uri(new Uri(this.AppConfig.apiUrl), url).OriginalString;
                if (_token != null) client.DefaultRequestHeaders.Add("Authorization", String.Format("{0} {1}", _token.token_type, _token.access_token));
                client.DefaultRequestHeaders.Add("Session", GetRequestHeaderSession());
                result = await client.GetAsync(apiUrl);
                string resultContent = await result.Content.ReadAsStringAsync();
                if (result.StatusCode != HttpStatusCode.OK) {
                    Console.WriteLine(resultContent);
                    if (result.StatusCode == HttpStatusCode.Unauthorized) {
                        await this.ExecJSAsync("NavigateWithoutHistory", "");
                    }
                }
            }
            return result;
        }

        public async Task<T> DeleteJsonAsync<T>(string url) {
            // Request Access Token 
            await RefreshAccessToken();

            string retVal = "";
            using (var client = new HttpClient()) {
                string apiUrl = new Uri(new Uri(this.AppConfig.apiUrl), url).OriginalString;
                if (_token != null) client.DefaultRequestHeaders.Add("Authorization", String.Format("{0} {1}", _token.token_type, _token.access_token));
                client.DefaultRequestHeaders.Add("Session", GetRequestHeaderSession());
                var result = await client.DeleteAsync(apiUrl);
                string resultContent = await result.Content.ReadAsStringAsync();
                retVal = resultContent;
                if (result.StatusCode != HttpStatusCode.OK) {
                    Console.WriteLine(resultContent);
                    if (result.StatusCode == HttpStatusCode.Unauthorized) {
                        await this.ExecJSAsync("NavigateWithoutHistory", "");
                    }
                }
            }
            return JsonConvert.DeserializeObject<T>(retVal);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string url) {
            // Request Access Token 
            await RefreshAccessToken();

            HttpResponseMessage result;
            using (var client = new HttpClient()) {
                string apiUrl = new Uri(new Uri(this.AppConfig.apiUrl), url).OriginalString;
                if (_token != null) client.DefaultRequestHeaders.Add("Authorization", String.Format("{0} {1}", _token.token_type, _token.access_token));
                client.DefaultRequestHeaders.Add("Session", GetRequestHeaderSession());
                result = await client.DeleteAsync(apiUrl);
                string resultContent = await result.Content.ReadAsStringAsync();
                if (result.StatusCode != HttpStatusCode.OK) {
                    Console.WriteLine(resultContent);
                    if (result.StatusCode == HttpStatusCode.Unauthorized) {
                        await this.ExecJSAsync("NavigateWithoutHistory", "");
                    }
                }
            }
            return result;
        }

        public bool IsMobileScreen() {
            if (this.ScreenSize["screenWidth"].ToInt() < 768)
                return true;
            else
                return false;
        }

        public async Task<object> ExecServerFuncAsync(string assemblyName, string className, string methodName, params object[] parameters) {
            ServerFunction func = new ServerFunction(assemblyName, className, methodName, parameters);
            return await this.PostJsonAsync<object>("EpfScreen/ExecServerFunction", func);
        }

        public async Task<T> ExecServerFuncAsync<T>(string assemblyName, string className, string methodName, params object[] parameters)
        {
            ServerFunction func = new ServerFunction(assemblyName, className, methodName, parameters);
            return await this.PostJsonAsync<T>("EpfScreen/ExecServerFunction", func);
        }

        public async Task NavigateV5URLAsync(string baseUri, string relativeURL, bool optPopup, bool adminRole) {
            await NavigateV5URLAsync(baseUri, relativeURL, optPopup, adminRole, false);
        }

        public async Task NavigateV5URLAsync(string baseUri, string relativeURL, Boolean optPopup) {
            await NavigateV5URLAsync(baseUri, relativeURL, optPopup, this.IsAdmin, false);
        }

        public async Task NavigateV5URLAsync(string baseUri, string relativeURL, bool optPopup, bool adminRole, bool currentWindow) {

            string url = await this.GetV5URLAsync(baseUri, relativeURL, adminRole);
            if (!string.IsNullOrEmpty(url)) {
                if (currentWindow) {
                    await this.ExecJSAsync("NavigateWithoutHistory", url);
                }
                else {
                    await this.ExecJSAsync("_navigateURL", url, optPopup);
                }
            }

        }


        public async Task<string> GetV5URLAsync(string baseUri, string relativeURL, bool adminRole) {
            if (!String.IsNullOrEmpty(relativeURL) && !String.IsNullOrEmpty(this.AppConfig.v5ConnectRoot)) {
                string apiurl = string.Format("Common/GetV5NavigationKey");
                JObject postData = new JObject();
                postData.Add("connectId", this.AppConfig.v5ConnectId);
                postData.Add("connectKey", this.AppConfig.v5ConnectKey);
                postData.Add("session", JObject.FromObject(this));
                postData.Add("relativeURL", relativeURL);
                
                if (adminRole) {
                    postData.Add("userRole", "1");
                }
                else {
                    postData.Add("userRole", "0");
                }

                JObject objData = await this.PostJsonAsync<JObject>(apiurl, postData);
                if (objData != null && objData.ContainsKey("encryptedKey")) {
                    string encryptedKey = objData["encryptedKey"].ToString();

                    List<KeyValuePair<string, string>> param = new List<KeyValuePair<string, string>>();
                    param.Add(new KeyValuePair<string, string>("key", encryptedKey));

                    Uri uri = new Uri(this.AppConfig.v5ConnectRoot);
                    uri = uri.Append("/Logon/EpfConnect.aspx");

                    string url = uri.ToString() + "?key=" + encryptedKey;

                    return url;
                }

                return string.Empty;
            }

            return string.Empty;
        }


        private string BuildUri(string baseUri, string uri, List<KeyValuePair<string, string>> parameters) {
            Uri newUri = new Uri(new Uri(baseUri), uri);
            var uriBuilder = new UriBuilder(newUri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            foreach (var param in parameters) {
                query[param.Key] = param.Value;
            }
            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri.ToString();
        }
    }

    

    public class QueryTemplate {
        public string SqlTemplate { get; set; }
        public object[] Parameters { get; set; }

    }

    public class ProcedureTemplate {

        public string ProcedureName { get; set; }
        public List<DbParameter> Params { get; set; } = new List<DbParameter>();
       
    }

    public class DbParameter : IDataParameter {
        public DbType DbType { get; set; }
        public ParameterDirection Direction { get; set; }
        public bool IsNullable => true;
        public string ParameterName { get; set; }
        public string SourceColumn { get; set; }
        public DataRowVersion SourceVersion { get; set; }
        public object Value { get; set; }

        public DbParameter() { }
        public DbParameter(string name, object value) {
            ParameterName = name;
            Value = value;
        }

        public DbParameter(string name, object value, System.Data.ParameterDirection direction) {
            ParameterName = name;
            Value = value;
            Direction = direction;
        }
    }




}

