using System;
using System.Collections.Generic;
using System.Text;

namespace MecWise.Blazor.Entities
{

    [System.AttributeUsage(System.AttributeTargets.Class |
                       System.AttributeTargets.Struct)]
    public class EntitySettingAttribute : System.Attribute
    {
        public string BaseViewName { get; set; }
        public string BaseProcedureName { get; set; }
        public string[] PrimaryKeys { get; set; }
    }
}
