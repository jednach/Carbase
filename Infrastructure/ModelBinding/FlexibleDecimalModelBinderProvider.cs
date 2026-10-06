using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Carbase.Infrastructure.ModelBinding
{
    public class FlexibleDecimalModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            var modelType = context.Metadata.ModelType;

            if (modelType == typeof(decimal) ||
                modelType == typeof(decimal?))
            {
                return new FlexibleDecimalModelBinder();
            }

            return null;
        }
    }
}
