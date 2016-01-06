#region License
// 
// Copyright (c) 2013, Kooboo team
// 
// Licensed under the BSD License
// See the file LICENSE.txt for details.
// 
#endregion
using Kooboo.CMS.Sites.Models;
using Kooboo.CMS.Sites.Services;
using Kooboo.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Kooboo.CMS.Web.Areas.Sites.Models.DataSources
{
    public class SiteDomainsDataSource : ISelectListDataSource
    {
        private static Regex hostNameRegex = new Regex("(?:https?:\\/{2})?([a-z0-9\\-\\.]+)", RegexOptions.IgnoreCase|RegexOptions.Singleline);
        #region IDropDownListDataSource Members

        public IEnumerable<SelectListItem> GetSelectListItems(RequestContext requestContext, string filter)
        {
            var site = Site.Current;
            var domains = new List<string>();
            if (site != null)
            {
                foreach (var domain in site.Domains)
                {
                    Match match = hostNameRegex.Match(domain);
                    if (match.Success)
                    {
                        var host = match.Groups[1].ToString().ToLower();
                        domains.Add(host);
                    }
                }
            }

            return domains.Distinct().Select(it => new SelectListItem() {Text = it, Value = it});
        }

        #endregion
    }
}