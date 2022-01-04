using System;
using System.Collections.Generic;
using System.Text;

namespace MecWise.Blazor.Entities
{
    [EntitySetting(BaseViewName = "SV_MF_SCH_OF_SRVC", BaseProcedureName = "HR_UP_MF_SCH_OF_SRVC",
        PrimaryKeys = new string[] { "COMP_CODE", "SCH_OF_SRVC_CODE" })]
    public partial class SV_MF_SCH_OF_SRVC
    {
        public string COMP_CODE { get; set; }

        public string DOC_TYPE { get; set; }

        public string DEPT_CODE { get; set; }

        public string RUN_NO { get; set; }

        public decimal? SRL_NO { get; set; }

        public string SCH_OF_SRVC_CODE { get; set; }

        public string SCH_OF_SRVC_DESC { get; set; }

        public decimal? NO_OF_VACANCY { get; set; }

        public decimal? PROBAT_PERD { get; set; }

        public string APLN_REMK { get; set; }

        public DateTime? EFF_DATE_FR { get; set; }

        public DateTime? EFF_DATE_TO { get; set; }

        public DateTime? CREA_DATE { get; set; }

        public string CREA_BY { get; set; }

        public DateTime? TIME_LOG { get; set; }

        public string USER_LOG { get; set; }

    }

}
