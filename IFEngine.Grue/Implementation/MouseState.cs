// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MouseState.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   A mouse state.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// A mouse state.
    /// </summary>
    internal struct MouseState
    {
        /// <summary>
        /// The buttons pressed.
        /// </summary>
        private readonly MouseButtons buttonsPressed;

        /// <summary>
        /// The mouse cursor position.
        /// </summary>
        private readonly DisplayPosition cursorPosition;

        /// <summary>
        /// The menu item clicked.
        /// </summary>
        private readonly byte menuItemSelected;

        /// <summary>
        /// The menu clicked.
        /// </summary>
        private readonly byte menuSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseState"/> struct.
        /// </summary>
        /// <param name="cursorPosition">
        /// The mouse cursor position.
        /// </param>
        /// <param name="buttonsPressed">
        /// The buttons pressed.
        /// </param>
        /// <param name="menuSelected">
        /// The menu clicked.
        /// </param>
        /// <param name="menuItemSelected">
        /// The menu item clicked.
        /// </param>
        internal MouseState(DisplayPosition cursorPosition, MouseButtons buttonsPressed, byte menuSelected, byte menuItemSelected)
        {
            this.cursorPosition = cursorPosition;
            this.buttonsPressed = buttonsPressed;
            this.menuSelected = menuSelected;
            this.menuItemSelected = menuItemSelected;
        }

        /// <summary>
        /// Gets the buttons pressed.
        /// </summary>
        /// <value>
        /// The buttons pressed.
        /// </value>
        internal MouseButtons ButtonsPressed
        {
            get
            {
                return this.buttonsPressed;
            }
        }

        /// <summary>
        /// Gets the mouse cursor position.
        /// </summary>
        /// <value>
        /// The mouse cursor position.
        /// </value>
        internal DisplayPosition CursorPosition
        {
            get
            {
                return this.cursorPosition;
            }
        }

        /// <summary>
        /// Gets the menu item clicked.
        /// </summary>
        /// <value>
        /// The menu item clicked.
        /// </value>
        internal byte MenuItemSelected
        {
            get
            {
                return this.menuItemSelected;
            }
        }

        /// <summary>
        /// Gets the menu clicked.
        /// </summary>
        /// <value>
        /// The menu clicked.
        /// </value>
        internal byte MenuSelected
        {
            get
            {
                return this.menuSelected;
            }
        }

        /// <summary>
        /// Operator ==.
        /// </summary>
        /// <param name="first">
        /// The first mouse state to compare.
        /// </param>
        /// <param name="second">
        /// The second mouse state to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two mouse states are equal.
        /// </returns>
        public static bool operator ==(MouseState first, MouseState second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Operator !=.
        /// </summary>
        /// <param name="first">
        /// The first mouse state to compare.
        /// </param>
        /// <param name="second">
        /// The second mouse state to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two mouse states are not equal.
        /// </returns>
        public static bool operator !=(MouseState first, MouseState second)
        {
            return !first.Equals(second);
        }

        /// <summary>
        /// Determines whether two mouse states are equal.
        /// </summary>
        /// <param name="obj">
        /// The object to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two mouse states are equal.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is MouseState && this.Equals((MouseState)obj);
        }

        /// <summary>
        /// Determines whether two mouse states are equal.
        /// </summary>
        /// <param name="mouseState">
        /// The mouse state to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two mouse states are equal.
        /// </returns>
        public bool Equals(MouseState mouseState)
        {
            return this.buttonsPressed == mouseState.buttonsPressed && this.cursorPosition == mouseState.cursorPosition && this.menuItemSelected == mouseState.menuItemSelected && this.menuSelected == mouseState.menuSelected;
        }

        /// <summary>
        /// Calculates the hashcode.
        /// </summary>
        /// <returns>
        /// The hashcode.
        /// </returns>
        public override int GetHashCode()
        {
            return this.cursorPosition.GetHashCode();
        }
    }
}