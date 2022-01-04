using System;
using System.Collections.Generic;
using System.Linq;
using MecWise.Blazor.Entities;
using MecWise.Blazor.Api.DataAccess;
using Newtonsoft.Json.Linq;

namespace MecWise.Blazor.Api.Repositories
{
    public class EPF_BLZ_MNU_SUBITEM_DTL_Repository : GenericRepository<EPF_BLZ_MNU_SUBITEM_DTL>
    {
        DBHelper _db;
        public EPF_BLZ_MNU_SUBITEM_DTL_Repository(DBHelper db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<EPF_BLZ_MNU_SUBITEM_DTL> GetMenuSubItemDtls(string userId, int langId, decimal? menuId, decimal? menuItemId, decimal? menuSubItemId, bool isMobile) {
            
            string sql = "SELECT * FROM DBO.CF_EPF_BLZ_MNU_SUBITEM_DTL_ESS(%s, %d, %s) WHERE MENU_ID = %d AND MENU_ITEM_ID=%d AND MENU_SUB_ITEM_ID=%d";
            return _db.GetRows(sql, userId, langId, ConvertToYesNo(isMobile), menuId, menuItemId, menuSubItemId).ToObject<List<EPF_BLZ_MNU_SUBITEM_DTL>>();
        }

        public IEnumerable<EPF_BLZ_MNU_SUBITEM_DTL> GetMenuSubItemDtls(string userId, int langId, decimal? menuId, decimal? menuItemId, decimal? menuSubItemId, decimal? menuSubItemDtlId, bool isMobile)
        {
            string sql = "SELECT * FROM DBO.CF_EPF_BLZ_MNU_SUBITEM_DTL_ESS(%s, %d, %s) WHERE MENU_ID = %d AND MENU_ITEM_ID=%d AND MENU_SUB_ITEM_ID=%d AND MENU_SUB_ITEM_DTL_ID = %d";
            return _db.GetRows(sql, userId, langId, ConvertToYesNo(isMobile), menuId, menuItemId, menuSubItemId, menuSubItemDtlId).ToObject<List<EPF_BLZ_MNU_SUBITEM_DTL>>();
        }

        public JArray GetQuickAccessMenu(string userId, int langId, bool isMobile) {
            string sql = "SELECT B.* FROM SV_EPF_BLZ_MNU_SUBITEM_DTL_QA_ESS A INNER JOIN DBO.CF_EPF_BLZ_MNU_SUBITEM_DTL_ESS(%s, %d, %s) B " +
                " ON A.MENU_ID = B.MENU_ID AND A.MENU_ITEM_ID = B.MENU_ITEM_ID AND A.MENU_SUB_ITEM_ID = B.MENU_SUB_ITEM_ID AND A.MENU_SUB_ITEM_DTL_ID = B.MENU_SUB_ITEM_DTL_ID " +
                " ORDER BY MENU_SUB_ITEM_DTL_QA_ORDER";
            return _db.GetRows(sql, userId, langId, ConvertToYesNo(isMobile));
        }

        private string ConvertToYesNo(bool value) {
            if (value) {
                return "Y";
            }
            return "N";
        }
    }
}
