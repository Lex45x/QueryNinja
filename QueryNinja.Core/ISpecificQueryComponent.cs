namespace QueryNinja.Core
{
    /// <summary>
    /// Represent a query component that targets at specific model <see cref="Property"/>.
    /// </summary>
    public interface ISpecificQueryComponent : IQueryComponent
    {
        /// <summary>
        /// Property of the model
        /// </summary>
        string Property { get; }
    }
}