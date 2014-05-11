define(['$', '_', 'analytics', 'rest', 'helper', 'app-context', 'text!templates/total/module.html', 'text!templates/total/row.html'],
    function ($, _, analytics, rest, helper, context, moduleTemplate, rowTemplate) {
    var self = {        
        activate: function () {
            analytics.page('Total', '/total');
            self._renderModule();
            self._navigate('all');
        },

        _renderModule: function() {
            self.$module = $(moduleTemplate);
            $('#modules').empty().append(self.$module);
            self.$navs = self.$module.find('[data-role=nav]').find('li');
            self.$navs.click(function () {                
                self._navigate($(this).data('role'));
            });
        },

        _navigate: function (renderType) {            
            self.$navs.removeClass('active');
            self.$navs.filter('[data-role=' + renderType + ']').addClass('active');

            switch (renderType) {
                case 'all': {
                    rest.get('total').done(function (total) {                        
                        self._renderTotalWithCurrencies(total);
                    });
                    break;
                }
                case 'base': {
                    rest.get('total/base').done(function (total) {
                        self._renderTotalInBase(total);
                    });
                    break;
                }
            }
        },

        // expects {bro, [{currency, amount}]}
        _renderTotalWithCurrencies: function (total) {
            var list = self.$module.find('#totalList');
            list.find('li').remove();
            _.chain(context.bros).sortBy(function (bro) {
                return bro.Name;
            }).each(function (bro) {
                var broAmounts = self._getBroAmounts(bro, total);
                list.append(helper.template(rowTemplate, broAmounts));
            });
        },

        _renderTotalInBase: function(total) {            
            var baseCurrency = _.find(context.currencies, function(c) {
                return c.IsBase;
            });
            total = _.map(total, function(broTotal) {
                return {
                    Bro: broTotal.Bro,
                    Amounts: [
                        {
                            Currency: baseCurrency.Id,
                            Amount: broTotal.Amount
                        }
                    ]
                };
            });
            self._renderTotalWithCurrencies(total);
        },
        
        _getBroAmounts: function(bro, total) {
            var broTotal = _.find(total, function(t) {
                return t.Bro == bro.Id;
            });
            var amounts = _.chain(broTotal.Amounts).map(function (amount) {                
                var currency = self._getCurrency(amount.Currency);
                var rawAmount = helper.withAccuracy(amount.Amount, currency.Accuracy);                
                if (rawAmount >= 1 || rawAmount <= -1) {
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