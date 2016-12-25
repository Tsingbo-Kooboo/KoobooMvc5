using Kooboo.CMS.Common.Runtime;
using Kooboo.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;

namespace Kooboo.CMS.Sites.Extension.UI.WebResources
{
    public class CustomWebResources
    {
        public static IEnumerable<WebResource> GetScripts(RequestContext requestContext)
        {
            var providers = EngineContext.Current.ResolveAll<IWebResourceProvider>();

            var matchedProviders = MatchProviders(providers, requestContext.RouteData);

            return matchedProviders
                .SelectMany(it => it.GetScripts(requestContext))
                .OrderBy(it => it.Order);
        }

        public static IEnumerable<WebResource> GetStyles(RequestContext requestContext)
        {
            var providers = EngineContext.Current.ResolveAll<IWebResourceProvider>();

            var matchedProviders = MatchProviders(providers, requestContext.RouteData);

            return matchedProviders
                .SelectMany(it => it.GetStyles(requestContext))
                .OrderBy(it => it.Order);
        }

        private static IEnumerable<IWebResourceProvider> MatchProviders(IEnumerable<IWebResourceProvider> providers, RouteData route)
        {
            var area = AreaHelpers.GetAreaName(route);
            var controller = route.Values["controller"].ToString();
            var action = route.Values["action"].ToString();

            return providers.Where(it => it.ApplyTo == null
                || it.ApplyTo.Any(at => at.Area.EqualsOrNullEmpty(area, StringComparison.OrdinalIgnoreCase)
                && at.Controller.Equals(controller, StringComparison.OrdinalIgnoreCase)
                && at.Action.EqualsOrNullEmpty(action, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
