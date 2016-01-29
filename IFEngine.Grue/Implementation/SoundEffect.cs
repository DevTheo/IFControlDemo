// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SoundEffect.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   A zmachine sound effect.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// A zmachine sound effect.
    /// </summary>
    internal sealed class SoundEffect
    {
        /// <summary>
        /// The routine.
        /// </summary>
        private readonly ushort routine;

        /// <summary>
        /// The sound.
        /// </summary>
        private readonly ushort sound;

        /// <summary>
        /// The volume.
        /// </summary>
        private readonly byte volume;

        /// <summary>
        /// The repetitions.
        /// </summary>
        private byte repetitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundEffect"/> class.
        /// </summary>
        /// <param name="sound">
        /// The sound.
        /// </param>
        /// <param name="volume">
        /// The volume.
        /// </param>
        /// <param name="repetitions">
        /// The repetitions.
        /// </param>
        /// <param name="routine">
        /// The routine.
        /// </param>
        internal SoundEffect(ushort sound, byte volume, byte repetitions, ushort routine)
        {
            this.sound = sound;
            this.volume = volume;
            this.repetitions = repetitions;
            this.routine = routine;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundEffect"/> class.
        /// </summary>
        /// <param name="sound">
        /// The sound.
        /// </param>
        /// <param name="volume">
        /// The volume.
        /// </param>
        /// <param name="loops">
        /// A value indicating whether the sound loops.
        /// </param>
        internal SoundEffect(ushort sound, byte volume, bool loops) : this(sound, volume, loops ? byte.MaxValue : (byte)0, 0)
        {
        }

        /// <summary>
        /// Gets the routine.
        /// </summary>
        /// <value>
        /// The routine.
        /// </value>
        internal ushort Routine
        {
            get
            {
                return this.routine;
            }
        }

        /// <summary>
        /// Gets the sound.
        /// </summary>
        /// <value>
        /// The sound.
        /// </value>
        internal ushort Sound
        {
            get
            {
                return this.sound;
            }
        }

        /// <summary>
        /// Gets the volume.
        /// </summary>
        /// <value>
        /// The volume.
        /// </value>
        internal byte Volume
        {
            get
            {
                return this.volume;
            }
        }

        /// <summary>
        /// Indicates whether the sound should loop.
        /// </summary>
        /// <returns>
        /// A value indicating whether the sound should loop.
        /// </returns>
        internal bool Loop()
        {
            if (this.repetitions > 0 && this.repetitions < byte.MaxValue)
            {
                this.repetitions--;
            }

            return this.repetitions > 0;
        }
    }
}