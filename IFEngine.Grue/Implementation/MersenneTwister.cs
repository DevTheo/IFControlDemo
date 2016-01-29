// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MersenneTwister.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   A random number generator using the Mersenne Twister algorithm.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// A random number generator using the Mersenne Twister algorithm.
    /// </summary>
    internal sealed class MersenneTwister : IRandomNumberGenerator
    {
        /// <summary>
        /// The initialization multipler.
        /// </summary>
        private const uint InitializationMultipler = 0x6c078965;

        /// <summary>
        /// Constant LowerMask.
        /// </summary>
        private const uint LowerMask = 0x7fffffff;

        /// <summary>
        /// Constant M.
        /// </summary>
        private const uint M = 397;

        /// <summary>
        /// Constant MatrixA.
        /// </summary>
        private const uint MatrixA = 0x9908b0df;

        /// <summary>
        /// Constant N.
        /// </summary>
        private const uint N = 624;

        /// <summary>
        /// The tempering mask 'B'.
        /// </summary>
        private const uint TemperingMaskB = 0x9d2c5680;

        /// <summary>
        /// The tempering mask 'C'.
        /// </summary>
        private const uint TemperingMaskC = 0xefc60000;

        /// <summary>
        /// Constant UpperMask.
        /// </summary>
        private const uint UpperMask = 0x80000000;

        /// <summary>
        /// Readonly mag01.
        /// </summary>
        private static readonly ImmutableArray<uint> mag01 = InitializeMag01();

        /// <summary>
        /// The state vector.
        /// </summary>
        private readonly uint[] stateVector = new uint[N];

        /// <summary>
        /// The state vector index.
        /// </summary>
        private uint stateVectorIndex;

        /// <summary>
        /// Gets the next random number.
        /// </summary>
        /// <returns>
        /// The next random number.
        /// </returns>
        public int Generate()
        {
            uint result;
            if (this.stateVectorIndex >= N)
            {
                int index;
                for (index = 0; index < N - M; index++)
                {
                    result = (this.stateVector[index] & UpperMask) | (this.stateVector[index + 1] & LowerMask);
                    this.stateVector[index] = this.stateVector[index + M] ^ (result >> 1) ^ mag01[(int)(result & 1)];
                }

                for (; index < N - 1; index++)
                {
                    result = (this.stateVector[index] & UpperMask) | (this.stateVector[index + 1] & LowerMask);
                    this.stateVector[index] = this.stateVector[index + M - N] ^ (result >> 1) ^ mag01[(int)(result & 1)];
                }

                result = (this.stateVector[N - 1] & UpperMask) | (this.stateVector[0] & LowerMask);
                this.stateVector[N - 1] = this.stateVector[M - 1] ^ (result >> 1) ^ mag01[(int)(result & 1)];
                this.stateVectorIndex = 0;
            }

            result = this.stateVector[this.stateVectorIndex++];
            result ^= result >> 11;
            result ^= (result << 7) & TemperingMaskB;
            result ^= (result << 15) & TemperingMaskC;
            result ^= result >> 18;
            return (int)result;
        }

        /// <summary>
        /// Seeds the generator.
        /// </summary>
        /// <param name="generatorSeed">
        /// The generator seed.
        /// </param>
        public void Seed(int generatorSeed)
        {
            this.stateVector[0] = (uint)generatorSeed;
            for (this.stateVectorIndex = 1; this.stateVectorIndex < N; this.stateVectorIndex++)
            {
                var previousValue = this.stateVector[this.stateVectorIndex - 1];
                this.stateVector[this.stateVectorIndex] = (InitializationMultipler * (previousValue ^ (previousValue >> 30))) + this.stateVectorIndex;
            }
        }

        /// <summary>
        /// Initializes the mag01.
        /// </summary>
        /// <returns>
        /// The mag01.
        /// </returns>
        private static ImmutableArray<uint> InitializeMag01()
        {
            var mag01Builder = new uint[2];
            mag01Builder[0] = 0;
            mag01Builder[1] = MatrixA;
            return new ImmutableArray<uint>(mag01Builder);
        }
    }
}