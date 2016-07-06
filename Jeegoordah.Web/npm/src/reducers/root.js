import totalViewReducer from './totalView'
import contextReducer from './context'
import p2pTransactionsReducer from './p2pTransactions'
import {combineReducers} from 'redux'

export default combineReducers({
    context: contextReducer,
    totalView: totalViewReducer,
    p2pTransactions: p2pTransactionsReducer
})