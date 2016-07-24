import React from 'react'
import {connect} from 'react-redux'
import Header from '../header'
import actions from '../../actions'
import CurrencySelector from './currencySelector'
import Spinner from '../spinner'
import BroList from './broList'
import * as _ from 'lodash'

class TotalView extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            selectedCurrency: _.find(props.currencies, {isBase: true}).id
        };
        this.selectCurrency = this.selectCurrency.bind(this);
    }

    render() {
        const props = this.props;
        const selectedCurrency = this.state.selectedCurrency;
        const selectedTotals = props.totals[selectedCurrency];
        let broList;
        let rateLabel;

        if (selectedTotals) {
            const currency = _.find(props.currencies, {id: selectedCurrency});
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
            <div>
                <Header>Total</Header>
                <CurrencySelector
                    currencies={props.currencies}
                    selectedCurrency={selectedCurrency}
                    onSelectCurrency={this.selectCurrency}/>
                <p></p>
                {rateLabel}
                {broList}
            </div>
        );
    }

    selectCurrency(currency) {
        if (!this.props.totals[currency]) {
            this.props.dispatch(actions.data.loadCurrencyTotals(currency));
        }
        this.setState({selectedCurrency: currency});
    }
}

const stateToProps = state => {
    return {
        currencies: state.data.currencies,
        bros: state.data.bros,
        totals: state.data.totals,
        //selectedCurrency: state.totalView.selectedCurrency
    };
};

export default connect(stateToProps)(TotalView);