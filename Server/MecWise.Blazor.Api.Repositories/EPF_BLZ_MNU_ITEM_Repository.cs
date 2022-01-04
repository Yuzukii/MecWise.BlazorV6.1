using System;
using System.Collections.Generic;
using System.Linq;
using MecWise.Blazor.Entities;
using MecWise.Blazor.Api.DataAccess;

namespace MecWise.Blazor.Api.Repositories
{
    public class EPF_BLZ_MNU_ITEM_Repository : GenericRepository<EPF_BLZ_MNU_ITEM>
    {
        DBHelper _db;
        public EPF_BLZ_MNU_ITEM_Repository(DBHelper db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<EPF_BLZ_MNU_ITEM> GetMenuItems(string userId, int langId, decimal? menuId, bool isMobile) {
            string mobile = "N";
            if (isMobile)
                mobile = "Y";
            string sql = "SELECT * FROM DBO.CF_EPF_BLZ_MNU_ITEM_ESS(%s, %d, %s) WHERE MENU_ID = %d";
            return _db.ExecQuery<EPF_BLZ_MNU_ITEM>(sql, userId, langId, mobile, menuId);
        }

        public IEnumerable<EPF_BLZ_MNU_ITEM> GetMenuItems(string userId, int langId, decimal? menuId, decimal? menuItemId, bool isMobile)
        {
            string mobile = "N";
            if (isMobile)
                mobile = "Y";
            string sql = "SELECT * FROM DBO.CF_EPF_BLZ_MNU_ITEM_ESS(%s, %d, %s) WHERE MENU_ID = %d AND MENU_ITEM_ID = %d";
            return _db.ExecQuery<EPF_BLZ_MNU_ITEM>(sql, userId,langId, mobile, menuId, menuItemId);
        }
    }
}
