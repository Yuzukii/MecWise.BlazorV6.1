const cacheVersion = "202106221101";  // Change to force clear service worker cache [Format: yyyyMMddHHmm]
const cacheEnable = true;
const filesToCache = [
    './'
];


const pathsToCache = [
    //---- paths
    'us1.locationiq.com/',
    '/Menu/GetMenu/',
    '/EpfScreen/GetScreen/',
    '/EpfScreen/GetColumnInfos/',
    '/_framework/',
    '/app-theme/',
    '/images/',
    '/js/',
    '/lib/',
    '/styles/',

    //----- extensions
    '.map',
    '.css',
    '.js',
    '.dat',
    '.dll',
    '.wasm',
    '.woff',
    '.woff2',
    '.jpg',
    '.png',
    '.svg',
    '.gif'
];

function contains(target, pattern) {
    var value = 0;
    pattern.forEach(function (word) {
        value = value + target.includes(word);
    });
    return (value > 0);
}

self.addEventListener('install', async event => {
    var cacheName = "mecwise-" + cacheVersion + "-" + event.target.registration.scope;
    console.log('[ServiceWorker] Install');
    const cache = await caches.open(cacheName);
    console.log('[ServiceWorker] Caching app shell');
    await cache.addAll(filesToCache);
    return self.skipWaiting();
});


self.addEventListener('activate', event => {

    //event.waitUntil(self.clients.claim());

    var cacheName = "mecwise-" + cacheVersion + "-" + event.target.registration.scope;
    var cacheKeeplist = [cacheName];
    event.waitUntil(
        caches.keys().then(keyList => {
            return Promise.all(keyList.map(key => {
                if (cacheKeeplist.indexOf(key) === -1) {
                    return caches.delete(key);
                }
            }));
        }).then(self.clients.claim())
    ); 

});


self.addEventListener('fetch', event => {
    // to prevent error, 'only-if-cached' can be set only with 'same-origin' mode
    if (event.request.cache === 'only-if-cached' && event.request.mode !== 'same-origin') {
        return;
    }

    var cacheName = "mecwise-" + cacheVersion + "-" + event.target.registration.scope;
    event.respondWith(
        caches.open(cacheName).then(function (cache) {

            //*** get from network if cache setting is not enable
            if (!cacheEnable) {
                return fetch(event.request);
            }

            //*** get from network if not a valid file to cache
            if (!contains(event.request.url, pathsToCache)) {
                return fetch(event.request);
            }

            // do not cache config file
            if (event.request.url.includes('/config.json')) {
                return fetch(event.request);
            }


            //*** check in cache first then get from network if request does not have in cache. Then save them in cache
            return cache.match(event.request).then(function (response) {
                return response || fetch(event.request).then(function (response) {
                    cache.put(event.request, response.clone());
                    return response;
                });
            });

        })
    );


    ////*** check in cache first then get from network if request does not exist in cache
    //event.respondWith(
    //    caches.match(event.request, { ignoreSearch: true }).then(response => {
    //        return response || fetch(event.request);
    //    })
    //);


});

/*
 * Push Notificaiton Events
*/
self.addEventListener('push', function (event) {
    if (Notification.permission === 'granted') {
        console.log('[ServiceWorker] Push Received.');
        //console.log('[ServiceWorker] Push had this data: " + ${event.data.text()});

        var data = JSON.parse(event.data.text());

        const title = data.Title;
        const regex = /<[^>]*>/gi;
        const message = data.Message.replace(regex, ''); // remove html tags
        const msgLaunchUrl = event.target.registration.scope
            + "/EpfScreen/INBOX?MODE=4&KEYS=MsgGrpID,MsgKey&PARENT_KEYS=MsgGrpID&PARAM="
            + data.MsgGrpID + "," + data.MsgKey;

        const options = {
            body: message,
            icon: './images/appIcon.png',
            badge: './images/appIcon.png',
            data: {
                url: msgLaunchUrl
            }
        };

        event.waitUntil(self.registration.showNotification(title, options));
    }
});


self.addEventListener('notificationclick', function (event) {
    console.log('[ServiceWorker] Notification click Received.');

    event.notification.close();

    event.waitUntil(
        clients.openWindow(event.notification.data.url)
    );
});