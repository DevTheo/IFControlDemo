// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Font.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   Display fonts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// Display fonts.
    /// </summary>
    internal enum Font
    {
        /// <summary>
        /// An invalid font.
        /// </summary>
        Invalid = 0, 

        /// <summary>
        /// The normal font.
        /// </summary>
        Normal = 1, 

        /// <summary>
        /// The graphics font used by Beyond Zork and Journey.
        /// </summary>
        Graphics = 3, 

        /// <summary>
        /// The fixed pitch font.
        /// </summary>
        FixedPitch = 4
    }
}