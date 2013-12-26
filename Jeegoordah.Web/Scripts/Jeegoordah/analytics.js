define(["http://www.google-analytics.com/analytics.js"], function () {    
    var ga = window.ga;
    delete window.ga;
    if (window.location.hostname === 'localhost') {
        ga = function() { };
    }
    ga('create', 'UA-46402422-1', 'auto');

    var self = {        
        page: function (title, page) {
            if (!page) {
                page = window.location.hash;
                page = '/' + page.substr(1);
            }            
            ga('send', 'pageview', {
                'page': page,
                'title': title
            });
        }
    };
    return self;
})