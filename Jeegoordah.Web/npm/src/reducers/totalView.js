import actions from '../actions'

export default function(state = {selectedCurrency: null}, action) {
    switch (action.type) {
        case actions.totalView.setSelectedCurrency: {
            return {
                ...state,
                selectedCurrency: action.currency
            }
        }
        default: return state
    }
}