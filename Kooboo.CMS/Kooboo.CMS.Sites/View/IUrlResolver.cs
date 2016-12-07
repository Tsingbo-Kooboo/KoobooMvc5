using Kooboo.CMS.Common.Runtime.Dependency;
using Kooboo.CMS.Sites.Models;
using Kooboo.CMS.Sites.Web;
using Kooboo.Web.Url;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Kooboo.CMS.Sites.View
{
    public interface IUrlResolver
    {
        IHtmlString WrapperUrl(string url);
        IHtmlString WrapperUrl(string url, bool? requireSSL);
        string ModuleScriptsUrl(string moduleName, string baseUri, bool compressed);
    }

    [Dependency(typeof(IUrlResolver))]
    public class UrlResolver : IUrlResolver
    {
        #region --- ctor ---
        public UrlHelper Url { get; private set; }
        public Site Site { get; private set; }
        public FrontRequestChannel RequestChannel { get; private set; }

        public UrlResolver(UrlHelper url, Site site, FrontRequestChannel requestChannel)
        {
            this.Url = url;
            this.Site = site;
            this.RequestChannel = requestChannel;
        }

        #endregion
        public virtual string ModuleScriptsUrl(string moduleName, string baseUri, bool compressed)
        {
            return UrlUtility.ToHttpAbsolute(baseUri, Url.Action("ModuleScripts", "Resource", new { siteName = Site.FullName, moduleName = moduleName, version = Site.VersionUsedInUrl, area = "", compressed = compressed }));
        }
    }
}
