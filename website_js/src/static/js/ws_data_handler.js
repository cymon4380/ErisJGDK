function handlePlayerData(data) {
    switch (data.key) {
        case 'welcome':
            setCookie('playerId', data.val.id);
            gameData.playerData = data.val;
            gameUI.init();
            break;

        case 'logo':
            gameUI.hideLogo();
            gameUI.showLogo();
            gameUI.hideInput();
            break;

        case 'input':
            gameUI.hideLogo();
            gameUI.hideInput();
            gameUI.showInput();
            gameUI.initInputs(data.val);
            break;

        case 'playerKicked':
            gameData.kicked = true;
            gameData.kickReason = data.val.reason;
            break;
    }
}

function handleModeratorData(data) {
    switch (data.key) {
        case 'welcome':
            moderatorUI.init();
            break;

        case 'moderateInput':
            moderatorUI.addInputCard(data.val);
            break;

        case 'dropInput':
            $(`.inputCard[data-input-id="${data.val.id}"]`).remove();

            if ($('.inputCard').length === 0)
                $('.waitingForInputs').show();
            break;

        case 'playerConnected':
            moderatorUI.addPlayer(data.val);
            break;

        case 'inputModerated':
            moderatorUI.modifyInputCard(data.val);
            break;

        case 'nameCensored':
            moderatorUI.modifyPlayer({
                name: data.val.name,
                nameCensored: true
            });
            break;

        case 'playerKicked':
            moderatorUI.modifyPlayer({
                name: data.val.name,
                kicked: true
            });
            break;

        case 'welcomeRestoreState':
            data.val.players.forEach(player => {
                moderatorUI.addPlayer(player);
            });
            
            data.val.inputs.forEach(input => {
                moderatorUI.addInputCard(input);
            });
            break;
    }
}