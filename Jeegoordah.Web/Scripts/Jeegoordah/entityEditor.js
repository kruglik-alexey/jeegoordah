define(['$', 'modal', 'text!templates/entityEditor.html'], function ($, modal, template) {
    var self = {        
        show: function ($content, entity, title, callbacks) {
            var $editor = $(template);
            $editor.find('#entityEditorContent').empty().append($content);
            $editor.validate();
            $editor.submit(function (e) {
                e.preventDefault();
                self._submit($editor, callbacks);
            });

            var formEntity = entity;
            if (callbacks.toForm) {
                formEntity = callbacks.toForm(formEntity);
            }
            $editor.populateForm(formEntity);
            
            modal.show($editor, title, {
                // TODO for some reason can't write it as _.bind($editor.submit, $editor). WTF?
                ok: function() {
                    $editor.submit();
                }
            });
            return $editor;
        },
        
        close: function() {
            modal.close();
        },
        
        _submit: function ($editor, callbacks) {
            if ($editor.valid()) {
                var r = $editor.toJson();
                if (callbacks.fromForm) {
                    r = callbacks.fromForm(r);
                }
                callbacks.ok(r);
            }
        }
    };

    return self;
})