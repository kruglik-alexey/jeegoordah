define(['$', '_', 'rest', 'helper', 'transactionsList', 'entityControls', 'eventEditor', 'notification', 'consts', 'text!templates/event-details/module.html'],
function ($, _, rest, helper, transactionsList, entityControls, eventEditor, notification, consts, moduleTemplate) {
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
                self.transactions = transactions[0];
                self._render();
            });
        },
        
        _editEvent: function() {
            eventEditor.edit(self.event, self.bros, 'Edit Event', function (updatedEvent) {
                updatedEvent = _.extend(self.event, updatedEvent);
                rest.post('events/update', updatedEvent).done(function () {
                    eventEditor.close();
                    notification.success('Event updated');
                    self._render();
                });
            });
        },
        
        _deleteEvent: function () {            
            rest.post('events/delete/' + self.event.Id).done(function () {
                notification.success('Event deleted');
                $('#module-event-details').fadeOut(consts.fadeDuration, function () {
                    require('nav').go('events');
                });
            });
        },

        _getUiEvent: function(restEvent) {
            var uiEvent = _.clone(restEvent);
            uiEvent.Description = helper.textToHtml(uiEvent.Description || '');
            uiEvent.Bros = _.chain(uiEvent.Bros).map(function (broId) {
                return _.find(self.bros, function (bro) { return bro.Id === broId; });
            }).sortBy('Name').value();            
            return uiEvent;
        },
        
        _render: function() {
            $('#modules').empty().append($($.jqote(moduleTemplate, self._getUiEvent(self.event))));
            $('#createTransactionButton').click(transactionsList.createTransaction);
            entityControls.render($('#module-event-details>.page-header>h1'), self._editEvent, self._deleteEvent);
            transactionsList.init(self.currencies, self.bros, self.event);
            transactionsList.renderTransactions(self.transactions, $('#transactions'));
        }
    };

    return self;
})