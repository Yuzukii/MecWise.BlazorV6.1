using System;
using System.Collections.Generic;
using System.Text;

namespace MecWise.Blazor.Entities
{
    [EntitySetting(BaseViewName = "SV_EPF_BRWS", BaseProcedureName = "", PrimaryKeys = new string[] { "BRWS_ID" })]
    public partial class EPF_BRWS
    {
        public string BRWS_ID { get; set; }

        public string BRWS_SQL { get; set; }

        public string TITLE_0 { get; set; }

        public string TITLE_1 { get; set; }

        public string COL_HDR_0 { get; set; }

        public string COL_HDR_1 { get; set; }

        public string COL_WIDTH_0 { get; set; }

        public string COL_WIDTH_1 { get; set; }

        public string COL_FORMAT_0 { get; set; }

        public string COL_FORMAT_1 { get; set; }

        public string COL_ALIGN_0 { get; set; }

        public string COL_ALIGN_1 { get; set; }

        public string COL_ASSIGN { get; set; }

        public string BRWS_REMK { get; set; }

        public DateTime? TIME_LOG { get; set; }

        public string USER_LOG { get; set; }

        public Dictionary<string, EPF_BRWS_COL> BRWS_COLUMNS { get; set; }
                
    }


    public partial class EPF_BRWS_COL {
        public string col_type { get; set; }

        public int column_ordinal { get; set; }

        public string name { get; set; }

        public int system_type_id { get; set; }

    }
}
