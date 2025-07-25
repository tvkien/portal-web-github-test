using Newtonsoft.Json;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.Models.CustomModelBinder
{
    public class JsonModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult != null)
            {
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

                var valueAsString = valueProviderResult.AttemptedValue;
                return JsonConvert.DeserializeObject(valueAsString, bindingContext.ModelType);
            }
            else
            {
                return base.BindModel(controllerContext, bindingContext);
            }
        }
    }
}
