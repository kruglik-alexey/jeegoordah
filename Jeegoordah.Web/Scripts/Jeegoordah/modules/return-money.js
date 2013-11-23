define(['$', 'transactionEditor', 'rest'], function($, transactionEditor, rest) {
    var self = {        
        activate: function (transactionCreated) {
            $.when(rest.get('bros'), rest.get('currencies'))
                .done(function (bros, currencies) {
                    transactionEditor.init(currencies[0], bros[0]);
                    transactionEditor.createTransaction({ Comment: "Money Return" }).always(transactionCreated);
                });           
        }
    };
    return self;
})