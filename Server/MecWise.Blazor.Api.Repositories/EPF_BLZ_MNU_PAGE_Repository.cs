using System;
using System.Collections.Generic;
using System.Linq;
using MecWise.Blazor.Entities;
using MecWise.Blazor.Api.DataAccess;

namespace MecWise.Blazor.Api.Repositories
{
    public class EPF_BLZ_MNU_PAGE_Repository : GenericRepository<EPF_BLZ_MNU_PAGE>
    {
        DBHelper _db;

        public EPF_BLZ_MNU_PAGE_Repository(DBHelper db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<EPF_BLZ_MNU_PAGE> GetMenuPages(string userId, int langId, bool isMobile) {
            string mobile = "N";
            if (isMobile)
                mobile = "Y";
            string sqlStr = "SELECT * FROM DBO.CF_EPF_BLZ_MNU_PAGE_ESS(%s, %d, %s)";
            List<EPF_BLZ_MNU_PAGE> menuPages = _db.ExecQuery<EPF_BLZ_MNU_PAGE>(sqlStr, userId, langId, mobile).ToList();
            return menuPages;
        }

        public IEnumerable<EPF_BLZ_MNU_PAGE> GetMenuPages(string userId, int langId, decimal menuId, bool isMobile)
        {
            string mobile = "N";
            if (isMobile)
                mobile = "Y";
            string sqlStr = "SELECT * FROM DBO.CF_EPF_BLZ_MNU_PAGE_ESS(%s, %d, %s) WHERE MENU_ID = %d";
            List<EPF_BLZ_MNU_PAGE> menuPages = _db.ExecQuery<EPF_BLZ_MNU_PAGE>(sqlStr, userId, langId, mobile, menuId).ToList();
            return menuPages;
        }

    }
}
