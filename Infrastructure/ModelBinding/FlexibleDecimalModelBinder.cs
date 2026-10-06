using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace Carbase.Infrastructure.ModelBinding
{
    public class FlexibleDecimalModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueResult == ValueProviderResult.None) return Task.CompletedTask;

            bindingContext.ModelState.SetModelValue(
                bindingContext.ModelName, 
                valueResult);

            var rawValue = valueResult.FirstValue;

            if (string.IsNullOrEmpty(rawValue))
            {
                if (Nullable.GetUnderlyingType(bindingContext.ModelType) != null)
                {
                    bindingContext.Result = ModelBindingResult.Success(null);
                }

                return Task.CompletedTask;
            }

            var normalizedValue = rawValue.Replace(',', '.');

            if (decimal.TryParse(
                normalizedValue,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var result))
            {
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            else
            {
                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName,
                    $"The value '{rawValue}' is not a valid number");
            }

            return Task.CompletedTask;
        }
    }
}
