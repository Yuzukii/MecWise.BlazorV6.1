using System;
using MecWise.Blazor;
using MecWise.Blazor.Common;
using MecWise.Blazor.Components;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Web;
namespace MecWise.Blazor.Utilities.Client {
    public class FILE_DIRECT_UPLD_BLZ : Screen {

        string _returnFieldId;

        #region "Screen Events"

        protected override async Task<object> LoadAsync() {
            
            SetTitleBarMenuVisible(false);

            _returnFieldId = GetQueryParamValue("RETURN_FIELD_ID");
            
            return await base.LoadAsync();
        }

        protected override async Task<object> ShowRecAsync() {
            if (ScrnMode == ScreenMode.Add) {
                await InitFieldsAsync();
            }

            return await Task.FromResult<object>(null);
        }

        protected override async Task<object> SavedRecAsync() {
            return await Task.FromResult<object>(null);
        }

        protected override async Task<bool> ValidEntryAsync() {
            if (ScrnMode == ScreenMode.Add) {
            }
            return await Task.FromResult<bool>(true);
        }

        private async Task<object> InitFieldsAsync() {
            
            return await Task.FromResult<object>(null);
        }

        #endregion



        #region "Contorl Events"

        public async void _btnChooseFile_OnClick() { 
            await Session.ExecJSAsync("_showFileOpenDialog", ScrnId);
        }

        public async void _btnFileUpload_OnClick() {
            string uploadFileName = await UploadFileAsync();
            if (!string.IsNullOrEmpty(uploadFileName)) {
                SetParentFieldValue(_returnFieldId, uploadFileName);
                this.Close();
            }
        }
        

        #endregion




        #region "Helper Functions"

        private async Task<string> UploadFileAsync() {
            
            //string upldFileName = string.Format("BA01_CLM_HRW_1401000014_1_b40bb167-3534-448d-88ee-9abe108e4f28.txt",
            //    Session.CompCode, _docType, _deptCode, _runNo,
            int fileSize = GetFieldValue<int>("_FILE_SIZE");

            // check selected file
            if (string.IsNullOrEmpty(GetFieldValue<string>("_SEL_FILE"))) {
                Session.ToastMessage("Please select the file to upload.", ToastMessageType.error);
                return "";
            }

            // not allow to upload file size greter than 2MB
            double maxUploadSize = Session.AppConfig.maxUploadSize * 1024 * 1000;
            if (fileSize > maxUploadSize || fileSize == 0) {
                Session.ToastMessage("Max file size allowed to upload is " + Session.AppConfig.maxUploadSize.ToStr() + " MB.", ToastMessageType.error);
                return "";
            }
                       
            JObject postData = new JObject();
            postData.Add("SEL_FILE", GetFieldValue<string>("_SEL_FILE"));
            postData.Add("FILE_TYPE", GetFieldValue<string>("_FILE_TYPE"));
            postData.Add("FILE_SIZE", GetFieldValue<string>("_FILE_SIZE"));
            postData.Add("FILE_EXT", GetFieldValue<string>("_FILE_EXT"));
            

            var uploadFileName = await Session.ExecJSAsync("_directUploadFile",
                Session.AccessToken.token_type, Session.AccessToken.access_token, postData.ToString());

            return uploadFileName.ToString();
            
        }

        #endregion

    }
}
