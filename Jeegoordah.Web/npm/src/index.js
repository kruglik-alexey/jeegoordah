import 'babel-polyfill'
import 'bootstrap-loader'
import IndexView from './views/index'
import ReactDOM from 'react-dom'
import React from 'react'
import domready from 'domready'

domready(() => {
    ReactDOM.render(React.createElement(IndexView), document.getElementById('app-container'));
});