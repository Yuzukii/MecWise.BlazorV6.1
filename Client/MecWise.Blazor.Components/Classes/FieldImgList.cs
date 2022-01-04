using MecWise.Blazor.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MecWise.Blazor.Components
{
    public class FieldImgList : Field
    {

        JArray _dataSource;

        public string BrwsID { get; set; }
        public string BaseView { get; set; }
        public string PARAM_KEYS { get; set; } = string.Empty;
        public JArray prevDataSource { get; set; }
        //public JArray DataSource { get; set; }
        public JArray DataSource
        {
            get { return _dataSource; }
            set
            {
                if (value != null)
                {
                    if (_dataSource != null)
                    {
                        if (_dataSource.ToString() != value.ToString())
                        {
                            _dataSource = value;
                        }
                    }
                    else {
                        _dataSource = value;
                    }
                }
                else
                {
                    _dataSource = value;
                }
            }
        }
        //public JObject SCRN_DS { get; set; } 
        public string ImgColName { get; set; } = string.Empty;
        public string DescColName { get; set; } = string.Empty;
        public string PriceColName { get; set; } = string.Empty;
        public string QntyColName { get; set; } = string.Empty;
		public string PromoColName { get; set; } = string.Empty;
        public string DiscColName { get; set; } = string.Empty;
        public string RESULT_FIELD { get; set; } = string.Empty;
		public string IMG_URL { get; set; } = string.Empty;
        public bool IsModi { get; set; } = false;

        public int IMG_COL { get; set; } = 3;

        public event EventHandler<JToken> OnImgItemClick;
        internal void InvokeOnImgItemClick(object sender, JToken e)
        {
            EventHandler<JToken> handler = OnImgItemClick;
            if (handler != null)
                handler(this, e);
        }

        public FieldImgList() : base()
        {
        }

        public FieldImgList(string id, string description) : base(id, description)
        {
        }

        public FieldImgList(string id, string description, string styleClass) : base(id, description, styleClass)
        {
        }
    }
}
