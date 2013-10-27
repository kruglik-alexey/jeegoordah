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
                filterContext.Result = new JsonResult
                {
                    Data = new {Error = "Invalid data"}                                       
                };
            }

            base.OnActionExecuting(filterContext);
        }
    }
}