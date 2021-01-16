using QueryNinja.Core.Extensibility;
using QueryNinja.Targets.Queryable.QueryBuilders;

namespace QueryNinja.Targets.Queryable
{
    /// <summary>
    /// Allows to register IQueryable Target in <see cref="QueryNinjaExtensions"/>
    /// </summary>
    public static class RegistrationExtensions
    {
        public static IExtensionsSettings WithQueryableTarget(this IExtensionsSettings settings)
        {
            settings.Register<CollectionFilterQueryBuilder>();
            settings.Register<ComparisonFilterQueryBuilder>();
            settings.Register<OrderQueryBuilder>();

            return settings;
        }
    }
}