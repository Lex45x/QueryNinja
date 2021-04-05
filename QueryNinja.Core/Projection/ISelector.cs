namespace QueryNinja.Core.Projection
{
    /// <summary>
    /// Allows to specify how the original properties should be selected into result projection.
    /// </summary>
    public interface ISelector
    {
        /// <summary>
        /// Path to the source property.
        /// </summary>
        string Source { get; }

        /// <summary>
        /// Path to the target property
        /// </summary>
        string Target { get; }
    }
}