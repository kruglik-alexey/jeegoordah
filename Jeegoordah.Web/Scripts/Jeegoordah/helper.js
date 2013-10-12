define(function() {
    return {                        
        // Replaces urls with <a> tags. Replaces newlines with <br /> tag.
        textToHtml: function(text) {            
            var exp = /(\b(https?|ftp|file):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/i;
            return text.replace(exp, "<a href='$1'>$1</a>").replace('\r\n','<br />');           
        },
        
        // Converts date string 01-10-2013 to the date object
        parseDate: function(dateStr) {
            var parts = dateStr.split('-');
            return new Date(parts[2], parts[1] - 1, parts[0]);
        }
    };
});