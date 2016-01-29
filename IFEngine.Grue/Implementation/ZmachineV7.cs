// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZmachineV7.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   The zmachine version 7.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// The zmachine version 7.
    /// </summary>
    /// <remarks>
    /// Version 7 changes the modifiers for calculating routine and string addresses and the story length.
    /// </remarks>
    
    internal class ZmachineV7 : ZmachineV5
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZmachineV7"/> class.
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
        internal ZmachineV7(FrontEnd frontEnd, ImmutableArray<byte> story, IRandomNumberGenerator random) : base(frontEnd, story, random)
        {
        }

        /// <summary>
        /// Gets the story length multiplier.
        /// </summary>
        /// <value>
        /// The story length multiplier.
        /// </value>
        protected override int StoryLengthMultiplier
        {
            get
            {
                return 8;
            }
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
        /// <remarks>
        /// Behaves the same as version 6.
        /// </remarks>
        protected override int UnpackRoutineAddress(ushort packedAddress)
        {
            var routinesOffset = this.Memory.ReadWord(40);
            return (packedAddress * 4) + (routinesOffset * 8);
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
        /// <remarks>
        /// Behaves the same as version 6.
        /// </remarks>
        protected override int UnpackStringAddress(ushort packedAddress)
        {
            var stringsOffset = this.Memory.ReadWord(42);
            return (packedAddress * 4) + (stringsOffset * 8);
        }
    }
}