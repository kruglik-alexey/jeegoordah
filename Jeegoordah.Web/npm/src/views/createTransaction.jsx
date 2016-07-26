import React from 'react'
import Header from './header'
import TransactionEditor, {createEmptyTransaction, validate} from './transactionEditor'
import {withRouter} from 'react-router'
import NotificationSystem from 'react-notification-system'
import {connect} from 'react-redux'
import actions from '../actions'
import Spinner from './spinner'

class CreateTransactionView extends React.Component {
    constructor(props) {
        super(props);
        this.state = {transaction: createEmptyTransaction()};
    }

    componentWillUnmount() {
        this.unmounted = true;
    }

    render() {
        const saveSpinner = this.state.saving ? <i className="fa fa-spinner fa-spin" style={{fontSize: `${24}px`}}></i> : null;
        return (
            <div className="container">
                <Header>Create Transaction</Header>
                <TransactionEditor transaction={this.state.transaction}
                                   onChange={t => this.setState({transaction: t})}/>
                <hr/>
                <div className="text-right">
                    {saveSpinner}
                    <button
                        className="btn btn-primary"
                        disabled={!!this.state.saving}
                        onClick={() => this.save()}>Save</button>
                    <span> </span>
                    <button className="btn btn-default" onClick={() => this.props.router.goBack()}>Cancel</button>
                </div>
                <NotificationSystem ref="notifications"/>
            </div>
        );
    }

    save() {
        const err = validate(this.state.transaction);
        if (err) {
            this.refs.notifications.addNotification({
                level: 'error',
                position: 'tc',
                message: 'Please enter ' + err
            });
        } else {
            this.setState({saving: true});
            this.props.dispatch(actions.data.createTransaction(this.state.transaction)).then(() => {
                if (!this.unmounted) {
                    this.props.router.goBack();
                }
            });
        }
    }
}

export default connect((state, ownProps) => ownProps)(withRouter(CreateTransactionView));