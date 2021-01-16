using Microsoft.AspNetCore.Mvc.ModelBinding;
using QueryNinja.Core;

namespace QueryNinja.Sources.AspNetCore.ModelBinding
{
    /// <summary>
    /// Allows to determine when <see cref="QueryNinjaModelBinder"/> should be used.
    /// </summary>
    public class QueryNinjaModelBinderProvider : IModelBinderProvider
    {
        /// <inheritdoc/>
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (!typeof(IQuery).IsAssignableFrom(context.Metadata.ModelType))
            {
                return null;
            }

            if (context.BindingInfo.BindingSource != BindingSource.Query)
            {
                return null;
            }

            return new QueryNinjaModelBinder();
        }
    }
}