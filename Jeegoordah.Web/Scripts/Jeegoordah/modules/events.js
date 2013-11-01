define(['_', '$', 'rest', 'notification', 'helper', 'consts', 'entityEditor', 'entityControls', 'broSelector',
        'text!templates/events/row.html', 'text!templates/events/editor.html', 'text!templates/events/module.html'],
    function (_, $, rest, notification, helper, consts, editor, entityControls, broSelector, rowTemplate, editorTemplate, moduleTemplate) {

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
            _.each(events, function (event) {
                self._createEventElement(event);
            });            
        },
        
        _createEvent: function () {
            self._showEventEditor({}, 'Create Event', function (event) {
                rest.post('events/create', event).done(function (createdEvent) {
                    self._createEventElement(createdEvent);
                    editor.close();
                    notification.success('Event created.');
                });
            });                                                 
        },
        
        _createEventElement: function (event, appendToList) {
            var uiEvent = _.clone(event);
            uiEvent.Description = helper.textToHtml(uiEvent.Description || '');
            uiEvent.Bros = _.chain(uiEvent.Bros).map(function(broId) {
                return _.find(self.bros, function(bro) { return bro.Id === broId; });
            }).sortBy('Name').value();
            var $event = $($.jqote(rowTemplate, uiEvent));
            entityControls.render($event.find('h3'), _.partial(self._editEvent, event), _.partial(self._deleteEvent, event));
            if (_.isUndefined(appendToList) || appendToList) {
                self._getEventList(event).append($event);
            }            
            
            return $event;
        },
        
        _showEventEditor: function(event, title, ok) {
            var rendered = $(editorTemplate);
            rendered.find('#eventBros').append(broSelector.render(false, self.bros));
            editor.show(rendered, event, title, {
                ok: ok,
                toForm: self._bindEvent,
                fromForm: self._unbindEvent,
                validate: self._validateEvent,
            });
        },
        
        _editEvent: function (event) {
            self._showEventEditor(event, 'Edit Event', function (updatedEvent) {
                updatedEvent = _.extend({ Id: event.Id }, updatedEvent);
                rest.post('events/update', updatedEvent).done(function () {
                    editor.close();
                    notification.success('Event updated.');
                    self._updateEventElement(updatedEvent);
                });
            });                    
        },
        
        _updateEventElement: function (event) {
            var oldElement = $('#event' + event.Id);
            var newElement = self._createEventElement(event, false);
            newElement.find('#collapse' + event.Id).addClass('in');
            newElement.hide();
            oldElement.fadeOut(consts.fadeDuration, function () {
                var newList = self._getEventList(event);
                var oldList = oldElement.parent();
                if (newList.is(oldList)) {
                    newElement.insertAfter(oldElement);
                } else {
                    newList.append(newElement);
                }
                
                oldElement.remove();
                newElement.fadeIn(consts.fadeDuration);
            });
        },
        
        _deleteEvent: function(event) {
            var $event = $('#event' + event.Id);
            rest.post('events/delete/' + event.Id).done(function () {
                $event.fadeOut(consts.fadeDuration, function () {
                    $event.remove();
                });
            });                       
        },
                
        _bindEvent: function (event, $editor) {
            broSelector.bind($editor.find('#eventBros'), event.Bros);            
            return event;
        },
        
        _unbindEvent: function (event, $editor) {            
            event.Bros = broSelector.unbind($editor.find('#eventBros'));
            return event;
        },
        
        _getEventList: function (event) {
            if (helper.parseDate(event.StartDate) >= new Date()) {
                return $('#event-list-pending');
            } else {
                return $('#event-list-past');
            }            
        },
        
        _validateEvent: function ($editor) {
            var targets = broSelector.unbind($editor.find('#eventBros'));
            if (targets.length < 2) {
                notification.error('Event should has at least two Bros');
                return false;
            }
            return true;
        }
    };
        
    return self;
});