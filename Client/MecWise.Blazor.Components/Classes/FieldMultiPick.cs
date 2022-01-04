using System;
using System.Collections;
using System.Collections.Generic;
using MecWise.Blazor.Common;
using Newtonsoft.Json.Linq;
using MecWise.Blazor.Entities;
using System.Threading.Tasks;

namespace MecWise.Blazor.Components
{
    public class FieldMultiPick : Field
    {

        public bool ServerSidePaging { get; set; } = false;
        public FieldGridViewScrollType ScrollType { get; set; } = FieldGridViewScrollType.@virtual;

        public override string Description { 
            get { return base.Description; } 
            set { 
                base.Description = value;
                if (this.MultiPickButton != null) {
                    this.MultiPickButton.Description = value;
                }
            } 
        }

        public override string StyleClass {
            get { return base.StyleClass; }
            set {
                base.StyleClass = value;
                if (this.MultiPickButton != null) {
                    this.MultiPickButton.StyleClass = value;
                }
            }
        }

        public override string StyleClassWidth {
            get { return base.StyleClassWidth; }
            set {
                base.StyleClassWidth = value;
                if (this.MultiPickButton != null) {
                    this.MultiPickButton.StyleClassWidth = value;
                }
            }
        }

        public override bool Disabled {
            get { return base.Disabled; }
            set {
                base.Disabled = value;
                if (this.MultiPickButton != null) {
                    this.MultiPickButton.Disabled = value;
                }
            }
        }

        public override bool ReadOnly {
            get { return base.ReadOnly; }
            set {
                base.ReadOnly = value;
                if (this.MultiPickButton != null) {
                    this.MultiPickButton.ReadOnly = value;
                }
            }
        }

        public FieldGridViewSelectionMode SelectionMode { get; set; } = FieldGridViewSelectionMode.multiple;
        public bool GirdVisible { get; set; } = false;
        public bool IsGroupSelection { get; set; } = false;
        public bool TwoStepMultiPick { get; set; } = false;
        public Dictionary<string,string> KeyValues { get; set; }

        public FieldGridView BrwsGrid { get; set; }

        public FieldButton MultiPickButton { get; set; }
        public JObject SelectedGroup { get; set; }

        public string BrwsId { get; set; }

        public string FooterText { get; set; }

        public event EventHandler<FieldClickEventArgs> OnMultiPickClick;
        internal void InvokeOnMultiPickClick(object sender, FieldClickEventArgs e)
        {
            EventHandler<FieldClickEventArgs> handler = OnMultiPickClick;
            if (handler != null)
                handler(sender, e);
        }

        public event EventHandler<FieldChangeEventArgs> OnSelectionChange;
        internal void InvokeOnSelectionChange(object sender, FieldChangeEventArgs e)
        {
            EventHandler<FieldChangeEventArgs> handler = OnSelectionChange;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        public event EventHandler<FieldChangeEventArgs> OnGroupSelectionChange;
        internal void InvokeOnGroupSelectionChange(object sender, FieldChangeEventArgs e) {
            EventHandler<FieldChangeEventArgs> handler = OnGroupSelectionChange;
            if (handler != null) {
                handler(sender, e);
            }
        }

        public event EventHandler<EventArgs> OnClose;
        internal void InvokeOnClose(object sender, EventArgs e) {
            EventHandler<EventArgs> handler = OnClose;
            if (handler != null) {
                handler(sender, e);
            }
        }

        public void ReInitBrwsGrid() {
            BrwsGrid = new FieldGridView();
            BrwsGrid.ID = string.Format("_brwsgrid_{0}", ID);
            BrwsGrid.SelectionMode = FieldGridViewSelectionMode.multiple;
            BrwsGrid.ShowTitleBar = false;
            BrwsGrid.ParentScreen = this.ParentScreen;
            BrwsGrid.BRWS_ID = this.BrwsId;
            BrwsGrid.ScrollType = this.ScrollType;
            BrwsGrid.ServerSidePaging = this.ServerSidePaging;
        }

        public FieldMultiPick() : base()
        {
            MultiPickButton = new FieldButton();
            MultiPickButton.ButtonType = FieldButtonType.epfMultiPick;
            MultiPickButton.OnClick += Button_OnClick;

            ReInitBrwsGrid();

            KeyValues = new Dictionary<string, string>();
        }

        public FieldMultiPick(string id, string description) : base(id, description)
        {
            MultiPickButton = new FieldButton(id, description);
            MultiPickButton.ButtonType = FieldButtonType.epfMultiPick;
            MultiPickButton.OnClick += Button_OnClick;

            ReInitBrwsGrid();

            KeyValues = new Dictionary<string, string>();
        }

        public FieldMultiPick(string id, string description, string styleClassWidth) : base(id, description, styleClassWidth)
        {
            MultiPickButton = new FieldButton(id, description, styleClassWidth);
            MultiPickButton.ButtonType = FieldButtonType.epfMultiPick;
            MultiPickButton.OnClick += Button_OnClick;

            ReInitBrwsGrid();

            KeyValues = new Dictionary<string, string>();
        }
        private void Button_OnClick(object sender, FieldClickEventArgs e)
        {
            this.InvokeOnMultiPickClick(this, e);
        }

        public async Task ShowPopup(SessionState session) {
            this.GirdVisible = false;
            this.BrwsGrid.ParentScreen = this.ParentScreen;
            await session.ExecJSAsync("_clearMultiPickSelectedRows", this.BrwsGrid.ElementID);
            await session.ExecJSAsync("_showMultiPick", "_modal_" + this.ElementID);
            await session.ExecJSAsync("_assign_field_prop", this.BrwsGrid.ElementID, this.BrwsGrid.GetFieldProp().ToString());
        }

        public void ShowGrid() {
            this.GirdVisible = true;
        }

        public void HideGrid() {
            this.GirdVisible = false;
        }

        public async Task HidePopup(SessionState session) {
            this.GirdVisible = false;
            await session.ExecJSAsync("_hideMultiPick", "_modal_" + this.ElementID);
        }
    }
}