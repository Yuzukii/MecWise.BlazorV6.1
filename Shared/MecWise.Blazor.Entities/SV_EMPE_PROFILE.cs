using System;
using System.Collections.Generic;
using System.Text;

namespace MecWise.Blazor.Entities
{
    [EntitySetting(BaseViewName = "SV_EMPE_PROFILE", BaseProcedureName = "HR_UP_EMPE_PROFILE",
        PrimaryKeys = new string[] {"COMP_CODE","EMPE_ID"})]
    public class SV_EMPE_PROFILE 
    {
        public string COMP_CODE { get; set; }

        public string EMPE_ID { get; set; }

        public string RECRUIT_DOC_TYPE { get; set; }

        public string RECRUIT_DEPT_CODE { get; set; }

        public string RECRUIT_RUN_NO { get; set; }

        public string APLN_NO { get; set; }

        public string APLN_STS_CODE { get; set; }

        public string FAM_NAME { get; set; }

        public string GIVE_NAME { get; set; }

        public string PREF_NAME { get; set; }

        public string CHINESE_NAME { get; set; }

        public string ALIAS_NAME { get; set; }

        public string BLDG_NO { get; set; }

        public string FLR_NO { get; set; }

        public string ST_NAME { get; set; }

        public string UNIT_NO { get; set; }

        public string POSTAL_CODE { get; set; }

        public string STATE_NAME { get; set; }

        public string CTY_CODE { get; set; }

        public string HOME_CNT { get; set; }

        public string BUSI_CNT { get; set; }

        public string PGR_NO { get; set; }

        public string HDPH_NO { get; set; }

        public string EMAIL_ADDR { get; set; }

        public string BIRTH_PLCE { get; set; }

        public DateTime? BIRTH_DATE { get; set; }

        public decimal? EMPE_AGE { get; set; }

        public string EMPE_SEX { get; set; }

        public string MARITAL_STS { get; set; }

        public string EMPE_RACE { get; set; }

        public string EMPE_CITZS { get; set; }

        public string EMPE_NAT { get; set; }

        public string EMPE_RLGN { get; set; }

        public decimal? EMPE_HT { get; set; }

        public decimal? EMPE_WT { get; set; }

        public string EMPE_DLCT { get; set; }

        public string NRIC_NO { get; set; }

        public string NRIC_COLR { get; set; }

        public string FIN_NO { get; set; }

        public string PASSPOT_NO { get; set; }

        public string ISUE_PLCE { get; set; }

        public DateTime? ISUE_DATE { get; set; }

        public string OLD_NRIC_NO { get; set; }

        public DateTime? OLD_NRIC_ISUE_DATE { get; set; }

        public string OLD_NRIC_ISUE_PLCE { get; set; }

        public string CPF_TYPE { get; set; }

        public string CPF_REF { get; set; }

        public string CPF_AC_NO { get; set; }

        public string CPF_AC_NO_2 { get; set; }

        public string CPF_VOLRY_IND { get; set; }

        public string CPF_POST_IND { get; set; }

        public string TAX_REF { get; set; }

        public string FWL_TYPE { get; set; }

        public DateTime? PERM_RES_DATE { get; set; }

        public DateTime? PERM_EXPY_DATE { get; set; }

        public string PERM_RES_STS { get; set; }

        public DateTime? MARRG_DATE { get; set; }

        public DateTime? RETIRE_DUE_DATE { get; set; }

        public DateTime? RETIRE_OPT_DATE { get; set; }

        public decimal? YR_OF_SRVC { get; set; }

        public decimal? MO_OF_SRVC { get; set; }

        public decimal? DAY_OF_SRVC { get; set; }

        public string WK_PMT_NO { get; set; }

        public DateTime? WK_PMT_ISUE { get; set; }

        public DateTime? WK_PMT_EXPY { get; set; }

        public string EMPT_PASS_NO { get; set; }

        public DateTime? EMPT_PASS_ISUE { get; set; }

        public DateTime? EMPT_PASS_EXPY { get; set; }

        public string EMPT_STS { get; set; }

        public DateTime? CITZS_GRNT_DATE { get; set; }

        public string TENR_OF_OFCR { get; set; }

        public decimal? EXPT_SAL { get; set; }

        public string NS_TYPE { get; set; }

        public string NS_RANK { get; set; }

        public string NS_LAST_RANK { get; set; }

        public DateTime? NS_START_DATE { get; set; }

        public DateTime? NS_END_DATE { get; set; }

        public decimal? NS_LEN { get; set; }

        public string NS_VOC { get; set; }

        public string NS_PRFM { get; set; }

        public string NS_CMPETNC { get; set; }

        public string NS_CONDUCT { get; set; }

        public string NS_UNIT { get; set; }

        public string NS_PLCE { get; set; }

        public DateTime? ATTAIN_DATE { get; set; }

        public string PAYR_TYPE { get; set; }

        public string SFT_IND { get; set; }

        public string SFT_CODE { get; set; }

        public string OT_IND { get; set; }

        public string PAYR_MODE { get; set; }

        public string PAYR_FREQ { get; set; }

        public string UNION_MBR { get; set; }

        public string WITS_TYPE { get; set; }

        public string MEDI_CODE { get; set; }

        public string PART_TIME_STS { get; set; }

        public string REAS_FOR_LEAVG { get; set; }

        public string BOND_STS { get; set; }

        public string INCT_IND { get; set; }

        public string MEDI_CAP_CODE { get; set; }

        public string DENTAL_CAP_CODE { get; set; }

        public decimal? ADJ_SRVC_YR { get; set; }

        public DateTime? EMPE_CMMC_DATE { get; set; }

        public DateTime? EMPE_END_DATE { get; set; }

        public string RSRV_FIELD_1 { get; set; }

        public string RSRV_FIELD_2 { get; set; }

        public string RSRV_FIELD_3 { get; set; }

        public decimal? RSRV_FIELD_4 { get; set; }

        public string DATA_ACES_ID { get; set; }

        public string CREA_BY { get; set; }

        public DateTime? CREA_DATE { get; set; }

        public string USER_LOG { get; set; }

        public DateTime? TIME_LOG { get; set; }

        public decimal? MILEAGE_HOME { get; set; }

        public string ZAKAT_REF { get; set; }

        public string ASNB_REF { get; set; }

        public string STATE_CODE { get; set; }

        public string PSN_EMAIL { get; set; }

        public string FELDA_REL { get; set; }

        public string PANEL { get; set; }

        public string ED_INTV { get; set; }

        public string CEO_RECOM { get; set; }

    }
}
