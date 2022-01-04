using System;
using System.Collections.Generic;
using System.Text;

namespace MecWise.Blazor.Entities
{
    [EntitySetting(BaseViewName = "SV_MF_COMP", BaseProcedureName = "HR_UP_MF_COMP",
        PrimaryKeys = new string[] { "COMP_CODE" })]
    public partial class SV_MF_COMP
    {
        public string COMP_CODE { get; set; }

        public string COMP_NAME { get; set; }

        public string LCY_CODE { get; set; }

        public string FCY_CODE { get; set; }

        public string LAST_YR { get; set; }

        public string LAST_FISCAL_PERD { get; set; }

        public decimal? BATCH_NO { get; set; }

        public string BLDG_NO { get; set; }

        public string FLR_NO { get; set; }

        public string UNIT_NO { get; set; }

        public string ST_NAME { get; set; }

        public string CTY_CODE { get; set; }

        public string POST_CODE { get; set; }

        public string FAX_NO { get; set; }

        public string TEL_NO { get; set; }

        public string EMAIL_ADDR { get; set; }

        public decimal? OT_HRS_RATE_1 { get; set; }

        public decimal? OT_HRS_RATE_2 { get; set; }

        public decimal? OT_HRS_RATE_3 { get; set; }

        public decimal? OT_DAY_RATE_1 { get; set; }

        public decimal? OT_DAY_RATE_2 { get; set; }

        public decimal? OT_DAY_RATE_3 { get; set; }

        public string BIMTH_IND { get; set; }

        public decimal? BIMTH_PC { get; set; }

        public string GST_DEPT_CODE { get; set; }

        public string GST_GL_CODE { get; set; }

        public string GST_DESC { get; set; }

        public decimal? GST_PER { get; set; }

        public string GST_REG_NO { get; set; }

        public string TAX_NO { get; set; }

        public string BANK_AC_NAME { get; set; }

        public string BANK_CODE { get; set; }

        public string BANK_BR_CODE { get; set; }

        public string BANK_NAME { get; set; }

        public string BANK_AC { get; set; }

        public string CPF_IND { get; set; }

        public string FUND_IND { get; set; }

        public string NET_ROUND_IND { get; set; }

        public decimal? SDF_CTBN_PC { get; set; }

        public decimal? SDF_SAL_CHK { get; set; }

        public decimal? SDF_MIN_AMT { get; set; }

        public decimal? MAX_ANL_ORD_WAGE { get; set; }

        public decimal? MAX_ANL_TOT_WAGE { get; set; }

        public decimal? MAX_ANL_ORD_WAGE_PC { get; set; }

        public decimal? MAX_TES_WAGE_CPF { get; set; }

        public decimal? MAX_OT_SAL { get; set; }

        public string RATE_BAS { get; set; }

        public string INVY_VAL_METH_FLAG { get; set; }

        public string PSA_CODE { get; set; }

        public decimal? MSO_PC { get; set; }

        public string COMY_CR_N { get; set; }

        public string COMY_IMPRT_BOX { get; set; }

        public string CSTM_BOX { get; set; }

        public string PERS_IN_CHRG_NAME_2 { get; set; }

        public string PERS_IN_CHRG_DESN_2 { get; set; }

        public string PERS_IN_CHRG_TEL_2 { get; set; }

        public string PERS_IN_CHRG_EMAIL { get; set; }

        public string POSTING_MODE { get; set; }

        public string PERS_IN_CHRG_NAME { get; set; }

        public string PERS_IN_CHRG_DESN { get; set; }

        public string PERS_IN_CHRG_TEL { get; set; }

        public string PERS_IN_CHRG_EMAIL_ADDR { get; set; }

        public decimal? CURR_YR { get; set; }

        public decimal? CURR_MTH { get; set; }

        public decimal? CURR_PAY_CYCLE { get; set; }

        public string MTHLY_PAY_PERD { get; set; }

        public string NEGATIVE_PAY { get; set; }

        public string BUSI_NATURE_CODE { get; set; }

        public string COMP_OWNRSHP { get; set; }

        public string COMP_OWNRSHP_CTY { get; set; }

        public decimal? COMP_FRGN_PC { get; set; }

        public DateTime? CREA_DATE { get; set; }

        public string CREA_BY { get; set; }

        public DateTime TIME_LOG { get; set; }

        public string USER_LOG { get; set; }

    }

}
