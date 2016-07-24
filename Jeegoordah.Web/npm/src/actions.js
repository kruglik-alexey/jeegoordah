import * as _ from 'lodash'
import {get} from './rest'

let actions = {
    data: {
        contextLoaded: '',
        currencyTotalsLoaded: '',
        loadCurrencyTotals(currency) {
            return (d, getState) => {
                if (getState().data.totals[currency]) {
                    return Promise.resolve();
                }
                return get(`total/${currency}`).then(totals => d({type: actions.data.currencyTotalsLoaded, currency, totals}));
            };
        },
        loadP2PTransactions() {
            return d => {
                return get('p2p').then(transactions => d({type: actions.data.p2pTransactionsLoaded, transactions}));
            };
        },
        p2pTransactionsLoaded: ''
    }
};

const setActionNames = (obj, prefix=null) => {
    return _.mapValues(obj, (v, k) => {
        const name = prefix ? `${prefix}.${k}` : k;
        if (_.isFunction(v)) {
            return v;
        }
        if (_.isString(v)) {
            return name;
        }
        if (_.isObject(v)) {
            return setActionNames(v, name);
        }
    });
};

actions = setActionNames(actions);

export default actions;