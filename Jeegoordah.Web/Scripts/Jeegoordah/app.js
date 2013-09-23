define(function() {
    require.config({
        baseUri: "jeegoordah",       
        paths: {
            jquery: '../jquery-2.0.3.min',
            underscore: '../underscore-min'
        },
        map: {
            '*': {
                '$': 'jeegoordah-jquery',
                '_': 'underscore'
            }
        }
    });
    
    require(['$'], function () {                
        require(['../bootstrap/bootstrap.min', '../bootstrap/bootstrap-datepicker', '../jquery.validate.min', '../jquery.jqote2.min'], function () {
            require(['modules/total', 'modules/events', 'nav'], function (total, events, nav) {
                nav.init();
                total.init();
                events.init();
            });
        });
    });        
});