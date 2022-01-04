using System;
using System.Collections;
using System.Collections.Generic;
using MecWise.Blazor.Common;
using Newtonsoft.Json.Linq;
using MecWise.Blazor.Entities;
using System.Threading.Tasks;

namespace MecWise.Blazor.Components
{
    public class FieldRadio : FieldEditor
    {
        
        public List<FieldRadioItem> Items { get; set; } = new List<FieldRadioItem>();
        public FieldRadioListDirection ListDirection { get; set; } = FieldRadioListDirection.Vertical;

        public FieldRadio() : base()
        {
        }

        public FieldRadio(string id, string description) : base(id, description)
        {
        }

        public FieldRadio(string id, string description, string styleClassWidth) : base(id, description, styleClassWidth)
        {
        }

        public void AddItem(string val, string txt) {
            this.Items.Add(new FieldRadioItem(val, txt));
        }

        public async void SetValue(SessionState session, object value) {
            this.Value = value;
            await session.ExecJSAsync("_setRadioValue", this.ID, value);
        }
    }

    public enum FieldRadioListDirection { 
        Vertical,
        Horizontal
    }

    public class FieldRadioItem {

        public string ID { get; set; }
        public string Text { get; set; }
        public string ID_Text { get { return string.Format("{0} ({1})", ID, Text); } }

        public FieldRadioItem(string id, string txt) {
            this.ID = id;
            this.Text = txt;
        }
    }
}