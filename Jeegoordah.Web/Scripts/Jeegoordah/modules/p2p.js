define(['_', '$', 'rest', 'app-context', 'transactionsList', 'text!templates/p2p/module.html'], function(_, $, rest, context, transactionsList, moduleTemplate) {
    var self = {        
        activate: function () {
            rest.get('transactions/p2p').done(function (transactions) {
                var module = $(moduleTemplate);
                $('#modules').empty().append(module);
                module.find('#createTransactionButton').click(transactionsList.createTransaction);
                
                transactionsList.init({
                    currencies: context.currencies,
                    bros: context.bros,
                    transactions: transactions,
                    target: module.find('#transactions')
                });
            });            
        }              
    };
    return self;
})