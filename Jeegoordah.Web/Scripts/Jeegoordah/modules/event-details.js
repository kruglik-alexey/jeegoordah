define(['$', '_', 'rest', 'helper', 'entityEditor', 'broSelector', 'notification', 'text!templates/event-details/module.html', 'text!templates/event-details/transactionEditor.html'],
function ($, _, rest, helper, editor, broSelector, notification, moduleTemplate, transactionEditorTemplate) {
    var self = {        
        activate: function(id) {
            $.when(rest.get('events/' + id), rest.get('bros'), rest.get('currencies')).done(function (event, bros, currencies) {
                self.bros = bros[0];
                self.event = self._getUiEvent(event[0]);
                self.currencies = currencies[0];

                $('#modules').empty().append($($.jqote(moduleTemplate, self.event)));
                $('#createTransactionButton').click(self._createTransaction);
            });
        },
        
        _createTransaction: function () {            
            var rendered = $($.jqote(transactionEditorTemplate, { currencies: self.currencies }));
            rendered.find('#transactionSource').append(broSelector.render(true, self.bros));
            rendered.find('#transactionTargets').append(broSelector.render(false, self.bros));
            rendered.find('input[name=Amount]').number(true, 0, '.', ' ');

            editor.show(rendered, { Date: self.event.StartDate, Currency: self.currencies[0].Id }, 'Create Transaction', {
                ok: function (transaction) {
                    transaction = _.defaults(transaction, {Event: self.event.Id});
                    rest.post('transactions/create', transaction).done(function (createdTransaction) {
                        debugger;
//                        createdEvent = self._fixStartDateFormat(createdEvent);
//                        self._createEventElement(createdEvent);
                        editor.close();
                        notification.success('Transaction created.');
                    });
                },
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