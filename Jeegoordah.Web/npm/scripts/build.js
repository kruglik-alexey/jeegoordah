var shell = require('shelljs');

shell.rm('-rf', './build');
shell.mkdir('build');
shell.cp('./src/index.html', './build/index.html');
shell.echo('Starting webpack');
shell.exec('cross-env NODE_ENV=development webpack');
