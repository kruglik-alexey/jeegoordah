define(['$', '_', 'rest', 'helper', 'entityEditor', 'broSelector', 'notification', 'text!templates/event-details/module.html', 'text!templates/event-details/transactionEditor.html',
        'text!templates/event-details/transaction.html'],
function ($, _, rest, helper, editor, broSelector, notification, moduleTemplate, transactionEditorTemplate, transactionTemplate) {
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
            var rendered = $($.jqote(transactionEditorTemplate, { currencies: self.currencies }));
            rendered.find('#transactionSource').append(broSelector.render(true, self.bros));
            rendered.find('#transactionTargets').append(broSelector.render(false, self.bros));
            rendered.find('input[name=Amount]').number(true, 0, '.', ' ');

            editor.show(rendered, { Date: self.event.StartDate, Currency: self.currencies[0].Id, Targets: self.event.Bros }, 'Create Transaction', {
                ok: function (transaction) {
                    transaction = _.defaults(transaction, {Event: self.event.Id});
                    rest.post('transactions/create', transaction).done(function (createdTransaction) {                        
                        self._createTransactionElement(createdTransaction);
                        editor.close();
                        notification.success('Transaction created.');
                    });
                },
                toForm: self._bindTransaction,
                fromForm: self._unbindTransaction,
                validate: self._validateTransaction
            });
        },
        
        _createTransactionElement: function (transaction) {
            var t = _.clone(transaction);
            t.Source = _.find(self.bros, function (bro) { return bro.Id === t.Source; });
            t.Targets = _.chain(t.Targets).map(function (target) {
                return _.find(self.bros, function (bro) { return bro.Id === target; });
            }).sortBy('Name').value();
            t.Amount = $.number(t.Amount, 0, '.', ' ');
            t.Currency = _.find(self.currencies, function (currency) { return currency.Id === t.Currency; });
            var element = $($.jqote(transactionTemplate, t));            
            $('#transactions').append(element);
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