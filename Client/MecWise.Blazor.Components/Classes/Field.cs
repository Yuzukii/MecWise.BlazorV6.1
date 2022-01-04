using System;
using System.Collections;
using System.Collections.Generic;
using MecWise.Blazor.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MecWise.Blazor.Entities;
using System.Threading.Tasks;
using System.Linq;

namespace MecWise.Blazor.Components
{

    public enum FieldTextMode
    {
        text,
        password,
        email,
        search,
        tel,
        url
    }

    public enum FieldDateType { 
        date,
        datetime,
        time
    }

    


    public class Field : ICloneable
    {
        private string _id = Guid.NewGuid().ToString();
        private bool _visible = true;

        protected enum _fieldType {
            dxTextBox,
            dxDateBox,
            dxTextArea,
            dxSelectBox,
            dxNumberBox,
            dxCheckBox,
            dxDataGrid,
            dxList,
            dxAccordion
        }

        [JsonIgnore]
        public Screen ParentScreen { get; set; }
        [JsonIgnore]
        public FieldContainer ParentContainer { get; set; }

        public string ID { 
            get { return _id;  } 
            set {
                _id = value;
            } 
        }

        public string ElementID {
            get {
                if (this.ParentScreen != null) {
                    return string.Format("{0}__{1}", this.ParentScreen.ScrnId, _id);
                }
                else {
                    return _id;
                }
            }
        }

        public virtual string Description { get; set; } = "(None)";

        private object _value = new object();
        public virtual object Value { get { return _value; }
            set {
                _value = value;
            } }
        public virtual bool Disabled { get; set; } = false;
        public virtual bool Visible { get { return _visible;  }
            set {
                
                if (string.IsNullOrEmpty(this.StyleClass)) {
                    this.StyleClass = "";
                }

                if (string.IsNullOrEmpty(this.StyleClassWidth)) {
                    this.StyleClassWidth = "";
                }

                _visible = value;
                if (_visible) {
                    this.StyleClass = this.StyleClass.Replace("d-none", "");
                    this.StyleClassWidth = this.StyleClassWidth.Replace("d-none", "");
                }
                else {
                    
                    if (!string.IsNullOrEmpty(this.StyleClassWidth)) {
                        if (!this.StyleClassWidth.Contains("d-none")) {
                            this.StyleClassWidth += " d-none";
                        }
                    }
                    else {
                        this.StyleClassWidth = " d-none";
                    }

                    if (!string.IsNullOrEmpty(this.StyleClass)) {
                        if (!this.StyleClass.Contains("d-none")) {
                            this.StyleClass += " d-none";
                        }
                    }
                    else {
                        this.StyleClass = " d-none";
                    }

                }
                this.StyleClass = string.Join(' ', this.StyleClass.Split(' ').Distinct().ToArray());
                this.StyleClassWidth = string.Join(' ', this.StyleClassWidth.Split(' ').Distinct().ToArray());
                this.Render = true;
                
            }
        }
        public virtual bool ReadOnly { get; set; } = false;
        public virtual string StyleClass { get; set; } = "";
        public virtual string StyleClassWidth { get; set; } = "";
        public string ParentFieldID { get; set; } = "";
        public bool Render { get; set; } = false;

        public event EventHandler<FieldClickEventArgs> OnClick;
        internal void InvokeOnClick(object sender, FieldClickEventArgs e)
        {
            EventHandler<FieldClickEventArgs> handler = OnClick;
            if (handler != null)
                handler(sender, e);
        }

        public event EventHandler<FieldChangeEventArgs> OnChange;
        internal void InvokeOnChange(object sender, FieldChangeEventArgs e)
        {
            EventHandler<FieldChangeEventArgs> handler = OnChange;
            if (handler != null) { 
                handler(sender, e);
            }
        }

        public event EventHandler OnFocusIn;
        internal void InvokeOnFocusIn(object sender) {
            EventHandler handler = OnFocusIn;
            if (handler != null) {
                handler(sender, null);
            }
        }

        public event EventHandler OnFocusOut;
        internal void InvokeOnFocusOut(object sender) {
            EventHandler handler = OnFocusOut;
            if (handler != null) {
                handler(sender, null);
            }
        }

        public Field() { }
        
        public Field(string id, string description) {
            this.ID = id;
            this.Description = description;
        }

        public Field(string id, string description, bool disabled)
        {
            this.ID = id;
            this.Description = description;
            this.Disabled = disabled;
        }

        public Field(string id, string description, string styleClassWidth)
        {
            this.ID = id;
            this.Description = description;
            this.StyleClassWidth = styleClassWidth;
        }

        public Field(string id, string description, string styleClassWidth, bool disabled)
        {
            this.ID = id;
            this.Description = description;
            this.StyleClassWidth = styleClassWidth;
            this.Disabled = disabled;
        }

        public Field(string id, string description, string styleClassWidth, bool disabled, bool readOnly)
        {
            this.ID = id;
            this.Description = description;
            this.StyleClassWidth = styleClassWidth;
            this.Disabled = disabled;
            this.ReadOnly = readOnly;
        }

        public virtual JObject GetFieldProp() { return new JObject(); }

        public virtual string GetFieldType() { return ""; }

        public object Clone()
        {
            if (this is FieldContainer) {
                return ContainerClone((FieldContainer)this);
            }
            else {
                return this.MemberwiseClone();
            }
          
        }

        private FieldContainer ContainerClone(FieldContainer containerField) {
            FieldContainer newContainer = new FieldContainer(containerField.StyleClass);
            foreach (Field field in containerField.Fields)
            {
                if (field is FieldContainer)
                {
                    newContainer.Fields.Add(ContainerClone((FieldContainer)field));
                }
                else
                {
                    newContainer.Fields.Add((Field)field.Clone());
                }
                
            }
            return newContainer;
        }



    }

    
    public class FieldContainer : Field
    {
        public bool TopBuffer { get; set; } = true;

        public List<Field> Fields { get; set; }

        public FieldContainer() : base()
        {
            this.Fields = new List<Field>();
        }

        public FieldContainer(string id, string styleClass) : base(id, "", styleClass)
        {
            this.Fields = new List<Field>();
            this.StyleClass = styleClass;
        }

        public FieldContainer(string styleClass) 
        {
            this.Fields = new List<Field>();
            this.StyleClass = styleClass;
        }

    }

    public class FieldTabPage : FieldContainer
    {

        public FieldTabPage() : base()
        {
            base.ID = string.Format("t{0}", Guid.NewGuid().ToString());
            this.Fields = new List<Field>();
        }

        public FieldTabPage(string description)
        {
            base.ID = string.Format("t{0}", Guid.NewGuid().ToString());
            base.Description = description;
            this.Fields = new List<Field>();
        }
        
    }

    public class FieldTabContainer : Field
    {
        public string ActiveTabPageId { get; set; } = "";
        public List<FieldTabPage> TabPages { get; set; }

        public FieldTabContainer() : base()
        {
            this.TabPages = new List<FieldTabPage>();
        }

        public FieldTabContainer(string id) : base(id, "")
        {
            this.TabPages = new List<FieldTabPage>();
        }

        
    }


    public class FieldCaptionLabel : Field
    {
        public string DateFormat { get; set; } = "dd/MM/yyyy HH:mm:ss";

        public FieldCaptionLabel() : base()
        {
        }

        public FieldCaptionLabel(string id, string description) : base(id, description)
        {
        }

        public FieldCaptionLabel(string id, string description, string styleClass) : base(id, description, styleClass)
        {
        }

        public FieldCaptionLabel(string id, string description, string styleClass, string dateFormat) : base(id, description, styleClass)
        {
            this.DateFormat = dateFormat;
        }

    }

    public class FieldLabel : Field
    {

        public FieldLabel() : base()
        {
        }

        public FieldLabel(string id, string description) : base(id, description)
        {
        }

        public FieldLabel(string id, string description, string styleClass) : base(id, description, styleClass)
        {
        }


    }


}
