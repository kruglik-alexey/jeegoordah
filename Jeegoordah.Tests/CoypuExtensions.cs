using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coypu;

namespace Jeegoordah.Tests
{
    static class CoypuExtensions
    {
        public static void WaitForAjax(this BrowserSession browser)
        {
            browser.Query(() => browser.ExecuteScript("return jQuery.active == 0"), "true");
        }
    }
}
