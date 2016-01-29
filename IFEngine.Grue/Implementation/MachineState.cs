// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MachineState.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   The zmachine state.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// The zmachine state.
    /// </summary>
    internal enum MachineState
    {
        /// <summary>
        /// The zmachine is initializing.
        /// </summary>
        Initializing, 

        /// <summary>
        /// The zmachine is running.
        /// </summary>
        Running, 

        /// <summary>
        /// The zmachine is reading input.
        /// </summary>
        ReadingInput, 

        /// <summary>
        /// The zmachine has halted.
        /// </summary>
        Halted
    }
}