import React from 'react'
import {connect} from 'react-redux'
import Header from '../header'
import actions, {selectTotalViewCurrency} from '../../actions'
import CurrencySelector from './currencySelector'
import Spinner from '../spinner'

const totalView = props => {
    const selectCurrency = id => props.dispatch(selectTotalViewCurrency(id));

    const selectedTotals = props.totals[props.selectedCurrency];
    const content = selectedTotals ? JSON.stringify(selectedTotals) : <Spinner />;

    return (
        <div>
            <Header title="Total"/>
            <CurrencySelector
                currencies={props.currencies}
                selectedCurrency={props.selectedCurrency}
                onSelectCurrency={selectCurrency}/>
            {content}
        </div>
    );
};

const stateToProps = state => {
    return {
        currencies: state.context.currencies,
        selectedCurrency: state.totalView.selectedCurrency,
        totals: state.totalView.totals
    };
};

export default connect(stateToProps)(totalView);