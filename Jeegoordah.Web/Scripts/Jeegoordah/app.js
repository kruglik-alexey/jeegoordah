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
        },
        shim: {
            '../sinon': {
                exports: 'sinon'
            }
        },
        urlArgs: window.jgdhCacheBuster
    });

    var deferred = {
        _callbacks: [],
        _resolved: false,
        done: function (callback) {
            if (this._resolved) {
                callback();
            } else {
                this._callbacks.push(callback);
            }            
        },
        resolve: function() {
            this._resolved = true;
            for (var i = 0; i < this._callbacks.length; i++) {
                this._callbacks[i]();
            }
        }
    };

    // First load jquery. Second load all libraries (see about noty below). Third load and initialize jeegoordah stuff.
    // Noty consists of multiple files. Main file should be loaded first, all other second. jeegoordah-noty returns deferred which resolves when they are loaded in the right order.
    require(['$'], function () {        
        require(['jeegoordah-noty', '../bootstrap/bootstrap.min', '../bootstrap/bootstrap-datepicker', '../jquery.validate.min', '../jquery.jqote2.min'], function (noty) {            
            require(['modules/total', 'modules/events', 'nav'], function (total, events, nav) {
                noty.done(function() {
                    nav.init();
                    total.init();
                    events.init();
                    deferred.resolve();
                });
            });
        });
    });

    return deferred;
});