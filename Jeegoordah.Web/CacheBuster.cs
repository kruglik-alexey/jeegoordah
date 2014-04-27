using System;
using Jeegoordah.Core;

namespace Jeegoordah.Web
{
    public class CacheBuster
    {
        private readonly string _cacheBuster;

        public CacheBuster()
        {
#if DEBUG
            _cacheBuster = (new Random()).Next().ToString();
#else
            _cacheBuster = "2.1.1";
#endif
        }

        public string RenderCssLink(string cssPath)
        {
            return @"<link href=""{0}?{1}"" type=""text/css"" rel=""stylesheet""/>".F(cssPath, _cacheBuster);
        }

        public string RenderRequireJsCacheBuster(string requireJsPath, string mainScriptPath)
        {
            return @"<script type=""text/javascript"">window.jgdhCacheBuster = '{0}'</script>
                     <script data-main=""{1}?{0}"" src=""{2}?{0}"" type=""text/javascript""></script>".F(_cacheBuster, mainScriptPath, requireJsPath);
        }
    }
}