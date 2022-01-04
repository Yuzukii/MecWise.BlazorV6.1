var _formFields = {};
var _activeScreenId = "";

class formField {
    constructor(id, callBack, inst) {
        try {
            this.fieldId = id;
            this.instance = inst;
            this.eventCallBackMethod = callBack;
            this.shouldRender = true;

            return true;
        } catch (err) {
            console.log(err);
            return false;
        }
    }

}

window.controlEventHandler = (fieldId, callBack, inst) => {
    _formFields[fieldId] = new formField(fieldId, callBack, inst);
    _formFields[fieldId].shouldRender = true;
};

function _setActiveScreenId(scrnId) {
    _activeScreenId = scrnId;
}

async function _load_page(pageUrl) {
    var result;
    await $.ajax({
        async: true,
        type: "GET",
        url: pageUrl,
        success: function (data, textStatus, jqXHR) {
            if (textStatus === "success") {
                result = data;
            }
        },
        error: function () {
            result = "Error while loading page.";
        }
    });
    return result;
}

function _updateQueryStringParameter(uri, key, value) {
    var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
    var separator = uri.indexOf('?') !== -1 ? "&" : "?";
    if (uri.match(re)) {
        return uri.replace(re, '$1' + key + "=" + value + '$2');
    }
    else {
        return uri + separator + key + "=" + value;
    }
}

function _change_url(url) {
    window.history.pushState("", "", url);
}

async function ExecCSharpAsync(scrnId, funcName, params) {
    return await _formFields[scrnId].instance.invokeMethodAsync(funcName, params);
}

function ExecCSharp(scrnId, funcName, params) {
    return _formFields[scrnId].instance.invokeMethod(funcName, params);
}

function _setSelectValue(id, value) {
    $("#" + getFieldId(id)).val(value);
}

function _setRadioValue(fieldId, value) {
    $('input:radio[name="' + getFieldId(fieldId) + '"][value="' + value + '"]').prop('checked', true);
}

function _closeBoostrapModel(id) {
    $('#' + getFieldId(id)).modal('hide');
    _resizePopupIframe();
}

function _resizePopupForPickList(pickListId) {
    var iframe = window.parent.document.getElementById("popupframe");
    if (iframe) {
        popupScreenHeight = iframe.contentWindow.document.body.scrollHeight;
        picklistHeight = document.getElementById(getFieldId(pickListId) + "_PickListBrws").scrollHeight;
        if (picklistHeight > popupScreenHeight) {
            $(iframe).css("height", picklistHeight + "px");
        }
    }
}

function _epfConnect_hideV5NavBar() {
    var iframe = window.parent.document.getElementById("_epfConnectIframe");
    if (iframe) {
        if (iframe.contentWindow.document.body) {
            if (iframe.contentWindow.$) {
                $(iframe).hide();
                iframe.contentWindow.$("#_ctl0_WebBanner").each(function () {
                    $(this).hide();
                });

                iframe.contentWindow.$(".content-wrapper").each(function () {
                    $(this).css("padding-top", "0px");
                });
                $(iframe).show();
            }
        }
    }
}

function _resizePopupIframe(freeze) {
    try {
        var iframe = window.parent.document.getElementById("popupframe");
        if (iframe) {
            if (freeze) {
                $(iframe).css("height", "80vh");
            }
            else {
                if (iframe.contentWindow.document.body) {

                    // check for height of parent popups 
                    var doc = iframe.ownerDocument;
                    var parentWindow = doc.defaultView || doc.parentWindow;
                    if (parentWindow.top != parentWindow.self) {
                        parentWindow._resizePopupIframe(true);
                    }

                    // set current popup height
                    // initialize back to "200px" to appear scroll and get its height
                    $(iframe).css("height", "200px");
                    var appBodyContainer = iframe.contentWindow.document.getElementById("appBodyContainer");
                    if (appBodyContainer) {
                        height = appBodyContainer.scrollHeight;
                        $(iframe).css("height", height + "px");
                    }

                }
            }
        }
    } catch (e) {

    }
    
}

function _stopAllVideoStream() {
    if (_stopVideoStream) {
        _stopVideoStream();
    }

    var iframe = window.parent.document.getElementById("popupframe");
    if (iframe) {
        if (iframe.contentWindow._stopVideoStream) {
            iframe.contentWindow._stopVideoStream();
        }
    }
}

function _hidePopupScreen(popupId) {
    var btnPopupClose = window.parent.document.getElementById(getFieldId(popupId) + "_btnPopupClose");
    $(btnPopupClose).click();
}

function _showPopupScreen(scrnId, title, width) {
    $('#' + scrnId).modal({
        backdrop: 'static',
        keyboard: false
    });

    _resizePopupIframe(200);


    $("#_popupScreen").find(".modal-title").text(title);

    if (width > 0) {
        $("#_popupScreen").find(".modal-dialog").css("max-width", width);
    }
    else {
        $("#_popupScreen").find(".modal-dialog").css("max-width", "");
    }

    $('#' + scrnId).modal('show');
}

function _picklist_keydown(evt, type, maxLength) {

    // ignore delete and backspace
    if (evt.keyCode == 8 || evt.keyCode == 9 || evt.keyCode == 46 || evt.keyCode == 37 || evt.keyCode == 39) {
        return;
    }

    if (type == "number") {
        _numbers_only(evt);
    }

    if (evt.currentTarget.value.length >= maxLength && maxLength != 0) {
        var theEvent = evt || window.event;
        theEvent.returnValue = false;
        if (theEvent.preventDefault) theEvent.preventDefault();
    }

}

function _numbers_only(evt) {
    var theEvent = evt || window.event;

    // ignore delete and backspace
    if (evt.keyCode == 8 || evt.keyCode == 9 || evt.keyCode == 46) {
        return;
    }

    // Handle paste
    if (theEvent.type === 'paste') {
        key = event.clipboardData.getData('text/plain');
    } else {
        // Handle key press
        var key = theEvent.keyCode || theEvent.which;
        key = String.fromCharCode(key);
    }
    var regex = /[0-9]|\./;
    if (!regex.test(key)) {
        theEvent.returnValue = false;
        if (theEvent.preventDefault) theEvent.preventDefault();
    }
}

function _hideMultiPick(fieldId) {
    $('#' + getFieldId(fieldId)).modal('hide');
}
function _showMultiPick(fieldId) {
    $('#' + getFieldId(fieldId)).modal('show');
}

function _getMultiPickSelectedRows(fieldId) {
    var field = $("#" + getFieldId(fieldId)).data("dxDataGrid");
    if (field) {
        return JSON.stringify(field.option("selectedRowKeys"));
    }
    return "[]";
}

function _clearMultiPickSelectedRows(fieldId) {
    var field = $("#" + getFieldId(fieldId)).data("dxDataGrid");
    if (field) {
        field.clearSelection();
    }
}

function _navigatePopupTo(url) {
    var popupframe = document.getElementById('popupframe');
    if (popupframe) {
        var contentWindow = popupframe.contentWindow;
        if (contentWindow) {
            try {
                if (contentWindow._navigateTo) {
                    contentWindow._navigateTo(url);
                }
                else {
                    popupframe.src = url;
                }
            }
            catch (err) {
                console.log("_navigatePopupTo error: " + err.message);
                contentWindow.location = document.getElementById("baseUrl").href + "Blank";
            }

        }
    }


}

function _navigateTo(url) {
    try {
        var link = document.createElement('a');
        link.href = url;
        document.body.appendChild(link);
        link.click();
    }
    catch (err) {
        console.log("_navigateTo error: " + err.message);
    }
}

async function GetParentFieldValueAsync(fieldId, scrnId) {
    if (!scrnId) {
        scrnId = _activeScreenId;
    }

    var eventData = {
        "FieldId": fieldId,
        "EventType": "OnGetParentFieldValue"
    };
    var jsonEventData = JSON.stringify(eventData);
    if (scrnId) {
        return await _formFields[scrnId].instance.invokeMethodAsync(_formFields[scrnId].eventCallBackMethod, jsonEventData);
    }
}

async function SetParentFieldValueAsync(fieldId, value, scrnId) {
    if (!scrnId) {
        scrnId = _activeScreenId;
    }

    var eventData = {
        "FieldId": fieldId,
        "Value": value,
        "EventType": "OnSetParentFieldValue"
    };
    var jsonEventData = JSON.stringify(eventData);
    if (scrnId) {
        await _formFields[scrnId].instance.invokeMethodAsync(_formFields[scrnId].eventCallBackMethod, jsonEventData);
    }
}

async function GetFieldValueAsync(fieldId, scrnId) {
    fieldId = _getFieldId(fieldId);

    if (!scrnId) {
        scrnId = _activeScreenId;
    }

    var eventData = {
        "FieldId": fieldId,
        "EventType": "OnGetFieldValue"
    };
    var jsonEventData = JSON.stringify(eventData);
    if (scrnId) {
        return await _formFields[scrnId].instance.invokeMethodAsync(_formFields[scrnId].eventCallBackMethod, jsonEventData);
    }
}


async function SetFieldValueAsync(fieldId, value, scrnId) {
    fieldId = _getFieldId(fieldId);

    if (!scrnId) {
        scrnId = _activeScreenId;
    }

    var eventData = {
        "FieldId": fieldId,
        "Value": value,
        "EventType": "OnSetFieldValue"
    };
    var jsonEventData = JSON.stringify(eventData);
    if (scrnId) {
        await _formFields[scrnId].instance.invokeMethodAsync(_formFields[scrnId].eventCallBackMethod, jsonEventData);
    }
}


async function SetFieldDisable(fieldId, disable, scrnId) {
    if (!scrnId) {
        scrnId = _activeScreenId;
    }

    var eventData = {
        "FieldId": fieldId,
        "Value": disable,
        "EventType": "OnSetFieldDisable"
    };
    var jsonEventData = JSON.stringify(eventData);
    if (scrnId) {
        await _formFields[scrnId].instance.invokeMethodAsync(_formFields[scrnId].eventCallBackMethod, jsonEventData);
        await Refresh(scrnId)
    }
}


async function SetFieldFocusAsync(fieldId, scrnId) {
    if (!scrnId) {
        scrnId = _activeScreenId;
    }

    var eventData = {
        "FieldId": fieldId,
        "EventType": "OnSetFieldFocus"
    };
    var jsonEventData = JSON.stringify(eventData);
    if (scrnId) {
        await _formFields[scrnId].instance.invokeMethodAsync(_formFields[scrnId].eventCallBackMethod, jsonEventData);
    }
}


async function RefreshAsync(scrnId) {
    if (!scrnId) {
        scrnId = _activeScreenId;
    }

    var eventData = {
        "EventType": "OnRefresh"
    };
    var jsonEventData = JSON.stringify(eventData);
    if (scrnId) {
        await _formFields[scrnId].instance.invokeMethodAsync(_formFields[scrnId].eventCallBackMethod, jsonEventData);
    }
}


async function OpenPopupScreenAsync(srcScrnId, scrnId, mode, targetKeys, parentKeys, param) {
    var eventData = {
        "FieldId": null,
        "Value": {
            "screenId": scrnId,
            "mode": mode,
            "targetKeys": targetKeys,
            "parentKeys": parentKeys,
            "parameters": param
        },
        "PreviousValue": null,
        "EventType": "OnOpenPopupScreen"
    };
    var jsonEventData = JSON.stringify(eventData);
    if (srcScrnId) {
        await _formFields[srcScrnId].instance.invokeMethodAsync(_formFields[srcScrnId].eventCallBackMethod, jsonEventData);
    }
    else {
        await _formFields['_epfScreen'].instance.invokeMethodAsync(_formFields['_epfScreen'].eventCallBackMethod, jsonEventData);
    }
}



function _clear_fields() {
    _formFields = {};
}

function _clear_focus() {
    var element = document.activeElement;
    element.blur();
}

function _set_field_focus(fieldId) {
    var element = document.getElementById(getFieldId(fieldId));
    if (element) {
        element.focus();
    }
    var fieldType = $("#" + getFieldId(fieldId)).attr("field-type");
    if (fieldType) {
        var field = $("#" + getFieldId(fieldId)).data(fieldType);
        if (field) {
            field.focus();
        }
    }
}

function _refreshAllGridInTabPage(tabPageId) {
    setTimeout(function () {
        $("#" + getFieldId(tabPageId) + " [field-type='dxDataGrid']").each(function (index, item) {
            $("#" + item.id).data("dxDataGrid").refresh();
            $("#" + item.id).data("dxDataGrid").repaint();
        });
    }, 100);
}

async function _assign_field_prop(id, jsonFieldProp) {
    var field;
    var fieldProp;
    var fieldType;

    if (!jsonFieldProp) {
        return;
    }

    fieldProp = JSON.parse(jsonFieldProp);
    if (Object.keys(fieldProp).length == 0) {
        return;
    }

    fieldType = fieldProp["fieldType"];
    fieldElement = $("#" + id);
    field = $("#" + id).data(fieldType);

    if (field) {




        // Set Other DevExpress options
        if (fieldType === "dxTextBox") {
            field.option("placeholder", fieldProp["placeholder"]);
            field.option("mode", fieldProp["mode"]);
            field.option("buttons", fieldProp["buttons"]);
            field.option("maxLength", fieldProp["maxLength"]);
            field.option("mask", fieldProp["mask"]);
            field.option("useMaskedValue", true);
            field.option("showMaskMode", "onFocus");
        }
        else if (fieldType === "dxDateBox") {
            field.option("placeholder", fieldProp["placeholder"]);
            field.option("type", fieldProp["type"]);
            field.option("displayFormat", fieldProp["displayFormat"]);
            field.option("useMaskBehavior", fieldProp["useMaskBehavior"]);
            field.option("min", fieldProp["min"]);
            field.option("max", fieldProp["max"]);
            if (fieldProp["type"] == "time") {
                field.option("pickerType", "rollers");
            }
        }
        else if (fieldType === "dxTextArea") {
            field.option("placeholder", fieldProp["placeholder"]);
            field.option("height", fieldProp["height"]);
            field.option("mask", fieldProp["mask"]);
            field.option("useMaskedValue", true);
            field.option("showMaskMode", "onFocus");
        }
        else if (fieldType === "dxSelectBox") {
            var items = fieldProp["items"];
            var dataSource = new DevExpress.data.ArrayStore({
                data: items,
                key: "Value"
            });
            field.option("placeholder", fieldProp["placeholder"]);
            field.option("displayExpr", fieldProp["displayExpr"]);
            field.option("valueExpr", fieldProp["valueExpr"]);
            field.option("dataSource", dataSource);
            field.option("acceptCustomValue", fieldProp["acceptCustomValue"]);
            field.option("searchEnabled", fieldProp["searchEnabled"]);
        }
        else if (fieldType === "dxList") {
            field.option("focusStateEnabled", false);
            field.option("selectionMode", fieldProp["selectionMode"]);
            field.option("displayExpr", fieldProp["displayExpr"]);
            field.option("valueExpr", fieldProp["valueExpr"]);
            if (JSON.stringify(field.option("dataSource")) != JSON.stringify(fieldProp["items"])) {
                field.option("dataSource", fieldProp["items"]);
            }
            field.option("height", fieldProp["height"]);
            field.option("readOnly", fieldProp["disabled"]);
            if (fieldProp["disabled"]) {
                field.option("activeStateEnabled", false);
                field.option("hoverStateEnabled", false)
                field.option("selectionMode", "none")
            }
            else {
                field.option("activeStateEnabled", true);
                field.option("hoverStateEnabled", true)
                field.option("selectionMode", "single")
            }
        }
        else if (fieldType === "dxNumberBox") {
            field.option("placeholder", fieldProp["placeholder"]);
            field.option("type", fieldProp["type"]);
            field.option("format", fieldProp["format"]);
            field.option("mode", "number");
            var maxLength = fieldProp["maxLength"];
            field.option("onInput", function (e) {
                var inputElement = e.jQueryEvent.target;
                if (inputElement.value.length > maxLength && maxLength != 0)
                    inputElement.value = inputElement.value.slice(0, maxLength);
            });
        }
        else if (fieldType === "dxCheckBox") {
            field.option("text", fieldProp["text"]);
            // change hyperlink text as link
            $("#" + id).find(".dx-checkbox-text").each(function () {
                if ($($.parseHTML($(this).text())).text() != $(this).text()) {
                    $(this).html($(this).text());

                    // prevent checkbox being checked when clicking the link
                    $(this).find("a").each(function () {
                        $(this).click(function (event) {
                            event.stopPropagation();
                        });
                    });
                }
            });

        }
        else if (fieldType === "dxAccordion") {
            field.beginUpdate();
            field.option("dataSource", fieldProp["dataSource"]);
            field.option("columns", fieldProp["columns"]);
            field.option("animationDuration", fieldProp["animationDuration"]);
            field.option("collapsible", fieldProp["collapsible"]);
            field.option("multiple", fieldProp["multiple"]);
            field.option("accordionType", fieldProp["accordionType"]);
            field.option("buttons", fieldProp["buttons"]);
            field.option("deleteRecord", fieldProp["deleteRecord"]);
            field.option("showDetail", fieldProp["showDetail"]);
            field.option("screenId", fieldProp["screenId"]);
            field.endUpdate();
            $(".dx-accordion-item").removeAttr("style");

        }
        else if (fieldType === "dxDataGrid") {


            field.beginUpdate();

            field.option("selectTargetField", fieldProp["selectTargetField"]);
            field.option("selectSourceKeys", fieldProp["selectSourceKeys"]);

            var columns = fieldProp["columns"];

            fieldProp["buttons"].forEach(function (button, index) {
                if (button === "download") {
                    fieldProp["buttons"][index] = {
                        hint: "Download",
                        icon: "download",
                        visible: function (e) {
                            return true;
                        },
                        onClick: function (e) {
                            var screenId = fieldProp["screenId"];
                            var elementId = e.element.attr("id");
                            var eventData = {
                                "FieldId": _getFieldId(elementId),
                                "Value": e.row.data,
                                "PreviousValue": "",
                                "EventType": "onDownloadClick"
                            };
                            var jsonEventData = JSON.stringify(eventData);
                            ExecCSharpAsync(screenId, "gridView_OnEditGridActionAsync", jsonEventData);
                        }
                    };
                }
            });

            if (fieldProp["buttons"].length > 0) {
                columns.unshift({
                    type: "buttons",
                    buttons: fieldProp["buttons"]
                });
            }

            // set settting columns
            field.option("columns", columns);

            // set cell template for image columns
            field.option("columns").forEach(function (column, index) {
                var baseUrl = document.getElementById("baseUrl").href;
                if (column.format === "IMAGE") {
                    column.cellTemplate = function (container, options) {
                        $("<div style='text-align:center'>")
                            .append($("<img>", {
                                "src": baseUrl + options.value,
                                "style": "width:100%; object-fit:contain;",
                                "loading": "lazy"
                            }))
                            .appendTo(container);
                    };
                }
            });

            // set button event for button columns
            field.option("columns").forEach(function (column, index) {
                if (column.type === "buttons") {
                    var colInfo = $.grep(fieldProp["columns"], function (col) {
                        return (col.dataField === column.dataField);
                    });
                    var temp = eval(colInfo[0].buttons[0].templateFunction);
                    column.buttons[0].template = temp;
                }
            });

            // extra column to fill up grid width
            field.addColumn({
                dataField: ""
            });

            field.repaint();


            field.option("screenId", fieldProp["screenId"]);
            field.option("editRowOnClick", fieldProp["editRowOnClick"]);
            field.option("showCommandButtons", fieldProp["showCommandButtons"]);


            field.option("paging", fieldProp["paging"]);
            field.option("selection", {
                allowSelectAll: true,
                selectAllMode: "allPages",
                mode: fieldProp["selectionMode"],
                showCheckBoxesMode: "always"
            });

            field.option("editing", {
                mode: "row",
                allowUpdating: fieldProp["allowUpdating"],
                allowDeleting: fieldProp["allowDeleting"],
                allowAdding: fieldProp["allowAdding"],
                confirmDelete: false,
                useIcons: true
            });

            field.option("scrolling", {
                mode: fieldProp["scrolling"],
                rowRenderingMode: "virtual"
            });


            if (!fieldProp["loadData"]) {
                if (!fieldProp["serverSidePaging"]) {
                    field.option("dataSource", []);
                    setTimeout(function () {
                        field.beginCustomLoading();
                    }, 50);
                }
            }

            if (fieldProp["loadData"]) {
                if (!fieldProp["serverSidePaging"]) {

                    field.endCustomLoading();
                    field.option("dataSource", fieldProp["dataSource"]);

                    //adjust grid height according to page sise for freezing column header
                    if (fieldProp["scrolling"] == "virtual") {
                        if (fieldProp["dataSource"]) {
                            var pageSize = fieldProp["paging"]["pageSize"];
                            var rowCount = fieldProp["dataSource"].length;
                            if (rowCount > pageSize) {
                                var gridHeaderHeight = 100;
                                var rowHeight = 32.2;
                                var gridHeight = gridHeaderHeight + (rowHeight * pageSize);
                                field.option("height", gridHeight);
                            }
                        }
                    }

                    //var sessionKey = GetSessionKey();
                    //var session = GetLocalStorageItem(sessionKey);
                    //session = JSON.parse(session);

                    //var store = new DevExpress.data.CustomStore({
                    //    loadMode: "raw",
                    //    load: function (loadOptions) {
                    //        //field.beginCustomLoading(" ");

                    //        var deferred = $.Deferred();
                    //        var postData = {
                    //            brwsId: fieldProp["brwsId"],
                    //            //session: fieldProp["session"],
                    //            dataSource: fieldProp["screenDataSource"],
                    //            baseView: fieldProp["baseView"]
                    //        };

                    //        var sessionForHeader = {
                    //            UserId: session.UserID,
                    //            CompCode: session.CompCode,
                    //            EmpeId: session.EmpeID,
                    //            LangId: session.LangId
                    //        };

                    //        sessionForHeader = btoa(JSON.stringify(sessionForHeader));

                    //        $.ajax({
                    //            type: "POST",
                    //            headers: {
                    //                "Authorization": session.AccessToken.token_type + " " + session.AccessToken.access_token,
                    //                "session": sessionForHeader
                    //            },
                    //            url: session.AppConfig.apiUrl + "/EpfScreen/GetBrwsData",
                    //            dataType: "json",
                    //            data: JSON.stringify(postData),
                    //            success: function (result) {
                    //                if (result.rows) {
                    //                    deferred.resolve(result.rows);
                    //                }
                    //                else {
                    //                    deferred.resolve([]);
                    //                }

                    //                // adjust grid height according to page sise for freezing column header
                    //                if (fieldProp["scrolling"] == "virtual") {
                    //                    if (result.rows) {
                    //                        var pageSize = fieldProp["paging"]["pageSize"];
                    //                        var rowCount = result.rows.length;
                    //                        if (rowCount > pageSize) {
                    //                            var gridHeaderHeight = 100;
                    //                            var rowHeight = 32.2;
                    //                            var gridHeight = gridHeaderHeight + (rowHeight * pageSize);
                    //                            field.option("height", gridHeight);
                    //                        }
                    //                    }
                    //                }

                    //                // resize the popup height after data loaded to the grid 
                    //                _resizePopupIframe();
                    //                //field.endCustomLoading();
                    //            },
                    //            error: function () {
                    //                deferred.reject("Data Loading Error");
                    //                //field.endCustomLoading();
                    //            },
                    //            //timeout: 5000
                    //        });

                    //        return deferred.promise();
                    //    },
                    //    insert: function (values) { },
                    //    update: function (key, values) { },
                    //    remove: function (key) { }
                    //});
                    //field.option("dataSource", store);
                }
                else {
                    field.endCustomLoading();
                    //field.option("remoteOperations", true);

                    // hide groupPanel since it is too complicated for server operations
                    field.option("groupPanel", {
                        visible: false
                    });

                    var sessionKey = GetSessionKey();
                    var session = GetLocalStorageItem(sessionKey);
                    session = JSON.parse(session);

                    var sessionHeader = {
                        "UserId": session.UserID,
                        "CompCode": session.CompCode,
                        "EmpeId": session.EmpeID,
                        "LangId": session.LangId
                    }

                    sessionHeader = window.btoa(JSON.stringify(sessionHeader));

                    var store = new DevExpress.data.CustomStore({
                        load: function (loadOptions) {
                            var deferred = $.Deferred();
                            var args = {};
                            var postData = {
                                brwsId: fieldProp["brwsId"],
                                session: fieldProp["session"],
                                dataSource: fieldProp["screenDataSource"],
                                baseView: fieldProp["baseView"]
                            };

                            [
                                "filter",
                                "group",
                                "groupSummary",
                                "parentIds",
                                "requireGroupCount",
                                "requireTotalCount",
                                "searchExpr",
                                "searchOperation",
                                "searchValue",
                                "select",
                                "sort",
                                "skip",
                                "take",
                                "totalSummary",
                                "userData"
                            ].forEach(function (i) {
                                if (i in loadOptions && isNotEmpty(loadOptions[i]))
                                    args[i] = JSON.stringify(loadOptions[i]);
                            });

                            postData.args = args;

                            $.ajax({
                                type: "POST",
                                headers: {
                                    "Authorization": session.AccessToken.token_type + " " + session.AccessToken.access_token,
                                    "Session": sessionHeader
                                },
                                url: session.AppConfig.apiUrl + "/EpfScreen/GetBrwsDataServerPaging",
                                dataType: "json",
                                data: JSON.stringify(postData),
                                success: function (result) {

                                    deferred.resolve(result.data, {
                                        totalCount: result.totalCount,
                                        summary: result.summary,
                                        groupCount: result.groupCount
                                    });

                                    field.endCustomLoading();

                                    // adjust grid height according to page sise for freezing column header
                                    if (fieldProp["scrolling"] == "virtual") {
                                        if (result.data) {
                                            var pageSize = fieldProp["paging"]["pageSize"];
                                            var rowCount = result.data.length;
                                            if (rowCount >= pageSize) {
                                                var gridHeaderHeight = 100;
                                                var rowHeight = 32.2;
                                                var gridHeight = gridHeaderHeight + (rowHeight * pageSize);
                                                field.option("height", gridHeight);
                                            }
                                        }
                                    }

                                    // resize the popup height after data loaded to the grid 
                                    _resizePopupIframe();
                                },
                                error: function () {
                                    deferred.reject("Data Loading Error");
                                },
                                //timeout: 5000
                            });

                            return deferred.promise();
                        },
                        insert: function (values) { },
                        update: function (key, values) { },
                        remove: function (key) { }
                    });
                    field.option("dataSource", store);
                    field.option("remoteOperations", true);
                }
            }


            // adjust grid height according to page sise for freezing column header
            if (fieldProp["scrolling"] == "virtual") {
                if (field.option("dataSource")) {
                    var pageSize = fieldProp["paging"]["pageSize"];
                    var rowCount = field.option("dataSource").length;
                    if (rowCount > pageSize) {
                        var gridHeaderHeight = 100;
                        var rowHeight = 32.2;
                        var gridHeight = gridHeaderHeight + (rowHeight * pageSize);
                        field.option("height", gridHeight);
                    }
                }
            }


            field.endUpdate();
            field.pageIndex(0);

            setTimeout(function () {
                field.repaint();
            }, 500);

            // resize the popup height after data loaded to the grid 
            _resizePopupIframe();

        }



        // Set Values 
        if (fieldType === "dxDateBox") {
            if (fieldProp["value"] === "") {
                field.option("value", null);
            }
            else {
                field.option("value", fieldProp["value"]);
            }
        }
        else if (fieldType === "dxSelectBox") {
            //var index = fieldProp["items"].findIndex(x => x.ID == fieldProp["value"]);
            field.option("value", fieldProp["value"] + "");
        }
        else if (fieldType === "dxList") {
            var index = fieldProp["items"].findIndex(x => x.ID == fieldProp["value"]);
            field.selectItem(index);
        }
        else {
            // [20/09/2021, phyozin] use try/catch because there is a error in devexpress lib when setting invalid value to masked textbox
            try {
                field.option("value", fieldProp["value"]);
            } catch (e) {
                console.log("_assign_field_prop: " + id + ": " + e.message);
            }
        }

        // for Code/Desc
        if (fieldType === "dxTextBox" || fieldType === "dxTextArea") {
            fieldElement.attr("code-desc", fieldProp["codeDesc"]);
            var obj = {
                element: fieldElement,
                component: field
            }
            _code_desc_visible(obj, fieldProp["codeDesc"]);
        }


        // Set Disable and Styling
        // set disabled as readOnly, so text are still able to copy and able to scroll.
        field.option("readOnly", fieldProp["disabled"]);
        if (fieldProp["styleClass"]) {
            var input = $("#" + id).find(".dx-texteditor-input");
            input.addClass(fieldProp["styleClass"]);
        }


    }



}

function _render_epfScreen(scrnId, scrnLoad) {
    $('#' + scrnId).each(function () {
        if (_formFields[scrnId]) {
            if (_formFields[scrnId].shouldRender) {
                _formFields[scrnId].shouldRender = false;

                setTimeout(function () {
                    if (_formFields[scrnId]) {
                        var eventData = {
                            "FieldId": scrnId,
                            "Value": "",
                            "PreviousValue": "",
                            "EventType": "OnScreenReady",
                            "ScrnLoad": scrnLoad
                        };
                        var jsonEventData = JSON.stringify(eventData);
                        _formFields[scrnId].instance.invokeMethodAsync(_formFields[scrnId].eventCallBackMethod, jsonEventData);
                    }
                }, 100);
            }
        }

    });
}

function _render_field(fieldId) {
    var fieldType = $("#" + fieldId).attr("field-type");
    if (fieldType === "dxTextBox") {
        _render_dxTextBox(fieldId);
    }
    else if (fieldType === "dxNumberBox") {
        _render_dxNumberBox(fieldId);
    }
    else if (fieldType === "dxCheckBox") {
        _render_dxCheckBox(fieldId);
    }
    else if (fieldType === "dxDateBox") {
        _render_dxDateBox(fieldId);
    }
    else if (fieldType === "dxTextArea") {
        _render_dxTextArea(fieldId);
    }
    else if (fieldType === "dxSelectBox") {
        _render_dxSelectBox(fieldId);
    }
    else if (fieldType === "dxDataGrid") {
        _render_dxDataGrid(fieldId);
    }
    else if (fieldType === "dxList") {
        _render_dxList(fieldId);
    }
    else if (fieldType === "dxAccordion") {
        _render_dxAccordion(fieldId);
    }
}

function _code_desc_visible(e, visible) {
    var codeDesc = $(e.element).attr("code-desc");
    var value = e.component.option("value");

    if (visible) { // display code-desc while focusOut
        if (codeDesc && value) {
            var value = e.component.option("value");
            if (!_contains(value, " (" + codeDesc + ")")) {
                value = value + " (" + codeDesc + ")";
                e.component.option("value", value);
            }
        }
    }
    else {
        if (codeDesc && value) {
            value = value.replace(" (" + codeDesc + ")", "");
            e.component.option("value", value);
        }
    }
}

function _contains(mainString, searchString) {
    mainString = mainString.toLowerCase();
    searchString = searchString.toLowerCase();
    return mainString.includes(searchString);
}

function _render_dxTextBox(fieldId) {
    $("#" + fieldId).dxTextBox({
        onFocusOut: async function (e) {
            if (e.event) {
                var fieldId = $(e.element).attr("id");
                var eventData = {
                    "FieldId": fieldId,
                    "Value": this.option("value"),
                    "PreviousValue": this.option("value"),
                    "EventType": e.event.type
                };
                if (_formFields[fieldId]) {
                    var jsonEventData = JSON.stringify(eventData);
                    await _formFields[fieldId].instance.invokeMethodAsync(_formFields[fieldId].eventCallBackMethod, jsonEventData);
                    if (!e.component.option("readOnly")) _code_desc_visible(e, true);
                }
            }
        },
        onFocusIn: async function (e) {
            if (e.event) {
                if (!e.component.option("readOnly")) _code_desc_visible(e, false);

                var fieldId = $(e.element).attr("id");
                var eventData = {
                    "FieldId": fieldId,
                    "Value": this.option("value"),
                    "PreviousValue": this.option("value"),
                    "EventType": e.event.type
                };
                if (_formFields[fieldId]) {
                    var jsonEventData = JSON.stringify(eventData);
                    await _formFields[fieldId].instance.invokeMethodAsync(_formFields[fieldId].eventCallBackMethod, jsonEventData);
                }
            }
        },
        onValueChanged: function (e) {
            if (e.event) {
                var fieldId = $(e.element).attr("id");
                var eventData = {
                    "FieldId": fieldId,
                    "Value": e.value,
                    "PreviousValue": e.previousValue,
                    "EventType": e.event.type
                };
                if (_formFields[fieldId]) {
                    var jsonEventData = JSON.stringify(eventData);
                    _formFields[fieldId].instance.invokeMethodAsync(_formFields[fieldId].eventCallBackMethod, jsonEventData);
                }

            }
        }
    });
}

function _render_dxNumberBox(fieldId) {
    $("#" + fieldId).dxNumberBox({
        onFocusOut: function (e) {
            if (e.event) {
                var fieldId = $(e.element).attr("id");
                var eventData = {
                    "FieldId": fieldId,
                    "Value": this.option("value"),
                    "PreviousValue": this.option("value"),
                    "EventType": e.event.type
                };
                var jsonEventData = JSON.stringify(eventData);
                _formFields[fieldId].instance.invokeMethodAsync(_formFields[fieldId].eventCallBackMethod, jsonEventData);
            }
        },
        onFocusIn: function (e) {
            if (e.event) {
                var fieldId = $(e.element).attr("id");
                var eventData = {
                    "FieldId": fieldId,
                    "Value": this.option("value"),
                    "PreviousValue": this.option("value"),
                    "EventType": e.event.type
                };
                var jsonEventData = JSON.stringify(eventData);
                _formFields[fieldId].instance.invokeMethodAsync(_formFields[fieldId].eventCallBackMethod, jsonEventData);
            }
        },
        onValueChanged: function (data) {
            if (data.event) {
                var fieldId = $(data.element).attr("id");
                var eventData = {
                    "FieldId": fieldId,
                    "Value": data.value,
                    "PreviousValue": data.previousValue,
                    "EventType": data.event.type
                };
                var jsonEventData = JSON.stringify(eventData);
                _formFields[fieldId].instance.invokeMethodAsync(_formFields[fieldId].eventCallBackMethod, jsonEventData);
            }
        },
        onKeyPress: function (e) {
            if (isNaN(e.event.key) && e.event.key !== ".") {
                e.event.preventDefault();
            }
        }
    });
}

function _render_dxCheckBox(fieldId) {
    $("#" + fieldId).dxCheckBox({
        //text: fieldProp["text"],
        onFocusOut: function (e) {
            if (e.event) {
                var fieldId = $(e.element).attr("id");
                var eventData = {
                    "FieldId": fieldId,
                    "Value": this.option("value"),
                    "PreviousValue": this.option("value"),
                    "EventType": e.event.type
                };
                var jsonEventData = JSON.stringify(eventData);
                _formFields[fieldId].instance.invokeMethodAsync(_formFields[fieldId].eventCallBackMethod, jsonEventData);
            }
        },
        onFocusIn: function (e) {
            if (e.event) {
                var fieldId = $(e.element).attr("id");
                var eventData = {
                    "FieldId": fieldId,
                    "Value": this.option("value"),
                    "PreviousValue": this.option("value"),
                    "EventType": e.event.type
                };
                var jsonEventData = JSON.stringify(eventData);
                _formFields[fieldId].instance.invokeMethodAsync(_formFields[fieldId].eventCallBackMethod, jsonEventData);
            }
        },
        onValueChanged: function (data) {
            if (data.event) {
                var fieldId = $(data.element).attr("id");
                var eventData = {
                    "FieldId": fieldId,
                    "Value": data.value,
                    "PreviousValue": data.previousValue,
                    "EventType": data.event.type
                };
                var jsonEventData = JSON.stringify(eventData);
                _formFields[fieldId].instance.invokeMethodAsync(_formFields[fieldId].eventCallBackMethod, jsonEventData);
            }
        }
    });
}

function _render_dxDateBox(fieldId) {
    $("#" + fieldId).dxDateBox({
        //type: fieldProp["type"],
        onFocusOut: function (e) {
            if (e.event) {
                var fieldId = $(e.element).attr("id");
                var eventData = {
                    "FieldId": fieldId,
                    "Value": this.option("value"),
                    "PreviousValue": this.option("value"),
                    "EventType": e.event.type
                };
                var jsonEventData = JSON.stringify(eventData);
                _formFields[fieldId].instance.invokeMethodAsync(_formFields[fieldId].eventCallBackMethod, jsonEventData);
            }
        },
        onFocusIn: function (e) {
            if (e.event) {
                var fieldId = $(e.element).attr("id");
                var eventData = {
                    "FieldId": fieldId,
                    "Value": this.option("value"),
                    "PreviousValue": this.option("value"),
                    "EventType": e.event.type
                };
                var jsonEventData = JSON.stringify(eventData);
                _formFields[fieldId].instance.invokeMethodAsync(_formFields[fieldId].eventCallBackMethod, jsonEventData);
            }
        },
        onValueChanged: function (data) {
            if (data.event) {
                var fieldId = $(data.element).attr("id");
                var dateStrPrev = null;
                var dateStr = null;

                if (data.value) {

                    var tempDate = new Date(data.value);
                    if (data.component.option("type") === "date") {
                        tempDate = new Date(new Date(data.value).toDateString());
                    }
                    dateStr = _formatDate(tempDate, "isodatetime");
                }


                if (data.previousValue) {

                    var tempDate = new Date(data.previousValue);
                    if (data.component.option("type") === "date") {
                        tempDate = new Date(new Date(data.previousValue).toDateString());
                    }
                    dateStrPrev = _formatDate(tempDate, "isodatetime");
                }


                var eventData = {
                    "FieldId": fieldId,
                    "Value": dateStr,
                    "PreviousValue": dateStrPrev,
                    "EventType": data.event.type
                };

                if (eventData.Value == eventData.PreviousValue) {
                    return;
                }

                var jsonEventData = JSON.stringify(eventData);
                _formFields[fieldId].instance.invokeMethodAsync(_formFields[fieldId].eventCallBackMethod, jsonEventData);
            }
        }
    });
}

function _render_dxTextArea(fieldId) {
    $("#" + fieldId).dxTextArea({
        onFocusOut: function (e) {
            if (e.event) {
                var fieldId = $(e.element).attr("id");
                var eventData = {
                    "FieldId": fieldId,
                    "Value": this.option("value"),
                    "PreviousValue": this.option("value"),
                    "EventType": e.event.type
                };
                var jsonEventData = JSON.stringify(eventData);
                _formFields[fieldId].instance.invokeMethodAsync(_formFields[fieldId].eventCallBackMethod, jsonEventData);
                if (!e.component.option("readOnly")) _code_desc_visible(e, true);
            }
        },
        onFocusIn: function (e) {
            if (e.event) {
                var fieldId = $(e.element).attr("id");
                var eventData = {
                    "FieldId": fieldId,
                    "Value": this.option("value"),
                    "PreviousValue": this.option("value"),
                    "EventType": e.event.type
                };
                var jsonEventData = JSON.stringify(eventData);
                _formFields[fieldId].instance.invokeMethodAsync(_formFields[fieldId].eventCallBackMethod, jsonEventData);
                if (!e.component.option("readOnly")) _code_desc_visible(e, false);
            }
        },
        onValueChanged: function (data) {
            if (data.event) {
                var fieldId = $(data.element).attr("id");
                var eventData = {
                    "FieldId": fieldId,
                    "Value": data.value,
                    "PreviousValue": data.previousValue,
                    "EventType": data.event.type
                };
                var jsonEventData = JSON.stringify(eventData);
                _formFields[fieldId].instance.invokeMethodAsync(_formFields[fieldId].eventCallBackMethod, jsonEventData);
            }
        }
    });
}

function _render_dxSelectBox(fieldId) {
    $("#" + fieldId).dxSelectBox({
        onFocusOut: function (e) {
            if (e.event) {
                var fieldId = $(e.element).attr("id");
                var eventData = {
                    "FieldId": fieldId,
                    "Value": this.option("value"),
                    "PreviousValue": this.option("value"),
                    "EventType": e.event.type
                };
                var jsonEventData = JSON.stringify(eventData);
                _formFields[fieldId].instance.invokeMethodAsync(_formFields[fieldId].eventCallBackMethod, jsonEventData);
            }
        },
        onFocusIn: function (e) {
            if (e.event) {
                var fieldId = $(e.element).attr("id");
                var eventData = {
                    "FieldId": fieldId,
                    "Value": this.option("value"),
                    "PreviousValue": this.option("value"),
                    "EventType": e.event.type
                };
                var jsonEventData = JSON.stringify(eventData);
                _formFields[fieldId].instance.invokeMethodAsync(_formFields[fieldId].eventCallBackMethod, jsonEventData);
            }
        },
        onValueChanged: function (data) {
            if (data.event) {
                var fieldId = $(data.element).attr("id");
                var eventData = {
                    "FieldId": fieldId,
                    "Value": data.value,
                    "PreviousValue": data.previousValue,
                    "EventType": "onValueChanged"
                };
                var jsonEventData = JSON.stringify(eventData);
                _formFields[fieldId].instance.invokeMethodAsync(_formFields[fieldId].eventCallBackMethod, jsonEventData);
            }
        },
        onCustomItemCreating: function (data) {
            var newItem = {
                ID: data.text,
                Text: data.text,
                ID_Text: data.text
            };
            data.customItem = newItem;

            var fieldId = $(data.element).attr("id");
            var eventData = {
                "FieldId": fieldId,
                "Value": data.text,
                "PreviousValue": "",
                "EventType": "onCustomItemCreating"
            };
            var jsonEventData = JSON.stringify(eventData);
            _formFields[fieldId].instance.invokeMethodAsync(_formFields[fieldId].eventCallBackMethod, jsonEventData);
        }
    });
}

function _render_dxAccordion(fieldId) {
    //var fieldProp = JSON.parse(json);
    //var colInfos = fieldProp.columns;
    //var visibleColInfos = $.grep(fieldProp.columns, function (a) {
    //    return (a.visible === true);
    //});

    $("#" + fieldId).dxAccordion({
        noDataText: "<span class='pl-2'>No data to display</span>",
        deferRendering: true,
        itemTitleTemplate: function (itemData, itemIndex, itemElement) {
            var title = "";
            var subTitle = "";
            var value = "";

            var visibleColInfos = $.grep(this.option("columns"), function (a) {
                return (a.visible === true);
            });

            if (visibleColInfos.length > 0) {
                value = itemData[visibleColInfos[0].dataField];
                if (!value) {
                    value = "";
                }

                if (visibleColInfos[0].showLabel) {
                    title = visibleColInfos[0].caption + " :  " + value;
                }
                else {
                    title = value;
                }

                for (i = 1; i < visibleColInfos.length && i <= 3; i++) {

                    if (visibleColInfos[i].showInSubTitle) {

                        if (visibleColInfos[i].dataType === "date") {
                            value = _formatDate(new Date(itemData[visibleColInfos[i].dataField]), "date");
                        }
                        else if (visibleColInfos[i].dataType === "datetime") {
                            value = _formatDate(new Date(itemData[visibleColInfos[i].dataField]), "datetime");
                        }
                        else {
                            value = itemData[visibleColInfos[i].dataField];
                        }

                        if (!value) {
                            value = ""
                        }

                        if (subTitle !== "") {
                            subTitle = subTitle + ", ";
                        }

                        if (visibleColInfos[i].showLabel) {
                            subTitle = subTitle + visibleColInfos[i].caption + " :  " + value;
                        }
                        else {
                            subTitle = subTitle + value;
                        }

                    }

                }
            }
            var titleElement = "<div><div class='field-accordion-title'>"
                + title
                + "</div>"
                + "<div class='field-accordion-subtitle'>"
                + subTitle
                + "</div></div>";
            itemElement.append(titleElement);
        },
        itemTemplate: function (itemData, itemIndex, itemElement) {
            if (this.option("showDetail")) {
                var colInfos = this.option("columns");
                var baseUrl = document.getElementById("baseUrl").href;
                var detailElement = "<div class='row field-accordion-detail'>";
                for (var index in colInfos) {
                    if (colInfos[index].showInDetail && colInfos[index].visible) {
                        var value = "";
                        if (colInfos[index].dataType === "date") {
                            value = _formatDate(new Date(itemData[colInfos[index].dataField]), "date");
                        }
                        else if (colInfos[index].dataType === "datetime") {
                            value = _formatDate(new Date(itemData[colInfos[index].dataField]), "datetime");
                        }
                        else if (colInfos[index].format === "IMAGE") {

                            value = $("<div style='text-align:center'>")
                                .append($("<img>", {
                                    "src": baseUrl + itemData[colInfos[index].dataField],
                                    "style": "width:100%; object-fit:contain;",
                                    "loading": "lazy"
                                })).html();

                        }
                        else {
                            value = itemData[colInfos[index].dataField];
                        }

                        if (!value) {
                            value = ""
                        }

                        if (colInfos[index].showLabel) {
                            detailElement = detailElement + "<div class='col-12 field-accordion-detail-item'>" + colInfos[index].caption + " :  " + value + "</div>";
                        }
                        else {
                            detailElement = detailElement + "<div class='col-12 field-accordion-detail-item'>" + value + "</div>";
                        }
                    }
                }
                detailElement = detailElement + "</div>";
                itemElement.append(detailElement);
            }


            if (this.option("accordionType") === "Navigate") {
                var btnUpdate = $("<input type='button' class='col btn btn-outline-primary text-nowrap field-accordion-detail-button' value='Detail'>");
                btnUpdate.click(function () {
                    var eventData = {
                        "FieldId": fieldId,
                        "Value": itemData,
                        "PreviousValue": "",
                        "EventType": "onDetailClick"
                    };
                    var jsonEventData = JSON.stringify(eventData);
                    _formFields[fieldId].instance.invokeMethodAsync(_formFields[fieldId].eventCallBackMethod, jsonEventData);
                });
                var btnUpdateWrapperCol = $("<div class='col'></div>").append(btnUpdate);
            }

            if (this.option("deleteRecord")) {
                var screenId = this.option("screenId");
                var btnDelete = $("<input type='button' class='col btn btn-outline-primary text-nowrap field-accordion-detail-button' value='Delete'>");
                btnDelete.click(function () {
                    var eventData = {
                        "FieldId": fieldId,
                        "Value": itemData,
                        "PreviousValue": "",
                        "EventType": "onRowRemoving"
                    };
                    var jsonEventData = JSON.stringify(eventData);
                    //_formFields[fieldId].instance.invokeMethodAsync(_formFields[fieldId].eventCallBackMethod, jsonEventData);
                    ExecCSharpAsync(screenId, "accordion_OnAccordionActionAsync", jsonEventData);
                });
                var btnDeleteWrapperCol = $("<div class='col'></div>").append(btnDelete);
            }

            // check download button is in button list
            if (this.option("buttons").indexOf("download") > -1) {
                var btnDownload = $("<input type='button' class='col btn btn-outline-primary text-nowrap field-accordion-detail-button' value='Download'>");
                btnDownload.click(function () {
                    var eventData = {
                        "FieldId": fieldId,
                        "Value": itemData,
                        "PreviousValue": "",
                        "EventType": "onDownloadClick"
                    };
                    var jsonEventData = JSON.stringify(eventData);
                    //_formFields[fieldId].instance.invokeMethodAsync(_formFields[fieldId].eventCallBackMethod, jsonEventData);
                    ExecCSharpAsync(screenId, "accordion_OnAccordionActionAsync", jsonEventData);
                });
                var btnDownloadWrapperCol = $("<div class='col'></div>").append(btnDownload);
            }

            var btnWrapperRow = $("<div class='row pt-3' style='float:right; clear:right;'></div>")
                .append(btnDownloadWrapperCol)
                .append(btnDeleteWrapperCol)
                .append(btnUpdateWrapperCol);

            itemElement.append(btnWrapperRow);

        }
    });

}

function _formatDate(tempDate, type) {
    var value = "";
    var dd;
    var MM;
    var yyyy;
    var hh;
    var ss;
    var mm;
    if (type === "date") {
        dd = tempDate.getDate();
        MM = tempDate.getMonth() + 1;
        yyyy = tempDate.getFullYear();
        if (dd < 10) {
            dd = "0" + dd;
        }
        if (MM < 10) {
            MM = "0" + MM;
        }
        value = dd + "/" + MM + "/" + yyyy;
    }
    else if (type === "datetime") {
        dd = tempDate.getDate();
        MM = tempDate.getMonth() + 1;
        yyyy = tempDate.getFullYear();
        HH = tempDate.getHours();
        mm = tempDate.getMinutes();
        ss = tempDate.getSeconds();
        if (dd < 10) {
            dd = "0" + dd;
        }
        if (MM < 10) {
            MM = "0" + MM;
        }
        if (HH < 10) {
            HH = "0" + HH;
        }
        if (mm < 10) {
            mm = "0" + mm;
        }
        if (ss < 10) {
            ss = "0" + ss;
        }
        value = dd + "/" + MM + "/" + yyyy + " " + HH + ":" + mm + ":" + ss;
    }
    else if (type === "isodatetime") {
        dd = tempDate.getDate();
        MM = tempDate.getMonth() + 1;
        yyyy = tempDate.getFullYear();
        HH = tempDate.getHours();
        mm = tempDate.getMinutes();
        ss = tempDate.getSeconds();
        if (dd < 10) {
            dd = "0" + dd;
        }
        if (MM < 10) {
            MM = "0" + MM;
        }
        if (HH < 10) {
            HH = "0" + HH;
        }
        if (mm < 10) {
            mm = "0" + mm;
        }
        if (ss < 10) {
            ss = "0" + ss;
        }
        //value = dd + "/" + MM + "/" + yyyy + " " + HH + ":" + mm + ":" + ss;
        value = yyyy + "-" + MM + "-" + dd + "T" + HH + ":" + mm + ":" + ss;
    }
    else {
        value = tempDate.toLocaleString();
    }
    return value;
}

const delay = (ms) => new Promise((resolve) => setTimeout(resolve, ms));

function _render_dxDataGrid(fieldId) {

    $("#" + fieldId).dxDataGrid({
        keyExpr: "",
        //columnFixing: {
        //    enabled: true
        //},
        hoverStateEnabled: true,
        allowColumnResizing: true,
        columnResizingMode: "nextColumn",
        columnMinWidth: 50,
        showColumnLines: true,
        showRowLines: true,
        rowAlternationEnabled: true,
        showBorders: true,
        allowColumnReordering: true,
        noDataText: "",
        filterRow: {
            visible: true,
            applyFilter: "auto"
        },
        pager: {
            showNavigationButtons: true,
            showPageSizeSelector: false
        },
        groupPanel: {
            visible: true
        },
        grouping: {
            autoExpandAll: false
        },
        errorRowEnabled: false,
        export: {
            enabled: true
        },
        onRowRemoving: function (e) {
            var gridview = this;

            var screenId = gridview.option("screenId");
            var elementId = e.element.attr("id");
            var eventData = {
                "FieldId": _getFieldId(elementId),
                "Value": e.data,
                "PreviousValue": "",
                "EventType": "onRowRemoving"
            };
            var jsonEventData = JSON.stringify(eventData);
            gridview.beginCustomLoading();
            const deferred = $.Deferred();
            ExecCSharpAsync(screenId, "gridView_OnEditGridActionAsync", jsonEventData).then(function (returnData) {
                if (returnData != "") {
                    var editData = JSON.parse(returnData);

                    $(gridview.option("columns")).each(function (index, item) {
                        if (item.dataField) {
                            var editField = editData.Cells[item.dataField];
                            if (editField.Value) {
                                e.data[item.dataField] = editField.Value;
                            }
                            else {
                                e.data[item.dataField] = editField.DefaultValueOnAdd;
                            }
                        }
                    });


                    if (editData.Ok) {
                        deferred.resolve();
                    }
                    else {
                        deferred.reject();
                    }
                }
                gridview.endCustomLoading();
            });
            e.cancel = deferred.promise();
        },
        onRowRemoved: function (e) {
            var screenId = this.option("screenId");
            var elementId = e.element.attr("id");
            var eventData = {
                "FieldId": _getFieldId(elementId),
                "Value": e.data,
                "PreviousValue": "",
                "EventType": "onRowRemoved"
            };
            var jsonEventData = JSON.stringify(eventData);
            ExecCSharpAsync(screenId, "gridView_OnEditGridActionAsync", jsonEventData);
        },
        onInitNewRow: function (e) {
            var gridview = this;

            $(gridview.option("columns")).each(function (index, item) {
                if (item.dataField) {
                    e.data[item.dataField] = item.DefValueOnAdd;
                }
            });

            var screenId = gridview.option("screenId");
            var elementId = e.element.attr("id");
            var eventData = {
                "FieldId": _getFieldId(elementId),
                "Value": e.data,
                "PreviousValue": "",
                "EventType": "onInitNewRow"
            };
            var jsonEventData = JSON.stringify(eventData);
            gridview.beginCustomLoading();
            e.promise = ExecCSharpAsync(screenId, "gridView_OnEditGridActionAsync", jsonEventData).then(function (returnData) {
                if (returnData != "") {
                    var editData = JSON.parse(returnData);
                    $(gridview).data("editData", JSON.parse(returnData));

                    $(gridview.option("columns")).each(function (index, item) {
                        if (item.dataField) {
                            var editField = editData.Cells[item.dataField];
                            if (editField.Value) {
                                e.data[item.dataField] = editField.Value;
                            }
                            else {
                                e.data[item.dataField] = editField.DefaultValueOnAdd;
                            }
                        }
                    });
                }
                gridview.endCustomLoading();
            });



        },
        onRowInserting: function (e) {
            var gridview = this;

            var screenId = gridview.option("screenId");
            var elementId = e.element.attr("id");
            var eventData = {
                "FieldId": _getFieldId(elementId),
                "Value": e.data,
                "PreviousValue": "",
                "EventType": "onRowInserting"
            };
            var jsonEventData = JSON.stringify(eventData);

            gridview.beginCustomLoading();
            const deferred = $.Deferred();
            ExecCSharpAsync(screenId, "gridView_OnEditGridActionAsync", jsonEventData).then(function (returnData) {
                if (returnData != "") {
                    var editData = JSON.parse(returnData);

                    $(gridview.option("columns")).each(function (index, item) {
                        if (item.dataField) {
                            var editField = editData.Cells[item.dataField];
                            if (editField.Value) {
                                e.data[item.dataField] = editField.Value;
                            }
                            else {
                                e.data[item.dataField] = editField.DefaultValueOnAdd;
                            }
                        }
                    });


                    if (editData.Ok) {
                        deferred.resolve();
                    }
                    else {
                        deferred.reject();
                    }
                }
                gridview.endCustomLoading();
            });
            e.cancel = deferred.promise();
        },
        onRowInserted: function (e) {
            var screenId = this.option("screenId");
            var elementId = e.element.attr("id");
            var eventData = {
                "FieldId": _getFieldId(elementId),
                "Value": e.data,
                "PreviousValue": "",
                "EventType": "onRowInserted"
            };
            var jsonEventData = JSON.stringify(eventData);
            ExecCSharpAsync(screenId, "gridView_OnEditGridActionAsync", jsonEventData);
        },
        onEditingStart: function (e) {
            var gridview = this;

            if ($(gridview).data("onEditingStartCallBack")) { // return immediately, if it is callback after data is loaded
                var editData = $(gridview).data("editData");
                $(gridview.option("columns")).each(function (index, item) {
                    if (item.dataField) {
                        var editField = editData.Cells[item.dataField];

                        if (editField.Value) {
                            e.data[item.dataField] = editField.Value;
                        }
                        else {
                            e.data[item.dataField] = editField.DefaultValueOnAdd;
                        }
                    }
                });

                $(gridview).data("onEditingStartCallBack", false);
                return;
            }

            var screenId = gridview.option("screenId");
            var elementId = e.element.attr("id");
            var eventData = {
                "FieldId": _getFieldId(elementId),
                "Value": e.data,
                "PreviousValue": "",
                "EventType": "onEditingStart"
            };
            var jsonEventData = JSON.stringify(eventData);
            gridview.beginCustomLoading();
            ExecCSharpAsync(screenId, "gridView_OnEditGridActionAsync", jsonEventData).then(function (returnData) {
                if (returnData != "") {
                    $(gridview).data("editData", JSON.parse(returnData));
                    $(gridview).data("onEditingStartCallBack", true);
                    gridview.editRow(gridview.getRowIndexByKey(e.key)); //Call back editing row fucntion after data is loaded
                }
                gridview.endCustomLoading();
            });

            if (!$(gridview).data("onEditingStartCallBack")) { //cancel editing row fucntion if data is not loaded yet
                e.cancel = true;
            }

        },
        onEditorPreparing: function (e) {
            var gridViewId = e.element.attr("id");

            if (!this.option("editing").allowAdding) {
                return;
            }

            if (e.parentType === "filterRow") {
                return;
            }

            if (!$(this).data("editData")) {
                return;
            }

            var editField = $(this).data("editData").Cells[e.dataField];

            if (editField.CellType == "dxTextBox") {
                e.editorName = editField.CellType;
                e.editorOptions.mask = editField.Mask;
                e.editorOptions.useMaskedValue = true;
            }
            else if (editField.CellType == "dxCheckBox") {
                e.editorName = editField.CellType;
            }
            else if (editField.CellType == "dxDateBox") {
                e.editorName = editField.CellType;
                e.editorOptions.type = editField.DateType;
                e.editorOptions.displayFormat = editField.DisplayFormat;
                e.editorOptions.useMaskBehavior = true;
            }
            else if (editField.CellType == "dxSelectBox") {
                e.editorName = editField.CellType;
                e.editorOptions.displayExpr = "TEXT";
                e.editorOptions.valueExpr = "ID";
                e.editorOptions.dataSource = editField.SelectData;
            }
            else if (editField.CellType == "pickList") {
                e.editorName = "dxTextBox";
                e.editorOptions.buttons = [{
                    name: e.dataField + "_dxBtn",
                    location: "after",
                    options: {
                        icon: "bulletlist",
                        stylingMode: "text",
                        type: "normal",
                        onClick: function () {
                            var gridview = $("#" + gridViewId).data("dxDataGrid");
                            var screenId = gridview.option("screenId");
                            var picklistBtnId = getFieldId(editField.PickListFieldId + "_BTN");
                            $("#" + picklistBtnId).click();
                        }
                    }
                }]
            }
            else if (editField.CellType == "dxButton") {
                e.editorName = editField.CellType;
                e.editorOptions.text = editField.Description;
                e.editorOptions.type = "normal";
                e.editorOptions.width = "100%";
            }


            e.editorOptions.onValueChanged = async function (args) {

                // change date format for datebox
                if (args.component.NAME == "dxDateBox") {
                    var tempDate = new Date(args.value);
                    if (args.component.option("type") === "date") {
                        tempDate = new Date(new Date(args.value).toDateString());
                    }
                    dateStr = _formatDate(tempDate, "isodatetime");
                    args.value = dateStr;
                }

                e.setValue(args.value);

                if (!editField.HasOnChangeEvent) {
                    return;
                }

                var data = {
                    "arg": {
                        "dataField": e.dataField,
                        "value": args.value,
                        "previousValue": args.previousValue
                    },
                    "rowData": e.row.data
                };

                var gridview = $("#" + gridViewId).data("dxDataGrid");
                var screenId = gridview.option("screenId");
                var eventData = {
                    "FieldId": _getFieldId(gridViewId),
                    "Value": data,
                    "PreviousValue": "",
                    "EventType": "onEditCellOnChanged"
                };
                var jsonEventData = JSON.stringify(eventData);

                gridview.beginCustomLoading();
                var returnData = await ExecCSharpAsync(screenId, "gridView_OnEditGridCellActionAsync", jsonEventData);
                if (returnData != "") {
                    var editData = JSON.parse(returnData);
                    $(gridview).data("editData", editData);

                    $(gridview.option("columns")).each(function (index, item) {
                        if (item.dataField) {
                            var editField = editData.Cells[item.dataField];
                            if (editField.Value) {
                                e.component.cellValue(e.row.rowIndex, item.dataField, editField.Value);
                            }
                            else {
                                e.component.cellValue(e.row.rowIndex, item.dataField, editField.DefaultValueOnAdd);
                            }
                        }
                    });

                }
                gridview.endCustomLoading();
            }


            // attached OnClick Event
            e.editorOptions.onClick = async function (args) {

                if (!editField.HasOnClickEvent) {
                    return;
                }

                var data = {
                    "arg": {
                        "dataField": e.dataField,
                        "value": args.value,
                        "previousValue": args.previousValue
                    },
                    "rowData": e.row.data
                };

                var gridview = $("#" + gridViewId).data("dxDataGrid");
                var screenId = gridview.option("screenId");
                var eventData = {
                    "FieldId": _getFieldId(gridViewId),
                    "Value": data,
                    "PreviousValue": "",
                    "EventType": "onEditCellOnClick"
                };
                var jsonEventData = JSON.stringify(eventData);

                gridview.beginCustomLoading();
                var returnData = await ExecCSharpAsync(screenId, "gridView_OnEditGridCellActionAsync", jsonEventData);
                if (returnData != "") {
                    var editData = JSON.parse(returnData);
                    $(gridview).data("editData", editData);

                    $(gridview.option("columns")).each(function (index, item) {
                        if (item.dataField) {
                            var editField = editData.Cells[item.dataField];
                            if (editField.Value) {
                                e.component.cellValue(e.row.rowIndex, item.dataField, editField.Value);
                            }
                            else {
                                e.component.cellValue(e.row.rowIndex, item.dataField, editField.DefaultValueOnAdd);
                            }
                        }
                    });

                }
                gridview.endCustomLoading();
            }


            // for disabled/enabled
            if (e.row.isNewRow) {
                // for inserting
                if (editField.Disabled) {
                    e.editorOptions.disabled = editField.Disabled;
                }
                else {
                    e.editorOptions.disabled = editField.DefaultDisabledOnAdd;
                }

            }
            else {
                // for editing
                if (editField.Disabled) {
                    e.editorOptions.disabled = editField.Disabled;
                }
                else {
                    e.editorOptions.disabled = editField.DefaultDisabledOnUpdate;
                }
            }

        },
        onEditorPrepared: function (e) {

            if ($(this).data("editData") && e.row.rowType == "data") {

                var gridViewId = e.element.attr("id");
                var gridview = $("#" + gridViewId).data("dxDataGrid");
                var screenId = gridview.option("screenId");

                var editField = $(this).data("editData").Cells[e.dataField];
                if (editField.CellType == "dxTextBox" || editField.CellType == "dxDateBox") {

                }
                else if (editField.CellType == "dxCheckBox") {
                    e.editorElement.parent().addClass("text-center");
                }
                else if (editField.CellType == "dxSelectBox") {

                }
                else if (editField.CellType == "pickList") {
                    var picklistElementId = getFieldId(editField.PickListFieldId + "_editField");
                    e.editorElement.attr("id", picklistElementId);
                }

                if (e.editorOptions.disabled) {
                    e.editorElement.parent().css("background-color", "#DFDFDF");
                }

            }

        },
        onRowUpdating: function (e) {
            var gridview = this;

            var data = new Object();
            $(gridview.option("columns")).each(function (index, item) {
                if (item.dataField) {
                    if (item.dataField in e.newData) {
                        data[item.dataField] = e.newData[item.dataField];
                    }
                    else {
                        data[item.dataField] = e.oldData[item.dataField];
                    }
                }
            });

            var screenId = gridview.option("screenId");
            var elementId = e.element.attr("id");
            var eventData = {
                "FieldId": _getFieldId(elementId),
                "Value": data,
                "PreviousValue": "",
                "EventType": "onRowUpdating"
            };
            var jsonEventData = JSON.stringify(eventData);
            gridview.beginCustomLoading();
            const deferred = $.Deferred();
            ExecCSharpAsync(screenId, "gridView_OnEditGridActionAsync", jsonEventData).then(function (returnData) {
                if (returnData != "") {
                    var editData = JSON.parse(returnData);

                    $(gridview.option("columns")).each(function (index, item) {
                        if (item.dataField) {
                            var editField = editData.Cells[item.dataField];
                            if (editField.Value) {
                                e.newData[item.dataField] = editField.Value;
                            }
                            else {
                                e.newData[item.dataField] = editField.DefaultValueOnAdd;
                            }
                        }
                    });

                    if (editData.Ok) {
                        deferred.resolve();
                    }
                    else {
                        deferred.reject();
                    }
                }
                gridview.endCustomLoading();
            });
            e.cancel = deferred.promise();
        },
        onRowUpdated: function (e) {
            var screenId = this.option("screenId");
            var elementId = e.element.attr("id");
            var eventData = {
                "FieldId": _getFieldId(elementId),
                "Value": e.data,
                "PreviousValue": "",
                "EventType": "onRowUpdated"
            };
            var jsonEventData = JSON.stringify(eventData);

            ExecCSharpAsync(screenId, "gridView_OnEditGridActionAsync", jsonEventData);
        },
        onSelectionChanged: async function (selectedItems) {

            if (this.option("selectTargetField")) {
                var screenId = this.option("screenId");
                var fieldId = this.option("selectTargetField");
                var selectSourceKeys = this.option("selectSourceKeys");

                var selectedData = selectedItems.selectedRowsData.map(function (a) {
                    var obj = {};
                    $(selectSourceKeys.split(";")).each(function (index, item) {
                        obj[item] = a[item];
                    });
                    return obj;
                });

                await SetFieldValueAsync(fieldId, JSON.stringify(selectedData), screenId);
            }


            if (this.option("selection.mode") !== "single") {
                return;
            } else if (this.option("buttonAction") == "" || this.option("buttonAction") == undefined) {
                var data = selectedItems.selectedRowsData[0];
                if (data) {
                    var elementId = $(selectedItems.element).attr("id");
                    var eventData = {
                        "FieldId": _getFieldId(elementId),
                        "Value": data,
                        "PreviousValue": "",
                        "EventType": "onSelectionChanged"
                    };
                    var jsonEventData = JSON.stringify(eventData);
                    if (_formFields[elementId]) {
                        _formFields[elementId].instance.invokeMethodAsync(_formFields[elementId].eventCallBackMethod, jsonEventData);
                    }
                }
            }
        },
        onRowPrepared: function (e) {
            e.rowElement.css({ height: 32 });
        },
        onRowClick: async function (e) {

            if (this.option("editRowOnClick")) { // for editing 
                var isEditing = this.element().find(".dx-edit-row").length > 0;
                if (!isEditing) {
                    e.component.editRow(e.rowIndex);
                }
                else {
                    await e.component.saveEditData(); // save the edited data first
                    e.component.editRow(e.rowIndex);
                }
            }
            else {
                $(this).data("rowIndexToEdit", null);
            }

            if (this.option("buttonAction") != "" && this.option("buttonAction") != undefined) {
                var data = e.data;
                if (data) {
                    var elementId = $(e.element).attr("id");
                    var eventData = {
                        "FieldId": _getFieldId(elementId),
                        "Value": data,
                        "ButtonAction": this.option("buttonAction"),
                        "PreviousValue": "",
                        "EventType": "onButtonAction"
                    };
                    this.option("buttonAction", "");
                    var jsonEventData = JSON.stringify(eventData);
                    if (_formFields[elementId]) {
                        _formFields[elementId].instance.invokeMethodAsync(_formFields[elementId].eventCallBackMethod, jsonEventData);
                    }
                }
            }
            else if (this.option("selection.mode") == "single" && (this.option("buttonAction") == "" || this.option("buttonAction") == undefined)) {
                var data = e.data;
                if (data) {
                    var elementId = $(e.element).attr("id");
                    var eventData = {
                        "FieldId": _getFieldId(elementId),
                        "Value": data,
                        "PreviousValue": "",
                        "EventType": "onRowClick"   // [20/08/2021, PhyoZin] change to onRowClick instead of onSelectionChanged
                    };
                    var jsonEventData = JSON.stringify(eventData);
                    if (_formFields[elementId]) {
                        _formFields[elementId].instance.invokeMethodAsync(_formFields[elementId].eventCallBackMethod, jsonEventData);
                    }
                }
            }
        },
        onContentReady: function (e) {
            var gridviewId = e.element.attr("id");
            var groupCount = $("#" + gridviewId).find(".dx-datagrid-group-panel").find(".dx-group-panel-item").length;
            if (groupCount > 0) {
                e.component.option("selection.selectAllMode", "page");
            }
            else {
                e.component.option("selection.selectAllMode", "allPages");
            }

        }


    });
}


function _render_dxList(fieldId) {
    $("#" + fieldId).dxList({
        itemTemplate: function (data, index, element) {
            if (this.option("readOnly")) {
                element.css("background-color", "#F8F5F0")
                element.text(data[this.option("displayExpr")]);
            }
            else {
                return data[this.option("displayExpr")];
            }
        },
        onItemClick: function (e) {
            if (e.itemData) {
                var fieldId = $(e.element).attr("id");
                var eventData = {
                    "FieldId": fieldId,
                    "Value": e.itemData.ID,
                    "PreviousValue": "",
                    "EventType": "onValueChanged"
                };
                var jsonEventData = JSON.stringify(eventData);
                _formFields[fieldId].instance.invokeMethodAsync(_formFields[fieldId].eventCallBackMethod, jsonEventData);
            }
        }

    });

    $("#" + fieldId).css("border", "1px solid #ced4da")
        .css("border-radius", "4px");
}

function _dxdatagrid_set_picklist_edit_field(scrnId, gridviewId, pickListFieldId, selectedVal) {
    var gridElementId = getFieldId(gridviewId);
    var picklistElementId = getFieldId(pickListFieldId);
    $("#" + gridElementId).find(".dx-edit-row").find("#" + picklistElementId + "_editField").data("dxTextBox").option("value", selectedVal);
}

function _dxdatagrid_add_row(screenId, gridFieldId) {
    var elementId = getFieldId(gridFieldId);
    var gridview = $("#" + elementId).data("dxDataGrid");
    gridview.addRow();
}

function isNotEmpty(value) {
    return value !== undefined && value !== null && value !== "";
}

function _getFieldId(elementId) {
    var scrnId = $("#" + elementId).closest(".screen-container").attr("id");
    return elementId.replace(scrnId + "__", "");
}

function getFieldId(fieldId) {
    if (fieldId.includes("__")) {
        return fieldId;
    }
    return _activeScreenId + "__" + fieldId;
}