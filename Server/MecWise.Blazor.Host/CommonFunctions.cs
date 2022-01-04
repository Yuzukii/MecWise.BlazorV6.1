using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MecWise.Blazor.Host {
    public class CommonFunctions {
        public static string EncryptString(string clearText) {
            Process compiler = new Process();
            compiler.StartInfo.FileName = "MecWise.Blazor.Encryptor.exe";
            compiler.StartInfo.Arguments = "\"" + clearText + "\"";
            compiler.StartInfo.UseShellExecute = false;
            compiler.StartInfo.RedirectStandardOutput = true;
            compiler.Start();
            string encryptedString = compiler.StandardOutput.ReadToEnd();
            compiler.WaitForExit();
            return encryptedString;
        }

        public static string GetTokenforOpenIDConnect(string userName) {
            string clearText = string.Format("{0};mecwise.blazor;{1};", userName, DateTime.Now.ToString("yyMMdd HH:mm:ss"));
            return EncryptString(clearText);
        }
    }
}
