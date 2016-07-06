import * as _ from 'lodash'
import {get} from './rest'

const names = [
    'context.load',
    'context.loaded',
    'totalView.selectCurrency',
    'totalView.totalsLoaded',
    'p2pTransactions.loaded'
];

const actions = names.reduce((acc, n) => {
    return _.set(acc, n ,n);
}, {});

export function selectTotalViewCurrency(currency) {
    return dispatch => {
        dispatch({type: actions.totalView.selectCurrency, currency});
        return get(`total/${currency}`).then(totals => dispatch({type: actions.totalView.totalsLoaded, currency, totals}));
    };
}

export function loadP2PTransactions() {
    return dispatch => {
        return get('p2p').then(transactions => dispatch({type: actions.p2pTransactions.loaded, transactions}));
    };
}

export default actions;