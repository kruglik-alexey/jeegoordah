define(['$', 'notification'], function ($, notification) {
    var getQueryVariable = function(variable) {
        var query = window.location.search.substring(1);
        var vars = query.split("&");
        for (var i = 0; i < vars.length; i++) {
            var pair = vars[i].split("=");
            if (pair[0] == variable) {
                return pair[1];
            }
        }
        return false;
    };

    var test = !!getQueryVariable('test');

    var showError = function (error) {        
        if (error.getAllResponseHeaders() === '') {
            notification.error('No connection');            
        } else if (error.status === 400) {
            notification.error(error.responseJSON.Message);
        } else {
            notification.error(error.status + ' Server Error');
        }        
    };

    var prepareUrl = function(url) {
        if (!test) {
            return url;
        }
        return url + '?test=1';
    };

    return {        
        get: function (resource) {
            var ajax = $.ajax({
                url: prepareUrl(resource),
                type: "GET",
            });
            ajax.fail(function (response) {                
                showError(response);
            });
            return ajax;
        },
        
        post: function(resource, data) {
            var ajax = $.ajax({
                url: prepareUrl(resource),
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(data)
            });
            ajax.fail(function (response) {                
                showError(response);
            });
            return ajax;
        }               
    };
});