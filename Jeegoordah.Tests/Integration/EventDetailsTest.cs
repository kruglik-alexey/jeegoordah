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
        }

        private EventSupport eventSupport;

        public override void SetUp()
        {
            base.SetUp();
             eventSupport = new EventSupport(Browser);
        }
    }
}
