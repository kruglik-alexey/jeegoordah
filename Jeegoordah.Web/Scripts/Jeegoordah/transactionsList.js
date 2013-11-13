define(['_', '$', 'rest', 'helper', 'entityEditor', 'broSelector', 'notification', 'entityControls', 'consts',
        'text!templates/transactions/transaction.html', 'text!templates/transactions/transactionEditor.html'],
function (_, $, rest, helper, editor, broSelector, notification, entityControls, consts, transactionTemplate, editorTemplate) {
    var self = {
        init: function(currencies, bros, event) {
            self.currencies = currencies;
            self.bros = bros;
            self.event = event;
        },
        
        renderTransactions: function (transactions) {
            _.each(transactions, function (t) {
                self._createTransactionElement(t);
            });
        },

        createTransaction: function () {
            var defaults;
            if (self.event) {
                defaults = {Date: self.event.StartDate, Currency: self.currencies[0].Id, Targets: self.event.Bros};
            } else {
                defaults = {Date: helper.dateToString(new Date())};
            }
            
            self._showTransactionEditor(defaults, 'Create Transaction', function (transaction) {
                if (self.event) {
                    transaction = _.defaults(transaction, {Event: self.event.Id});
                }                
                rest.post('transactions/create', transaction).done(function (createdTransaction) {
                    self._createTransactionElement(createdTransaction);
                    editor.close();
                    notification.success('Transaction created');
                });
            });
        },
        
        _showTransactionEditor: function (transaction, title, ok) {
            var rendered = $($.jqote(editorTemplate, { currencies: self.currencies }));
            rendered.find('#transactionSource').append(broSelector.render(true, self.bros));
            rendered.find('#transactionTargets').append(broSelector.render(false, self.bros));
            rendered.find('input[name=Amount]').number(true, 0, '.', ' ');

            editor.show(rendered, transaction, title, {
                ok: ok,
                toForm: self._bindTransaction,
                fromForm: self._unbindTransaction,
                validate: self._validateTransaction
            });
        },
        
        _bindTransaction: function (transaction, $editor) {
            broSelector.bind($editor.find('#transactionSource'), transaction.Source);
            broSelector.bind($editor.find('#transactionTargets'), transaction.Targets);
            $editor.find('#transactionCurrencies>label[data-id=' + transaction.Currency + ']').addClass('active');
            return transaction;
        },

        _unbindTransaction: function (transaction, $editor) {
            transaction.Source = broSelector.unbind($editor.find('#transactionSource'));
            transaction.Targets = broSelector.unbind($editor.find('#transactionTargets'));
            transaction.Currency = parseInt($editor.find('#transactionCurrencies>label.active').attr('data-id'));
            return transaction;
        },

        _validateTransaction: function ($editor) {
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
        
        _createTransactionElement: function (transaction, appendToList) {
            var ui = _.clone(transaction);
            ui.Source = _.find(self.bros, function (bro) { return bro.Id === ui.Source; });
            ui.Targets = _.chain(ui.Targets).map(function (target) {
                return _.find(self.bros, function (bro) { return bro.Id === target; });
            }).sortBy('Name').value();
            ui.Amount = $.number(ui.Amount, 0, '.', ' ');
            ui.Currency = _.find(self.currencies, function (currency) { return currency.Id === ui.Currency; });
            ui.targetsEqualsEvent = self.event && helper.equalArrays(transaction.Targets, self.event.Bros);

            var element = $($.jqote(transactionTemplate, ui));
            entityControls.render(element.find('.entity-controls'),
                _.partial(self._editTransaction, transaction),
                _.partial(self._deleteTransaction, transaction));

            if (_.isUndefined(appendToList) || appendToList) {
                $('#transactions').append(element);
            }

            return element;
        },
        
        _updateTransactionElement: function (transaction) {
            var oldElement = $('#transaction' + transaction.Id);
            var newElement = self._createTransactionElement(transaction, false);
            newElement.hide();
            oldElement.fadeOut(consts.fadeDuration, function () {
                newElement.insertAfter(oldElement);
                oldElement.remove();
                newElement.fadeIn(consts.fadeDuration);
            });
        },
        
        _editTransaction: function (transaction) {
            self._showTransactionEditor(transaction, 'Edit Transaction', function (updatedTransaction) {
                var eventId = self.event ? self.event.Id : undefined;
                updatedTransaction = _.defaults(updatedTransaction, {Id: transaction.Id, Event: eventId});
                rest.post('transactions/update', updatedTransaction).done(function () {
                    self._updateTransactionElement(updatedTransaction);
                    editor.close();
                    notification.success('Transaction updated');
                });
            });
        },

        _deleteTransaction: function (transaction) {
            var $transaction = $('#transaction' + transaction.Id);
            rest.post('transactions/delete/' + transaction.Id).done(function () {
                $transaction.fadeOut(consts.fadeDuration, function () {
                    $transaction.remove();
                });
            });
        }
    };
    return self;
})