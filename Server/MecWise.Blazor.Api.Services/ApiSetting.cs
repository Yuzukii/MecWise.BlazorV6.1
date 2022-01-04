using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Configuration;
using System.Runtime.Remoting.Messaging;
using MecWise.Blazor.Common;
using System.Collections.Specialized;

namespace MecWise.Blazor.Api.Services {
    public static class ApiSetting {
        public static string ConnectionString { 
            get {
                var connectInfo = (NameValueCollection)ConfigurationManager.GetSection("connectInfo");
                return connectInfo["DEFAULT"];
            } 
        }

        public static string UploadFolder {
            get {
                return ConfigurationManager.AppSettings["UPLD_FOLDER"];
            }
        }

        public static string UploadFolderAbsolutePath {
            get {
                string baseDirectory = System.Web.HttpContext.Current.Server.MapPath("~");
                string uploadfolder = ConfigurationManager.AppSettings["UPLD_FOLDER"];
                string uploadFolderAbsolutePath = Path.GetFullPath(baseDirectory + "\\" + uploadfolder.Trim('\\'));
                return uploadFolderAbsolutePath;
            }
        }

        public static string WebsiteRootFolderAbsolutePath {
            get {
                string rootDirectory = System.Web.HttpContext.Current.Server.MapPath("~");
                return rootDirectory;
            }
        }

        public static string WebClientRootFolderAbsolutePath
        {
            get
            {
                string baseDirectory = System.Web.HttpContext.Current.Server.MapPath("~");
                string webClientFolder = ConfigurationManager.AppSettings["WEB_CLIENT_ROOT_FOLDER"];
                string webClientAbsolutePath = Path.GetFullPath(baseDirectory + "\\" + webClientFolder.Trim('\\'));
                return webClientAbsolutePath;
            }
        }

        public static int SessionExpireTime
        {
            get
            {
                if (ConfigurationManager.AppSettings.AllKeys.Contains("SESSION_EXPIRE_TIME"))
                {
                    return ConfigurationManager.AppSettings["SESSION_EXPIRE_TIME"].ToInt();
                }
                else {
                    return 60; // default 60 minutes
                }
            }
        }

        public static Dictionary<string, string> MicroServiceForModules {
            get {
                string keyPrefix = "MicroServices.";
                var keys = ConfigurationManager.AppSettings.AllKeys.ToList().FindAll(x => x.StartsWith(keyPrefix));
                if (keys != null) {
                    Dictionary<string, string> result = new Dictionary<string, string>();
                    foreach (var key in keys) {
                        string keyWithoutPrefix = key.Replace(keyPrefix, "");
                        result.Add(keyWithoutPrefix, ConfigurationManager.AppSettings[key]);
                    }
                    return result;
                }

                return null;
            }
        }

    }
}