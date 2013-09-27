define(['$', 'modules/total', 'modules/people', 'modules/events'], function ($, total, people, events) {
    var routes = {        
        total: total,
        people: people,
        events: events,
        default: 'total'
    };

    return {        
        init: function() {
            window.onhashchange = this._processRoute;            
            this._processRoute();
            
            // Navigation and modules container are hidden in markup. Showing them after navigation and module are initialized.
            $('ul.nav').show();
            $('#modules').show();                        
        },
        
        _processRoute: function() {
            var hash = window.location.hash.substring(1) || routes.default;
            $('ul.nav>li').removeClass('active');
            var navLi = $('#nav-' + hash);            
            navLi.addClass('active');
            
            var moduleName = navLi.find('a').text();
            document.title = 'Jeegoordah | ' + moduleName;
            
            $('h1#module-name').text(moduleName);
            // Hide all modules
            $('div#modules>div').hide();
            // Show current module
            $('div#module-' + hash).show();

            var route = routes[hash];
            if (route) {
                route.activate();
            }
        }
    };       
});