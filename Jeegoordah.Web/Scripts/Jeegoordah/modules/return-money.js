define(['$', 'transactionEditor', 'app-context'], function($, transactionEditor, context) {
    var self = {        
        activate: function (transactionCreated) {            
            transactionEditor.init(context.currencies, context.bros);
            transactionEditor.createTransaction({ Comment: "Money Return" }).always(transactionCreated);                           
        }
    };
    return self;
})