define(['$', 'modules/total', 'modules/people', 'modules/events', 'modules/event-details', 'text!templates/nav.html', '../crossroads/crossroads.min'],
    function ($, total, people, events, eventDetail, template, crossroads) {

    var self = {        
        init: function () {
            this._initRoutes();
            $('#nav-container').append($(template));            
            window.onhashchange = this._processRoute;            
            this._processRoute();                       
        },
        
        _initRoutes: function() {
            crossroads.addRoute('events', function () {
                events.activate();
                self._activateNavigation('events');
            });
            crossroads.addRoute('events/{id}', function (id) {
                eventDetail.activate(id);
                self._activateNavigation('events');
            });
        },
        
        _activateNavigation: function(navigation) {
            $('ul.nav>li').removeClass('active');
            var navLi = $('#nav-' + navigation);
            navLi.addClass('active');
        },
        
        _processRoute: function () {            
            var hash = window.location.hash.substring(1) || 'total';
            $('#modules').empty();
            crossroads.parse(hash);            
        }
    };
        
    return self;
});