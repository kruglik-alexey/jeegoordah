define(['$', '_', 'analytics', 'rest', 'helper', 'app-context', 'text!templates/total/module.html', 'text!templates/total/row.html'],
    function ($, _, analytics, rest, helper, context, moduleTemplate, rowTemplate) {
    var self = {                
        activate: function () {
            analytics.page('Total', '/total');

            self.total = {};
            self._renderModule();
            self._navigate('usd');
        },

        _renderModule: function() {
            self.$module = $(moduleTemplate);
            $('#modules').empty().append(self.$module);
            self.$currencies = self.$module.find('[data-role=currencies]').find('li');
            self.$currencies.click(function () {
                self._navigate($(this).data('currency'));
            });
        },

        _navigate: function (currencyName) {
            analytics.page('Total/' + currencyName, '/total/' + currencyName);
            self.$currencies.removeClass('active');
            self.$currencies.filter('[data-currency=' + currencyName + ']').addClass('active');
            var currency = self._getCurrencyByName(currencyName);
            self._getTotal(currency.Id).done(function (total) {
                _renderTotal(currency, total);
            });            
        },

        _getTotal: function (currencyId) {
            if (!self.total[currencyId]) {
                self.total[currencyId] = rest.get('total/' + currencyId).promise();
            }
            return self.total[currencyId];
        },

        // expects [{bro, [{currency, amount}]}]
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
        
        // expects [{bro, amount}]
        _renderTotal: function(currency, total) {            
            total = _.map(total, function(broTotal) {
                return {
                    bro: broTotal.Bro,
                    amounts: [
                        {
                            currency: currency,
                            amount: broTotal.Amount
                        }
                    ]
                };
            });
            self._renderTotalWithCurrencies(total);
        },
        
        _getBroAmounts: function(bro, total) {
            var broTotal = _.find(total, function(t) {
                return t.bro == bro.Id;
            });
            var amounts = _.chain(broTotal.amounts).map(function (amount) {                                
                var rawAmount = helper.withAccuracy(amount.amount, amount.currency.Accuracy);
                if (rawAmount >= 1 || rawAmount <= -1) {
                    return {
                        amount: helper.formatNumber(rawAmount),
                        rawAmount: rawAmount,
                        currency: amount.currency
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
        
        _getCurrencyByName: function (currencyName) {
            currencyName = currencyName.toLowerCase();
            return _.find(context.currencies, function(c) {
                return c.Name.toLowerCase() == currencyName;
            });
        }
    };
    return self;
});