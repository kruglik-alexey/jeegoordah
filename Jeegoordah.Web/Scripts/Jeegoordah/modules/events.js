define(['_', '$', 'rest', 'notification', 'helper', 'consts'], function (_, $, rest, notification, helper, consts) {
    return {
        init: function () {
            var self = this;
            this.eventEditorDialog = $('#eventEditor').modal({ show: false });
            this.eventEditorDialog.form = this.eventEditorDialog.find('#createEventForm');
            this.eventEditorDialog.find('#saveEventButton').click(function () {
                self.eventEditorDialog.form.submit();
            });           
            this.eventEditorDialog.form.submit(function (e) {
                if (self.eventEditorDialog.form.valid()) {
                    self.eventEditorDialog.save();
                }
                e.preventDefault();
            });
            this.eventEditorDialog.form.validator = this.eventEditorDialog.form.validate();
            this.eventEditorDialog.on('hidden.bs.modal', function () {                
                self.eventEditorDialog.form.validator.resetForm();
                self.eventEditorDialog.form[0].reset();
            });
            
            $('#createEventButton').click(function () {
                self.eventEditorDialog.find('.modal-title').text('Create Event');
                self.eventEditorDialog.save = _.bind(self._createEvent, self);
                self.eventEditorDialog.modal('show');
            });
        },

        activate: function () {
            var self = this;
            $('#event-list>tr').remove();
            rest.get('events').done(function (events) {
                // TODO add spinner while loading
                _.each(events, function (event) {
                    self._fixStartDateFormat(event);
                    self._createEventElement(event);
                });
            });
        },
        
        _createEvent: function () {
            var self = this;
            rest.post('events/create', self.eventEditorDialog.form.toJson()).done(function (event) {
                self._fixStartDateFormat(event);
                self._createEventElement(event);
                self.eventEditorDialog.modal('hide');
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
            var $event = $($.jqote(this.eventTemplate, uiEvent));
            var eventList = $('#event-list');
            if (appendToList) {
                eventList.append($event);
            }            

            $event.find('button[data-action=edit]').click(function() {
                self.eventEditorDialog.find('.modal-title').text('Edit Event');
                self.eventEditorDialog.form.populateForm(event);
                self.eventEditorDialog.save = function () {
                    var updatedEvent = _.extend({Id: event.Id}, self.eventEditorDialog.form.toJson());
                    rest.post('events/update', updatedEvent).done(function () {
                        self.eventEditorDialog.modal('hide');
                        notification.success('Event updated.');
                        
                        var oldElement = eventList.find('#event' + event.Id);
                        var newElement = self._createEventElement(updatedEvent, false);
                        oldElement.fadeOut(consts.fadeDuration, function() {
                            newElement.insertAfter(oldElement).hide();
                            oldElement.remove();
                            newElement.fadeIn(consts.fadeDuration);
                        });                                                
                    });
                };
                self.eventEditorDialog.modal('show');
            });
            
            $event.find('button[data-action=delete]').click(function () {
                var popoverTarget = this;
                var $content = $('<div style="width=100%"><button type="button" class="btn btn-danger" data-action="Yes">Yes</button>' +
                                 '<button type="button" class="btn btn-default" data-action="No">No</button></div>');
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
        
        _fixStartDateFormat: function(event) {
            if (event.StartDate) {
                event.StartDate = helper.fixJsonDate(event.StartDate);
            } else {
                event.StartDate = '';
            }                           
        },
               
        eventTemplate: 
            '<tr id="event<%= this.Id %>">' +
                '<td><%= this.Name %></td>' +
                '<td class="auto-width"><%= this.StartDate %></td>' +
                '<td><div class="entity-controls-host">' +
                    '<%= this.Description %>' +
                    '<div class="entity-controls btn-group btn-group-xs">' +
                        '<button type="button" class="btn btn-default" data-action="edit">Edit</button>' +
                        '<button type="button" class="btn btn-danger" data-action="delete">Delete</button>' +
                    '</div>' +                    
                '</div></td>' +
            '</tr>'        
    };
});