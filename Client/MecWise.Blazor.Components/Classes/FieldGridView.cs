using System;
using System.Collections;
using System.Collections.Generic;
using MecWise.Blazor.Common;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using MecWise.Blazor.Entities;
using System.Threading.Tasks;
using System.Linq;

namespace MecWise.Blazor.Components
{

    public class FieldGridEditRow {
        public FieldGridEditRowAction Action { get; set; }
        public Dictionary<string, FieldGridEditCell> Cells { get; set; }
        public bool Ok { get; set; } = true;

        [JsonIgnore]
        public bool Initiating { get; set; } = false;

        [JsonIgnore]
        public Field ParentField { get; set; }
        public JObject Data { get; set; }
        public FieldGridEditRow(Field sourceGridView) {
            this.ParentField = sourceGridView;
            this.Cells = new Dictionary<string, FieldGridEditCell>();
        }

        public object GetFieldValue(string fieldId) {
            if (this.Cells.ContainsKey(fieldId)) {
                return this.Cells[fieldId].Value;
            }
            return null;
        }
        public T GetFieldValue<T>(string fieldId) {
            T result;

            if (this.Cells.ContainsKey(fieldId)) {
                if (typeof(T).Equals(typeof(string))) {
                    return (T)Convert.ChangeType(this.Cells[fieldId].Value.ToString(), typeof(T));
                }
                else if (typeof(T).Equals(typeof(JObject))) {
                    return (T)Convert.ChangeType(JObject.Parse(this.Cells[fieldId].Value.ToString()), typeof(T));
                }
                else if (typeof(T).Equals(typeof(DateTime))) {
                    return (T)Convert.ChangeType(this.Cells[fieldId].Value.ToDate(), typeof(T));
                }
                else if (typeof(T).Equals(typeof(bool)) || typeof(T).Equals(typeof(Boolean))) {
                    object value = this.Cells[fieldId].Value.ToStr().ToBool();
                    return (T)value;
                }
                else {
                    try {
                        result = (T)Convert.ChangeType(this.Cells[fieldId].Value, typeof(T));
                    }
                    catch {
                        result = default(T);
                    }

                    return result;
                }

            }

            return default(T);
        }
        public void SetFieldValue(string fieldId, object value) {
            if (this.Cells.ContainsKey(fieldId)) {
                this.Cells[fieldId].Value = value;
                if (this.Data.ContainsKey(fieldId)) {
                    this.Data[fieldId] = JToken.FromObject(value.ToStr());
                }
            }
        }

        public void SetFieldDisable(string fieldId, bool disable) {
            if (this.Cells.ContainsKey(fieldId)) {
                this.Cells[fieldId].Disabled = disable;
            }
            else {
                throw new Exception("Unable to find Field ID: " + fieldId);
            }
        }
        public void SetFieldListDataSource(string fieldId, JArray dataSource, string itemId, string itemText) {
            this.Cells[fieldId].SelectData = new JArray();
            foreach (var obj in dataSource) {
                JObject item = new JObject();
                item.Add("ID", obj[itemId]);
                item.Add("TEXT", obj[itemText]);
                this.Cells[fieldId].SelectData.Add(item);
            }
        }

        public bool SetDataSource(JObject data) {
            this.Data = data;
            foreach (var prop in this.Data) {
                FieldGridEditCell editCell = this.Cells[prop.Key];

                // assign value
                editCell.Value = data[prop.Key];

                // assign default value for add/update mode if current value is empty
                if (string.IsNullOrEmpty(editCell.Value.ToStr())) {
                    if (this.Action == FieldGridEditRowAction.eOnInsert) {
                        if (editCell.DefaultValueOnAdd != null) {
                            editCell.Value = editCell.DefaultValueOnAdd;
                            this.Data[prop.Key] = JToken.FromObject(editCell.Value);
                        }
                    }
                    else if (this.Action == FieldGridEditRowAction.eOnUpdate) {
                        if (editCell.DefaultValueOnUpdate != null) {
                            editCell.Value = editCell.DefaultValueOnUpdate;
                            this.Data[prop.Key] = JToken.FromObject(editCell.Value);
                        }
                    }
                }

                // convert checkbox value to 1/0 instead of true/false
                if (editCell.CellType == FieldGridEditCellType.dxCheckBox) {
                    if (editCell.Value != null) {
                        editCell.Value = editCell.Value.ToInt();
                        this.Data[prop.Key] = JToken.FromObject(editCell.Value);
                    }
                }

            }

            return true;
        }

        public bool SetDataSource(JObject data, FieldGridEditRowAction action) {
            this.Action = action;
            return SetDataSource(data);
        }

        public void InitCell(SessionState session, EPF_SCRN_FIELDS field, FieldGridEditCell editCell) {
            
            if (field != null) {

                editCell.CellType = FieldGridEditCellType.dxTextBox; // default type, textbox
                editCell.DefaultValueOnAdd = GetDefaultValue(session, ScreenMode.Add, field);
                editCell.DefaultValueOnUpdate = GetDefaultValue(session, ScreenMode.Update, field);
                editCell.Mask = field.FIELD_MASK;

                // apply disable
                if (field.FIELD_ATTR.SplitAndGetItem(ScreenMode.Add.ToInt()) == "D") {
                    editCell.DefaultDisabledOnAdd = true;
                }

                if (field.FIELD_ATTR.SplitAndGetItem(ScreenMode.Update.ToInt()) == "D") {
                    editCell.DefaultDisabledOnUpdate = true;
                }


                if (field.CTRL_TYPE == "LIST" && field.CLS_CTRL == "clsDropDown") { // for dropdown field
                    editCell.CellType = FieldGridEditCellType.dxSelectBox;
                    dynamic ctrlProp = DynamicXml.Parse(field.CTRL_PROP_0);
                    string[] listTexts = Convert.ToString(ctrlProp.ListText).Split(',');
                    string[] listValues = Convert.ToString(ctrlProp.ListValue).Split(',');
                    editCell.SelectData = new JArray();
                    for (int i = 0; i <= listValues.Length - 1; i++) {
                        JObject selectItem = new JObject();
                        selectItem.Add("ID", listValues[i]);
                        selectItem.Add("TEXT", listTexts[i]);
                        editCell.SelectData.Add(selectItem);
                    }
                }
                else if (field.CTRL_TYPE == "DATETIME") { // for DateTime field
                    editCell.CellType = FieldGridEditCellType.dxDateBox;

                    dynamic ctrlProp = DynamicXml.Parse(field.CTRL_PROP_0);
                    if (ctrlProp.DateTimeType == "epfDate") {
                        editCell.DateType = FieldDateType.date;
                        editCell.DisplayFormat = "dd/MM/yyyy";
                    }
                    else if (ctrlProp.DateTimeType == "epfDateTime") {
                        editCell.DateType = FieldDateType.datetime;
                        editCell.DisplayFormat = "dd/MM/yyyy HH:mm";
                    }
                    else if (ctrlProp.DateTimeType == "epfTime") {
                        editCell.DateType = FieldDateType.time;
                        editCell.DisplayFormat = "HH:mm";
                    }
                }
                else if (field.CTRL_TYPE == "CHECKBOX") { // for checkbox field
                    editCell.CellType = FieldGridEditCellType.dxCheckBox;
                }
                else if (field.CTRL_TYPE == "TEXTBOX" && !string.IsNullOrEmpty(field.BRWS_ID)) { // for picklist
                    if (this.ParentField is FieldGridView) {
                        FieldGridView gridview = (FieldGridView)this.ParentField;

                        string pickListFieldID = gridview.ID + "_" + field.FIELD_ID;
                        Field existingField = gridview.ParentScreen.Fields.Search(pickListFieldID);
                        if (existingField == null) {
                            string styleClass = field.CLS_BS_CTRL;
                            string styleClassWidth = field.CLS_BS_CTRL_WIDTH;

                            dynamic ctrlProp = DynamicXml.Parse(field.CTRL_PROP_0);

                            string brwsKeys = ctrlProp.BrwsKeys;
                            bool pickOnly = Convert.ToBoolean(ctrlProp.PickOnly);
                            string brwsTarget = ctrlProp.BrwsTarget;
                            string brwsAssign = ctrlProp.BrwsAssign;

                            int height = 20;

                            FieldPickList pickList = new FieldPickList() {
                                ID = pickListFieldID,
                                Description = field.FIELD_NAME_0,
                                StyleClass = styleClass,
                                StyleClassWidth = styleClassWidth,
                                BrwsId = field.BRWS_ID,
                                BrwsKeys = brwsKeys,
                                PickOnly = pickOnly,
                                BrwsTarget = gridview.ID + "_" + field.FIELD_ID,
                                BrwsAssign = brwsAssign,
                                Height = height,
                                HideTextBox = true,
                                ParentScreen = gridview.ParentScreen
                            };

                            pickList.OnPickListSelection += PickList_OnPickListSelection;
                            pickList.OnChange += picklist_OnChange;
                            pickList.ParentContainer = gridview.ParentScreen.MainContainer;

                            gridview.ParentScreen.MainContainer.Fields.Add(pickList);
                        }
                                               

                        editCell.CellType = FieldGridEditCellType.pickList;
                        editCell.PickListFieldId = pickListFieldID;
                    }

                }
                else if (field.CTRL_TYPE == "BUTTON") { // for button field
                    editCell.CellType = FieldGridEditCellType.dxButton;
                    editCell.Description = field.FIELD_NAME_0;
                }

            }
        }

        private async void picklist_OnChange(object sender, FieldChangeEventArgs e) {
            if (this.ParentField is FieldGridView) {
                FieldGridView gridview = (FieldGridView)this.ParentField;
                FieldPickList picklist = (FieldPickList)sender;
                await picklist.ParentScreen.Session.ExecJSAsync("_dxdatagrid_set_picklist_edit_field", gridview.ParentScreen.SCRN_ID, gridview.ID, picklist.ID, e.Value);
            }
        }

        private async void PickList_OnPickListSelection(object sender, FieldClickEventArgs e) {
            FieldPickList picklist = (FieldPickList)sender;
            SessionState session = picklist.ParentScreen.Session;
            FieldGridView gridview = (FieldGridView)this.ParentField;

            if (picklist.BrwsGrid == null) {

                string url = string.Format("EpfScreen/GetBrwsData");
                JObject postData = new JObject();
                postData.Add("brwsId", picklist.BrwsId);
                postData.Add("session", JObject.FromObject(session));
                postData.Add("dataSource", this.Data);
                postData.Add("baseView", gridview.NavigateScrnDefinition.BASE_VIEW);

                JObject brwsData = await session.PostJsonAsync<JObject>(url, postData);
                if (brwsData.ContainsKey("brws") && brwsData.ContainsKey("cols")) {
                    EPF_BRWS brws = brwsData["brws"].ToObject<EPF_BRWS>();
                    JObject colInfos = JObject.FromObject(brwsData["cols"]);
                    picklist.BrwsGrid = new FieldGridView();
                    picklist.BrwsGrid.BRWS_ID = picklist.BrwsId;
                    picklist.BrwsGrid.ParentScreen = picklist.ParentScreen;
                    picklist.BrwsGrid.ShowTitleBar = false;
                    if (brwsData.ContainsKey("rows")) {
                        picklist.BrwsGrid.DataSource = JArray.FromObject(brwsData["rows"]);
                    }
                    picklist.BrwsGrid.InitColumns(colInfos.ToObject<Dictionary<string, EPF_BRWS_COL>>(), brws.COL_WIDTH_0, brws.COL_HDR_0, brws.COL_FORMAT_0, brws.COL_ALIGN_0);
                    picklist.BrwsGrid.Render = true;
                    picklist.ParentScreen.Refresh();
                }

            }
            picklist.BrwsGrid.RequireRefresh = true;
        }

        private object GetDefaultValue(SessionState session, ScreenMode ScrnMode, EPF_SCRN_FIELDS epfField) {
            // Apply Default Value
            if (!string.IsNullOrEmpty(epfField.DEF_VALUE)) {
                string defValue = "";
                if (epfField.DEF_VALUE.Contains(",")) {
                    defValue = epfField.DEF_VALUE.SplitAndGetItem(',', ScrnMode.ToInt());
                }
                else {
                    defValue = epfField.DEF_VALUE;
                }

                switch (defValue) {
                    case ":COMP_CODE":
                        return session.CompCode;

                    case ":EMPE_ID":
                        return session.EmpeID;

                    case ":USER_ID":
                        return session.UserID;

                    case ":LANG_ID":
                        return session.LangId;

                    case ":DATE":
                        return DateTime.Now;

                    default:
                        return defValue;
                }
            }

            return null;
        }
    }

    public class FieldGridEditCell {
        [JsonIgnore]
        public FieldGridEditRow ParentEditRow { get; set; }
        public string DataField { get; set; }
        public object Value { get; set; }
        public object DefaultValueOnAdd { get; set; }
        public object DefaultValueOnUpdate { get; set; }
        public bool Disabled { get; set; }
        public bool DefaultDisabledOnAdd { get; set; }
        public bool DefaultDisabledOnUpdate { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public FieldGridEditCellType CellType { get; set; } = FieldGridEditCellType.dxTextBox;
        public JArray SelectData { get; set; }
        public string PickListFieldId { get; set; }
        public string Mask { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public FieldDateType DateType { get; set; }
        public string DisplayFormat { get; set; }
        public bool HasOnChangeEvent { get; set; } = false;
        public event EventHandler<FieldChangeEventArgs> OnChange;
        public bool HasOnClickEvent { get; set; } = false;
        internal void InvokeOnChange(object sender, FieldChangeEventArgs e) {
            EventHandler<FieldChangeEventArgs> handler = OnChange;
            if (handler != null) {
                handler(sender, e);
            }
        }
        public string Description { get; set; }
        public FieldGridEditCell(FieldGridEditRow parentEditRow) {
            this.ParentEditRow = parentEditRow;
        }
    }

    public enum FieldGridEditCellType {
        dxTextBox,
        dxDateBox,
        dxSelectBox,
        dxCheckBox,
        pickList,
        dxButton
    }

    public enum FieldGridEditRowAction {
        eOnInsert,
        eOnUpdate,
        eOnDelete,
        eOnCancel,
        eOnBeforeInsert,
        eOnBeforeUpdate,
        eOnBeforeDelete,
        eOnAfterInsert,
        eOnAfterUpdate,
        eOnAfterDelete,
        onDownloadClick,
    }


    public enum FieldGridActionEventType {
        onInitNewRow,
        onRowInserting,
        onRowInserted,
        onRowUpdated,
        onRowUpdating,
        onRowRemoving,
        onRowRemoved,
        onEditingStart,
        onEditCanceled,
        onEditCellOnChanged,
        onEditCellOnClick,
        onDownloadClick,
        onSelectionChanged,
        onButtonAction,
        onRowClick
    }

    
    public class FieldGridActionEventArgs {
        public string FieldId { get; set; }
        public string Value { get; set; }
        public string PreviousValue { get; set; }
        public JObject DataSource { get; set; }
        public bool Cancel { get; set; } = false;

        [JsonConverter(typeof(StringEnumConverter))]
        public FieldGridActionEventType EventType { get; set; }
    }

    public enum FieldGridViewColumnType { 
        @string,
        number,
        date,
        boolean,
        @object,
        datetime
    }

    public enum FieldGridViewColumnAlignment {
        left,
        right,
        center
    }

    public class FieldGridViewColumn {
        private FieldGridViewColumnType _columnType = FieldGridViewColumnType.@string;
        private FieldGridViewColumnAlignment _columnAlignment = FieldGridViewColumnAlignment.left;
        public string dataField { get; set; } = "";
        public string caption { get; set; } = "";
        public int minWidth { get; set; } = 50;
        public string dataType { get { return _columnType.ToStr(); } }  
        public FieldGridViewColumnType columnType { set { _columnType = value; } } // string, number, date, boolean, object, datetime
        public string alignment { get { return _columnAlignment.ToStr(); } }
        public FieldGridViewColumnAlignment ColumnAlignment { set { _columnAlignment = value; } } // left, right
        public string format { get; set; } = "";
        public int width { get; set; } = 200;
        public bool visible { get; set; } = true;
        public string type { get; set; } = null;
        public JArray buttons { get; set; }
        public bool EditorDisableOnAdd { get; set; } = false;
        public bool EditorDisableOnUpdate { get; set; } = false;
        public object DefValueOnAdd { get; set; } = null;
        public object DefValueOnUpdate { get; set; } = null;
    }


    public enum FieldGridViewScrollType {
        standard,
        @virtual,
        infinite
    }

    public enum FieldGridViewType { 
        Brws,
        Editable,
        MultiSelect,
        SingleSelect,
        Navigate
    }

    public enum FieldGridViewSelectionMode
    {
        none,
        multiple,
        single
    }

    public enum FieldGridViewButtonType {
        edit,
        delete,
        download
    }

    public class FieldGridView : Field
    {
        private bool _showTitleBarAddNew = true;

        FieldGridViewType _gridType = FieldGridViewType.Brws;

        internal FieldTitleBar TitleBar { get; set; }
        public bool ShowTitleBar { get; set; } = true;
        public bool ShowTitleBarAddNew
        {
            get
            {
                return _showTitleBarAddNew;
            }
            set
            {
                _showTitleBarAddNew = value;
                this.InitializeTitleBar();
            }
        }
        public string BRWS_ID { get; set; }
        public JArray DataSource { get; set; }
        public FieldGridViewType GridType { 
            get {
                return _gridType;
            } 
            set {
                this.AllowAdding = false;
                this.AllowUpdating = false;
                
                if (value == FieldGridViewType.Editable) {
                    this.AllowAdding = true;
                    this.AllowUpdating = true;
                }

                _gridType = value;
            } 
        }
        public string SelectTargetField { get; set; }
        public string SelectSourceKeys { get; set; }

        public string NavigateScrnId { get; set; }
        public EPF_SCRN NavigateScrnDefinition { get; set; }
        
        public bool RequireRefresh { get; set; } = false;
        public string TargetKeys { get; set; }
        public string ScreenKeys { get; set; }
        public bool SaveOnNavigate { get; set; } = false;
        public bool AllowDeleting { get; set; } = false;
        public bool AllowUpdating { get; set; } = false;
        public bool AllowAdding { get; set; } = false;
        public List<FieldGridViewButtonType> Buttons { get; set; } = new List<FieldGridViewButtonType>();
        public string AssignField { get; set; }
        public FieldGridViewSelectionMode SelectionMode { get; set; } = FieldGridViewSelectionMode.single;
        public bool PopupLinkScreen { get; set; } = false;
        public List<FieldGridViewColumn> Columns { get; set; }

        public int PageSize { get; set; } = 10;
        public FieldGridViewScrollType ScrollType { get; set; } = FieldGridViewScrollType.@virtual;
        public FieldGridEditRow EditRow { get; set; }
        public bool EditRowOnClick { get; set; } = false;
        public bool ShowCommandButtons { get; set; } = true;
        public bool ServerSidePaging { get; set; } = false;
        public bool LoadData { get; set; } = true;

        public event EventHandler<FieldGridActionEventArgs> OnGridAction;
        internal void InvokeOnGridAction(object sender, FieldGridActionEventArgs e) {
            EventHandler<FieldGridActionEventArgs> handler = OnGridAction;
            if (handler != null) {
                handler(sender, e);
            }
        }

        public event EventHandler<FieldChangeEventArgs> OnSelectionChanged;
        internal void InvokeOnSelectionChanged(object sender, FieldChangeEventArgs e)
        {
            EventHandler<FieldChangeEventArgs> handler = OnSelectionChanged;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<FieldClickEventArgs> OnAddNewClick;
        internal void InvokeOnAddNewClick(object sender, FieldClickEventArgs e) {
            EventHandler<FieldClickEventArgs> handler = OnAddNewClick;
            if (handler != null)
                handler(this, e);
        }

        public FieldGridView() : base()
        {
            this.DataSource = new JArray();
            this.Columns = new List<FieldGridViewColumn>();
            InitializeTitleBar();
        }

        public FieldGridView(string id) : base(id, "")
        {
            this.DataSource = new JArray();
            this.Columns = new List<FieldGridViewColumn>();
            InitializeTitleBar();
        }


        public void InitializeTitleBar() {
            this.TitleBar = new FieldTitleBar();
            this.TitleBar.ParentScreen = this.ParentScreen;
            if (this.ShowTitleBar) {
                this.TitleBar.Description = this.Description;
                this.TitleBar.SubTitle = true;

                if (this.ShowTitleBarAddNew) {

                    if (this.TitleBar.MenuItemList.Find(x => x is FieldTitleBarMenuItemNew) == null) {
                        this.TitleBar.MenuItemList.Add(new FieldTitleBarMenuItemNew());
                    }

                }
            }

            this.TitleBar.ApplyAccessRights();
        }

        public override string GetFieldType() {
            return _fieldType.dxDataGrid.ToString();
        }
        public override JObject GetFieldProp()
        {
            
            JObject prop = new JObject();
            prop.Add("fieldType", _fieldType.dxDataGrid.ToString());
            prop.Add("dataSource", DataSource);
            prop.Add("columns", JArray.FromObject(Columns));
            
            JObject paging = new JObject();
            paging.Add("pageSize", this.PageSize);
            prop.Add("paging", paging);
            prop.Add("selectionMode", this.SelectionMode.ToString());
            prop.Add("allowDeleting", this.AllowDeleting);
            prop.Add("allowAdding", this.AllowAdding);
            prop.Add("allowUpdating", this.AllowUpdating);
            prop.Add("editRowOnClick", this.EditRowOnClick);
            prop.Add("showCommandButtons", this.ShowCommandButtons);


            if (this.AllowDeleting && !this.Buttons.Contains(FieldGridViewButtonType.delete)) {
                this.Buttons.Add(FieldGridViewButtonType.delete);
            }

            if (this.GridType == FieldGridViewType.Editable && !this.Buttons.Contains(FieldGridViewButtonType.edit)) {
                this.Buttons.Add(FieldGridViewButtonType.edit);
            }

            List<string> buttons = this.Buttons.Distinct().ToList().ConvertAll<string>(x => x.ToString());
            prop.Add("buttons", JArray.FromObject(buttons));
            prop.Add("scrolling", ScrollType.ToString());
            prop.Add("screenId", this.ParentScreen.ScrnId);

            prop.Add("brwsId", this.BRWS_ID);
            prop.Add("session", JObject.FromObject(this.ParentScreen.Session));
            prop.Add("screenDataSource", this.ParentScreen.DataSource);
            prop.Add("baseView", this.ParentScreen.BASE_VIEW);
            prop.Add("serverSidePaging", this.ServerSidePaging);
            prop.Add("loadData", this.LoadData);
            prop.Add("selectTargetField", this.SelectTargetField);
            prop.Add("selectSourceKeys", this.SelectSourceKeys);
            
            return prop;
        }
        public void InitColumns(Dictionary<string, EPF_BRWS_COL> columnInfos, string colWidths, string colCaptions, string colFormats, string colAligns) {
            this.Columns.Clear();

            foreach (var col in columnInfos.Values) {
                int colIndex = col.column_ordinal - 1;

                string type = null;
                string colName = col.name.ToStr();
                bool colVisible = true;
                int colWidth = colWidths.SplitAndGetItem(',', colIndex).ToInt();
                string colCaption = colCaptions.SplitAndGetItem(',', colIndex);
                if (colWidth <= 0) {
                    colVisible = false;
                }

                FieldGridViewColumnType colType = ParseEnum<FieldGridViewColumnType>(col.col_type.ToStr());
               
                string colFormat = colFormats.SplitAndGetItem('|', colIndex);
                if (colFormat.ToUpper() == "DD/MM/YYYY") {
                    colType = FieldGridViewColumnType.date;
                }
                else if (colFormat.ToUpper() == "BUTTON" || colFormat.ToUpper() == "LINK" || colFormat.ToUpper() == "LINKTXT")
                {
                    type = "buttons";
                }

                FieldGridViewColumnAlignment colAlign = FieldGridViewColumnAlignment.left;
                if (colAligns.SplitAndGetItem(',', colIndex).ToInt() == 0) {
                    colAlign = FieldGridViewColumnAlignment.left;
                }
                else if (colAligns.SplitAndGetItem(',', colIndex).ToInt() == 1)
                {
                    colAlign = FieldGridViewColumnAlignment.right;
                }
                else if (colAligns.SplitAndGetItem(',', colIndex).ToInt() == 2)
                {
                    colAlign = FieldGridViewColumnAlignment.center;
                }
                else
                {
                    colAlign = FieldGridViewColumnAlignment.left;
                }

                // overwrite colAlign as center for boolean type
                if (colType == FieldGridViewColumnType.boolean) {
                    colAlign = FieldGridViewColumnAlignment.center;
                }

                if (type != "buttons")
                {
                    this.Columns.Add(new FieldGridViewColumn() {
                        dataField = colName,
                        caption = colCaption,
                        columnType = colType,
                        width = colWidth / 10,
                        visible = colVisible,
                        format = colFormat,
                        ColumnAlignment = colAlign
                    });
                }
                else
                {
                    JArray btnArr = new JArray();

                    string[] colSplit; string colCSS = "";
                    if (colCaption.Contains(":"))
                    {
                        colSplit = colCaption.Split(':');
                        colCaption = colSplit[0];
                        if(colSplit.Length > 1) colCSS = colSplit[1];
                    }

                    string template = " (function(){ ";
                    //template += (colFormat.ToUpper() == "BUTTON" ? "    var link = $(\"<input type='button'>\").val(\"" + colCaption + "\").attr(\"href\", \"#\"); " : "    var link = $(\"<a class='"+ colCaption + "'>\").attr(\"href\", \"#\"); "); 
                    if (colFormat.ToUpper() == "BUTTON")
                    {
                        template += (colCSS == "" ? "    var link = $(\"<input type='button'>\").val(\"" + colCaption + "\").attr(\"href\", \"#\"); " : "    var link = $(\"<input type='button' class='" + colCSS + "'>\").val(\"" + colCaption + "\").attr(\"href\", \"#\"); ");
                    }
                    else if (colFormat.ToUpper() == "LINK")
                    {
                        //template += "    var link = $(\"<a class='" + colCSS + "'>\").attr(\"href\", \"#\"); ";
                        template += "    var link = $(\"<button class='btn btn-default'><i class='" + colCSS + " text-blue'></i></button>\"); ";
                    }
                    else if (colFormat.ToUpper() == "LINKTXT")
                    {
                        template += "    var link = $(\"<input type='button' class='" + colCSS + "'>\").val(\"" + colCaption + "\").attr(\"href\", \"#\"); ";
                    }

                    template += "    link.click(\"click\", function(){ ";                    
                    template += "        var gridview = $(\"#\" + id).data(fieldType);";
                    template += "        gridview.option(\"buttonAction\",\"" + colCaption + "\"); "; 
                    template += "     }); ";
                    template += "    return link;";
                    template += "})";

                    JObject btnObj = new JObject {
                        ["text"] =colCaption,
                        ["visible"] = true,
                        ["name"] = colCaption,
                        ["templateFunction"] = template
                    };
                    btnArr.Add(btnObj);

                    this.Columns.Add(new FieldGridViewColumn()
                    {
                        dataField = colName,
                        caption = "",
                        columnType = colType,
                        width = colWidth / 10,
                        visible = colVisible,
                        format = colFormat,
                        ColumnAlignment = colAlign,
                        type = type,
                        buttons = btnArr,                         
                    });

                }
                

            }

            
        }

        

        private static T ParseEnum<T>(string value) {
            return (T)Enum.Parse(typeof(T), value, true);
        }


        public void InitEditRow(JObject data, FieldGridEditRowAction action) {
            this.EditRow = new FieldGridEditRow(this);
            this.EditRow.Action = action;
            this.EditRow.Data = data;
            
            foreach (var prop in this.EditRow.Data) {
                FieldGridEditCell editCell = new FieldGridEditCell(this.EditRow);
                editCell.DataField = prop.Key;

                // assign value
                editCell.Value = data[prop.Key];

                // assign cell type => text/date/dropdown/checkbox/picklist
                if (this.GridType == FieldGridViewType.Editable && this.NavigateScrnDefinition != null) {
                    EPF_SCRN_FIELDS field = this.NavigateScrnDefinition.FIELDS.Find(x => x.FIELD_ID == prop.Key);
                    this.EditRow.InitCell(this.ParentScreen.Session, field, editCell);

                    EPF_SCRN_EVENTS onChangeEvent = this.NavigateScrnDefinition.EVENTS.Find(x => x.OBJ_ID == prop.Key && x.EVENT_ID == "onchange");
                    if (onChangeEvent != null) {
                        editCell.HasOnChangeEvent = true;
                    }
                    else {
                        editCell.HasOnChangeEvent = false;
                    }

                    EPF_SCRN_EVENTS onClickEvent = this.NavigateScrnDefinition.EVENTS.Find(x => x.OBJ_ID == prop.Key && x.EVENT_ID == "onclick");
                    if (onClickEvent != null) {
                        editCell.HasOnClickEvent = true;
                    }
                    else {
                        editCell.HasOnClickEvent = false;
                    }

                    // make sure the value is DateTime type for DateTime Field
                    if (field != null) {
                        if (field.CTRL_TYPE == "DATETIME" && !string.IsNullOrEmpty(data[prop.Key].ToStr()))
                        {
                            editCell.Value = data[prop.Key].ToDate();
                        }
                    }

                }


                // assign default value for add/update mode if current value is empty
                if (string.IsNullOrEmpty(editCell.Value.ToStr())) {
                    if (this.EditRow.Action == FieldGridEditRowAction.eOnInsert) {
                        if (editCell.DefaultValueOnAdd != null) {
                            editCell.Value = editCell.DefaultValueOnAdd;
                            this.EditRow.Data[prop.Key] = JToken.FromObject(editCell.Value);
                        }
                    }
                    else if (this.EditRow.Action == FieldGridEditRowAction.eOnUpdate) {
                        if (editCell.DefaultValueOnUpdate != null) {
                            editCell.Value = editCell.DefaultValueOnUpdate;
                            this.EditRow.Data[prop.Key] = JToken.FromObject(editCell.Value);
                        }
                    }
                }

                // convert checkbox value to 1/0 instead of true/false
                if (editCell.CellType == FieldGridEditCellType.dxCheckBox) {
                    if (editCell.Value != null) {
                        editCell.Value = editCell.Value.ToInt();
                        this.EditRow.Data[prop.Key] = JToken.FromObject(editCell.Value);
                    }
                }

                this.EditRow.Cells.Add(editCell.DataField, editCell);
            }

            this.ParentScreen._gridEditingRow = this.EditRow;
            this.ParentScreen.Refresh();

        }


        public void RemoveEditRow() {
            if(this.EditRow == null) {
                return;
            }

            foreach (KeyValuePair<string, FieldGridEditCell> cell in this.EditRow.Cells) {
                if (cell.Value.CellType == FieldGridEditCellType.pickList) {
                    Field field = this.ParentScreen.Fields.Search(cell.Value.PickListFieldId);
                    field.ParentContainer.Fields.Remove(field);
                }
            }

            this.EditRow = null;
            this.ParentScreen.Refresh();
        }
    }

}
