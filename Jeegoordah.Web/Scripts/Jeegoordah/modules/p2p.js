define(['_', '$', 'rest', 'app-context', 'transactionsList', 'text!templates/p2p/module.html'], function(_, $, rest, context, transactionsList, moduleTemplate) {
    var self = {        
        activate: function () {
            rest.get('transactions/p2p').done(function (transactions) {
                transactionsList.init(context.currencies, context.bros);

                var module = $(moduleTemplate);
                $('#modules').empty().append(module);
                module.find('#createTransactionButton').click(transactionsList.createTransaction);
                transactionsList.renderTransactions(transactions, module.find('#transactions'));
            });            
        }              
    };
    return self;
})