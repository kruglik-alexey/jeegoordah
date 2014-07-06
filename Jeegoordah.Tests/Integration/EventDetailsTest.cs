using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jeegoordah.Tests.Integration.Common;
using NUnit.Framework;

namespace Jeegoordah.Tests.Integration
{
    [TestFixture]
    public class EventDetailsTest : IntegrationTest
    {
        [Test]
        public void ShouldDisplayRightInfo()
        {
            eventSupport.VisitEventsList();
            var e = eventSupport.CreateEvent(new TestEvent {Name = "Event Name", StartDate = "01-09-2013", Bros = defaultBros});
            eventSupport.GetPastEvents()[0].ClickLink(e.Name);
            Browser.WaitForAjax();

            Assert.True(Browser.HasContent(e.Name));
            Assert.True(Browser.HasContent(e.StartDate));
            var bros = Browser.FindAllCss("#module-event-details ul.list-group li").ToList();
            Assert.AreEqual(e.Bros.Count, bros.Count());
            Assert.True(e.Bros.All(b => bros.Any(be => be.HasContent(b))));
        }

        private EventSupport eventSupport;
        private readonly List<string> defaultBros = new List<string> {"Шылдон", "БлекД"};

        public override void SetUp()
        {
            base.SetUp();
             eventSupport = new EventSupport(Browser);
        }
    }
}
