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

    asyncTest('Should create event', function () {
        this.sinon.stub(rest, 'get').withArgs('events').returns($.Deferred().resolve([]));
        this.sinon.stub(rest, 'post', function (event) {
            equal(event.Name, 'TestName');
            equal(event.StartDate, '01-09-2013');
            equal(event.Description, 'Event Description');
            event.Id = 42;
            return $.Deferred().resolve(event);
        });
        $('#module-events').wait(function ($module) {
            $module.find('#createEventButton').click();
            $('#entityEditor').wait(function($editor) {
                equal($editor.find('[name=Name]').val(), '');
                equal($editor.find('[name=StartDate]').val(), '');
                equal($editor.find('[name=Description]').val(), '');

                $editor.find('[name=Name]').val('EventName');
                $editor.find('[name=StartDate]').val('01-09-2013');
                $editor.find('[name=Description]').val('Event Description');
                $editor.submit();
            });
        });
        $('tr#event42').wait(function ($event) {            
            start();
        });
    });
    
    asyncTest('Should edit event', function () {
        this.sinon.stub(rest, 'get').withArgs('events').returns($.Deferred().resolve([{
            Id: 42,
            Name: 'TestEvent',
            Description: 'Event Description',
            StartDate: '\/Date(1378846800000)\/'
        }]));
        this.sinon.stub(rest, 'post', function (event) {
            equal(event.Id, 42);
            equal(event.Name, 'Updated Name');
            equal(event.StartDate, '01-09-2013');
            equal(event.Description, 'Updated Description');
            setTimeout(function () {
                var $row = $('tr#event42');
                equal($row.length, 1);
                equal($row.find('td:nth-child(1)').text(), 'Updated Name', 'Row has name value');
                equal($row.find('td:nth-child(2)').text(), '01-09-2013', 'Row has start date value');
                equal($row.find('td:nth-child(3)>div>span').text(), 'Updated Description', 'Row has start date value');
                start();
            }, 100);
            return $.Deferred().resolve(event);
        });
        $('tr#event42').wait(function ($event) {
            $event.find('[data-action=edit]').click();
            $('#entityEditor').wait(function ($editor) {
                equal($editor.find('[name=Name]').val(), 'TestEvent');
                equal($editor.find('[name=StartDate]').val(), '11-09-2013');
                equal($editor.find('[name=Description]').val(), 'Event Description');

                $editor.find('[name=Name]').val('UpdatedName');
                $editor.find('[name=StartDate]').val('01-09-2013');
                $editor.find('[name=Description]').val('Updated Description');
                $editor.submit();
            });
        });
    });
})