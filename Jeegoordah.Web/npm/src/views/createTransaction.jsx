import 'bootstrap-datepicker'
import '../../libs/jquery.number'
import React from 'react'
import Header from './header'
import $ from 'jquery'
import {connect} from 'react-redux'
import classNames from 'classnames'

class CreateTransactionView extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            selectedCurrency: null
        };
        this.currencyClick = this.currencyClick.bind(this);
    }

    componentDidMount() {
        $(this.refs.dateInput).datepicker({
            format: 'dd-mm-yyyy',
            autoclose: true,
            weekStart: 1
        });
        $(this.refs.amountInput).number(true, 0, '.', ' ');
    }

    componentWillUnmount() {
        $(this.refs.dateInput).datepicker('destroy');
    }

    render() {
        return (
            <div className="container">
                <Header>Create Transaction</Header>
                <form>
                    {this.renderDate()}
                    {this.renderAmount()}
                </form>
            </div>
        );
    }

    renderDate() {
        return (
            <div className="form-group">
                <label>Date</label>
                <input className="form-control" type="text" name="date" ref="dateInput"/>
            </div>
        );
    }

    renderAmount() {
        const currencies = this.props.currencies.map(c => {
            const cx = {
                btn: true,
                'btn-default': true,
                active: this.state.selectedCurrency === c.id
            };
            return (
                <label key={c.id} className={classNames(cx)} onClick={() => this.currencyClick(c.id)}>
                    {c.name}
                </label>
            );
        });
        
        return (
            <div className="form-group">
                <label>Amount</label>
                <div className="row">
                    <div className="col-sm-12">
                        <div className="input-group">
                            <input className="form-control" type="text" name="amount" ref="amountInput"/>
                            <span className="input-group-btn">
                                {currencies}
                            </span>
                        </div>
                    </div>
                </div>
            </div>
        );
    }

    currencyClick(currency) {
        this.setState({selectedCurrency: currency});
    }
}

const stateToProps = state => {
    return {
        currencies: state.data.currencies
    }
};

export default connect(stateToProps)(CreateTransactionView)