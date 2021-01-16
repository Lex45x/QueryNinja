using System;

namespace QueryNinja.Core.Extensibility
{
    ///<inheritdoc/>
    public class AbstractComponentExtension<TComponent> : IQueryComponentExtension
        where TComponent : IQueryComponent
    {
        ///<inheritdoc/>
        public Type QueryComponent => typeof(TComponent);
    }
}
