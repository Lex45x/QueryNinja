using Microsoft.EntityFrameworkCore;

namespace QueryNinja.Targets.EntityFrameworkCore.Filters
{
    /// <summary>
    ///   Represent operations defined by <see cref="EF.Functions" /> class.
    /// </summary>
    public enum DatabaseFunction
    {
        /// <summary>
        ///   Corresponds to SQL Like operation.
        /// </summary>
        Like = 1
    }
}