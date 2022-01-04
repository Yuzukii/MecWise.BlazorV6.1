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

    public class FieldQuickAccessMenu : Field 
    {
        SessionState _session;

        public string HeaderText { get; set; } = "Quick Access";

        public JArray MenuList { get; set; }

        public FieldQuickAccessMenu(SessionState sesion) : base() {
            _session = sesion;
            InitQuickAccessMenu();
        }

        public FieldQuickAccessMenu(SessionState sesion, string id, string description) : base(id, description) {
            _session = sesion;
            InitQuickAccessMenu();
        }

        public FieldQuickAccessMenu(SessionState sesion, string id, string description, string styleClassWidth) : base(id, description, styleClassWidth) {
            _session = sesion;
            InitQuickAccessMenu();
        }

        private async void InitQuickAccessMenu() {
            string url = string.Format("Menu/GetQuickAccessMenu/{0}/{1}", _session.UserID, _session.LangId);
            MenuList = await _session.GetJsonAsync<JArray>(_session.GetAbsoluteApiUrl(url));
        }
    }

}
