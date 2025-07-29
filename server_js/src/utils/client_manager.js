const enums = require('../models/enums.js');
const server = require('../models/server.js');
const room = require('../models/room.js');
const json_loader = require('./json_loader.js');
const player = require('../models/player.js');
const moderator = require('../models/moderator.js');


function activateRoom(ws, validationResult) {
    let _room = server.Server.getRoom(validationResult.id);

    if (!_room)
        throw new TypeError('Room not found');
    if (_room.ws)
        throw new TypeError('Room is already activated');

    _room.ws = ws;

    let appData = _room.appData;
    ws.send(JSON.stringify({
        ok: true,
        key: 'roomCreated',
        val: {
            code: _room.code,
            audienceEnabled: _room.audienceEnabled,
            minPlayers: appData.players.min,
            maxPlayers: appData.players.max,
            expires: _room.expires / 1000
        }
    }));

    return _room;
}

function createRoom(data) {
    let _room = new room.Room(null, server.Server.generateRoomCode(), data);
    server.Server.clients.push(_room);

    console.debug(`Room ${_room.code} has been created`);
    setTimeout(() => { server.Server.destroyClient(_room, enums.ConnectionClosedReason.roomDestroyed) },
                json_loader.config.rooms.lifetime * 1000);

    return _room;
}

function joinPlayer(ws, data) {
    let _room = server.Server.getRoom(data.code);

    if (!_room)
        throw new TypeError('Room not found');
    if (!_room.ws)
        throw new TypeError('The room is not activated');

    if (!Object.values(enums.PlayerRole).find(r => r === data.role))
        throw new TypeError('Invalid role');

    if (data.role === enums.PlayerRole.player) {
        if (_room.password && data.password != _room.password)
            throw new TypeError('Incorrect password');

        let playerToReconnect = _room.getPlayers(enums.PlayerRole.player).find(p => p.id === data.id);
        if (Object.keys(_room.kickedIds).find(x => x === data.id))
            throw new TypeError('You have been kicked by a moderator.'
                                + ` Reason: ${_room.kickedIds[data.id] || 'No reason provided'}`);

        if (playerToReconnect) {
            if (playerToReconnect.kicked)
                throw new TypeError('You have been kicked by a moderator.'
                                    + ` Reason: ${playerToReconnect.kickReason || 'No reason provided'}`);
            if (!playerToReconnect.disconnected)
                throw new TypeError('The player is not disconnected');

            playerToReconnect.reconnected = true;
            playerToReconnect.disconnected = false;
            playerToReconnect.ws = ws;

            ws.send(JSON.stringify({
                ok: true,
                key: 'welcome',
                val: {
                    name: playerToReconnect.name,
                    id: playerToReconnect.id,
                    role: playerToReconnect.role
                }
            }));

            _room.ws.send(JSON.stringify({
                key: 'playerConnected',
                val: {
                    name: playerToReconnect.name,
                    id: playerToReconnect.id,
                    role: playerToReconnect.role,
                    reconnect: true
                }
            }));
            
            return playerToReconnect;
        }

        if (_room.isFull())
            throw new TypeError('The room is full');
        if (_room.locked)
            throw new TypeError('The room is locked');
        if (_room.isNameTaken(data.name))
            throw new TypeError('Sorry, this name has already been taken');
    } else if (data.role === enums.PlayerRole.audience) {
        if (!_room.audienceEnabled)
            throw new TypeError('Audience is not enabled for this room');
        if (_room.isAudienceFull())
            throw new TypeError('Audience limit exceeded');
    }

    let _player = new player.Player(ws, data.role, data);
    _room.players.push(_player);
    server.Server.clients.push(_player);

    ws.send(JSON.stringify({
        ok: true,
        key: 'welcome',
        val: {
            name: _player.name,
            id: _player.id,
            role: _player.role
        }
    }));

    _room.ws.send(JSON.stringify({
        key: 'playerConnected',
        val: {
            name: _player.name,
            id: _player.id,
            role: _player.role,
            reconnect: false
        }
    }));

    if (_player.role === enums.PlayerRole.player) {
        _room.moderators.forEach(moderator => {
            moderator.ws.send(JSON.stringify({
                key: 'playerConnected',
                val: {
                    name: _player.name,
                    reconnect: false
                }
            }));
        });
    }

    return _player;
}

function joinModerator(ws, data) {
    let _room = server.Server.getRoom(data.code);

    if (!_room)
        throw new TypeError('Room not found');
    if (!_room.ws)
        throw new TypeError('The room is not activated');
    if (!_room.moderationEnabled)
        throw new TypeError('Moderation is not enabled for this room');
    if (data.password != _room.moderationPassword)
        throw new TypeError('Incorrect password');

    let _moderator = new moderator.Moderator(ws, data);
    _room.moderators.push(_moderator);
    server.Server.clients.push(_moderator);

    _room.ws.send(JSON.stringify({
        key: 'moderatorJoined',
        val: {
            id: _moderator.id
        }
    }));

    _moderator.ws.send(JSON.stringify({
        ok: true,
        key: 'welcome',
        val: {
            id: _moderator.id
        }
    }));

    return _moderator;
}

function onClose(ws) {
    let _client = server.Server.clients.find(c => c.ws === ws);
    if (!_client)
        return;

    if (_client instanceof player.Player) {
        let _room = server.Server.getRoom(_client.roomCode);
        if (!_room || !_room.ws)
            return server.Server.destroyClient(_client);
        if (!_room.locked && _client.kicked)
            return server.Server.destroyClient(_client);

        _client.disconnected = true;
        _room.ws.send(JSON.stringify({
            key: 'playerDisconnected',
            val: {
                id: _client.id,
                name: _client.name,
                role: _client.role,
                kicked: _client.kicked
            }
        }));

        if (_client.role === enums.PlayerRole.audience)
            server.Server.destroyClient(_client)
    } else {
        if (_client instanceof moderator.Moderator) {
            let _room = server.Server.getRoom(_client.roomCode);
            if (!_room || !_room.ws)
                return server.Server.destroyClient(_client);

            _room.moderators = _room.moderators.filter(m => m.id !== _client.id);

            _room.ws.send(JSON.stringify({
                key: 'moderatorDisconnected',
                val: {
                    id: _client.id,
                    moderatorCount: _room.moderators.length
                }
            }));
        }

        server.Server.destroyClient(_client);
    }
}


exports.activateRoom = activateRoom;
exports.createRoom = createRoom;
exports.joinPlayer = joinPlayer;
exports.joinModerator = joinModerator;
exports.onClose = onClose;