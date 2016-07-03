import actions from '../actions'

export default function(state = {selectedCurrency: null}, action) {
    switch (action.type) {
        case actions.context.loaded: {
            return {
                ...state,
                selectedCurrency: action.currencies[0].id
            }
        }
        case actions.totalView.selectCurrency: {
            return {
                ...state,
                selectedCurrency: action.currency
            }
        }
        default: return state
    }
}