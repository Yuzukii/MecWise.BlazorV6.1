function SetNumOfUnRead(num) {
    if (iniFrame()) {
        if (num > 0) {
            $(".unread",parent.document).find(".background").css("display", "block");
            $(".unread", parent.document).find(".num").css("display", "block");
            $(".unread", parent.document).find(".num").text(num);
        }
        else {
            $(".unread", parent.document).find(".background").css("display", "none");
            $(".unread", parent.document).find(".num").css("display", "none");
            $(".unread", parent.document).find(".num").text(0);
        }
        return;
    }

    if (num > 0) {
        $(".unread").find(".background").css("display", "block");
        $(".unread").find(".num").css("display", "block");
        $(".unread").find(".num").text(num);
    }
    else {
        $(".unread").find(".background").css("display", "none");
        $(".unread").find(".num").css("display", "none");
        $(".unread").find(".num").text(0);
    }
}

async function GetGeoLocation() {
    if ('geolocation' in navigator) {
        var location = await GetCoordinates();
        if (location) {
            var geoCode = await ReverseGeocode(location.coords.latitude, location.coords.longitude);
            return JSON.stringify(geoCode);
        }
        else {
            return "";
        }
    } else {
        return 'Geolocation API not supported.';
    }
}

async function GetGeoCoordinates() {
    if ('geolocation' in navigator) {
        var location = await GetCoordinates();
        if (location) {
            var coordinates = {
                "latitude": location.coords.latitude,
                "longitude": location.coords.longitude
            };
            return JSON.stringify(coordinates);
        }
        else {
            return "";
        }
    } else {
        return 'Geolocation API not supported.';
    }
}

async function GetCoordinates() {
    return new Promise(function (resolve, reject) {
        navigator.geolocation.getCurrentPosition(resolve, reject);
    });
}

async function ReverseGeocode(latitude, longitude) {
    return new Promise(function (resolve, reject) {
        // from locationiq api
        //-------------------------
        $.getJSON("https://us1.locationiq.com/v1/reverse.php?key=2d46c201b3549a&lat=" + latitude + "&lon=" + longitude + "&format=json ", function (result) {
            var address = "";
            if (result) {
                address = result.display_name;
            }
            resolve(address);
        });

        // from google map api
        //-------------------------
        //$.getJSON("https://maps.googleapis.com/maps/api/geocode/json?latlng=" + latitude + "," + longitude + "&key=AIzaSyDlj62UmhsQRuMJ10IwrcgxW7W_5uaZojc", function (result) {
        //    var address = "";
        //    if (result && result.results[0]) {
        //        address = result.results[0].formatted_address;
        //    }
        //    resolve(address);
        //});
    });
}

function LoginRedirect() {

    //[07/12/2021, PhyoZin] no need to check session expire from popup iframe page
    if (window.frameElement) {
        if (window.frameElement.id == "popupframe") {
            return;
        }
    }

    var json = GetLocalStorageItem(GetSessionKey());
    if (json) {
        var session = JSON.parse(json);
        var expiredTime = new Date(session.AccessToken.ExpiredTime);

        var now = new Date();
        if (now > expiredTime) {

            //[07/12/2021, PhyoZin] Commented unregistering Push Noti
            // User should still get notification even after session expired.

            //// Unregister iOS Push Noti
            //if (IsIOS()) {
            //    ExecIOSFunc("UnregisterPushNoti", "");
            //}

            //// Unregister Android Push Noti
            //if (IsAndroid()) {
            //    console.log('request_for_action#deregister_push_notification');
            //}

            // Unsubscribe Web Push Noti
            if (!IsIOS() && !IsAndroid()) {
                unsubscribeUser();
            }

            RemoveLocalStorageItem(GetSessionKey());
            SetLocalStorageItem(GetSessionKey() + "-expired", JSON.stringify(session));
            window.location.replace(""); // navigate back to website root
        }
    }

    setTimeout("LoginRedirect()", 70000);
}


function GetSessionKey() {
    return "blazor-app-session-" + document.getElementById("baseUrl").getAttribute("href");
}

async function iniFrame() {
    // Checking if webpage is embedded 
    if (window.top != window.self) {
        // The page is in an iFrame 
        return true;
    }
    else {
        // The page is not in an iFrame 
        return false;
    }
} 

async function PromtMessage(promtText, defaultValue) {
    var taskTitle = prompt(promtText, defaultValue);
    return taskTitle;
}

async function ConfirmMessage(confirmText) {
    var isConfirm = confirm(confirmText);
    return isConfirm;
}

async function ShowMessage(msg) {
    if (IsAndroid()) {
        console.log("request_for_action:display_message/" + msg);
    }
    else {
        alert(msg);
    }
}

async function ToastMessage(msg, type, displayTime) {
    msg = JSON.parse(msg);
    msg.animation = {
        show: { type: 'fade', duration: 400, from: 0, to: 1 },
        hide: { type: 'fade', duration: 400, to: 0 }
    };
    DevExpress.ui.notify(msg, type, displayTime);
}

async function RegisterMobilePushNoti(regId) {
    if (IsAndroid()) {
        console.log("request_for_action:register_push_notification/" + regId);
    }
}

async function DeRegisterMobilePushNoti() {
    if (IsAndroid()) {
        console.log("request_for_action:deregister_push_notification");
    }
}


function GetScreenSize() {
    var screenSize = { screenWidth: screen.availWidth, screenHeight: screen.availHeight };
    return JSON.stringify(screenSize);
}


function GetBrowserType() {
    var ua = navigator.userAgent;

    if (/Chrome/i.test(ua)) {
        if (IsMobileScreen())
            return 'mobile-chrome';
        else
            return 'chrome';
    }
    else if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini|Mobile|mobile|CriOS/i.test(ua))
        return 'mobile-other';
    else
        return 'desktop-other';
}

function IsAndroid() {
    var userAgent = navigator.userAgent;
    if (userAgent.includes("IsMecWiseBlazorAndroid"))
        return true;
    else
        return false;
}

function IsIOS() {
    if (window.webkit && typeof ExecIOSFunc === "function") {
        return true;
    }
    
    return false;
}

function IsMobileScreen() {
    if (screen.availWidth < 768) {
        return true;
    }
    else {
        return false;
    }
}

async function LogMessage(msg) {
    console.log(msg);
}

async function SetStorageItem(key, data) {
    if (data) {
        data = window.btoa(data); // Encode to base64String
    }
    sessionStorage.setItem(key, data);
}

async function GetStorageItem(key) {
    var data = sessionStorage.getItem(key);
    if (data) {
        data = window.atob(data); // Decode from base64String
    }
    return data;
}

async function RemoveStorageItem(key) {
    sessionStorage.removeItem(key);
}

async function SetLocalStorageItem(key, data) {
    if (data) {
        data = window.btoa(data); // Encode to base64String
    }
    localStorage.setItem(key, data);
}

function GetLocalStorageItem(key) {
    var data = localStorage.getItem(key);
    if (data) {
        data = window.atob(data); // Decode from base64String
    }
    return data;
}

async function RemoveLocalStorageItem(key) {
    localStorage.removeItem(key);
}


async function NavigateWithoutHistory(url) {
    if (url === "") {
        var base = document.getElementsByTagName("base");
        window.location.replace(base[0].href);
    }
    else {
        window.location.replace(url);
    }
}

function NewWindow(url, displayMenu, popup) {
    if (popup) {
        var wnd = window.open("about:blank", "", "_blank");
    }
    else {
        var wnd = window.open("about:blank");
    }

    if (displayMenu) {
        wnd.location.replace(url);
    }
    else {
        url = _updateQueryStringParameter(url, "MENU", 0);
        wnd.location.replace(url);
    }
}

async function FixBrokenMenuImages(defaultImage) {
    $("img[field-type='menuImage']").each(function () {
        this.onerror = function () {
            this.src = defaultImage;
        };
    });
}

async function _navigateURL(url, optPopup) {
    if (optPopup == true) {
        var myWindow = window.open(url, "", "width=800,height=800");
    } else {
        window.open(url);
    }
}


/* Image List Control - Start */

function RenderImageList() {
	ChangeImageListType();
	AdjustContainer();
}


function ChangeImageListType() {
    $('.btnLayoutH').removeClass('active');
    $('.btnLayoutV').addClass('active').click();
}


async function FixBrokenItemImages(defaultImage) {
    $("img[field-type='itemImage']").each(function () {
        this.onerror = function () {
            this.src = defaultImage;
        };
    });
}

function AdjustCellContainer(cntSetting, PageSettCtrl, currPageCtrl, currPage) {
    SetFieldValue("CELL_PAGE_SETT", cntSetting, "KDS_CELL_BLZ");
    $('.cell-item').css('min-width', '30vw');
    switch (cntSetting) {
        case 3:
            $('.cell-item').css('min-width', '30vw');
            break;
        case 4:
            $('.cell-item').css('min-width', '22vw');
            break;
        case 5:
            $('.cell-item').css('min-width', '18vw');
            break;
        case 6:
            $('.cell-item').css('min-width', '30vw');
            break;
        case 8:
            $('.cell-item').css('min-width', '22vw');
            break;
        case 10:
            $('.cell-item').css('min-width', '18vw');
            break;
        default:
            $('.cell-item').css('min-width', '18vw');
            break;
        // code block
    }
}

function AdjustContainer() {

	$('.inner-shopping-container-h').each(function () {

		var noOfItem = $(this).find('.shopping-item').length;


		if (noOfItem > 0) {
			$(this).css('width', (noOfItem * 260) + 'px');
		}
		else
			$(this).css('width', '100%');

	});

	$('.inner-shopping-container-v').each(function () {
		$(this).css('width', 'auto');
	});
}

function setlayout(btn, layout,imgCol) {
	var container = $(btn).parents('.imageListContainer');
	if (layout == 'h') {
		container.find('.outer-shopping-container-v')
			.removeClass('outer-shopping-container-v').addClass('outer-shopping-container-h');
		container.find('.inner-shopping-container-v')
            .removeClass('inner-shopping-container-v').addClass('inner-shopping-container-h');
        if (imgCol == 2) {
            container.find('.shopping-item-container-v')
                .removeClass('shopping-item-container-v col-md-6 col-sm-6 col-6 padding-adj').addClass('shopping-item-container-h');
        } else {
            container.find('.shopping-item-container-v')
                .removeClass('shopping-item-container-v col-md-4 col-sm-6 col-6 padding-adj').addClass('shopping-item-container-h');
        }
		//container.find('.shopping-item-container-v')
		//	.removeClass('shopping-item-container-v col-md-4 col-sm-6 col-6 padding-adj').addClass('shopping-item-container-h');
		container.find('.shopping-item')
			.removeClass('shopping-v-border').addClass('shopping-h-border');

		container.find('[id$="ImageListLayoutType"]').val(layout);
	}
	else if (layout == 'v') {
		container.find('.outer-shopping-container-h')
			.removeClass('outer-shopping-container-h').addClass('outer-shopping-container-v');
		container.find('.inner-shopping-container-h')
            .removeClass('inner-shopping-container-h').addClass('inner-shopping-container-v');
        if (imgCol == 2) {
            container.find('.shopping-item-container-h')
                .removeClass('shopping-item-container-h').addClass('shopping-item-container-v col-md-6 col-sm-6 col-6 padding-adj');
        } else {
            container.find('.shopping-item-container-h')
                .removeClass('shopping-item-container-h').addClass('shopping-item-container-v col-md-4 col-sm-6 col-6 padding-adj');
        }
		//container.find('.shopping-item-container-h')
		//	.removeClass('shopping-item-container-h').addClass('shopping-item-container-v col-md-4 col-sm-6 col-6 padding-adj');
		container.find('.shopping-item')
			.removeClass('shopping-h-border').addClass('shopping-v-border');
		container.find('[id$="ImageListLayoutType"]').val(layout);
	}
	else {
		//do nothing
	}
	AdjustContainer();
}

function HideImageListLayoutOption() {
	$('.imagelist-option').hide();
}

async function ModalDialogAlert(title, message) {
    return new Promise(function (resolve, reject) {
        BootstrapDialog.show({
            title: title,
            message: message,
            closable: false,
            buttons: [{
                label: 'Ok',
                cssClass: 'btn-primary',
                action: function (dialogItself) {
                    resolve(true);
                    dialogItself.close();
                }
            }]
        });
    });
}

async function ModalDialogConfirm(title, message, btnOkText, btnCancelText) {
    return new Promise(function (resolve, reject) {
        BootstrapDialog.show({
            title: title,
            message: message,
            closable: false,
            buttons: [{
                label: btnOkText,
                cssClass: 'btn-primary',
                action: function (dialogItself) {
                    resolve(true);
                    dialogItself.close();
                }
            }, {
                label: btnCancelText,
                cssClass: 'btn-primary',
                action: function (dialogItself) {
                    resolve(false);
                    dialogItself.close();
                }
            }
            ]
        });
    });
}

function CloseErrorMessage() {
    $("#blazor-error-ui").css("display", "none");
}

function CloseWindow() {
    if (iniFrame()) {
        window.parent.close();
    }
    else {
        window.close();
    }
}

function SetPageTitle(title) {
    document.title = title;
}