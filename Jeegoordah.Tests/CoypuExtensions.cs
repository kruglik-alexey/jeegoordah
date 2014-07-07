using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Coypu;
using OpenQA.Selenium;

namespace Jeegoordah.Tests
{
    static class CoypuExtensions
    {
        public static void WaitForAjax(this BrowserSession browser)
        {
            browser.Query(() => browser.ExecuteScript("return jQuery.active == 0"), "true");
        }

        public static void VisitTest(this BrowserSession browser, string url)
        {
            browser.Visit(GetTestUrl(url));
        }

        private static readonly Regex urlParser = new Regex(@"^(.*?){0,1}(\?.*?){0,1}(\#.*?){0,1}$", RegexOptions.Compiled);

        private static string GetTestUrl(string url)
        {
            var match = urlParser.Match(url);
            var path = match.Groups[1].Value;
            var query = match.Groups[2].Value;
            var hash = match.Groups[3].Value;
            query = string.IsNullOrEmpty(query) ? "?test=1" : query + "&test=1";
            return path + query + hash;
        }

        public static void PressEsc(this BrowserSession browser)
        {
            ((OpenQA.Selenium.Chrome.ChromeDriver)browser.Native).Keyboard.PressKey(Keys.Escape);
        }

        public static void PressEnter(this BrowserSession browser)
        {
            ((OpenQA.Selenium.Chrome.ChromeDriver)browser.Native).Keyboard.PressKey(Keys.Enter);
        }
    }
}
