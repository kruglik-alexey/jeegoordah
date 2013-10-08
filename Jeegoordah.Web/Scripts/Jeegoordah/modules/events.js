define(['_', '$', 'rest', 'notification', 'helper', 'consts', 'entityEditor', 'text!templates/events/row.html', 'text!templates/events/editor.html', 'text!templates/events/module.html'],
    function (_, $, rest, notification, helper, consts, editor, rowTemplate, editorTemplate, moduleTemplate) {

    var self = {
        init: function () {
            // TODO remove?
        },

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
                event = self._fixStartDateFormat(event);
                self._createEventElement(event);
            });            
        },
        
        _createEvent: function () {
            var rendered = $.jqote(editorTemplate, { bros: self.bros });
            editor.show($(rendered), {}, 'Create Event', {
                ok: function (event) {
                    rest.post('events/create', event).done(function (createdEvent) {
                        createdEvent = self._fixStartDateFormat(createdEvent);
                        self._createEventElement(createdEvent);
                        editor.close();
                        notification.success('Event created.');
                    });
                },
                toForm: _.bind(self._eventToForm, self),
                fromForm: _.bind(self._eventFromForm, self),
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
            if (appendToList) {
                self._getEventList(event).append($event);
            }            

            $event.find('button[data-action=edit]').click(_.partial(self._editEvent, event));
            $event.find('button[data-action=delete]').click(_.partial(self._deleteEvent, event));
            return $event;
        },
        
        _editEvent: function (event) {                      
            var rendered = $.jqote(editorTemplate, { bros: self.bros });
            editor.show($(rendered), event, 'Edit Event', {
                ok: function(updatedEvent) {
                    updatedEvent = _.extend({ Id: event.Id }, updatedEvent);
                    rest.post('events/update', updatedEvent).done(function() {
                        editor.close();
                        notification.success('Event updated.');
                        self._updateEventElement(updatedEvent);
                    });
                },
                toForm: _.bind(self._eventToForm, self),
                fromForm: _.bind(self._eventFromForm, self),
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
            var $popoverTarget = $event.find('button[data-action=delete]');
            var $content = $('<div style="min-width: 100px"><div class="btn btn-danger" data-action="Yes">Yes</div>' +
                             '<div class="btn btn-default" data-action="No">No</div></div>');
            
            $content.find('[data-action=Yes]').click(function () {
                rest.post('events/delete/' + event.Id).done(function () {
                    $event.fadeOut(consts.fadeDuration, function () {
                        $event.remove();
                    });
                });
                $popoverTarget.popover('destroy');
            });
            
            $content.find('[data-action=No]').click(function () {
                $popoverTarget.popover('destroy');
            });
            
            $popoverTarget.popover({
                title: 'Are you sure?',
                content: $content,
                html: true
            });
            $popoverTarget.popover('show');
        },
        
        _fixStartDateFormat: function (event) {
            // This method called on deferred result. We can't change event object because deferred may have several subscribers.
            var e = _.clone(event);
            if (e.StartDate) {
                e.StartDate = helper.fixJsonDate(e.StartDate);
            } else {
                e.StartDate = '';
            }
            return e;
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