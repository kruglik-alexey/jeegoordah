export default {
    bros: [
        {id: 0, name: 'Bro1'},
        {id: 1, name: 'Bro2'}
    ],
    currencies: [
        {id: 0, name: 'USD', accuracy: 0, isBase: true},
        {id: 1, name: 'BYR', accuracy: 3, isBase: false}
    ],
    'total/0': {
        rate: {
            currency: 0, date: '28-06-2016', rate: 1
        },
        totals: [
            {bro: 0, amount: 42},
            {bro: 1, amount: 9000}
        ]
    },
    'total/1': {
        rate: {
            currency: 1, date: '25-06-2016', rate: 20000
        },
        totals: [
            {bro: 0, amount: 840000},
            {bro: 1, amount: 180000000}
        ]
    }
}
