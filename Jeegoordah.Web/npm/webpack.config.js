module.exports = {
    entry: './src/index.js',
    output: {
        path: './build',
        filename: 'jeegoordah.js'
    },
    module: {
        loaders: [
            {
                test: /\.js$/,
                exclude: /node_modules/,
                loader: 'babel-loader'
            }
        ]
    },
    devtool: "#inline-source-map"
};