import actions from '../actions'

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
        default: return state
    }
}