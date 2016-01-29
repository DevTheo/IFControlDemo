// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowAttributes.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   Window attributes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// Window attributes.
    /// </summary>
    internal static class WindowAttributes
    {
        /// <summary>
        /// Buffering is disabled.
        /// </summary>
        internal const ushort BufferingDisabled = BufferingEnabled ^ ushort.MaxValue;

        /// <summary>
        /// Buffering is enabled.
        /// </summary>
        internal const ushort BufferingEnabled = 8;

        /// <summary>
        /// Scripting is enabled.
        /// </summary>
        internal const ushort ScriptingEnabled = 4;

        /// <summary>
        /// Scrolling is enabled.
        /// </summary>
        internal const ushort ScrollingEnabled = 2;

        /// <summary>
        /// Wrapping is enabled.
        /// </summary>
        internal const ushort WrappingEnabled = 1;
    }
}