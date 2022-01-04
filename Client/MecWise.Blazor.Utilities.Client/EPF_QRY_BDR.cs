using System;
using MecWise.Blazor;
using MecWise.Blazor.Common;
using MecWise.Blazor.Components;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Web;
using System.Linq;

namespace MecWise.Blazor.Utilities.Client {
    public class QueryField {
        public string FieldName { get; set; }
        public string FieldDescription { get; set; }
        public string FieldType { get; set; }
    }

    public class EPF_QRY_BDR : Screen {

        string _returnFieldId;
        string _fieldDataId;

        JArray _lstExpressionDataSource;
        JArray _lstExpressionDescDataSource;
        JArray _lstFieldsDataSource;

        #region "Screen Events"

        protected override async Task<object> LoadAsync() {

            _returnFieldId = GetQueryParamValue("RETURN_FIELD_ID");
            _fieldDataId = GetQueryParamValue("FIELD_DATA_ID");

            SetFieldValue("optLogicOperator", "AND");
            SetFieldValue("optOperators", "Equal To");

            string returnDataStr = GetParentFieldValue<string>(_returnFieldId);
            if (string.IsNullOrEmpty(returnDataStr)) {
                _lstExpressionDataSource = new JArray();
                _lstExpressionDescDataSource = new JArray();
            }
            else {
                JObject returnData = JObject.Parse(returnDataStr);
                _lstExpressionDataSource = JArray.FromObject(returnData["ExpressionData"]);
                _lstExpressionDescDataSource = JArray.FromObject(returnData["ExpressionDescData"]);
            }
            
            
            string fieldDataStr = GetParentFieldValue<string>(_fieldDataId);
            if (string.IsNullOrEmpty(fieldDataStr)) {
                _lstFieldsDataSource = new JArray();
            }
            else {
                _lstFieldsDataSource = JArray.Parse(fieldDataStr);
            }
            
            SetFieldListDataSource("lstFields", _lstFieldsDataSource, "FieldName", "FieldDescription");
            SetFieldListDataSource("lstExpression", _lstExpressionDescDataSource, "ID", "TEXT");

            return await base.LoadAsync();
        }

        protected override async Task<object> ShowRecAsync() {
            if (ScrnMode == ScreenMode.Add) {
                await InitFieldsAsync();
            }

            return await Task.FromResult<object>(null);
        }

        protected override async Task<object> SavedRecAsync() {
            return await Task.FromResult<object>(null);
        }

        protected override async Task<bool> ValidEntryAsync() {
            if (ScrnMode == ScreenMode.Add) {
            }
            return await Task.FromResult<bool>(true);
        }

        private async Task<object> InitFieldsAsync() {
            
            return await Task.FromResult<object>(null);
        }

        #endregion



        #region "Contorl Events"

        public void btnOk_OnClick() {
            JObject returnData = new JObject();
            returnData.Add("ExpressionData", _lstExpressionDataSource);
            returnData.Add("ExpressionDescData", _lstExpressionDescDataSource);
            SetParentFieldValue(_returnFieldId, returnData.ToStr());
            this.Close();
        }

        public void btnCancel_OnClick() {
            this.Close();
        }

        public void btnInsert_OnClick() {
            string expLogicOperator = GetFieldValue<string>("optLogicOperator");
            string expFieldId = GetFieldValue<string>("lstFields");
            string expOperator = GetFieldValue<string>("optOperators");
            string expValue = GetFieldValue<string>("txtValue");


            string str = GetFormatString(expLogicOperator, expFieldId, expOperator, expValue);
            if (!string.IsNullOrEmpty(str)) {
                JObject item = new JObject();
                string id = Guid.NewGuid().ToStr();

                item.Add("ID", id);
                item.Add("TEXT", str);
                _lstExpressionDataSource.Add(item);

                item = new JObject();
                item.Add("ID", id);
                item.Add("TEXT", str.Replace(expFieldId, string.Format("[{0}]", GetQuertFieldDescription(expFieldId))));
                _lstExpressionDescDataSource.Add(item);


                SetFieldListDataSource("lstExpression", _lstExpressionDescDataSource, "ID", "TEXT");

            }

        }

        public void btnRemove_OnClick() {
            string selectedId = GetFieldValue<string>("lstExpression");
            JObject itemToRemove = _lstExpressionDataSource.Children<JObject>().FirstOrDefault(x => x["ID"].ToStr() == selectedId);
            
            int index = _lstExpressionDataSource.IndexOf(itemToRemove);
            
            _lstExpressionDataSource[index].Remove();
            _lstExpressionDescDataSource[index].Remove();


            if (_lstExpressionDataSource.Count > 0) {
                string str = RemoveLogicalOperator(_lstExpressionDataSource[0]["TEXT"].ToStr());
                _lstExpressionDataSource[0]["TEXT"] = str;
            }

            if (_lstExpressionDescDataSource.Count > 0) {
                string str = RemoveLogicalOperator(_lstExpressionDescDataSource[0]["TEXT"].ToStr());
                _lstExpressionDescDataSource[0]["TEXT"] = str;
            }

            SetFieldListDataSource("lstExpression", _lstExpressionDescDataSource, "ID", "TEXT");

        }

        public void btnReplace_OnClick() {
            string expLogicOperator = GetFieldValue<string>("optLogicOperator");
            string expFieldId = GetFieldValue<string>("lstFields");
            string expOperator = GetFieldValue<string>("optOperators");
            string expValue = GetFieldValue<string>("txtValue");

            string selectedId = GetFieldValue<string>("lstExpression");
            JObject itemToReplace = _lstExpressionDataSource.Children<JObject>().FirstOrDefault(x => x["ID"].ToStr() == selectedId);
            int indexToReplace = _lstExpressionDataSource.IndexOf(itemToReplace);

            string str = GetFormatString(expLogicOperator, expFieldId, expOperator, expValue, indexToReplace);
            if (!string.IsNullOrEmpty(str)) {
                _lstExpressionDataSource[indexToReplace]["TEXT"] = str;
                _lstExpressionDescDataSource[indexToReplace]["TEXT"] = str.Replace(expFieldId, string.Format("[{0}]", GetQuertFieldDescription(expFieldId)));

                SetFieldListDataSource("lstExpression", _lstExpressionDescDataSource, "ID", "TEXT");

            }
        }

        public void btnAddOpenBracket_OnClick() {
            string selectedId = GetFieldValue<string>("lstExpression");
            JObject itemToAddBracket = _lstExpressionDataSource.Children<JObject>().FirstOrDefault(x => x["ID"].ToStr() == selectedId);
            int indexToAddBracket = _lstExpressionDataSource.IndexOf(itemToAddBracket);

            string str = AddOpenBracket(_lstExpressionDataSource[indexToAddBracket]["TEXT"].ToStr());
            _lstExpressionDataSource[indexToAddBracket]["TEXT"] = str;

            str = AddOpenBracket(_lstExpressionDescDataSource[indexToAddBracket]["TEXT"].ToStr());
            _lstExpressionDescDataSource[indexToAddBracket]["TEXT"] = str;

            SetFieldListDataSource("lstExpression", _lstExpressionDescDataSource, "ID", "TEXT");


        }

        public void btnAddCloseBracket_OnClick() {
            string selectedId = GetFieldValue<string>("lstExpression");
            JObject itemToAddBracket = _lstExpressionDataSource.Children<JObject>().FirstOrDefault(x => x["ID"].ToStr() == selectedId);
            int indexToAddBracket = _lstExpressionDataSource.IndexOf(itemToAddBracket);

            string str = AddCloseBracket(_lstExpressionDataSource[indexToAddBracket]["TEXT"].ToStr());
            _lstExpressionDataSource[indexToAddBracket]["TEXT"] = str;

            str = AddCloseBracket(_lstExpressionDescDataSource[indexToAddBracket]["TEXT"].ToStr());
            _lstExpressionDescDataSource[indexToAddBracket]["TEXT"] = str;

            SetFieldListDataSource("lstExpression", _lstExpressionDescDataSource, "ID", "TEXT");

        }

        public void btnRemoveOpenBracket_OnClick() {
            string selectedId = GetFieldValue<string>("lstExpression");
            JObject itemToAddBracket = _lstExpressionDataSource.Children<JObject>().FirstOrDefault(x => x["ID"].ToStr() == selectedId);
            int indexToAddBracket = _lstExpressionDataSource.IndexOf(itemToAddBracket);

            string str = RemoveOpenBracket(_lstExpressionDataSource[indexToAddBracket]["TEXT"].ToStr());
            _lstExpressionDataSource[indexToAddBracket]["TEXT"] = str;

            str = RemoveOpenBracket(_lstExpressionDescDataSource[indexToAddBracket]["TEXT"].ToStr());
            _lstExpressionDescDataSource[indexToAddBracket]["TEXT"] = str;

            SetFieldListDataSource("lstExpression", _lstExpressionDescDataSource, "ID", "TEXT");

        }


        public void btnRemoveCloseBracket_OnClick() {
            string selectedId = GetFieldValue<string>("lstExpression");
            JObject itemToAddBracket = _lstExpressionDataSource.Children<JObject>().FirstOrDefault(x => x["ID"].ToStr() == selectedId);
            int indexToAddBracket = _lstExpressionDataSource.IndexOf(itemToAddBracket);

            string str = RemoveCloseBracket(_lstExpressionDataSource[indexToAddBracket]["TEXT"].ToStr());
            _lstExpressionDataSource[indexToAddBracket]["TEXT"] = str;

            str = RemoveCloseBracket(_lstExpressionDescDataSource[indexToAddBracket]["TEXT"].ToStr());
            _lstExpressionDescDataSource[indexToAddBracket]["TEXT"] = str;

            SetFieldListDataSource("lstExpression", _lstExpressionDescDataSource, "ID", "TEXT");


        }

        #endregion




        #region "Helper Functions"
        private string RemoveLogicalOperator(string str) {

            if (str.StartsWith("AND NOT")) {
                str = str.Replace("AND NOT", "");
            }
            else if (str.StartsWith("OR NOT")) {
                str = str.Replace("OR NOT", "");
            }
            else if (str.StartsWith("AND")) {
                str = str.Replace("AND", "");
            }
            else if (str.StartsWith("OR")) {
                str = str.Replace("OR", "");
            }

            str = str.Trim();

            return str;
        }

        private string RemoveCloseBracket(string str) {

            if (!str.EndsWith(" )")) {
                return str;
            }

            str = str.Substring(0, str.Length - 2);

            return str;
        }

        private string RemoveOpenBracket(string str) {

            if (str.StartsWith("AND NOT (")) {
                str = str.Replace("AND NOT (", "AND NOT");
            }
            else if (str.StartsWith("OR NOT (")) {
                str = str.Replace("OR NOT (", "OR NOT");
            }
            else if (str.StartsWith("AND (")) {
                str = str.Replace("AND (", "AND");
            }
            else if (str.StartsWith("OR (")) {
                str = str.Replace("OR (", "OR");
            }
            else if (str.StartsWith("( ")) {
                str = str.Replace("( ", "");
            }
           
            return str;
        }

        private string AddCloseBracket(string str) {

            if (str.EndsWith(" )")) {
                return str;
            }

            str = string.Format("{0} )", str);

            return str;
        }

        private string AddOpenBracket(string str) {

            if (str.StartsWith("AND NOT (")
                || str.StartsWith("OR NOT (")
                || str.StartsWith("AND (")
                || str.StartsWith("OR (")
                || str.StartsWith("(")) {
                return str;
            }

            if (str.StartsWith("AND NOT")) {
                str = str.Replace("AND NOT", "AND NOT (");
            }
            else if (str.StartsWith("OR NOT")) {
                str = str.Replace("OR NOT", "OR NOT (");
            }
            else if (str.StartsWith("AND")) {
                str = str.Replace("AND", "AND (");
            }
            else if (str.StartsWith("OR")) {
                str = str.Replace("OR", "OR (");
            }
            else {
                str = string.Format("({0}", str);
            }

            return str;
        }

        private string GetQuertFieldDescription(string fieldId) {
            JObject item = _lstFieldsDataSource.Children<JObject>().FirstOrDefault(x => x["FieldName"].ToStr() == fieldId);
            return item["FieldDescription"].ToStr();
        }

        private string GetFormatString(string expLogicOperator, string expFieldId, string expOperator, string expValue, int itemIndex = -1) {
            
            if (string.IsNullOrEmpty(expFieldId)) {
                return "";
            }

            QueryField field = _lstFieldsDataSource.Children<JObject>().FirstOrDefault(x => x["FieldName"].ToStr() == expFieldId).ToObject<QueryField>();
            
            if (expOperator == "Like") {
                if (string.IsNullOrEmpty(expValue)) {
                    expValue = "N'%'";
                }
                else {
                    if (field.FieldType == "D") {
                        if (expValue.IsDate("dd/MM/yyyy") || expValue.IsDate("MM/dd/yyyy") || expValue.IsDate("dd/MMM/yyyy") || expValue.IsDate("dd-MMM-yyyy")) {
                            expValue = expValue.ToDate().ToString("dd/MM/yyyy");
                            expValue = string.Format("N'{0}'", expValue);
                        }
                        else {
                            return "";
                        }
                    }
                    else if (field.FieldType == "C") {
                        expValue = string.Format("N'{0}%'", expValue);
                    }
                    else {
                        expValue = string.Format("N'{0}'", expValue);
                    }
                }
            }
            else if (expOperator == "Is NULL") {
                expValue = "";
            }
            else if (expOperator == "Is Not NULL") {
                expValue = "";
            }
            else if (expOperator == "In") {
                if (string.IsNullOrEmpty(expValue)) {
                    expValue = "()";
                }
                else {
                    if (field.FieldType == "D") {
                        if (expValue.IsDate("dd/MM/yyyy") || expValue.IsDate("MM/dd/yyyy") || expValue.IsDate("dd/MMM/yyyy") || expValue.IsDate("dd-MMM-yyyy")) { 
                            expValue = expValue.ToDate().ToString("dd/MM/yyyy");
                        }
                        else {
                            return "";
                        }
                    }

                    string temp = "";
                    foreach (string item in expValue.Split(',')) {
                        temp = temp + (temp == "" ? "" : ",");
                        if (field.FieldType == "N") {
                            temp = temp + string.Format("{0}", item);
                        }
                        else {
                            temp = temp + string.Format("'{0}'", item);
                        }
                    }
                    expValue = String.Format("({0})", temp);
                }
            }
            else if (expOperator == "Not In") {
                if (string.IsNullOrEmpty(expValue)) {
                    expValue = "()";
                }
                else {
                    if (field.FieldType == "D") {
                        if (expValue.IsDate("dd/MM/yyyy") || expValue.IsDate("MM/dd/yyyy") || expValue.IsDate("dd/MMM/yyyy") || expValue.IsDate("dd-MMM-yyyy")) {
                            expValue = expValue.ToDate().ToString("dd/MM/yyyy");
                        }
                        else {
                            return "";
                        }
                    }

                    string temp = "";
                    foreach (string item in expValue.Split(',')) {
                        temp = temp + (temp == "" ? "" : ",");
                        if (field.FieldType == "N") {
                            temp = temp + string.Format("{0}", item);
                        }
                        else {
                            temp = temp + string.Format("'{0}'", item);
                        }
                    }
                    expValue = String.Format("({0})", temp);
                }
            }
            else {
                if (string.IsNullOrEmpty(expValue)) {
                    expValue = "NULL";
                }
                else {
                    if (field.FieldType == "N") {
                        expValue = string.Format("{0}", expValue);
                    }
                    else {
                        if (field.FieldType == "D") {
                            if (expValue.IsDate("dd/MM/yyyy") || expValue.IsDate("MM/dd/yyyy") || expValue.IsDate("dd/MMM/yyyy") || expValue.IsDate("dd-MMM-yyyy")) {
                                expValue = expValue.ToDate().ToString("dd/MM/yyyy");
                            }
                            else {
                                return "";
                            }
                        }
                        expValue = string.Format("N'{0}'", expValue);
                    }
                }
            }            

            if (_lstExpressionDataSource.Count == 0 || itemIndex == 0) {
                expLogicOperator = "";
            }

            string str = string.Format("{0} {1} {2} {3}", expLogicOperator, expFieldId, GetOperatorSymbol(expOperator), expValue);
            return str;
        }

        private string GetOperatorSymbol(string expOperator) {
            switch (expOperator) {
                case "Equal To":
                    return "=";
                case "Not Equal To":
                    return "<>";
                case "Greater Than":
                    return ">";
                case "Greater Than Equal To":
                    return ">=";
                case "Less Than":
                    return "<";
                case "Less Than Equal To":
                    return "<=";
                case "Like":
                    return "Like";
                case "Is NULL":
                    return "IS NULL";
                case "Is Not NULL":
                    return "IS NOT NULL";
                case "In":
                    return "IN";
                case "Not In":
                    return "NOT IN";
            }

            return "";
        }


        #endregion

    }
}
