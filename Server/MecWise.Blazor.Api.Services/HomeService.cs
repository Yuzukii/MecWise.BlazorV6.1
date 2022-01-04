using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MecWise.Blazor.Entities;
using MecWise.Blazor.Api.DataAccess;
using MecWise.Blazor.Api.Repositories;
using MecWise.Blazor.Common;

namespace MecWise.Blazor.Api.Services
{
    public class HomeService
    {
        
        GenericRepository<SV_EMPE_PROFILE> _empeProfileRepo;

        public HomeService() {
            DBHelper db = new DBHelper(ApiSetting.ConnectionString);
            _empeProfileRepo = new GenericRepository<SV_EMPE_PROFILE>(db);
        }

        public HomeService(SessionState session) {
            DBHelper db = new DBHelper(session.UserID, session.CompCode, session.LangId, ApiSetting.ConnectionString);
            _empeProfileRepo = new GenericRepository<SV_EMPE_PROFILE>(db);
        }

        public string GetEmpeName(string compCode, string empeID)
        {
            List<SV_EMPE_PROFILE> result = _empeProfileRepo.GetByCriteria(new { COMP_CODE = compCode, EMPE_ID = empeID }).ToList();
            if (result.Count > 0)
            {
                return result.First().FAM_NAME;
            }
            return "";
        }
    }
}
