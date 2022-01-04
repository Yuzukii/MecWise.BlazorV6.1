using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;
using MecWise.Blazor.Api.Services;
using System.IO;
using MecWise.Blazor.Common;

namespace MecWise.Blazor.Api.Controllers
{
    [Authorize]
    public class FileController : ApiController
    {
        FileService _fileService;

        public FileController() {
            string[] sessionHeader = System.Web.HttpContext.Current.Request.Headers.GetValues("Session");
            if (sessionHeader != null) {
                string jsonStr = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(sessionHeader[0]));
                SessionState session = JObject.Parse(jsonStr).ToObject<SessionState>();
                _fileService = new FileService(session);
            }
            else {
                _fileService = new FileService();
            }
        }

        [HttpPost]
        [Route("File/Upload")]
        public async Task<HttpResponseMessage> Upload() {
            string value = await Request.Content.ReadAsStringAsync();
            try {
                JObject postData = JObject.Parse(value);
                _fileService.SaveUploadFile(ApiSetting.UploadFolderAbsolutePath, postData);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("File/DirectUpload")]
        public async Task<HttpResponseMessage> DirectUpload() {
            string value = await Request.Content.ReadAsStringAsync();
            try {
                JObject postData = JObject.Parse(value);
                string fileName = _fileService.SaveDirectUploadFile(ApiSetting.UploadFolderAbsolutePath, postData);
                JObject returnData = new JObject();
                returnData.Add("UPLOAD_FILE_NAME", fileName);
                return Request.CreateResponse(HttpStatusCode.OK, returnData);
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("File/Delete")]
        public async Task<HttpResponseMessage> Delete() {
            string value = await Request.Content.ReadAsStringAsync();
            try {
                JObject postData = JObject.Parse(value);
                _fileService.DeleteFile(ApiSetting.UploadFolderAbsolutePath, postData);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("File/Download")]
        public async Task<HttpResponseMessage> Download() {
            string value = await Request.Content.ReadAsStringAsync();
            try {
                JObject postData = JObject.Parse(value);
                return Request.CreateResponse(HttpStatusCode.OK, _fileService.DownloadFile(postData));
            }
            catch (Exception ex) {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

    }
}
