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


    public enum FieldButtonType {
        epfNormal,
        epfAdd,
        epfSave,
        epfDelete,
        epfHistoryBack,
        epfMultiPick,
        epfUpload,
        epfMovePrev,
        epfMoveNext
    }


    public class FieldButton : Field 
    {
        string _iconCssClass;

        public bool SaveOnNavigate { get; set; } = false;
        public FieldButtonType ButtonType { get; set; } = FieldButtonType.epfNormal;
        public bool FileUploadReadOnly { get; set; } = false;
        public string IconCssClass {
            set {
                _iconCssClass = value;
            }
            get {
                string iconClass = "";
                switch (ButtonType) {
                    case FieldButtonType.epfNormal:
                        iconClass = _iconCssClass;
                        break;
                    case FieldButtonType.epfAdd:
                        iconClass = "oi oi-plus mr-2";
                        break;
                    case FieldButtonType.epfSave:
                        iconClass = "oi oi-check mr-2";
                        break;
                    case FieldButtonType.epfDelete:
                        iconClass = "oi oi-trash mr-2";
                        break;
                    case FieldButtonType.epfHistoryBack:
                        iconClass = "oi oi-arrow-thick-left mr-2";
                        break;
                    case FieldButtonType.epfMultiPick:
                        iconClass = "oi oi-list-rich mr-2";
                        break;
                    case FieldButtonType.epfUpload:
                        iconClass = "oi oi-cloud-upload mr-2";
                        break;
                }
                return iconClass;
            } 
        }
        public FieldButton() : base()
        {
        }

        public FieldButton(string id, string description) : base(id, description)
        {
        }

        public FieldButton(string id, string description, string styleClassWidth) : base(id, description, styleClassWidth)
        {
        }

    }

        


}
