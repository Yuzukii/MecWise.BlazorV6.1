using System;
using System.Collections.Generic;
using System.Text;

namespace MecWise.Blazor.Entities
{
    [EntitySetting(BaseViewName = "SV_SC_USER_PROFILE", BaseProcedureName = "UP_SC_USER_PROFILE",
        PrimaryKeys = new string[] { "USER_ID" })]
    public class SV_SC_USER_PROFILE
    {
        public string USER_ID { get; set; }

        public string USER_DESC { get; set; }

        public string EMPE_ID { get; set; }

        public string COMP_ACES { get; set; }

        public DateTime? AC_EFF_DATE { get; set; }

        public DateTime? AC_EXPY_DATE { get; set; }

        public string AC_PROFILE { get; set; }

        public string AC_FLAG { get; set; }

        public DateTime? PSWD_EXPY_DATE { get; set; }

        public decimal? PSWD_GRACE_USED { get; set; }

        public string PSWD_GNR_HIST { get; set; }

        public string AC_ATTR { get; set; }

        public DateTime? LAST_LOGIN { get; set; }

        public DateTime? CREA_DATE { get; set; }

        public string CREA_BY { get; set; }

        public DateTime? TIME_LOG { get; set; }

        public string USER_LOG { get; set; }
    }
}
