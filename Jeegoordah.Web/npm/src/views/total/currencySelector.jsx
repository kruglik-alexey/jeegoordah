import React from 'react'
import classNames from 'classnames'
import * as _ from 'lodash'

const visibleCurrencies = ['USD', 'EUR', 'BYR', 'BYN'];

export default function(props) {
    const currencies = visibleCurrencies
        .map(c => _.find(props.currencies, {name: c}))
        .map(c => {
            const cs = classNames({active: c.id === props.selectedCurrency});
            return (
                <li key={c.id} className={cs}>
                    <a onClick={() => props.onSelectCurrency(c.id)}>{c.name}</a>
                </li>
            );
        });

    return (
        <ul className="nav nav-pills">
            {currencies}
        </ul>
    );
}