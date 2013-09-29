define(['$', '../sinon', 'rest'], function ($, sinon, rest) {
    module("nav", {
        setup: function () {            
            this.sinon = sinon.sandbox.create();
        },
        teardown: function () {
            this.sinon.restore();
        }
    });

    test('Should initialize nav', function () {
        var $nav = $('nav');
        equal($nav.length, 1, 'There is one navigator panel');
        ok($nav.is(':visible'), 'Nav is visible');
        equal($nav.find('li:visible').length, 5, 'There is 5 visible navigation links');
    });
    
    asyncTest('Should navigate to Total module if hash empty', function () {
        window.location.hash = '';
        $('li#nav-total.active').wait(function ($total) {
            equal($total.length, 1, 'Total module active');
            start();
        });        
    });

    asyncTest('Should navigate to module', function () {
        $('#module-events').remove();
        this.sinon.stub(rest, 'get').withArgs('events').returns($.Deferred().resolve([]));
        var $nav = $('nav');       
        // You can't follow the link by calling $.click() on it
        window.location = $nav.find('li#nav-events>a').attr('href');
        
        $('#module-events').wait(function ($selector) {
            equal($selector.length, 1, 'Module was rendered');
            equal(document.title, 'Jeegoordah | Events', 'Title was changed');
            equal($("#module-name").text(), 'Events', 'Module header was changed');
            ok($('li#nav-events').hasClass('active'), 'Module nav element was activated');
            start();
        });
    });
})