let swRegistration = null;
let isSubscribed = false;

checkUrlForSlash();


window.addEventListener('load', () => {
  registerSW();
});

async function registerSW() {

    //Registering Service Worker
    if ('serviceWorker' in navigator) {
        try {
            
            var pwaManifest = document.getElementById("pwaManifest");
            if (pwaManifest) {
                await navigator.serviceWorker.register('./sw.js').then(function (swReg) {
                    swRegistration = swReg;
                });
            }

            await checkForOidcUser();

        } catch (e) {
            console.log('SW registration failed');
            console.log(e);
        }
    }
}


function initializePushNotification() {
    // Set the initial subscription value
    swRegistration.pushManager.getSubscription()
        .then(function (subscription) {
            isSubscribed = !(subscription === null);

            if (isSubscribed) {
                console.log('User IS subscribed.');
                DisplaySubscription(subscription);
            } else {
                console.log('User is NOT subscribed.');
            }

        });
}


async function checkForOidcUser() {
    var oidc_user = document.getElementById("oidc_user");
    if (oidc_user) {
        SetStorageItem("oidc_user", oidc_user.value);
    }
    else {
        // no oidc_user element, remove oidc_user from storage if there is
        await RemoveStorageItem("oidc_user");
        await RemoveStorageItem(GetSessionKey());
        return;
    }
}

function checkUrlForSlash() {
    var url = window.location.href;
    var fullBaseWithSlash = document.getElementById("baseUrl").href;

    if (url.includes(fullBaseWithSlash) == false) {

        //check for slash at the end
        if ((url + "/").toUpperCase() === fullBaseWithSlash.toUpperCase()) {
            url = fullBaseWithSlash;
        }

        //replace base url because it is case sensitive
        var regEx = new RegExp(fullBaseWithSlash, "ig");
        var url = url.replace(regEx, fullBaseWithSlash);

        window.location.replace(url);
    }
    
}


async function subscribeUser(applicationServerPublicKey) {
    if (!swRegistration) {
        return "";
    }

    if (!IsIOS()) {
        Notification.requestPermission();
        const applicationServerKey = urlB64ToUint8Array(applicationServerPublicKey);
        const options = { applicationServerKey: applicationServerKey, userVisibleOnly: true };
        var subscription = await swRegistration.pushManager.subscribe(options);
        return JSON.stringify(subscription);
    }
    else {
        return "";
    }
}



function urlB64ToUint8Array(base64String) {
    const padding = '='.repeat((4 - base64String.length % 4) % 4);
    const base64 = (base64String + padding)
        .replace(/\-/g, '+')
        .replace(/_/g, '/');

    const rawData = window.atob(base64);
    const outputArray = new Uint8Array(rawData.length);

    for (let i = 0; i < rawData.length; ++i) {
        outputArray[i] = rawData.charCodeAt(i);
    }
    return outputArray;
}

function DisplaySubscription(subscription) {
    if (subscription) {
        var subscriptionJson = JSON.stringify(subscription);
        console.log(subscriptionJson);
    }
}

function updateSubscriptionOnServer(subscription) {
    // TODO: Send subscription to application server
    DisplaySubscription(subscription);
}


function unsubscribeUser() {
    var userAgent = window.navigator.userAgent;

    if (userAgent.match(/iPad/i) || userAgent.match(/iPhone/i) || userAgent.match('CriOS')) {
        // iPad or iPhone or Chrome in IOS
    }
    else {
        if (!swRegistration) {
            return;
        }

        swRegistration.pushManager.getSubscription()
            .then(function (subscription) {
                if (subscription) {
                    return subscription.unsubscribe();
                }
            })
            .catch(function (error) {
                console.log('Error unsubscribing', error);
            })
            .then(function () {
                updateSubscriptionOnServer(null);

                console.log('User is unsubscribed.');
                isSubscribed = false;
            });
    }

    
}