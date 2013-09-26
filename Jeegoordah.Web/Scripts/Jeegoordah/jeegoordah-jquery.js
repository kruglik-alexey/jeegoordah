define('jeegoordah-jquery', ['jquery'], function ($) {    
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

    return $.noConflict();
});