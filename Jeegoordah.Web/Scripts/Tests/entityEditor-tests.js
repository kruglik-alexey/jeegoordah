define(['$', 'entityEditor'], function($, editor) {
    module('entityEditor', {
        setup: function () {
        },
        teardown: function () {
            editor.close();
        }
    });

    asyncTest('Should show editor', function () {
        var $content = $('<input id="testInput" type="text" name="test">');
        editor.show($content, { test: 'testValue' }, 'Title', function (entity) {
            equal(entity.test, 'newValue', 'Save callback called with updated entity');
            start();
        });        
        $('#entityEditor:visible').wait(function ($editor) {
            ok($editor.is(':visible'), 'Editor is visible');
            var $testInput = $editor.find('#testInput');
            ok($testInput.is(':visible'), 'Test input is visible');
            equal($testInput.val(), 'testValue', 'Test input has value');
            $testInput.val('newValue');
            $('#modalOkButton').click();            
        });
    });
    
    asyncTest('Should validate form', function () {
        var $content = $('<input id="testInput" type="text" name="test" required>');
        editor.show($content, {}, '', function (entity) {
            fail();            
        });
        $('#entityEditor:visible').wait(function ($editor) {
            $('#modalOkButton').click();
            $editor.find('label.error[for=test]').wait(function ($label) {
                ok(true, 'Validation error shown');
                start();
            });
        });        
    });
    
    asyncTest('Should submit form by enter', function () {
        var $content = $('<input id="testInput" type="text" name="test">');
        editor.show($content, {}, '', function (entity) {
            ok(true, 'Form submitted');
            start();
        });
        $('#entityEditor:visible').wait(function ($editor) {
            // Sending keypress event to the input or form won't cause submit to happen. So can't test this in true way.
            // At least test thta there is (hidden) submit button and it actually submits.
            $editor.find('input[type=submit]').click();
        });
    });
})