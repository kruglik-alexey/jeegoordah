define(['_', '$', 'helper', 'notification', 'entityEditor', 'broSelector', 'rest', 'text!templates/transactions/transactionEditor.html'],
function (_, $, helper, notification, editor, broSelector, rest, editorTemplate) {
    var self = {
        init: function(currencies, bros) {
            self.currencies = currencies;
            self.visibleCurrencies = _.filter(self.currencies, function(c) {
                return !c.IsHidden;
            });
            self.hiddenCurrencies = _.filter(self.currencies, function (c) {
                return c.IsHidden;
            });
            self.bros = bros;
        },

        createTransaction: function(defaults) {
            defaults = _.defaults(defaults, { Date: helper.dateToString(new Date()) });
            
            var deferred = self._showTransactionEditor(defaults, 'Create Transaction', function (transaction) {
                transaction = _.defaults(transaction, { Event: defaults.Event });
                rest.post('transactions/create', transaction).done(function(createdTransaction) {                    
                    editor.close();
                    notification.success('Transaction created');
                    deferred.resolve(createdTransaction);
                }).fail(deferred.reject);
            });
            return deferred.promise();
        },
        
        editTransaction: function (transaction) {
            var deferred = self._showTransactionEditor(transaction, 'Edit Transaction', function (updatedTransaction) {                
                updatedTransaction = _.defaults(updatedTransaction, { Id: transaction.Id, Event: transaction.Event });
                rest.post('transactions/update', updatedTransaction).done(function () {                    
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
            var rendered = helper.template(editorTemplate, { currencies: currencies });
            rendered.find('#transactionSource').append(broSelector.render(true, self.bros));
            rendered.find('#transactionTargets').append(broSelector.render(false, self.bros));
            rendered.find('input[name=Amount]').number(true, 0, '.', ' ');

            editor.show(rendered, transaction, title, {
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

        _getCurrenciesForTransaction: function(transaction) {            
            var hidden = _.find(self.hiddenCurrencies, function(c) {
                return c.Id === transaction.Currency;
            });
            return hidden ? self.currencies : self.visibleCurrencies;
        },

        _bindTransaction: function(transaction, $editor) {
            broSelector.bind($editor.find('#transactionSource'), transaction.Source);
            broSelector.bind($editor.find('#transactionTargets'), transaction.Targets);
            $editor.find('#transactionCurrencies>label[data-id=' + transaction.Currency + ']').addClass('active');
            return transaction;
        },

        _unbindTransaction: function(transaction, $editor) {
            transaction.Source = broSelector.unbind($editor.find('#transactionSource'));
            transaction.Targets = broSelector.unbind($editor.find('#transactionTargets'));
            transaction.Currency = parseInt($editor.find('#transactionCurrencies>label.active').attr('data-id'));
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