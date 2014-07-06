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

//        [Test]
//        public void ShouldDeleteEvent()
//        {
//            Visit("#events");
//            CreateEvent(new TestEvent {Name = "Event Name", StartDate = "01-09-2013", Bros = defaultBros});
//            var e = CreateEvent(new TestEvent {Name = "Event Name2", StartDate = "01-09-2013", Bros = defaultBros});
//            IList<SnapshotElementScope> rows = GetPastEvents();
//            rows[0].Hover();
//            rows[0].FindCss("a").Click();
//            rows[0].ClickButton("Delete");
//            Browser.ClickButton("Yes");
//
//            rows = Browser.FindAllCss("#event-list-past>div", r => r.Count() == 1).ToList();
//            Assert.AreEqual(1, rows.Count);
//            AssertEvent(rows[0], e);
//
//            Visit("#events");
//
//            rows = Browser.FindAllCss("#event-list-past>div", r => r.Count() == 1).ToList();
//            Assert.AreEqual(1, rows.Count);
//            AssertEvent(rows[0], e);
//        }

//        [Test]
//        public void ShouldUpdateEvent()
//        {
//            Visit("#events");
//            var e = CreateEvent(new TestEvent
//            {
//                Name = "Event Name",
//                StartDate = "01-09-2013",
//                Description = "Event Description",
//                Bros = defaultBros
//            });
//
//            var ue = new TestEvent
//            {
//                Name = "Updated Name",
//                StartDate = "01-09-2113",
//                Description = "Update Description",
//                Bros = {"Моер", "Копыч"}
//            };
//
//            ElementScope row = Browser.FindCss("#event-list-past>div");
//            row.FindCss("a").Click();
//            row.ClickButton("Edit");
//            Assert.AreEqual(e.Name, Browser.FindField("Name").Value);
//            Assert.AreEqual(e.StartDate, Browser.FindField("StartDate").Value);
//            Assert.AreEqual(e.Description, Browser.FindField("Description").Value);
//            foreach (var bro in e.Bros)
//            {
//                Assert.True(Browser.FindAllCss("label.bro-checkbox.active").Any(i => i.Text == bro));    
//            }
//
//            FillEditor(ue);
//            Browser.ClickButton("modalOkButton");
//            Assert.IsEmpty(Browser.FindAllCss("#event-list-past>div", r => !r.Any()));
//
//            row = Browser.FindCss("#event-list-pending>div");
//            AssertEvent(row, ue);
//
//            Visit("#events");
//
//            row = Browser.FindCss("#event-list-pending>div");
//            AssertEvent(row, ue);
//        }

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

//            List<SnapshotElementScope> bros = row.FindAllCss("li").ToList();
//            Assert.True(e.Bros.OrderBy(b => b).SequenceEqual(bros.Select(b => b.Text).OrderBy(b => b)));
//
//            if (!string.IsNullOrEmpty(e.Description))
//            {
//                ElementScope description = row.FindCss("div.panel-body>div");
//                Assert.AreEqual(e.Description, description.Text);
//            }
//            else
//            {
//                Assert.False(row.FindCss("div.panel-body>div").Exists());
//            }
        }        
    }
}
