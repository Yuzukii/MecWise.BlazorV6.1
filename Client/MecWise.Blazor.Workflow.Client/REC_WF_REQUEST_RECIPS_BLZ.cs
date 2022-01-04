using MecWise.Blazor.Common;
using MecWise.Blazor.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MecWise.Blazor.Workflow.Client
{
    class REC_WF_REQUEST_RECIPS_BLZ : Screen
    {
        #region "Screen Events"
        protected override async Task<object> LoadAsync()
        {
            Console.WriteLine("REC_WF_REQUEST_RECIPS_BLZ - LoadAsync");
            return await base.LoadAsync();
        }

        protected override async Task<object> ShowRecAsync()
        {
            Console.WriteLine("REC_WF_REQUEST_RECIPS_BLZ - ShowRecAsync");

            if (ScrnMode == ScreenMode.Add)
            {
                await InitFieldsAsync();
            }
            return await Task.FromResult<object>(null);
        }

        protected override async Task<object> SavedRecAsync()
        {
            Console.WriteLine("REC_WF_REQUEST_RECIPS_BLZ - SavedRecAsync");
            return await Task.FromResult<object>(null);
        }

        protected override async Task<bool> ValidEntryAsync()
        {
            Console.WriteLine("REC_WF_REQUEST_RECIPS_BLZ - ValidEntryAsync");

            if (ScrnMode == ScreenMode.Add)
            {
                SetFieldValue("USER_LOG", Session.UserID);
                SetFieldValue("TIME_LOG", DateTime.Now);
            }
            return await Task.FromResult<bool>(true);
        }

        private async Task<object> InitFieldsAsync()
        {
            Console.WriteLine("REC_WF_REQUEST_RECIPS_BLZ - InitFieldsAsync");
            if (PARAM.ContainsKey("COMP_CODE"))
            {
                Console.WriteLine(PARAM["COMP_CODE"].ToStr());
                SetFieldValue("COMP_CODE", PARAM["COMP_CODE"].ToStr());
            }
            if (PARAM.ContainsKey("APP_ID"))
            {
                Console.WriteLine(PARAM["APP_ID"].ToStr());
                SetFieldValue("APP_ID", PARAM["APP_ID"].ToStr());
            }
            if (PARAM.ContainsKey("WF_ID"))
            {
                Console.WriteLine(PARAM["WF_ID"].ToStr());
                SetFieldValue("WF_ID", PARAM["WF_ID"].ToStr());
            }
            return await Task.FromResult<object>(null);
        }
        #endregion
    }
}
