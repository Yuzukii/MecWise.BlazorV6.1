using System;
using MecWise.Blazor;
using MecWise.Blazor.Common;
using MecWise.Blazor.Components;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Web;
namespace MecWise.Blazor.Utilities.Client {
    public class EPF_PHOTO_UPLD_BLZ : Screen {

        string _returnFieldId; // parent screen field that will return photo file name
        bool _noConfirm;    // no confirmation require, photo will upload straight away after capturing

        FieldButton _btnGetImage;
        FieldButton _btnConfirm;
        FieldButton _btnFrontBack;

        private enum CameraFacingMode {
            environment,
            user
        }
        private enum VidoeAction { 
            none,
            video,
            image
        }

        VidoeAction _currAction;
        CameraFacingMode _cameraFacingMode;

        #region "Screen Events"

        protected override async Task<object> AfterRenderAsync() {
            
            await Session.ExecJSAsync("_renderVideoElements", this.ScrnId);
            btnGetImage_OnClick();
                  
            return base.AfterRenderAsync();
        }

        protected override async Task<object> LoadAsync() {
            _returnFieldId = GetQueryParamValue("RETURN_FIELD_ID");
            _noConfirm = GetQueryParamValue("NO_CONFIRM").ToBool();

            _currAction = VidoeAction.none;
            _cameraFacingMode = CameraFacingMode.environment;

            _btnGetImage = Fields.Search<FieldButton>("btnGetImage");
            _btnConfirm = Fields.Search<FieldButton>("btnConfirm");
            _btnFrontBack = Fields.Search<FieldButton>("btnFrontBack");

            _btnFrontBack.Description = "Front";
            _btnFrontBack.IconCssClass = "oi oi-loop-circular";

            _btnConfirm.Description = "Confirm";
            _btnConfirm.IconCssClass = "oi oi-check";
            if (_noConfirm) {
                _btnConfirm.Visible = false;
            }

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

        public async void btnGetImage_OnClick() {
            
            if (_currAction == VidoeAction.video) {
                _currAction = VidoeAction.image;
                _btnConfirm.Disabled = false;
                _btnGetImage.Description = "Undo";
                _btnGetImage.IconCssClass = "oi oi-action-undo";
                await Session.ExecJSAsync("_captureImage");

                if (_noConfirm) { // no confirmation require, photo will upload straight away after capturing
                    btnConfirm_OnClick();
                }
            }
            else {
                _currAction = VidoeAction.video;
                _btnConfirm.Disabled = true;
                _btnGetImage.Description = "Capture";
                _btnGetImage.IconCssClass = "oi oi-camera-slr";
                await Session.ExecJSAsync("_getVideoStream", _cameraFacingMode.ToStr());
            }
        }

        public async void btnConfirm_OnClick() {
            
            string fileName = string.Format("PHOTO_{0}.jpg", DateTime.Now.ToString("yyyyMMddhhmmssff"));

            JObject postData = new JObject();
            postData.Add("SEL_FILE", fileName);
            postData.Add("FILE_TYPE", GetFieldValue<string>("_FILE_TYPE"));
            postData.Add("FILE_SIZE", GetFieldValue<string>("_FILE_SIZE"));
            postData.Add("FILE_EXT", GetFieldValue<string>("_FILE_EXT"));

            var uploadFileName = await Session.ExecJSAsync("_directUploadPhoto",
                Session.AccessToken.token_type, Session.AccessToken.access_token, postData.ToString());

            SetParentFieldValue(_returnFieldId, uploadFileName);

            this.Close();
        }

        public async void btnFrontBack_OnClick() {
            _btnFrontBack.IconCssClass = "oi oi-loop-circular";

            if (_cameraFacingMode == CameraFacingMode.user) {
                _btnFrontBack.Description = "Front";
                _cameraFacingMode = CameraFacingMode.environment;
            }
            else {
                _btnFrontBack.Description = "Back";
                _cameraFacingMode = CameraFacingMode.user;
            }    

            if(_currAction == VidoeAction.video)
                await Session.ExecJSAsync("_getVideoStream", _cameraFacingMode.ToStr());
        }

        #endregion




        #region "Helper Functions"

        #endregion

    }
}
