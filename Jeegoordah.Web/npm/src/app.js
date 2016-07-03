import AppView from './views/app'
import ReactDOM from 'react-dom'
import React from 'react'
import domReady from 'domready'
import {get} from './rest'
import {createStore} from 'redux'
import {Provider} from 'react-redux'
import rootReducer from './reducers/root'
import actions from './actions'

const $bros = get('bros');
const $currencies = get('currencies');
const store = createStore(rootReducer, {});

domReady(() => {
    Promise.all([$bros, $currencies]).then(([bros, currencies]) => {
        store.dispatch({type: actions.contextLoaded, bros, currencies});
        ReactDOM.render(<Provider store={store}><AppView /></Provider>, document.getElementById('app-container'));
    });
});