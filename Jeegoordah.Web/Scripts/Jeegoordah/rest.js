define(['$', 'notification', 'helper'], function ($, notification, helper) {    
    var test = !!helper.getQueryVariable('test');

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
                type: "GET"
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