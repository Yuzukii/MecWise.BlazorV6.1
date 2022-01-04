using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using MecWise.Blazor.Common;
using System.Linq;
using MecWise.Blazor.Entities;

namespace MecWise.Blazor.Components
{

        
    public enum FieldAccordionButtonType {
        edit,
        delete,
        download
    }

    public class FieldAccordion: Field
    {
        public event EventHandler<FieldChangeEventArgs> OnItemDetailClick;
        internal void InvokeOnItemDetailClick(object sender, FieldChangeEventArgs e)
        {
            EventHandler<FieldChangeEventArgs> handler = OnItemDetailClick;
            if (handler != null)
                handler(sender, e);
        }

        public event EventHandler<FieldClickEventArgs> OnAddNewClick;
        internal void InvokeOnAddNewClick(object sender, FieldClickEventArgs e)
        {
            EventHandler<FieldClickEventArgs> handler = OnAddNewClick;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<FieldGridActionEventArgs> OnAccordionAction;
        internal void InvokeOnAccordionAction(object sender, FieldGridActionEventArgs e) {
            EventHandler<FieldGridActionEventArgs> handler = OnAccordionAction;
            if (handler != null) {
                handler(sender, e);
            }
        }

        public int AnimationDuration { get; set; } = 300;
        public bool Collapsible { get; set; } = true;
        public bool Multiple { get; set; } = false;
        public JArray DataSource { get; set; }
        public List<FieldAccordionColumn> Columns { get; set; }
        public string BRWS_ID { get; set; }
        public FieldAccordionType AccordionType { get; set; }
        public string ScreenId { get; set; }
        public string NavigateScrnId { get; set; }
        public EPF_SCRN NavigateScrnDefinition { get; set; }
        public string TargetKeys { get; set; }
        public string ScreenKeys { get; set; }
        public bool SaveOnNavigate { get; set; } = false;
        public bool ShowTitleBar { get; set; } = true;
        public bool ShowTitleBarAddNew { get; set; } = true;
        internal FieldTitleBar TitleBar { get; set; }
        public bool PopupLinkScreen { get; set; } = false;
        public List<FieldAccordionButtonType> Buttons { get; set; } = new List<FieldAccordionButtonType>();
        public bool DeleteRecord { get; set; } = false;
        public bool ShowDetail { get; set; } = true;
        public FieldGridEditRow EditRow { get; set; }

        public FieldAccordion()
        {
            this.DataSource = new JArray();
            this.Columns = new List<FieldAccordionColumn>();
            this.TitleBar = new FieldTitleBar();
        }

        public override string GetFieldType() {
            return _fieldType.dxAccordion.ToString();
        }

        public override JObject GetFieldProp()
        {
            JObject prop = new JObject();
            prop.Add("fieldType", "dxAccordion");
            prop.Add("dataSource", this.DataSource);
            prop.Add("accordionType", this.AccordionType.ToStr());
            prop.Add("columns", JArray.FromObject(this.Columns));
            prop.Add("animationDuration", this.AnimationDuration);
            prop.Add("collapsible", this.Collapsible);
            prop.Add("multiple", this.Multiple);

            List<string> buttons = this.Buttons.Distinct().ToList().ConvertAll<string>(x => x.ToString());
            prop.Add("buttons", JArray.FromObject(buttons));
            prop.Add("deleteRecord", this.DeleteRecord);
            prop.Add("showDetail", this.ShowDetail);
            prop.Add("screenId", this.ScreenId);

            return prop;
        }

        public void InitColumns(Dictionary<string, JObject> columnInfos, string colWidths, string colCaptions, string colFormats, string colAligns)
        {
            this.Columns.Clear();

            foreach (var col in columnInfos.Values)
            {
                int colIndex = col["column_ordinal"].ToInt() - 1;

                string colName = col["name"].ToStr();
                bool colVisible = true;
                int colWidth = colWidths.SplitAndGetItem(',', colIndex).ToInt();
                string colCaption = colCaptions.SplitAndGetItem(',', colIndex);
                if (colWidth < 0)
                {
                    colVisible = false;
                }

                FieldAccordionColumnType colType = ParseEnum<FieldAccordionColumnType>(col["col_type"].ToStr());

                string colFormat = colFormats.SplitAndGetItem('|', colIndex);
                if (colFormat.ToUpper() == "DD/MM/YYYY")
                {
                    colType = FieldAccordionColumnType.date;
                }

                FieldAccordionColumnAlignment colAlign = FieldAccordionColumnAlignment.left;
                if (colAligns.SplitAndGetItem(',', colIndex).ToInt() == 0)
                {
                    colAlign = FieldAccordionColumnAlignment.left;
                }
                else
                {
                    colAlign = FieldAccordionColumnAlignment.right;
                }


                this.Columns.Add(new FieldAccordionColumn()
                {
                    dataField = colName,
                    caption = colCaption,
                    columnType = colType,
                    width = colWidth / 10,
                    visible = colVisible,
                    format = colFormat,
                    ColumnAlignment = colAlign

                });

            }
        }

        private static T ParseEnum<T>(string value) {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public void InitEditRow(SessionState session, JObject data, FieldGridEditRowAction action) {
            this.EditRow = new FieldGridEditRow(this);
            this.EditRow.Action = action;
            this.EditRow.Data = data;

            foreach (var prop in this.EditRow.Data) {
                FieldGridEditCell editCell = new FieldGridEditCell(this.EditRow);
                editCell.DataField = prop.Key;

                // assign cell type => text/date/dropdown/checkbox/picklist
                if (this.AccordionType == FieldAccordionType.Editable && this.NavigateScrnDefinition != null) {
                    EPF_SCRN_FIELDS field = this.NavigateScrnDefinition.FIELDS.Find(x => x.FIELD_ID == prop.Key);
                    this.EditRow.InitCell(session, field, editCell);
                }


                // assign value
                editCell.Value = data[prop.Key];

                // assign default value for add/update mode if current value is empty
                if (string.IsNullOrEmpty(editCell.Value.ToStr())) {
                    if (this.EditRow.Action == FieldGridEditRowAction.eOnInsert) {
                        if (editCell.DefaultValueOnAdd != null) {
                            editCell.Value = editCell.DefaultValueOnAdd;
                            this.EditRow.Data[prop.Key] = JToken.FromObject(editCell.Value);
                        }
                    }
                    else if (this.EditRow.Action == FieldGridEditRowAction.eOnUpdate) {
                        if (editCell.DefaultValueOnUpdate != null) {
                            editCell.Value = editCell.DefaultValueOnUpdate;
                            this.EditRow.Data[prop.Key] = JToken.FromObject(editCell.Value);
                        }
                    }
                }

                // convert checkbox value to 1/0 instead of true/false
                if (editCell.CellType == FieldGridEditCellType.dxCheckBox) {
                    if (editCell.Value != null) {
                        editCell.Value = editCell.Value.ToInt();
                        this.EditRow.Data[prop.Key] = JToken.FromObject(editCell.Value);
                    }
                }

                this.EditRow.Cells.Add(editCell.DataField, editCell);
            }

            this.ParentScreen.Refresh();

        }

        public void RemoveEditRow() {
            if (this.EditRow == null) {
                return;
            }

            foreach (KeyValuePair<string, FieldGridEditCell> cell in this.EditRow.Cells) {
                if (cell.Value.CellType == FieldGridEditCellType.pickList) {
                    Field field = this.ParentScreen.Fields.Search(cell.Value.PickListFieldId);
                    field.ParentContainer.Fields.Remove(field);
                }
            }

            this.EditRow = null;
            this.ParentScreen.Refresh();
        }

    }

    public enum FieldAccordionUserActionType {
        eOnInsert,
        eOnUpdate,
        eOnDelete,
        eOnCancel,
        eOnBeforeInsert,
        eOnBeforeUpdate,
        eOnBeforeDelete,
        eOnAfterInsert,
        eOnAfterUpdate,
        eOnAfterDelete,
        onDownloadClick,
    }


    public enum FieldAccordionType
    {
        Brws,
        Editable,
        MultiSelect,
        SingleSelect,
        Navigate
    }

    public enum FieldAccordionColumnType
    {
        @string,
        number,
        date,
        boolean,
        @object,
        datetime
    }

    public enum FieldAccordionColumnAlignment
    {
        left,
        right
    }


    public class FieldAccordionColumn
    {
        private FieldAccordionColumnType _columnType = FieldAccordionColumnType.@string;
        private FieldAccordionColumnAlignment _columnAlignment = FieldAccordionColumnAlignment.left;
        public string dataField { get; set; } = "";
        public string caption { get; set; } = "";
        public int minWidth { get; set; } = 50;
        public string dataType { get { return _columnType.ToStr(); } }
        public FieldAccordionColumnType columnType { set { _columnType = value; } } // string, number, date, boolean, object, datetime
        public string alignment { get { return _columnAlignment.ToStr(); } }
        public FieldAccordionColumnAlignment ColumnAlignment { set { _columnAlignment = value; } } // left, right
        public string format { get; set; } = "";
        public int width { get; set; } = 200;
        public bool visible { get; set; } = true;


        public bool showLabel { get; set; } = true;
        public bool showInSubTitle { get; set; } = true;
        public bool showInDetail { get; set; } = true;

    }
}
