define(['$', 'rest'], function($, rest) {
    var self = {
        cache: {},
        get: function (date) {
            var cachedRates = self.cache[date];
            if (cachedRates) {
                if (cachedRates.state() !== 'rejected') {
                    return cachedRates.promise();
                } else {
                    self.cache[date] = null;
                    return self.get(date);
                }
            } else {
                var request = rest.get('rates/' + date);
                self.cache[date] = request;
                return request.promise();                    
            }         
        }
    };
    return self;
});