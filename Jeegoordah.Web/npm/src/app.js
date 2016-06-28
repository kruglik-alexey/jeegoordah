import IndexView from './views/index'
import ReactDOM from 'react-dom'
import React from 'react'
import domReady from 'domready'
import {get} from './rest'

const $ready = new Promise(resolve => domReady(resolve));
const $bros = get('bros');
const $currencies = get('currencies');
const $total = $currencies.then(cs => {
   return get(`total/${cs[0].id}`);
});

Promise.all([$bros, $currencies, $total, $ready]).then(([bros, currencies, total]) => {
    ReactDOM.render(React.createElement(IndexView, {bros, currencies, total}), document.getElementById('app-container'));
});