using System;
using System.Collections.Generic;
using System.Text;

namespace MecWise.Blazor.Entities
{
    [EntitySetting(BaseViewName = "SV_EPF_UPLD_FILE", BaseProcedureName = "UP_EPF_UPLD_FILE",
       PrimaryKeys = new string[] { "KEY_CODE", "LINE_NO" })] //BaseViewName is from View, BaseProcedureName is store procedure
        public partial class SV_EPF_UPLD_FILE {
        public string KEY_CODE { get; set; }

        public decimal LINE_NO { get; set; }

        public string UPLD_FILENAME { get; set; }

        public string ACT_FILENAME { get; set; }

        public string FRIENDLY_NAME { get; set; }

        public string FILE_REMARKS { get; set; }

        public string FILE_TYPE { get; set; }

        public decimal? FILE_SIZE { get; set; }

        public string FILE_EXT { get; set; }

        public string USER_LOG { get; set; }

        public DateTime TIME_LOG { get; set; }

    }

}
