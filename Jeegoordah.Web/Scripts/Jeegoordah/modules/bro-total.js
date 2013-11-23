define(['$', '_', 'rest', 'transactionsList', 'helper', 'app-context', 'text!templates/bro-total/module.html'],
function ($, _, rest, transactionsList, helper, context, moduleTemplate) {
    var self = {        
        activate: function (id) {
            id = parseInt(id);            
            rest.get('bros/' + id + '/transactions').done(function (broTransactions) {
                 var bro = _.find(context.bros, function (b) { return b.Id === id; });
                 var module = helper.template(moduleTemplate, bro);
                 $('#modules').empty().append(module);
                 transactionsList.init(context.currencies, context.bros, null);
                 transactionsList.renderTransactions(broTransactions, module.find('#transactions'), bro);
             });            
        }
    };
    return self;
})