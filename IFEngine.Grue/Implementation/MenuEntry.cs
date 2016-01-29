// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuEntry.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   A menu entry.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// A menu entry.
    /// </summary>
    internal sealed class MenuEntry
    {
        /// <summary>
        /// The menu name.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// The menu number.
        /// </summary>
        private readonly int number;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuEntry"/> class.
        /// </summary>
        /// <param name="name">
        /// The menu name.
        /// </param>
        /// <param name="number">
        /// The menu number.
        /// </param>
        internal MenuEntry(string name, int number)
        {
            this.name = name;
            this.number = number;
        }

        /// <summary>
        /// Gets the menu name.
        /// </summary>
        /// <value>
        /// The menu name.
        /// </value>
        internal string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets the menu number.
        /// </summary>
        /// <value>
        /// The menu number.
        /// </value>
        internal int Number
        {
            get
            {
                return this.number;
            }
        }
    }
}