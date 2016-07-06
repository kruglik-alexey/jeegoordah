import React from 'react'
import * as _ from 'lodash'
import {signClass, formatMoney} from '../../utils'
import classNames from 'classnames'

export default function(props) {
    const bros = props.bros.map(b => {
        const total = _.find(props.totals, {bro: b.id});
        const amountClasses = classNames({
            ['text-right']: true,
            [signClass(total.amount)]: true
        });
        return (
            <li key={b.id} className="list-group-item">
                <table style={{width: '100%'}}>
                    <tbody>
                    <tr>
                        <td>
                            <a href="#">{b.name}</a>
                        </td>
                        <td>
                            <div className={amountClasses}>
                                {formatMoney(total.amount, props.currency)}
                            </div>
                        </td>
                        </tr>
                    </tbody>
                </table>
            </li>
        );
    });

    return (
        <ul className="list-group">
            {bros}
        </ul>
    )
}
