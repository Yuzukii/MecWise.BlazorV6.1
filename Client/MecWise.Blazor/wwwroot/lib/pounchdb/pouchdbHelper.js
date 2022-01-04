"use strict";

var _pouchdbList = {};

function pouchdbList_new(dbName, remoteCouch) {
    _pouchdbList[dbName] = new pouchdbHelper(dbName, remoteCouch);
}

async function pouchdbList_destory(dbName) {
    return await _pouchdbList[dbName].destory();
}

async function pouchdbList_allDocs(dbName, jsonOption) {
    return await _pouchdbList[dbName].allDocs(jsonOption);
}

async function pouchdbList_post(dbName, jsonObj) {
    return await _pouchdbList[dbName].post(jsonObj);
}

async function pouchdbList_put(dbName, jsonObj) {
    return await _pouchdbList[dbName].put(jsonObj);
}

async function pouchdbList_remove(dbName, id) {
    return await _pouchdbList[dbName].remove(id);
}

async function pouchdbList_get(dbName, id) {
    return await _pouchdbList[dbName].get(id);
}

async function pouchdbList_cancelLiveChanges(dbName) {
    return await _pouchdbList[dbName].cancelLiveChanges();
}

async function pouchdbList_cancelLiveSync(dbName) {
    return await _pouchdbList[dbName].cancelLiveSync();
}

window.pouchdbList_dbChanges = (dbName, callBack, instance) => {
    _pouchdbList[dbName].changes(instance, callBack);
};

window.pouchdbList_dbSyncAsync = (dbName, callBack, instance) => {
    _pouchdbList[dbName].syncAsync(instance, callBack);
};

window.pouchdbList_dbRepliFromCouch = (dbName, callBack, instance) => {
    _pouchdbList[dbName].repliFromCouch(instance, callBack);
};


class pouchdbHelper {
    constructor(dbName, remoteCouch) {
        try {
            this.db = new PouchDB(dbName);
            this.remoteCouch = remoteCouch;
            this.liveChanges = null;
            this.liveSync = null;
            return true;
        } catch (err) {
            console.log(err);
            return false;
        }
    }

    async destory() {
        try {
            var response = await this.db.destroy();
            return true;
        } catch (err) {
            console.log(err);
            return false;
        }
    }

    async allDocs(jsonOption) {
        try {
            var option = JSON.parse(jsonOption);
            var result = await this.db.allDocs(option);
            return JSON.stringify(result);
        } catch (err) {
            console.log(err);
            return "";
        }
    }

    async put(jsonObj) {
        try {
            var jObj = JSON.parse(jsonObj);
            var doc = await this.db.get(jObj._id);
            jObj._rev = doc._rev;
            var response = await this.db.put(jObj);
            return true;
        } catch (err) {
            console.log(err);
            return false;
        }
    }

    async post(jsonObj) {
        try {
            var jObj = JSON.parse(jsonObj);
            var response = await this.db.post(jObj);
            return true;
        } catch (err) {
            console.log(err);
            return false;
        }
    }

    async remove(id) {
        try {
            var doc = await this.db.get(id);
            var response = await this.db.remove(doc);
            return true;
        } catch (err) {
            console.log(err);
            return false;
        }
    }

    async get(id) {
        try {
            var doc = await this.db.get(id);
            var jsonDoc = JSON.stringify(doc);
            return jsonDoc;
        } catch (err) {
            console.log(err);
            return "";
        }
    }

    
    async cancelLiveChanges() {
        try {
            this.liveChanges.cancel();
            return true;
        } catch (err) {
            console.log(err);
            return false;
        }
    }

    async cancelLiveSync() {
        try {
            this.liveSync.cancel();
            return true;
        } catch (err) {
            console.log(err);
            return false;
        }
    }

    async changes(instance, callBackFunctionName) {
        this.liveChanges = this.db.changes({
            since: 'now',
            live: true
        }).on('change', function (change) {
            // handle change
            var jsonChange = JSON.stringify(change);
            instance.invokeMethodAsync(callBackFunctionName, jsonChange);
        });
    }

    async syncAsync(instance, callBackFunctionName) {
        if (this.remoteCouch) {
            var opts = { live: true, retry: true };
            //db.replicate.to(remoteCouch, opts, syncError);
            //db.replicate.from(remoteCouch, opts, syncError);

            var dbhelper = this;
            dbhelper.db.replicate.from(dbhelper.remoteCouch).on('complete', function (info) {

                // replicate from couchdb done!
                dbhelper.liveSync = dbhelper.db.sync(dbhelper.remoteCouch, opts).on('change', function (change) {
                    // yo, something changed!
                    instance.invokeMethodAsync(callBackFunctionName, 'Active');

                }).on('paused', function (info) {
                    // replication was paused, usually because of a lost connection
                    instance.invokeMethodAsync(callBackFunctionName, 'Paused');

                }).on('active', function (info) {
                    // replication was resumed
                    instance.invokeMethodAsync(callBackFunctionName, 'Active');

                }).on('error', function (err) {
                    // totally unhandled error (shouldn't happen)
                    instance.invokeMethodAsync(callBackFunctionName, 'Error');

                });

            });
        }
    }

    async repliFromCouch(instance, callBackFunctionName) {
        if (this.remoteCouch) {
            var dbhelper = this;
            dbhelper.db.replicate.from(dbhelper.remoteCouch).on('complete', function (info) {
                // replicate from couchdb done!
                instance.invokeMethodAsync(callBackFunctionName, 'complete');
            }).on('error', function (err) {
                // log error and still callback with complete status
                console.log(err);
                instance.invokeMethodAsync(callBackFunctionName, 'complete');
            });

        }
    }
}


