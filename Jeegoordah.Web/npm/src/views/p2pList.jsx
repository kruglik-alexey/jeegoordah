import React from 'react'
import {connect} from 'react-redux'
import Spinner from './spinner'
import TransactionList from './transactionList'
import * as _ from 'lodash'
import Header from './header'

const P2PList = props => {
    if (props.transactions) {
        return (
            <div>
                <Header>
                    <span>
                        Last P2P Transactions&nbsp;
                        <button type="button" className="btn btn-primary">New Transaction</button>
                    </span>
                </Header>
                <TransactionList
                    bros={props.bros}
                    currencies={props.currencies}
                    transactions={_.take(props.transactions, 20)}/>
            </div>
        );
    } else {
        return <Spinner />;
    }
};

const stateToProps = state => {
    return {
        currencies: state.context.currencies,
        bros: state.context.bros,
        transactions: state.p2pTransactions.list
    };
};

export default connect(stateToProps)(P2PList)