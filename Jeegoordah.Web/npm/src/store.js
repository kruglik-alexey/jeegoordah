import rootReducer from './reducers/root'
import {createStore, applyMiddleware, compose} from 'redux'
import thunk from 'redux-thunk'

const devTools = window.devToolsExtension ? window.devToolsExtension() : f => f;
const store = createStore(rootReducer, compose(applyMiddleware(thunk), devTools));
export default store;