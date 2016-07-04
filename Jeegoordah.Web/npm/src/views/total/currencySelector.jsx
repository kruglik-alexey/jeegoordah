import React from 'react'
import classNames from 'classnames'

export default function(props) {
    const currencies = props.currencies.map(c => {
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