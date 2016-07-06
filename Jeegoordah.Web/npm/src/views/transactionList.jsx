import React from 'react'
import {formatMoney} from '../utils'
import * as _ from 'lodash'

export default function(props) {
    const baseCurrency = _.find(props.currencies, {isBase: true});
    const rows = props.transactions.map(t => {
        const currency = _.find(props.currencies, {id: t.currency});
        const source = _.find(props.bros, {id: t.source});
        let targets = props.bros
            .filter(b => t.targets.includes(b.id))
            .map(b => <span key={b.id}>{b.name}</span>);
        targets = targets.reduce((acc, e, i) => {
            acc.push(e);
            if (i < targets.length - 1) {
                acc.push(' ');
            }
            return acc;
        }, []);
        return (
            <tr key={t.id}>
                <td className="auto-width">{t.date}</td>
                <td className="auto-width text-right">
                    <span title={formatMoney(t.amount / t.rate, baseCurrency)}>
                        {formatMoney(t.amount, currency)}
                    </span>
                </td>
                <td className="auto-width">{source.name}</td>
                <td className="auto-width">{targets}</td>
                <td>{t.comment}</td>
            </tr>
        );
    });

    return (
        <table className="table">
            <thead>
            <tr>
                <th>Date</th>
                <th>Amount</th>
                <th>Source</th>
                <th>Targets</th>
                <th>Comment</th>
            </tr>
            </thead>
            <tbody>
                {rows}
            </tbody>
        </table>
    );
}