using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Coypu;
using Coypu.Drivers;
using NUnit.Framework;

namespace Jeegoordah.Tests.Integration
{
    public abstract class IntegrationTest
    {
        protected BrowserSession Browser;

        private readonly Regex _urlParser =
            new Regex(@"^(.*?){0,1}(\?.*?){0,1}(\#.*?){0,1}$",
                RegexOptions.Compiled);

        private string GetTestUrl(string url)
        {
            var match = _urlParser.Match(url);
            var path = match.Groups[1].Value;
            var query = match.Groups[2].Value;
            var hash = match.Groups[3].Value;
            query = string.IsNullOrEmpty(query) ? "?test=1" : query + "&test=1";
            return path + query + hash;
        }

        protected void Visit(string url)
        {
            Browser.Visit(GetTestUrl(url));
        }       

        [SetUp]
        public void SetUp()
        {
            Browser = IntegrationTestsSetup.Browser;
            Visit("/test/cleardatabase");
        }       
    }
}
