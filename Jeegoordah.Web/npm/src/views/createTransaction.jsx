import React from 'react'
import Header from './header'
import $ from 'jquery'

export default class extends React.Component {
    componentDidMount() {
        $(this.refs.dateInput).datepicker({
            format: 'dd-mm-yyyy',
            autoclose: true
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
                        <input className="form-control" type="text" name="date" ref="dateInput" value="01-01-2000"/>
                    </div>
                </form>
            </div>
        );
    }
}