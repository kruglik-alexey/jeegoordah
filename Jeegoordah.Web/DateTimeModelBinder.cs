using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;

namespace Jeegoordah.Web
{
    public class DateTimeModelBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            string date = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).AttemptedValue;
            if (String.IsNullOrEmpty(date))
                return false;

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, bindingContext.ValueProvider.GetValue(bindingContext.ModelName));
            try
            {
                bindingContext.Model = JsonDate.Parse(date);
                return true;
            }
            catch (Exception)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, String.Format("\"{0}\" is invalid.", bindingContext.ModelName));
                return false;
            }
        }
    }
}