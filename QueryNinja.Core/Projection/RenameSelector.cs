namespace QueryNinja.Core.Projection
{
    /// <summary>
    /// Represent selector that allows to place selected properties to different path.
    /// </summary>
    public sealed class RenameSelector : ISelector
    {
        /// <summary>
        /// Creates instance of <see cref="RenameSelector"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public RenameSelector(string source, string target)
        {
            Source = source;
            Target = target;
        }

        /// <inheritdoc />
        public string Source { get; }

        /// <inheritdoc />
        public string Target { get; }
    }
}