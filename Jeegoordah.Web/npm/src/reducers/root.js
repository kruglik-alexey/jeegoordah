import totalViewReducer from './totalView'
import dataReducer from './data'
import {combineReducers} from 'redux'

export default combineReducers({
    data: dataReducer,
    totalView: totalViewReducer
})