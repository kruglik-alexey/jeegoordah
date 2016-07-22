import React from 'react'
import Header from './header'
import $ from 'jquery'
import {connect} from 'react-redux'

class CreateTransactionView extends React.Component {
    componentDidMount() {
        $(this.refs.dateInput).datepicker({
            format: 'dd-mm-yyyy',
            autoclose: true,
            weekStart: 1
        });
    }

    componentWillUnmount() {
        $(this.refs.dateInput).datepicker('destroy');
    }

    render() {
        return (
            <div className="container">
                <Header>Create Transaction</Header>
                <form>
                    <div className="form-group">
                        <label>Date</label>
                        <input className="form-control" type="text" name="date" ref="dateInput"/>
                    </div>
                    <div className="form-group">
                        <label>Amount</label>
                        <div className="row">
                            <div className="col-sm-12">
                                <div className="input-group">
                                    <input className="form-control" type="text" name="amount"/>
                                    <span className="input-group-btn">
                                        {this.renderCurrencies(this.props.currencies)}
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                </form>
            </div>
        );
    }

    renderCurrencies(currencies) {
        return currencies.map(c => <label key={c.id}>{c.name}</label>);
    }
}

const stateToProps = state => {
    return {
        currencies: state.data.currencies
    }
};

export default connect(stateToProps)(CreateTransactionView)