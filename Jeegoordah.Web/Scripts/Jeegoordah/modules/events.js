define(['_', '$', 'rest', 'notification', 'helper'], function (_, $, rest, notification, helper) {
    return {
        init: function () {
            var self = this;
            this.eventEditorDialog = $('#eventEditor').modal({ show: false });
            this.eventEditorDialog.form = this.eventEditorDialog.find('#createEventForm');
            this.eventEditorDialog.find('#saveEventButton').click(function () {
                self.eventEditorDialog.form.submit();
            });
            this.eventEditorDialog.form.keypress(function (e) {
                if (e.which == 13) {
                    self.eventEditorDialog.form.submit();
                }
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
                _.each(events, function(e) {
                    self._createEventElement(e);
                });
            });
        },
        
        _createEvent: function () {
            var self = this;
            rest.post('events/create', self.eventEditorDialog.form.toJson()).done(function (event) {
                self._createEventElement(event);
                self.eventEditorDialog.modal('hide');
                notification.success('Event created.');                                
            });            
        },
        
        _createEventElement: function (event) {
            var self = this;
            if (event.StartDate) {
                event.StartDate = helper.fixJsonDate(event.StartDate);
            } else {
                event.StartDate = '';
            }            
            event.Description = helper.textToHtml(event.Description || '');
            var $event = $($.jqote(this.eventTemplate, event));            
            var eventList = $('#event-list');
            eventList.append($event);

            $event.find('button[data-action=edit]').click(function() {
                self.eventEditorDialog.find('.modal-title').text('Edit Event');
                self.eventEditorDialog.form.populateForm(event);
                self.eventEditorDialog.save = function() {
                    rest.post('events/update/' + event.id, self.eventEditorDialog.form.toJson()).done(function (updatedEvent) {
                        eventList.find('#event' + event.Id).replaceWith(self._createEventElement(updatedEvent));
                        self.eventEditorDialog.modal('hide');
                        notification.success('Event updated.');
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
                        $event.fadeOut(200, function() {
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