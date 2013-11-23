define(function() {
    require.config({
        baseUri: "jeegoordah",       
        paths: {
            jquery: '../jquery-2.0.3.min',
            underscore: '../underscore-min',
            text: '../text',
            signals: '../crossroads/signals.min'
        },
        map: {
            '*': {
                '$': 'jeegoordah-jquery',
                '_': 'underscore'
            }
        },
        shim: {
            '../sinon': {
                exports: 'sinon'
            }            
        },
        urlArgs: window.jgdhCacheBuster
    });   

    // First load jquery. Second load all libraries (see about noty below). Third load and initialize jeegoordah stuff.
    // Noty consists of multiple files. Main file should be loaded first, all other second. jeegoordah-noty returns deferred which resolves when they are loaded in the right order.
    require(['$'], function ($) {        
        require(['jeegoordah-noty', '../bootstrap/bootstrap.min', '../bootstrap/bootstrap-datepicker', '../jquery.validate.min', '../jquery.number.min'],
        function (noty) {
            require(['nav', 'app-context'], function (nav, context) {
                $.when(noty, context.init()).done(function() {
                    nav.init();
                });                
            });
        });
    });
});