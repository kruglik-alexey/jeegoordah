import AppView from './views/app'
import ReactDOM from 'react-dom'
import React from 'react'
import domReady from 'domready'
import {get} from './rest'
import {Provider} from 'react-redux'
import actions from './actions'
import * as _ from 'lodash'
import store from './store'

const $domReady = new Promise(resolve => domReady(resolve));

store.dispatch(dispatch => {
    const $bros = get('bros');
    const $currencies = get('currencies');
    Promise.all([$bros, $currencies, $domReady]).then(([bros, currencies]) => {
        dispatch({type: actions.data.contextLoaded, bros, currencies});
        dispatch(actions.data.loadP2PTransactions());
        dispatch(actions.totalView.selectCurrency(_.find(currencies, {isBase: true}).id)).then(() => {
            ReactDOM.render(<Provider store={store}><AppView /></Provider>, document.getElementById('app-container'));
        });
    });
});