using System;
using System.Collections;
using System.Collections.Generic;
using MecWise.Blazor.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace MecWise.Blazor.Components
{
    public class FieldEditor : Field {

        public bool IsCodeDesc { get; set; } = false;
        public string SqlDesc { get; set; } = "";
        public string CodeDesc { get; set; } = "";

        FieldTextBoxCharacterCase _characterCase = FieldTextBoxCharacterCase.normal;

        public FieldTextBoxCharacterCase CharacterCase {
            get {
                return _characterCase;
            } set {
                _characterCase = value;

                if (string.IsNullOrEmpty(this.StyleClass)) {
                    this.StyleClass = "";
                }

                if (string.IsNullOrEmpty(this.StyleClassWidth)) {
                    this.StyleClassWidth = "";
                }

                this.StyleClass = this.StyleClass.Replace(" uppercase ", " ");
                this.StyleClass = this.StyleClass.Replace(" lowercase ", " ");

                switch (value) {
                    case FieldTextBoxCharacterCase.uppercase:
                        if (!this.StyleClass.Contains("uppercase")) {
                            this.StyleClass = this.StyleClass + " uppercase ";
                        }
                        break;
                    case FieldTextBoxCharacterCase.lowercase:
                        if (!this.StyleClass.Contains("lowercase")) {
                            this.StyleClass = this.StyleClass + " lowercase ";
                        }
                        break;
                }
            } 
        }

        public string Mask { get; set; } = "";

        private string _placeHolder = string.Empty;

        public virtual string PlaceHolder { 
            get {
                if (this.Disabled) {
                    return string.Empty;
                }
                return _placeHolder;
            }
            set {
                _placeHolder = value;
            }
        }

        public override object Value {
            get { return base.Value; }
            set {
                if (!value.IsNumeric() && !value.IsDate()) {
                    base.Value = PerformCharacterCase(value.ToStr());
                }
                else {
                    base.Value = value;
                }
            }
        }

        public FieldEditor() : base() { }

        public FieldEditor(string id, string description) : base(id, description) { }

        public FieldEditor(string id, string description, string styleClassWidth) : base(id, description, styleClassWidth) { }

        public FieldEditor(string id, string description, string styleClassWidth, bool disable) : base(id, description, styleClassWidth, disable) { }

        public FieldEditor(string id, string description, string styleClassWidth, bool disable, bool readOnly) : base(id, description, styleClassWidth, disable, readOnly) { }

        private string PerformCharacterCase(string value) {
            if (this.CharacterCase == FieldTextBoxCharacterCase.uppercase) {
                value = value.ToUpper();
            }
            else if (this.CharacterCase == FieldTextBoxCharacterCase.lowercase) {
                value = value.ToLower();
            }
            return value;
        }
    }


    public enum FieldTextButtonType {
        back,
        danger,
        @default,
        normal,
        success
    }
    public enum FieldTextButtonStylingMode {
        text,
        outlined,
        contained
    }
    public class FieldTextButtonOptions {
        public bool disabled { get; set; }
        public string icon { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public FieldTextButtonStylingMode stylingMode { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public FieldTextButtonType type { get; set; }
    }

    public enum FieldTextButtonLocation {
        before,
        after
    }
    public enum FieldTextBoxCharacterCase { 
        normal,
        uppercase,
        lowercase
    }

    public class FieldTextButton {
        public string name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public FieldTextButtonLocation location { get; set; }
        public FieldTextButtonOptions options { get; set; } = new FieldTextButtonOptions();
    }

    public class FieldText : FieldEditor {

        public int MaxLength { get; set; } = 0;

        public FieldTextMode Mode { get; set; } = FieldTextMode.text;

        public Dictionary<string, FieldTextButton> buttons = new Dictionary<string, FieldTextButton>();

        public FieldText() : base() { }

        public FieldText(string id, string description) : base(id, description) { }

        public FieldText(string id, string description, string styleClassWidth) : base(id, description, styleClassWidth) { }

        public FieldText(string id, string description, string styleClassWidth, bool disable) : base(id, description, styleClassWidth, disable) { }

        public FieldText(string id, string description, string styleClassWidth, bool disable, bool readOnly) : base(id, description, styleClassWidth, disable, readOnly) { }

        public FieldText(string id, string description, string styleClassWidth, bool disable, bool readOnly, string placeHolder, FieldTextMode mode) : base(id, description, styleClassWidth, disable, readOnly) {
            this.PlaceHolder = placeHolder;
            this.Mode = mode;
        }

        public override string GetFieldType() {
            return _fieldType.dxTextBox.ToString();
        }

        public override JObject GetFieldProp() {
            JObject prop = new JObject();
            prop.Add("fieldType", _fieldType.dxTextBox.ToString());

            if (base.Value != null) {
                if (base.Value.GetType().ToString() == "System.Object")
                    prop.Add("value", new JValue(""));
                else
                    prop.Add("value", JToken.FromObject(base.Value));
            }

            prop.Add("placeholder", this.PlaceHolder);
            prop.Add("mode", this.Mode.ToString());
            prop.Add("readOnly", base.ReadOnly);
            prop.Add("disabled", base.Disabled);
            prop.Add("styleClass", base.StyleClass);
            prop.Add("maxLength", this.MaxLength);
            prop.Add("charCase", this.CharacterCase.ToStr());
            prop.Add("mask", this.Mask);
            prop.Add("codeDesc", this.CodeDesc);

            prop.Add("buttons", JArray.FromObject(this.buttons.Values));

            return prop;
        }
    }

    public class FieldNumber : FieldEditor {
        public string Format { get; set; }
        public int MaxLength { get; set; } = 0;

        public FieldNumber() : base() { }

        public FieldNumber(string id, string description) : base(id, description) { }

        public FieldNumber(string id, string description, string styleClassWidth) : base(id, description, styleClassWidth) { }

        public FieldNumber(string id, string description, string styleClassWidth, bool disable) : base(id, description, styleClassWidth, disable) { }

        public FieldNumber(string id, string description, string styleClassWidth, bool disable, bool fullWidth, bool readOnly) : base(id, description, styleClassWidth, disable, readOnly) { }

        public FieldNumber(string id, string description, string styleClassWidth, bool disable, bool fullWidth, bool readOnly, string format) : base(id, description, styleClassWidth, disable, readOnly) {
            this.Format = format;
        }

        public override string GetFieldType() {
            return _fieldType.dxNumberBox.ToString();
        }

        public override JObject GetFieldProp() {
            JObject prop = new JObject();
            prop.Add("fieldType", _fieldType.dxNumberBox.ToString());
            if (base.Value != null) {
                if (base.Value.GetType().ToString() == "System.Object")
                    prop.Add("value", new JValue(""));
                else
                    prop.Add("value", JToken.FromObject(base.Value));
            }
            prop.Add("placeholder", this.PlaceHolder);
            prop.Add("format", this.Format);
            prop.Add("readOnly", base.ReadOnly);
            prop.Add("disabled", base.Disabled);
            prop.Add("styleClass", base.StyleClass);
            prop.Add("mask", this.Mask);
            prop.Add("maxLength", this.MaxLength);

            return prop;
        }
    }

    public class FieldCheckBox : FieldEditor {
        private object _value = new object();
        public override object Value
        {
            get { 
                return _value.ToInt(); 
            }
            set
            {
                _value = value.ToBool();
            }
        }

        public FieldCheckBox() : base() { }

        public FieldCheckBox(string id, string description) : base(id, description) { }

        public FieldCheckBox(string id, string description, string styleClassWidth) : base(id, description, styleClassWidth) { }

        public FieldCheckBox(string id, string description, string styleClassWidth, bool disabled) : base(id, description, styleClassWidth, disabled) { }

        public FieldCheckBox(string id, string description, string styleClassWidth, bool disable, bool readOnly) : base(id, description, styleClassWidth, disable, readOnly) { }

        public override string GetFieldType() {
            return _fieldType.dxCheckBox.ToString();
        }

        public override JObject GetFieldProp() {
            JObject prop = new JObject();
            prop.Add("fieldType", _fieldType.dxCheckBox.ToString());
            if (this.Value != null) {
                if (this.Value.GetType().ToString() == "System.Object")
                    prop.Add("value", new JValue(""));
                else {
                    prop.Add("value", this.Value.ToBool());
                }
                    
            }
            prop.Add("text", base.Description);
            prop.Add("readOnly", base.ReadOnly);
            prop.Add("disabled", base.Disabled);
            prop.Add("styleClass", base.StyleClass);
            return prop;
        }
    }

    public class FieldTextArea : FieldEditor {
        public int Height { get; set; } = 60;

        public FieldTextArea() : base() { }

        public FieldTextArea(string id, string description) : base(id, description) { }

        public FieldTextArea(string id, string description, string styleClassWidth) : base(id, description, styleClassWidth) { }

        public FieldTextArea(string id, string description, string styleClassWidth, bool disabled) : base(id, description, styleClassWidth, disabled) { }

        public FieldTextArea(string id, string description, string styleClassWidth, bool disable, bool readOnly) : base(id, description, styleClassWidth, disable, readOnly) { }

        public FieldTextArea(string id, string description, string styleClassWidth, bool disabled, bool readOnly, int height) : base(id, description, styleClassWidth, disabled, readOnly) {
            this.Height = height;
        }

        public override string GetFieldType() {
            return _fieldType.dxTextArea.ToString();
        }

        public override JObject GetFieldProp() {
            JObject prop = new JObject();
            prop.Add("fieldType", _fieldType.dxTextArea.ToString());
            if (base.Value != null) {
                if (base.Value.GetType().ToString() == "System.Object")
                    prop.Add("value", new JValue(""));
                else
                    prop.Add("value", JToken.FromObject(base.Value));
            }
            prop.Add("placeholder", this.PlaceHolder);
            prop.Add("height", this.Height);
            prop.Add("readOnly", base.ReadOnly);
            prop.Add("disabled", base.Disabled);
            prop.Add("styleClass", base.StyleClass);
            prop.Add("charCase", this.CharacterCase.ToStr());
            prop.Add("mask", this.Mask);
            prop.Add("codeDesc", this.CodeDesc);

            return prop;
        }

    }

    public class FieldDate : FieldEditor {

        public string DisplayFormat { get; set; } = "dd/MM/yyyy";
        public FieldDateType Type { get; set; } = FieldDateType.date;

        public DateTime Min { get; set; } = DateTime.MinValue;
        public DateTime Max { get; set; } = DateTime.MaxValue;

        public bool UseMaskBehavior { get; set; } = true;

        public FieldDate() : base() { }

        public FieldDate(string id, string description) : base(id, description) { }

        public FieldDate(string id, string description, string styleClassWidth) : base(id, description, styleClassWidth) { }

        public FieldDate(string id, string description, string styleClassWidth, bool disabled) : base(id, description, styleClassWidth, disabled) { }

        public FieldDate(string id, string description, string styleClassWidth, bool disable, bool readOnly) : base(id, description, styleClassWidth, disable, readOnly) { }

        public FieldDate(string id, string description, string styleClassWidth, bool disabled, FieldDateType type) : base(id, description, styleClassWidth, disabled) {
            this.Type = type;
            if (this.Type == FieldDateType.date)
                this.DisplayFormat = "dd/MM/yyyy";
            else if (this.Type == FieldDateType.time)
                this.DisplayFormat = "HH:mm";
            else if (this.Type == FieldDateType.datetime)
                this.DisplayFormat = "dd/MM/yyyy HH:mm";
        }

        public override string GetFieldType() {
            return _fieldType.dxDateBox.ToString();
        }

        public override JObject GetFieldProp() {
            JObject prop = new JObject();
            prop.Add("fieldType", _fieldType.dxDateBox.ToString());
            if (base.Value != null) {
                if (base.Value.GetType().ToString() == "System.Object") {
                    prop.Add("value", new JValue(""));
                }
                else {
                    if (base.Value.IsDate()) {
                        prop.Add("value", JToken.FromObject(base.Value.ToDate().ToString("yyyy-MM-ddTHH:mm:ss")));
                    }
                    else {
                        prop.Add("value", JToken.FromObject(base.Value));
                    }
                }

            }
            prop.Add("placeholder", this.PlaceHolder);
            prop.Add("type", this.Type.ToString());
            prop.Add("displayFormat", this.DisplayFormat);
            prop.Add("useMaskBehavior", this.UseMaskBehavior);
            prop.Add("readOnly", base.ReadOnly);
            prop.Add("disabled", base.Disabled);
            prop.Add("styleClass", base.StyleClass);
            prop.Add("min", this.Min);
            prop.Add("max", this.Max);

            return prop;
        }
    }

    public class FieldPickList : FieldEditor {

        public bool ServerSidePaging { get; set; } = false;
        public FieldGridViewScrollType ScrollType { get; set; } = FieldGridViewScrollType.@virtual;

        public string FieldType { get; set; } = "";

        public string BrwsId { get; set; }
        public string BrwsKeys { get; set; }
        public bool PickOnly { get; set; }
        public string BrwsTarget { get; set; } = "";
        public string BrwsAssign { get; set; } = "";
        public int Height { get; set; } = 20;
        public string FooterText { get; set; }
        public bool HideTextBox { get; set; } = false;
        public int MaxLength { get; set; } = 0;
        public FieldGridView BrwsGrid { get; set; }

        public event EventHandler<FieldClickEventArgs> OnPickListSelection;
        internal void InvokeOnPickListSelection(object sender, FieldClickEventArgs e) {
            EventHandler<FieldClickEventArgs> handler = OnPickListSelection;
            if (handler != null)
                handler(sender, e);
        }

        public event EventHandler<FieldChangeEventArgs> OnPickListSelectionChanged;
        internal void InvokeOnPickListSelectionChanged(object sender, FieldChangeEventArgs e) {
            EventHandler<FieldChangeEventArgs> handler = OnPickListSelectionChanged;
            if (handler != null)
                handler(sender, e);
        }

        public FieldPickList() : base() {
        }

        public FieldPickList(string id, string description) : base(id, description) {
        }

        public FieldPickList(string id, string description, string styleClassWidth) : base(id, description, styleClassWidth) {
        }
    }

    public class FieldSelectItem {

        public string ID { get; set; }
        public string Text { get; set; }
        public string ID_Text { get { return string.Format("{0} ({1})", ID, Text); } }

        public FieldSelectItem(string id, string txt) {
            this.ID = id;
            this.Text = txt;
        }
    }

    public class FieldSelect : FieldEditor {
        private JArray _dataSource = new JArray();
        public List<FieldSelectItem> Items { get; set; } = new List<FieldSelectItem>();
        public bool ShowTextWithID { get; set; } = false;
        public bool AcceptCustomValue { get; set; } = false;
        public bool SearchEnabled { get; set; } = false;

        public void SetDataSource(JArray dataSoruce, string itemId, string itemText) {
            _dataSource = dataSoruce;
            this.Items.Clear();
            foreach (var item in _dataSource) {
                this.AddItem(item[itemId].ToStr(), item[itemText].ToStr());
            }
        }

        public void ClearDataSource() {
            this.Items.Clear();
            this._dataSource = new JArray();
        }

        public FieldSelect() : base() { }

        public FieldSelect(string id, string description) : base(id, description) { }

        public FieldSelect(string id, string description, string styleClassWidth) : base(id, description, styleClassWidth) { }

        public FieldSelect(string id, string description, string styleClassWidth, bool disabled) : base(id, description, styleClassWidth, disabled) { }

        public FieldSelect(string id, string description, string styleClassWidth, bool disable, bool readOnly) : base(id, description, styleClassWidth, disable, readOnly) { }

        public void AddItem(string val, string txt) {
            this.Items.Add(new FieldSelectItem(val, txt));
        }

        public override string GetFieldType() {
            return _fieldType.dxSelectBox.ToString();
        }

        public override JObject GetFieldProp() {
            JObject prop = new JObject();
            prop.Add("fieldType", _fieldType.dxSelectBox.ToString());
            if (base.Value != null) {
                if (base.Value.GetType().ToString() == "System.Object")
                    prop.Add("value", new JValue(""));
                else
                    prop.Add("value", JToken.FromObject(base.Value));
            }
            prop.Add("valueExpr", "ID");

            if (ShowTextWithID)
                prop.Add("displayExpr", "ID_Text");
            else
                prop.Add("displayExpr", "Text");

            prop.Add("placeholder", this.PlaceHolder);
            prop.Add("items", JToken.FromObject(this.Items));
            prop.Add("acceptCustomValue", AcceptCustomValue);
            prop.Add("searchEnabled", SearchEnabled);
            prop.Add("readOnly", base.ReadOnly);
            prop.Add("disabled", base.Disabled);
            prop.Add("styleClass", base.StyleClass);

            return prop;
        }
    }

    public class FieldListItem {

        public string ID { get; set; }
        public string Text { get; set; }
        public string ID_Text { get { return string.Format("{0} ({1})", ID, Text); } }

        public FieldListItem(string id, string txt) {
            this.ID = id;
            this.Text = txt;
        }
    }

    public class FieldList : FieldEditor {
        public int Height { get; set; } = 100;

        private JArray _dataSource = new JArray();
        public List<FieldSelectItem> Items { get; set; } = new List<FieldSelectItem>();
        public bool ShowTextWithID { get; set; } = false;
        public bool AcceptCustomValue { get; set; } = false;
        public bool SearchEnabled { get; set; } = false;
        public void SetDataSource(JArray dataSoruce, string itemId, string itemText) {
            _dataSource = dataSoruce;
            this.Items.Clear();
            foreach (var item in _dataSource) {
                this.AddItem(item[itemId].ToStr(), item[itemText].ToStr());
            }
        }

        public void ClearDataSource() {
            this.Items.Clear();
            this._dataSource = new JArray();
        }

        public FieldList() : base() { }

        public FieldList(string id, string description) : base(id, description) { }

        public FieldList(string id, string description, string styleClassWidth) : base(id, description, styleClassWidth) { }

        public FieldList(string id, string description, string styleClassWidth, bool disabled) : base(id, description, styleClassWidth, disabled) { }

        public FieldList(string id, string description, string styleClassWidth, bool disable, bool readOnly) : base(id, description, styleClassWidth, disable, readOnly) { }

        public void AddItem(string val, string txt) {
            this.Items.Add(new FieldSelectItem(val, txt));
        }

        public override string GetFieldType() {
            return _fieldType.dxList.ToString();
        }

        public override JObject GetFieldProp() {
            JObject prop = new JObject();
            prop.Add("fieldType", _fieldType.dxList.ToString());
            if (base.Value != null) {
                if (base.Value.GetType().ToString() == "System.Object")
                    prop.Add("value", new JValue(""));
                else
                    prop.Add("value", JToken.FromObject(base.Value));
            }
            prop.Add("valueExpr", "ID");

            if (ShowTextWithID)
                prop.Add("displayExpr", "ID_Text");
            else
                prop.Add("displayExpr", "Text");

            prop.Add("items", JToken.FromObject(this.Items));
            prop.Add("height", this.Height);
            prop.Add("selectionMode", "single");

            prop.Add("readOnly", base.ReadOnly);
            prop.Add("disabled", base.Disabled);
            prop.Add("styleClass", base.StyleClass);

            return prop;
        }
    }

}


