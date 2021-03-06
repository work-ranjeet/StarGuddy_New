﻿
namespace System
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// GUID Extensions
    /// </summary>
    public static class GuidExtensions
    {
        /// <summary>
        /// Determines whether [is null or empty].
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>
        ///   <c>true</c> if [is null or empty] [the specified unique identifier]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(this Guid guid)
        {
            return guid == null || guid == Guid.Empty;
        }
    }
}
