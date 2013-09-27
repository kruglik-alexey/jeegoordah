define('notification', function () {
    return {        
        success: function(content) {
            this._notification('success', content);
        },
        error: function(content) {
            this._notification('error', content);
        },
        _notification: function (kind, content) {
            noty({
                layout: 'topCenter',
                type: kind,
                text: content,
                timeout: 2000
            });            
        }
    };
});