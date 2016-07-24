import dataReducer from './data'
import {combineReducers} from 'redux'
import {routerReducer} from 'react-router-redux'

export default combineReducers({
    data: dataReducer,
    routing: routerReducer
})