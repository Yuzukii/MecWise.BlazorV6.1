using System;
using System.Collections.Generic;
using System.Text;

namespace MecWise.Blazor.Entities
{
    [EntitySetting(BaseViewName = "", BaseProcedureName = "",
        PrimaryKeys = new string[] { })]
    public class EPF_BLZ_MNU_ITEM
    {
        public decimal? MENU_ID { get; set; }

        public decimal? MENU_ITEM_ID { get; set; }

        public decimal? MENU_ITEM_ORDER { get; set; }

        public string MENU_ITEM_NAME { get; set; }

        public string SCRN_ID { get; set; }

        public string URL_TMP { get; set; }

        public string MDULE_ID { get; set; }

        public string MENU_ITEM_ATTR { get; set; }

        public string ITEM_ENABLE { get; set; }

        public string ITEM_LINK { get; set; }

        public List<EPF_BLZ_MNU_SUBITEM> MENU_SUBITEMS { get; set; }
    }
}
