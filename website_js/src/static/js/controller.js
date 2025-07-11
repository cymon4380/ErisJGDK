const pingInterval = 30;

var gameData = { },
moderator = false;

function getRoom() {
    let roomCode = $('#roomCode').val().toUpperCase();
    console.log(`Trying to get information about room ${roomCode}`);

    let gameNameText;

    $.ajax({
        url: `${getRoomURL}?code=${roomCode}`,
        type: 'GET',
        crossDomain: true,
        dataType: 'json',
        success: res => {
            gameData.roomData = res.body;
            gameNameText = res.body.appName;

            let enableButton = true;
            let buttonText = moderator ? 'Модерировать' : 'Играть';
            let button = moderator ? $('#moderateBtn') : $('#playBtn');

            if (moderator) {
                if (res.body.moderationEnabled)
                    enableButton = true;
                else {
                    enableButton = false;
                    buttonText = 'Модерация отключена';
                }
            } else {
                if (res.body.locked) {
                    buttonText = 'Игра уже началась';
                } else if (res.body.full) {
                    buttonText = 'Комната заполнена';
                }
            }

            if (enableButton)
                button.removeAttr('disabled');
            else
                button.attr('disabled', '');

            $('.gameName').text(gameNameText);
            button.text(buttonText);
        },
        error: (xhr) => {
            switch (xhr.status) {
                case 404:
                    gameNameText = 'Комната не найдена';
                    break;

                case 429:
                    gameNameText = 'Вы делаете слишком много запросов';
                    break;

                case 500:
                    gameNameText = 'Внутренняя ошибка сервера';
                    break;

                case 0:
                    gameNameText = 'Потеряно соединение с сервером';
                    break;

                default:
                    gameNameText = `Код ошибки: ${xhr.status}`;
                    break;
            }

            $('.gameName').text(gameNameText);
        }
    });
}

function play(enteredPassword) {
    let roomCode = $('#roomCode').val().toUpperCase(),
    name = $('#playerName').val().trim(),
    role = $('#joinAs').val() || 'player';
    
    if (name.length === 0)
        return swalError('Укажите имя');

    let oldName = getCookie('playerName');

    if (gameData.roomData.passwordRequired && role === 'player' && !enteredPassword) {
        let messageText = 'Чтобы зайти как игрок, нужно ввести 5-значный пароль.';
        if (gameData.roomData.audienceEnabled)
            messageText += ' Если у вас нет пароля, вы можете попробовать присоединиться к зрителям.';

        return Swal.fire({
            title: 'Требуется пароль',
            text: messageText,
            input: 'text',
            inputAttributes: {
                maxlength: 5,
                autocomplete: 'off'
            },
            confirmButtonText: 'Играть',
            preConfirm: password => {
                play(password);
            }
        });
    }

    setCookie('roomCode', roomCode);
    setCookie('playerName', name);

    let uuid = getCookie('playerId') || crypto.randomUUID();
    if (name !== oldName) {
        uuid = crypto.randomUUID();
        setCookie('playerId', uuid);
    }

    let wsURL = webSocketURL + `/play?code=${roomCode}&name=${encodeURIComponent(name)}&role=${role}`;
    if (enteredPassword)
        wsURL += `&password=${enteredPassword}`;
    if (uuid)
        wsURL += `&id=${uuid}`;

    $('#playBtn').attr('disabled', '');

    gameData.app = new WebSocket(wsURL);
    gameData.app.onopen = onWsOpen;
    gameData.app.onmessage = onWsMessage;
    gameData.app.onclose = onWsClose;
    gameData.app.onerror = onWsError;

    document.title = `ErisJGDK | ${gameData.roomData.appName}`;
}

function moderate() {
    let roomCode = $('#roomCode').val().toUpperCase(),
    password = $('#moderationPassword').val()

    setCookie('roomCode', roomCode);
    
    if (password.length === 0)
        return swalError('Укажите пароль');

    let wsURL = webSocketURL + `/moderate?code=${roomCode}&password=${password}`;

    $('#moderateBtn').attr('disabled', '');

    gameData.app = new WebSocket(wsURL);
    gameData.app.onopen = onWsOpen;
    gameData.app.onmessage = onWsMessage;
    gameData.app.onclose = onWsClose;
    gameData.app.onerror = onWsError;
}

function ping() {
    gameData.app.send(JSON.stringify({
        key: 'ping',
        val: null
    }));
}

function onWsOpen() {
    $('header').remove();
    $('.gameJoin').remove();
    $('footer').remove();

    if (pingInterval > 0)
        setInterval(ping, pingInterval * 1000);
}

function onWsMessage(e) {
    let data = JSON.parse(e.data);
    console.log(data);

    if (data.error) {
        gameData.error = data.error;
        gameData.errorTime = Date.now() / 1000;
        return;
    }

    if (moderator)
        handleModeratorData(data);
    else
        handlePlayerData(data);
}

function onWsClose() {
    let errors = {
        'Incorrect password': 'Неверный пароль',
        'Sorry, this name has already been taken': 'Данное имя уже занято',
        'The room is full': 'Комната заполнена',
        'Audience limit exceeded': 'Достигнуто максимальное число зрителей в комнате',
        'Audience is not enabled for this room': 'Зрители отключены для этой комнаты',
        'Moderation is not enabled for this room': 'Модерация отключена для этой комнаты',
        'You are being rate limited': 'Вы отправляете запросы слишком часто',
        'Room not found': 'Комната не найдена',
        'The room is locked': 'Игра уже началась'
    };

    $('#playBtn').removeAttr('disabled');
    $('#moderateBtn').removeAttr('disabled');

    if (gameData.error && Date.now() / 1000 - gameData.errorTime <= 20) {
        swalError(errors[gameData.error] || gameData.error).then(() => window.location.reload());
    } else {
        let description;
        if (gameData.kicked) {
            description = 'Вы были исключены модератором.'
            if (gameData.kickReason)
                description += `\nПричина: ${gameData.kickReason}`;
            else
                description += '\nПричина не указана.';
        }

        if (description)
            description = escapeHTML(description);

        Swal.fire('Отключено', description).then(() => window.location.reload());
    }
}

function onWsError() {
    swalError('Произошла ошибка при подключении');
}

function onPlayDataChange() {
    $('#playBtn').attr('disabled', '');
    $('#moderateBtn').attr('disabled', '');
    $('.gameName').text('');

    let roomCode = $('#roomCode').val().toUpperCase();
    if (roomCode.length < $('#roomCode').attr('maxlength'))
        return;

    getRoom(moderator);
}

function getUrlParameter(parameter) {
    let url = window.location.search.substring(1),
    vars = url.split('&');

    for (let i = 0; i < vars.length; i++) {
        let currentParam = vars[i].split('=');

        if (currentParam[0] === parameter)
            return decodeURIComponent(currentParam[1]) || true;
    }

    return false;
}

$('#roomCode').on('input', onPlayDataChange);
$('#joinAs').change(onPlayDataChange);

$('#playBtn').click(() => play());
$('#moderateBtn').click(() => moderate());
$(document).ready(() => {
    let urlCode = getUrlParameter('code');
    if (urlCode) {
        setCookie('roomCode', urlCode);
        window.location.href = window.location.href.split('?')[0];
    }

    let roomCode = getCookie('roomCode');
    if (roomCode && roomCode.length == $('#roomCode').attr('maxlength')) {
        $('#roomCode').val(roomCode);
        getRoom();
    }

    $('#playerName').val(getCookie('playerName'));
});