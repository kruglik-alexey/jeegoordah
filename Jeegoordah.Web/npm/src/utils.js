import accounting from 'accounting'

export function signClass(num) {
    if (num > 0) {
        return 'positive';
    }
    if (num < 0) {
        return 'negative';
    }
    return 'zero';
}

export function formatMoney(amount, currency) {
    let fixedAmount = Math.round(amount);
    if (currency.accuracy > 0) {
        const factor = Math.pow(10, currency.accuracy);
        fixedAmount = Math.round(fixedAmount / factor) * factor;
    }
    const num = accounting.formatNumber(fixedAmount, 0, ' ');
    return `${num} ${currency.name}`;
}

export function formatDate(date) {
    let month = date.getMonth() + 1;
    if (month < 10) {
        month = '0' + month;
    }
    return `${date.getDate()}-${month}-${date.getFullYear()}`;
}