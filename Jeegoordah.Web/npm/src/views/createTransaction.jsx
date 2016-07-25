import React from 'react'
import Header from './header'
import TransactionEditor from './transactionEditor'
import stubData from '../stubData'

export default props => {
    return (
        <div className="container">
            <Header>Create Transaction</Header>
            <TransactionEditor />
            <hr/>
            <div className="text-right">
                <button className="btn btn-primary">Save</button>
                <span> </span>
                <button className="btn btn-default">Cancel</button>
            </div>
        </div>
    );
}