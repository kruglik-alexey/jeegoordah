define(['$', 'modal', 'text!templates/entityEditor.html'], function ($, modal, template) {
    return {        
        show: function($content, entity, title, saveCallback) {           
            var $editor = $(template);
            $editor.find('#entityEditorContent').empty().append($content);
            $editor.validate();
            $editor.submit(function (e) {
                e.preventDefault();
                if ($editor.valid()) {
                    console.log('saveCallback');
                    saveCallback($editor.toJson());
                }
            });
            $editor.populateForm(entity);
            modal.show($editor, title, {
                // TODO for some reason can't write it as _.bind($editor.submit, $editor). WTF?
                okCallback: function() {
                    $editor.submit();
                }
            });
            return $editor;
        },
        close: function() {
            modal.close();
        }
    };
})