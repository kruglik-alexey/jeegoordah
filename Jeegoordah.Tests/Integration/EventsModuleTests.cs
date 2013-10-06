using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Coypu;
using NUnit.Framework;

namespace Jeegoordah.Tests.Integration
{
    [TestFixture]
    public class EventsModuleTests : IntegrationTest
    {
        private void CreateEvent(string name, string startDate, string description)
        {
            Browser.ClickButton("createEventButton");
            Browser.FillIn("Name").With(name);
            Browser.FillIn("StartDate").With(startDate);
            Browser.FillIn("Description").With(description);
            Browser.ClickButton("modalOkButton");            
        }

        [Test]
        public void ShouldCreateEvent()
        {            
            Visit("#events");
            CreateEvent("Event Name", "01-09-2013", "Event Description\r\nhttp://foo.com");            

            List<SnapshotElementScope> rows = Browser.FindAllCss("tbody#event-list>tr", r => r.Count() == 1).ToList();
            Assert.AreEqual(1, rows.Count);
            SnapshotElementScope row = rows[0];
            Assert.AreEqual("Event Name", row.FindCss("td:nth-child(1)").Text);
            Assert.AreEqual("01-09-2013", row.FindCss("td:nth-child(2)").Text);
            var description = row.FindCss("td:nth-child(3)");
            Assert.AreEqual("Event Description\r\nhttp://foo.com", description.FindCss("div>span").Text);
        }

        [Test]
        public void ShouldDeleteEvent()
        {
            Visit("#events");
            CreateEvent("Event Name", "01-09-2013", "Event Description");
            CreateEvent("Event Name2", "01-09-2013", "Event Description");
            ElementScope row = Browser.FindCss("tbody#event-list>tr");
            row.Hover();
            row.ClickButton("Delete");
            Browser.ClickButton("Yes");
            Visit("#events");
            List<SnapshotElementScope> rows = Browser.FindAllCss("tbody#event-list>tr", r => r.Count() == 1).ToList();
            Assert.AreEqual(1, rows.Count);
            row = rows[0];
            Assert.AreEqual("Event Name2", row.FindCss("td:nth-child(1)").Text);
        }

        [Test]
        public void ShouldUpdateEvent()
        {
            Visit("#events");
            CreateEvent("Event Name", "01-09-2013", "Event Description");
            ElementScope row = Browser.FindCss("tbody#event-list>tr");
            row.Hover();
            row.ClickButton("Edit");
            Assert.AreEqual("Event Name", Browser.FindField("Name").Value);
            Assert.AreEqual("01-09-2013", Browser.FindField("StartDate").Value);
            Assert.AreEqual("Event Description", Browser.FindField("Description").Value);

            Browser.FillIn("Name").With("Updated Event");
            Browser.FillIn("StartDate").With("02-09-2013");
            Browser.FillIn("Description").With("");
            Browser.ClickButton("modalOkButton");
            Visit("#events");
            row = Browser.FindCss("tbody#event-list>tr");
            Assert.AreEqual("Updated Event", row.FindCss("td:nth-child(1)").Text);
            Assert.AreEqual("02-09-2013", row.FindCss("td:nth-child(2)").Text);
            Assert.IsEmpty(row.FindCss("td:nth-child(3)>div>span", new Options { ConsiderInvisibleElements = true }).Text);
        }

        [Test]
        public void ShouldLoadEvents()
        {
            Visit("#events");
            CreateEvent("Event Name", "01-09-2013", "Event Description\r\nhttp://foo.com"); 
            CreateEvent("Event Name 2", "02-09-2013", "");             

            Visit("#events");
            List<SnapshotElementScope> rows = Browser.FindAllCss("tbody#event-list>tr", r => r.Count() == 2).ToList();
            Assert.AreEqual(2, rows.Count);
            SnapshotElementScope row = rows[0];
            Assert.AreEqual("Event Name", row.FindCss("td:nth-child(1)").Text);
            Assert.AreEqual("01-09-2013", row.FindCss("td:nth-child(2)").Text);
            var description = row.FindCss("td:nth-child(3)");
            Assert.AreEqual("Event Description\r\nhttp://foo.com", description.FindCss("div>span").Text);

            row = rows[1];
            Assert.AreEqual("Event Name 2", row.FindCss("td:nth-child(1)").Text);
            Assert.AreEqual("02-09-2013", row.FindCss("td:nth-child(2)").Text);
            description = row.FindCss("td:nth-child(3)>div>span", new Options {ConsiderInvisibleElements = true});
            Assert.IsEmpty(description.Text);
        }

        [Test]
        public void ShouldNotCreateEventWithDuplicatedName()
        {
            Visit("#events");
            CreateEvent("Event Name", "01-09-2013", "Event Description\r\nhttp://foo.com");
            CreateEvent("Event Name", "02-09-2013", "");
            Assert.NotNull(Browser.FindCss("#entityEditor"));
            Browser.ClickButton("Cancel");
            List<SnapshotElementScope> rows = Browser.FindAllCss("tbody#event-list>tr", r => r.Count() == 1).ToList();
            Assert.AreEqual(1, rows.Count);
        }
    }
}
