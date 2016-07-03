import actions from '../actions'

export default function(state = {selectedCurrency: null, totals: {}}, action) {
    switch (action.type) {
        case actions.totalView.selectCurrency: {
            return {
                ...state,
                selectedCurrency: action.currency
            }
        }
        case actions.totalView.totalsLoaded: {
            return {
                ...state,
                totals: {
                    ...state.totals,
                    [action.currency]: action.totals
                }
            }
        }
        default: return state
    }
}