using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Coypu;
using Jeegoordah.Tests.Integration.Common;
using NUnit.Framework;

namespace Jeegoordah.Tests.Integration
{
    [TestFixture]
    public class EventListTests : IntegrationTest
    {
        [Test]
        public void ShouldCreateEvent()
        {            
            eventSupport.VisitEventsList();
            var e1 = eventSupport.CreateEvent(new TestEvent
            {
                Name = "Event Name",
                StartDate = "01-09-2013",
                Description = "Event Description\r\nhttp://foo.com",
                Bros = defaultBros
            });

            var rows = eventSupport.GetPastEvents();
            Assert.AreEqual(1, rows.Count);
            AssertEvent(rows[0], e1);            
        }        

        [Test]
        public void ShouldLoadEvents()
        {
            eventSupport.VisitEventsList();
            var e1 = eventSupport.CreateEvent(new TestEvent
            {
                Name = "Event Name",
                StartDate = "01-09-2013",
                Description = "Event Description\r\nhttp://foo.com",
                Bros = defaultBros
            });
            var e2 = eventSupport.CreateEvent(new TestEvent
            {
                Name = "Event Name2",
                StartDate = "01-09-2113",
                Description = "Description",
                Bros = defaultBros
            });

            Browser.Refresh();
            Browser.WaitForAjax();

            IList<SnapshotElementScope> rows = eventSupport.GetPastEvents();
            Assert.AreEqual(1, rows.Count);
            AssertEvent(rows[0], e1);

            rows = eventSupport.GetPendingEvents();
            Assert.AreEqual(1, rows.Count);
            AssertEvent(rows[0], e2);
        }

        private readonly List<string> defaultBros = new List<string> { "Шылдон", "БлекД" };
        private EventSupport eventSupport;

        public override void SetUp()
        {
            base.SetUp();
            eventSupport = new EventSupport(Browser);
        }        

        private void AssertEvent(ElementScope row, TestEvent e)
        {
            Assert.AreEqual(e.Name, row.FindCss("a").Text);                        
            Assert.AreEqual(e.StartDate, row.FindCss("small").Text);
        }        
    }
}
