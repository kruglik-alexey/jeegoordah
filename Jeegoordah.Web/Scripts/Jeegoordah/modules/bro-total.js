define(['$', '_', 'analytics', 'rest', 'transactionsList', 'helper', 'app-context', 'text!templates/bro-total/module.html', 'text!templates/bro-total/transactionsList.html',
        'text!templates/bro-total/transaction.html'],
function ($, _, analytics, rest, transactionsList, helper, context, moduleTemplate, transactionsListTemplate, transactionTemplate) {
    var self = {
        activate: function (id) {
            id = parseInt(id);
            $.when(rest.get('bros/' + id + '/transactions'), rest.get('events')).done(function (broTransactions, events) {
                self.bro = _.find(context.bros, function (b) { return b.Id === id; });
                analytics.page('Total for ' + self.bro.Name);
                self.events = events[0];
                var module = helper.template(moduleTemplate, self.bro);
                $('#modules').empty().append(module);
                
                transactionsList.init({
                    currencies: context.currencies,
                    bros: context.bros,                     
                    transactions: broTransactions[0],                    
                    target: module.find('#transactions'),
                    transactionsListTemplate: transactionsListTemplate,
                    transactionTemplate: transactionTemplate,
                    transactionAugmenter: self.transactionAugmenter
                });                 
             });
        },
        
        transactionAugmenter: function (transaction) {
            transaction.targetsBro = !!_.find(transaction.Targets, function (t) { return t.Id === self.bro.Id; });            
            transaction.highlightBro = self.bro;
            
            transaction.eventName = '';
            if (transaction.Event) {
                var event = _.find(self.events, function (e) { return e.Id === transaction.Event; });
                transaction.eventName = event.Name;
            }
            
            if (transaction.Source.Id === self.bro.Id) {
                if (transaction.targetsBro) {
                    transaction.delta = transaction.Amount / transaction.Targets.length * (transaction.Targets.length - 1);
                } else {
                    transaction.delta = transaction.Amount;
                }
            } else {
                transaction.delta = -1 * transaction.Amount / transaction.Targets.length;
            }
            transaction.deltaFormatted = helper.formatNumber(transaction.delta);

            transaction.allTargets = _.pluck(transaction.Targets, "Name").join(', ');
        }
    };
    return self;
})