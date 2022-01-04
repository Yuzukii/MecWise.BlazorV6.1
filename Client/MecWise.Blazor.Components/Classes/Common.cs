using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace MecWise.Blazor.Components
{
    public enum DataListItemColumnFormat
    {
        col1,
        col2,
        col3,
        col4,
        col5,
        col6,
        col7,
        col8,
        col9,
        col10,
        col11,
        col12
    }

    public class GridCellUpdateEventArgs {
        public JObject DataObject { get; set; }
        public string PropName { get; set; }
        public GridCellUpdateEventArgs() { }
        public GridCellUpdateEventArgs(JObject obj, string propName) {
            DataObject = obj;
            PropName = propName;
        }
    }


    public class FieldChangeEventArgs
    {
        public string FieldId { get; set; }
        public string Value { get; set; }
        public string PreviousValue { get; set; }
        public JObject DataSource { get; set; }
        public bool Cancel { get; set; } = false;
        public string EventType { get; set; }
        public string ButtonActionName { get; set; }
    }


    public class FieldClickEventArgs
    {
        public string FieldId { get; set; }
        public string Value { get; set; }
        public string PreviousValue { get; set; }
        public JObject DataSource { get; set; }
        public bool Cancel { get; set; } = false;

    }

}
