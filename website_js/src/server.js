const express = require('express');
const fs = require('fs');
const path = require('path');
const json_loader = require('./utils/json_loader.js');
const settings_builder = require('./utils/settings_builder.js');

const app = express();

app.set('view engine', 'ejs');
app.set('views', __dirname + json_loader.config.views_folder)
app.use('/static', express.static(path.join(__dirname, json_loader.config.static.folder)))

app.get('/', (_, res) => {
    res.render('index', settings_builder.generateVariables());
});

app.get('/moderator', (_, res) => {
    res.render('moderator', settings_builder.generateVariables(true));
});

app.get('/privacy', (_, res) => {
    res.render('privacy');
});

app.get('/github', (_, res) => {
    res.redirect(json_loader.config.github_url);
});

function listen() {
    let port = Number(process.env.PORT || json_loader.config.port);
    app.listen(port, () => {
        console.log(`Server is listening on port ${port}`);
    })
}


exports.listen = listen;