using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coypu;
using Jeegoordah.Tests.Integration.Common;
using NUnit.Framework;

namespace Jeegoordah.Tests.Integration
{
    [TestFixture]
    internal class EventEditorTest : IntegrationTest
    {
        private EventSupport eventSupport;

        [Test]
        public void ShouldNotCreateEventWithLessThan2Bros()
        {
            eventSupport.VisitEventsList();
            eventSupport.CreateEvent(new TestEvent
            {
                Name = "Event Name",
                StartDate = "01-09-2013",
                Description = "Description",
                Bros = {}
            });
            AssertEntityEditor();
            Browser.ClickButton("Cancel");
            Assert.AreEqual(0, eventSupport.GetPastEvents().Count);

            eventSupport.CreateEvent(new TestEvent
            {
                Name = "Event Name2",
                StartDate = "01-09-2013",
                Description = "Description",
                Bros = {"Шылдон"}
            });
            AssertEntityEditor();
            Browser.ClickButton("Cancel");
            Assert.AreEqual(0, eventSupport.GetPastEvents().Count);
        }

        [Test]
        public void ShouldNotCreateEventWithDuplicatedName()
        {
            eventSupport.VisitEventsList();
            eventSupport.CreateEvent(new TestEvent {Name = "Event Name", StartDate = "01-09-2013", Bros = defaultBros});
            eventSupport.CreateEvent(new TestEvent {Name = "Event Name", StartDate = "01-09-2113", Bros = defaultBros});

            AssertEntityEditor();
            Browser.ClickButton("Cancel");
            Assert.AreEqual(1, eventSupport.GetPastEvents().Count);
        }

//        [Test]
//        public void ShouldNotUpdateEventWithDuplicatedName()
//        {
//            Visit("#events");
//            var e1 = eventSupport.CreateEvent(new TestEvent { Name = "Event Name", StartDate = "01-09-2013", Bros = defaultBros });
//            var e2 = eventSupport.CreateEvent(new TestEvent { Name = "Event Name2", StartDate = "01-09-2013", Bros = defaultBros });
//
//            var row = eventSupport.GetPastEvents()[0];
//            row.Hover();
//            row.ClickButton("Edit");
//
//            Browser.FillIn("Name").With("Event Name2");
//            Browser.ClickButton("Save");
//
//            AssertEntityEditor();
//            Browser.ClickButton("Cancel");
//
//            IList<SnapshotElementScope> rows = GetPastEvents();
//            AssertEvent(rows[0], e1);
//            AssertEvent(rows[1], e2);
//        }

        private readonly List<string> defaultBros = new List<string> {"Шылдон", "БлекД"};

        public override void SetUp()
        {
            base.SetUp();
            eventSupport = new EventSupport(Browser);
        }

        private void AssertEntityEditor()
        {
            Assert.NotNull(Browser.FindCss("#entityEditor"));
        }
    }
}
