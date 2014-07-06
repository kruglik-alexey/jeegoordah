using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Coypu;
using Jeegoordah.Core;
using NUnit.Framework;

namespace Jeegoordah.Tests.Integration
{
    public abstract class IntegrationTest
    {
        protected BrowserSession Browser;
        private const string ScreenshotsDir = "screenshots";        

        [SetUp]
        public virtual void SetUp()
        {
            Directory.CreateDirectory(ScreenshotsDir);
            Directory.EnumerateFiles(ScreenshotsDir, "*.png").ForEach(File.Delete);
            Browser = new BrowserSession(new SessionConfiguration
            {
                Browser = Coypu.Drivers.Browser.Chrome,
                AppHost = "localhost",
                Port = 3107
            });
            Browser.VisitTest("/test/cleardatabase");
        }

        [TearDown]
        public virtual void TearDown()
        {
            try
            {
                if (TestContext.CurrentContext.Result.Status == TestStatus.Failed)
                {
                    Browser.SaveScreenshot(Path.Combine(ScreenshotsDir, TestContext.CurrentContext.Test.Name + ".png"), ImageFormat.Png);
                }
            }
            finally
            {
                Browser.Dispose();
            }                        
        }
    }
}
