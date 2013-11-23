define(['$', '_', 'rest', 'helper', 'text!templates/total/module.html', 'text!templates/total/row.html'], function ($, _, rest, helper, moduleTemplate, rowTemplate) {
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
                list.append(helper.template(rowTemplate, broAmounts));
            });
        },
        
        _getBroAmounts: function(bro, total, currencies) {
            var broTotal = _.find(total, function(t) {
                return t.Bro == bro.Id;
            });
            var amounts = _.chain(broTotal.Amounts).map(function (amount) {
                var currency = self._getCurrency(amount.Currency, currencies);
                var rawAmount = helper.withAccuracy(amount.Amount, currency.Accuracy);
                if (rawAmount !== 0) {
                    return {
                        amount: $.number(rawAmount, 0, '.', ' '),
                        rawAmount: rawAmount,
                        currency: currency
                    };
                } else {
                    return null;
                }                
            }).filter(function(i) { return i !== null; }).value();
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