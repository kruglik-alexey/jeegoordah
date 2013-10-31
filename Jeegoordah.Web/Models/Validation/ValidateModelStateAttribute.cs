using System.Linq;
using System.Web.Mvc;

namespace Jeegoordah.Web.Models.Validation
{
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var viewData = filterContext.Controller.ViewData;            
            if (!viewData.ModelState.IsValid)
            {
                filterContext.RequestContext.HttpContext.Response.StatusCode = 400;
                var m = viewData.ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .Aggregate("", (acc, e) => acc + e + "\r\n");
                filterContext.Result = new JsonResult {Data = new {Message = m}};
            }

            base.OnActionExecuting(filterContext);
        }
    }
}