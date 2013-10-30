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
                // TODO aggregate message from validation errors
                filterContext.Result = new JsonResult {Data = new {Message = "Invalid data"}};
            }

            base.OnActionExecuting(filterContext);
        }
    }
}