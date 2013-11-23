define(['_', '$', 'rest', 'notification', 'helper', 'eventEditor', 'text!templates/events/row.html', 'text!templates/events/editor.html', 'text!templates/events/module.html'],
    function (_, $, rest, notification, helper, eventEditor, rowTemplate, editorTemplate, moduleTemplate) {

    var self = {
        activate: function () {            
            // TODO add spinner while loading
            $.when(rest.get('bros'), rest.get('events')).done(function (bros, events) {
                self.bros = bros[0];                
                $('#modules').empty().append($(moduleTemplate));
                $('#createEventButton').click(self._createEvent);
                self._loadEvents(events[0]);                                
            });                        
        },
        
        _loadEvents: function (events) {            
            _.chain(events)
                .map(function (event) {                    
                    event.StartDateObj = helper.parseDate(event.StartDate);
                    return event;
                })
                .sortBy(function(event) {
                    return event.StartDateObj;
                })
                .each(self._createEventElement);
        },
        
        _createEvent: function () {
            eventEditor.edit({}, self.bros, 'Create Event', function(event) {
                rest.post('events/create', event).done(function (createdEvent) {
                    createdEvent.StartDateObj = helper.parseDate(createdEvent.StartDate);
                    self._createEventElement(createdEvent);
                    eventEditor.close();
                    notification.success('Event created');
                });
            });                                                         
        },
        
        _createEventElement: function (event) {
            var uiEvent = _.clone(event);
            uiEvent.Description = helper.textToHtml(uiEvent.Description || '');
            uiEvent.Bros = _.chain(uiEvent.Bros).map(function(broId) {
                return _.find(self.bros, function(bro) { return bro.Id === broId; });
            }).sortBy('Name').value();
            var $event = $($.jqote(rowTemplate, uiEvent));            
            self._getEventList(event).prepend($event);                        
        },                                       
        
        _getEventList: function (event) {
            if (event.StartDateObj >= new Date()) {
                return $('#event-list-pending');
            } else {
                return $('#event-list-past');
            }            
        }              
    };
        
    return self;
});