define(['$', '_', 'rest', 'transactionsList', 'helper', 'app-context', 'text!templates/bro-total/module.html'],
function ($, _, rest, transactionsList, helper, context, moduleTemplate) {
    var self = {        
        activate: function (id) {
            id = parseInt(id);
            $.when(rest.get('bros/' + id + '/transactions'), rest.get('events')).done(function (broTransactions, events) {
                 var bro = _.find(context.bros, function (b) { return b.Id === id; });
                 var module = helper.template(moduleTemplate, bro);
                 $('#modules').empty().append(module);
                 transactionsList.init(context.currencies, context.bros, null, events[0]);
                 transactionsList.renderTransactions(broTransactions[0], module.find('#transactions'), bro);
             });
        }
    };
    return self;
})