define(['$', 'notification'], function ($, notification) {
    var rest = {        
        get: function (resource) {
            var ajax = $.ajax({
                url: resource,
                type: "GET",
            });
            ajax.fail(function (response) {                
                rest._showError(response.responseJSON);
            });
            return ajax;
        },
        
        post: function(resource, data) {
            var ajax = $.ajax({
                url: resource,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(data)
            });
            ajax.fail(function (response) {
                rest._showError(response.responseJSON);
            });
            return ajax;
        },
        
        _showError: function (error) {
            // TODO now only supports BL errors from controllers (400), can't handle 500, 404 etc
            notification.error(error.Message);            
        }
    };

    return rest;    
});