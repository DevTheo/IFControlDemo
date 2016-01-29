// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImmutableStackExtensions.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   Immutable stack extension methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    using System.Collections.Generic;

    /// <summary>
    /// Immutable stack extension methods.
    /// </summary>
    internal static class ImmutableStackExtensions
    {
        /// <summary>
        /// Adds an item to a stack.
        /// </summary>
        /// <param name="stack">
        /// The stack.
        /// </param>
        /// <param name="item">
        /// The item to add.
        /// </param>
        /// <typeparam name="T">
        /// The element type.
        /// </typeparam>
        /// <returns>
        /// A new stack containing the item.
        /// </returns>
        internal static ImmutableStack<T> Add<T>(this ImmutableStack<T> stack, T item)
        {
            return new ImmutableStack<T>(stack, item);
        }

        /// <summary>
        /// Gets the element count of a stack.
        /// </summary>
        /// <param name="stack">
        /// The stack.
        /// </param>
        /// <typeparam name="T">
        /// The element type.
        /// </typeparam>
        /// <returns>
        /// The element count.
        /// </returns>
        internal static int Count<T>(this ImmutableStack<T> stack)
        {
            return ImmutableStack<T>.Count(stack);
        }

        /// <summary>
        /// Gets an enumerable collection of values from a stack.
        /// </summary>
        /// <typeparam name="T">
        /// The element type.
        /// </typeparam>
        /// <param name="stack">
        /// The stack.
        /// </param>
        /// <returns>
        /// The enumerable collection of values from the stack.
        /// </returns>
        internal static IEnumerable<T> Enumerable<T>(this ImmutableStack<T> stack)
        {
            return ImmutableStack<T>.Enumerable(stack);
        }

        /// <summary>
        /// Reverses a stack.
        /// </summary>
        /// <typeparam name="T">
        /// The element type.
        /// </typeparam>
        /// <param name="stack">
        /// The stack.
        /// </param>
        /// <returns>
        /// The reversed stack.
        /// </returns>
        internal static ImmutableStack<T> Reverse<T>(this ImmutableStack<T> stack)
        {
            return ImmutableStack<T>.Reverse(stack);
        }

        /// <summary>
        /// Converts a stack of characters to a string.
        /// </summary>
        /// <param name="characters">
        /// The stack of characters.
        /// </param>
        /// <returns>
        /// The string.
        /// </returns>
        internal static string StackToString(this ImmutableStack<char> characters)
        {
            var chars = new char[characters.Count()];
            var characterNumber = 0;
            foreach (var character in characters.Enumerable())
            {
                chars[characterNumber++] = character;
            }

            return new string(chars);
        }
    }
}