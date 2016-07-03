import totalViewReducer from './totalView'
import contextReducer from './context'
import {combineReducers} from 'redux'

export default combineReducers({
    context: contextReducer,
    totalView: totalViewReducer
})