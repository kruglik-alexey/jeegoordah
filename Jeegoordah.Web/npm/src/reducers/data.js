import actions from '../actions'

export default function(state = {bros: [], currencies: [], totals: {}, p2pTransactions: []}, action) {
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
        default: return state
    }
}