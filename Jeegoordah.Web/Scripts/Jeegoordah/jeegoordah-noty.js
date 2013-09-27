define(['$', '../noty/jquery.noty'], function($) {
    var d = $.Deferred();
    require(['../noty/topCenter', '../noty/default'], function () {        
        d.resolve();        
    });
    return d;
});