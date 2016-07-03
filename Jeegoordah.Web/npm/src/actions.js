import _ from 'lodash'

const names = [
    'context.loaded',
    'totalView.selectCurrency'
];

const actions = names.reduce((acc, n) => {
    return _.set(acc, n ,n);
}, {});

export default actions;