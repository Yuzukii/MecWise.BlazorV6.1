using System;
using System.Collections;
using System.Collections.Generic;
using MecWise.Blazor.Common;
using Newtonsoft.Json.Linq;
using MecWise.Blazor.Entities;
using System.Threading.Tasks;
using System.Linq;

namespace MecWise.Blazor.Components
{

    public class FieldScreenWrapper : Field 
    {
        
        public string Source { get; set; }

        public FieldScreenWrapper() : base() {
        }

        public FieldScreenWrapper(string id, string description) : base(id, description) {
        }

        public FieldScreenWrapper(string id, string description, string styleClassWidth) : base(id, description, styleClassWidth) {
        }

    }

}
