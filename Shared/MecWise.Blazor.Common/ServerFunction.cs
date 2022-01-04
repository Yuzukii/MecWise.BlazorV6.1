using System;
using System.Collections.Generic;
using System.Text;

namespace MecWise.Blazor.Common {
    public class ServerFunction {
        public string AssemblyName { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public object[] Parameters { get; set; }

        public ServerFunction() { }

        public ServerFunction(string assemblyName, string className, string methodName, params object[] parameters) {
            AssemblyName = assemblyName;
            ClassName = className;
            MethodName = methodName;
            Parameters = parameters;
        }
    }
}
