using System;
using System.Collections;
using System.Collections.Generic;
using MecWise.Blazor.Common;
using Newtonsoft.Json.Linq;
using MecWise.Blazor.Entities;
using System.Threading.Tasks;

namespace MecWise.Blazor.Components
{
    public class FieldImage : Field
    {

        public int Height { get; set; } = 60;
        public string Source { get; set; } = "";

        public FieldImage() : base()
        {
        }

        public FieldImage(string id, string description) : base(id, description)
        {
        }

        public FieldImage(string id, string description, string styleClassWidth) : base(id, description, styleClassWidth)
        {
        }

    }

}