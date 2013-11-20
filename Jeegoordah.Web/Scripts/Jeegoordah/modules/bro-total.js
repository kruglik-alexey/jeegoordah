define(['$', '_', 'rest', 'transactionsList', 'text!templates/bro-total/module.html'], function($, _, rest, transactionsList, moduleTemplate) {
    var self = {        
        activate: function (id) {
            id = parseInt(id);
            $.when(rest.get('bros'), rest.get('bros/' + id + '/transactions'), rest.get('currencies'))
             .done(function (bros, broTransactions, currencies) {
                 var bro = _.find(bros[0], function(b) { return b.Id === id; });
                 $('#modules').empty().append($($.jqote(moduleTemplate, bro)));
                 transactionsList.init(currencies[0], bros[0], null);
                 transactionsList.renderTransactions(transactions[0]);
             });            
        }
    };
    return self;
})