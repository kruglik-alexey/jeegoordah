define(['$', '_', 'rest', 'text!templates/total/module.html', 'text!templates/total/row.html'], function ($, _, rest, moduleTemplate, rowTemplate) {
    var self = {        
        activate: function () {
            $.when(rest.get('total'),
                   rest.get('bros'),
                   rest.get('currencies'))
            .done(function (total, bros, currencies) {
                $('#modules').empty().append($(moduleTemplate));
                self._render(total[0], bros[0], currencies[0]);                            
            });
        },
        
        _render: function (total, bros, currencies) {
            var list = $('#totalList');
            _.chain(bros).sortBy(function (bro) {
                return bro.Name;
            }).each(function (bro) {
                var broAmounts = self._getBroAmounts(bro, total, currencies);
                list.append($($.jqote(rowTemplate, broAmounts)));
            });
        },
        
        _getBroAmounts: function(bro, total, currencies) {
            var broTotal = _.find(total, function(t) {
                return t.Bro == bro.Id;
            });
            var amounts = _.map(broTotal.Amounts, function(amount) {
                return {
                    amount: $.number(amount.Amount, 0, '.', ' '),
                    rawAmount: Math.floor(amount.Amount),
                    currency: self._getCurrency(amount.Currency, currencies)
                };
            });
            return {
                bro: bro,
                amounts: amounts
            };
        },
        
        _getCurrency: function(currency, currencies) {
            return _.find(currencies, function(c) {
                return c.Id == currency;
            });
        }
    };
    return self;
});