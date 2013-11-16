define(['$', '_', 'text!templates/bro-total/module.html'], function($, _, moduleTemplate) {
    var self = {        
        activate: function() {
            $('#modules').empty().append($($.jqoute(moduleTemplate, {Name: 'Bro'})));
        }
    };
    return self;
})