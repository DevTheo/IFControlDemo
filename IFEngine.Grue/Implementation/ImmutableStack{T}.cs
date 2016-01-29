// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImmutableStack{T}.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   An immutable stack.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    using System.Collections.Generic;

    /// <summary>
    /// An immutable stack.
    /// </summary>
    /// <typeparam name="T">
    /// The element type.
    /// </typeparam>
    internal sealed class ImmutableStack<T>
    {
        /// <summary>
        /// The element count.
        /// </summary>
        private readonly int count;

        /// <summary>
        /// The stack tail.
        /// </summary>
        private readonly ImmutableStack<T> tail;

        /// <summary>
        /// The stack top.
        /// </summary>
        private readonly T top;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableStack{T}"/> class.
        /// </summary>
        /// <param name="top">
        /// The element to add to the top of the stack.
        /// </param>
        internal ImmutableStack(T top) : this(null, top)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableStack{T}"/> class.
        /// </summary>
        /// <param name="tail">
        /// The stack tail.
        /// </param>
        /// <param name="top">
        /// The element to add to the top of the stack.
        /// </param>
        internal ImmutableStack(ImmutableStack<T> tail, T top)
        {
            this.count = 1 + Count(tail);
            this.tail = tail;
            this.top = top;
        }

        /// <summary>
        /// Gets the stack tail.
        /// </summary>
        /// <value>
        /// The stack tail.
        /// </value>
        internal ImmutableStack<T> Tail
        {
            get
            {
                return this.tail;
            }
        }

        /// <summary>
        /// Gets the stack top.
        /// </summary>
        /// <value>
        /// The stack top.
        /// </value>
        internal T Top
        {
            get
            {
                return this.top;
            }
        }

        /// <summary>
        /// Gets the element count of a stack.
        /// </summary>
        /// <param name="stack">
        /// The stack.
        /// </param>
        /// <returns>
        /// The element count.
        /// </returns>
        internal static int Count(ImmutableStack<T> stack)
        {
            return stack == null ? 0 : stack.count;
        }

        /// <summary>
        /// Gets an enumerable collection of values from a stack.
        /// </summary>
        /// <param name="stack">
        /// The stack.
        /// </param>
        /// <returns>
        /// The enumerable collection of values from the stack.
        /// </returns>
        internal static IEnumerable<T> Enumerable(ImmutableStack<T> stack)
        {
            while (stack != null)
            {
                yield return stack.Top;
                stack = stack.Tail;
            }
        }

        /// <summary>
        /// Reverses a stack.
        /// </summary>
        /// <param name="stack">
        /// The stack.
        /// </param>
        /// <returns>
        /// The reversed stack.
        /// </returns>
        internal static ImmutableStack<T> Reverse(ImmutableStack<T> stack)
        {
            ImmutableStack<T> reversed = null;
            foreach (var element in Enumerable(stack))
            {
                reversed = reversed.Add(element);
            }

            return reversed;
        }
    }
}