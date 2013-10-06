using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coypu;
using Coypu.Drivers;
using NUnit.Framework;

namespace Jeegoordah.Tests.Integration
{
    [SetUpFixture]
    public class IntegrationTestsSetup
    {
        public static BrowserSession Browser { get; private set; } 

        [SetUp]
        public void SetUp()
        {
            Browser = new BrowserSession(new SessionConfiguration
            {
                Browser = Coypu.Drivers.Browser.Chrome,
                AppHost = "localhost",
                Port = 3107
            });
        }

        [TearDown]
        public void TearDown()
        {
            Browser.Dispose();
        }
    }
}
