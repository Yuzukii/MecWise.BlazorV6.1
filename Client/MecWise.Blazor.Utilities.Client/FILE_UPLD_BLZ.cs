using System;
using MecWise.Blazor;
using MecWise.Blazor.Common;
using MecWise.Blazor.Components;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Web;
namespace MecWise.Blazor.Utilities.Client {
    public class FILE_UPLD_BLZ : Screen {

        bool _readOnly;

        #region "Screen Events"

        protected override async Task<object> LoadAsync() {
            
            SetTitleBarMenuVisible(false);

            string docType = GetParentFieldValue<string>("DOC_TYPE");
            string deptCode = GetParentFieldValue<string>("DEPT_CODE");
            string runNo = GetParentFieldValue<string>("RUN_NO");

            if (!string.IsNullOrEmpty(docType) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(runNo)) {
                string keyCode = string.Format("{0}:{1}:{2}:{3}", Session.CompCode, docType, deptCode, runNo);
                SetFieldValue("_KEY_CODE", keyCode);
                await RefreshGridViewDataAsync("_UPLD_FILE_GRID");
            }
            else {
                Session.ToastMessage("Unable to initialize file upload! Require DOC_TYPE, DEPT_CODE and RUN_NO fields.", ToastMessageType.error);
                this.Close();
            }

            if (Session.IsMobileScreen()) {
                FieldAccordion accordion = (FieldAccordion)Fields.Search("_UPLD_FILE_GRID");
                accordion.Buttons.Add(FieldAccordionButtonType.download);
            }
            else {
                FieldGridView gridView = (FieldGridView)Fields.Search("_UPLD_FILE_GRID");
                gridView.Buttons.Add(FieldGridViewButtonType.download);
            }

            SetFieldDescription("lblMaxFileSize", "Max file size allowed to upload is " + Session.AppConfig.maxUploadSize.ToStr() + " MB.");

            ApplyReadOnly();

            Refresh();

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
            if (await UploadFileAsync()) {
                await RefreshGridViewDataAsync("_UPLD_FILE_GRID");
            }
        }
        public async Task<bool> _UPLD_FILE_GRID_OnGridAction() {

            switch (EditRow.Action) {
                case FieldGridEditRowAction.eOnInsert:
                    //User clicked the Add button (Initialize)...
                    return true;

                case FieldGridEditRowAction.eOnUpdate:
                    //User clicked the Update button
                    return true;

                case FieldGridEditRowAction.eOnDelete:
                    //User clicked the Delete button on a row 
                    return true;

                case FieldGridEditRowAction.eOnCancel:
                    //User clicked the Cancel button on a row
                    return true;

                case FieldGridEditRowAction.eOnBeforeInsert:
                    //User clicked Save button to add a new record and data NOT yet inserted in db (ValidEntry)...
                    return true;

                case FieldGridEditRowAction.eOnBeforeUpdate:
                    //User clicked Save button to update a record and data NOT yet updated in db (ValidEntry)...
                    return true;

                case FieldGridEditRowAction.eOnBeforeDelete:
                    //User clicked Delete button to delete a record and data NOT yet deleted in db (ValidEntry)...
                    JObject deleteRowData = EditRow.Data;
                    await Session.PostAsync("File/Delete", deleteRowData);
                    return true;

                case FieldGridEditRowAction.eOnAfterInsert:
                    //User clicked Save button to add a new record and data already inserted in db (SavedRec)...
                    return true;

                case FieldGridEditRowAction.eOnAfterUpdate:
                    //User clicked Save button to update a record and data already updated in db (SavedRec)...
                    return true;

                case FieldGridEditRowAction.eOnAfterDelete:
                    //User clicked Delete button to delete a record and data already deleted in db (SavedRec)...
                    return true;

                case FieldGridEditRowAction.onDownloadClick:
                    //User clicked Download button of a record
                    JObject downloadRowData = EditRow.Data;
                    string apiUrl = Session.GetAbsoluteApiUrl("File/Download/"); 
                    await Session.ExecJSAsync("_downloadFile", apiUrl, Session.AccessToken.token_type, Session.AccessToken.access_token, downloadRowData.ToString());
                    return true;
            }


            return await Task.FromResult<bool>(true);

        }

        #endregion




        #region "Helper Functions"

        private void ApplyReadOnly() {
            _readOnly = GetQueryParamValue("READ_ONLY").ToBool();

            Console.WriteLine("ApplyReadOnly - " + GetQueryParamValue("READ_ONLY"));
            Console.WriteLine("ApplyReadOnly - " + _readOnly);

            SetFieldDisable("_btnChooseFile", _readOnly);
            SetFieldDisable("_btnFileUpload", _readOnly);
            SetFieldDisable("_FRIENDLY_NAME", _readOnly);
            SetFieldDisable("_FILE_REMARKS", _readOnly);
            if (Session.IsMobileScreen()) {
                FieldAccordion accordion = (FieldAccordion)Fields.Search("_UPLD_FILE_GRID");
                accordion.Buttons.Add(FieldAccordionButtonType.download);
                accordion.DeleteRecord = !_readOnly;
            }
            else {
                FieldGridView gridView = (FieldGridView)Fields.Search("_UPLD_FILE_GRID");
                gridView.AllowDeleting = !_readOnly;
            }
        }

        private async Task<bool> UploadFileAsync() {
            
            //string upldFileName = string.Format("BA01_CLM_HRW_1401000014_1_b40bb167-3534-448d-88ee-9abe108e4f28.txt",
            //    Session.CompCode, _docType, _deptCode, _runNo,
            int fileSize = GetFieldValue<int>("_FILE_SIZE");

            // check selected file
            if (string.IsNullOrEmpty(GetFieldValue<string>("_SEL_FILE"))) {
                Session.ToastMessage("Please select the file to upload.", ToastMessageType.error);
                return false;
            }

            // not allow to upload file size greter than 2MB
            double maxUploadSize = Session.AppConfig.maxUploadSize * 1024 * 1000;
            if (fileSize > maxUploadSize || fileSize == 0) {
                Session.ToastMessage("Max file size allowed to upload is " + Session.AppConfig.maxUploadSize.ToStr() + " MB.", ToastMessageType.error);
                return false;
            }

           
            JObject postData = new JObject();
            postData.Add("COMP_CODE", Session.CompCode);
            postData.Add("DOC_TYPE", GetParentFieldValue<string>("DOC_TYPE"));
            postData.Add("DEPT_CODE", GetParentFieldValue<string>("DEPT_CODE"));
            postData.Add("RUN_NO", GetParentFieldValue<string>("RUN_NO"));
            postData.Add("SEL_FILE", GetFieldValue<string>("_SEL_FILE"));
            postData.Add("FRIENDLY_NAME", GetFieldValue<string>("_FRIENDLY_NAME"));
            postData.Add("FILE_REMARKS", GetFieldValue<string>("_FILE_REMARKS"));
            postData.Add("KEY_CODE", GetFieldValue<string>("_KEY_CODE"));
            postData.Add("LINE_NO", GetFieldValue<string>("_LINE_NO"));
            postData.Add("UPLD_FILENAME", GetFieldValue<string>("_UPLD_FILENAME"));
            postData.Add("ACT_FILENAME", GetFieldValue<string>("_ACT_FILENAME"));
            postData.Add("FILE_TYPE", GetFieldValue<string>("_FILE_TYPE"));
            postData.Add("FILE_SIZE", GetFieldValue<string>("_FILE_SIZE"));
            postData.Add("FILE_EXT", GetFieldValue<string>("_FILE_EXT"));
            postData.Add("USER_LOG", Session.UserID);
            postData.Add("TIME_LOG", DateTime.Now);

            var result = await Session.ExecJSAsync("_uploadFile",
                Session.AccessToken.token_type, Session.AccessToken.access_token, postData.ToString(), ScrnId);


            return result.ToBool();
            
        }

        #endregion

    }
}
