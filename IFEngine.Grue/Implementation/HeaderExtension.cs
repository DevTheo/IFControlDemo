// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeaderExtension.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   The header extension table entries.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// The header extension table entries.
    /// </summary>
    internal enum HeaderExtension
    {
        /// <summary>
        /// The entry count.
        /// </summary>
        EntryCount = 0, 

        /// <summary>
        /// The default true background colour.
        /// </summary>
        DefaultTrueBackgroundColour = 6, 

        /// <summary>
        /// The default true foreground colour.
        /// </summary>
        DefaultTrueForegroundColour = 5, 

        /// <summary>
        /// The third set of flags.
        /// </summary>
        Flags3 = 4, 

        /// <summary>
        /// The mouse column.
        /// </summary>
        MouseColumn = 1, 

        /// <summary>
        /// The mouse row.
        /// </summary>
        MouseRow = 2, 

        /// <summary>
        /// The unicode translation table.
        /// </summary>
        UnicodeTranslationTable = 3
    }
}