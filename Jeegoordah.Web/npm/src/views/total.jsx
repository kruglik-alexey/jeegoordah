import React from 'react'
import {connect} from 'react-redux'
import Header from './header'
import classNames from 'classnames'
import actions from '../actions'

const totalView = props => {
    const selectCurrency = id => props.dispatch({type: actions.total.selectCurrency, currency: id});

    const currencies = props.currencies.map(c => {
        const cs = classNames({active: c.id === props.selectedCurrency});
        return (
            <li key={c.id} className={cs}>
                <a href="#" onClick={() => selectCurrency(c.id)}>{c.name}</a>
            </li>
        );
    });
    
    return (
        <div>
            <Header title="Total"/>
            <ul className="nav nav-pills">
                {currencies}
            </ul>
        </div>
    );
};

const stateToProps = state => {
    return {
        currencies: state.currencies,
        selectedCurrency: state.totalView.selectedCurrency
    };
};

export default connect(stateToProps)(totalView);