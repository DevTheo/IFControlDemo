// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImmutableQueueExtensions.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   Immutable queue extension methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    using System.Collections.Generic;

    /// <summary>
    /// Immutable queue extension methods.
    /// </summary>
    internal static class ImmutableQueueExtensions
    {
        /// <summary>
        /// Adds an item to a queue.
        /// </summary>
        /// <param name="queue">
        /// The queue.
        /// </param>
        /// <param name="item">
        /// The item to add.
        /// </param>
        /// <typeparam name="T">
        /// The element type.
        /// </typeparam>
        /// <returns>
        /// A new queue containing the item.
        /// </returns>
        internal static ImmutableQueue<T> Add<T>(this ImmutableQueue<T> queue, T item)
        {
            return new ImmutableQueue<T>(queue, item);
        }

        /// <summary>
        /// Gets the element count of a queue.
        /// </summary>
        /// <param name="queue">
        /// The queue.
        /// </param>
        /// <typeparam name="T">
        /// The element type.
        /// </typeparam>
        /// <returns>
        /// The element count.
        /// </returns>
        internal static int Count<T>(this ImmutableQueue<T> queue)
        {
            return ImmutableQueue<T>.Count(queue);
        }

        /// <summary>
        /// Gets an enumerable collection of values from a queue.
        /// </summary>
        /// <typeparam name="T">
        /// The element type.
        /// </typeparam>
        /// <param name="queue">
        /// The queue.
        /// </param>
        /// <returns>
        /// The enumerable collection of values from the queue.
        /// </returns>
        internal static IEnumerable<T> Enumerable<T>(this ImmutableQueue<T> queue)
        {
            return ImmutableQueue<T>.Enumerable(queue);
        }
    }
}