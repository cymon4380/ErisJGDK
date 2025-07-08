const fs = require('fs');
const path = require('path');

const appDirectory = path.dirname(require.main.filename);
const config = JSON.parse(fs.readFileSync(path.join(appDirectory, 'json/config.json')));
const apps = JSON.parse(fs.readFileSync(path.join(appDirectory, 'json/apps.json'))).apps;


function getApp(appTag) {
    if (!(appTag in apps))
        throw new TypeError('App not found');

    return apps[appTag];
}

exports.config = config;
exports.apps = apps;
exports.getApp = getApp;