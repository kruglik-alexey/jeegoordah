define(['$', 'text!templates/modal.html'], function ($, template) {
    var $modal;

    return {
        show: function($content, title, callbacks) {
            if (!$modal) {
                $modal = $(template).modal({ show: false });                
            }
            callbacks = callbacks || {};
            $modal.find('.modal-title').text(title);
            $modal.find('.modal-body').empty().append($content);
            
            $modal.find('#modalOkButton').off('click');
            $modal.find('#modalCancelButton').off('click');
            $modal.off('hidden.bs.modal');
            $modal.on('hidden.bs.modal', function () {                
                if (callbacks && callbacks.hidden) {
                    callbacks.hidden();
                }                
            });

            $modal.find('#modalOkButton').click(callbacks.ok || $.noop);
            $modal.find('#modalCancelButton').click(callbacks.cancel || $.noop);                
            $modal.modal('show');                                                  
        },
        close: function() {
            $modal.modal('hide');            
        }
    };
})