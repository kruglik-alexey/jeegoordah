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
    let broList;
    let rateLabel;
    if (selectedTotals) {
        const currency = _.find(props.currencies, {id: props.selectedCurrency});
        broList = <BroList currency={currency} bros={props.bros} totals={selectedTotals.totals}/>;
        if (!currency.isBase) {
            const baseCurrency = _.find(props.currencies, {isBase: true});
            rateLabel = (
                <p className="text-muted" style={{marginLeft: '5px'}}>
                    Rate to {baseCurrency.name} is {selectedTotals.rate.rate}
                </p>
            );
        }
    } else {
        broList = <Spinner />;
        rateLabel = null;
    }

    return (
        <div style={{width: '300px'}}>
            <Header title="Total"/>
            <CurrencySelector
                currencies={props.currencies}
                selectedCurrency={props.selectedCurrency}
                onSelectCurrency={selectCurrency}/>
            <p></p>
            {rateLabel}
            {broList}
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