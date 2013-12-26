define(['$', 'analytics', 'transactionEditor', 'app-context'], function($, analytics, transactionEditor, context) {
    var self = {        
        activate: function (transactionCreated) {
            analytics.page('Return Money');
            transactionEditor.init(context.currencies, context.bros);
            transactionEditor.createTransaction({ Comment: "Money Return" }).always(transactionCreated);                           
        }
    };
    return self;
})