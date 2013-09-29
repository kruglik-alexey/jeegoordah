define(['jquery'], function ($) {    
    $.fn.toJson = function() {
        var o = {};
        var a = this.serializeArray();
        $.each(a, function() {
            if (o[this.name] !== undefined) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        return o;
    };

    $.fn.populateForm = function (json) {
        var $form = this;
        $.each(json, function (name, val) {
            var $el = $form.find('[name="' + name + '"]');
            var type = $el.attr('type');
            switch (type) {
                case 'checkbox':
                    $el.attr('checked', 'checked');
                    break;
                case 'radio':
                    $el.filter('[value="' + val + '"]').attr('checked', 'checked');
                    break;
                default:
                    $el.val(val);
            }
        });
    };

    // Wait for selector to return at least one element or for timeout.    
    $.fn.wait = function (callback, start) {
        if (!start) {            
            start = new Date();
        }
                
        var $selector = $(this.selector);
        if ($selector.length > 0 || new Date() - start > 500) {            
            $.proxy(callback, $selector)($selector);
            return;
        }

        var callee = arguments.callee;
        var self = this;
        var timeout = setTimeout(function () {
            clearTimeout(timeout);
            callee.apply(self, [callback, start]);
        }, 10);
    };

    return $.noConflict();
});