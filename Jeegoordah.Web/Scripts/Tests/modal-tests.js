define(['$', 'modal', '../sinon'], function ($, modal, sinon) {
    module('modal', {
        setup: function () {
        },
        teardown: function () {
            modal.close();
        }
    });

    asyncTest('Should show modal', function () {
        var $content = $('<span id="test">Foobar</span>');
        var title = 'Title';
        var okCalled = false;
        modal.show($content, title, {
            ok: function() {
                okCalled = true;
            }
        });
        $('#modal:visible').wait(function ($modal) {
            ok($modal.is(':visible'), 'Modal shown');
            equal($modal.find('.modal-title').text(), title, 'Expected title');
            equal($modal.find('.modal-body>span').text(), 'Foobar', 'Content rendered');
            $modal.find('#modalOkButton').click();
            ok(okCalled, 'Ok callback called');
            start();
        });        
    });   
})