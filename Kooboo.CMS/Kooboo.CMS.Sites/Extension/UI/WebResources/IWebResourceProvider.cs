using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace Kooboo.CMS.Sites.Extension.UI.WebResources
{
    public interface IWebResourceProvider
    {
        MvcRoute[] ApplyTo { get; }

        IEnumerable<WebResource> GetStyles(RequestContext requestContext);

        IEnumerable<WebResource> GetScripts(RequestContext requestContext);
    }
}
