using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MecWise.Blazor.Components
{
    public class FieldCellList : Field
    {        
        public string BrwsID { get; set; }
        public string BaseView { get; set; }
        public string PARAM_KEYS { get; set; } = string.Empty;
        public int CellPageSize { get; set; } = 10;
        public string CellPageSetting { get; set; }
        public int CurrentPage { get; set; } = 0;
        public String CurrentPageCtrlName { get; set; } = string.Empty;
        public JArray DataSource { get; set; } = new JArray();
        public JObject SCRN_DS { get; set; }        
        public string RESULT_FIELD { get; set; } = string.Empty;

        public event EventHandler<JObject> OnCellItemClick;
        internal void InvokeOnCellItemClick(object sender, JObject e)
        {
            EventHandler<JObject> handler = OnCellItemClick;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<string> OnCellSettingClick;
        internal void InvokeOnCellSettingClick(object sender, string e)
        {
            EventHandler<string> handler = OnCellSettingClick;
            if (handler != null)
                handler(this, e);
        }

        public FieldCellList() : base()
        {
        }

        public FieldCellList(string id, string description) : base(id, description)
        {
        }

        public FieldCellList(string id, string description, string styleClass) : base(id, description, styleClass)
        {
        }
    }
}
