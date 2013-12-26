define(['$', '_', 'analytics', 'rest', 'helper', 'app-context', 'text!templates/total/module.html', 'text!templates/total/row.html'],
    function ($, _, analytics, rest, helper, context, moduleTemplate, rowTemplate) {
    var self = {        
        activate: function () {
            analytics.page('Total', '/total');
            rest.get('total').done(function (total) {
                self.module = $(moduleTemplate);
                $('#modules').empty().append(self.module);
                self._render(total);                            
            });
        },
        
        _render: function (total) {
            var list = self.module.find('#totalList');
            _.chain(context.bros).sortBy(function (bro) {
                return bro.Name;
            }).each(function (bro) {
                var broAmounts = self._getBroAmounts(bro, total);
                list.append(helper.template(rowTemplate, broAmounts));
            });
        },
        
        _getBroAmounts: function(bro, total) {
            var broTotal = _.find(total, function(t) {
                return t.Bro == bro.Id;
            });
            var amounts = _.chain(broTotal.Amounts).map(function (amount) {
                var currency = self._getCurrency(amount.Currency);
                var rawAmount = helper.withAccuracy(amount.Amount, currency.Accuracy);
                if (rawAmount !== 0) {
                    return {
                        amount: helper.formatNumber(rawAmount),
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
        
        _getCurrency: function(currency) {
            return _.find(context.currencies, function(c) {
                return c.Id == currency;
            });
        }
    };
    return self;
});