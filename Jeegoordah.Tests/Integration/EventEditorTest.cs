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
        public void ShouldDisplayRightInfo()
        {
            var e = eventSupport.OpenEventDetails(new TestEvent
            {
                Name = "Event Name",
                StartDate = "01-09-2013",
                Description = "Description",
                Bros = {"Копыч", "Даша"}
            });
            Browser.ClickButton("Edit");
            Assert.AreEqual(e.Name, Browser.FindField("Name").Value);
            Assert.AreEqual(e.StartDate, Browser.FindField("StartDate").Value);
            Assert.AreEqual(e.Description, Browser.FindField("Description").Value);
            Assert.True(eventSupport.GetActiveBroCheckboxes().All(c => e.Bros.Any(b => b == c.Text)));            
        }

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

        [Test]
        public void ShouldNotUpdateEventWithDuplicatedName()
        {
            Browser.VisitTest("#events");
            var e1 = eventSupport.CreateEvent(new TestEvent { Name = "Event Foo", StartDate = "01-09-2013", Bros = defaultBros });
            var e2 = eventSupport.CreateEvent(new TestEvent { Name = "Event Bar", StartDate = "01-09-2013", Bros = defaultBros });
            eventSupport.GetPastEvents().First(e => e.HasContent(e1.Name)).ClickLink(e1.Name);
            Browser.WaitForAjax();
            Browser.ClickButton("Edit");
            Browser.FindField("Name").FillInWith(e2.Name);
            Browser.ClickButton("Save");
            Browser.WaitForAjax();
            AssertEntityEditor();
            Browser.GoBack();
            Browser.WaitForAjax();
            Assert.True(eventSupport.GetPastEvents().Any(e => e.HasContent(e1.Name)));
            Assert.True(eventSupport.GetPastEvents().Any(e => e.HasContent(e2.Name)));
        }

        [Test]
        public void ShouldCloseByEsc()
        {
            Assert.Fail();
        }

        [Test]
        public void ShouldCloseByEnter()
        {
            Assert.Fail();
        }

        [Test]
        public void ShouldRequireName()
        {
            Assert.Fail();
        }

        [Test]
        public void ShouldRequireStartDate()
        {
            Assert.Fail();
        }

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
