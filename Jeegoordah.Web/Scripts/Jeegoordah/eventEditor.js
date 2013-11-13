define(['$', '_', 'broSelector', 'entityEditor', 'notification', 'text!templates/events/editor.html'],
function ($, _, broSelector, entityEditor, notification, editorTemplate) {
    var self = {        
        edit: function(event, bros, title, ok) {
            self.event = event;
            self.bros = bros;
            self._showEditor(title, ok);
        },
        
        close: function() {
            entityEditor.close();
        },
        
        _showEditor: function (title, ok) {
            var rendered = $(editorTemplate);
            self.$broSelector = rendered.find('#eventBros');
            self.$broSelector.append(broSelector.render(false, self.bros));
            entityEditor.show(rendered, self.event, title, {
                ok: ok,
                toForm: self._bindEvent,
                fromForm: self._unbindEvent,
                validate: self._validateEvent,
            });
        },
        
        _bindEvent: function (event, $editor) {
            broSelector.bind(self.$broSelector, event.Bros);
            return event;
        },

        _unbindEvent: function (event, $editor) {
            event.Bros = broSelector.unbind(self.$broSelector);
            return event;
        },
        
        _validateEvent: function ($editor) {
            var targets = broSelector.unbind(self.$broSelector);
            if (targets.length < 2) {
                notification.error('Event should has at least two Bros');
                return false;
            }
            return true;
        }                
    };
    return self;
})