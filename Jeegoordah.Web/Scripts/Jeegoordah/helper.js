define(function() {
    return {        
        fixJsonDate: function(date) {
            var d = new Date(parseInt(date.substr(6)));
            var day = d.getDate();
            if (day < 10) {
                day = '0' + day;
            }
            // yeah, month is 0-indexed
            var month = d.getMonth() + 1;
            if (month < 10) {
                month = '0' + month;
            }            
            return day + "-" + month + "-" + d.getFullYear();            
        },
        
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