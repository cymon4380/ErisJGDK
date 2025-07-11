const json_loader = require('./json_loader.js');
const connection = json_loader.config.connection || {};
const package = require('../../package.json');

function generateBaseURL(websocket) {
    let protocol = websocket ? 'ws' : 'http';
    if (connection.secure)
        protocol += 's';
    
    let parts = [
        protocol, '://',
        process.env.SERVER_HOST || connection.host || '127.0.0.1',
        ':', Number(process.env.SERVER_PORT || connection.port || 2096),
        websocket ? connection.websocket_suffix || '' : ''
    ]

    return parts.join('');
}

function generateSettings(moderator) {
    return {
        moderator: Boolean(moderator),
        get_room_url: generateBaseURL(false) + (connection.get_room_url || '/getRoom'),
        websocket_url: generateBaseURL(true)
    };
}

function generateVersionInfo() {
    return {
        version: package.version,
        build_date: package.build_date
    }
}

function generateVariables(moderator) {
    return Object.assign({}, generateSettings(moderator), generateVersionInfo());
}


exports.generateBaseURL = generateBaseURL;
exports.generateSettings = generateSettings;
exports.generateVersionInfo = generateVersionInfo;
exports.generateVariables = generateVariables;