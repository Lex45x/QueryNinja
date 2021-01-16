namespace QueryNinja.Core.Filters
{
    /// <summary>
    /// Represent desired filter operation for the <see cref="ComparisonFilter"/>
    /// </summary>
    public enum ComparisonOperation
    {
        /// <summary>
        /// Applicable to primitive types. <br/>
        /// </summary>
        Equals = 1 << 0,

        /// <summary>
        /// Applicable to primitive types. <br/>
        /// </summary>
        NotEquals = 1 << 1,

        /// <summary>
        /// Applicable to primitive types. <br/>
        /// </summary>
        Greater = 1 << 2,

        /// <summary>
        /// Applicable to primitive types. <br/>
        /// </summary>
        GreaterOrEquals = 1 << 3,

        /// <summary>
        /// Applicable to primitive types. <br/>
        /// </summary>
        Less = 1 << 4,

        /// <summary>
        /// Applicable to primitive types. <br/>
        /// </summary>
        LessOrEquals = 1 << 5
    }
}
