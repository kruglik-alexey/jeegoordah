define(['$', 'rest', 'helper', '_'], function($, rest, helper, _) {
    var self = {        
        init: function () {
            var deferred = $.Deferred();
            $.when(rest.get('bros'), rest.get('currencies')).done(function(bros, currencies) {
                self.bros = bros[0];
                self.showAllBros = helper.getQueryVariable('allBros');
                if (self.showAllBros) {
                    self.notHiddenBros = self.bros;
                } else {
                    self.notHiddenBros = _.filter(self.bros,
                        function(b) {
                            return !b.IsHidden;
                        });
                }                
                self.currencies = currencies[0];
                deferred.resolve();
            }).fail(deferred.reject);
            return deferred.promise();
        }
    };
    return self;
})