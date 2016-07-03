import actions from '../actions'

export default function root(state, action) {
    switch (action.type) {
        case actions.contextLoaded: {
            const totalView = {
                ...state.totalView,
                selectedCurrency: action.currencies[0].id
            };
            return {
                ...state,
                bros: action.bros,
                currencies: action.currencies,
                totalView
            };
        }
        case actions.total.selectCurrency: {
            return {
                ...state,
                totalView: {
                    selectedCurrency: action.currency
                }
            }
        }
        default: return state
    }
}