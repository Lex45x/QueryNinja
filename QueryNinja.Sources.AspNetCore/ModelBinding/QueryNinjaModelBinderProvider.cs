using Microsoft.AspNetCore.Mvc.ModelBinding;
using QueryNinja.Core;

namespace QueryNinja.Sources.AspNetCore.ModelBinding
{
    /// <summary>
    ///   Allows to determine when <see cref="QueryNinjaModelBinder" /> should be used.
    /// </summary>
    public class QueryNinjaModelBinderProvider : IModelBinderProvider
    {
        /// <inheritdoc />
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (!typeof(IQuery).IsAssignableFrom(context.Metadata.ModelType))
            {
                return null;
            }

            return context.BindingInfo.BindingSource != BindingSource.Query ? null : new QueryNinjaModelBinder();
        }
    }
}