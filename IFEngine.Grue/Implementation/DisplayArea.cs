// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayArea.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   An area of the display.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// An area of the display.
    /// </summary>
    internal struct DisplayArea
    {
        /// <summary>
        /// The display area size.
        /// </summary>
        private readonly DisplayAreaSize size;

        /// <summary>
        /// The top left corner position.
        /// </summary>
        private readonly DisplayPosition topLeftCornerPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayArea"/> struct.
        /// </summary>
        /// <param name="topLeftCornerPosition">
        /// The top left corner position.
        /// </param>
        /// <param name="size">
        /// The display area size.
        /// </param>
        internal DisplayArea(DisplayPosition topLeftCornerPosition, DisplayAreaSize size)
        {
            this.topLeftCornerPosition = topLeftCornerPosition;
            this.size = size;
        }

        /// <summary>
        /// Gets the display area size.
        /// </summary>
        /// <value>
        /// The display area size.
        /// </value>
        internal DisplayAreaSize Size
        {
            get
            {
                return this.size;
            }
        }

        /// <summary>
        /// Gets the top left corner position.
        /// </summary>
        /// <value>
        /// The top left corner position.
        /// </value>
        internal DisplayPosition TopLeftCornerPosition
        {
            get
            {
                return this.topLeftCornerPosition;
            }
        }

        /// <summary>
        /// Operator ==.
        /// </summary>
        /// <param name="first">
        /// The first display area to compare.
        /// </param>
        /// <param name="second">
        /// The second display area to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two display areas are equal.
        /// </returns>
        public static bool operator ==(DisplayArea first, DisplayArea second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Operator !=.
        /// </summary>
        /// <param name="first">
        /// The first display area to compare.
        /// </param>
        /// <param name="second">
        /// The second display area to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two display areas are not equal.
        /// </returns>
        public static bool operator !=(DisplayArea first, DisplayArea second)
        {
            return !first.Equals(second);
        }

        /// <summary>
        /// Determines whether the area contains a given position.
        /// </summary>
        /// <param name="position">
        /// The display position.
        /// </param>
        /// <returns>
        /// A value indicating whether the display area contains the given display position.
        /// </returns>
        internal bool ContainsPosition(DisplayPosition position)
        {
            var cornerRow = this.topLeftCornerPosition.Row;
            var cornerColumn = this.topLeftCornerPosition.Column;
            return position.Row >= cornerRow && position.Column >= cornerColumn && position.Row < cornerRow + this.size.Height && position.Column < cornerColumn + this.size.Width;
        }

        /// <summary>
        /// Determines whether two display areas are equal.
        /// </summary>
        /// <param name="obj">
        /// The object to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two display areas are equal.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is DisplayArea && this.Equals((DisplayArea)obj);
        }

        /// <summary>
        /// Determines whether two display areas are equal.
        /// </summary>
        /// <param name="displayArea">
        /// The display area to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two display areas are equal.
        /// </returns>
        internal bool Equals(DisplayArea displayArea)
        {
            return this.size == displayArea.size && this.topLeftCornerPosition == displayArea.topLeftCornerPosition;
        }

        /// <summary>
        /// Calculates the hashcode.
        /// </summary>
        /// <returns>
        /// The hashcode.
        /// </returns>
        public override int GetHashCode()
        {
            return this.size.GetHashCode();
        }
    }
}