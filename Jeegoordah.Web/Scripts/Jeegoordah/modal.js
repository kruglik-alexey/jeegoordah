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
                                   
            $modal.off('hidden.bs.modal');
            $modal.on('hidden.bs.modal', function () {                
                if (callbacks && callbacks.hidden) {
                    callbacks.hidden();
                }                
            });

            var assignClick = function(selector, click) {
                var target = $modal.find(selector);
                target.off('click');
                target.click(click || $.noop);
            };

            assignClick('#modalOkButton', callbacks.ok);
            assignClick('#modalCancelButton', callbacks.cancel);
            assignClick('[data-dismiss=modal]', callbacks.cancel);
                      
            $modal.modal('show');                                                  
        },
        
        close: function() {
            $modal.modal('hide');            
        }
    };
})