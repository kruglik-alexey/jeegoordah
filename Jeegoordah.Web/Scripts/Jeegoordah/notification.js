// TODO They're ugly.
define('notification', ['$'], function ($) {
    return {        
        success: function(content) {
            this._notification('success', content);
        },
        error: function(content) {
            this._notification('danger', content);
        },
        _notification: function(kind, content) {
            var notification = $('#notification');
            notification.find('#notification-content').text(content);
            notification.removeClass('alert-success alert-danger').addClass('alert-' + kind);
            notification.show();
        }
    };
});