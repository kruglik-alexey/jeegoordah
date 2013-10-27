define(['$', '_', 'rest', 'helper', 'entityEditor', 'broSelector', 'notification', 'entityControls', 'consts',
        'text!templates/event-details/module.html', 'text!templates/event-details/transactionEditor.html', 'text!templates/event-details/transaction.html'],
function ($, _, rest, helper, editor, broSelector, notification, entityControls, consts, moduleTemplate, transactionEditorTemplate, transactionTemplate) {
    var self = {        
        activate: function(id) {
            $.when(rest.get('events/' + id),
                   rest.get('events/' + id + '/transactions'),
                   rest.get('bros'),
                   rest.get('currencies'))
            .done(function (event, transactions, bros, currencies) {
                self.bros = bros[0];
                self.event = event[0];
                self.currencies = currencies[0];

                $('#modules').empty().append($($.jqote(moduleTemplate, self._getUiEvent(self.event))));
                $('#createTransactionButton').click(self._createTransaction);
                self._renderTransactions(transactions[0]);
            });
        },
        
        _renderTransactions: function (transactions) {
            _.each(transactions, function(t) {
                self._createTransactionElement(t);
            });
        },
        
        _createTransaction: function () {
            self._showTransactionEditor({Date: self.event.StartDate, Currency: self.currencies[0].Id, Targets: self.event.Bros}, 'Create Transaction', function(transaction) {
                transaction = _.defaults(transaction, { Event: self.event.Id });
                rest.post('transactions/create', transaction).done(function (createdTransaction) {
                    self._createTransactionElement(createdTransaction);
                    editor.close();
                    notification.success('Transaction created.');
                });
            });            
        },
        
        _showTransactionEditor: function(transaction, title, ok) {
            var rendered = $($.jqote(transactionEditorTemplate, { currencies: self.currencies }));
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
        
        _createTransactionElement: function (transaction, appendToList) {
            var ui = _.clone(transaction);
            ui.Source = _.find(self.bros, function (bro) { return bro.Id === ui.Source; });
            ui.Targets = _.chain(ui.Targets).map(function (target) {
                return _.find(self.bros, function (bro) { return bro.Id === target; });
            }).sortBy('Name').value();
            ui.Amount = $.number(ui.Amount, 0, '.', ' ');
            ui.Currency = _.find(self.currencies, function (currency) { return currency.Id === ui.Currency; });
            ui.targetsEqualsEvent = _.union(transaction.Targets, self.event.Bros).length === transaction.Targets.length;
            
            var element = $($.jqote(transactionTemplate, ui));            
            entityControls.render(element.find('.entity-controls'),
                _.partial(self._editTransaction, transaction),
                _.partial(self._deleteTransaction, transaction));
            
            if (_.isUndefined(appendToList) || appendToList) {
                $('#transactions').append(element);
            }

            return element;
        },
        
        _editTransaction: function (transaction) {
            self._showTransactionEditor(transaction, 'Edit Transaction', function (updatedTransaction) {
                updatedTransaction = _.defaults(updatedTransaction, { Id: transaction.Id, Event: self.event.Id });
                rest.post('transactions/update', updatedTransaction).done(function () {
                    self._updateTransactionElement(updatedTransaction);
                    editor.close();
                    notification.success('Transaction updated.');
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
        },
        
        _updateTransactionElement: function(transaction) {
            var oldElement = $('#transaction' + transaction.Id);
            var newElement = self._createTransactionElement(transaction, false);            
            newElement.hide();
            oldElement.fadeOut(consts.fadeDuration, function () {                                
                newElement.insertAfter(oldElement);                
                oldElement.remove();
                newElement.fadeIn(consts.fadeDuration);
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
            if (_.isUndefined(broSelector.unbind($editor.find('#transactionSource')))) {
                notification.error('Please specify transaction Source');
                return false;
            }
            if (broSelector.unbind($editor.find('#transactionTargets')).length === 0) {
                notification.error('Please specify transaction Targets');
                return false;
            }
            return true;
        },

        _getUiEvent: function(restEvent) {
            var uiEvent = _.clone(restEvent);
            uiEvent.Description = helper.textToHtml(uiEvent.Description || '');
            uiEvent.Bros = _.chain(uiEvent.Bros).map(function (broId) {
                return _.find(self.bros, function (bro) { return bro.Id === broId; });
            }).sortBy('Name').value();            
            return uiEvent;
        }
    };

    return self;
})