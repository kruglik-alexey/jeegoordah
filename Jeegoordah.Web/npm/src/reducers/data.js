import actions from '../actions'
import _ from 'lodash'

export default function(state = {bros: [], currencies: [], totals: {}, p2pTransactions: [], rates: {}}, action) {
    switch (action.type) {
        case actions.data.contextLoaded: {
            return {
                ...state,
                bros: action.bros,
                currencies: action.currencies
            }
        }
        case actions.data.currencyTotalsLoaded: {
            const totals = {
                ...state.totals,
                [action.currency]: action.totals
            };
            return {
                ...state,
                totals
            }
        }
        case actions.data.p2pTransactionsLoaded: {
            return {
                ...state,
                p2pTransactions: action.transactions
            }
        }
        case actions.data.dateRatesLoaded: {
            const rates = {
                ...state.rates,
                [action.date]: action.rates
            };
            return {
                ...state,
                rates
            }
        }
        case actions.data.transactionCreated: {
            const p2p = _.concat(action.transaction, state.p2pTransactions);
            return {
                ...state,
                p2pTransactions: p2p
            };
        }
        default: return state
    }
}