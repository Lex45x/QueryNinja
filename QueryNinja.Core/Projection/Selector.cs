namespace QueryNinja.Core.Projection
{
    /// <summary>
    /// Represent selector that leaves path to properties as-is.
    /// </summary>
    public class Selector : ISelector
    {
        /// <summary>
        /// Creates instance of Selector.
        /// </summary>
        /// <param name="source"></param>
        public Selector(string source)
        {
            Source = source;
        }

        /// <inheritdoc />
        public string Source { get; }

        /// <inheritdoc />
        public string Target => Source;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Source} -> {Target}";
        }
    }
}