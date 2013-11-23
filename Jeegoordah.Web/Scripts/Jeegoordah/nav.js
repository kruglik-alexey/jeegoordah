define(['$', 'modules/total', 'modules/events', 'modules/event-details', 'modules/p2p', 'modules/bro-total', 'text!templates/nav.html', '../crossroads/crossroads.min'],
    function ($, total, events, eventDetail, p2p, broTotal, template, crossroads) {

    var self = {        
        init: function () {
            this._initRoutes();
            self.nav = $('#nav-container');
            self.nav.append($(template));
            window.onhashchange = this._processRoute;            
            this._processRoute();                       
        },

        go: function(route) {
            window.location.hash = '#' + route;
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
            crossroads.addRoute('p2p', function () {
                p2p.activate();
                self._activateNavigation('p2p');
            });
            crossroads.addRoute('total', function () {
                total.activate();
                self._activateNavigation('total');
            });
            crossroads.addRoute('bros/{id}/total', function (id) {
                broTotal.activate(id);
                self._activateNavigation('total');
            });
        },
        
        _activateNavigation: function(navigation) {
            self.nav.find('ul.nav>li').removeClass('active');
            var navLi = self.nav.find('#nav-' + navigation);
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