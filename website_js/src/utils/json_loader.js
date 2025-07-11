const fs = require('fs');
const path = require('path');

const appDirectory = path.dirname(require.main.filename);
const config = JSON.parse(fs.readFileSync(path.join(appDirectory, 'config.json')));


exports.config = config;