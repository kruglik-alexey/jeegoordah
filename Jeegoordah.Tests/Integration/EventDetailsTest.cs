using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
            var e = eventSupport.OpenEventDetails(new TestEvent { Name = "Event Name", StartDate = "01-09-2013", Bros = defaultBros, Description = "Event Description" });
            AssertEvent(e);
        }

        [Test]
        public void ShouldDisplayUrlsAsLinksInDescription()
        {
            eventSupport.OpenEventDetails(new TestEvent { Name = "Event Name", StartDate = "01-09-2013", Bros = defaultBros, Description = "Event Description. http://youporn.com" });                
            Assert.NotNull(Browser.FindLink("http://youporn.com"));
        }

        [Test]
        public void ShouldDeleteEvent()
        {
            eventSupport.OpenEventDetails(new TestEvent { Name = "Event Name", StartDate = "01-09-2013", Bros = defaultBros, Description = "Event Description. http://youporn.com" });
            Browser.ClickButton("Delete");
            Browser.ClickButton("Yes");
            Browser.WaitForAjax();  
            Assert.AreEqual("#events", Browser.Location.Fragment);
            Assert.AreEqual(0, eventSupport.GetPastEvents().Count);            
        }

        [Test]
        public void ShouldNotDeleteEvent()
        {
            eventSupport.OpenEventDetails(new TestEvent { Name = "Event Name", StartDate = "01-09-2013", Bros = defaultBros, Description = "Event Description. http://youporn.com" });
            Browser.ClickButton("Delete");
            Browser.ClickButton("No");
            Browser.WaitForAjax();
            Assert.True(Browser.Location.Fragment.StartsWith("#events/", StringComparison.InvariantCultureIgnoreCase));
            Browser.GoBack();
            Assert.AreEqual(1, eventSupport.GetPastEvents().Count);
        }

        [Test]
        public void ShouldUpdateEvent()
        {
            eventSupport.OpenEventDetails(new TestEvent { Name = "Event Name", StartDate = "01-09-2013", Bros = defaultBros, Description = "Event Description" });
            var ue = new TestEvent { Name = "Updated Name", StartDate = "01-09-2014", Bros = new List<string> { "Копыч", "Сильвер" }, Description = "Updated Description" };
            Browser.ClickButton("Edit");                        
            eventSupport.FillEditor(ue);
            Browser.ClickButton("Save");
            Browser.WaitForAjax();
            AssertEvent(ue);
        }        

        private EventSupport eventSupport;
        private readonly List<string> defaultBros = new List<string> {"Шылдон", "БлекД"};

        private void AssertEvent(TestEvent e)
        {
            Assert.True(Browser.HasContent(e.Name));
            Assert.True(Browser.HasContent(e.StartDate));
            Assert.True(Browser.HasContent(e.Description));
            var bros = Browser.FindAllCss("#module-event-details ul.list-group li").ToList();
            Assert.AreEqual(e.Bros.Count, bros.Count());
            Assert.True(e.Bros.All(b => bros.Any(be => be.HasContent(b))));
        }      

        public override void SetUp()
        {
            base.SetUp();
            eventSupport = new EventSupport(Browser);
        }
    }
}
