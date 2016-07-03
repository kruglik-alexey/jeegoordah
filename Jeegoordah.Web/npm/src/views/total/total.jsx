import React from 'react'
import {connect} from 'react-redux'
import Header from '../header'
import actions, {selectTotalViewCurrency} from '../../actions'
import CurrencySelector from './currencySelector'
import Spinner from '../spinner'
import BroList from './broList'
import * as _ from 'lodash'

const totalView = props => {
    const selectCurrency = id => props.dispatch(selectTotalViewCurrency(id));

    const selectedTotals = props.totals[props.selectedCurrency];
    const content = selectedTotals ?
        <BroList
            currency={_.find(props.currencies, {id: props.selectedCurrency})}
            bros={props.bros}
            totals={selectedTotals.totals}/> :
        <Spinner />;

    return (
        <div style={{width: '300px'}}>
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
        bros: state.context.bros,
        selectedCurrency: state.totalView.selectedCurrency,
        totals: state.totalView.totals
    };
};

export default connect(stateToProps)(totalView);