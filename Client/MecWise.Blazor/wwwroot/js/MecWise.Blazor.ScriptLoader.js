var cssId = 'App_Themes';
$.getJSON("config.json", function (config) {
    if (!document.getElementById(cssId)) {
        var head = document.getElementsByTagName('head')[0];
        var appTheme = config.appTheme;

        //Add more common css here, by adding more css to the array indicated below
        var ArrayOfLink = ['main.css'];

        //for loop to loop every element that is indicated in ArrayOfLink
        for (var i = 0; i < ArrayOfLink.length; i++) {
            var link = document.createElement('link');
            link.id = cssId;
            link.rel = 'stylesheet';
            link.type = 'text/css';
            link.href = "app-theme/" + appTheme + "/" + ArrayOfLink[i];
            link.media = 'all';
            head.appendChild(link);
        }
    }
});

// loadScript: returns a promise that completes when the script loads
async function _loadScreenScript(scrnId, scriptType, scriptPath) {
    if (scrnId === "http:" || scrnId === "https:") {
        return;
    }

    return new Promise(function (resolve, reject) {
        // create JS library script element or CSS link
        var script;
        if (scriptType === "text/javascript") {
            script = document.createElement("script");
            script.src = scriptPath;
        }
        else {
            script = document.createElement("link");
            script.rel = 'stylesheet';
            script.href = scriptPath;
            script.media = 'all';
        }
        script.type = scriptType; // "text/javascript", "text/css"
        
        // if the script returns okay, return resolve
        script.onload = function () {
            //console.log(scriptPath + " loaded ok");
            resolve(scriptPath);
        };

        // if it fails, return reject
        script.onerror = function () {
            console.log(scriptPath + " load failed");
            reject(scriptPath);
        }

        // scripts will load at end of body
        if (document.getElementById(scrnId)) {
            document.getElementById(scrnId).appendChild(script);
        }
        
    });
}

async function _clearScreenScript(scrnId) {
    if (scrnId === "http:" || scrnId === "https:") {
        return;
    }

    $("#" + scrnId).find("script").remove();
    $("#" + scrnId).find("link").remove();
}


