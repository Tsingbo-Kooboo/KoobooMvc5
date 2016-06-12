using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Kooboo.CMS.Sites.Controllers.ActionFilters
{
    public class ActionLogAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            var request = filterContext.HttpContext.Request;
            var method = request.HttpMethod.ToLower();
            if (method != "get")
            {
                var msg = String.Concat(method, " ", request.RawUrl);
                Kooboo.HealthMonitoring.Log.LogInformation(msg, null);
            }
        }
    }
}

