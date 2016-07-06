import AppView from './views/app'
import ReactDOM from 'react-dom'
import React from 'react'
import domReady from 'domready'
import {get} from './rest'
import {createStore, applyMiddleware, compose} from 'redux'
import thunk from 'redux-thunk'
import {Provider} from 'react-redux'
import rootReducer from './reducers/root'
import actions, {selectTotalViewCurrency, loadP2PTransactions} from './actions'
import * as _ from 'lodash'

const $domReady = new Promise(resolve => domReady(resolve));

const store = createStore(rootReducer, compose(applyMiddleware(thunk), window.devToolsExtension ? window.devToolsExtension() : f => f));

store.dispatch(dispatch => {
    const $bros = get('bros');
    const $currencies = get('currencies');
    Promise.all([$bros, $currencies, $domReady]).then(([bros, currencies]) => {
        dispatch({type: actions.context.loaded, bros, currencies});
        dispatch(loadP2PTransactions());
        dispatch(selectTotalViewCurrency(_.find(currencies, {isBase: true}).id)).then(() => {
            ReactDOM.render(<Provider store={store}><AppView /></Provider>, document.getElementById('app-container'));
        });
    });
});