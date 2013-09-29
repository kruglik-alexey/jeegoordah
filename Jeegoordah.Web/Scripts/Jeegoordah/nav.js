define(['$', 'modules/total', 'modules/people', 'modules/events', 'text!templates/nav.html'], function ($, total, people, events, template) {
    var routes = {        
        total: total,
        people: people,
        events: events,
        default: 'total'
    };

    return {        
        init: function () {
            $('#nav-container').append($(template));            
            window.onhashchange = this._processRoute;            
            this._processRoute();                       
        },
        
        _processRoute: function() {
            var hash = window.location.hash.substring(1) || routes.default;
            $('ul.nav>li').removeClass('active');
            var navLi = $('#nav-' + hash);            
            navLi.addClass('active');
            
            var moduleName = navLi.find('a').text();
            document.title = 'Jeegoordah | ' + moduleName;            
            $('h1#module-name').text(moduleName);
            
            var route = routes[hash];
            if (route) {
                route.activate();
            }
        }
    };       
});