const express = require('express');
const fs = require('fs');
const path = require('path');
const package = require('../package.json');

const config = JSON.parse(fs.readFileSync(path.join(__dirname, 'config.json')));
const app = express();

app.set('view engine', 'ejs');
app.set('views', __dirname + config.views_folder)
app.use('/static', express.static(path.join(__dirname, config.static.folder)))

app.get('/', (_, res) => {
    res.render('index', {
        version: package.version,
        build_date: package.build_date
    });
});

app.get('/moderator', (_, res) => {
    res.render('moderator', {
        version: package.version,
        build_date: package.build_date
    });
});

app.get('/privacy', (_, res) => {
    res.render('privacy');
});

app.get('/github', (_, res) => {
    res.redirect(config.github_url);
});

function listen() {
    app.listen(config.port, () => {
        console.log(`Server is listening on port ${config.port}`);
    })
}


exports.listen = listen;