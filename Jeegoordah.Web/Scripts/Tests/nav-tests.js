define(['$', '../sinon', 'rest'], function ($, sinon, rest) {
    module("nav", {
        setup: function () {
            this.sandbox = sinon.sandbox.create();
        },
        teardown: function () {
            this.sandbox.restore();
        }
    });

    test('Should initialize nav', function () {
        var $nav = $('nav');
        ok($nav.length === 1, 'There is one navigator panel');
        ok($nav.is(':visible'), 'Nav is visible');
        ok($nav.find('li:visible').length === 5, 'There is 5 visible navigation links');
        ok($nav.find('li#nav-total').hasClass('active'), 'Total module is active by default');
    });

    asyncTest('Should navigate to module', function () {
        sinon.stub(rest, 'get').withArgs('events').returns($.Deferred().resolve([]));
        var $nav = $('nav');
        // You can't follow the link by calling $.click() on it
        window.location = $nav.find('li#nav-events>a').attr('href');        
        $('#module-events').wait(function ($selector) {
            ok($selector.length === 1, 'Module was rendered');
            equal(document.title, 'Jeegoordah | Events', 'Title was changed');
            equal($("#module-name").text(), 'Events', 'Module header was changed');
            start();
        });
    });
})