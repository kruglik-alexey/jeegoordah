import actions from '../actions'

export default function(state = {bros: [], currencies: []}, action) {
    switch (action.type) {
        case actions.context.loaded: {
            return {
                ...state,
                bros: action.bros,
                currencies: action.currencies
            }
        }
        default: return state
    }
}