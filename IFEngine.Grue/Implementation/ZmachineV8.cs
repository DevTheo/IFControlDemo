// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZmachineV8.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   The zmachine version 8.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// The zmachine version 8.
    /// </summary>
    /// <remarks>
    /// Version 8 changes the modifiers for calculating routine and string addresses.
    /// </remarks>
    
    internal class ZmachineV8 : ZmachineV7
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZmachineV8"/> class.
        /// </summary>
        /// <param name="frontEnd">
        /// The front end.
        /// </param>
        /// <param name="story">
        /// The story.
        /// </param>
        /// <param name="random">
        /// The random number generator.
        /// </param>
        internal ZmachineV8(FrontEnd frontEnd, ImmutableArray<byte> story, IRandomNumberGenerator random) : base(frontEnd, story, random)
        {
        }

        /// <summary>
        /// Unpacks a routine address.
        /// </summary>
        /// <param name="packedAddress">
        /// Packed address.
        /// </param>
        /// <returns>
        /// Unpacked address.
        /// </returns>
        protected override int UnpackRoutineAddress(ushort packedAddress)
        {
            return packedAddress * 8;
        }

        /// <summary>
        /// Unpacks a string address.
        /// </summary>
        /// <param name="packedAddress">
        /// Packed address.
        /// </param>
        /// <returns>
        /// Unpacked address.
        /// </returns>
        protected override int UnpackStringAddress(ushort packedAddress)
        {
            return packedAddress * 8;
        }
    }
}