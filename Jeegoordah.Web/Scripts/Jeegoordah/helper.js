define(['_', '$'], function(_, $) {
    var self = {                        
        // Replaces urls with <a> tags. Replaces newlines with <br /> tag.
        textToHtml: function (text) {
            var exp = /(\b(https?|ftp|file):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/i;
            text = String(text).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');            
            return text.replace(exp, '<a href="$1" target="_blank">$1</a>').replace('\r\n','<br />');           
        },
        
        // Converts date string 01-10-2013 to the date object
        parseDate: function(dateStr) {
            var parts = dateStr.split('-');
            if (parts[1].charAt(0) === '0') {
                parts[1] = parts[1].charAt(1);
            }
            if (parts[0].charAt(0) === '0') {
                parts[0] = parts[0].charAt(1);
            }
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
        },
        
        withAccuracy: function(number, accuracy) {
            if (accuracy == 0) {
                return number;
            }
            if (accuracy > 0) {
                var factor = Math.pow(10, accuracy);
                return Math.round(number / factor) * factor;
            }
            throw new Error('Not implemented, duh');
        },
        
        template: function (template, obj) {
            obj = obj || {};
            obj.helper = self;
            return $(_.template(template, obj, {variable: 'self'}));
        },
        
        getSignClass: function(amount) {
            if (amount === 0) return 'zero';
            if (amount > 0) return 'positive';
            return 'negative';
        },
        
        plural: function (number, singular, plural) {
            return number > 1 ? plural : singular;
        },
        
        formatNumber: function(number) {
            return $.number(number, 0, '.', ' ');
        },

        isToday: function(date) {
            var today = new Date();
            today.setHours(0, 0, 0, 0);
            return date.valueOf() === today.valueOf();
        },

        getQueryVariable: function(variable) {
            var query = window.location.search.substring(1);
            var vars = query.split("&");
            for (var i = 0; i < vars.length; i++) {
                var pair = vars[i].split("=");
                if (pair[0] == variable) {
                    return pair[1];
                }
            }
            return false;
        }
    };
    return self;
});