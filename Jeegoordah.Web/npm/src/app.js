import IndexView from './views/index'
import AppView from './views/app'
import ReactDOM from 'react-dom'
import React from 'react'
import domReady from 'domready'
import {get} from './rest'
import {Provider} from 'react-redux'
import actions from './actions'
import * as _ from 'lodash'
import store from './store'
import {Router, Route, hashHistory, IndexRoute} from 'react-router'
import {syncHistoryWithStore} from 'react-router-redux'
import CreateTransactionView from './views/createTransaction'

store.dispatch(d => {
    const $bros = get('bros');
    const $currencies = get('currencies');
    Promise.all([$bros, $currencies]).then(([bros, currencies]) => {
        d({type: actions.data.contextLoaded, bros, currencies});
        const $p2p = d(actions.data.loadP2PTransactions());
        const baseCurrency = _.find(currencies, {isBase: true}).id;
        const $total = d(actions.totalView.selectCurrency(baseCurrency));
        const $domReady = new Promise(resolve => domReady(resolve));

        Promise.all([$p2p, $total, $domReady]).then(() => {
            const history = syncHistoryWithStore(hashHistory, store);
            ReactDOM.render(
                <Provider store={store}>
                    <Router history={history}>
                        <Route path="/" component={AppView}>
                            <IndexRoute component={IndexView}/>
                            <Route path="createTransaction" component={CreateTransactionView}/>
                        </Route>
                    </Router>
                </Provider>,
                document.getElementById('app-container')
            );
        });
    });
});