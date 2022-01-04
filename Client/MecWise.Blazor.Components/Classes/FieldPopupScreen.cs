using System;
using System.Collections;
using System.Collections.Generic;
using MecWise.Blazor.Common;
using Newtonsoft.Json.Linq;
using MecWise.Blazor.Entities;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Web;

namespace MecWise.Blazor.Components
{

    public class FieldPopupScreen : Field
    {

        public Action CallBackFunction { get; set; }
        public Screen CallerScreen { get; set; }
        public string Title { get; set; } = "Popup Screen";
        public string CallerScreenId { get; set; }
        public string URL { get; set; } = string.Empty;
        public string ScreenId { get; set; }
        public ScreenMode Mode { get; set; } = ScreenMode.Enquiry;
        public string Keys { get; set; } = string.Empty;
        public string ParentKeys { get; set; } = string.Empty;
        public string Param { get; set; } = string.Empty;
        public bool ReloadParentOnClose { get; set; } = true;
        public int PopupSreenWidth { get; set; } = 0;

        public event EventHandler<EventArgs> OnClose;
        internal void InvokeOnClose(object sender, EventArgs e) {
            EventHandler<EventArgs> handler = OnClose;
            if (handler != null)
                handler(sender, e);
        }

        public FieldPopupScreen(Screen callerScreen) : base()
        {
            CallerScreen = callerScreen;
        }

        public FieldPopupScreen(string id, string description, Screen callerScreen) : base(id, description)
        {
            CallerScreen = callerScreen;
        }

        public FieldPopupScreen(string id, string description, string styleClass, Screen callerScreen) : base(id, description, styleClass)
        {
            CallerScreen = callerScreen;
        }

        public async Task Show(SessionState session, NavigationManager NavigationManager) {
            this.Render = true;
            string url = "";
            if (string.IsNullOrEmpty(this.URL)) {
                url = BuildUri(NavigationManager);
            }
            else {
                url = BuildUri(NavigationManager, this.URL);
            }

            if (string.IsNullOrEmpty(url)) {
                url = NavigationManager.BaseUri + "Blank"; ;
            }

            await session.ExecJSAsync("_showPopupScreen", this.ElementID, this.Title, this.PopupSreenWidth);

            await session.ExecJSAsync("_navigatePopupTo", url);
        }

        public async Task Hide(SessionState session, NavigationManager NavigationManager) {
            this.ScreenId = "";
            this.URL = "";
            this.Render = false;
            string url = NavigationManager.BaseUri + "/Blank";
            await session.ExecJSAsync("_closeBoostrapModel", this.ElementID);
            await session.ExecJSAsync("_navigatePopupTo", url);
            this.InvokeOnClose(this, new EventArgs());
        }

        public string BuildUri(NavigationManager NavigationManager) {

            if (string.IsNullOrEmpty(this.ScreenId)) {
                return string.Empty;
            }

            Uri newUri = new Uri(new Uri(NavigationManager.BaseUri), string.Format("EpfScreen/{0}", this.ScreenId));
            var uriBuilder = new UriBuilder(newUri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            //check additional existing param from current query and forward
            Uri currUri = new Uri(NavigationManager.Uri);
            var currParam = HttpUtility.ParseQueryString(currUri.Query);
            foreach (var key in currParam.Keys) {
                if (!key.ToStr().In("MODE", "KEYS", "PARENT_KEYS", "PARAM", "RETURN")) {
                    query[key.ToStr()] = currParam.Get(key.ToStr());
                }
            }

            query["MODE"] = this.Mode.ToInt().ToStr();
            query["KEYS"] = this.Keys;
            query["PARENT_KEYS"] = this.ParentKeys;
            query["PARAM"] = this.Param;
            query["CALLER_SCRN_ID"] = this.CallerScreenId;
            query["SCRN_TYPE"] = EpfScreen.ScreenType.Popup.ToInt().ToStr();
            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri.ToString();
        }

        public string BuildUri(NavigationManager NavigationManager, string uri) {

            if (string.IsNullOrEmpty(uri)) {
                return string.Empty;
            }

            Uri newUri;
            if (!uri.StartsWith("http")) {
                newUri = new Uri(new Uri(NavigationManager.BaseUri), uri);
            }
            else {
                newUri = new Uri(uri);
            }
            
            var uriBuilder = new UriBuilder(newUri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query["CALLER_SCRN_ID"] = this.CallerScreenId;
            query["SCRN_TYPE"] = EpfScreen.ScreenType.Popup.ToInt().ToStr();
            uriBuilder.Query = query.ToString();

            return uriBuilder.Uri.ToString();
        }

    }
}