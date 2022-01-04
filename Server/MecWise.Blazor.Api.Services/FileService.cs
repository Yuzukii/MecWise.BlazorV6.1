using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MecWise.Blazor.Entities;
using MecWise.Blazor.Api.DataAccess;
using MecWise.Blazor.Api.Repositories;
using MecWise.Blazor.Common;
using System.IO;
using Newtonsoft.Json.Linq;

namespace MecWise.Blazor.Api.Services
{
    public class FileService {

        FileRepository _fileRepository;

        public FileService() {
            DBHelper db = new DBHelper(ApiSetting.ConnectionString);
            _fileRepository = new FileRepository(db);
        }

        public FileService(SessionState session) 
        {
            DBHelper db = new DBHelper(session.UserID, session.CompCode, session.LangId, ApiSetting.ConnectionString);
            _fileRepository = new FileRepository(db);
        }

        public void SaveUploadFile(string upldFolder, JObject postData)
        {
            // generate id & file names.
            string uniqueId = Guid.NewGuid().ToString();
            int lineNo = _fileRepository.GetNewLineNo(postData);
            string upldFileName = string.Format("{0}_{1}_{2}_{3}_{4}_{5}.{6}",
                postData["COMP_CODE"], postData["DOC_TYPE"], postData["DEPT_CODE"], postData["RUN_NO"], lineNo, uniqueId, postData["FILE_EXT"]);
            string actFileName = string.Format("{0}.{1}", uniqueId, postData["FILE_EXT"]);


            // get absolute file path under upload folder
            string absolutePath = upldFolder + "\\" + upldFileName;

            // convert posted base64FileString to byte[]
            byte[] bytes = Convert.FromBase64String(postData["base64FileString"].ToString());

            // write file
            File.WriteAllBytes(absolutePath, bytes);

            // save record to database 
            postData["LINE_NO"] = lineNo;
            postData["UPLD_FILENAME"] = lineNo;
            postData["UPLD_FILENAME"] = upldFileName;
            postData["ACT_FILENAME"] = actFileName;
            _fileRepository.SaveFileRecord(postData);
        }

        public string SaveDirectUploadFile(string upldFolder, JObject postData) {
            // generate id & file names.
            string uniqueId = Guid.NewGuid().ToString();
            string upldFileName = string.Format("{0}@{1}@{2}", DateTime.Now.ToString("yyyyMMdd"), uniqueId, postData["SEL_FILE"]);
            
            // get absolute file path under upload folder
            string absolutePath = upldFolder + "\\" + upldFileName;

            // convert posted base64FileString to byte[]
            byte[] bytes = Convert.FromBase64String(postData["base64FileString"].ToString());

            // write file
            File.WriteAllBytes(absolutePath, bytes);

            return upldFileName;
        }

        public void DeleteFile(string upldFolder, JObject postData) {
            // get file path under upload folder
            string absolutePath = upldFolder  + "\\" + postData["UPLD_FILENAME"].ToString();

            // Delete file
            File.Delete(absolutePath);
            
            // delete record from database 
            _fileRepository.DeleteFileRecord(postData);
        }

        public JObject DownloadFile(JObject postData) {
            string absolutePath = "";

            if (postData.ContainsKey("UTILITY_DOWNLOAD")) {
                if (postData["UTILITY_DOWNLOAD"].ToBool()) {
                    // get from wwwroot\app-data\download folder for Utility download
                    // get file path under wwwroot\app-data\download folder
                    absolutePath = Path.Combine(ApiSetting.WebClientRootFolderAbsolutePath, "app-data", "download", postData["FILE_NAME"].ToStr());
                }
            }
            else {
                // get the file from upload folder for re-downloading uploaded file
                // get file path under upload folder
                absolutePath = Path.Combine(ApiSetting.UploadFolderAbsolutePath, postData["UPLD_FILENAME"].ToStr());
            }


            // Read file
            byte[] bytes = File.ReadAllBytes(absolutePath);
            string base64FileString = Convert.ToBase64String(bytes);

            if (!postData.ContainsKey("base64FileString")) {
                postData.Add("base64FileString", base64FileString);
            }

            return postData;
        }

    }
}
