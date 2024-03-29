﻿using System.Collections.Generic;
using QueryNinja.Core.Projection;

namespace QueryNinja.Core
{
    /// <summary>
    /// This is core interface for queries that allows to select desired properties from original result aka projection.
    /// </summary>
    public interface IDynamicQuery : IQuery
    {
        /// <summary>
        /// Returns a collection that defines how to construct result projection
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<ISelector> GetSelectors();
    }
}