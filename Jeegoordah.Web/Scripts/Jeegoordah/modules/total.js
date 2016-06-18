define(['$', '_', 'analytics', 'rest', 'helper', 'app-context', 'text!templates/total/module.html', 'text!templates/total/row.html'],
    function ($, _, analytics, rest, helper, context, moduleTemplate, rowTemplate) {
    var self = {                
        activate: function () {
            analytics.page('Total', '/total');

            self.total = {};
            self.baseCurrency = _.findWhere(context.currencies, { IsBase: true });
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
                self._renderTotal(currency, total);
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
            _.chain(context.notHiddenBros).sortBy(function (bro) {
                return bro.Name;
            }).each(function (bro) {
                var broAmounts = self._getBroAmounts(bro, total);
                list.append(helper.template(rowTemplate, broAmounts));
            });
        },
                
        _renderTotal: function(currency, total) {            
            var totals = _.map(total.Totals, function(broTotal) {
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
            self._renderTotalWithCurrencies(totals);
            self._renderRate(total);
        },

        _renderRate: function(total) {
            var $rate = self.$module.find('#rate');
            if (total.Rate.Currency === self.baseCurrency.Id) {
                $rate.text('');
                $rate.hide();
            } else {
                var text = 'Rate to ' + self.baseCurrency.Name + ' is ' + total.Rate.Rate;
                var date = helper.parseDate(total.Rate.Date);                
                if (!helper.isToday(date)) {
                    text += ' (' + total.Rate.Date + ')';
                }                
                $rate.text(text);
                $rate.show();
            }
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