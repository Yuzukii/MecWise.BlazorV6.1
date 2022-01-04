using System;
using System.Collections.Generic;
using System.Text;
using MecWise.Blazor.Common;
using MecWise.Blazor.Entities;
using Newtonsoft.Json.Linq;

namespace MecWise.Blazor.Components
{
    public enum FieldTitleBarMenuItemType { 
        Ok,
        Cancel,
        New,
        Save,
        Delete,
        Return,
        Next,
        Prev
    }

    public class FieldTitleBar : Field
    {

        public bool MenuVisible { get; set; } = true;
        public bool ShowUnderline { get; set; } = true;
        public bool SubTitle { get; set; } = false;
        public string TitleText { get; set; }
        public List<FieldTitleBarMenuItem> MenuItemList { get; set; }

        public event EventHandler<FieldTitleBarMenuItem> OnItemClick;
        internal void InvokeOnItemClick(object sender, FieldTitleBarMenuItem e)
        {
            EventHandler<FieldTitleBarMenuItem> handler = OnItemClick;
            if (handler != null)
                handler(sender, e);
        }


        public FieldTitleBar() {
            this.MenuItemList = new List<FieldTitleBarMenuItem>();
        }

        public void SetMenuItemVisible(FieldTitleBarMenuItemType menuType, bool visible) {
            switch (menuType) {
                case FieldTitleBarMenuItemType.Ok:
                    this.MenuItemList.FindAll(x => x is FieldTitleBarMenuItemOk).ForEach(y => {
                        y.Visible = visible;
                    });
                    break;
                case FieldTitleBarMenuItemType.Cancel:
                    this.MenuItemList.FindAll(x => x is FieldTitleBarMenuItemCancel).ForEach(y => {
                        y.Visible = visible;
                    });
                    break;
                case FieldTitleBarMenuItemType.New:
                    this.MenuItemList.FindAll(x => x is FieldTitleBarMenuItemNew).ForEach(y => {
                        y.Visible = visible;
                    });
                    break;
                case FieldTitleBarMenuItemType.Save:
                    this.MenuItemList.FindAll(x => x is FieldTitleBarMenuItemSave).ForEach(y => {
                        y.Visible = visible;
                    });
                    break;
                case FieldTitleBarMenuItemType.Delete:
                    this.MenuItemList.FindAll(x => x is FieldTitleBarMenuItemDelete).ForEach(y => {
                        y.Visible = visible;
                    });
                    break;
                case FieldTitleBarMenuItemType.Return:
                    this.MenuItemList.FindAll(x => x is FieldTitleBarMenuItemReturn).ForEach(y => {
                        y.Visible = visible;
                    });
                    break;
                case FieldTitleBarMenuItemType.Next:
                    this.MenuItemList.FindAll(x => x is FieldTitleBarMenuItemNext).ForEach(y => {
                        y.Visible = visible;
                    });
                    break;
                case FieldTitleBarMenuItemType.Prev:
                    this.MenuItemList.FindAll(x => x is FieldTitleBarMenuItemPrev).ForEach(y => {
                        y.Visible = visible;
                    });
                    break;
                default:
                    break;
            }
        }

        public void Initialize() {
            this.MenuItemList = new List<FieldTitleBarMenuItem>();
            
            if (this.ParentScreen.SCRN_DEF.FORM_TYPE != "BRWS") {
                if (this.ParentScreen.ScrnMode == ScreenMode.Add) {
                    this.MenuItemList.Add(new FieldTitleBarMenuItemSave());
                }
                else if (this.ParentScreen.ScrnMode == ScreenMode.Update) {
                    this.MenuItemList.Add(new FieldTitleBarMenuItemNew());
                    this.MenuItemList.Add(new FieldTitleBarMenuItemNext());
                    this.MenuItemList.Add(new FieldTitleBarMenuItemPrev());

                    if (this.ParentScreen.SCRN_DEF.EDIT_MODES.Contains("U")) {
                        this.MenuItemList.Add(new FieldTitleBarMenuItemSave());
                    }

                    if (this.ParentScreen.SCRN_DEF.EDIT_MODES.Contains("D")) {
                        this.MenuItemList.Add(new FieldTitleBarMenuItemDelete());
                    }

                }
                else if (this.ParentScreen.ScrnMode == ScreenMode.Neutral) {
                    this.MenuItemList.Add(new FieldTitleBarMenuItemNew());
                }
                else if (this.ParentScreen.ScrnMode == ScreenMode.Enquiry) {
                    this.MenuItemList.Add(new FieldTitleBarMenuItemNew());
                    this.MenuItemList.Add(new FieldTitleBarMenuItemDelete());
                    this.MenuItemList.Add(new FieldTitleBarMenuItemNext());
                    this.MenuItemList.Add(new FieldTitleBarMenuItemPrev());

                }
            }

            if (this.ParentScreen.ParentScreen != null || this.ParentScreen.Session.GetNavigationHistoryCount() > 1) {
                this.MenuItemList.Add(new FieldTitleBarMenuItemReturn());
            }
            else {
                //if (screen.SCRN_TYPE == EpfScreen.ScreenType.Popup) {
                //    this.MenuItemList.Add(new FieldTitleBarMenuItemClose());
                //}
            }

            this.ApplyAccessRights();
        }


        public void ApplyAccessRights() {
            
            if (this.ParentScreen == null) {
                return;
            }


            if (!this.ParentScreen.HasRights(ScreenMode.Add)) {
                this.SetMenuItemVisible(FieldTitleBarMenuItemType.New, false);
            }

            if (!this.ParentScreen.HasRights(ScreenMode.Update) && this.ParentScreen.ScrnMode == ScreenMode.Update) {
                this.SetMenuItemVisible(FieldTitleBarMenuItemType.Save, false);
            }

            if (!this.ParentScreen.HasRights(ScreenMode.Delete)) {
                this.SetMenuItemVisible(FieldTitleBarMenuItemType.Delete, false);
            }

        }

    }

    public class FieldTitleBarMenuItem
    {
        protected string _iconText;
        protected string _iconCssClass;
        public string IconText { get { return _iconText; } }
        public string IconCssClass { get { return _iconCssClass; } }
        public bool Visible { get; set; } = true;
    }

    public class FieldTitleBarMenuItemClose : FieldTitleBarMenuItem {
        public FieldTitleBarMenuItemClose() {
            this._iconText = "Close";
            this._iconCssClass = "oi oi-x";
        }
    }
    public class FieldTitleBarMenuItemOk : FieldTitleBarMenuItem
    {
        public FieldTitleBarMenuItemOk() {
            this._iconText = "Ok";
            this._iconCssClass = "";
        }
    }
    public class FieldTitleBarMenuItemCancel : FieldTitleBarMenuItem {
        public FieldTitleBarMenuItemCancel() {
            this._iconText = "Cancel";
        }
    }
    public class FieldTitleBarMenuItemNew : FieldTitleBarMenuItem {
        public FieldTitleBarMenuItemNew() {
            this._iconText = "New";
            this._iconCssClass = "oi oi-plus";
        }
    }
    public class FieldTitleBarMenuItemSave : FieldTitleBarMenuItem
    {
        public FieldTitleBarMenuItemSave()
        {
            this._iconText = "Save";
            this._iconCssClass = "oi oi-check";
        }
    }
    public class FieldTitleBarMenuItemDelete : FieldTitleBarMenuItem {
        public FieldTitleBarMenuItemDelete() {
            this._iconText = "Delete";
            this._iconCssClass = "oi oi-trash";
        }
    }
    public class FieldTitleBarMenuItemReturn : FieldTitleBarMenuItem {
        public FieldTitleBarMenuItemReturn() {
            this._iconText = "Return";
            this._iconCssClass = "oi oi-arrow-thick-left";
        }
    }
    public class FieldTitleBarMenuItemNext : FieldTitleBarMenuItem {
        public FieldTitleBarMenuItemNext() {
            this._iconText = "Move Next";
            this._iconCssClass = "oi oi-media-step-forward";
            this.Visible = false;
        }
    }
    public class FieldTitleBarMenuItemPrev : FieldTitleBarMenuItem {
        public FieldTitleBarMenuItemPrev() {
            this._iconText = "Move Previous";
            this._iconCssClass = "oi oi-media-step-backward";
            this.Visible = false;
        }
    }
}


