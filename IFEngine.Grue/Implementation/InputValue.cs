// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputValue.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   An input value.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// An input value.
    /// </summary>
    internal struct InputValue
    {
        /// <summary>
        /// The value.
        /// </summary>
        private readonly object value;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputValue"/> struct.
        /// </summary>
        /// <param name="character">
        /// The character.
        /// </param>
        internal InputValue(char character)
        {
            this.value = character;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputValue"/> struct.
        /// </summary>
        /// <param name="mouseClick">
        /// The mouse click.
        /// </param>
        internal InputValue(MouseClick mouseClick)
        {
            this.value = mouseClick;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputValue"/> struct.
        /// </summary>
        /// <param name="inputKey">
        /// The input character.
        /// </param>
        internal InputValue(InputKey inputKey)
        {
            this.value = inputKey;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        internal object Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// Operator ==.
        /// </summary>
        /// <param name="first">
        /// The first input value to compare.
        /// </param>
        /// <param name="second">
        /// The second input value to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two input values are equal.
        /// </returns>
        public static bool operator ==(InputValue first, InputValue second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Operator !=.
        /// </summary>
        /// <param name="first">
        /// The first input value to compare.
        /// </param>
        /// <param name="second">
        /// The second input value to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two input values are not equal.
        /// </returns>
        public static bool operator !=(InputValue first, InputValue second)
        {
            return !first.Equals(second);
        }

        /// <summary>
        /// Determines whether two input values are equal.
        /// </summary>
        /// <param name="obj">
        /// The object to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two input values are equal.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is InputValue && this.Equals((InputValue)obj);
        }

        /// <summary>
        /// Determines whether two input values are equal.
        /// </summary>
        /// <param name="inputValue">
        /// The input value to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two input values are equal.
        /// </returns>
        public bool Equals(InputValue inputValue)
        {
            return this.value.Equals(inputValue.value);
        }

        /// <summary>
        /// Calculates the hashcode.
        /// </summary>
        /// <returns>
        /// The hashcode.
        /// </returns>
        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }
    }
}