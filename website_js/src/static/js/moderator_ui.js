moderatorUI = { };

const mainContainer = $(`<div class="container d-flex flex-column justify-content-center col-lg-5 col-md-6 mt-5 moderation"></div>`),
buttonsHeader = $(`<div class="d-flex flex-row justify-content-center mb-5"></div>`),
playersCollapse = $(`<div class="collapse col-7 mx-auto" id="playersCollapse"></div>`),
inputsContainer = $(`<div class="container col-7 mx-auto"></div>`);

const elements = [
    {
        type: 'button',
        attributes: {
            class: 'btn btn-primary mx-2',
            id: 'managePlayersBtn',
            'data-bs-toggle': 'collapse',
            'data-bs-target': '#' + playersCollapse.attr('id'),
            'aria-expanded': false,
            'aria-controls': playersCollapse.attr('id')
        },
        text: 'Управление игроками',
        parent: buttonsHeader
    },
    {
        type: 'p',
        attributes: {
            class: 'text-center text-muted waitingForInputs'
        },
        text: 'Пока нечего модерировать...',
        parent: mainContainer
    }
];


moderatorUI.createElements = (_elements) => {
    let createdElements = [];

    (_elements || elements).forEach(element => {
        let _element = $(`<${element.type}></${element.type}>`);
        Object.entries(element.attributes).forEach(attr => _element.attr(attr[0], attr[1]));

        if (element.text)
            _element.text(element.text);

        let parent = (typeof element.parent === 'string' ? $(element.parent) : element.parent);
        parent.append(_element);
        createdElements.push(_element);
    });

    return createdElements;
};

moderatorUI.init = () => {
    $('body').append(mainContainer);
    mainContainer.append(buttonsHeader);
    mainContainer.append(playersCollapse);
    mainContainer.append(inputsContainer);

    moderatorUI.createElements();
    playersCollapse.after('<hr>');
};

moderatorUI.addInputCard = (inputData) => {
    $('.waitingForInputs').hide();
    let card = null;

    if ($(`.inputCard[data-input-id="${inputData.id}"]`).length == 0) {
        card = moderatorUI.createElements([
            {
                type: 'div',
                attributes: {
                    class: 'card card-body text-start inputCard my-2',
                    'data-input-id': inputData.id
                },
                parent: inputsContainer
            }
        ])[0];

        let mainRow = $(`<div class="row"></div>`),
        contentColumn = $(`<div class="col float-start h-100 text-lg-start text-md-center text-sm-center"></div>`),
        controlsColumn = $(`<div class="col float-end d-flex flex-column"></div>`);

        card.append(mainRow);
        mainRow.append(contentColumn);
        mainRow.append(controlsColumn);

        contentColumn.append(`
            <div class="row">
                <p>${escapeHTML(inputData.content)}</p>
            </div>
            <div class="row">
                <small class="text-muted">Автор: ${escapeHTML(inputData.playerName)}</small>
            </div>
        `);

        moderatorUI.createElements([
            {
                type: 'button',
                attributes: {
                    class: 'btn btn-sm btn-success my-1 moderateInputBtn',
                    'data-input-id': inputData.id,
                    'data-value': 'approved'
                },
                text: 'Одобрить',
                parent: controlsColumn
            },
            {
                type: 'button',
                attributes: {
                    class: 'btn btn-sm btn-danger my-1 moderateInputBtn',
                    'data-input-id': inputData.id,
                    'data-value': 'rejected'
                },
                text: 'Отклонить',
                parent: controlsColumn
            }
        ]);
    }

    moderatorUI.modifyInputCard(inputData, card);
    moderatorUI.updateListeners();
};

moderatorUI.addPlayer = (playerData) => {
    let card = null;

    if ($(`.playerCard[data-player-name="${playerData.name}"]`).length == 0) {
        card = moderatorUI.createElements([
            {
                type: 'div',
                attributes: {
                    class: 'card card-body text-start playerCard my-2',
                    'data-player-name': playerData.name
                },
                parent: playersCollapse
            }
        ])[0];

        let mainRow = $(`<div class="row"></div>`),
        contentColumn = $(`<div class="col float-start h-100 text-lg-start text-md-center text-sm-center"></div>`),
        controlsColumn = $(`<div class="col float-end d-flex flex-column"></div>`);

        card.append(mainRow);
        mainRow.append(contentColumn);
        mainRow.append(controlsColumn);

        contentColumn.append(`<p class="align-middle">${escapeHTML(playerData.name)}</p>`);

        moderatorUI.createElements([
            {
                type: 'button',
                attributes: {
                    class: 'btn btn-sm btn-primary my-1 censorNameBtn',
                    'data-player-name': playerData.name
                },
                text: 'Скрыть имя',
                parent: controlsColumn
            },
            {
                type: 'button',
                attributes: {
                    class: 'btn btn-sm btn-danger my-1 kickBtn',
                    'data-player-name': playerData.name
                },
                text: 'Исключить',
                parent: controlsColumn
            }
        ]);
    }

    moderatorUI.modifyPlayer(playerData);
    moderatorUI.updateListeners();
};

moderatorUI.modifyInputCard = (inputData, card) => {
    let _card = card || $(`.inputCard[data-input-id="${inputData.id}"]`),
    buttons = [
        $(`.moderateInputBtn[data-input-id="${inputData.id}"][data-value="approved"]`),
        $(`.moderateInputBtn[data-input-id="${inputData.id}"][data-value="rejected"]`)
    ];

    buttons[0].removeAttr('disabled');
    buttons[1].removeAttr('disabled');

    if (inputData.status === 'approved') {
        _card.attr('class', _card.attr('class').replace(' bg-rejected', '') + ' bg-approved');
        buttons[0].attr('disabled', '');
    } else if (inputData.status === 'rejected') {
        _card.attr('class', _card.attr('class').replace(' bg-approved', '') + ' bg-rejected');
        buttons[1].attr('disabled', '');
    }
};

moderatorUI.modifyPlayer = (playerData) => {
    buttons = [
        $(`.censorNameBtn[data-player-name="${playerData.name}"]`),
        $(`.kickBtn[data-player-name="${playerData.name}"]`)
    ];

    if (playerData.nameCensored !== undefined) {
        buttons[0].removeAttr('disabled');
        buttons[0].text('Скрыть имя');
    }

    if (playerData.kicked !== undefined) {
        buttons[1].removeAttr('disabled');
        buttons[1].text('Исключить');
    }

    if (playerData.nameCensored) {
        buttons[0].attr('disabled', '');
        buttons[0].text('Имя скрыто');
    } 
    
    if (playerData.kicked) {
        buttons[1].attr('disabled', '');
        buttons[1].text('Исключён');
    }
};

moderatorUI.updateListeners = () => {
    $('.moderateInputBtn').off('click');
    $('.censorNameBtn').off('click');
    $('.kickBtn').off('click');

    $('.moderateInputBtn').click(function() {
        moderatorUI.moderateInput($(this).attr('data-input-id'), $(this).attr('data-value'));
    });

    $('.censorNameBtn').click(function() {
        gameData.app.send(JSON.stringify({
            key: 'censorName',
            val: {
                name: $(this).attr('data-player-name')
            }
        }));
    });

    $('.kickBtn').click(function() {
        moderatorUI.kickPlayer($(this).attr('data-player-name'));
    });
};

moderatorUI.moderateInput = (inputID, status) => {
    gameData.app.send(JSON.stringify({
        key: 'moderateInput',
        val: {
            id: inputID,
            status: status
        }
    }));

    console.log(`Input moderated: ${inputID} (${status})`);
}

moderatorUI.kickPlayer = (playerName) => {
    Swal.fire({
        title: `Исключить игрока ${playerName}?`,
        text: 'Вы можете указать причину исключения игрока.',
        input: 'text',
        inputAttributes: {
            maxlength: 100,
            autocomplete: 'off'
        },
        confirmButtonText: 'Исключить',
        preConfirm: reason => {
            gameData.app.send(JSON.stringify({
                key: 'kickPlayer',
                val: {
                    name: playerName,
                    reason: reason.trim()
                }
            }));
        }
    });
};