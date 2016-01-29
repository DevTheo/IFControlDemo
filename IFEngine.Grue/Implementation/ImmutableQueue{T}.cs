// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImmutableQueue{T}.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   An immutable queue.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    using System.Collections.Generic;

    /// <summary>
    /// An immutable queue.
    /// </summary>
    /// <typeparam name="T">
    /// The element type.
    /// </typeparam>
    internal sealed class ImmutableQueue<T>
    {
        /// <summary>
        /// The dequeue stack.
        /// </summary>
        private readonly ImmutableStack<T> dequeueStack;

        /// <summary>
        /// The enqueue stack.
        /// </summary>
        private readonly ImmutableStack<T> enqueueStack;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableQueue{T}"/> class.
        /// </summary>
        /// <param name="back">
        /// The element to add to the back of the queue.
        /// </param>
        internal ImmutableQueue(T back) : this(null, back)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableQueue{T}"/> class.
        /// </summary>
        /// <param name="tail">
        /// The queue tail.
        /// </param>
        /// <param name="back">
        /// The element to add to the back of the queue.
        /// </param>
        internal ImmutableQueue(ImmutableQueue<T> tail, T back)
        {
            if (tail == null)
            {
                this.dequeueStack = this.dequeueStack.Add(back);
                return;
            }

            this.enqueueStack = tail.enqueueStack.Add(back);
            this.dequeueStack = tail.dequeueStack;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableQueue{T}"/> class.
        /// </summary>
        /// <param name="enqueueStack">
        /// The enqueue stack.
        /// </param>
        /// <param name="dequeueStack">
        /// The dequeue stack.
        /// </param>
        private ImmutableQueue(ImmutableStack<T> enqueueStack, ImmutableStack<T> dequeueStack)
        {
            this.enqueueStack = enqueueStack;
            this.dequeueStack = dequeueStack;
        }

        /// <summary>
        /// Gets the queue front.
        /// </summary>
        /// <value>
        /// The queue front.
        /// </value>
        internal T Front
        {
            get
            {
                return this.dequeueStack.Top;
            }
        }

        /// <summary>
        /// Gets the queue tail.
        /// </summary>
        /// <value>
        /// The queue tail.
        /// </value>
        internal ImmutableQueue<T> Tail
        {
            get
            {
                if (this.dequeueStack.Tail != null)
                {
                    return new ImmutableQueue<T>(this.enqueueStack, this.dequeueStack.Tail);
                }

                if (this.enqueueStack != null)
                {
                    return new ImmutableQueue<T>(null, this.enqueueStack.Reverse());
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the element count of a queue.
        /// </summary>
        /// <param name="queue">
        /// The queue.
        /// </param>
        /// <returns>
        /// The element count.
        /// </returns>
        internal static int Count(ImmutableQueue<T> queue)
        {
            return queue == null ? 0 : queue.dequeueStack.Count() + queue.enqueueStack.Count();
        }

        /// <summary>
        /// Gets an enumerable collection of values from a queue.
        /// </summary>
        /// <param name="queue">
        /// The queue.
        /// </param>
        /// <returns>
        /// The enumerable collection of values from the queue.
        /// </returns>
        internal static IEnumerable<T> Enumerable(ImmutableQueue<T> queue)
        {
            while (queue != null)
            {
                yield return queue.Front;
                queue = queue.Tail;
            }
        }
    }
}