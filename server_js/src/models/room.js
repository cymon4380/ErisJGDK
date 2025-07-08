const json_loader = require('../utils/json_loader.js');
const client = require('../models/client.js')
const enums = require('../models/enums.js');


class Room extends client.Client {
    constructor(ws, code, data) {
        super(ws);
        this.code = code.toUpperCase();
        this.appTag = data.appTag;
        this.appData = json_loader.getApp(data.appTag);
        this.audienceEnabled = Boolean(String(data.audienceEnabled).toLowerCase() == 'true' && this.appData.audience_enabled);
        this.password = data.password;
        this.moderationPassword = data.moderationPassword;
        this.moderationEnabled = Boolean(data.moderationPassword && this.appData.moderation_enabled);        
        this.players = [];
        this.moderators = [];
        this.kickedIds = {};
        this.locked = false;
        this.createdAt = Date.now();
        this.expires = Date.now() + json_loader.config.rooms.lifetime * 1000;
    }

    getPlayers(playerRole) {
        if (!playerRole)
            return this.players;

        return this.players.filter(p => p.role == playerRole);
    }

    isFull() {
        return this.getPlayers(enums.PlayerRole.player).length >= this.appData.players.max;
    }

    isAudienceFull() {
        return this.getPlayers(enums.PlayerRole.audience).length >= json_loader.config.rooms.audience_limit;
    }

    isNameTaken(name) {
        return this.getPlayers(enums.PlayerRole.player)
                .filter(p => p.name.toLowerCase() === name.toLowerCase()).length > 0;
    }

    kickPlayer(playerId, reason) {
        let player = this.players.find(p => p.id === playerId);
        if (!player)
            throw new TypeError('Player not found');
        if (player.role !== enums.PlayerRole.player)
            throw new TypeError('You cannot kick an audience player');
        if (player.kicked || Object.keys(this.kickedIds).find(x => x === playerId))
            throw new TypeError('This player has already been kicked');

        player.kicked = true;
        player.kickReason = reason;
        this.kickedIds[playerId] = reason;

        this.ws.send(JSON.stringify({
            key: 'playerKicked',
            val: {
                id: player.id,
                reason: reason
            }
        }));

        player.ws.send(JSON.stringify({
            key: 'playerKicked',
            val: {
                reason: reason
            }
        }));
        player.ws.close();
    }

    censorPlayerName(playerId) {
        let player = this.players.find(p => p.id === playerId);
        if (!player)
            throw new TypeError('Player not found');
        if (player.role !== enums.PlayerRole.player)
            throw new TypeError('You cannot censor name of an audience player');
        if (player.nameCensored)
            throw new TypeError('This player\'s name has already been censored');

        player.nameCensored = true;
        this.ws.send(JSON.stringify({
            key: 'nameCensored',
            val: {
                id: player.id,
                name: player.name
            }
        }));
    }
}


exports.Room = Room;