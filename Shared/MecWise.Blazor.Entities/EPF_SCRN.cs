using System;
using System.Collections.Generic;
using System.Text;

namespace MecWise.Blazor.Entities
{
    [EntitySetting(BaseViewName = "SV_EPF_SCRN", BaseProcedureName = "", PrimaryKeys = new string[] { "SCRN_ID" })]
    public partial class EPF_SCRN {
        public string SCRN_ID { get; set; }

        public string MDULE_ID { get; set; }

        public string BASE_TBL { get; set; }

        public string BASE_VIEW { get; set; }

        public string BASE_PROC { get; set; }

        public string BASE_BRWS { get; set; }

        public string FIELD_KEYS { get; set; }

        public string FORM_TYPE { get; set; }

        public string CSS_FILES { get; set; }

        public string INCLUDE_FILES { get; set; }

        public string EDIT_MODES { get; set; }

        public string FIRST_FIELD { get; set; }

        public DateTime TIME_LOG { get; set; }

        public string USER_LOG { get; set; }

        public string BASE_SCRN { get; set; }

        public string EVENT_VLDN { get; set; }

        public string DISBFLDS_CHECK { get; set; }

        public string DISBFLDS_CHKLIST { get; set; }

        public string SCRN_VER { get; set; }

        public List<EPF_SCRN_FIELDS>  FIELDS { get; set; }

        public List<EPF_SCRN_EVENTS> EVENTS { get; set; }

        public List<EPF_BRWS> BRWS { get; set; }

    }


    [EntitySetting(BaseViewName = "SV_EPF_SCRN_FIELDS", BaseProcedureName = "", PrimaryKeys = new string[] { "SCRN_ID", "FIELD_ID" })]
    public partial class EPF_SCRN_FIELDS {
        public string SCRN_ID { get; set; }

        public string FIELD_ID { get; set; }

        public string FIELD_NAME_0 { get; set; }

        public string FIELD_NAME_1 { get; set; }

        public string FIELD_TYPE { get; set; }

        public string FIELD_ATTR { get; set; }

        public string FIELD_CHK { get; set; }

        public string FIELD_MASK { get; set; }

        public string DEF_VALUE { get; set; }

        public string BRWS_ID { get; set; }

        public string CONTAINER_ID { get; set; }

        public string CAPTION_DIMENSION { get; set; }

        public string CAPTION_ALIGN { get; set; }

        public decimal? MAX_LENGTH { get; set; }

        public decimal? TAB_SEQUENCE { get; set; }

        public string CTRL_TYPE { get; set; }

        public string CTRL_DIMENSION { get; set; }

        public decimal? X_POS { get; set; }

        public decimal? Y_POS { get; set; }

        public string CLS_CTRL { get; set; }

        public string CLS_CAPTION { get; set; }

        public string CTRL_PROP_0 { get; set; }

        public string CTRL_PROP_1 { get; set; }

        public DateTime? TIME_LOG { get; set; }

        public string USER_LOG { get; set; }

        public string CLS_BS_CTRL { get; set; }

        public string CLS_BS_CAPTION { get; set; }

        public string CLS_BS_CTRL_WIDTH { get; set; }

    }


    [EntitySetting(BaseViewName = "SV_EPF_SCRN_EVENTS", BaseProcedureName = "", PrimaryKeys = new string[] { "SCRN_ID", "OBJ_ID", "EVENT_ID", "SEQ_NO" })]
    public partial class EPF_SCRN_EVENTS {
        public string SCRN_ID { get; set; }

        public string OBJ_ID { get; set; }

        public string EVENT_ID { get; set; }

        public string SEQ_NO { get; set; }

        public string PROG_ID { get; set; }

        public string BO_NAME { get; set; }

        public string FUNC_CLASS { get; set; }

        public string FUNC_NAME { get; set; }

        public string FUNC_PROP { get; set; }

        public DateTime TIME_LOG { get; set; }

        public string USER_LOG { get; set; }

    }

}
