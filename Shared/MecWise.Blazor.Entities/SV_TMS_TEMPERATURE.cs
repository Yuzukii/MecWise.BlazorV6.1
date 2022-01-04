using System;
using System.Collections.Generic;
using System.Text;

namespace MecWise.Blazor.Entities
{
    [EntitySetting(BaseViewName = "SV_TMS_TEMP_REC", BaseProcedureName = "SP_INSERT_TEMPERATURE_RECORD",
       PrimaryKeys = new string[] { "COMP_CODE", "ENTRY_ID" })] //BaseViewName is from View, BaseProcedureName is store procedure
    public partial class SV_TMS_TEMPERATURE
    {
        public string COMP_CODE { get; set; }
            
        public string EMP_NAME { get; set; }
        
        public float EMP_TEMPERATURE { get; set; }

        public DateTime? CREA_DATE { get; set; }
    }
}
