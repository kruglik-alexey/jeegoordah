define(['_', '$', 'rest', 'helper', 'notification', 'entityControls', 'consts', 'transactionEditor',
        'text!templates/transactions/transactionsList.html', 'text!templates/transactions/transaction.html'],
function (_, $, rest, helper, notification, entityControls, consts, transactionEditor, listTemplate, transactionTemplate) {
    var self = {
        init: function(currencies, bros, event, events) {
            self.currencies = currencies;
            self.bros = bros;
            self.event = event;
            self.events = events;
            transactionEditor.init(self.currencies, self.bros);
        },        
        
        renderTransactions: function (transactions, target, highlightBro) {
            self.highlightBro = highlightBro || {Id: null};
            var table = helper.template(listTemplate, {events: self.events});
            self.list = table.find('tbody');
            target.append(table);
            _.each(transactions, function (t) {
                self._createTransactionElement(t, function(list, element) {
                    list.append(element);
                });
            });
        },
        
        createTransaction: function () {
            var defaults = {};
            if (self.event) {
                defaults = { Currency: self.currencies[0].Id, Targets: self.event.Bros };
            }
            transactionEditor.createTransaction(defaults).done(function(transaction) {
                self._createTransactionElement(transaction, function (list, element) {
                    list.prepend(element);
                });
            });
        },
        
        _createTransactionElement: function (transaction, action) {
            var ui = self._createUiTransaction(transaction);
            var element = helper.template(transactionTemplate, ui);
            entityControls.render(element.find('.entity-controls'),
                _.partial(self._editTransaction, transaction),
                _.partial(self._deleteTransaction, transaction));

            if (!_.isUndefined(action)) {
                action(self.list, element);
            }

            return element;
        },
        
        _createUiTransaction: function(transaction) {
            var ui = _.clone(transaction);
            ui.Source = _.find(self.bros, function (bro) { return bro.Id === ui.Source; });
            ui.Targets = _.chain(ui.Targets).map(function (target) {
                return _.find(self.bros, function (bro) { return bro.Id === target; });
            }).sortBy('Name').value();
            ui.Amount = $.number(ui.Amount, 0, '.', ' ');
            ui.Currency = _.find(self.currencies, function (currency) { return currency.Id === ui.Currency; });
            ui.targetsEqualsEvent = self.event && helper.equalArrays(transaction.Targets, self.event.Bros);
            ui.Comment = helper.textToHtml(ui.Comment);
            ui.highlightBro = self.highlightBro;
            if (ui.Event) {
                ui.Event = _.find(self.events, function (e) {
                    return e.Id === ui.Event;
                });
            } else {
                ui.Event = { Name: '' };
            }
            ui.events = self.events;
            return ui;
        },
        
        _updateTransactionElement: function (transaction) {
            var oldElement = $('#transaction' + transaction.Id);
            var newElement = self._createTransactionElement(transaction);
            newElement.hide();
            oldElement.fadeOut(consts.fadeDuration, function () {
                newElement.insertAfter(oldElement);
                oldElement.remove();
                newElement.fadeIn(consts.fadeDuration);
            });
        },
        
        _editTransaction: function (transaction) {
            transactionEditor.editTransaction(transaction).done(self._updateTransactionElement);
        },

        _deleteTransaction: function (transaction) {
            var $transaction = self.list.find('#transaction' + transaction.Id);
            rest.post('transactions/delete/' + transaction.Id).done(function () {
                notification.success("Transaction deleted");
                $transaction.fadeOut(consts.fadeDuration, function () {
                    $transaction.remove();
                });
            });
        }
    };
    return self;
})