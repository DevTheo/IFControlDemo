// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RandomNumberGenerator.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   The zmachine random number generator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// The zmachine random number generator.
    /// </summary>
    internal sealed class RandomNumberGenerator
    {
        /// <summary>
        /// The random number generator.
        /// </summary>
        private readonly IRandomNumberGenerator generator;

        /// <summary>
        /// The next value to return when counting is enabled.
        /// </summary>
        private int count;

        /// <summary>
        /// A value indicating whether the generator should output a sequence of increasing values.
        /// </summary>
        private bool counting;

        /// <summary>
        /// The generator seed.
        /// </summary>
        private int seed;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomNumberGenerator"/> class.
        /// </summary>
        /// <param name="generator">
        /// The random number generator generator.
        /// </param>
        internal RandomNumberGenerator(IRandomNumberGenerator generator)
        {
            this.generator = generator;
        }

        /// <summary>
        /// Generates a random number.
        /// </summary>
        /// <returns>
        /// A random number.
        /// </returns>
        internal int Generate()
        {
            if (this.counting)
            {
                var result = this.count;
                this.count = (this.count + 1) % this.seed;
                return result;
            }

            return this.generator.Generate();
        }

        /// <summary>
        /// Seeds the random number generator.
        /// </summary>
        /// <param name="newSeed">
        /// The new seed.
        /// </param>
        /// <param name="countingMode">
        /// A value indicating whether the generator should output a sequence of increasing values.
        /// </param>
        internal void Seed(int newSeed, bool countingMode)
        {
            this.count = 0;
            this.seed = newSeed;
            this.counting = countingMode;
            if (!countingMode)
            {
                this.generator.Seed(newSeed);
            }
        }
    }
}