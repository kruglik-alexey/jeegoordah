define(['_', '$', 'rest', 'notification', 'helper', 'consts', 'entityEditor', 'entityControls',
        'text!templates/events/row.html', 'text!templates/events/editor.html', 'text!templates/events/module.html'],
    function (_, $, rest, notification, helper, consts, editor, entityControls, rowTemplate, editorTemplate, moduleTemplate) {

    var self = {
        init: function () {
            // TODO remove?
        },

        activate: function () {            
            // TODO add spinner while loading
            $.when(rest.get('bros'), rest.get('events')).done(function (bros, events) {
                self.bros = bros[0];                
                $('#modules').empty().append($(moduleTemplate));
                $('#createEventButton').click(self._showCreateEvent);
                self._loadEvents(events[0]);                                
            });                        
        },
        
        _loadEvents: function (events) {                                    
            _.each(events, function (event) {
                self._createEventElement(event);
            });            
        },
        
        _showCreateEvent: function () {
            self._showEventEditor({}, 'Create Event', function (event) {
                rest.post('events/create', event).done(function (createdEvent) {
                    self._createEventElement(createdEvent);
                    editor.close();
                    notification.success('Event created.');
                });
            });                                                 
        },
        
        _createEventElement: function (event, appendToList) {
            if (_.isUndefined(appendToList)) {
                appendToList = true;
            }
            var uiEvent = _.clone(event);
            uiEvent.Description = helper.textToHtml(uiEvent.Description || '');
            uiEvent.Bros = _.chain(uiEvent.Bros).map(function(broId) {
                return _.find(self.bros, function(bro) { return bro.Id === broId; });
            }).sortBy('Name').value();
            var $event = $($.jqote(rowTemplate, uiEvent));
            entityControls.render($event.find('h3'), _.partial(self._showEditEvent, event), _.partial(self._deleteEvent, event));
            if (appendToList) {
                self._getEventList(event).append($event);
            }            
            
            return $event;
        },
        
        _showEventEditor: function(event, title, ok) {
            var rendered = $.jqote(editorTemplate, { bros: self.bros });
            editor.show($(rendered), event, title, {
                ok: ok,
                toForm: _.bind(self._eventToForm, self),
                fromForm: _.bind(self._eventFromForm, self),
            });
        },
        
        _showEditEvent: function (event) {
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
                
        _eventToForm: function (event, $editor) {
            _.each(event.Bros, function (broId) {
                $editor.find('.bro-checkbox[data-id=' + broId + ']').addClass('active');
            });
            return event;
        },
        
        _eventFromForm: function (event, $editor) {
            event.Bros = [];
            $editor.find('.bro-checkbox.active').each(function (tmp, el) {
                event.Bros.push(parseInt($(el).attr('data-id')));
            });
            return event;
        },
        
        _getEventList: function (event) {
            if (helper.parseDate(event.StartDate) >= new Date()) {
                return $('#event-list-pending');
            } else {
                return $('#event-list-past');
            }            
        }
    };
        
    return self;
});