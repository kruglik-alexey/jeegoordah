define(['$', '_', 'analytics', 'rest', 'helper', 'transactionsList', 'entityControls', 'eventEditor', 'notification', 'consts', 'app-context', 'text!templates/event-details/module.html'],
function ($, _, analytics, rest, helper, transactionsList, entityControls, eventEditor, notification, consts, context, moduleTemplate) {
    var self = {        
        activate: function(id) {
            self.baseCurrency = _.findWhere(context.currencies, { IsBase: true });
            $.when(rest.get('events/' + id),
                   rest.get('events/' + id + '/transactions'))
            .done(function (event, transactions) {                
                self.event = event[0];
                analytics.page('Event ' + self.event.Name);
                self.transactions = transactions[0];
                self._render();
            });
        },
        
        _editEvent: function() {
            eventEditor.edit(self.event, context.bros, 'Edit Event', function (updatedEvent) {
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
                self.module.fadeOut(consts.fadeDuration, function () {
                    require('nav').go('events');
                });
            });
        },

        _getUiEvent: function(restEvent) {
            var uiEvent = _.clone(restEvent);
            uiEvent.Description = helper.textToHtml(uiEvent.Description || '');
            uiEvent.Bros = _.chain(uiEvent.Bros).map(function (broId) {
                var bro = _.find(context.bros, function (b) { return b.Id === broId; });
                return _.extend({}, bro, { Total: helper.formatNumber(self._getBroTotal(broId)) });
            }).sortBy('Name').value();
            var total = _.reduce(self.transactions, function (acc, tr) { return acc + tr.Amount / tr.Rate; }, 0);
            uiEvent.TotalFormatted = helper.formatNumber(total);
            uiEvent.BaseCurrencyName = self.baseCurrency.Name;
            return uiEvent;
        },

        _getBroTotal: function(broId) {
            return _.chain(self.transactions)
                .filter(function(t) { return _.contains(t.Targets, broId); })
                .reduce(function(acc, t) { return acc + t.Amount / t.Rate / t.Targets.length; }, 0)
                .value();
        },
        
        _render: function () {
            self.module = helper.template(moduleTemplate, self._getUiEvent(self.event));
            $('#modules').empty().append(self.module);
            self.module.find('#createTransactionButton').click(transactionsList.createTransaction);
            entityControls.render(self.module.find('.page-header>h1'), self._editEvent, self._deleteEvent);
            
            transactionsList.init({
                currencies: context.currencies,
                bros: context.bros,
                event: self.event,
                transactions: self.transactions,
                target: self.module.find('#transactions')
            });           
        }
    };

    return self;
})