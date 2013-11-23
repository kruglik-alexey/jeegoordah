define(['$', 'rest'], function($, rest) {
    var self = {        
        init: function () {
            var deferred = $.Deferred();
            $.when(rest.get('bros'), rest.get('currencies')).done(function(bros, currencies) {
                self.bros = bros[0];
                self.currencies = currencies[0];
                deferred.resolve();
            }).fail(deferred.reject);
            return deferred.promise();
        }
    };
    return self;
})