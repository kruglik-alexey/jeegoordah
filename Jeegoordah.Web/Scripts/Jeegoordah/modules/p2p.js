define(['_', '$', 'text!templates/p2p/module.html'], function(_, $, moduleTemplate) {
    var self = {        
        activate: function() {
            $('#modules').empty().append($(moduleTemplate));
            $('#createTransactionButton').click(self._createTransaction);
        },
        
        _createTransaction: function() {
            
        }
    };
    return self;
})