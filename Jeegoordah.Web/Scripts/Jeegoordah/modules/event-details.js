define(['$', '_', 'rest', 'helper', 'transactionsList', 'text!templates/event-details/module.html'], function ($, _, rest, helper, transactionsList, moduleTemplate) {
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
                transactionsList.init(self.currencies, self.bros, self.event);

                $('#modules').empty().append($($.jqote(moduleTemplate, self._getUiEvent(self.event))));
                $('#createTransactionButton').click(transactionsList.createTransaction);
                transactionsList.renderTransactions(transactions[0]);
            });
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