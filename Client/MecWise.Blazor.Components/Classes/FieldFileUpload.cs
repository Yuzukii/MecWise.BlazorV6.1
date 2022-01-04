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

    public class FieldFileUpload : Field 
    {

        public FieldButton MultiPickButton { get; set; }

        public event EventHandler<FieldClickEventArgs> OnFileUploadClick;
        internal void InvokeOnFileUploadClick(object sender, FieldClickEventArgs e) {
            EventHandler<FieldClickEventArgs> handler = OnFileUploadClick;
            if (handler != null)
                handler(sender, e);
        }

        public FieldFileUpload() : base() {
            MultiPickButton = new FieldButton();
            MultiPickButton.ButtonType = FieldButtonType.epfMultiPick;
            MultiPickButton.OnClick += Button_OnClick;
        }

        public FieldFileUpload(string id, string description) : base(id, description) {
            MultiPickButton = new FieldButton();
            MultiPickButton.ButtonType = FieldButtonType.epfUpload;
            MultiPickButton.OnClick += Button_OnClick;
        }

        public FieldFileUpload(string id, string description, string styleClass) : base(id, description, styleClass) {
            MultiPickButton = new FieldButton();
            MultiPickButton.ButtonType = FieldButtonType.epfMultiPick;
            MultiPickButton.OnClick += Button_OnClick;
        }

        private void Button_OnClick(object sender, FieldClickEventArgs e) {
            this.InvokeOnFileUploadClick(this, e);
        }

    }

        


}
