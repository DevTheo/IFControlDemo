// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayPosition.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   A display position.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// A display position.
    /// </summary>
    internal struct DisplayPosition
    {
        /// <summary>
        /// The display column.
        /// </summary>
        private readonly int column;

        /// <summary>
        /// The display row.
        /// </summary>
        private readonly int row;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayPosition"/> struct.
        /// </summary>
        /// <param name="column">
        /// The display column.
        /// </param>
        /// <param name="row">
        /// The display row.
        /// </param>
        internal DisplayPosition(int column, int row)
        {
            this.row = row;
            this.column = column;
        }

        /// <summary>
        /// Gets the display column.
        /// </summary>
        /// <value>
        /// The display column.
        /// </value>
        internal int Column
        {
            get
            {
                return this.column;
            }
        }

        /// <summary>
        /// Gets the display row.
        /// </summary>
        /// <value>
        /// The display row.
        /// </value>
        internal int Row
        {
            get
            {
                return this.row;
            }
        }

        /// <summary>
        /// Operator ==.
        /// </summary>
        /// <param name="first">
        /// The first display position to compare.
        /// </param>
        /// <param name="second">
        /// The second display position to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two display positions are equal.
        /// </returns>
        public static bool operator ==(DisplayPosition first, DisplayPosition second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Operator !=.
        /// </summary>
        /// <param name="first">
        /// The first display position to compare.
        /// </param>
        /// <param name="second">
        /// The second display position to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two display positions are not equal.
        /// </returns>
        public static bool operator !=(DisplayPosition first, DisplayPosition second)
        {
            return !first.Equals(second);
        }

        /// <summary>
        /// Determines whether two display positions are equal.
        /// </summary>
        /// <param name="obj">
        /// The object to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two display positions are equal.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is DisplayPosition && this.Equals((DisplayPosition)obj);
        }

        /// <summary>
        /// Determines whether two display positions are equal.
        /// </summary>
        /// <param name="displayPosition">
        /// The display position to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two display positions are equal.
        /// </returns>
        public bool Equals(DisplayPosition displayPosition)
        {
            return this.row == displayPosition.row && this.column == displayPosition.column;
        }

        /// <summary>
        /// Calculates the hashcode.
        /// </summary>
        /// <returns>
        /// The hashcode.
        /// </returns>
        public override int GetHashCode()
        {
            return this.column ^ this.row;
        }
    }
}