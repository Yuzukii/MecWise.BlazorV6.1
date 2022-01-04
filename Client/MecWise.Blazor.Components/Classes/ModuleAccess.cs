using MecWise.Blazor.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MecWise.Blazor.Components
{
    public class ModuleAccess
    {
        
        public string ModuleId { get; set; } = "";
        public bool Select { get; set; } = false;
        public bool Delete { get; set; } = false;
        public bool Insert { get; set; } = false;
        public bool Update { get; set; } = false;
        public bool Print { get; set; } = false;

        public ModuleAccess()
        {
           
        }

        //public async Task InitPermissions(string moduleId) {
        //    ModuleId = moduleId;

        //    //string url = string.Format("EpfScreen/GetModuleAccess/{0}", moduleId);
        //    //string result = await SessionState.GetJsonAsync<string>(SessionState.GetAbsoluteApiUrl(url));
            
        //    if (result.Contains('S'))
        //    {
        //        Select = true;
        //    }

        //    if (result.Contains('D'))
        //    {
        //        Delete = true;
        //    }

        //    if (result.Contains('I'))
        //    {
        //        Insert = true;
        //    }

        //    if (result.Contains('U'))
        //    {
        //        Update = true;
        //    }

        //    if (result.Contains('P'))
        //    {
        //        Print = true;
        //    }
            
        //}

        
    }
}
