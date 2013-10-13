define(['$', '_', 'text!templates/entityControls.html'], function($, _, template) {
    return {        
        render: function ($target, editCallback, deleteCallback) {
            var $el = $(template);
            $el.find('button[data-action=edit]').click(editCallback);
            
            $el.find('button[data-action=delete]').click(function() {
                var $popoverTarget = $(this);
                var $content = $('<div style="min-width: 100px"><div class="btn btn-danger" data-action="Yes">Yes</div>' +
                                 '<div class="btn btn-default" data-action="No">No</div></div>');

                $content.find('[data-action=Yes]').click(function () {
                    deleteCallback();
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
            });
            
            $target.append($el);
        }
    };
})