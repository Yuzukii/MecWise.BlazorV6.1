using System;
using System.Collections.Generic;
using System.Text;

namespace MecWise.Blazor.Entities
{
    [EntitySetting(BaseViewName = "", BaseProcedureName = "",
        PrimaryKeys = new string[] { })]
    public class EPF_BLZ_MNU_PAGE
    {
        public decimal? MENU_ID { get; set; }

        public decimal? MENU_ORDER { get; set; }

        public string MENU_GRP { get; set; }

        public string MENU_NAME { get; set; }

        public string SCRN_ID { get; set; }

        public string URL_TMP { get; set; }

        public string MDULE_ID { get; set; }

        public string MENU_ATTR { get; set; }

        public string MENU_ENABLE { get; set; }

        public decimal? MENU_LINK { get; set; }

        public List<EPF_BLZ_MNU_ITEM> MENU_ITEMS { get; set; }

    }
}
