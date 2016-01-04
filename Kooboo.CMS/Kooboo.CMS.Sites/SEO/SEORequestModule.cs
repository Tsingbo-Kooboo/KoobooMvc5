#region License
// 
// Copyright (c) 2013, Kooboo team
// 
// Licensed under the BSD License
// See the file LICENSE.txt for details.
// 
#endregion
using Kooboo.CMS.Common.Runtime.Dependency;
using Kooboo.CMS.Sites.Extension;
using Kooboo.CMS.Sites.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Kooboo.CMS.Sites.Controllers;
using Kooboo.CMS.Sites.Models;

namespace Kooboo.CMS.Sites.ABTest
{
    [Dependency(typeof(IPageRequestModule), Key = "SEORequestModule", Order = 0)]
    public class SEORequestModule : PageRequestModuleBase
    {
        public override void OnResolvedSite(HttpContextBase httpContext)
        {
            var site = Site.Current;
            if (httpContext.Request.HttpMethod.ToLower() != "get")
            {
                return;
            }

            string rawPath = httpContext.Request.Path;
            string path = rawPath;
            string query = httpContext.Request.Url.Query;

            if (!path.Equals("/"))
            {
                if (site.LowerCasePageUrl)
                {
                    path = path.ToLower();
                }

                if (string.IsNullOrEmpty(query))
                {
                    path = path.TrimEnd('/');
                }
            }

            bool forceRedirect = site.RemoveTrailingSlash && !path.Equals(rawPath);
            if (forceRedirect)
            {
                string redirectUrl = path + query;
                httpContext.Response.RedirectPermanent(redirectUrl, false);
            }
            else
            {
                base.OnResolvedSite(httpContext);
            }
        }
    }
}
