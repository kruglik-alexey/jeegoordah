import IndexView from './views/index'
import ReactDOM from 'react-dom'
import React from 'react'
import domReady from 'domready'
import {get} from './rest'

const $ready = new Promise(resolve => domReady(resolve));
const $bros = get('bros');
const $currencies = get('currencies');

Promise.all([$bros, $currencies, $ready]).then(([bros, currencies]) => {
    ReactDOM.render(React.createElement(IndexView, {bros, currencies}), document.getElementById('app-container'));
});