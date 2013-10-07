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
        private void CreateEvent(string name, string startDate, string description, IEnumerable<string> bros = null)
        {
            Browser.ClickButton("createEventButton");
            Browser.FillIn("Name").With(name);
            Browser.FillIn("StartDate").With(startDate);
            Browser.FillIn("Description").With(description);
            bros = bros ?? new string[0];
            foreach (string bro in bros)
            {
                Browser.Check(bro);
            }
            Browser.ClickButton("modalOkButton");            
        }

        [Test]
        public void ShouldCreateEvent()
        {            
            Visit("#events");
            CreateEvent("Event Name", "01-09-2013", "Event Description\r\nhttp://foo.com", new[] {"Шылдон"});

            List<SnapshotElementScope> rows = Browser.FindAllCss("tbody#event-list>tr", r => r.Count() == 1).ToList();
            Assert.AreEqual(1, rows.Count);
            SnapshotElementScope row = rows[0];
            Assert.AreEqual("Event Name", row.FindCss("td:nth-child(1)").Text);
            Assert.AreEqual("01-09-2013", row.FindCss("td:nth-child(2)").Text);
            Assert.AreEqual("Шылдон", row.FindCss("td:nth-child(3)").Text);
            ElementScope description = row.FindCss("td:nth-child(4)");
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
            CreateEvent("Event Name", "01-09-2013", "Event Description", new[] {"Шылдон", "Моер"});
            ElementScope row = Browser.FindCss("tbody#event-list>tr");
            row.Hover();
            row.ClickButton("Edit");
            Assert.AreEqual("Event Name", Browser.FindField("Name").Value);
            Assert.AreEqual("01-09-2013", Browser.FindField("StartDate").Value);
            Assert.AreEqual("Event Description", Browser.FindField("Description").Value);
            Assert.True(Browser.FindField("Шылдон").Selected);
            Assert.True(Browser.FindField("Моер").Selected);

            Browser.FillIn("Name").With("Updated Event");
            Browser.FillIn("StartDate").With("02-09-2013");
            Browser.FillIn("Description").With("Updated Description");
            Browser.FindField("Шылдон").Uncheck();
            Browser.Check("Копыч");
            Browser.ClickButton("modalOkButton");
            Visit("#events");
            row = Browser.FindCss("tbody#event-list>tr");
            Assert.AreEqual("Updated Event", row.FindCss("td:nth-child(1)").Text);
            Assert.AreEqual("02-09-2013", row.FindCss("td:nth-child(2)").Text);
            Assert.AreEqual("Моер\r\nКопыч", row.FindCss("td:nth-child(3)").Text);
            Assert.AreEqual("Updated Description", row.FindCss("td:nth-child(4)>div>span").Text);
        }

        [Test]
        public void ShouldLoadEvents()
        {
            Visit("#events");
            CreateEvent("Event Name", "01-09-2013", "Event Description\r\nhttp://foo.com", new[] {"Шылдон"});
            CreateEvent("Event Name 2", "02-09-2013", "");             

            Visit("#events");
            List<SnapshotElementScope> rows = Browser.FindAllCss("tbody#event-list>tr", r => r.Count() == 2).ToList();
            Assert.AreEqual(2, rows.Count);
            SnapshotElementScope row = rows[0];
            Assert.AreEqual("Event Name", row.FindCss("td:nth-child(1)").Text);
            Assert.AreEqual("01-09-2013", row.FindCss("td:nth-child(2)").Text);
            Assert.AreEqual("Шылдон", row.FindCss("td:nth-child(3)").Text);
            var description = row.FindCss("td:nth-child(4)");
            Assert.AreEqual("Event Description\r\nhttp://foo.com", description.FindCss("div>span").Text);

            row = rows[1];
            Assert.AreEqual("Event Name 2", row.FindCss("td:nth-child(1)").Text);
            Assert.AreEqual("02-09-2013", row.FindCss("td:nth-child(2)").Text);
            Assert.IsEmpty(row.FindCss("td:nth-child(3)").Text);
            description = row.FindCss("td:nth-child(4)>div>span", new Options {ConsiderInvisibleElements = true});
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

        [Test]
        public void ShouldNotUpdateEventWithDuplicatedName()
        {
            Visit("#events");
            CreateEvent("Event Name", "01-09-2013", "Event Description\r\nhttp://foo.com");
            CreateEvent("Event Name2", "02-09-2013", "");

            ElementScope row = Browser.FindCss("tbody#event-list>tr");
            row.Hover();
            row.ClickButton("Edit");
            Browser.FillIn("Name").With("Event Name2");
            Browser.ClickButton("Save");
            Assert.NotNull(Browser.FindCss("#entityEditor"));
            Browser.ClickButton("Cancel");
            List<SnapshotElementScope> rows = Browser.FindAllCss("tbody#event-list>tr", r => r.Count() == 2).ToList();
            Assert.AreEqual("Event Name", rows[0].FindCss("td:nth-child(1)").Text);
            Assert.AreEqual("Event Name2", rows[1].FindCss("td:nth-child(1)").Text);
        }
    }
}
