define(function() {
    require.config({
        baseUri: "jeegoordah",       
        paths: {
            jquery: '../jquery-2.0.3.min',
            underscore: '../underscore-min',
            text: '../text'
        },
        map: {
            '*': {
                '$': 'jeegoordah-jquery',
                '_': 'underscore'
            }
        }
    });
    
    // First load jquery. Second load all libraries (see about noty below). Third load and initialize jeegoordah stuff.
    // Noty consists of multiple files. Main file should be loaded first, all other second. jeegoordah-noty returns deferred which resolves when they are loaded in the right order.
    require(['$'], function () {        
        require(['jeegoordah-noty', '../bootstrap/bootstrap.min', '../bootstrap/bootstrap-datepicker', '../jquery.validate.min', '../jquery.jqote2.min'], function (noty) {            
            require(['modules/total', 'modules/events', 'nav'], function (total, events, nav) {
                noty.done(function() {
                    nav.init();
                    total.init();
                    events.init();
                });
            });
        });
    });        
});