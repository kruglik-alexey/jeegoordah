define(['_', '$', 'rest', 'notification', 'helper'], function (_, $, rest, notification, helper) {
    return {
        init: function () {
            var self = this;
            var form = $('#createEventForm');
            form.on('submit', function (e) {                
                e.preventDefault();
                self._createEvent(form);
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
        
        _createEvent: function (form) {
            var self = this;
            rest.post('events/create', form.toJson()).done(function (event) {
                self._createEventElement(event);
                form[0].reset();
                form.find('[name=Name]').focus();
                notification.success('Event created.');                                
            });            
        },
        
        _createEventElement: function (event) {
            if (event.StartDate) {
                event.StartDate = helper.fixJsonDate(event.StartDate);
            } else {
                event.StartDate = '';
            }            
            event.Description = helper.textToHtml(event.Description || '');
            var $event = $($.jqote(this.eventTemplate, event));            
            $('#event-list').append($event);

            $event.find('button[data-action=edit]').click(function() {
                console.log('edit ' + event.Id);
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
            '<tr>' +
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