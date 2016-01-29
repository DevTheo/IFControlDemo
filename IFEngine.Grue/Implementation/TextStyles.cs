// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextStyles.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   Text styles.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    using System;

    /// <summary>
    /// Text styles.
    /// </summary>
    [Flags]
    internal enum TextStyles
    {
        /// <summary>
        /// Normal style.
        /// </summary>
        None = 0, 

        /// <summary>
        /// Reverse style.
        /// </summary>
        Reverse = 1, 

        /// <summary>
        /// Bold style.
        /// </summary>
        Bold = 2, 

        /// <summary>
        /// Italic style.
        /// </summary>
        Italic = 4, 

        /// <summary>
        /// Fixed style.
        /// </summary>
        Fixed = 8, 
    }
}