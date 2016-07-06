import actions from '../actions'

export default function(state = {list: []}, action) {
    switch (action.type) {
        case actions.p2pTransactions.loaded: {
            return {
                ...state,
                list: action.transactions
            }
        }
        default: return state
    }
}
