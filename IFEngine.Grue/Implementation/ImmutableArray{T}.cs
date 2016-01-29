// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImmutableArray{T}.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   An immutable single dimensional array.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// An immutable single dimensional array.
    /// </summary>
    /// <typeparam name="T">
    /// The element type.
    /// </typeparam>
    internal sealed class ImmutableArray<T>
    {
        /// <summary>
        /// The array of data.
        /// </summary>
        private readonly T[] data;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableArray{T}"/> class.
        /// </summary>
        /// <param name="data">
        /// The array of data.
        /// </param>
        internal ImmutableArray(T[] data)
        {
            this.data = new T[data.Length];
            data.CopyTo(this.data, 0);
        }

        /// <summary>
        /// Gets the array length.
        /// </summary>
        /// <value>
        /// The array length.
        /// </value>
        internal int Length
        {
            get
            {
                return this.data.Length;
            }
        }

        /// <summary>
        /// Gets the elements of the array.
        /// </summary>
        /// <param name="index">
        /// The index of the element.
        /// </param>
        /// <returns>
        /// The element at the given index.
        /// </returns>
        internal T this[int index]
        {
            get
            {
                return this.data[index];
            }
        }

        /// <summary>
        /// Returns an array containig the same elements as the immutable array.
        /// </summary>
        /// <returns>
        /// An array containig the same elements as the immutable array.
        /// </returns>
        internal T[] ToArray()
        {
            var result = new T[this.data.Length];
            this.data.CopyTo(result, 0);
            return result;
        }
    }
}