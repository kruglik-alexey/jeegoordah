define(['$', '_', 'rest', 'transactionsList', 'helper', 'text!templates/bro-total/module.html'], function($, _, rest, transactionsList, helper, moduleTemplate) {
    var self = {        
        activate: function (id) {
            id = parseInt(id);            
            $.when(rest.get('bros'), rest.get('bros/' + id + '/transactions'), rest.get('currencies'))
             .done(function (bros, broTransactions, currencies) {
                 var bro = _.find(bros[0], function (b) { return b.Id === id; });
                 var module = helper.template(moduleTemplate, bro);
                 $('#modules').empty().append(module);
                 transactionsList.init(currencies[0], bros[0], null);
                 transactionsList.renderTransactions(broTransactions[0], module.find('#transactions'), bro);
             });            
        }
    };
    return self;
})