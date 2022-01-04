using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;

//using MecWise.Blazor.Pages;
using MecWise.Blazor.Entities;
using MecWise.Blazor.Components;
using MecWise.Blazor.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.JSInterop;
using System.Security.Cryptography;

namespace MecWise.Blazor.Components {

    public class Screen : IDisposable {

        Dictionary<string, object> _params = new Dictionary<string, object>();
        string[] _keys = { };
        string[] _parentKeys = { };
        EPF_SCRN _epfScrnDef;
        FieldCollection _fields;
        JObject _dataSource;
        JObject _originalDataSource;
        Dictionary<string, JObject> _dataColumnInfos;

        EpfScreen _scrnComponent;
        SessionState _session;
        NavigationManager _navManager;
        Screen _parentScreen;
        FieldTitleBar _titleBar = null;
        FieldPopupScreen _popupScreen;
        
        bool _recordFetched = false;
        bool _isiniFrame = false;
        FieldContainer _mainContainer;
        internal FieldGridEditRow _gridEditingRow;
        internal bool _changeModeExecutedFlag = false;
        internal bool _noChangeModeFlag = false;
        internal List<ScreenMode> _editModes;
        int _nextRecOffset = 1;
        int _prevRecOffset = -1;
        JObject _nextRecKeys = null;
        JObject _prevRecKeys = null;

        public FieldGridEditRow EditRow { get { return _gridEditingRow; } }
        public bool IsPopupScreen { get { return _isiniFrame; } }
        public bool RecordFetched { get { return _recordFetched; } }
        public SessionState Session { get { return _session; } set { _session = value; } }
        
        public Dictionary<string, JObject> DataColumnInfos { get { return _dataColumnInfos; } set { _dataColumnInfos = value; } }
        public string ScrnId {
            get {
                if (_epfScrnDef != null) {
                    return _epfScrnDef.SCRN_ID;
                }
                else {
                    return "";
                }
            }
        }
        public string BaseUri { get { return _navManager.BaseUri; } }
        public JObject DataSource { get { return _dataSource; } set { _dataSource = value; } }

        public FieldCollection Fields { get { return _fields; } set { _fields = value; } }

        [JsonIgnore]
        public ScreenMode ScrnMode { get { return _scrnComponent.MODE; } }
        public string PARENT_KEYS { get { return _scrnComponent.PARENT_KEYS; } }
        public Screen ParentScreen { get { return _parentScreen; } }
        public Dictionary<string, object> PARAM { get { return _params; } }
        public string BASE_VIEW { get { return _epfScrnDef.BASE_VIEW; } }
        public string BASE_PROC { get { return _epfScrnDef.BASE_PROC; } }
        public string BASE_BRWS { get { return _epfScrnDef.BASE_BRWS; } }
        public string BASE_TBL { get { return _epfScrnDef.BASE_TBL; } }
        public string SCRN_ID { get { return _epfScrnDef.SCRN_ID; } }
        public string FIELD_KEYS { get { return _epfScrnDef.FIELD_KEYS; } }
        public EPF_SCRN SCRN_DEF { get { return _epfScrnDef; } }
        public EpfScreen.ScreenType SCRN_TYPE { get { return _scrnComponent.SCRN_TYPE; } }
        public string CALLER_SCRN_ID { get { return _scrnComponent.CALLER_SCRN_ID; } }
        public Screen CALLER_SCREEN { get { return _scrnComponent.CALLER_SCREEN; } }
        public string CURRENT_URL { get { return _scrnComponent.CURRENT_URL; } }
        public FieldContainer MainContainer { get { return _mainContainer; } }

        // when false, screen will display wihtout waiting AfterRenderAsync event
        bool _displayAfterRender = false;
        public bool DisplayAfterRender { 
            get {
                return _displayAfterRender;
            } set {
                // when _confirmOnNavigateRecord is true, then _displayAfterRender also must be true
                if (_confirmOnNavigateRecord) {
                    _displayAfterRender = true;
                }
                else {
                    _displayAfterRender = value;
                }
            } 
        }

        // when true, system will confirm to save before moving to another record NavigateRecordAsyn
        bool _confirmOnNavigateRecord = false;
        public bool ConfirmOnNavigateRecord {
            get {
                return _confirmOnNavigateRecord;
            }
            set {
                _confirmOnNavigateRecord = value;
                // when _confirmOnNavigateRecord is true, then _displayAfterRender also must be true
                if (_confirmOnNavigateRecord) {
                    _displayAfterRender = true;
                }
            }
        }

        // when true, system will navigate record by Move Next/Previous
        bool _navigateRecord = false;
        public bool NavigateRecord {
            get {
                return _navigateRecord;
            }
            set {
                _navigateRecord = value;
            }
        }
        public virtual void Dispose() {

        }

        [JSInvokable]
        public async Task<object> ScreenJSCallBack(string jsonEventData) {
            JObject jObj = JObject.Parse(jsonEventData);
            if (jObj.ContainsKey("EventType")) {
                if (jObj["EventType"].ToStr() == "OnScreenReady") {

                    _changeModeExecutedFlag = false;
                    if (jObj["ScrnLoad"].ToBool()) {
                        await this.LoadAsync();
                    }

                    if (!this.DisplayAfterRender) {
                        this.HideLoadPanel();
                    }

                    if (!_changeModeExecutedFlag) {
                        await this.ScreenShowRecAsync();
                    }
                }
                else if (jObj["EventType"].ToStr() == "OnGetFieldValue") {
                    string fieldId = jObj["FieldId"].ToStr();
                    return this.GetFieldValue(fieldId).ToStr();
                }
                else if (jObj["EventType"].ToStr() == "OnGetParentFieldValue") {
                    string fieldId = jObj["FieldId"].ToStr();
                    return this.GetParentFieldValue(fieldId).ToStr();
                }
                else if (jObj["EventType"].ToStr() == "OnSetFieldValue") {
                    string fieldId = jObj["FieldId"].ToStr();
                    string value = jObj["Value"].ToStr();
                    this.SetFieldValue(fieldId,value);
                }
                else if (jObj["EventType"].ToStr() == "OnSetParentFieldValue") {
                    string fieldId = jObj["FieldId"].ToStr();
                    string value = jObj["Value"].ToStr();
                    this.SetParentFieldValue(fieldId, value);
                }
                else if (jObj["EventType"].ToStr() == "OnSetFieldDisable") {
                    string fieldId = jObj["FieldId"].ToStr();
                    bool disable = jObj["Value"].ToBool();
                    this.SetFieldDisable(fieldId, disable);
                }
                else if (jObj["EventType"].ToStr() == "OnRefresh") {
                    this.Refresh();
                }
                else if (jObj["EventType"].ToStr() == "OnSetFieldFocus") {
                    string fieldId = jObj["FieldId"].ToStr();
                    this.SetFieldFocus(fieldId);
                }
                else if (jObj["EventType"].ToStr() == "OnOpenPopupScreen") {
                    JToken param = jObj["Value"];
                    await OpenPopupScreenAsync(param["screenId"].ToStr(), (ScreenMode)param["mode"].ToInt(),
                        param["targetKeys"].ToStr(), param["parentKeys"].ToStr(), param["parameters"].ToStr());
                }
            }

            return null;
        }

        public Screen() {
            _dataSource = new JObject();
        }

        internal async Task<bool> ScreenLoadAsync() {
            await this.LoadAsync();
            _scrnComponent.Refresh();

            return true;
        }

        internal async Task<bool> ScreenShowRecAsync() {
            return await ScreenShowRecAsync(null, true);
        }
        
        internal async Task<bool> ScreenShowRecAsync(JObject options, bool showLoading) {

            await this.ShowRecAsync();
            this.Refresh();

            //resize height of the popup before refreshing grid views
            await Session.ExecJSAsync("_resizePopupIframe");

            //[09/Jul/2021, PhyoZin] change to call RefreshGridViews instead of RefreshGridViewsAsync, do not wait for refreshing grid views
            //await this.RefreshGridViews();
            var _ = this.RefreshGridViewsAsync().ContinueWith(async x => {

                if (options != null) {
                    if (options.ContainsKey("detailGridAciton")) {
                        string gridId = options["detailGridAciton"]["gridId"].ToStr();
                        string action = options["detailGridAciton"]["action"].ToStr();
                        if (action == "addNew") {
                            await Task.Delay(400).ContinueWith(async x => {
                                await Session.ExecJSAsync("_dxdatagrid_add_row", this.SCRN_ID, gridId);
                            });
                        }
                    }
                }

                await this.ScreenAfterRenderAsync().ContinueWith(async x => {

                    //saving original data to warn user for saving when NavigateRecordAsync
                    if (this.ConfirmOnNavigateRecord) {
                        _originalDataSource = JObject.Parse(_dataSource.ToStr());
                    }

                    if (showLoading) this.HideLoadPanel(); // hide load panel if it is not hide yet
                    this.Refresh();
                    //resize height of the popup if screen is open in popup
                    await Session.ExecJSAsync("_resizePopupIframe");
                });
            });

            //get next/prev record's keys for Move Next/Prev
            if (_navigateRecord && _recordFetched) {
                _nextRecKeys = await GetNavigateRecordKeysAsync(_nextRecOffset);
                _prevRecKeys = await GetNavigateRecordKeysAsync(_prevRecOffset);
            }

            return true;
        }

        internal async Task ScreenAfterRenderAsync() {
            
            await this.AfterRenderAsync();

            _scrnComponent.Refresh();

        }

        internal async Task<bool> InitAsync(EpfScreen scrnPage, SessionState session, NavigationManager navManager, Screen parentScreen, bool initTitleBar) {

            _session = session;
            _navManager = navManager;
            _parentScreen = parentScreen;
            _fields = new FieldCollection();
            _epfScrnDef = scrnPage.SCRN_DEF;
            _scrnComponent = scrnPage;
            _isiniFrame = await Session.ExecJSAsync<bool>("iniFrame");

            Session.ActiveScrnId = this.SCRN_ID;

            if (_epfScrnDef != null) {

                await InitEditModesAsync();
                await InitParamsAsync();
                await InitDataAsync();
                await InitFieldsAsync(initTitleBar);

                _scrnComponent.Refresh();
            }
            
            
            return true;
        }

        
        private async Task InitEditModesAsync() {
            string url = string.Format("EpfScreen/GetRights/{0}", _epfScrnDef.MDULE_ID);
            string rights = await Session.GetJsonAsync<string>(url);

            _editModes = new List<ScreenMode>();
            if (_epfScrnDef.EDIT_MODES.Contains("N")) _editModes.Add(ScreenMode.Neutral);

            // access right with Insert permission
            if (rights.Contains("I")) {
                if (_epfScrnDef.EDIT_MODES.Contains("A")) _editModes.Add(ScreenMode.Add);
            }

            // access right with Update permission
            if (rights.Contains("U")) {
                if (_epfScrnDef.EDIT_MODES.Contains("U")) _editModes.Add(ScreenMode.Update);
            }

            // access right with Delete permission
            if (rights.Contains("D")) {
                if (_epfScrnDef.EDIT_MODES.Contains("D")) _editModes.Add(ScreenMode.Delete);
            }

            // access right with Select permission
            if (rights.Contains("S")) {
                if (_epfScrnDef.EDIT_MODES.Contains("E")) _editModes.Add(ScreenMode.Enquiry);
                if (_epfScrnDef.EDIT_MODES.Contains("Q")) _editModes.Add(ScreenMode.Query);
            }

            // access right with Print permission
            if (rights.Contains("P")) {
                if (_epfScrnDef.EDIT_MODES.Contains("P")) _editModes.Add(ScreenMode.Print);
            }

        }
       
        public async Task RegisterScreenJSCallBackAsync(bool scrnLoad) {
            await Session.ExecJSAsync("controlEventHandler", SCRN_ID, "ScreenJSCallBack", DotNetObjectReference.Create(this));
            await Session.ExecJSAsync("_render_epfScreen", SCRN_ID, scrnLoad); // let JS call back ShowRec & AfterRender when the screen is ready
        }

        private async Task<bool> InitParamsAsync() {
            _params = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(_scrnComponent.KEYS)) {
                _keys = _scrnComponent.KEYS.Split(",");

                if (!string.IsNullOrEmpty(_scrnComponent.PARAM)) {
                    string[] param = _scrnComponent.PARAM.Split(",");

                    for (int i = 0; i <= _keys.Length - 1; i++) {
                        if (i < param.Length) {
                            _params.Add(_keys[i], param[i]);
                        }
                        else {
                            _params.Add(_keys[i], null);
                        }
                    }
                }

            }

            if (string.IsNullOrEmpty(_scrnComponent.PARENT_KEYS)) {
                _parentKeys = _epfScrnDef.FIELD_KEYS.Split(",");
            }
            else {
                _parentKeys = _scrnComponent.PARENT_KEYS.Split(",");
            }

            return await Task.FromResult<bool>(true);
        }
        private async Task InitDataAsync() {
            _recordFetched = false;
            _dataSource = new JObject();
            await InitColumnInfoAsync();

            if (_scrnComponent.MODE == ScreenMode.Add) {
                foreach (KeyValuePair<string, JObject> col in _dataColumnInfos) {
                    if (_params.ContainsKey(col.Key) && _parentKeys.Contains<string>(col.Key) &&
                        _scrnComponent.KEYS != _scrnComponent.PARENT_KEYS) {

                        if (_dataSource.ContainsKey(col.Key)) {
                            _dataSource[col.Key] = JToken.FromObject(_params[col.Key]);
                        }
                        else {
                            _dataSource.Add(col.Key, JToken.FromObject(_params[col.Key]));
                        }

                    }
                    else {

                        if (_dataSource.ContainsKey(col.Key)) {
                            _dataSource[col.Key] = null;
                        }
                        else {
                            _dataSource.Add(col.Key, null);
                        }

                    }
                }
            }
            if (_scrnComponent.MODE == ScreenMode.Neutral) {
                foreach (KeyValuePair<string, JObject> col in _dataColumnInfos) {
                    if (_dataSource.ContainsKey(col.Key)) {
                        _dataSource[col.Key] = null;
                    }
                    else {
                        _dataSource.Add(col.Key, null);
                    }
                }
            }
            else if (_scrnComponent.MODE == ScreenMode.Update || _scrnComponent.MODE == ScreenMode.Enquiry) {
                await FetchRecAsync();

                //assign param values to fields after fetch
                foreach (var item in _params) {
                    if (_dataSource.ContainsKey(item.Key)) {
                        if (item.Value == null) {
                            _dataSource[item.Key] = JToken.FromObject(item.Value.ToStr());
                        }
                        else {
                            _dataSource[item.Key] = JToken.FromObject(item.Value);
                        }
                    }
                    else {
                        if (item.Value == null) {
                            _dataSource.Add(item.Key, JToken.FromObject(item.Value.ToStr()));
                        }
                        else {
                            _dataSource.Add(item.Key, JToken.FromObject(item.Value));
                        }
                       
                    }
                }
            }


        }
        private async Task FetchRecAsync() {
            _recordFetched = false;
            JObject postData = new JObject();
            postData.Add("viewName", _epfScrnDef.BASE_VIEW);
            postData.Add("fieldKeys", _epfScrnDef.FIELD_KEYS);
            postData.Add("dataColInfos", JToken.FromObject(_dataColumnInfos));
            postData.Add("param", JToken.FromObject(_params));

            JArray rows = await _session.PostJsonAsync<JArray>("EpfScreen/FetchData", postData);

            if (rows.Count > 0) {
                JObject record = JObject.FromObject(rows[0]);
                foreach (var item in record) {
                    if (_dataSource.ContainsKey(item.Key)) {
                        _dataSource[item.Key] = item.Value;
                    }
                    else {
                        _dataSource.Add(item.Key, item.Value);
                    }
                }
                _recordFetched = true;
            }
        }
        private async Task InitColumnInfoAsync() {
            _dataColumnInfos = new Dictionary<string, JObject>();
            string url = string.Format("EpfScreen/GetColumnInfos/{0}", _epfScrnDef.BASE_VIEW);
            _dataColumnInfos = await _session.GetJsonAsync<Dictionary<string, JObject>>(url);
        }
        public async Task InitFieldsAsync(bool initTitleBar) {


            if (initTitleBar) {
                EPF_SCRN_FIELDS bannerContainer = _epfScrnDef.FIELDS.Find(x => x.CONTAINER_ID == "$$EPF$$_$$MAIN$$_$$PANEL$$" && x.CLS_CTRL == "clsBanner");
                if (bannerContainer != null) {
                    EPF_SCRN_FIELDS bannerLabel = _epfScrnDef.FIELDS.Find(x => x.CONTAINER_ID == bannerContainer.FIELD_ID && x.CTRL_TYPE == "LABEL");
                    if (bannerLabel != null) {
                        _titleBar = new FieldTitleBar();

                        _titleBar.ID = "_scrnTitleBar";
                        _titleBar.StyleClass = bannerLabel.CLS_BS_CTRL;
                        _titleBar.TitleText = bannerLabel.FIELD_NAME_0;
                        _titleBar.ParentScreen = this;

                        _titleBar.OnItemClick += titleBar_OnItemClick;
                    }
                }
            }

            if (_parentScreen == null) {
                _fields.Clear();
                await Session.ExecJSAsync("_clear_fields"); // [09/12/2021, PhyoZin] Clear JS _formFields array to reinitialize if no parent screen
                Refresh();
            }
            
            _mainContainer = new FieldContainer("$$EPF$$_$$MAIN$$_$$PANEL$$", "container-fluid d-flex flex-column mt-0 p-0");
            Refresh();

            if (_titleBar != null) {
                _titleBar.Initialize();
                _mainContainer.Fields.Add(_titleBar);
            }

            await AddChildFieldsAsync(_mainContainer);

            // init popup
            _popupScreen = new FieldPopupScreen("popupDialog", "", this);
            _popupScreen.ParentScreen = this;
            _mainContainer.Fields.Add(_popupScreen);


            _fields.Add(_mainContainer);
            Refresh();

            //[18/09/2021, PhyoZin] refresh code/desc for textbox fields
            this.RefreshCodeDescFields();
        }

        private EpfScreen.ScreenType GetScreenType(string url) {
            Uri baseUri = new Uri(_navManager.BaseUri);
            url = new Uri(baseUri, url).ToStr();
            return (EpfScreen.ScreenType)GetQueryStringValue("SCRN_TYPE", url).ToInt();
        }

        private string GetCallerScreenId(string url) {
            Uri baseUri = new Uri(_navManager.BaseUri);
            url = new Uri(baseUri, url).ToStr();
            return GetQueryStringValue("CALLER_SCRN_ID", url).ToStr();
        }

        private async void popupScreen_OnClose(object sender, EventArgs e) {

            Session.ActiveScrnId = this.SCRN_ID;

            // clear url list from history of popup navigation
            bool breakLoop = false;
            while (!breakLoop) {
                string peekUrl = Session.PeekNavigationHistory();
                if (GetScreenType(peekUrl) == EpfScreen.ScreenType.Popup) {
                    string callerScreenId = GetCallerScreenId(peekUrl);
                    if (!string.IsNullOrEmpty(callerScreenId)) {
                        breakLoop = true;
                    }
                    Session.PopNavigationHistory();
                }
                else {
                    breakLoop = true;
                }
            }
            

            _popupScreen.OnClose -= popupScreen_OnClose;
            _popupScreen.ScreenId = "";
            _popupScreen.Mode = ScreenMode.Neutral;
            _popupScreen.Keys = "";
            _popupScreen.ParentKeys = "";
            _popupScreen.Param = "";
            _popupScreen.Render = true;

            
            if (_popupScreen.CallBackFunction != null) {
                _popupScreen.CallBackFunction.Invoke();
            }

            ////[30/11/2021, phyozin] parent screen always need to reload due to popup changes for removing iframe 
            //await Task.Delay(100).ContinueWith(async x => {
            //    await this.ReloadAsync();
            //});

            if (_popupScreen.ReloadParentOnClose) {

                await Task.Delay(100).ContinueWith(async x => {
                    await this.ReloadAsync();
                });


                //await this.ChangeModeAsync(this.ScrnMode);

            }
            else {
                _scrnComponent.Refresh();
            }
        }

        private bool RefreshCodeDescFields() {

            List<EPF_SCRN_FIELDS> codeDescFields = _epfScrnDef.FIELDS.FindAll(x => x.CTRL_TYPE == "TEXTBOX" 
                && x.CTRL_PROP_0.Contains("<TextBoxType>epfCodeDesc</TextBoxType>"));

            foreach (EPF_SCRN_FIELDS epfField in codeDescFields) {
                Field field = Fields.Search(epfField.FIELD_ID);
                var task = RefreshCodeDescAsync(field);
            }

            return true;
        }

        private async Task<bool> RefreshCodeDescAsync(Field field) {

            if (field == null) {
                return false;
            }

            if (field is not FieldEditor) {
                return false;
            }

            FieldEditor editorField = (FieldEditor)field;
            if (!editorField.IsCodeDesc) {
                return false;
            }

            editorField.CodeDesc = "";
            if (!string.IsNullOrEmpty(editorField.Value.ToStr())) {
                var desc = await GetCodeDescAsync(editorField.SqlDesc);
                if (desc != null) {
                    editorField.CodeDesc = desc;
                }
            }

            if (editorField is FieldPickList) {
                this.Refresh();
            }
            else {
                await Session.ExecJSAsync("_assign_field_prop", editorField.ElementID, editorField.GetFieldProp().ToString());
            }

            return true;
        }

        // refresh all grids and do not wait for it
        public bool RefreshGridViews() {
            List<EPF_SCRN_FIELDS> editGrids = _epfScrnDef.FIELDS.FindAll(x => x.CTRL_TYPE == "EDITGRID");
            foreach (EPF_SCRN_FIELDS editGrid in editGrids) {
                var task = RefreshGridViewDataAsync(editGrid.FIELD_ID);
            }


            return true;
        }

        // refresh all grids and wait for it
        public async Task<bool> RefreshGridViewsAsync() {
            List<EPF_SCRN_FIELDS> editGrids = _epfScrnDef.FIELDS.FindAll(x => x.CTRL_TYPE == "EDITGRID");
            foreach (EPF_SCRN_FIELDS editGrid in editGrids) {
                await RefreshGridViewDataAsync(editGrid.FIELD_ID);
            }
            return true;
        }

        private async Task AddChildFieldsAsync(FieldContainer parentContainer) {


            List<EPF_SCRN_FIELDS> epfFields = _epfScrnDef.FIELDS.FindAll(x =>
                !(x.CONTAINER_ID == "$$EPF$$_$$MAIN$$_$$PANEL$$" && x.CLS_CTRL == "clsBanner")  //removing banner container
                && (
                    (parentContainer.ID == "$$EPF$$_$$MAIN$$_$$PANEL$$" &&
                        (x.CONTAINER_ID == "$$EPF$$_$$MAIN$$_$$PANEL$$" || (string.IsNullOrEmpty(x.CONTAINER_ID) && x.CTRL_TYPE == "TABCONTROL")))
                    || x.CONTAINER_ID == parentContainer.ID
                )
            );

            epfFields = epfFields.OrderBy(epfField => epfField.X_POS).ThenBy(epfField => epfField.Y_POS).ToList();

            foreach (EPF_SCRN_FIELDS epfField in epfFields) {
                
                Field newField = RenderField(epfField);

                if (newField != null) {
                    
                    newField.ParentScreen = this;
                    newField.ParentContainer = parentContainer;

                    var _ = Task.Run(() => {
                        ApplyDisable(epfField, newField);
                        AttachedEvents(newField);    
                    });

                    ApplyDefaultValue(epfField, newField);

                    parentContainer.Fields.Add(newField);


                    if (newField is FieldContainer) {
                        FieldContainer container = (FieldContainer)newField;
                        await AddChildFieldsAsync(container);
                    }
                    else if (newField is FieldTabContainer) {
                        FieldTabContainer tabContainer = (FieldTabContainer)newField;
                        foreach (FieldTabPage tabPage in tabContainer.TabPages) {
                            await AddChildFieldsAsync(tabPage);
                        }
                    }
                    else if (newField is FieldEditor) {
                        
                        if (newField.Value is object) {
                            newField.Value = string.Empty;
                        }

                        if (DataSource.ContainsKey(newField.ID)) {
                            newField.Value = DataSource[newField.ID];
                        }
                        else {
                            _dataSource.Add(newField.ID, JToken.FromObject(newField.Value));
                        }
                    }
                }

            }


        }
        private void ApplyDisable(EPF_SCRN_FIELDS epfField, Field newField) {

            // overwirte disable for sys buttons
            if (newField is FieldButton) {
                FieldButton button = (FieldButton)newField;
                switch (button.ButtonType) {
                    case FieldButtonType.epfAdd:
                        epfField.FIELD_ATTR = "DEEEEEE";
                        break;

                    case FieldButtonType.epfDelete:
                        epfField.FIELD_ATTR = "DDEDEDD";
                        break;

                    case FieldButtonType.epfMoveNext:
                    case FieldButtonType.epfMovePrev:
                        epfField.FIELD_ATTR = "DDEDEDD";
                        break;

                    case FieldButtonType.epfSave:
                        epfField.FIELD_ATTR = "DEEEDDD";
                        break;
                }
            }

            if (epfField.FIELD_ATTR.SplitAndGetItem(_scrnComponent.MODE.ToInt()) == "D") {
                newField.Disabled = true;
            }
        }
        private void AttachedEvents(Field newField) {
            newField.OnChange += Field_OnChange;
            newField.OnFocusIn += Field_OnFocusIn;
            newField.OnFocusOut += Field_OnFocusOut;
            newField.OnClick += Field_OnClick;

            //if (newField is FieldGridView) {
            //    FieldGridView gridView = (FieldGridView)newField;
            //    gridView.OnGridAction += gridView_OnGridAction;
            //}
            //else if (newField is FieldAccordion) {
            //    FieldAccordion accordion = (FieldAccordion)newField;
            //    accordion.OnAccordionAction += gridView_OnGridAction;
            //}
        }
        private void ApplyDefaultValue(EPF_SCRN_FIELDS epfField, Field newField) {
            // Apply Default Value
            if (!string.IsNullOrEmpty(epfField.DEF_VALUE)) {
                if (string.IsNullOrEmpty(GetFieldValue(newField.ID).ToStr())) {
                    if (epfField.DEF_VALUE.Contains(",")) {
                        string defValue = epfField.DEF_VALUE.SplitAndGetItem(',', ScrnMode.ToInt());
                        switch (defValue) {
                            case ":COMP_CODE":
                                SetFieldValue(newField, Session.CompCode);
                                break;
                            case ":EMPE_ID":
                                SetFieldValue(newField, Session.EmpeID);
                                break;
                            case ":USER_ID":
                                SetFieldValue(newField, Session.UserID);
                                break;
                            case ":LANG_ID":
                                SetFieldValue(newField, Session.LangId);
                                break;
                            case ":DATE":
                                SetFieldValue(newField, DateTime.Now);
                                break;
                            default:
                                SetFieldValue(newField, defValue);
                                break;
                        }

                    }
                    else {
                        switch (epfField.DEF_VALUE) {
                            case ":COMP_CODE":
                                SetFieldValue(newField, Session.CompCode);
                                break;
                            case ":EMPE_ID":
                                SetFieldValue(newField, Session.EmpeID);
                                break;
                            case ":USER_ID":
                                SetFieldValue(newField, Session.UserID);
                                break;
                            case ":LANG_ID":
                                SetFieldValue(newField, Session.LangId);
                                break;
                            case ":DATE":
                                SetFieldValue(newField, DateTime.Now);
                                break;
                            default:
                                SetFieldValue(newField, epfField.DEF_VALUE);
                                break;
                        }
                    }
                }
            }
        }

        private Field RenderField(EPF_SCRN_FIELDS epfField) {
            Field newField = null;
            if (epfField.CTRL_TYPE == "CONTAINER") {
                newField = new FieldContainer() {
                    ID = epfField.FIELD_ID,
                    StyleClass = epfField.CLS_BS_CTRL
                };
                if (string.IsNullOrEmpty(newField.StyleClass)) {
                    newField.StyleClass = "container-fluid p-0";
                }
            }
            else if (epfField.CTRL_TYPE == "SUBCONTAINER") {
                newField = new FieldContainer() {
                    ID = epfField.FIELD_ID,
                    StyleClass = epfField.CLS_BS_CTRL
                };

            }
            else if (epfField.CTRL_TYPE == "TABCONTROL") {
                FieldTabContainer newTabField = new FieldTabContainer() {
                    ID = epfField.FIELD_ID,
                    StyleClass = epfField.CLS_BS_CTRL
                };

                dynamic ctrlProp = DynamicXml.Parse(epfField.CTRL_PROP_0);
                string[] tabPageIDs = Convert.ToString(ctrlProp.TabContainerIDS).Split(',');
                string tabCaptions = Convert.ToString(ctrlProp.TabCaptions);
                int index = 0;
                while (index <= tabPageIDs.Length - 1) {
                    FieldTabPage tabPage = new FieldTabPage() {
                        ID = tabPageIDs[index],
                        Description = tabCaptions.SplitAndGetItem(',', index)
                    };

                    newTabField.TabPages.Add(tabPage);
                    index += 1;
                }

                if (tabPageIDs.Length >= 1) {
                    newTabField.ActiveTabPageId = tabPageIDs[0];
                }

                newField = newTabField;
            }
            else if (epfField.CTRL_TYPE == "TEXTBOX" && string.IsNullOrEmpty(epfField.BRWS_ID)) {
                string styleClass = epfField.CLS_BS_CTRL;
                string styleClassWidth = epfField.CLS_BS_CTRL_WIDTH;
                decimal? maxlength = epfField.MAX_LENGTH;
                dynamic ctrlProp = DynamicXml.Parse(epfField.CTRL_PROP_0);
                int height = 60;
                if (epfField.CTRL_DIMENSION.SplitAndGetItem(':', 1).IsNumeric()) {
                    height = epfField.CTRL_DIMENSION.SplitAndGetItem(':', 1).ToInt();
                }

                FieldTextBoxCharacterCase charCase = FieldTextBoxCharacterCase.normal;
                if (ctrlProp.CharacterCase == "epfUpper") {
                    charCase = FieldTextBoxCharacterCase.uppercase;
                }
                else if (ctrlProp.CharacterCase == "epfLower") {
                    charCase = FieldTextBoxCharacterCase.lowercase;
                }

                if (epfField.FIELD_TYPE == "N") {
                    newField = new FieldNumber() {
                        ID = epfField.FIELD_ID,
                        Description = epfField.FIELD_NAME_0,
                        StyleClass = styleClass,
                        StyleClassWidth = styleClassWidth,
                        MaxLength = maxlength.ToInt(),
                        CharacterCase = charCase,
                        Mask = epfField.FIELD_MASK
                    };
                }
                else if (epfField.FIELD_TYPE == "C" && ctrlProp.TextBoxType == "epfMultiLine") {
                    FieldTextArea textArea = new FieldTextArea() {
                        ID = epfField.FIELD_ID,
                        Description = epfField.FIELD_NAME_0,
                        StyleClass = styleClass,
                        StyleClassWidth = styleClassWidth,
                        Height = height,
                        CharacterCase = charCase,
                        Mask = epfField.FIELD_MASK
                    };

                    //[17/09/2021, PhyoZin] add in CodeDesc feature for TextArea
                    if (ctrlProp.TextBoxType == "epfCodeDesc") {
                        textArea.IsCodeDesc = true;
                        textArea.SqlDesc = ctrlProp.SqlTemplate;
                    }

                    //if (textArea.IsCodeDesc) {
                    //    if (_dataSource.ContainsKey(textArea.ID)) {
                    //        if (!string.IsNullOrEmpty(_dataSource[textArea.ID].ToStr())) {
                    //            textArea.CodeDesc = await GetCodeDescAsync(textArea.SqlDesc);
                    //        }
                    //    }
                    //}

                    newField = textArea;
                }
                else {

                    FieldText textField = new FieldText() {
                        ID = epfField.FIELD_ID,
                        Description = epfField.FIELD_NAME_0,
                        StyleClass = styleClass,
                        StyleClassWidth = styleClassWidth,
                        MaxLength = maxlength.ToInt(),
                        CharacterCase = charCase,
                        Mode = (ctrlProp.TextBoxType == "epfPassword") ? FieldTextMode.password: FieldTextMode.text,
                        Mask = epfField.FIELD_MASK
                    };

                    //[17/09/2021, PhyoZin] add in CodeDesc feature for TextBox
                    if (ctrlProp.TextBoxType == "epfCodeDesc") {
                        textField.IsCodeDesc = true;
                        textField.SqlDesc = ctrlProp.SqlTemplate;
                    }

                    //if (textField.IsCodeDesc) {
                    //    if (_dataSource.ContainsKey(textField.ID)) {
                    //        if (!string.IsNullOrEmpty(_dataSource[textField.ID].ToStr())) {
                    //            textField.CodeDesc = await GetCodeDescAsync(textField.SqlDesc);
                    //        }
                    //    }
                    //}

                    newField = textField;
                }

            }
            else if (epfField.CTRL_TYPE == "TEXTBOX" && !string.IsNullOrEmpty(epfField.BRWS_ID)) {
                string styleClass = epfField.CLS_BS_CTRL;
                string styleClassWidth = epfField.CLS_BS_CTRL_WIDTH;

                dynamic ctrlProp = DynamicXml.Parse(epfField.CTRL_PROP_0);

                string brwsKeys = ctrlProp.BrwsKeys;
                bool pickOnly = Convert.ToBoolean(ctrlProp.PickOnly);
                string brwsTarget = ctrlProp.BrwsTarget;
                string brwsAssign = ctrlProp.BrwsAssign;
                decimal? maxlength = epfField.MAX_LENGTH;

                int height = 60;
                if (epfField.CTRL_DIMENSION.SplitAndGetItem(':', 1).IsNumeric()) {
                    height = epfField.CTRL_DIMENSION.SplitAndGetItem(':', 1).ToInt();
                }

                FieldTextBoxCharacterCase charCase = FieldTextBoxCharacterCase.normal;
                if (ctrlProp.CharacterCase == "epfUpper") {
                    charCase = FieldTextBoxCharacterCase.uppercase;
                }
                else if (ctrlProp.CharacterCase == "epfLower") {
                    charCase = FieldTextBoxCharacterCase.lowercase;
                }

                FieldPickList pickListField = new FieldPickList() {
                    ID = epfField.FIELD_ID,
                    Description = epfField.FIELD_NAME_0,
                    StyleClass = styleClass,
                    StyleClassWidth = styleClassWidth,
                    BrwsId = epfField.BRWS_ID,
                    BrwsKeys = brwsKeys,
                    PickOnly = pickOnly,
                    BrwsTarget = brwsTarget,
                    BrwsAssign = brwsAssign,
                    Height = height,
                    CharacterCase = charCase,
                    MaxLength = maxlength.ToInt(),
                    FieldType = epfField.FIELD_TYPE
                };

                if (ctrlProp.TextBoxType == "epfCodeDesc") {
                    pickListField.IsCodeDesc = true;
                    pickListField.SqlDesc = ctrlProp.SqlTemplate;
                }

                //pickListField.OnChange += picklist_OnChange;
                pickListField.OnPickListSelection += picklist_OnPickListSelection;
                //pickListField.OnPickListSelected += picklist_OnPickListSelected;


                newField = pickListField;
            }
            else if (epfField.CTRL_TYPE == "DATETIME") {
                string styleClass = epfField.CLS_BS_CTRL;
                string styleClassWidth = epfField.CLS_BS_CTRL_WIDTH;

                FieldDateType type = FieldDateType.date;
                string format = "";
                dynamic ctrlProp = DynamicXml.Parse(epfField.CTRL_PROP_0);

                if (ctrlProp.DateTimeType == "epfDate") {
                    type = FieldDateType.date;
                    format = string.IsNullOrEmpty(epfField.FIELD_MASK) ? "dd/MM/yyyy": epfField.FIELD_MASK;
                }
                else if (ctrlProp.DateTimeType == "epfDateTime") {
                    type = FieldDateType.datetime;
                    format = string.IsNullOrEmpty(epfField.FIELD_MASK) ? "dd/MM/yyyy HH:mm" : epfField.FIELD_MASK;
                }
                else if (ctrlProp.DateTimeType == "epfTime") {
                    type = FieldDateType.time;
                    format = string.IsNullOrEmpty(epfField.FIELD_MASK) ? "HH:mm" : epfField.FIELD_MASK;
                }


                newField = new FieldDate() {
                    ID = epfField.FIELD_ID,
                    Description = epfField.FIELD_NAME_0,
                    StyleClass = styleClass,
                    StyleClassWidth = styleClassWidth,
                    Type = type,
                    DisplayFormat = format
                };
            }
            else if (epfField.CTRL_TYPE == "LIST" && epfField.CLS_CTRL == "clsDropDown") {
                string styleClass = epfField.CLS_BS_CTRL;
                string styleClassWidth = epfField.CLS_BS_CTRL_WIDTH;

                dynamic ctrlProp = DynamicXml.Parse(epfField.CTRL_PROP_0);
                string[] listTexts = Convert.ToString(ctrlProp.ListText).Split(',');
                string[] listValues = Convert.ToString(ctrlProp.ListValue).Split(',');

                FieldSelect fieldSelect = new FieldSelect() {
                    ID = epfField.FIELD_ID,
                    Description = epfField.FIELD_NAME_0,
                    StyleClass = styleClass,
                    StyleClassWidth = styleClassWidth
                };

                for (int i = 0; i <= listValues.Length - 1; i++) {
                    fieldSelect.AddItem(listValues[i], listTexts[i]);
                }

                newField = fieldSelect;
            }
            else if (epfField.CTRL_TYPE == "LIST" && epfField.CLS_CTRL == "clsListBox") {
                string styleClass = epfField.CLS_BS_CTRL;
                string styleClassWidth = epfField.CLS_BS_CTRL_WIDTH;

                dynamic ctrlProp = DynamicXml.Parse(epfField.CTRL_PROP_0);
                string[] listTexts = Convert.ToString(ctrlProp.ListText).Split(',');
                string[] listValues = Convert.ToString(ctrlProp.ListValue).Split(',');
                int height = 100;
                if (epfField.CTRL_DIMENSION.SplitAndGetItem(':', 1).IsNumeric()) {
                    height = epfField.CTRL_DIMENSION.SplitAndGetItem(':', 1).ToInt();
                }

                FieldList fieldList = new FieldList() {
                    ID = epfField.FIELD_ID,
                    Description = epfField.FIELD_NAME_0,
                    StyleClass = styleClass,
                    StyleClassWidth = styleClassWidth,
                    Height = height
                };

                for (int i = 0; i <= listValues.Length - 1; i++) {
                    fieldList.AddItem(listValues[i], listTexts[i]);
                }

                newField = fieldList;
            }
            else if (epfField.CTRL_TYPE == "LIST" && epfField.CLS_CTRL == "clsCaption") {
                string styleClass = epfField.CLS_BS_CTRL;
                string styleClassWidth = epfField.CLS_BS_CTRL_WIDTH;

                dynamic ctrlProp = DynamicXml.Parse(epfField.CTRL_PROP_0);
                string[] listTexts = Convert.ToString(ctrlProp.ListText).Split(',');
                string[] listValues = Convert.ToString(ctrlProp.ListValue).Split(',');
                string listDirection = ctrlProp.ListDirection;

                FieldRadio fieldRadio = new FieldRadio() {
                    ID = epfField.FIELD_ID,
                    Description = epfField.FIELD_NAME_0,
                    StyleClass = styleClass,
                    StyleClassWidth = styleClassWidth
                };

                fieldRadio.ListDirection = ParseEnum<FieldRadioListDirection>(listDirection);

                for (int i = 0; i <= listValues.Length - 1; i++) {
                    fieldRadio.AddItem(listValues[i], listTexts[i]);
                }

                newField = fieldRadio;
            }
            else if (epfField.CTRL_TYPE == "LIST" && epfField.CLS_CTRL == "clsCellList") {
                string BRWS_ID = "", BASE_VIEW = "", PARAM_KEYS = "", RESULT_FIELD = "" , CELL_PAGE_SETT = "" , CURR_PAG_CTRL = ""; int CELL_PAGE_SIZE = 10;
                dynamic ctrlProp = DynamicXml.Parse(epfField.CTRL_PROP_0);
                string[] listKeys = Convert.ToString(ctrlProp.ListText).Split(',');
                string[] listValues = Convert.ToString(ctrlProp.ListValue).Split(',');
                if (!string.IsNullOrEmpty(ctrlProp.ListText) && !string.IsNullOrEmpty(ctrlProp.ListValue)) {
                    if (listKeys.Length == listValues.Length) {
                        for (int k = 0; k <= listKeys.Length - 1; k++) {
                            string value = listValues[k];
                            switch (listKeys[k].ToUpper()) {
                                case "BRWS_ID":
                                    BRWS_ID = value;
                                    break;
                                case "BASE_VIEW":
                                    BASE_VIEW = value;
                                    break;
                                case "PARAM_KEYS":
                                    PARAM_KEYS = (value.Contains(":") ? value.Replace(':', ',') : value);
                                    break;
                                case "RESULT_FIELD":
                                    RESULT_FIELD = value;
                                    break;
                                case "CELL_PAGE_SETT":
                                    CELL_PAGE_SETT = value;
                                    CELL_PAGE_SIZE = _dataSource[value].ToInt(); // GetFieldValue<int>(value);
                                    break;
                                case "CURR_PAGE_CTRL":
                                    CURR_PAG_CTRL = value;
                                    break;
                                default:
                                    // code block
                                    break;
                            }
                        }
                    }
                }

                FieldCellList cellList = new FieldCellList() {
                    ID = epfField.FIELD_ID,
                    BrwsID = BRWS_ID,
                    BaseView = BASE_VIEW,
                    PARAM_KEYS = PARAM_KEYS,
                    SCRN_DS = _dataSource,
                    RESULT_FIELD = RESULT_FIELD,
                    CellPageSetting = CELL_PAGE_SETT,
                    CellPageSize = CELL_PAGE_SIZE,
                    CurrentPageCtrlName = CURR_PAG_CTRL,
                    CurrentPage = 0
                };
                cellList.OnCellSettingClick += cellList_OnCellSettingClick;
                cellList.OnCellItemClick += cellList_OnCellItemClick;
                newField = cellList;
            }
            else if (epfField.CTRL_TYPE == "LIST" && epfField.CLS_CTRL == "clsImgList") {
                string BRWS_ID = "", BASE_VIEW = "", PARAM_KEYS = "", ImgColName = "", DescColName = "", PriceColName = "", PromoColName = "", DiscColName = "", QntyColName = "",  RESULT_FIELD = "", IMG_URL = "";
                Boolean IsModi = false; int IMG_COL = 3;
                dynamic ctrlProp = DynamicXml.Parse(epfField.CTRL_PROP_0);
                string[] listKeys = Convert.ToString(ctrlProp.ListText).Split(',');
                string[] listValues = Convert.ToString(ctrlProp.ListValue).Split(',');
                if (!string.IsNullOrEmpty(ctrlProp.ListText) && !string.IsNullOrEmpty(ctrlProp.ListValue)) {
                    if (listKeys.Length == listValues.Length) {
                        for (int k = 0; k <= listKeys.Length - 1; k++) {
                            string value = listValues[k];
                            switch (listKeys[k].ToUpper()) {
                                case "BRWS_ID":
                                    BRWS_ID = value;
                                    break;
                                case "BASE_VIEW":
                                    BASE_VIEW = value;
                                    break;
                                case "PARAM_KEYS":
                                    PARAM_KEYS = (value.Contains(":") ? value.Replace(':', ',') : value);
                                    break;
                                case "IMGCOLNAME":
                                    ImgColName = value;
                                    break;
                                case "DESCCOLNAME":
                                    DescColName = value;
                                    break;
                                case "PRICECOLNAME":
                                    PriceColName = value;
                                    break;
                                case "QNTYCOLNAME":
                                    QntyColName = value;
                                    break;
								case "PROMOCOLNAME":
                                    PromoColName = value;
                                    break;
                                case "DISCCOLNAME":
                                    DiscColName = value;
                                    break;
                                case "RESULT_FIELD":
                                    RESULT_FIELD = value;
                                    break;
								case "IMG_URL":
                                    IMG_URL = value;
                                    break;
                                case "ISMODI":
                                    IsModi = true;
                                    break;
                                case "IMG_COL":
                                    IMG_COL = value.ToInt();
                                    break;
                                default:
                                    // code block
                                    break;
                            }
                        }
                    }
                }

                FieldImgList imgList = new FieldImgList() {
                    ID = epfField.FIELD_ID,
                    BrwsID = BRWS_ID,
                    BaseView = BASE_VIEW,
                    PARAM_KEYS = PARAM_KEYS,
                    //SCRN_DS = _dataSource,
                    ImgColName = ImgColName,
                    DescColName = DescColName,
                    PriceColName = PriceColName,
                    QntyColName = QntyColName,
					PromoColName = PromoColName,
                    DiscColName = DiscColName,
                    RESULT_FIELD = RESULT_FIELD,
					IMG_URL = IMG_URL,
                    IsModi = IsModi,
                    IMG_COL = IMG_COL
                };

                imgList.OnImgItemClick += imgList_OnImgItemClick;
                newField = imgList;
            }
			else if (epfField.CTRL_TYPE == "BUTTON") {
                dynamic ctrlProp = DynamicXml.Parse(epfField.CTRL_PROP_0);

                if (ctrlProp.ButtonType == FieldButtonType.epfMultiPick.ToStr()) {
                    FieldMultiPick multiPick = new FieldMultiPick(epfField.FIELD_ID, epfField.FIELD_NAME_0);
                    multiPick.MultiPickButton.SaveOnNavigate = Convert.ToBoolean(ctrlProp.SaveOnNavigate);

                    FieldGridViewSelectionMode selectionMode = FieldGridViewSelectionMode.multiple;
                    string keyValuesProp = ctrlProp.KeyValues;
                    if (!string.IsNullOrEmpty(keyValuesProp)) {
                        string[] keyValues = keyValuesProp.Split(',');
                        foreach (string keyValue in keyValues) {
                            string key = keyValue.Split('=')[0];
                            string value = keyValue.Split('=')[1];

                            if (key == "SEL_MODE") {
                                selectionMode = (FieldGridViewSelectionMode)value.ToInt();
                            }
                            else {
                                multiPick.KeyValues.Add(key, value);
                            }
                        }
                    }

                    if (multiPick.KeyValues.ContainsKey("MP_ID")) {
                        multiPick.TwoStepMultiPick = true;
                    }

                    multiPick.BrwsId = epfField.BRWS_ID;
                    multiPick.SelectionMode = selectionMode;
                    multiPick.BrwsGrid.SelectionMode = selectionMode;
                    multiPick.OnMultiPickClick += multiPick_OnClick;
                    multiPick.OnSelectionChange += multiPick_OnSelectionChange;
                    multiPick.OnGroupSelectionChange += multiPick_OnGroupSelectionChange;
                    multiPick.OnClose += multiPick_OnClose;

                    string tempStyle = string.Format("{0} {1}", epfField.CLS_BS_CTRL, epfField.CLS_BS_CTRL_WIDTH);
                    string[] tempStyles = tempStyle.Split(' ').Distinct().ToArray();
                    foreach (string style in tempStyles) {
                        if (!string.IsNullOrEmpty(style)) {
                            if (style.Contains("col") || style.Contains("offset")) {
                                multiPick.StyleClassWidth += " " + style;
                            }
                            else {
                                multiPick.StyleClass += " " + style;
                            }
                        }

                    }
                    newField = multiPick;
                }
                else {
                    FieldButton button = new FieldButton(epfField.FIELD_ID, epfField.FIELD_NAME_0);
                    button.ButtonType = ParseEnum<FieldButtonType>(Convert.ToString(ctrlProp.ButtonType));
                    button.SaveOnNavigate = Convert.ToBoolean(ctrlProp.SaveOnNavigate);
                    
                    string tempStyle = string.Format("{0} {1}", epfField.CLS_BS_CTRL, epfField.CLS_BS_CTRL_WIDTH);
                    string[] tempStyles = tempStyle.Split(' ').Distinct().ToArray();
                    foreach (string style in tempStyles) {
                        if (!string.IsNullOrEmpty(style)) {
                            if (style.Contains("col") || style.Contains("offset")) {
                                button.StyleClassWidth += " " + style;
                            }
                            else {
                                button.StyleClass += " " + style;
                            }
                        }

                    }

                    newField = button;
                }

            }
            else if (epfField.CTRL_TYPE == "EDITGRID") {

                if (_session.AppConfig.enableAccordionView && _session.IsMobileScreen() && epfField.CLS_CTRL != "clsDesktopGrid") {
                    dynamic ctrlProp = DynamicXml.Parse(epfField.CTRL_PROP_0);

                    FieldAccordion accordionField = new FieldAccordion() {
                        ID = epfField.FIELD_ID,
                        BRWS_ID = epfField.BRWS_ID,
                        AccordionType = ParseEnum<FieldAccordionType>(Convert.ToString(ctrlProp.GridType)),
                        NavigateScrnId = ctrlProp.ScreenId,
                        TargetKeys = ctrlProp.TargetKeys,
                        ScreenKeys = ctrlProp.ScreenKeys,
                        SaveOnNavigate = Convert.ToBoolean(ctrlProp.SaveOnNavigate),
                        ShowTitleBar = Convert.ToBoolean(ctrlProp.DisplayToolbar),
                        ShowTitleBarAddNew = false,
                        PopupLinkScreen = Convert.ToBoolean(ctrlProp.PopupLinkScreen),
                        DeleteRecord = Convert.ToBoolean(ctrlProp.DeleteRecord),
                        ScreenId = SCRN_ID
                    };

                    if (Convert.ToBoolean(ctrlProp.AddNewRecord)) {
                        if (_scrnComponent.MODE == ScreenMode.Enquiry
                        || _scrnComponent.MODE == ScreenMode.Update
                        || _scrnComponent.MODE == ScreenMode.Add) {
                            accordionField.ShowTitleBarAddNew = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(epfField.BRWS_ID)) {
                        accordionField.OnItemDetailClick += accordion_OnItemDetailClick;
                        accordionField.OnAddNewClick += accordion_OnAddNewClick;
                    }


                    newField = accordionField;
                }
                else {
                    dynamic ctrlProp = DynamicXml.Parse(epfField.CTRL_PROP_0);

                    FieldGridView gridView = new FieldGridView() {
                        ID = epfField.FIELD_ID,
                        BRWS_ID = epfField.BRWS_ID,
                        PageSize = Convert.ToInt32(ctrlProp.PageSize),
                        GridType = ParseEnum<FieldGridViewType>(Convert.ToString(ctrlProp.GridType)),
                        NavigateScrnId = ctrlProp.ScreenId,
                        TargetKeys = ctrlProp.TargetKeys,
                        ScreenKeys = ctrlProp.ScreenKeys,
                        SaveOnNavigate = Convert.ToBoolean(ctrlProp.SaveOnNavigate),
                        ShowTitleBar = Convert.ToBoolean(ctrlProp.DisplayToolbar),
                        ShowTitleBarAddNew = false,
                        PopupLinkScreen = Convert.ToBoolean(ctrlProp.PopupLinkScreen),
                        AssignField = Convert.ToString(ctrlProp.SelectTargetField),
                        AllowDeleting = false,
                        EditRowOnClick = Convert.ToBoolean(ctrlProp.EditRowOnClick),
                        ShowCommandButtons = Convert.ToBoolean(ctrlProp.ShowCommandButtons),
                        Description = epfField.FIELD_NAME_0,
                        SelectTargetField = ctrlProp.SelectTargetField,
                        SelectSourceKeys = ctrlProp.SelectSourceKeys,
                        SelectionMode = Convert.ToBoolean(ctrlProp.EnableCheckBoxes) == true ? FieldGridViewSelectionMode.multiple : FieldGridViewSelectionMode.single,
                        ParentScreen = this
                    };


                    if (this.HasRights(ScreenMode.Delete)) {
                        gridView.AllowDeleting = Convert.ToBoolean(ctrlProp.DeleteRecord);
                    }

                    if (Convert.ToBoolean(ctrlProp.AddNewRecord)) {
                        if (_scrnComponent.MODE == ScreenMode.Enquiry
                        || _scrnComponent.MODE == ScreenMode.Update
                        || _scrnComponent.MODE == ScreenMode.Add) {
                            gridView.ShowTitleBarAddNew = true;
                        }
                    }

                    if (!string.IsNullOrEmpty(epfField.BRWS_ID)) {
                        gridView.OnSelectionChanged += gridView_OnSelectionChanged;
                        gridView.OnAddNewClick += gridView_OnAddNewClick;
                    }

                    EPF_BRWS brwsDef = SCRN_DEF.BRWS.Find(x => x.BRWS_ID == epfField.BRWS_ID);
                    if (brwsDef != null) {
                        gridView.InitColumns(brwsDef.BRWS_COLUMNS, brwsDef.COL_WIDTH_0, brwsDef.COL_HDR_0, brwsDef.COL_FORMAT_0, brwsDef.COL_ALIGN_0);
                    }

                    gridView.LoadData = false;
                    gridView.Render = true;

                    newField = gridView;
                }


            }
            else if (epfField.CTRL_TYPE == "LABEL") {
                string styleClass = epfField.CLS_BS_CTRL;
                string styleClassWidth = epfField.CLS_BS_CTRL_WIDTH;

                dynamic ctrlProp = DynamicXml.Parse(epfField.CTRL_PROP_0);

                FieldLabel labelField = new FieldLabel() {
                    ID = epfField.FIELD_ID,
                    Description = epfField.FIELD_NAME_0
                };

                string tempStyle = string.Format("{0} {1}", styleClass, styleClassWidth);
                string[] tempStyles = tempStyle.Split(' ').Distinct().ToArray();

                foreach (string style in tempStyles) {
                    if (!string.IsNullOrEmpty(style)) {

                        if (style.Contains("col") || style.Contains("offset")) {
                            labelField.StyleClassWidth += " " + style;
                        }
                        else {
                            labelField.StyleClass += " " + style;
                        }
                    }

                }

                newField = labelField;

            }
            else if (epfField.CTRL_TYPE == "CHECKBOX") {
                string styleClass = epfField.CLS_BS_CTRL;
                string styleClassWidth = epfField.CLS_BS_CTRL_WIDTH;
                dynamic ctrlProp = DynamicXml.Parse(epfField.CTRL_PROP_0);

                FieldCheckBox checkBoxField = new FieldCheckBox() {
                    ID = epfField.FIELD_ID,
                    Description = epfField.FIELD_NAME_0,
                    StyleClass = styleClass,
                    StyleClassWidth = styleClassWidth
                };

                newField = checkBoxField;

            }
            else if (epfField.CTRL_TYPE == "IMAGE") {
                string styleClass = epfField.CLS_BS_CTRL;
                string styleClassWidth = epfField.CLS_BS_CTRL_WIDTH;
                dynamic ctrlProp = DynamicXml.Parse(epfField.CTRL_PROP_0);

                int height = 60;
                if (epfField.CTRL_DIMENSION.SplitAndGetItem(':', 1).IsNumeric()) {
                    height = epfField.CTRL_DIMENSION.SplitAndGetItem(':', 1).ToInt();
                }

                FieldImage imageField = new FieldImage() {
                    ID = epfField.FIELD_ID,
                    Description = epfField.FIELD_NAME_0,
                    StyleClass = styleClass,
                    StyleClassWidth = styleClassWidth,
                    Height = height,
                    Source = ctrlProp.Url
                };

                newField = imageField;
            }

            return newField;
		}
        private async void cellList_OnCellItemClick(object sender, JObject e) {
            FieldCellList cellList = (FieldCellList)sender;
            if (GetFieldValue<string>(cellList.RESULT_FIELD) == "") {
                SetFieldValue(cellList.RESULT_FIELD, e.ToString());
                SetFieldValue(cellList.CellPageSetting, cellList.CellPageSize);
                SetFieldValue(cellList.CurrentPageCtrlName, cellList.CurrentPage);
                SetFieldValue("CELL_PAGE_SETT", cellList.CellPageSize);
                await this.ShowRecAsync();
            }
        }
        private async void cellList_OnCellSettingClick(object sender, String e)
        {
            FieldCellList cellList = (FieldCellList)sender;
            SetFieldValue(cellList.RESULT_FIELD, "PAGE SIZE CHANGES");
            SetFieldValue(cellList.CurrentPageCtrlName, cellList.CurrentPage);
            SetFieldValue(cellList.CellPageSetting, cellList.CellPageSize);
            SetFieldValue("CELL_PAGE_SETT", cellList.CellPageSize);
            await this.ShowRecAsync();
        }

        private static T ParseEnum<T>(string value) {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        private void ApplyStyleClass(Field field, string styleClasses) {
            field.StyleClass = "";
            field.StyleClassWidth = "";
            string[] tempStyles = styleClasses.Split(' ').Distinct().ToArray();
            foreach (string style in tempStyles) {
                if (!string.IsNullOrEmpty(style)) {
                    if (style.Contains("col") || style.Contains("offset")) {
                        field.StyleClassWidth += " " + style;
                    }
                    else {
                        field.StyleClass += " " + style;
                    }
                }

            }
            _scrnComponent.Refresh();
        }

        [JSInvokable]
        public async Task<string> gridView_OnEditGridCellActionAsync(string jsonEventData) {

            JObject jObj = JObject.Parse(jsonEventData);

            FieldGridActionEventArgs arg = new FieldGridActionEventArgs();
            arg.FieldId = jObj["FieldId"].ToString();
            arg.PreviousValue = jObj["PreviousValue"].ToString();
            arg.Value = jObj["Value"].ToString();
            arg.DataSource = JObject.FromObject(jObj["Value"]);
            arg.EventType = jObj["EventType"].ToString().ToEnum<FieldGridActionEventType>();

            FieldGridView gridview = (FieldGridView)Fields.Search(arg.FieldId);
            string dataField = arg.DataSource["arg"]["dataField"].ToStr();

            gridview.EditRow.SetDataSource(JObject.FromObject(arg.DataSource["rowData"]));

            //call for validation from BO
            gridview.EditRow.Ok = await InvokeEditGridCellActionAsync(gridview, dataField, arg.EventType);
            return JObject.FromObject(gridview.EditRow).ToStr();

        }

        private async Task<bool> InvokeEditGridCellActionAsync(FieldGridView gridview, string dataField, FieldGridActionEventType eventType) {
            bool ret = true;

            string eventId = "";
            if (eventType == FieldGridActionEventType.onEditCellOnChanged) {
                eventId = "onchange";
            }
            else if (eventType == FieldGridActionEventType.onEditCellOnClick) {
                eventId = "onclick";
            }

            List<EPF_SCRN_EVENTS> events = gridview.NavigateScrnDefinition.EVENTS.FindAll(x => x.OBJ_ID == dataField && x.EVENT_ID == eventId);
            if (events.Count > 0) {
                foreach (EPF_SCRN_EVENTS evt in events) {
                    var type = this.GetType();
                    var method = type.GetMethod(evt.BO_NAME);
                    if (method == null) {
                        Session.ToastMessage("Method not found! " + evt.BO_NAME, ToastMessageType.error);
                        return true;
                    }
                    if (method.ReturnType.Name.Contains("Task<bool>")) {
                        Task<bool> cellActionTask = (Task<bool>)method.Invoke(this, null);
                        ret = await cellActionTask;
                    }
                    else if (method.ReturnType.Name.Contains("Task")) {
                        Task cellActionTask = (Task)method.Invoke(this, null);
                        await cellActionTask;
                        ret = true;
                    }
                    else {
                        method.Invoke(this, null);
                        ret = true;
                    }
                }
            }
            return ret;
        }

        private string GetActualFieldId(string elementId) {
            string scrnIdPrefix = this.SCRN_ID + "__";
            return elementId.Replace(scrnIdPrefix, "");
        }

        [JSInvokable]
        public async Task<string> gridView_OnEditGridActionAsync(string jsonEventData) {
            JObject jObj = JObject.Parse(jsonEventData);
             
            FieldGridActionEventArgs arg = new FieldGridActionEventArgs();
            arg.FieldId = jObj["FieldId"].ToString();
            arg.PreviousValue = jObj["PreviousValue"].ToString();
            arg.Value = jObj["Value"].ToString();
            arg.DataSource = JObject.FromObject(jObj["Value"]);
            arg.EventType = jObj["EventType"].ToString().ToEnum<FieldGridActionEventType>();

            FieldGridView gridview = (FieldGridView)Fields.Search(arg.FieldId);
            
            if (arg.EventType == FieldGridActionEventType.onInitNewRow && gridview.GridType == FieldGridViewType.Editable) {

                if (ScrnMode == ScreenMode.Add && gridview.SaveOnNavigate) {
                    gridview.InitEditRow(arg.DataSource, FieldGridEditRowAction.eOnInsert);
                    gridview.EditRow.Initiating = true;
                }
                else {
                    if (gridview.EditRow == null) {
                        gridview.InitEditRow(arg.DataSource, FieldGridEditRowAction.eOnInsert);
                    }
                    if (!gridview.EditRow.Initiating) {
                        gridview.InitEditRow(arg.DataSource, FieldGridEditRowAction.eOnInsert);
                    }
                    else {
                        gridview.EditRow.SetDataSource(arg.DataSource, FieldGridEditRowAction.eOnInsert);
                        gridview.EditRow.Initiating = false;  // InitEditRow is now completed, set a flag
                    }
                    
                    gridview.EditRow.Ok = await InvokeEditGridActionAsync(gridview.ID);
                }
                
                return JObject.FromObject(gridview.EditRow).ToStr();

            }
            else if (arg.EventType == FieldGridActionEventType.onRowInserting && gridview.GridType == FieldGridViewType.Editable) {

                //FieldGridEditRow e = gridview.CreateEditRow(Session, arg.DataSource, FieldGridEditRowAction.eOnBeforeInsert);

                gridview.EditRow.SetDataSource(arg.DataSource, FieldGridEditRowAction.eOnBeforeInsert);

                //call for validation from BO
                gridview.EditRow.Ok = await InvokeEditGridActionAsync(gridview.ID);
                if (gridview.EditRow.Ok) {
                    // validation success
                    // insert record to database
                    gridview.EditRow.Ok = await SaveAsync("I", gridview.NavigateScrnDefinition.BASE_PROC, gridview.EditRow.Data);
                    if (!gridview.EditRow.Ok) {
                        Session.ToastMessage("Error while saving record!", ToastMessageType.error);
                    }
                }
                return JObject.FromObject(gridview.EditRow).ToStr();

            }
            else if (arg.EventType == FieldGridActionEventType.onRowInserted && gridview.GridType == FieldGridViewType.Editable) {

                gridview.EditRow.SetDataSource(arg.DataSource, FieldGridEditRowAction.eOnAfterInsert);
                gridview.EditRow.Ok = await InvokeEditGridActionAsync(gridview.ID);
                gridview.RemoveEditRow();
                return string.Empty;

            }
            else if (arg.EventType == FieldGridActionEventType.onEditingStart && gridview.GridType == FieldGridViewType.Editable) {

                gridview.InitEditRow(arg.DataSource, FieldGridEditRowAction.eOnUpdate);
                gridview.EditRow.Ok = await InvokeEditGridActionAsync(gridview.ID);
                return JObject.FromObject(gridview.EditRow).ToStr();

            }
            else if (arg.EventType == FieldGridActionEventType.onRowUpdating && gridview.GridType == FieldGridViewType.Editable) {

                gridview.EditRow.SetDataSource(arg.DataSource, FieldGridEditRowAction.eOnBeforeUpdate);

                //call for validation from BO
                gridview.EditRow.Ok = await InvokeEditGridActionAsync(gridview.ID);
                if (gridview.EditRow.Ok) {
                    // validation success
                    // update record to database
                    gridview.EditRow.Ok = await SaveAsync("U", gridview.NavigateScrnDefinition.BASE_PROC, gridview.EditRow.Data);
                    if (!gridview.EditRow.Ok) {
                        Session.ToastMessage("Error while updating record!", ToastMessageType.error);
                    }
                }
                return JObject.FromObject(gridview.EditRow).ToStr();

            }
            else if (arg.EventType == FieldGridActionEventType.onRowUpdated && gridview.GridType == FieldGridViewType.Editable) {

                gridview.EditRow.SetDataSource(arg.DataSource, FieldGridEditRowAction.eOnAfterUpdate);
                gridview.EditRow.Ok = await InvokeEditGridActionAsync(gridview.ID);
                gridview.RemoveEditRow();
                return string.Empty;

            }
            else if (arg.EventType == FieldGridActionEventType.onRowRemoving && gridview.AllowDeleting) {

                gridview.InitEditRow(arg.DataSource, FieldGridEditRowAction.eOnDelete);

                gridview.EditRow.Ok = await InvokeEditGridActionAsync(gridview.ID);
                if (gridview.EditRow.Ok) {
                    gridview.EditRow.SetDataSource(arg.DataSource, FieldGridEditRowAction.eOnBeforeDelete);
                    gridview.EditRow.Ok = await InvokeEditGridActionAsync(gridview.ID);
                    if (gridview.EditRow.Ok) {
                        // validation success
                        // delete record from database
                        if (!string.IsNullOrEmpty(gridview.NavigateScrnId)) {
                            gridview.EditRow.Ok = await DeleteRec(gridview.NavigateScrnDefinition.BASE_PROC, gridview.EditRow.Data);
                            if (!gridview.EditRow.Ok) {
                                Session.ToastMessage("Error while deleting record!", ToastMessageType.error);
                            }
                        }
                    }
                }
                return JObject.FromObject(gridview.EditRow).ToStr();

            }
            else if (arg.EventType == FieldGridActionEventType.onRowRemoved && gridview.AllowDeleting) {
                gridview.EditRow.SetDataSource(arg.DataSource, FieldGridEditRowAction.eOnAfterDelete);
                gridview.EditRow.Ok = await InvokeEditGridActionAsync(gridview.ID);
                gridview.RemoveEditRow();
                return string.Empty;

            }
            else if (arg.EventType == FieldGridActionEventType.onDownloadClick) {
                // perform download action
                gridview.InitEditRow(arg.DataSource, FieldGridEditRowAction.onDownloadClick);
                gridview.EditRow.Ok = await InvokeEditGridActionAsync(gridview.ID);
                gridview.RemoveEditRow();
                return string.Empty;
            }


            return "";
        }

        [JSInvokable]
        public async Task<string> accordion_OnAccordionActionAsync(string jsonEventData) {
           
            JObject jObj = JObject.Parse(jsonEventData);

            FieldGridActionEventArgs arg = new FieldGridActionEventArgs();
            arg.FieldId = jObj["FieldId"].ToString();
            arg.PreviousValue = jObj["PreviousValue"].ToString();
            arg.Value = jObj["Value"].ToString();
            arg.DataSource = JObject.FromObject(jObj["Value"]);
            arg.EventType = jObj["EventType"].ToString().ToEnum<FieldGridActionEventType>();

            FieldAccordion accordion = (FieldAccordion)Fields.Search(arg.FieldId);
            
            if (arg.EventType == FieldGridActionEventType.onRowRemoving) {

                accordion.InitEditRow(Session, arg.DataSource, FieldGridEditRowAction.eOnDelete);
                accordion.EditRow.Ok = await InvokeEditGridActionAsync(accordion.ID);
                if (accordion.EditRow.Ok) {
                    accordion.EditRow.SetDataSource(arg.DataSource, FieldGridEditRowAction.eOnBeforeDelete);
                    accordion.EditRow.Ok = await InvokeEditGridActionAsync(accordion.ID);
                    if (accordion.EditRow.Ok) {
                        // validation success
                        // delete record from database
                        if (!string.IsNullOrEmpty(accordion.NavigateScrnId)) {
                            accordion.EditRow.Ok = await DeleteRec(accordion.NavigateScrnDefinition.BASE_PROC, accordion.EditRow.Data);
                            if (!accordion.EditRow.Ok) {
                                Session.ToastMessage("Error while deleting record!", ToastMessageType.error);
                            }
                        }

                        if (accordion.EditRow.Ok) {
                            accordion.EditRow.SetDataSource(arg.DataSource, FieldGridEditRowAction.eOnAfterDelete);
                            accordion.EditRow.Ok = await InvokeEditGridActionAsync(accordion.ID);
                            if (accordion.EditRow.Ok) {
                                await RefreshGridViewDataAsync(accordion.ID);
                            }
                        }

                    }
                }
                accordion.RemoveEditRow();
                return string.Empty;

            }
            else if (arg.EventType == FieldGridActionEventType.onDownloadClick) {
                // perform download action
                accordion.InitEditRow(Session, arg.DataSource, FieldGridEditRowAction.onDownloadClick);
                accordion.EditRow.Ok = await InvokeEditGridActionAsync(accordion.ID);
                accordion.RemoveEditRow();
                return string.Empty;
            }



            return "";
        }


        private async Task<bool> InvokeEditGridActionAsync(string gridFieldId) {
            bool ret = true;
            List<EPF_SCRN_EVENTS> events = _epfScrnDef.EVENTS.FindAll(x => x.OBJ_ID == gridFieldId && x.EVENT_ID == "ongridaction");
            if (events.Count > 0) {
                foreach (EPF_SCRN_EVENTS evt in events) {
                    var type = this.GetType();
                    var method = type.GetMethod(evt.BO_NAME);
                    if (method.ReturnType.Name.Contains("Task")) {
                        Task<bool> editGridActionTask = (Task<bool>)method.Invoke(this, null);
                        ret = await editGridActionTask;
                    }
                    else {
                        Session.ToastMessage("Error while calling OnGridAction event.", ToastMessageType.error);
                        ret = false;
                    }
                }
            }
            return ret;
        }

        

        private async void Field_OnClick(object sender, FieldClickEventArgs e) {
            Field field = (Field)sender;

            if (field is FieldButton) {
                FieldButton button = (FieldButton)field;

                //Actions base on button type
                switch (button.ButtonType) {
                    case FieldButtonType.epfMoveNext:
                        await this.NavigateRecordAsync(_nextRecOffset);
                        break;
                    case FieldButtonType.epfMovePrev:
                        await this.NavigateRecordAsync(_prevRecOffset);
                        break;
                    case FieldButtonType.epfMultiPick: //Action for multipick handled in multipick_OnClick event
                        break;
                    case FieldButtonType.epfUpload: // Action for Upload button
                        if (button.SaveOnNavigate) { // Open Upload screen only if saved successful when SaveOnNavigate
                            bool isValid = await IsValidEntryAsync(true);
                            if (isValid) {
                                bool isSaved = await ScreenSaveRecAsync(false);
                                if (isSaved) {
                                    // reload parent if SaveOnNavigate is true
                                    await OpenPopupScreenAsync("FILE_UPLD_BLZ", ScreenMode.Add, "READ_ONLY", "READ_ONLY", button.FileUploadReadOnly.ToStr(), true);
                                }
                                else {
                                    Session.ToastMessage("Error while saving record!", ToastMessageType.error);
                                }
                            }
                        }
                        else {
                            await OpenPopupScreenAsync("FILE_UPLD_BLZ", ScreenMode.Add, "READ_ONLY", "READ_ONLY", button.FileUploadReadOnly.ToStr(), false);
                        }
                        
                        break;

                    case FieldButtonType.epfNormal: // Action for normal button
                        if (button.SaveOnNavigate) { // Execute event function only of saved successful if SaveOnNavigate
                            bool isValid = await IsValidEntryAsync(true);
                            if (isValid) {
                                bool isSaved = await ScreenSaveRecAsync(false);
                                if (isSaved) {
                                    _epfScrnDef.EVENTS.FindAll(x => x.OBJ_ID == field.ID && x.EVENT_ID == "onclick").ForEach(y => {
                                        var type = this.GetType();
                                        var method = type.GetMethod(y.BO_NAME);
                                        this.InvokeEventMethod(field, method, true);
                                    });
                                }
                                else {
                                    Session.ToastMessage("Error while saving record!", ToastMessageType.error);
                                }
                            }
                        }
                        else {
                            _epfScrnDef.EVENTS.FindAll(x => x.OBJ_ID == field.ID && x.EVENT_ID == "onclick").ForEach(y => {
                                var type = this.GetType();
                                var method = type.GetMethod(y.BO_NAME);
                                this.InvokeEventMethod(field, method, true);
                            });
                        }
                        break;

                    case FieldButtonType.epfHistoryBack: // Action for HistoryBack button
                        if (button.SaveOnNavigate) { // Execute HistoryBack only of saved successful if SaveOnNavigate
                            if (await IsValidEntryAsync(true)) {
                                if (await ScreenSaveRecAsync()) {
                                    await HistoryBack();
                                }
                                else {
                                    Session.ToastMessage("Error while saving record!", ToastMessageType.error);
                                }
                            }
                        }
                        else {
                            await HistoryBack();
                        }
                        break;

                    case FieldButtonType.epfAdd: // Action for Add button
                        ShowLoadPanel();
                        await ChangeModeAsync(ScreenMode.Add);
                        HideLoadPanel();
                        break;

                    case FieldButtonType.epfSave: // Action for Save button
                        if (await IsValidEntryAsync(false)) {
                            if (await ScreenSaveRecAsync()) {
                                Session.ToastMessage("Record has been saved successfully!", ToastMessageType.success);
                            }
                            else {
                                Session.ToastMessage("Error while saving record!", ToastMessageType.error);
                            }
                        }
                        break;

                    case FieldButtonType.epfDelete: // Action for Delete button
                        if ((_scrnComponent.MODE == ScreenMode.Update || _scrnComponent.MODE == ScreenMode.Enquiry) && _recordFetched) {
                            await ScreenDeleteAsync();
                        }
                        break;

                }

            }
            else {
                _epfScrnDef.EVENTS.FindAll(x => x.OBJ_ID == field.ID && x.EVENT_ID == "onclick").ForEach(y => {
                    var type = this.GetType();
                    var method = type.GetMethod(y.BO_NAME);
                    this.InvokeEventMethod(field, method, true);
                });
            }


        }

        private void Field_OnFocusOut(object sender, EventArgs e) {
            Field field = (Field)sender;
            _epfScrnDef.EVENTS.FindAll(x => x.OBJ_ID == field.ID && x.EVENT_ID == "onblur").ForEach(y => {
                var type = this.GetType();
                var method = type.GetMethod(y.BO_NAME);
                this.InvokeEventMethod(field, method, false);
            });
        }
        private void Field_OnFocusIn(object sender, EventArgs e) {
            Field field = (Field)sender;
            _epfScrnDef.EVENTS.FindAll(x => x.OBJ_ID == field.ID && x.EVENT_ID == "onfocus").ForEach(y => {
                var type = this.GetType();
                var method = type.GetMethod(y.BO_NAME);
                this.InvokeEventMethod(field, method, false);
            });
        }
        private void Field_OnChange(object sender, FieldChangeEventArgs e) {
            Field field = (Field)sender;

            if (field is FieldPickList) {
                FieldPickList picklist = (FieldPickList)field;
                JObject selectedRow = e.DataSource;
                if (selectedRow != null) {
                    if (picklist.FieldType == "N" && !e.Value.IsNumeric()) {
                        if (selectedRow.ContainsKey(picklist.ID)) {
                            selectedRow[picklist.ID] = "";
                        }
                    }
                    picklistAssignFields(selectedRow, picklist.BrwsTarget, picklist.BrwsAssign);
                }
                else {
                    if (picklist.FieldType == "N" && !e.Value.IsNumeric()) {
                        e.Value = "";
                    }
                    SetFieldValue(picklist, e.Value);
                }
                Refresh();
            }
            else {
                SetFieldValue(field, field.Value);
            }

            _epfScrnDef.EVENTS.FindAll(x => x.OBJ_ID == field.ID && x.EVENT_ID == "onchange").ForEach(y => {    
                var type = this.GetType();
                var method = type.GetMethod(y.BO_NAME);
                this.InvokeEventMethod(field, method, false);
            });


            //if (!string.IsNullOrEmpty(field.GetFieldType())) {
            //    await Session.ExecJSAsync("_assign_field_prop", field.ElementID, field.GetFieldProp().ToString());
            //}

            //// handle code/desc for editor fields
            //if (field is FieldEditor) { 
            //    // get description of value for code desc field.
            //    FieldEditor editorField = (FieldEditor)field;
            //    if (editorField.IsCodeDesc) {
            //        Task _ = Task.Run(async () => {
            //            editorField.CodeDesc = "";
            //            if (!string.IsNullOrEmpty(editorField.Value.ToStr())) {
            //                var desc = await GetCodeDescAsync(editorField.SqlDesc);
            //                if (desc != null) {
            //                    editorField.CodeDesc = desc;
            //                    _scrnComponent.Refresh();
            //                }
            //            }

            //            if (!string.IsNullOrEmpty(field.GetFieldType())) {
            //                await Session.ExecJSAsync("_assign_field_prop", field.ElementID, field.GetFieldProp().ToString());
            //            }
            //        });
            //    }
            //}

        }

        private void InvokeEventMethod(Field field, System.Reflection.MethodInfo method, bool disableField) {
            if (disableField) {
                SetFieldDisable(field.ID, true);
            }

            Task.Run(() => {
                if (method.ReturnType.Name.Contains("Task")) {
                    Task result = (Task)method.Invoke(this, null);
                    result.Wait();
                }
                else {
                    method.Invoke(this, null);
                }
            }).ContinueWith(x => {
                if (disableField) {
                    SetFieldDisable(field.ID, false);
                }
                _scrnComponent.Refresh();
            });
        }

        private async void multiPick_OnSelectionChange(object sender, FieldChangeEventArgs e) {
            FieldMultiPick multiPick = (FieldMultiPick)sender;
            SetFieldValue(multiPick.ID, e.Value);

            if (multiPick.TwoStepMultiPick) {
                if (!string.IsNullOrEmpty(multiPick.SelectedGroup["UPD_PROCE"].ToStr())
                    && e.Value != "[]" && !string.IsNullOrEmpty(e.Value)
                    && !string.IsNullOrEmpty(multiPick.SelectedGroup["PKLT_RTN_FLDS"].ToStr())) {

                    if (multiPick.SelectedGroup != null) {

                        // execute multipick procedure
                        string url = string.Format("EpfScreen/ExecMultiPickSelection");
                        JObject postData = new JObject();
                        postData.Add("selectedGroup", multiPick.SelectedGroup);
                        postData.Add("selectedRecords", JArray.Parse(e.Value));

                        JObject data = JObject.Parse(_dataSource.ToStr());
                        foreach (var item in multiPick.KeyValues) {
                            if (data.ContainsKey(item.Key)) {
                                data[item.Key] = item.Value;
                            }
                            else {
                                data.Add(item.Key, item.Value);
                            }
                        }
                        postData.Add("dataSource", data);
                        await _session.PostAsync(url, postData);

                        // if SaveOnNavigate then screen will reload at multipick_OnClose event else only refresh gridviews
                        if (!multiPick.MultiPickButton.SaveOnNavigate) {
                            await RefreshGridViewsAsync();
                        }
                            
                    }
                }
            }
            multiPick.InvokeOnChange(sender, e);
        }
        private async void multiPick_OnClose(object sender, EventArgs e) {
            FieldMultiPick multiPick = (FieldMultiPick)sender;

            // call multipick button onClick event
            _epfScrnDef.EVENTS.FindAll(x => x.OBJ_ID == multiPick.ID && x.EVENT_ID == "onclick").ForEach(y => {
                var type = this.GetType();
                var method = type.GetMethod(y.BO_NAME);
                this.InvokeEventMethod(multiPick, method, true);
            });


            //Refresh the screen if SaveOnNavigate is true
            if (multiPick.MultiPickButton.SaveOnNavigate) {
                if (ScrnMode == ScreenMode.Add) {
                    await ChangeModeAsync(ScreenMode.Update);
                }
                else if (ScrnMode == ScreenMode.Update) {
                    _scrnComponent.ShowLoadPanel();
                    await InitParamsAsync();
                    await InitDataAsync();
                    await InitFieldsAsync(false);
                    await ScreenShowRecAsync();
                    _scrnComponent.HideLoadPanel();
                    _scrnComponent.Refresh();
                }
            }
        }
        private async void multiPick_OnGroupSelectionChange(object sender, FieldChangeEventArgs e) {

            FieldMultiPick multiPick = (FieldMultiPick)sender;
            multiPick.HideGrid();

            JArray selectedRows = JArray.Parse(e.Value);
            if (selectedRows.Count > 0) {
                multiPick.SelectedGroup = JObject.FromObject(selectedRows[0]);
                multiPick.BrwsGrid.BRWS_ID = multiPick.SelectedGroup["BRWS_ID"].ToStr();

                string url = string.Format("EpfScreen/GetBrwsData");
                JObject postData = new JObject();
                postData.Add("brwsId", multiPick.SelectedGroup["BRWS_ID"]);
                
                foreach (var item in multiPick.KeyValues) {
                    if (_dataSource.ContainsKey(item.Key)) {
                        _dataSource[item.Key] = item.Value;
                    }
                    else {
                        _dataSource.Add(item.Key, item.Value);
                    }
                }
                postData.Add("dataSource", _dataSource);
                postData.Add("baseView", SCRN_DEF.BASE_VIEW);
                JObject brwsData = await _session.PostJsonAsync<JObject>(url, postData);
                if (brwsData.ContainsKey("brws") && brwsData.ContainsKey("cols")) {

                    EPF_BRWS brws = brwsData["brws"].ToObject<EPF_BRWS>();
                    JObject colInfos = JObject.FromObject(brwsData["cols"]);
                    if (brwsData.ContainsKey("rows")) {
                        multiPick.BrwsGrid.DataSource = JArray.FromObject(brwsData["rows"]);
                    }
                    multiPick.BrwsGrid.InitColumns(colInfos.ToObject<Dictionary<string, EPF_BRWS_COL>>(), brws.COL_WIDTH_0, brws.COL_HDR_0, brws.COL_FORMAT_0, brws.COL_ALIGN_0);
                }
                multiPick.BrwsGrid.SelectionMode = multiPick.SelectionMode;
                multiPick.ShowGrid();
                _scrnComponent.Refresh();
            }

        }

        private async void multiPick_OnClick(object sender, FieldClickEventArgs e) {

            FieldMultiPick multiPick = (FieldMultiPick)sender;

            //Save the screen if SaveOnNavigate is true
            if (multiPick.MultiPickButton.SaveOnNavigate) {

                if (ScrnMode == ScreenMode.Add || ScrnMode == ScreenMode.Update) {
                    string cmd = "I";
                    if (this.ScrnMode == ScreenMode.Update) {
                        cmd = "U";
                    }

                    if (await IsValidEntryAsync(true)) {
                        await SaveAsync(cmd);
                    }
                    else {
                        return;
                    }
                }

            }

            await multiPick.ShowPopup(Session);
            _scrnComponent.Refresh();
            multiPick.ReInitBrwsGrid();

            string url = string.Format("EpfScreen/GetBrwsData");
            JObject postData = new JObject();

            postData.Add("brwsId", multiPick.BrwsId);
            //JObject data = JObject.Parse(_dataSource.ToStr());
            foreach (var item in multiPick.KeyValues) {
                if (_dataSource.ContainsKey(item.Key)) {
                    _dataSource[item.Key] = item.Value;
                }
                else {
                    _dataSource.Add(item.Key, item.Value);
                }
            }
            postData.Add("dataSource", _dataSource);
            postData.Add("baseView", SCRN_DEF.BASE_VIEW);

            JObject brwsData = await _session.PostJsonAsync<JObject>(url, postData);
            if (brwsData.ContainsKey("brws") && brwsData.ContainsKey("cols")) {

                EPF_BRWS brws = brwsData["brws"].ToObject<EPF_BRWS>();
                JObject colInfos = JObject.FromObject(brwsData["cols"]);
                if (brwsData.ContainsKey("rows")) {
                    multiPick.BrwsGrid.DataSource = JArray.FromObject(brwsData["rows"]);
                }
                multiPick.BrwsGrid.InitColumns(colInfos.ToObject<Dictionary<string, EPF_BRWS_COL>>(), brws.COL_WIDTH_0, brws.COL_HDR_0, brws.COL_FORMAT_0, brws.COL_ALIGN_0);
            }

            if (multiPick.TwoStepMultiPick) {
                multiPick.BrwsGrid.SelectionMode = FieldGridViewSelectionMode.single;
                multiPick.IsGroupSelection = true;
            }
            else {
                multiPick.BrwsGrid.SelectionMode = multiPick.SelectionMode;
            }

            multiPick.ShowGrid();
            _scrnComponent.Refresh();

        }

        private async void titleBar_OnItemClick(object sender, FieldTitleBarMenuItem item) {
            this.ClearFocus();


            if (item is FieldTitleBarMenuItemNew) {
                ShowLoadPanel();
                await ChangeModeAsync(ScreenMode.Add);
                HideLoadPanel();
            }
            else if (item is FieldTitleBarMenuItemDelete) {
                await ScreenDeleteAsync();
            }
            else if (item is FieldTitleBarMenuItemReturn) {
                await HistoryBack();
            }
            else if (item is FieldTitleBarMenuItemClose) {
                await _popupScreen.Hide(Session, _navManager);
            }
            else if (item is FieldTitleBarMenuItemNext) {
                await NavigateRecordAsync(_nextRecOffset);
            }
            else if (item is FieldTitleBarMenuItemPrev) {
                await NavigateRecordAsync(_prevRecOffset);
            }
            else if (item is FieldTitleBarMenuItemSave) {
                _scrnComponent.ShowLoadPanel();
                if (await IsValidEntryAsync(false))
                {
                    if (await ScreenSaveRecAsync())
                    {
                        Session.ToastMessage("Record has been saved successfully!", ToastMessageType.success);
                    }
                    else
                    {
                        Session.ToastMessage("Error while saving record!", ToastMessageType.error);
                    }
                    // _scrnComponent.HideLoadPanel(); // <-- cause error in RefreshGridViews due to state change before grid properties initialize
                }
                else {
                    _scrnComponent.HideLoadPanel();
                }
            }

        }

        private async Task NavigateRecordAsync(int offset) {
            if (!_navigateRecord) {
                return;
            }

            // only should navigate on Enquiry or Update mode
            if (!(this.ScrnMode == ScreenMode.Enquiry || this.ScrnMode == ScreenMode.Update)) {
                return;
            }

            // confirm to save before navigating to another record
            if (this.ConfirmOnNavigateRecord) {
                if (_originalDataSource.ToStr() != _dataSource.ToStr()) {
                    bool confirmToSave = await Session.ConfirmMessageAsync("Confirm Navigation",
                        "Do you want to save your changes to the current record before moving?", "Yes", "No");
                    if (confirmToSave) {
                        bool saveSuccess = await this.ScreenSaveRecAsync(false);
                        if (!saveSuccess) {
                            return;
                        }
                    }
                }
            }

            this.ShowLoadPanel();

            JObject navigateRecKeys = null;
            if (offset > 0) {
                navigateRecKeys = _nextRecKeys;
            }
            else {
                navigateRecKeys = _prevRecKeys;
            }

            if (navigateRecKeys == null) {
                navigateRecKeys = await GetNavigateRecordKeysAsync(offset);
            }

            if (navigateRecKeys != null) {
                string param = ConstructParam(navigateRecKeys, this.FIELD_KEYS, this.FIELD_KEYS);

                await this.NavigateToAsync(this.ScrnId, this.ScrnMode, this.FIELD_KEYS, this.PARENT_KEYS, param, false);
            }
            else {

                this.HideLoadPanel();

                if (offset > 0) {
                    Session.ToastMessage("You are at last record.", ToastMessageType.info);
                }
                else {
                    Session.ToastMessage("You are at first record.", ToastMessageType.info);
                }
            }
        }

        private async Task<JObject> GetNavigateRecordKeysAsync(int offset) {
            string url = string.Format("EpfScreen/GetNavigateRecordKeys");
            JObject postData = new JObject();
            postData.Add("baseView", this.BASE_VIEW);
            postData.Add("brwsId", this.BASE_BRWS);
            postData.Add("dataSource", this.DataSource);
            postData.Add("fieldKeys", this.FIELD_KEYS);
            postData.Add("offsetFromCurrent", offset);

            JObject navigateRecKeys = await _session.PostJsonAsync<JObject>(url, postData);
            return navigateRecKeys;
        }

        private string ConstructParam(JObject data, string targetKeyStr, string screenKeyStr) {
            string param = "";
            if (string.IsNullOrEmpty(targetKeyStr)) {
                return param;
            }

            string[] targetKeys = targetKeyStr.Trim().Split(',');
            string[] screenKeys = screenKeyStr.Trim().Split(',');
            for (int i = 0; i <= targetKeys.Length - 1; i++) {
                if (param != "") {
                    param = string.Format("{0},", param);
                }

                string key = targetKeys[i];
                if (i < screenKeys.Length && !string.IsNullOrEmpty(screenKeys[i])) {
                    key = screenKeys[i];
                }

                if (data.ContainsKey(key)) {
                    param = param + data[key].ToString();
                }
            }
            return param;
        }
        private async void gridView_OnAddNewClick(object sender, FieldClickEventArgs e) {
            FieldGridView gridView = (FieldGridView)sender;

            if (gridView.GridType == FieldGridViewType.Navigate) {

                if (gridView.SaveOnNavigate) {
                    if (!(await IsValidEntryAsync(true))) {
                        return;
                    }

                    if (!(await ScreenSaveRecAsync(false))) {
                        return;
                    }

                }

                string param = ConstructParam(_dataSource, gridView.TargetKeys, gridView.ScreenKeys);

                string parentKeys = gridView.ScreenKeys;
                if (string.IsNullOrEmpty(parentKeys)) {
                    parentKeys = _epfScrnDef.FIELD_KEYS;
                }

                if (gridView.PopupLinkScreen) {
                    await this.OpenPopupScreenAsync(gridView.NavigateScrnId, ScreenMode.Add, gridView.TargetKeys, parentKeys, param);
                }
                else {
                    await this.NavigateToAsync(gridView.NavigateScrnId, ScreenMode.Add, gridView.TargetKeys, parentKeys, param);
                }

            }
            else if (gridView.GridType == FieldGridViewType.Editable) {
                if (this.ScrnMode == ScreenMode.Add && gridView.SaveOnNavigate) {
                    if (!(await IsValidEntryAsync(true))) {
                        return;
                    }

                    JObject options = new JObject();
                    JObject detailGridAciton = new JObject();
                    detailGridAciton.Add("gridId", gridView.ID);
                    detailGridAciton.Add("action", "addNew");
                    options.Add("detailGridAciton", detailGridAciton);

                    if (!(await ScreenSaveRecAsync(false, false, options))) {
                        return;
                    }
                }
                else {
                    await Session.ExecJSAsync("_dxdatagrid_add_row", this.SCRN_ID, gridView.ID);
                }
            }
        }
        private async void gridView_OnSelectionChanged(object sender, FieldChangeEventArgs e) {
            if (e.EventType == "onSelectionChanged") {
                FieldGridView gridView = (FieldGridView)sender;
                if (gridView.GridType == FieldGridViewType.Navigate) {
                    if (gridView.SaveOnNavigate) {
                        if (!(await IsValidEntryAsync(true))) {
                            return;
                        }

                        if (!(await ScreenSaveRecAsync(false))) {
                            return;
                        }
                    }

                    JObject selectedRow = JObject.Parse(e.Value);
                    string param = ConstructParam(selectedRow, gridView.TargetKeys, gridView.ScreenKeys);

                    string parentKeys = gridView.ScreenKeys;
                    if (string.IsNullOrEmpty(parentKeys)) {
                        parentKeys = _epfScrnDef.FIELD_KEYS;
                    }

                    if (gridView.PopupLinkScreen) {
                        await this.OpenPopupScreenAsync(gridView.NavigateScrnId, ScreenMode.Update, gridView.TargetKeys, parentKeys, param);
                    }
                    else {
                        await this.NavigateToAsync(gridView.NavigateScrnId, ScreenMode.Update, gridView.TargetKeys, parentKeys, param);
                    }

                }
            }
            else if (e.EventType == "onButtonAction") {
                FieldGridView gridView = (FieldGridView)sender;
                JObject btnObj = new JObject {
                    ["fieldID"] = e.FieldId,
                    ["buttonAction"] = e.ButtonActionName,
                    ["selectedRow"] = e.Value
                };
                SetFieldValue(gridView.AssignField, btnObj.ToString());
                await this.ShowRecAsync();
            }

        }
        private async void imgList_OnImgItemClick(object sender, JToken e) {
            FieldImgList imgList = (FieldImgList)sender;
            if (GetFieldValue<string>(imgList.RESULT_FIELD) == "") {
                SetFieldValue(imgList.RESULT_FIELD, e);
                await this.ShowRecAsync();
            }
        }
        private async void accordion_OnAddNewClick(object sender, FieldClickEventArgs e) {
            FieldAccordion accordion = (FieldAccordion)sender;

            if (accordion.AccordionType == FieldAccordionType.Navigate) {

                if (accordion.SaveOnNavigate) {
                    if (!(await IsValidEntryAsync(true))) {
                        return;
                    }

                    if (!(await ScreenSaveRecAsync(false))) {
                        return;
                    }
                }

                string param = ConstructParam(_dataSource, accordion.TargetKeys, accordion.ScreenKeys);

                string parentKeys = accordion.ScreenKeys;
                if (string.IsNullOrEmpty(parentKeys)) {
                    parentKeys = _epfScrnDef.FIELD_KEYS;
                }

                if (accordion.PopupLinkScreen) {
                    await this.OpenPopupScreenAsync(accordion.NavigateScrnId, ScreenMode.Add, accordion.TargetKeys, parentKeys, param);
                }
                else {
                    await this.NavigateToAsync(accordion.NavigateScrnId, ScreenMode.Add, accordion.TargetKeys, parentKeys, param);
                }


            }
        }
        private async void accordion_OnItemDetailClick(object sender, FieldChangeEventArgs e) {
            FieldAccordion accordion = (FieldAccordion)sender;
            if (accordion.AccordionType == FieldAccordionType.Navigate) {

                if (accordion.SaveOnNavigate) {
                    if (!(await IsValidEntryAsync(true))) {
                        return;
                    }

                    if (!(await ScreenSaveRecAsync(false))) {
                        return;
                    }
                }

                JObject selectedRow = JObject.Parse(e.Value);

                string param = ConstructParam(selectedRow, accordion.TargetKeys, accordion.ScreenKeys);

                string parentKeys = accordion.ScreenKeys;
                if (string.IsNullOrEmpty(parentKeys)) {
                    parentKeys = _epfScrnDef.FIELD_KEYS;
                }

                if (accordion.PopupLinkScreen) {
                    await this.OpenPopupScreenAsync(accordion.NavigateScrnId, ScreenMode.Update, accordion.TargetKeys, parentKeys, param);
                }
                else {
                    await this.NavigateToAsync(accordion.NavigateScrnId, ScreenMode.Update, accordion.TargetKeys, parentKeys, param);
                }


            }
        }

        private string BuildUri(string uri, List<KeyValuePair<string, string>> parameters) {
            Uri newUri = new Uri(new Uri(_navManager.BaseUri), uri);
            var uriBuilder = new UriBuilder(newUri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            foreach (var param in parameters) {
                query[param.Key] = param.Value;
            }
            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri.ToString();
        }

        //private async void picklist_OnChange(object sender, FieldChangeEventArgs e) {

        //    FieldPickList picklist = (FieldPickList)sender;
        //    JObject selectedRow = e.DataSource;
        //    if (selectedRow != null) {
        //        if (picklist.FieldType == "N" && !e.Value.IsNumeric()) {
        //            if (selectedRow.ContainsKey(picklist.ID)) {
        //                selectedRow[picklist.ID] = "";
        //            }
        //        }
        //        picklistAssignFields(selectedRow, picklist.BrwsTarget, picklist.BrwsAssign);
        //    }
        //    else {
        //        if (picklist.FieldType == "N" && !e.Value.IsNumeric()) {
        //            e.Value = "";
        //        }
        //        SetFieldValue(picklist, e.Value);
        //    }

        //    //if (picklist.IsCodeDesc) {
        //    //    if (string.IsNullOrEmpty(picklist.Value.ToStr())) {
        //    //        picklist.CodeDesc = string.Empty;
        //    //    }
        //    //    else {
        //    //        var desc = await GetCodeDescAsync(picklist.SqlDesc);
        //    //        if (desc != null) {
        //    //            picklist.CodeDesc = desc;
        //    //        }
        //    //    }
        //    //}

        //    _scrnComponent.Refresh();
        //}

        private async Task<string> GetCodeDescAsync(string sqlDesc) {
            if (string.IsNullOrEmpty(sqlDesc)) {
                return string.Empty;
            }

            string url = string.Format("EpfScreen/GetCodeDesc");
            JObject postData = new JObject();
            postData.Add("sqlDesc", sqlDesc);
            postData.Add("dataSource", _dataSource);
            postData.Add("dataColInfos", JObject.FromObject(_dataColumnInfos));

            object desc = await _session.PostJsonAsync<object>(url, postData);
            if (desc != null) {
                return desc.ToString();
            }
            return null;
        }

        private async void picklist_OnPickListSelection(object sender, FieldClickEventArgs e) {
            FieldPickList picklist = (FieldPickList)sender;
            if (picklist.BrwsGrid == null) {

                string url = string.Format("EpfScreen/GetBrwsData");
                JObject postData = new JObject();
                postData.Add("brwsId", picklist.BrwsId);
                postData.Add("dataSource", _dataSource);
                postData.Add("baseView", SCRN_DEF.BASE_VIEW);

                JObject brwsData = await _session.PostJsonAsync<JObject>(url, postData);
                if (brwsData.ContainsKey("brws") && brwsData.ContainsKey("cols")) {
                    EPF_BRWS brws = brwsData["brws"].ToObject<EPF_BRWS>();
                    JObject colInfos = JObject.FromObject(brwsData["cols"]);
                    picklist.BrwsGrid = new FieldGridView();
                    picklist.BrwsGrid.BRWS_ID = picklist.BrwsId;
                    picklist.BrwsGrid.ParentScreen = this;
                    picklist.BrwsGrid.ShowTitleBar = false;
                    picklist.BrwsGrid.ScrollType = picklist.ScrollType;
                    picklist.BrwsGrid.ServerSidePaging = picklist.ServerSidePaging;
                    if (brwsData.ContainsKey("rows")) {
                        picklist.BrwsGrid.DataSource = JArray.FromObject(brwsData["rows"]);
                    }
                    picklist.BrwsGrid.InitColumns(colInfos.ToObject<Dictionary<string, EPF_BRWS_COL>>(), brws.COL_WIDTH_0, brws.COL_HDR_0, brws.COL_FORMAT_0, brws.COL_ALIGN_0);
                    picklist.BrwsGrid.Render = true;
                    _scrnComponent.Refresh();
                }

            }
            picklist.BrwsGrid.RequireRefresh = true;
            await Task.Delay(100).ContinueWith(async x => {
                await Session.ExecJSAsync("_resizePopupForPickList", picklist.ID);
            });
        }
        private void picklistAssignFields(JObject selectedRow, string brwsTarget, string brwsAssign) {

            string[] brwsTargets = brwsTarget.Replace(" ", "").Split(',');
            string[] brwsAssigns = brwsAssign.Replace(" ", "").Split(',');

            for (int i = 0; i <= brwsTargets.Length - 1; i++) {
                Field field = Fields.Search(brwsTargets[i]);
                if (field != null) {
                    object value = null;

                    if (selectedRow.ContainsKey(brwsAssigns[i])) {
                        value = selectedRow[brwsAssigns[i]];
                    }
                    
                    if (value != null) {

                        if (field is FieldPickList) {
                            FieldPickList picklist = (FieldPickList)field;
                            if (picklist.FieldType == "N" && !value.IsNumeric()) {  // numeric field checking for picklist
                                value = "";
                            }
                        }

                        this.SetFieldValue(field, value);
                    }
                    
                }
                //if (_dataSource.ContainsKey(brwsTargets[i]) && selectedRow.ContainsKey(brwsAssigns[i])) {
                //    _dataSource[brwsTargets[i]] = selectedRow[brwsAssigns[i]];
                //}
            }

        }

        #region "Helper Functions"

        private async Task ScreenDeleteAsync() {
            try
            {
                if (!await Session.ConfirmMessageAsync("Are you sure to delete this record?"))
                {
                    return;
                }

                if (await DeleteRec())
                {
                    Session.ToastMessage("Record has been deleted successfully!", ToastMessageType.success);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while deleting record! " + ex.Message);
                Session.ToastMessage("Error while deleting record!", ToastMessageType.error);
            }
        }

        private async Task<bool> DeleteRec() {

            _scrnComponent.ShowLoadPanel();

            ScreenMode prevMode = _scrnComponent.MODE;
            _scrnComponent.SetScreenMode(ScreenMode.Delete);

            bool isValid = await IsValidEntryAsync(false);
            if (!isValid)
            {
                _scrnComponent.SetScreenMode(prevMode); // re-assign previous mode when fail to delete. 
                _scrnComponent.HideLoadPanel();
                return false;
            }

            bool deleteSuccess = await DeleteRec(_epfScrnDef.BASE_PROC, _dataSource);
            if (!deleteSuccess)
            {
                _scrnComponent.SetScreenMode(prevMode); // re-assign previous mode when fail to delete. 
                _scrnComponent.HideLoadPanel();
                return false;
            }

            ScreenMode scrnModeBeforeSavedRec = this.ScrnMode;
            await SavedRecAsync(false);
            if (this.ScrnMode == scrnModeBeforeSavedRec) {
                await ChangeModeAsync(ScreenMode.Neutral);
            }

            return true;

        }

        private async Task<bool> DeleteRec(string baseProcdure, JObject data)
        {
            string cmd = "D";
            string uri = string.Format("EpfScreen/Action/{0}/{1}", baseProcdure, cmd);
            JObject postData = new JObject();
            postData.Add("DataSource", data);
            postData.Add("Session", JToken.FromObject(Session));

            HttpResponseMessage res = await _session.PostAsync(uri, postData);
            bool bOk = true;
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                bOk = true;
            }
            else
            {
                bOk = false;
            }

            return bOk;
        }

        private async Task<bool> SaveAsync() {
            string cmd = "I";
            if (this.ScrnMode == ScreenMode.Update) {
                cmd = "U";
            }
            return await SaveAsync(cmd, _epfScrnDef.BASE_PROC, _dataSource);
        }

        private async Task<bool> SaveAsync(string cmd) {
            return await SaveAsync(cmd, _epfScrnDef.BASE_PROC, _dataSource);
        }

        private async Task<bool> SaveAsync(string cmd, string scrnBaseProc, JObject data) {
            string uri = string.Format("EpfScreen/Action/{0}/{1}", scrnBaseProc, cmd);
            JObject postData = new JObject();
            postData.Add("Session", JToken.FromObject(_session));
            postData.Add("DataSource", data);
            HttpResponseMessage res = await _session.PostAsync(uri, postData);
            if (res.StatusCode == System.Net.HttpStatusCode.OK) {
                return true;
            }
            else {
                return false;
            }
        }

        protected async Task<bool> ScreenSaveRecAsync() {
            return await ScreenSaveRecAsync(true, true, null);
        }
        protected async Task<bool> ScreenSaveRecAsync(bool saveFromMenu) {
            return await ScreenSaveRecAsync(saveFromMenu, true, null);
        }
        protected async Task<bool> ScreenSaveRecAsync(bool saveFromMenu, bool showLoading) {
            return await ScreenSaveRecAsync(saveFromMenu, showLoading, null);
        }
        protected async Task<bool> ScreenSaveRecAsync(bool saveFromMenu, bool showLoading, JObject options) {

            if (showLoading) {
                _scrnComponent.ShowLoadPanel();
            }

            string cmd = "";
            bool bOk = true;
            if (_scrnComponent.MODE == ScreenMode.Add || _scrnComponent.MODE == ScreenMode.Enquiry) {
                cmd = "I";
            }
            else if (_scrnComponent.MODE == ScreenMode.Update) {
                cmd = "U";
            }

            if (await SaveAsync(cmd)) {
                _changeModeExecutedFlag = false;
                _noChangeModeFlag = !saveFromMenu;
                await SavedRecAsync(!saveFromMenu);
                _noChangeModeFlag = false;

                if (!_changeModeExecutedFlag) {
                    //saving original data to warn user for saving when NavigateRecordAsync
                    //if ChangeModeAsync called, no need to save to _originalDataSource here, it will be saved after changing mode
                    if (_scrnComponent.MODE != ScreenMode.Update) {
                        await ChangeModeAsync(ScreenMode.Update, showLoading, options);
                    }
                    else {
                        if (this.ConfirmOnNavigateRecord) {
                            _originalDataSource = JObject.Parse(_dataSource.ToStr());
                        }
                    }
                }


                if (showLoading) {
                    _scrnComponent.HideLoadPanel();
                }

                bOk = true;
            }
            else {
                bOk = false;
            }

            
            return bOk;
        }


        private async Task<bool> IsValidEntryAsync(bool saveOnNavigate) {

            switch (_scrnComponent.MODE) {

                case ScreenMode.Add:
                case ScreenMode.Update:
                    foreach (EPF_SCRN_FIELDS fieldDef in _epfScrnDef.FIELDS) {
                        Field field;
                        string fieldCheck = fieldDef.FIELD_CHK.SplitAndGetItem(_scrnComponent.MODE.ToInt());
                        switch (fieldCheck) {
                            case "M":
                                field = _fields.Search(fieldDef.FIELD_ID);
                                if (string.IsNullOrEmpty(field.Value.ToStr())) {
                                    Session.ToastMessage(string.Format("{0} cannot be blank!", field.Description), ToastMessageType.warning);
                                    return await Task.FromResult<bool>(false);
                                }

                                break;

                            case "D":
                                field = _fields.Search(fieldDef.FIELD_ID);
                                if (!field.Value.IsNumeric()) {
                                    Session.ToastMessage(string.Format("{0} must be numeric!", field.Description), ToastMessageType.warning);
                                    return await Task.FromResult<bool>(false);
                                }

                                break;

                            case "X":
                                field = _fields.Search(fieldDef.FIELD_ID);
                                if (string.IsNullOrEmpty(field.Value.ToStr())) {
                                    Session.ToastMessage(string.Format("{0} cannot be blank!", field.Description), ToastMessageType.warning);
                                    return await Task.FromResult<bool>(false);
                                }

                                if (!field.Value.IsNumeric()) {
                                    Session.ToastMessage(string.Format("{0} must be numeric!", field.Description), ToastMessageType.warning);
                                    return await Task.FromResult<bool>(false);
                                }

                                break;

                            default:
                                break;
                        }

                    }

                    break;

                default:
                    break;
            }

            return await ValidEntryAsync(saveOnNavigate);
        }


        #endregion



        #region "BO Screen Functions"
        protected Uri ToAbsoluteUri(string relativeUri)
        {
            return _navManager.ToAbsoluteUri(relativeUri);
        }

        //[15/12/2021, PhyoZin] Commented, it cause duplicate screen 
        //protected async void Reload() {
        //    await this.ChangeModeAsync(this.ScrnMode, true);
        //}

        protected async Task ReloadAsync() {
            await this.ChangeModeAsync(this.ScrnMode, true);
        }

        protected void SetFieldTabActivePage(string tabFieldId, string activePageId) {
            FieldTabContainer tab = (FieldTabContainer)_fields.Search(tabFieldId);
            if (tab != null) {
                FieldTabPage activePage = tab.TabPages.Find(x => x.ID == activePageId);
                if (!activePage.Visible) {
                    SetFieldTabPageVisible(tabFieldId, activePageId, true);
                }
                tab.ActiveTabPageId = activePageId;
            }
        }

        protected void SetFieldTabPageVisible(string tabFieldId, string pageId, bool visible) {
            FieldTabContainer tab = (FieldTabContainer)_fields.Search(tabFieldId);
            if (tab != null) {
                FieldTabPage pageForVisible = tab.TabPages.Find(x => x.ID == pageId);
                if (pageForVisible != null) {
                    pageForVisible.Visible = visible;
                }

                // reset active page to first page that is visible if active page is hidden
                FieldTabPage activePage = tab.TabPages.Find(x => x.ID == tab.ActiveTabPageId);
                if (activePage.Visible == false) {
                    foreach (FieldTabPage page in tab.TabPages) {
                        if (page.Visible) {
                            this.SetFieldTabActivePage(tabFieldId, page.ID);
                        }
                    }
                }

            }
        }

        protected void ShowLoadPanel() {
            _scrnComponent.ShowLoadPanel();
        }
        protected void ShowLoadPanel(string message) {
            _scrnComponent.ShowLoadPanel(message);
        }
        protected void HideLoadPanel() {
            _scrnComponent.HideLoadPanel();
        }
        protected string GetQueryString() {
            Uri currUri = new Uri(_navManager.Uri);
            return currUri.Query;
        }

        protected string GetQueryStringValue(string key) {
            return GetQueryStringValue(key, _navManager.Uri);
        }

        protected string GetQueryStringValue(string key, string url) {
            Uri currUri = new Uri(url);
            return HttpUtility.ParseQueryString(currUri.Query).Get(key);
        }

        protected string GetQueryParamValue(string paramKey) {
            if (_params.ContainsKey(paramKey)) {
                return _params[paramKey].ToString();
            }
            return string.Empty;
        }

        protected async void NavigateURL(String url, Boolean optPopup) {
            if (!String.IsNullOrEmpty(url)) {
                await Session.ExecJSAsync("_navigateURL", url, optPopup);
            }
        }

        protected async void NavigateV5URL(String relativeURL, Boolean optPopup) {
            await Session.NavigateV5URLAsync(_navManager.BaseUri, relativeURL, optPopup);
        }

        protected void NavigateScreen(string screenId, ScreenMode mode, string targetKeys, string parentkeys, string param) {
            NavigateScreen(screenId, mode, targetKeys, parentkeys, param, true);
        }
        protected async void NavigateScreen(string uri) {
            await this.NavigateToAsync(uri);
        }
        protected async void NavigateScreen(string screenId, ScreenMode mode, string targetKeys, string parentkeys, string param, bool saveHistoryBack) {
            _scrnComponent.ShowLoadPanel();
            await this.NavigateToAsync(screenId, mode, targetKeys, parentkeys, param, saveHistoryBack);
            //_scrnComponent.HideLoadPanel(); // <-- cause error in RefreshGridViews due to state change before grid properties initialize
        }

        protected async Task OpenPopupScreenAsync(string screenId, ScreenMode mode, string targetKeys, string parentkeys, string param) {
            await OpenPopupScreenAsync(screenId, mode, targetKeys, parentkeys, param, "Popup Screen", true, null);
        }
        protected async Task OpenPopupScreenAsync(string screenId, ScreenMode mode, string targetKeys, string parentkeys, string param, string title) {
            await OpenPopupScreenAsync(screenId, mode, targetKeys, parentkeys, param, title, true, null);
        }
        protected async Task OpenPopupScreenAsync(string screenId, ScreenMode mode, string targetKeys, string parentkeys, string param, string title, int screenWidth) {
            await OpenPopupScreenAsync(screenId, mode, targetKeys, parentkeys, param, title, true, null, screenWidth);
        }
        protected async Task OpenPopupScreenAsync(string screenId, ScreenMode mode, string targetKeys, string parentkeys, string param, bool reloadParentOnClose) {
            await OpenPopupScreenAsync(screenId, mode, targetKeys, parentkeys, param, "Popup Screen", reloadParentOnClose, null);
        }
        protected async Task OpenPopupScreenAsync(
            string screenId, ScreenMode mode, string targetKeys, string parentkeys, string param, string title, bool reloadParentOnClose, Action callBack) {

            await OpenPopupScreenAsync(screenId, mode, targetKeys, parentkeys, param, title, reloadParentOnClose, callBack, 0);

        }
        protected async Task OpenPopupScreenAsync(
            string screenId, ScreenMode mode, string targetKeys, string parentkeys, string param, 
            string title, bool reloadParentOnClose, Action callBack, int screenWidth) {

            string url = BuildUri(screenId, mode.ToInt().ToStr(), targetKeys, parentkeys, param, false);
            await OpenPopupScreenAsync(url, title, reloadParentOnClose, callBack, screenWidth);
        }

        protected async Task OpenPopupScreenAsync(string url, string title, bool reloadParentOnClose, Action callBack) {
            await OpenPopupScreenAsync(url, title, reloadParentOnClose, callBack, 0);
        }

        protected async Task OpenPopupScreenAsync(string url, string title, bool reloadParentOnClose, Action callBack, int screenWidth) {
            
            if ( string.IsNullOrEmpty(url) || !(url.Contains("EpfScreen") || url.Contains("EpfLaunch"))) {
                return;
            }

            if (!url.StartsWith("http")) { 
                Uri baseUri = new Uri(_navManager.BaseUri);
                url = new Uri(baseUri, url).ToString();
            }

            Uri uri = new Uri(url);
            string scrnId = uri.Segments.Last();

            ScreenMode mode = (ScreenMode)GetQueryStringValue("MODE", url).ToInt();
            string keys = GetQueryStringValue("KEYS", url);
            string parentKeys = GetQueryStringValue("PARENT_KEYS", url);
            string param = GetQueryStringValue("PARAM", url);

            url = BuildUri(scrnId, mode.ToInt().ToStr(), keys, parentKeys, param, false, this.SCRN_ID, EpfScreen.ScreenType.Popup);

            _popupScreen.OnClose += popupScreen_OnClose;

            _popupScreen.Title = title;
            _popupScreen.CallerScreenId = this.SCRN_ID;
            _popupScreen.URL = url;
            _popupScreen.ScreenId = scrnId;
            _popupScreen.Mode = mode;
            _popupScreen.Keys = keys;
            _popupScreen.ParentKeys = parentKeys;
            _popupScreen.Param = param;
            _popupScreen.ReloadParentOnClose = reloadParentOnClose;
            _popupScreen.CallBackFunction = callBack;
            _popupScreen.PopupSreenWidth = screenWidth;
            _popupScreen.Render = true;

            _scrnComponent.Refresh();

            await _popupScreen.Show(Session, _navManager);
        }


        protected async void Close() {
            await ClosePopupScreenAsync();
        }

        protected async Task ClosePopupScreenAsync()
        {
            if (this.ParentScreen != null) {
                await Session.ExecJSAsync("_hidePopupScreen", this.ParentScreen._popupScreen.ElementID);
            }
        }


        protected async Task PhotoUploadAsync(string returnFieldId) {
            await PhotoUploadAsync(returnFieldId, true, null, false);
        }
        protected async Task PhotoUploadAsync(string returnFieldId, bool reloadParentOnClose) {
            await PhotoUploadAsync(returnFieldId, reloadParentOnClose, null, false);
        }
        protected async Task PhotoUploadAsync(string returnFieldId, bool reloadParentOnClose, Action callBackOnClose) {
            await PhotoUploadAsync(returnFieldId, reloadParentOnClose, callBackOnClose, false);
        }

        protected async Task PhotoUploadAsync(string returnFieldId, bool reloadParentOnClose, Action callBackOnClose, bool noConfirm) {
            Action action = async () => {
                await Session.ExecJSAsync("_stopAllVideoStream");
                if (callBackOnClose != null) {
                    callBackOnClose.Invoke();
                }
            };
            string param = string.Format("{0},{1}", returnFieldId, noConfirm);
            await OpenPopupScreenAsync("EPF_PHOTO_UPLD_BLZ", ScreenMode.Add, "RETURN_FIELD_ID,NO_CONFIRM", "RETURN_FIELD_ID,NO_CONFIRM", param, 
                "Take Photo", reloadParentOnClose, action);
        }

        protected async Task OpenQueryBuilderAsync(string fieldDataId, string returnFieldId) {
            await OpenPopupScreenAsync("EPF_QRY_BDR", ScreenMode.Add, "FIELD_DATA_ID,RETURN_FIELD_ID", "FIELD_DATA_ID,RETURN_FIELD_ID", fieldDataId + "," + returnFieldId, "Query Builer", false, null);
        }

        protected async Task OpenQueryBuilderAsync(string fieldDataId, string returnFieldId, bool reloadParentOnClose) {
            await OpenPopupScreenAsync("EPF_QRY_BDR", ScreenMode.Add, "FIELD_DATA_ID,RETURN_FIELD_ID", "FIELD_DATA_ID,RETURN_FIELD_ID", fieldDataId + "," + returnFieldId, "Query Builer", reloadParentOnClose, null);
        }

        protected async Task OpenQueryBuilderAsync(string fieldDataId, string returnFieldId, bool reloadParentOnClose, Action callBackOnClose) {
            await OpenPopupScreenAsync("EPF_QRY_BDR", ScreenMode.Add, "FIELD_DATA_ID,RETURN_FIELD_ID", "FIELD_DATA_ID,RETURN_FIELD_ID", fieldDataId + "," + returnFieldId, "Query Builer", reloadParentOnClose, callBackOnClose);
        }

        protected async Task DirectUploadAsync(string returnFieldId) {
            await DirectUploadAsync(returnFieldId, true, null);
        }
        protected async Task DirectUploadAsync(string returnFieldId, bool reloadParentOnClose) {
            await DirectUploadAsync(returnFieldId, reloadParentOnClose, null);
        }
        protected async Task DirectUploadAsync(string returnFieldId, bool reloadParentOnClose, Action callBackOnClose) {
            await OpenPopupScreenAsync("FILE_DIRECT_UPLD_BLZ", ScreenMode.Add, "RETURN_FIELD_ID", "RETURN_FIELD_ID", returnFieldId, "Direct Upload", reloadParentOnClose, callBackOnClose);
        }

        protected void SetTitleBarMenuVisible(FieldTitleBarMenuItemType menuType, bool visible) {
            if (_titleBar != null) {
                _titleBar.SetMenuItemVisible(menuType, visible);
            }
        }
        protected void SetTitleBarMenuVisible(bool visible) {
            if (_titleBar != null) {
                _titleBar.MenuVisible = visible;
            }
        }
        protected void SetTitleBarText(string titleText) {
            if (_titleBar != null) {
                _titleBar.TitleText = titleText;
            }
        }

        protected void SetGridViewAddNewVisible(string fieldId, bool visible) {
            Field field = _fields.Search(fieldId);
            if (field is FieldAccordion) {
                FieldAccordion accordion = (FieldAccordion)field;
                if (accordion != null) {
                    accordion.ShowTitleBarAddNew = visible;
                }
                else {
                    throw new Exception("Unable to find Grid View ID: " + fieldId);
                }
            }
            else if (field is FieldGridView) {
                FieldGridView gridView = (FieldGridView)field;
                if (gridView != null) {
                    gridView.ShowTitleBarAddNew = visible;
                }
                else {
                    throw new Exception("Unable to find Grid View ID: " + fieldId);
                }
            }

        }
        protected void SetGridViewTitleBarVisible(string gridViewId, bool visible) {
            FieldGridView gridView = (FieldGridView)_fields.Search(gridViewId);
            if (gridView != null) {
                gridView.ShowTitleBar = visible;
            }
            else {
                throw new Exception("Unable to find Grid View ID: " + gridViewId);
            }
        }

        protected Task<bool> ChangeModeAsync(ScreenMode mode) {
            return ChangeModeAsync(mode, true, null);
        }
        protected Task<bool> ChangeModeAsync(ScreenMode mode, bool showLoading) {
            return ChangeModeAsync(mode, showLoading, null);
        }
        protected async Task<bool> ChangeModeAsync(ScreenMode mode, bool showLoading, JObject options) {

            if (_noChangeModeFlag) { 
                return await Task.FromResult<bool>(true);
            }

            _changeModeExecutedFlag = true;

            _scrnComponent.SetScreenMode(mode);
            
            string uri = "";
            if (mode == ScreenMode.Update) {
                string param = this.ConstructParam(_dataSource, _scrnComponent.KEYS, "");
                Session.PopNavigationHistory();
                
                //[22/09/2021, PhyoZin] Commented to call InitAsync directly
                //await this.NavigateToAsync(_scrnComponent.SCRN_ID, mode, _scrnComponent.KEYS, _scrnComponent.PARENT_KEYS, param, true, showLoading);

                uri = BuildUri(_scrnComponent.SCRN_ID, mode.ToInt().ToStr(), _scrnComponent.KEYS, _scrnComponent.PARENT_KEYS, param, true);
                Session.PushNavigationHistory(_navManager.BaseUri, uri);
                await Session.ExecJSAsync("_change_url", uri);
            }
            else {
                Session.PopNavigationHistory();
                
                //[22/09/2021, PhyoZin] Commented to call InitAsync directly
                //await this.NavigateToAsync(_scrnComponent.SCRN_ID, mode, _scrnComponent.KEYS, _scrnComponent.PARENT_KEYS, _scrnComponent.PARAM, true, showLoading);

                uri = BuildUri(_scrnComponent.SCRN_ID, mode.ToInt().ToStr(), _scrnComponent.KEYS, _scrnComponent.PARENT_KEYS, _scrnComponent.PARAM, true);
                Session.PushNavigationHistory(_navManager.BaseUri, uri);
                await Session.ExecJSAsync("_change_url", uri);
            }


            if (showLoading) ShowLoadPanel();

            _scrnComponent.InitQueryStringValues(uri);
            await this.InitAsync(_scrnComponent, _session, _navManager, _parentScreen, false);
            await Session.ExecJSAsync("controlEventHandler", SCRN_ID, "ScreenJSCallBack", DotNetObjectReference.Create(this));
            await this.ScreenShowRecAsync(options, showLoading);

            if (showLoading && !this.DisplayAfterRender) HideLoadPanel();

            return await Task.FromResult<bool>(true);
        }
        
        protected async Task HistoryBack() {

            // close the popup if it is last screen from the  popup window
            if (this.SCRN_TYPE == EpfScreen.ScreenType.Popup && !string.IsNullOrEmpty(this.CALLER_SCRN_ID)) {
                await ClosePopupScreenAsync();
                return;
            }
            else {
                if (Session.GetNavigationHistoryCount() > 0) {
                    Session.PopNavigationHistory();
                }
            }

            if (Session.GetNavigationHistoryCount() > 0) {

                string url = Session.PopNavigationHistory();

                List<KeyValuePair<string, string>> paramList = new List<KeyValuePair<string, string>>();
                paramList.Add(new KeyValuePair<string, string>("RETURN", "1"));
                url = BuildUri(url, paramList);

                await this.NavigateToAsync(url, true, true, true);
            }
            
        }

        private string BuildUri(string scrnId, string scrnMode, string keys, string parendKeys, string parameters) {
            return BuildUri(scrnId, scrnMode, keys, parendKeys, parameters, false);
        }
        private string BuildUri(string scrnId, string scrnMode, string keys, string parendKeys, string parameters, bool forReload) {
            return BuildUri(scrnId, scrnMode, keys, parendKeys, parameters, forReload, "", EpfScreen.ScreenType.None);
        }
        private string BuildUri(string scrnId, string scrnMode, string keys, string parendKeys, string parameters, bool forReload, string callerScrnID, EpfScreen.ScreenType scrnType) {

            Uri newUri = new Uri(new Uri(_navManager.BaseUri), string.Format("EpfScreen/{0}", scrnId));
            var uriBuilder = new UriBuilder(newUri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            //check additional existing param from current query and forward
            Uri currUri = new Uri(this.CURRENT_URL);
            var currParam = HttpUtility.ParseQueryString(currUri.Query);
            foreach (var key in currParam.Keys) {
                if (forReload) { // for reloading the same screen, will reuse "RETURN", "SCRN_TYPE", "CALLER_SCRN_ID"
                    if (!key.ToStr().In("MODE", "KEYS", "PARENT_KEYS", "PARAM")) {
                        query[key.ToStr()] = currParam.Get(key.ToStr());
                    }
                }
                else {
                    //if (!key.ToStr().In("MODE", "KEYS", "PARENT_KEYS", "PARAM", "RETURN", "SCRN_TYPE", "CALLER_SCRN_ID")) {
                    if (!key.ToStr().In("MODE", "KEYS", "PARENT_KEYS", "PARAM", "RETURN", "CALLER_SCRN_ID")) {
                        query[key.ToStr()] = currParam.Get(key.ToStr());
                    }
                }
            }

            query["MODE"] = scrnMode;
            query["KEYS"] = keys;
            query["PARENT_KEYS"] = parendKeys;
            query["PARAM"] = parameters;
            if (!string.IsNullOrEmpty(callerScrnID)) {
                query["CALLER_SCRN_ID"] = callerScrnID;
            }
            if (scrnType != EpfScreen.ScreenType.None) {
                query["SCRN_TYPE"] = scrnType.ToInt().ToStr();
            }
            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri.ToString();
        }

        public async Task NavigateToAsync(string scrnId, ScreenMode scrnMode, string keys, string parendKeys, string parameters, bool saveHistory, bool showLoading) {
            string uri = BuildUri(scrnId, scrnMode.ToInt().ToStr(), keys, parendKeys, parameters);
            await NavigateToAsync(uri, saveHistory, showLoading, false);
        }

        public async Task NavigateToAsync(string scrnId, ScreenMode scrnMode, string keys, string parendKeys, string parameters, bool saveHistory) {
            string uri = BuildUri(scrnId, scrnMode.ToInt().ToStr(), keys, parendKeys, parameters);
            await NavigateToAsync(uri, saveHistory, true, false);
        }

        public async Task NavigateToAsync(string scrnId, ScreenMode scrnMode, string keys, string parendKeys, string parameters) {
            string uri = BuildUri(scrnId, scrnMode.ToInt().ToStr(), keys, parendKeys, parameters);
            await NavigateToAsync(uri, true, true, false);
        }

        public async Task NavigateToAsync(string uri) {
            await NavigateToAsync(uri, true, true, false);
        }
        public async Task NavigateToAsync(string uri, bool saveHistory, bool showLoading, bool isHistoryBack) {

            if (!isHistoryBack && Session.GetNavigationHistoryCount() > 0) {
                if (uri == Session.PeekNavigationHistory()) {
                    // no need to naviate if uri is same as current
                    return;
                }
            }


            if (saveHistory) {
                Session.PushNavigationHistory(_navManager.BaseUri, uri);
            }
            this.Refresh();

            if (this.SCRN_TYPE == EpfScreen.ScreenType.Popup) {
                await _scrnComponent.StartInitializeAsync(uri);
                
            }
            else {
                Session.Loading = showLoading;
                _navManager.NavigateTo(uri);
            }

        }

        public void Refresh() {
            _scrnComponent.Refresh();
        }

        protected async Task RefreshImgListDataAsync(string fieldId)
        {
            Field field = _fields.Search(fieldId);
            if(field is FieldImgList)
            {
                FieldImgList imgList = (FieldImgList)field;
                if (imgList != null)
                {
                    string url = string.Format("EpfScreen/GetBrwsData");
                    JObject postData = new JObject();
                    postData.Add("brwsId", imgList.BrwsID);
                    postData.Add("dataSource", _dataSource);
                    postData.Add("baseView", SCRN_DEF.BASE_VIEW);
                    JObject brwsData = await _session.PostJsonAsync<JObject>(url, postData);
                    if (brwsData.ContainsKey("rows"))
                    {
                        if(!String.IsNullOrEmpty(imgList.DataSource.ToStr())) imgList.prevDataSource = imgList.DataSource;
                        if(!imgList.IMG_URL.Contains("/")) imgList.IMG_URL = GetFieldValue<String>(imgList.IMG_URL);
                        imgList.DataSource = JArray.FromObject(brwsData["rows"]);
                    }
                    else
                    {
                        imgList.DataSource = new JArray();
                    }
                    _scrnComponent.Refresh();
                }
            }
        }

        protected async Task RefreshCellListDataAsync(string fieldId) {
            Field field = _fields.Search(fieldId);
            if (field is FieldCellList) {
                FieldCellList cellList = (FieldCellList)field;
                if (cellList != null) {
                    string url = string.Format("EpfScreen/GetBrwsData");
                    JObject postData = new JObject();
                    postData.Add("brwsId", cellList.BrwsID);
                    postData.Add("dataSource", _dataSource);
                    postData.Add("baseView", SCRN_DEF.BASE_VIEW);
                    JObject brwsData = await _session.PostJsonAsync<JObject>(url, postData);

                    if (brwsData.ContainsKey("rows")) {
                        cellList.CellPageSize = _dataSource[cellList.CellPageSetting].ToInt();
                        cellList.CurrentPage = _dataSource[cellList.CurrentPageCtrlName].ToInt();
                        cellList.DataSource = JArray.FromObject(brwsData["rows"]);
                        SetFieldValue("CELL_PAGE_SETT", cellList.CellPageSize);
                    }
                    else {
                        cellList.DataSource = new JArray();
                    }
                }
            }
            _scrnComponent.Refresh();
        }

        protected async Task RefreshGridViewDataAsync(string fieldId) {
            Field field = _fields.Search(fieldId);
            if (field is FieldAccordion) {
                string url = "";
                FieldAccordion accordion = (FieldAccordion)field;
                if (accordion == null) {
                    return;
                }


                //Loading detial screen definition if editable
                if ((accordion.AccordionType == FieldAccordionType.Editable || accordion.DeleteRecord) && !string.IsNullOrEmpty(accordion.NavigateScrnId)) {
                    url = string.Format("EpfScreen/GetScreen/{0}", accordion.NavigateScrnId);
                    var result = await Session.GetJsonAsync<List<EPF_SCRN>>(Session.GetAbsoluteApiUrl(url));
                    if (result.Count > 0) {
                        accordion.NavigateScrnDefinition = result.First();
                    }
                    else {
                        accordion.NavigateScrnDefinition = null;
                    }
                }


                url = string.Format("EpfScreen/GetBrwsData");
                JObject postData = new JObject();
                postData.Add("brwsId", accordion.BRWS_ID);
                postData.Add("dataSource", _dataSource);
                postData.Add("baseView", SCRN_DEF.BASE_VIEW);

                if (string.IsNullOrEmpty(accordion.BRWS_ID)) {
                    return;
                }

                JObject brwsData = await _session.PostJsonAsync<JObject>(url, postData);
                if (brwsData.ContainsKey("brws") && brwsData.ContainsKey("cols")) {
                    EPF_BRWS brws = brwsData["brws"].ToObject<EPF_BRWS>();
                    JObject colInfos = JObject.FromObject(brwsData["cols"]);
                    if (brwsData.ContainsKey("rows")) {
                        accordion.DataSource = JArray.FromObject(brwsData["rows"]);
                    }
                    else {
                        accordion.DataSource = new JArray();
                    }
                    accordion.InitColumns(colInfos.ToObject<Dictionary<string, JObject>>(), brws.COL_WIDTH_0, brws.COL_HDR_0, brws.COL_FORMAT_0, brws.COL_ALIGN_0);
                }
                accordion.Render = true;
            }
            else if (field is FieldGridView) {
                string url = "";
                FieldGridView gridView = (FieldGridView)field;
                if (gridView == null || string.IsNullOrEmpty(gridView.BRWS_ID)) {
                    return;
                }

                //Loading detial screen definition if editable
                if ((gridView.GridType == FieldGridViewType.Editable || gridView.AllowDeleting) && !string.IsNullOrEmpty(gridView.NavigateScrnId)) {
                    url = string.Format("EpfScreen/GetScreen/{0}", gridView.NavigateScrnId);
                    var result = await Session.GetJsonAsync<List<EPF_SCRN>>(Session.GetAbsoluteApiUrl(url));
                    if (result.Count > 0) {
                        gridView.NavigateScrnDefinition = result.First();
                    }
                    else {
                        gridView.NavigateScrnDefinition = null;
                    }
                }

                if (!gridView.ServerSidePaging) {
                    url = string.Format("EpfScreen/GetBrwsData");
                    JObject postData = new JObject();
                    postData.Add("brwsId", gridView.BRWS_ID);
                    postData.Add("dataSource", _dataSource);
                    postData.Add("baseView", SCRN_DEF.BASE_VIEW);
                    //postData.Add("navigateScrnId", gridView.NavigateScrnId);

                    JObject brwsData = await _session.PostJsonAsync<JObject>(url, postData);
                    if (brwsData.ContainsKey("brws") && brwsData.ContainsKey("cols")) {
                        EPF_BRWS brws = brwsData["brws"].ToObject<EPF_BRWS>();
                        JObject colInfos = JObject.FromObject(brwsData["cols"]);
                        if (brwsData.ContainsKey("rows")) {
                            gridView.DataSource = JArray.FromObject(brwsData["rows"]);
                        }
                        else {
                            gridView.DataSource = new JArray();
                        }
                        gridView.InitColumns(colInfos.ToObject<Dictionary<string, EPF_BRWS_COL>>(), brws.COL_WIDTH_0, brws.COL_HDR_0, brws.COL_FORMAT_0, brws.COL_ALIGN_0);
                    }

                    //Loading detial screen definition if editable
                    //gridView.NavigateScrnDefinition = null;
                    //if (brwsData.ContainsKey("navigateScrn")) {
                    //    gridView.NavigateScrnDefinition = brwsData["navigateScrn"].ToObject<EPF_SCRN>();
                    //}
                }

                gridView.LoadData = true;
                gridView.Render = true;

            }

            _scrnComponent.Refresh();
        }
        protected internal bool HasRights(ScreenMode mode) {
            int index = _editModes.FindIndex(x => x == mode);
            if (index == -1) {
                return false; 
            }
            return true;
        }
        protected bool FieldExists(string fieldId) {
            Field field = _fields.Search(fieldId);
            if (field != null) {
                return true;
            }
            return false;
        }
        protected void SetFieldDisable(string fieldIds, bool disable) {
            foreach (string fieldId in fieldIds.Split(",")) {
                Field field = _fields.Search(fieldId);
                if (field != null) {
                    field.Disabled = disable;
                }
                else {
                    throw new Exception("Unable to find Field ID: " + fieldId);
                }
            }
        }

        protected void SetFieldDisable(ICollection<Field> fieldList, bool disable) {
            foreach (Field field in fieldList) {
                if (field != null) {
                    field.Disabled = disable;
                }
            }
        }

        protected T GetField<T>(string fieldId) {
            T field = _fields.Search<T>(fieldId);
            if (field != null) {
                return field;
            }
            else {
                throw new Exception("Unable to find Field ID: " + fieldId);
            }
        }

        protected Field GetField(string fieldId) {
            Field field = _fields.Search(fieldId);
            if (field != null) {
                return field;
            }
            else {
                throw new Exception("Unable to find Field ID: " + fieldId);
            }
        }

        protected async Task<bool> SetFieldValueAsync(string fieldId, object value)
        {
            Field field = _fields.Search(fieldId);
            if (field != null)
            {
                SetFieldValue(field, value.ToStr());
            }
            else
            {
                if (value != null)
                {
                    _dataSource[fieldId] = JToken.FromObject(value.ToStr());
                }
                else
                {

                }
            }

            return await Task.FromResult<bool>(true);
        }

        protected void SetFieldValue(string fieldId, object value) {
            if (fieldId == "USER_LOG") {
                if (_dataSource.ContainsKey("USER_LOG")) {
                    _dataSource["USER_LOG"] = value.ToStr();
                }
                else {
                    _dataSource.Add("USER_LOG", value.ToStr());
                }
                fieldId = "__CompForm_USER_LOG";
                string timeLog = _dataSource.ContainsKey("TIME_LOG") ? _dataSource["TIME_LOG"].ToDate().ToString("dd/MM/yyyy HH:mm:ss") : "";
                value = string.Format("{0} - {1}", value, timeLog);
            }

            if (fieldId == "TIME_LOG") {
                if (_dataSource.ContainsKey("TIME_LOG")) {
                    _dataSource["TIME_LOG"] = value.ToStr();
                }
                else {
                    _dataSource.Add("TIME_LOG", value.ToStr());
                }
                fieldId = "__CompForm_USER_LOG";
                string userLog = _dataSource.ContainsKey("USER_LOG") ? _dataSource["USER_LOG"].ToStr() : "";
                string timeLog = value.IsDate() ? value.ToDate().ToString("dd/MM/yyyy HH:mm:ss") : "";
                value = string.Format("{0} - {1}", userLog, timeLog);
            }

            if (fieldId == "CREA_BY") {
                if (_dataSource.ContainsKey("CREA_BY")) {
                    _dataSource["CREA_BY"] = value.ToStr();
                }
                else {
                    _dataSource.Add("CREA_BY", value.ToStr());
                }
                fieldId = "__CompForm_CREA_BY";
                string creaDate = _dataSource.ContainsKey("CREA_DATE") ? _dataSource["CREA_DATE"].ToDate().ToString("dd/MM/yyyy HH:mm:ss") : "";
                value = string.Format("{0} - {1}", value, creaDate);
            }

            if (fieldId == "CREA_DATE") {
                if (_dataSource.ContainsKey("CREA_DATE")) {
                    _dataSource["CREA_DATE"] = value.ToStr();
                }
                else {
                    _dataSource.Add("CREA_DATE", value.ToStr());
                }
                fieldId = "__CompForm_CREA_BY";
                string creaBy = _dataSource.ContainsKey("CREA_BY") ? _dataSource["CREA_BY"].ToStr() : "";
                string creaDate = value.IsDate() ? value.ToDate().ToString("dd/MM/yyyy HH:mm:ss") : "";
                value = string.Format("{0} - {1}", creaBy, creaDate);
            }


            Field field = _fields.Search(fieldId);
            if (field != null) {
                SetFieldValue(field, value.ToStr());
            }
            else {
                if (_dataSource.ContainsKey(fieldId)) {
                    _dataSource[fieldId] = JToken.FromObject(value.ToStr());
                }
                else {
                    _dataSource.Add(fieldId, JToken.FromObject(value.ToStr()));
                }
            }
        }

        protected async void SetFieldValue(Field field, object value) {

            // assign only date if date control type date
            if (field is FieldDate) {
                FieldDate dateCtrl = (FieldDate)field;
                if (dateCtrl.Type == FieldDateType.date) {
                    if (value.IsDate()) {
                        value = value.ToDate().Date;
                    }
                }
            }

            
            if (field != null) {

                // assign value;
                field.Value = value;
                _dataSource[field.ID] = JToken.FromObject(field.Value);

                // get description of value for code desc field.
                var _ = RefreshCodeDescAsync(field);

                // for label control set value to description
                if (field is FieldLabel) {
                    SetFieldDescription(field.ID, value.ToStr());
                }

            }

            if (!string.IsNullOrEmpty(field.GetFieldType())) {
                await Session.ExecJSAsync("_assign_field_prop", field.ElementID, field.GetFieldProp().ToString());
            }

        }
        protected void ClearFieldsData() {
            foreach (KeyValuePair<string, JObject> col in _dataColumnInfos) {
                if (_params.ContainsKey(col.Key) && _parentKeys.Contains<string>(col.Key) &&
                    _scrnComponent.KEYS != _scrnComponent.PARENT_KEYS) {

                    if (_dataSource.ContainsKey(col.Key)) {
                        _dataSource[col.Key] = JToken.FromObject(_params[col.Key]);
                    }
                    else {
                        _dataSource.Add(col.Key, JToken.FromObject(_params[col.Key]));
                    }

                }
                else {

                    if (_dataSource.ContainsKey(col.Key)) {
                        _dataSource[col.Key] = null;
                    }
                    else {
                        _dataSource.Add(col.Key, null);
                    }

                }
            }
        }
        
        protected void ClearFieldListDataSource(string fieldId) {
            Field field = _fields.Search(fieldId);
            if (field is FieldSelect) {
                FieldSelect selectField = (FieldSelect)field;
                selectField.ClearDataSource();
                _scrnComponent.Refresh();
            }
            else if (field is FieldList) {
                FieldList listField = (FieldList)field;
                listField.ClearDataSource();
                _scrnComponent.Refresh();
            }
        }

        
        protected void SetFieldListDataSource(string fieldId, JArray dataSource, string itemId, string itemText) {
            Field field = _fields.Search(fieldId);
            if (field is FieldSelect) {
                FieldSelect selectField = (FieldSelect)field;
                selectField.SetDataSource(dataSource, itemId, itemText);
                _scrnComponent.Refresh();
            }
            else if (field is FieldList) {
                FieldList listField = (FieldList)field;
                listField.SetDataSource(dataSource, itemId, itemText);
                _scrnComponent.Refresh();
            }
        }
        protected async void ClearFocus() {
            await Session.ExecJSAsync("_clear_focus");
        }
        protected async void SetFieldFocus(string fieldId) {
            await Session.ExecJSAsync("_set_field_focus", fieldId);
        }
        protected void SetFieldDescription(string fieldId, string description) {
            Field field = _fields.Search(fieldId);
            if (field != null) {
                field.Description = description;
                field.Render = true;
            }
            else {
                throw new Exception("Unable to find Field ID: " + fieldId);
            }
        }
        protected void SetFieldPlaceholder(string fieldId, string placeholder) {
            _fields.Search<FieldEditor>(fieldId).PlaceHolder = placeholder;
        }
        protected string GetFieldDescription(string fieldId) {
            Field field = _fields.Search(fieldId);
            if (field != null) {
                return field.Description;
            }
            else {
                throw new Exception("Unable to find Field ID: " + fieldId);
            }
        }
        protected void SetFieldVisible(string fieldId, bool visible) {
            Field field = _fields.Search(fieldId);
            if (field != null) {
                field.Visible = visible;
            }
            else {
                throw new Exception("Unable to find Field ID: " + fieldId);
            }
        }
        protected void SetFieldCssClass(string fieldId, string classNames) {
            Field field = _fields.Search(fieldId);
            if (field != null) {
                ApplyStyleClass(field, classNames);
            }
            else {
                throw new Exception("Unable to find Field ID: " + fieldId);
            }
        }

        protected async Task<object> GetFieldValueAsync(string fieldId) {
            if (_dataSource.ContainsKey(fieldId)) {
                return _dataSource[fieldId];
            }
            else {
                Field field = _fields.Search(fieldId);
                if (field != null) {
                    return await Session.ExecJSAsync("GetFieldValue", fieldId);
                }
                else {
                    return null;
                }
            }
        }
        protected object GetFieldValue(string fieldId) {
            if (_dataSource.ContainsKey(fieldId)) {
                return _dataSource[fieldId];
            }
            else {
                Field field = _fields.Search(fieldId);
                if (field != null) {
                    return Session.ExecJS("GetFieldValue", fieldId);
                }
                else {
                    return null;
                }
            }
        }

        protected T GetParentFieldValue<T>(string fieldId) {
            
            try {
                object value = GetParentFieldValue(fieldId); 
                if (value != null) {
                    if (typeof(T).Equals(typeof(string))) {
                        return (T)Convert.ChangeType(value.ToStr(), typeof(T));
                    }
                    else if (typeof(T).Equals(typeof(JObject))) {
                        return (T)Convert.ChangeType(JObject.Parse(value.ToStr()), typeof(T));
                    }
                    else if (typeof(T).Equals(typeof(DateTime))) {
                        return (T)Convert.ChangeType(value.ToDate(), typeof(T));
                    }
                    else if (typeof(T).Equals(typeof(bool)) || typeof(T).Equals(typeof(Boolean))) {
                        object result = value.ToStr().ToBool();
                        return (T)result;
                    }
                    else {
                        return (T)Convert.ChangeType(value.ToStr(), typeof(T));
                    }

                }
                else {
                    return default(T);
                }
            }
            catch (Exception) {
                return default(T);
            }
            
        }

        protected object GetParentFieldValue(string fieldId) {
            if (_parentScreen != null) {
                return _parentScreen.GetFieldValue(fieldId);
            }
            return null;
        }

        protected void SetParentFieldValue(string fieldId, object value) {
            if (_parentScreen != null) {
                _parentScreen.SetFieldValue(fieldId, value);
            }
        }

        protected T GetFieldValue<T>(string fieldId) {
            T result;

            if (_dataSource.ContainsKey(fieldId)) {
                if (typeof(T).Equals(typeof(string))) {
                    return (T)Convert.ChangeType(_dataSource[fieldId].ToString(), typeof(T));
                }
                else if (typeof(T).Equals(typeof(JObject))) {
                    return (T)Convert.ChangeType(JObject.Parse(_dataSource[fieldId].ToString()), typeof(T));
                }
                else if (typeof(T).Equals(typeof(DateTime))) {
                    return (T)Convert.ChangeType(_dataSource[fieldId].ToDate(), typeof(T));
                }
                else if (typeof(T).Equals(typeof(bool)) || typeof(T).Equals(typeof(Boolean))) {
                    object value = _dataSource[fieldId].ToStr().ToBool();
                    return (T)value;
                }
                else {
                    try {
                        result = (T)Convert.ChangeType(_dataSource[fieldId], typeof(T));
                    }
                    catch {
                        result = default(T);
                    }

                    return result;
                }

            }
            else {

                Field field = _fields.Search(fieldId);
                if (field != null) {
                    if (typeof(T).Equals(typeof(string))) {
                        return (T)Convert.ChangeType(Session.ExecJS("GetFieldValue", fieldId).ToString(), typeof(T));
                    }
                    else if (typeof(T).Equals(typeof(JObject))) {
                        return (T)Convert.ChangeType(JObject.Parse(Session.ExecJS("GetFieldValue", fieldId).ToString()), typeof(T));
                    }
                    else if (typeof(T).Equals(typeof(DateTime))) {
                        return (T)Convert.ChangeType(Session.ExecJS("GetFieldValue", fieldId).ToDate(), typeof(T));
                    }
                    else if (typeof(T).Equals(typeof(bool)) || typeof(T).Equals(typeof(Boolean))) {
                        object value = Session.ExecJS("GetFieldValue", fieldId).ToStr().ToBool();
                        return (T)value;
                    }
                    else {
                        try {
                            result = (T)Convert.ChangeType(Session.ExecJS("GetFieldValue", fieldId).ToString(), typeof(T));
                        }
                        catch {
                            result = default(T);
                        }

                        return result;
                    }

                }
                else {
                    return default(T);
                }
            }

        }
        protected virtual async Task<object> ShowRecAsync() {
            return await Task.FromResult<object>(null);
        }
        protected internal virtual async Task<object> SavedRecAsync() {
            return await Task.FromResult<object>(null);
        }
        protected internal virtual async Task<object> SavedRecAsync(bool saveOnNavigate) {
            return await SavedRecAsync();
        }
        protected virtual async Task<bool> ValidEntryAsync() {
            return await Task.FromResult<bool>(true);
        }
        protected virtual async Task<bool> ValidEntryAsync(bool saveOnNavigate) {
            return await ValidEntryAsync();
        }
        protected virtual async Task<object> LoadAsync() {
            return await Task.FromResult<object>(null);
        }
        protected virtual async Task<object> AfterRenderAsync() {
            return await Task.FromResult<object>(null);
        }

        


        #endregion



    }



}
