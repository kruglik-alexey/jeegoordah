define(['_', '$', 'rest', 'transactionsList', 'text!templates/p2p/module.html'], function(_, $, rest, transactionsList, moduleTemplate) {
    var self = {        
        activate: function () {
            $.when(rest.get('transactions/p2p'),
                   rest.get('bros'),
                   rest.get('currencies'))
            .done(function (transactions, bros, currencies) {
                transactionsList.init(currencies[0], bros[0]);

                var module = $(moduleTemplate);
                $('#modules').empty().append(module);
                module.find('#createTransactionButton').click(transactionsList.createTransaction);
                transactionsList.renderTransactions(transactions[0], module.find('#transactions'));
            });            
        }              
    };
    return self;
})