/********* Start of batch file upload ***********/

async function _clearFileUpload(scrnId) {
    var inputFile = $('#_fileUpld');
    inputFile.remove();
}

async function _showFileOpenDialog(scrnId) {

    var inputFile = $('#_fileUpld');

    if (!inputFile.length) {
        inputFile = $('<input type="file" id="_fileUpld" name="filename" style="display: none;">');

        inputFile.change(async function () {

            if (inputFile.prop('files').length === 0) {
                return;
            }

            var file = inputFile.prop('files')[0];

            SetFieldValueAsync("_SEL_FILE", file.name, scrnId);
            SetFieldValueAsync("_FILE_TYPE", file.type, scrnId);
            SetFieldValueAsync("_FILE_SIZE", file.size, scrnId);
            var ext = file.name.split('.').pop();
            SetFieldValueAsync("_FILE_EXT", ext, scrnId);

        });


        $('#' + scrnId).append(inputFile);
    }

    inputFile.trigger('click');

}


async function _uploadFile(tokenType, token, postData, scrnId) {
    var result = false;
    var inputFile = $('#_fileUpld');
    var file = inputFile.prop('files')[0];

    const toBase64 = file => new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => resolve(reader.result);
        reader.onerror = error => reject(error);
    });

    const fileString = await toBase64(file).catch(e => Error(e));
    if (fileString instanceof Error) {
        console.log('Error: ', fileString.message);
        return;
    }

    postData = JSON.parse(postData);
    postData.base64FileString = fileString.split(",")[1];


    var apiUrl;
    $.ajax({
        async: false,
        type: "GET",
        url: "config.json",
        success: function (config, textStatus, jqXHR) {
            if (textStatus === "success") {
                apiUrl = config.apiUrl;
            }
        }
    });

    $.ajax({
        async: false,
        type: "POST",
        url: apiUrl + "/File/Upload",
        headers: { "Authorization": tokenType + " " + token },
        data: JSON.stringify(postData),
        success: function (data, textStatus, jqXHR) {
            if (textStatus === "success") {
                SetFieldValueAsync("_SEL_FILE", "", scrnId);
                SetFieldValueAsync("_FILE_TYPE", "", scrnId);
                SetFieldValueAsync("_FILE_SIZE", "", scrnId);
                SetFieldValueAsync("_FILE_EXT", "", scrnId);
                SetFieldValueAsync("_FRIENDLY_NAME", "", scrnId);
                SetFieldValueAsync("_FILE_REMARKS", "", scrnId);
                _clearFileUpload();
                result = true;
            }
            else {
                result = false;
            }
        },
        error: function () {
            result = false;
        }
    });


    return result;
}

async function _downloadFile(apiUrl, tokenType, token, postData) {
    var result = false;

    postData = JSON.parse(postData);

    $.ajax({
        async: true,
        type: "POST",
        url: apiUrl,
        headers: { "Authorization": tokenType + " " + token },
        data: JSON.stringify(postData),
        success: function (data, textStatus, jqXHR) {
            if (textStatus === "success") {

                const b64toBlob = (b64Data, contentType = '', sliceSize = 512) => {
                    const byteCharacters = atob(b64Data);
                    const byteArrays = [];

                    for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
                        const slice = byteCharacters.slice(offset, offset + sliceSize);

                        const byteNumbers = new Array(slice.length);
                        for (let i = 0; i < slice.length; i++) {
                            byteNumbers[i] = slice.charCodeAt(i);
                        }

                        const byteArray = new Uint8Array(byteNumbers);
                        byteArrays.push(byteArray);
                    }

                    const blob = new Blob(byteArrays, { type: contentType });
                    return blob;
                }

                const blob = b64toBlob(data["base64FileString"], data["FILE_TYPE"]);
                const blobUrl = URL.createObjectURL(blob);

                if (data["UTILITY_DOWNLOAD"]) {
                    var fileName = data["FILE_NAME"];
                }
                else {
                    var fileName = data["FRIENDLY_NAME"];
                    var extension = "." + data["FILE_EXT"];
                    fileName = fileName.replace(extension, "");
                    fileName = fileName + extension;
                }
                

                var anchor = document.createElement('a');
                anchor.href = blobUrl;
                anchor.target = '_blank';
                anchor.download = fileName;
                anchor.click();

                result = true;
            }
            else {
                result = false;
            }
        },
        error: function () {
            result = false;
        }
    });

    return result;
}


//async function _utltDownloadFile(apiUrl, tokenType, token, fileName) {
//    var result = false;


//    $.ajax({
//        async: true,
//        type: "GET",
//        url: apiUrl,
//        headers: { "Authorization": tokenType + " " + token },
//        success: function (data, textStatus, jqXHR) {
//            if (textStatus === "success") {

//                var blob = new Blob([data]);
//                var link = document.createElement('a');
//                link.href = window.URL.createObjectURL(blob);
//                link.download = fileName;
//                link.click();

//                result = true;
//            }
//            else {
//                result = false;
//            }
//        },
//        error: function () {
//            result = false;
//        }
//    });

//    return result;
//}
/********* End of batch file upload ***********/



/********* Start of report file download ***********/

async function _downloadReport(base64FileString, fileName, extension, previewEnable) {

    const b64toBlob = (b64Data, contentType = '', sliceSize = 512) => {
        const byteCharacters = atob(b64Data);
        const byteArrays = [];

        for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
            const slice = byteCharacters.slice(offset, offset + sliceSize);

            const byteNumbers = new Array(slice.length);
            for (let i = 0; i < slice.length; i++) {
                byteNumbers[i] = slice.charCodeAt(i);
            }

            const byteArray = new Uint8Array(byteNumbers);
            byteArrays.push(byteArray);
        }

        const blob = new Blob(byteArrays, { type: contentType });
        return blob;
    }

    var contentType = "";
    if (extension == "pdf") {
        contentType = "application/pdf";
    }

    const blob = b64toBlob(base64FileString, contentType);
    const blobUrl = URL.createObjectURL(blob);

    extension = "." + extension;
    fileName = fileName.replace(extension, "");
    fileName = fileName + extension;

    if (previewEnable == 1 && extension == ".pdf") {
        window.open(blobUrl, "_blank");
    }
    else {
        var anchor = document.createElement('a');
        anchor.href = blobUrl;
        anchor.target = '_blank';
        anchor.download = fileName;
        anchor.click();
    }

}

/********* End of report file download ***********/


/********* Start of direct upload ***********/

async function _directUploadFile(tokenType, token, postData) {
    var inputFile = $('#_fileUpld');
    var file = inputFile.prop('files')[0];

    const toBase64 = file => new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = () => resolve(reader.result);
        reader.onerror = error => reject(error);
    });

    const fileString = await toBase64(file).catch(e => Error(e));
    if (fileString instanceof Error) {
        console.log('Error: ', fileString.message);
        return;
    }

    var base64FileString = fileString.split(",")[1];
    return await _directUpload(tokenType, token, postData, base64FileString)
}

async function _directUpload(tokenType, token, postData, base64FileString) {
    var uploadFileName = "";

    postData = JSON.parse(postData);
    postData.base64FileString = base64FileString;

    var apiUrl;
    $.ajax({
        async: false,
        type: "GET",
        url: "config.json",
        success: function (config, textStatus, jqXHR) {
            if (textStatus === "success") {
                apiUrl = config.apiUrl;
            }
        }
    });

    $.ajax({
        async: false,
        type: "POST",
        url: apiUrl + "/File/DirectUpload",
        headers: { "Authorization": tokenType + " " + token },
        data: JSON.stringify(postData),
        success: function (data, textStatus, jqXHR) {
            if (textStatus === "success") {
                _clearFileUpload();
                uploadFileName = data.UPLOAD_FILE_NAME;
            }
            else {
                uploadFileName = "";
            }
        },
        error: function () {
            uploadFileName = "";
        }
    });


    return uploadFileName;
}

/********* End of direct upload ***********/




/********* Start of photo upload (take photo and upload) ***********/

function _renderVideoElements(scrnId) {
    $("#" + getFieldId("EpfSubContainer3")).css("background-color", "black");
    $("#" + getFieldId("_mediaContainer")).css("background-color", "black");

    var _video = $("<video id='_video' style='width:100%; height:auto; object-fit:scale-down; margin:auto;' autoplay loop muted playsinline ></video>")
    _video.on("loadeddata ", function () {
        _resizePopupIframe();
    });
    var _canvas = $("<canvas id='_canvas' style='width:100%; height:auto; object-fit:scale-down; margin:auto;'></canvas>")
    _canvas.addClass("d-none");

    $("#" + getFieldId("_mediaContainer")).append(_video);
    $("#" + getFieldId("_mediaContainer")).append(_canvas);
}

function _getUserMedia(constraints) {
    // if Promise-based API is available, use it
    if (navigator.mediaDevices) {
        return navigator.mediaDevices.getUserMedia(constraints);
    }

    // otherwise try falling back to old, possibly prefixed API...
    var legacyApi = navigator.getUserMedia || navigator.webkitGetUserMedia ||
        navigator.mozGetUserMedia || navigator.msGetUserMedia;

    if (legacyApi) {
        // ...and promisify it
        return new Promise(function (resolve, reject) {
            legacyApi.bind(navigator)(constraints, resolve, reject);
        });
    }
}

var _theVideoStream;
function _getVideoStream(cameraFacingMode) {

    if (!navigator.mediaDevices && !navigator.getUserMedia && !navigator.webkitGetUserMedia &&
        !navigator.mozGetUserMedia && !navigator.msGetUserMedia) {
        alert('User Media API not supported.');
        return;
    }

    var constraints = {
        video: {
            width: {
                min: 320,
                max: 640,
            },
            height: {
                min: 240,
                max: 480,
            },
            facingMode: cameraFacingMode
        }
    }

    _getUserMedia(constraints)
        .then(function (stream) {
            var _video = document.getElementById('_video');

            if ('srcObject' in _video) {
                _video.srcObject = stream;
            } else if (navigator.mozGetUserMedia) {
                _video.mozSrcObject = stream;
            } else {
                _video.src = (window.URL || window.webkitURL).createObjectURL(stream);
            }
            _theVideoStream = stream;
            _video.play();
        })
        .catch(function (err) {
            alert('Error: ' + err);
        });


    $(_canvas).addClass("d-none");
    $(_video).removeClass("d-none");

}

function _captureImage() {
    var _canvas = document.getElementById('_canvas');
    var _video = document.getElementById('_video');
    _canvas.width = _video.videoWidth;
    _canvas.height = _video.videoHeight;
    _canvas.getContext('2d').drawImage(_video, 0, 0, _video.videoWidth, _video.videoHeight);
    _canvas.toBlob(function (blob) {
        const img = new Image();
        img.src = URL.createObjectURL(blob);
    }, 'image/jpg');


    $(_canvas).removeClass("d-none");
    $(_video).addClass("d-none");


    _stopVideoStream();
}

function _stopVideoStream() {
    if (_theVideoStream) {
        _theVideoStream.getTracks().forEach(function (track) {
            if (track.readyState == 'live') {
                track.stop();
            }
        });
    }
}

async function _directUploadPhoto(tokenType, token, postData) {
    var _canvas = document.getElementById('_canvas');
    var base64FileString = _canvas.toDataURL("image/jpeg");
    base64FileString = base64FileString.replace("data:image/jpeg;base64,", "");
    return await _directUpload(tokenType, token, postData, base64FileString);
}


/********* End of photo upload (take photo and upload) ***********/