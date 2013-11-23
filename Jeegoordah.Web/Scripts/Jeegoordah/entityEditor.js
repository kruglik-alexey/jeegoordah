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
                formEntity = callbacks.toForm(formEntity, $editor);
            }
            $editor.populateForm(formEntity);
            
            modal.show($editor, title, {
                // for some reason can't write it as _.bind($editor.submit, $editor). WTF?
                ok: function() {
                    $editor.submit();
                },
                cancel: callbacks.cancel
            });
            return $editor;
        },
        
        close: function() {
            modal.close();
        },
        
        _submit: function ($editor, callbacks) {
            if ($editor.valid() && (!callbacks.validate || callbacks.validate($editor))) {
                var r = $editor.toJson();
                if (callbacks.fromForm) {
                    r = callbacks.fromForm(r, $editor);
                }
                callbacks.ok(r);
            }
        }
    };

    return self;
})