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
                        Last P2P Transactions
                        <span> </span>
                        <button type="button" className="btn btn-primary" onClick={() => props.router.push('/createTransaction')}>New Transaction</button>
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

const stateToProps = (state, ownProps) => {
    return {
        router: ownProps.router,
        currencies: state.data.currencies,
        bros: state.data.bros,
        transactions: state.data.p2pTransactions
    };
};

export default connect(stateToProps)(P2PList)