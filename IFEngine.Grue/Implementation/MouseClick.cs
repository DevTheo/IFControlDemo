// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MouseClick.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   A mouse click.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// A mouse click.
    /// </summary>
    internal struct MouseClick
    {
        /// <summary>
        /// The click position.
        /// </summary>
        private readonly DisplayPosition clickPosition;

        /// <summary>
        /// The click type.
        /// </summary>
        private readonly ClickType clickType;

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseClick"/> struct.
        /// </summary>
        /// <param name="clickType">
        /// The click type.
        /// </param>
        /// <param name="clickPosition">
        /// The click position.
        /// </param>
        internal MouseClick(ClickType clickType, DisplayPosition clickPosition)
        {
            this.clickType = clickType;
            this.clickPosition = clickPosition;
        }

        /// <summary>
        /// Gets the click position.
        /// </summary>
        /// <value>
        /// The click position.
        /// </value>
        internal DisplayPosition ClickPosition
        {
            get
            {
                return this.clickPosition;
            }
        }

        /// <summary>
        /// Gets the click type.
        /// </summary>
        /// <value>
        /// The click type.
        /// </value>
        internal ClickType ClickType
        {
            get
            {
                return this.clickType;
            }
        }

        /// <summary>
        /// Operator ==.
        /// </summary>
        /// <param name="first">
        /// The first mouse click to compare.
        /// </param>
        /// <param name="second">
        /// The second mouse click to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two mouse clicks are equal.
        /// </returns>
        public static bool operator ==(MouseClick first, MouseClick second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Operator !=.
        /// </summary>
        /// <param name="first">
        /// The first mouse click to compare.
        /// </param>
        /// <param name="second">
        /// The second mouse click to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two mouse clicks are not equal.
        /// </returns>
        public static bool operator !=(MouseClick first, MouseClick second)
        {
            return !first.Equals(second);
        }

        /// <summary>
        /// Determines whether two mouse clicks are equal.
        /// </summary>
        /// <param name="obj">
        /// The object to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two mouse clicks are equal.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is MouseClick && this.Equals((MouseClick)obj);
        }

        /// <summary>
        /// Determines whether two mouse clicks are equal.
        /// </summary>
        /// <param name="mouseClick">
        /// The mouse click to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two mouse clicks are equal.
        /// </returns>
        public bool Equals(MouseClick mouseClick)
        {
            return this.clickType == mouseClick.clickType && this.clickPosition == mouseClick.clickPosition;
        }

        /// <summary>
        /// Calculates the hashcode.
        /// </summary>
        /// <returns>
        /// The hashcode.
        /// </returns>
        public override int GetHashCode()
        {
            return this.clickPosition.GetHashCode();
        }
    }
}