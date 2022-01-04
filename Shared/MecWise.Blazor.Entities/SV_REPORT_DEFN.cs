using System;
using System.Collections.Generic;
using System.Text;

namespace MecWise.Blazor.Entities
{
    [EntitySetting(BaseViewName = "SV_REPORT_DEFN", BaseProcedureName = "UP_REPORT_DEFN",
       PrimaryKeys = new string[] { "REPORT_ID" })] //BaseViewName is from View, BaseProcedureName is store procedure
    public partial class SV_REPORT_DEFN {
        public string REPORT_ID { get; set; }

        public string REPORT_TYPE { get; set; }

        public string REPORT_FILE_NAME { get; set; }

        public string REPORT_TITLE_0 { get; set; }

        public string REPORT_TITLE_1 { get; set; }

        public string OVERWRITE_REC_SEL { get; set; }

        public string REPORT_WHERE_CLAUSE { get; set; }

        public string PROPOGATE_WHERE_CLAUSE { get; set; }

    }

}
