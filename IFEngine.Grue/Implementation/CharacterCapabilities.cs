// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CharacterCapabilities.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   Character capabilites.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    using System;

    /// <summary>
    /// Character capabilites.
    /// </summary>
    [Flags]
    internal enum CharacterCapabilities
    {
        /// <summary>
        /// No capability.
        /// </summary>
        None = 0, 

        /// <summary>
        /// Output capability.
        /// </summary>
        Output = 1, 

        /// <summary>
        /// Input capability.
        /// </summary>
        Input = 2, 

        /// <summary>
        /// Both output and input capabilities.
        /// </summary>
        Both = 3
    }
}