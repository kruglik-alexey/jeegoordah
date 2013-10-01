define(['_', '$', 'rest', 'notification', 'helper', 'consts', 'entityEditor', 'text!templates/events/row.html', 'text!templates/events/editor.html', 'text!templates/events/module.html'],
    function (_, $, rest, notification, helper, consts, editor, rowTemplate, editorTemplate, moduleTemplate) {
        
    return {
        init: function () {
            // TODO remove?
        },

        activate: function () {
            var self = this;
            $('#modules').empty().append($(moduleTemplate));
            $('#createEventButton').click(function () {
                editor.show($(editorTemplate), {}, 'Create Event', _.bind(self._createEvent, self));
            });
            $('#event-list>tr').remove();
            rest.get('events').done(function (events) {                
                // TODO add spinner while loading
                _.each(events, function (event) {                    
                    event = self._fixStartDateFormat(event);
                    self._createEventElement(event);
                });
            });
        },
        
        _createEvent: function (event) {
            var self = this;
            rest.post('events/create', event).done(function () {
                event = self._fixStartDateFormat(event);
                self._createEventElement(event);
                editor.close();
                notification.success('Event created.');                                
            });            
        },
        
        _createEventElement: function (event, appendToList) {
            var self = this;
            if (_.isUndefined(appendToList)) {
                appendToList = true;
            }
            var uiEvent = _.clone(event);
            uiEvent.Description = helper.textToHtml(uiEvent.Description || '');
            var $event = $($.jqote(rowTemplate, uiEvent));
            var eventList = $('#event-list');
            if (appendToList) {
                eventList.append($event);
            }            

            $event.find('button[data-action=edit]').click(function () {
                editor.show($(editorTemplate), event, 'Edit Event', function(updatedEvent) {
                    updatedEvent = _.extend({ Id: event.Id }, updatedEvent);
                    rest.post('events/update', updatedEvent).done(function () {
                        editor.close();
                        notification.success('Event updated.');

                        var oldElement = eventList.find('#event' + event.Id);
                        var newElement = self._createEventElement(updatedEvent, false);
                        oldElement.fadeOut(consts.fadeDuration, function () {
                            newElement.insertAfter(oldElement).hide();
                            oldElement.remove();
                            newElement.fadeIn(consts.fadeDuration);
                        });
                    });
                });
            });
            
            $event.find('button[data-action=delete]').click(function () {
                var popoverTarget = this;
                var $content = $('<div style="min-width: 100px"><div class="btn btn-danger" data-action="Yes">Yes</div>' +
                                 '<div class="btn btn-default" data-action="No">No</div></div>');
                $content.find('[data-action=Yes]').click(function() {
                    rest.post('events/delete/' + event.Id).done(function () {                        
                        $event.fadeOut(consts.fadeDuration, function() {
                            $event.remove();
                        });
                    });
                    $(popoverTarget).popover('destroy');
                });
                $content.find('[data-action=No]').click(function () {
                    $(popoverTarget).popover('destroy');
                });
                $(popoverTarget).popover({
                    title: 'Are you sure?',
                    content: $content,
                    html: true
                });
                $(popoverTarget).popover('show');
            });

            return $event;
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
        }                          
    };
});