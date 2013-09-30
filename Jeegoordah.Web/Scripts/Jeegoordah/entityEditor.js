define(['$', 'modal', 'text!templates/entityEditor.html'], function ($, modal, template) {
    return {        
        show: function($content, entity, title, saveCallback) {           
            var $editor = $(template);
            $editor.find('#entityEditorContent').empty().append($content);
            $editor.validate();
            $editor.submit(function (e) {
                e.preventDefault();
                if ($editor.valid()) {
                    saveCallback($editor.toJson());
                }
            });
            $editor.populateForm(entity);
            modal.show($editor, title, {
                okCallback: $editor.submit()                
            });
            return $editor;
        },
        hide: function() {
            modal.hide();
        }
    };
})