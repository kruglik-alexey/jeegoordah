import React from 'react'
import Header from './header'

export default function (props) {
    return (
        <div className="container">
            <Header>Create Transaction</Header>
            <form>
                <div className="form-group">
                    <label>Date</label>
                </div>
            </form>
        </div>
    );
}