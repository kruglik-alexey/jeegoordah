var shell = require('shelljs');

shell.rm('-rf', './build');
shell.mkdir('build');
shell.cp('./src/index.html', './build/index.html');
shell.exec('webpack-dev-server');