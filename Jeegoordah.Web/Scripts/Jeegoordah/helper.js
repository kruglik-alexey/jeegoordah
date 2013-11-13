define(['_'], function(_) {
    return {                        
        // Replaces urls with <a> tags. Replaces newlines with <br /> tag.
        textToHtml: function(text) {            
            var exp = /(\b(https?|ftp|file):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/i;
            return text.replace(exp, '<a href="$1" target="_blank">$1</a>').replace('\r\n','<br />');           
        },
        
        // Converts date string 01-10-2013 to the date object
        parseDate: function(dateStr) {
            var parts = dateStr.split('-');
            return new Date(parts[2], parts[1] - 1, parts[0]);
        },

        // Converts date to string of format 01-10-2013
        dateToString: function (date) {
            var lead = function(num) {
                if (num < 10) {
                    num = '0' + num;
                }
                return num;
            };
            return lead(date.getDate()) + '-' + lead(date.getMonth() + 1) + '-' + date.getFullYear();
        },
        
        equalArrays: function (array1, array2) {
            if (array1.length !== array2.length) {
                return false;
            }
            return _.union(array1, array2).length === array1.length;
        }
    };
});