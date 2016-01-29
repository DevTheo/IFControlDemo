// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayAreaSize.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   The size of a display area.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// The size of a display area.
    /// </summary>
    internal struct DisplayAreaSize
    {
        /// <summary>
        /// The height.
        /// </summary>
        private readonly int height;

        /// <summary>
        /// The width.
        /// </summary>
        private readonly int width;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayAreaSize"/> struct.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        internal DisplayAreaSize(int width, int height)
        {
            this.height = height;
            this.width = width;
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        internal int Height
        {
            get
            {
                return this.height;
            }
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        internal int Width
        {
            get
            {
                return this.width;
            }
        }

        /// <summary>
        /// Operator ==.
        /// </summary>
        /// <param name="first">
        /// The first area size to compare.
        /// </param>
        /// <param name="second">
        /// The second area size to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two area sizes are equal.
        /// </returns>
        public static bool operator ==(DisplayAreaSize first, DisplayAreaSize second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Operator !=.
        /// </summary>
        /// <param name="first">
        /// The first area size to compare.
        /// </param>
        /// <param name="second">
        /// The second area size to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two area sizes are not equal.
        /// </returns>
        public static bool operator !=(DisplayAreaSize first, DisplayAreaSize second)
        {
            return !first.Equals(second);
        }

        /// <summary>
        /// Determines whether two area sizes are equal.
        /// </summary>
        /// <param name="obj">
        /// The object to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two area sizes are equal.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is DisplayAreaSize && this.Equals((DisplayAreaSize)obj);
        }

        /// <summary>
        /// Determines whether two area sizes are equal.
        /// </summary>
        /// <param name="displayAreaSize">
        /// The area size to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two area sizes are equal.
        /// </returns>
        internal bool Equals(DisplayAreaSize displayAreaSize)
        {
            return this.width == displayAreaSize.width && this.height == displayAreaSize.height;
        }

        /// <summary>
        /// Calculates the hashcode.
        /// </summary>
        /// <returns>
        /// The hashcode.
        /// </returns>
        public override int GetHashCode()
        {
            return this.width ^ this.height;
        }
    }
}