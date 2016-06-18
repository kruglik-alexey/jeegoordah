define(['_', '$', 'helper', 'app-context', 'notification', 'entityEditor', 'broSelector', 'rest', 'exchangeRates', 'text!templates/transactions/transactionEditor.html'],
function (_, $, helper, context, notification, editor, broSelector, rest, exchangeRates, editorTemplate) {
    var self = {
        init: function(currencies, bros) {
            self.currencies = currencies;
            self.visibleCurrencies = _.filter(self.currencies, function(c) {
                return !c.IsHidden;
            });
            self.hiddenCurrencies = _.filter(self.currencies, function(c) {
                return c.IsHidden;
            });
            self.bros = bros;
        },

        createTransaction: function(defaults) {
            defaults = _.defaults(defaults, { Date: helper.dateToString(new Date()) });

            var deferred = self._showTransactionEditor(defaults, 'Create Transaction', function(transaction) {
                transaction = _.defaults(transaction, { Event: defaults.Event });
                rest.post('transactions/create', transaction).done(function(createdTransaction) {
                    editor.close();
                    notification.success('Transaction created');
                    deferred.resolve(createdTransaction);
                }).fail(deferred.reject);
            });
            return deferred.promise();
        },

        editTransaction: function(transaction) {
            var deferred = self._showTransactionEditor(transaction, 'Edit Transaction', function(updatedTransaction) {
                updatedTransaction = _.defaults(updatedTransaction, { Id: transaction.Id, Event: transaction.Event });
                rest.post('transactions/update', updatedTransaction).done(function() {
                    editor.close();
                    notification.success('Transaction updated');
                    deferred.resolve(updatedTransaction);
                }).fail(deferred.reject);
            });
            return deferred.promise();
        },

        _showTransactionEditor: function(transaction, title, ok) {
            var deferred = $.Deferred();
            var currencies = self._getCurrenciesForTransaction(transaction);
            var bros = self._getBrosForTransaction(transaction);
            var baseCurrencyName = _.findWhere(self.currencies, { IsBase: true }).Name;

            self.$rendered = helper.template(editorTemplate, { currencies: currencies, baseCurrencyName: baseCurrencyName });
            self.$rate = self.$rendered.find('input[name=Rate]');
            self.$date = self.$rendered.find('input[name=Date]');
            self.$amount = self.$rendered.find('input[name=Amount]');
            self.$amountInBase = self.$rendered.find('input[name=AmountInBase]');

            self.$rendered.find('#transactionSource').append(broSelector.render(true, bros));
            self.$rendered.find('#transactionTargets').append(broSelector.render(false, bros));
            
            self.$amount.number(true, 0, '.', ' ');
            self._subscribeOnInputChanges(self.$amount, self._updateAmountInBase);
            self._subscribeOnInputChanges(self.$rate, self._rateChanged);
            self._subscribeOnInputChanges(self.$date, self._updateRate);            

            self.$rendered.find('#transactionCurrencies>label').click(self._updateRate);            

            editor.show(self.$rendered, transaction, title, {
                ok: function(modifiedTransaction) {
                    ok(modifiedTransaction, deferred);
                },
                cancel: deferred.reject,
                toForm: self._bindTransaction,
                fromForm: self._unbindTransaction,
                validate: self._validateTransaction
            });
            return deferred;
        },

        _subscribeOnInputChanges: function($input, handler) {
            $input.change(handler);
            $input.keyup(handler);
            $input.on('paste', handler);
        },

        _updateRate: function () {
            _.defer(function() {                                
                self.$rate.val('');
                self._getRate().done(function (rate) {                    
                    self.$rate.val(rate.Rate);
                    self._updateAmountInBase();
                    self._updateRateDate(true, rate.Date);
                });
            });           
        },

        _rateChanged: function() {
            self._updateAmountInBase();
            self._updateRateDate(false);
        },

        _updateRateDate: function(display, date) {
            var $el = self.$rendered.find('#rateDate');
            $el.text('');
            if (!display) {
                return;
            }
            var dateObj = helper.parseDate(date);
            display = !helper.isToday(dateObj);
            if (display) {                
                $el.text(date);
            }
        },

        _updateAmountInBase: function () {
            var rate = self.$rate.val();            
            var amount = self.$amount.val();
            self.$amountInBase.val($.number(amount / rate, 2, '.', ' '));
        },        

        _getRate: function () {
            var deferred = $.Deferred();
            var date = self.$date.val();
            var currency = self._getSelectedCurrency();
            if (currency) {
                exchangeRates.get(date).done(function (rates) {
                    var rate = _.findWhere(rates, { Currency: currency });
                    if (rate) {
                        deferred.resolve(rate);
                    } else {
                        deferred.reject();
                    }
                });
            } else {                
                deferred.reject();
            }            
            return deferred.promise();
        },

        _getCurrenciesForTransaction: function(transaction) {            
            var hidden = _.find(self.hiddenCurrencies, function(c) {
                return c.Id === transaction.Currency;
            });
            return hidden ? self.currencies : self.visibleCurrencies;
        },

        _getBrosForTransaction: function (transaction) {
            if (context.showAllBros) {
                return self.bros;
            }
            var hasHiddenBros = _.chain([transaction.Source].concat(transaction.Targets))
                .filter(function(b) {
                    return !_.isUndefined(b);
                })
                .map(function(b) {
                    return _.findWhere(self.bros, {Id: b});
                })
                .filter(function(b) {
                    return b.IsHidden;
                })
                .value().length > 0;
            if (hasHiddenBros) {
                return self.bros;
            }
            return _.filter(self.bros, function(b) {
                return !b.IsHidden;
            });
        },

        _getSelectedCurrency: function() {
            return parseInt(self.$rendered.find('#transactionCurrencies>label.active').attr('data-id'));
        },

        _bindTransaction: function(transaction, $editor) {
            broSelector.bind($editor.find('#transactionSource'), transaction.Source);
            broSelector.bind($editor.find('#transactionTargets'), transaction.Targets);
            $editor.find('#transactionCurrencies>label[data-id=' + transaction.Currency + ']').addClass('active');
            _.defer(self._updateAmountInBase);
            return transaction;
        },

        _unbindTransaction: function(transaction, $editor) {
            transaction.Source = broSelector.unbind($editor.find('#transactionSource'));
            transaction.Targets = broSelector.unbind($editor.find('#transactionTargets'));
            transaction.Currency = self._getSelectedCurrency();
            return transaction;
        },

        _validateTransaction: function($editor) {
            if ($editor.find('#transactionCurrencies>.active').length === 0) {
                notification.error('Please specify transaction Currency');
                return false;
            }
            var source = broSelector.unbind($editor.find('#transactionSource'));
            if (_.isUndefined(source)) {
                notification.error('Please specify transaction Source');
                return false;
            }
            var targets = broSelector.unbind($editor.find('#transactionTargets'));
            if (targets.length === 0) {
                notification.error('Please specify transaction Targets');
                return false;
            }
            if (targets.length === 1 && targets[0] === source) {
                notification.error("You can't spent money on yourself alone, fucking egoist!");
                return false;
            }
            return true;
        },
    };
    return self;
})