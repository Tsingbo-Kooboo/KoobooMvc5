#region License
// 
// Copyright (c) 2013, Kooboo team
// 
// Licensed under the BSD License
// See the file LICENSE.txt for details.
// 
#endregion

using System;
using System.Web;
using Kooboo.CMS.Common.Runtime.Dependency;
using Kooboo.CMS.Sites.Extension;
using Kooboo.CMS.Sites.Models;

namespace Kooboo.CMS.Sites.SEO
{
    [Dependency(typeof(IPageRequestModule), Key = "SEORequestModule", Order = 0)]
    public class SEORequestModule : PageRequestModuleBase
    {
        public override void OnResolvedSite(HttpContextBase httpContext)
        {
            var site = Site.Current;
            var request = httpContext.Request;
            var response = httpContext.Response;
            if (request.HttpMethod.ToUpper() != "GET" || site == null)
            {
                return;
            }

            string rawPath = request.Path;
            string path = rawPath;
            string query = request.Url.Query;

            var redirectUrl = string.Empty;

            if (!path.Equals("/") && site.RemoveTrailingSlash)
            {
                path = path.TrimEnd('/');

                if (!path.Equals(rawPath))
                {
                    redirectUrl = path + query;
                }
            }

            if (!request.Url.Host.StartsWith("www.") && site.DisableNakedDomain)
            {
                var builder = new UriBuilder(request.Url);
                builder.Host = "www." + request.Url.Host;
                builder.Path = path;
                builder.Query = HttpUtility.ParseQueryString(query).ToString();
                redirectUrl = builder.ToString();
            }

            if (site.OutputLowerCasePageUrl)
            {
                redirectUrl = redirectUrl.ToLower();
            }

            if (!string.IsNullOrEmpty(redirectUrl))
            {
                response.StatusCode = 301;
                response.AddHeader("Location", redirectUrl);
                response.End();
            }
        }
    }
}
