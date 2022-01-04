using System;
using System.Collections;
using System.Collections.Generic;
using MecWise.Blazor.Common;
using Newtonsoft.Json.Linq;
using MecWise.Blazor.Entities;
using System.Threading.Tasks;

namespace MecWise.Blazor.Components
{
    public class FieldCustomList : Field
    {
        JArray _dataSource;

        public int PageSize { get; set; } = 10;

        public DateTime MonthFilterStartDate { get; set; } = DateTime.Now.AddMonths(-12);
        public DateTime MonthFilterEndDate { get; set; } = DateTime.Now.AddMonths(12);
        public DateTime CurrentDate { get; set; } = DateTime.Now;

        public JArray DataSource { 
            get { return _dataSource; } 
            set { _dataSource = value; this.Render = true; } 
        }

        public string FilterDateField { get; set; }
        public string HighlightRowBoolFieldName { get; set; }
        public bool HighlightRowBoolFieldIsFalse { get; set; } = false;
        public bool Render { get; set; } = false;
        public bool ShowMultiPickFilter { get; set; } = false;
        public JArray FilterMultiPickDataSource { get; set; }
        public string FilterMultiPickDataField { get; set; }

        public bool ShowDayFilter { get; set; } = false;

        public FieldCustomListColumn Column1 { get; set; } = new FieldCustomListColumn();

        public FieldCustomListColumn Column2 { get; set; } = new FieldCustomListColumn();

        public FieldCustomListColumn Column3 { get; set; } = new FieldCustomListColumn();

        public FieldCustomListColumn Column4 { get; set; } = new FieldCustomListColumn();

        public FieldCustomListColumn Column5 { get; set; } = new FieldCustomListColumn();

        public FieldCustomListColumn Column6 { get; set; } = new FieldCustomListColumn();


        public event EventHandler<FieldClickEventArgs> OnRowClick;
        internal void InvokeOnRowClick(object sender, FieldClickEventArgs e)
        {
            EventHandler<FieldClickEventArgs> handler = OnRowClick;
            if (handler != null)
                handler(this, e);
        }



        public FieldCustomList() : base()
        {
        }

        public FieldCustomList(string id, string description) : base(id, description)
        {
        }

        public FieldCustomList(string id, string description, string styleClassWidth) : base(id, description, styleClassWidth)
        {
        }

    }

    public class FieldCustomListColumn 
    {
        public string DataField { get; set; }
        public List<FieldCustomListColumnColor> ColorSettings { get; set; } =  new List<FieldCustomListColumnColor>();
    }


    public class FieldCustomListColumnColor {
        public string MatchValue { get; set; }
        public string ColorString { get; set; }
    }
}