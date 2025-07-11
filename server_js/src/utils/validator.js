const query = require('../utils/query.js');
const json_loader = require('../utils/json_loader.js');
const enums = require('../models/enums.js');


function validate(url, connectionType) {
    let password;

    switch (connectionType) {
        case enums.ConnectionType.play:
            let name = query.getQueryParameter('name', url),
            code = query.getQueryParameter('code', url),
            role = query.getQueryParameter('role', url),
            playerId = query.getQueryParameter('id', url);

            password = query.getQueryParameter('password', url);

            if (!password)
                password = null;

            if (name)
                name = name.trim();
            if (playerId)
                playerId = playerId.trim();

            if (!name || !code || !role || !playerId)
                throw new TypeError('Invalid form body');

            return {
                name: String(name).substring(0, json_loader.config.players.max_name_length),
                code: String(code).toUpperCase(),
                role: String(role),
                id: String(playerId),
                password: password ? String(password) : null
            };

        case enums.ConnectionType.createRoom:
            let roomId = query.getQueryParameter('id', url);

            if (!roomId)
                throw new TypeError('Invalid form body');

            return {
                id: String(roomId)
            };

        case enums.ConnectionType.moderate:
            let _code = query.getQueryParameter('code', url);
            password = query.getQueryParameter('password', url);

            if (!_code || !password)
                throw new TypeError('Invalid form body');

            return {
                code: String(_code).toUpperCase(),
                password: String(password)
            };

        case enums.ConnectionType.createRoomHttp:
            let appTag = url.appTag,
            audienceEnabled = String(url.audienceEnabled).toLowerCase() === 'true',
            moderationPassword = url.moderationPassword;
            password = url.password;
            
            if (!appTag)
                throw new TypeError('Invalid form body');

            return {
                appTag: appTag,
                appData: json_loader.getApp(appTag),
                audienceEnabled: audienceEnabled,
                moderationPassword: moderationPassword || null,
                password: password || null
            };

        case enums.ConnectionType.findRoom:
            let roomCode = query.getQueryParameter('code', url);

            if (!roomCode)
                throw new TypeError('Invalid form body');

            return {
                code: roomCode
            }

        default:
            throw new TypeError('Invalid connection type');
    }
}


exports.validate = validate;