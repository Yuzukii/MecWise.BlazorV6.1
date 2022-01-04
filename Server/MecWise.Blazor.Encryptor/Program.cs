using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using ePlatform.Security;

namespace MecWise.Blazor.Encryptor {
    class Program {
        static void Main(string[] args) {
            if (args.Length == 0) {
                return;
            }

            string clearText = args[0];
            EPEncrypt enc = new EPEncrypt();
            string encryptedString = enc.EncryptString(clearText);
            Console.WriteLine(encryptedString);
        }
    }
}
