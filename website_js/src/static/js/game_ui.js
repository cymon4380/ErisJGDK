gameUI = { };

gameUI.init = () => {
    $('body').append(`<div class="app"></div>`);
    $('body').attr('class', gameData.roomData.appTag);
    $('.app').append(`<h1>${escapeHTML(gameData.playerData.name)}</h1>`);
    gameUI.showLogo();
};

gameUI.submitAll = () => {
    $('.submitBtn').attr('disabled', '');

    let inputs = { },
    error;

    $('.inputContainer').children().each(function() {
        if ($(this).prop('tagName') !== 'INPUT')
            return;

        let value = $(this).val();

        if ($(this).attr('type') === 'text') {
            if (!$(this).val().trim() && $(this).attr('required')) {
                error = 'Не все поля заполнены';
                return;
            }
        } else if ($(this).attr('type') === 'checkbox') {
            value = $(this).is(':checked');
        } else if ($(this).attr('type') === 'range') {
            value = Number(value);
        }

        inputs[$(this).attr('id')] = value;
    });

    if (error) {
        $('.submitBtn').removeAttr('disabled');
        return swalError(error);
    }

    gameData.app.send(JSON.stringify({
        key: 'input',
        val: inputs
    }));
}

gameUI.showLogo = () => {
    $('.app').append(`<div class="logo"></div>`);
};

gameUI.hideLogo = () => {
    $('.logo').remove();
};

gameUI.showInput = () => {
    let content = $(`<div class="container content"></div>`),
    prompt = $(`<p class="prompt"></p>`),
    inputContainer = $(`<div class="container inputContainer pb-4"></div>`);

    content.append(prompt);
    content.append(inputContainer);
    $('.app').append(content);
};

gameUI.hideInput = () => {
    $('.content').remove();
};

gameUI.initInputs = (data) => {
    let rows = [];

    let buttonsContainer = $(`<div class="container mx-auto col-lg-6 mb-3"></div>`);
    $('.prompt').text(data.prompt);

    Object.entries(data.inputs).forEach(entry => {
        let key = entry[0],
        value = entry[1],
        parent = $('.inputContainer'),
        input, label = { };
        

        switch (value.type) {
            case 'text':
                input = $(`<input>`);
                input.attr('type', 'text');
                input.attr('id', key);
                input.attr('name', key);

                input.attr('maxlength', value.maxLength || 100);
                input.attr('placeholder', value.placeholder);
                input.attr('class', value.class || 'col-lg-6 col-md-8 col-sm-8');
                input.attr('autocomplete', 'off');
                if (value.required)
                    input.attr('required', '');

                label.text = value.label;
                label.after = false;
                break;

            case 'checkbox':
                input = $(`<input>`);
                input.attr('type', 'checkbox');
                input.attr('id', key);
                input.attr('name', key);

                label.text = value.label;
                label.after = true;
                break;

            case 'range':
                input = $(`<input>`);
                input.attr('type', 'range');
                input.attr('id', key);
                input.attr('name', key);

                input.attr('class', 'form-range ' + (value.class || 'w-50'));
                input.attr('min', value.min || 0);
                input.attr('max', value.max || 100);
                input.attr('step', value.step || 1);

                input.attr('value', value.value || value.min);

                if (value.showValue) {
                    label.text = (value.value || value.min);
                    label.after = true;
                }
                break;

            case 'button':
                parent = buttonsContainer;

                input = $(`<button></button>`);
                input.attr('class', 'btn btn-lg choiceBtn');
                if (value.class)
                    input.attr('class', input.attr('class') + ' ' + value.class);

                input.text(value.text);

                if (value.image) {
                    let image = $('<img>');

                    image.attr('src', value.image.src);

                    if (value.image.alt)
                        image.attr('alt', value.image.alt);

                    if (value.image.width)
                        image.attr('width', value.image.width + 'px');
                    if (value.image.height)
                        image.attr('height', value.image.height + 'px');

                    input.append(image);
                }
                if (value.disabled)
                    input.attr('disabled', '');

                input.attr('data-key', value.key);
                input.attr('data-value', value.value);

                if (value.row) {
                    if (!value.image)
                        input.attr('class', input.attr('class') + ' w-100');

                    let row = rows[value.row - 1];
                    if (!row) {
                        row = $(`<div class="row justify-content-md-center"></div>`);
                        parent.append(row);
                        rows.push(row);
                    }

                    parent = $(`<div class="mx-auto col col-lg-4 col-md-3 col-sm-8"></div>`);
                    row.append(parent);
                }
                break;

            case 'label':
                input = $('<p></p>');
                let font = $('<font></font>');

                font.attr('color', value.color || '#f4f4f4');
                font.attr('size', value.size || 16);
                font.text(value.text);

                input.append(font);

                break;

            case 'image':
                input = $('<img>');

                input.attr('src', value.src);
                if (value.alt)
                    input.attr('alt', value.alt);

                if (value.width)
                    input.attr('width', value.width + 'px');
                if (value.height)
                    input.attr('height', value.height + 'px');

                input.attr('draggable', value.draggable || false);

                break;
        }

        parent.append(input);
        if (label.text) {
            let _label = $(`<label for="${key}">${escapeHTML(label.text)}</label>`);
            label.after ? input.after(_label) : input.before(_label);
        }

        parent.append('<br>');
    });

    $('.inputContainer').append(buttonsContainer);
    buttonsContainer.after('<br>');

    if (data.submitButton)
        $('.inputContainer').append(`<button class="btn btn-lg btn-primary mx-auto submitBtn">Отправить</button>`);

    gameUI.subscribeToEvents();
};


gameUI.subscribeToEvents = () => {
    $('.submitBtn').off('click');
    $('input[type="range"]').off('input change');
    $('.choiceBtn').off('click');

    $('.submitBtn').click(() => gameUI.submitAll());

    $('input[type="range"]').on('input change', function() {
        $(`label[for=${$(this).attr('name')}]`).text($(this).val());
    });

    $('.choiceBtn').click(function () {
        $('.choiceBtn').attr('disabled', '');

        let data = { };
        data[$(this).attr('data-key')] = $(this).attr('data-value');
    
        gameData.app.send(JSON.stringify({
            key: 'input',
            val: data
        }));
    });
};


gameUI.subscribeToEvents();