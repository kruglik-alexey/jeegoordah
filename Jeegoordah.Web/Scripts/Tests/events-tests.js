define(['$', '../sinon', 'rest'], function ($, sinon, rest) {
    module("events module", {
        setup: function () {
            this.sinon = sinon.sandbox.create();
            window.location.hash = 'events';
        },
        teardown: function () {
            this.sinon.restore();
            window.location.hash = '';
        }
    });       

    asyncTest('Should render template', function () {        
        this.sinon.stub(rest, 'get').withArgs('events').returns($.Deferred().resolve([]));                
        $('#module-events').wait(function ($module) {
            equal($module.find('#createEventButton').length, 1, 'Renders create button');
            equal($module.find('tbody#event-list').length, 1, 'Renders event list');
            start();
        });
    });
    
    asyncTest('Should render events', function () {
        this.sinon.stub(rest, 'get').withArgs('events').returns($.Deferred().resolve([{
            Id: 42,
            Name: 'TestEvent',
            Description: 'Event Description here: http://foo.com',
            StartDate: '\/Date(1378846800000)\/'
        }]));
        $('tbody#event-list>tr').wait(function ($events) {
            equal($events.length, 1, 'Renders one event row');
            equal($events.attr('id'), 'event42', 'Row has event id');
            equal($events.find('td').length, 3, 'Row has 3 columns');
            equal($events.find('td:nth-child(1)').text(), 'TestEvent', 'Row has name value');
            equal($events.find('td:nth-child(2)').text(), '11-09-2013', 'Row has start date value');
            equal($events.find('td:nth-child(3)>div>span').text(), 'Event Description here: http://foo.com', 'Row has description value');
            equal($events.find('td:nth-child(3)>div>span>a').text(), 'http://foo.com', 'Description has <a> tag');
            equal($events.find('td:nth-child(3)>div>span>a').attr('href'), 'http://foo.com', 'Description has <a> tag with right href');
            start();
        });  
    });

//    asyncTest('Should create event', function () {
//        this.sinon.stub(rest, 'get').withArgs('events').returns($.Deferred().resolve([]));
//        $('#module-events').wait(function ($module) {
//            $module.find('#createEventButton').click();
//            ok();
//            start();
//        });
//    });
})