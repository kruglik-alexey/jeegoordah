define(['$', '_', 'helper', 'text!templates/broSelector.html'], function ($, _, helper, template) {
    var self = {
        render: function (isRadio, bros) {            
            var $selector = helper.template(template, { isRadio: isRadio, bros: bros });
            var $buttons = $selector.find('.btn');
            $buttons.click(function () {
                if (isRadio) {
                    $buttons.removeClass('active');
                }
                var $this = $(this);
                if ($this.hasClass('active') && !isRadio) {
                    $this.removeClass('active');
                } else {
                    $this.addClass('active');
                }                
            });
            return $selector;
        },

        bind: function ($element, bros) {
            if (_.isUndefined(bros)) {
                bros = [];
            }
            if (!_.isArray(bros)) {
                bros = [bros];
            }
            _.each(bros, function (bro) {
                $element.find('[data-id=' + bro + ']').addClass('active');
            });
        },

        unbind: function ($element) {
            var bros = [];
            $element.find('.active').each(function (tmp, el) {
                bros.push(parseInt($(el).attr('data-id')));
            });
            if ($element.find('div').attr('data-radio') === 'true') {
                return bros[0];
            } else {
                return bros;
            }            
        }
    };
    return self;
})