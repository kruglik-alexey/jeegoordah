import AppView from './views/app'
import ReactDOM from 'react-dom'
import React from 'react'
import domReady from 'domready'
import {get} from './rest'
import {createStore, applyMiddleware} from 'redux'
import thunk from 'redux-thunk'
import {Provider} from 'react-redux'
import rootReducer from './reducers/root'
import actions, {selectTotalViewCurrency} from './actions'

const $domReady = new Promise(resolve => domReady(resolve));

const store = createStore(rootReducer, applyMiddleware(thunk));
store.dispatch(dispatch => {
    const $bros = get('bros');
    const $currencies = get('currencies');
    Promise.all([$bros, $currencies, $domReady]).then(([bros, currencies]) => {
        dispatch({type: actions.context.loaded, bros, currencies});
        dispatch(selectTotalViewCurrency(currencies[0].id)).then(() => {
            ReactDOM.render(<Provider store={store}><AppView /></Provider>, document.getElementById('app-container'));
        });
    });
});