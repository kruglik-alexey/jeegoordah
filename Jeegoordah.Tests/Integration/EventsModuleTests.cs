using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Coypu;
using Jeegoordah.Core;
using NUnit.Framework;

namespace Jeegoordah.Tests.Integration
{
    [TestFixture]
    public class EventsModuleTests : IntegrationTest
    {
        class TestEvent
        {
            public TestEvent()
            {
                Name = "";
                Description = "";
                StartDate = "";
                Bros = new List<string>();
            }

            public string Name { get; set; }
            public string StartDate { get; set; }
            public string Description { get; set; }
            public List<string> Bros { get; set; }
        }

        private TestEvent CreateEvent(TestEvent e)
        {
            Browser.ClickButton("createEventButton");
            FillEditor(e);
            Browser.ClickButton("modalOkButton");
            return e;
        }

        private void FillEditor(TestEvent e)
        {
            Browser.FillIn("Name").With(e.Name);
            Browser.FillIn("StartDate").With(e.StartDate);
            Browser.FillIn("Description").With(e.Description);
            foreach (var checkbox in Browser.FindAllCss("label.bro-checkbox.active"))
            {
                checkbox.Click();
            }
            foreach (string bro in e.Bros)
            {
                Browser.ClickButton(bro);
            }
        }

        private void AssertEvent(ElementScope row, TestEvent e)
        {
            Assert.AreEqual(e.Name, row.FindCss("a.accordion-toggle").Text);
            if (!row.FindCss(".panel-collapse.in").Exists())
            {
                row.FindCss("a.accordion-toggle").Click();
            }            
            Assert.AreEqual(e.Name, row.FindCss("h3>span").Text);
            Assert.AreEqual(e.StartDate, row.FindCss("p").Text);

            List<SnapshotElementScope> bros = row.FindAllCss("li").ToList();
            Assert.True(e.Bros.OrderBy(b => b).SequenceEqual(bros.Select(b => b.Text).OrderBy(b => b)));

            if (!string.IsNullOrEmpty(e.Description))
            {
                ElementScope description = row.FindCss("div.panel-body>div");
                Assert.AreEqual(e.Description, description.Text);
            }
            else
            {
                Assert.False(row.FindCss("div.panel-body>div").Exists());
            }
        }

        [Test]
        public void ShouldCreateEvent()
        {            
            Visit("#events");
            var e1 = CreateEvent(new TestEvent
            {
                Name = "Event Name",
                StartDate = "01-09-2013",
                Description = "Event Description\r\nhttp://foo.com",
                Bros = {"Шылдон"}
            });
            var e2 = CreateEvent(new TestEvent
            {
                Name = "Event Name2",
                StartDate = "01-09-2113",
                Description = "Description",
                Bros = {}
            });

            List<SnapshotElementScope> rows = Browser.FindAllCss("#event-list-past>div", r => r.Count() == 1).ToList();
            Assert.AreEqual(1, rows.Count);
            AssertEvent(rows[0], e1);

            rows = Browser.FindAllCss("#event-list-pending>div", r => r.Count() == 1).ToList();
            Assert.AreEqual(1, rows.Count);
            AssertEvent(rows[0], e2);
        }

        [Test]
        public void ShouldDeleteEvent()
        {
            Visit("#events");
            var e1 = CreateEvent(new TestEvent {Name = "Event Name", StartDate = "01-09-2013"});
            var e2 = CreateEvent(new TestEvent {Name = "Event Name2", StartDate = "01-09-2013"});
            List<SnapshotElementScope> rows = Browser.FindAllCss("#event-list-past>div", r => r.Count() == 2).ToList();
            rows[0].FindCss("a").Click();
            rows[0].ClickButton("Delete");
            Browser.ClickButton("Yes");

            rows = Browser.FindAllCss("#event-list-past>div", r => r.Count() == 1).ToList();
            Assert.AreEqual(1, rows.Count);
            AssertEvent(rows[0], e2);

            Visit("#events");

            rows = Browser.FindAllCss("#event-list-past>div", r => r.Count() == 1).ToList();
            Assert.AreEqual(1, rows.Count);
            AssertEvent(rows[0], e2);            
        }

        [Test]
        public void ShouldUpdateEvent()
        {
            Visit("#events");
            var e = CreateEvent(new TestEvent
            {
                Name = "Event Name",
                StartDate = "01-09-2013",
                Description = "Event Description",
                Bros = {"Шылдон", "Моер"}
            });

            var ue = new TestEvent
            {
                Name = "Updated Name",
                StartDate = "01-09-2113",
                Description = "Update Description",
                Bros = {"Моер", "Копыч"}
            };

            ElementScope row = Browser.FindCss("#event-list-past>div");
            row.FindCss("a").Click();
            row.ClickButton("Edit");
            Assert.AreEqual(e.Name, Browser.FindField("Name").Value);
            Assert.AreEqual(e.StartDate, Browser.FindField("StartDate").Value);
            Assert.AreEqual(e.Description, Browser.FindField("Description").Value);
            foreach (var bro in e.Bros)
            {
                Assert.True(Browser.FindAllCss("label.bro-checkbox.active").Any(i => i.Text == bro));    
            }

            FillEditor(ue);
            Browser.ClickButton("modalOkButton");
            Assert.IsEmpty(Browser.FindAllCss("#event-list-past>div", r => !r.Any()));

            row = Browser.FindCss("#event-list-pending>div");
            AssertEvent(row, ue);

            Visit("#events");

            row = Browser.FindCss("#event-list-pending>div");
            AssertEvent(row, ue);
        }

        [Test]
        public void ShouldLoadEvents()
        {
            Visit("#events");
            var e1 = CreateEvent(new TestEvent
            {
                Name = "Event Name",
                StartDate = "01-09-2013",
                Description = "Event Description\r\nhttp://foo.com",
                Bros = { "Шылдон" }
            });
            var e2 = CreateEvent(new TestEvent
            {
                Name = "Event Name2",
                StartDate = "01-09-2113",
                Description = "Description",
                Bros = { }
            });

            Visit("#events");

            List<SnapshotElementScope> rows = Browser.FindAllCss("#event-list-past>div", r => r.Count() == 1).ToList();
            Assert.AreEqual(1, rows.Count);
            AssertEvent(rows[0], e1);

            rows = Browser.FindAllCss("#event-list-pending>div", r => r.Count() == 1).ToList();
            Assert.AreEqual(1, rows.Count);
            AssertEvent(rows[0], e2);
        }

        [Test]
        public void ShouldNotCreateEventWithDuplicatedName()
        {
            Visit("#events");
            CreateEvent(new TestEvent {Name = "Event Name", StartDate = "01-09-2013"});
            CreateEvent(new TestEvent {Name = "Event Name", StartDate = "01-09-2113"});

            Assert.NotNull(Browser.FindCss("#entityEditor"));
            Browser.ClickButton("Cancel");
            List<SnapshotElementScope> rows = Browser.FindAllCss("#event-list-past>div", r => r.Count() == 1).ToList();
            Assert.AreEqual(1, rows.Count);
        }

        [Test]
        public void ShouldNotUpdateEventWithDuplicatedName()
        {
            Visit("#events");
            var e1 = CreateEvent(new TestEvent {Name = "Event Name", StartDate = "01-09-2013"});
            var e2 = CreateEvent(new TestEvent {Name = "Event Name2", StartDate = "01-09-2013"});

            ElementScope row = Browser.FindCss("#event-list-past>div");
            row.FindCss("a").Click();
            row.ClickButton("Edit");
            Browser.FillIn("Name").With("Event Name2");
            Browser.ClickButton("Save");
            Assert.NotNull(Browser.FindCss("#entityEditor"));
            Browser.ClickButton("Cancel");

            List<SnapshotElementScope> rows = Browser.FindAllCss("#event-list-past>div", r => r.Count() == 2).ToList();
            AssertEvent(rows[0], e1);
            AssertEvent(rows[1], e2);
        }
    }
}
