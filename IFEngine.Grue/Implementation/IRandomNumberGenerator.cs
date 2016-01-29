// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRandomNumberGenerator.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   The random number generator interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// The random number generator interface.
    /// </summary>
    internal interface IRandomNumberGenerator
    {
        /// <summary>
        /// Gets the next random number.
        /// </summary>
        /// <returns>
        /// The next random number.
        /// </returns>
        int Generate();

        /// <summary>
        /// Seeds the generator.
        /// </summary>
        /// <param name="generatorSeed">
        /// The generator seed.
        /// </param>
        void Seed(int generatorSeed);
    }
}