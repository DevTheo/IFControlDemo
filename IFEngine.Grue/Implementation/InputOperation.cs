// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputOperation.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   A zmachine input operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// A zmachine input operation.
    /// </summary>
    internal sealed class InputOperation
    {
        /// <summary>
        /// The call stack catch value when the input operation began.
        /// </summary>
        private readonly int catchValue;

        /// <summary>
        /// The maximum input character count.
        /// </summary>
        private readonly byte maxCharacters;

        /// <summary>
        /// The parent.
        /// </summary>
        private readonly InputOperation parent;

        /// <summary>
        /// The parse buffer address.
        /// </summary>
        private readonly ushort parseBuffer;

        /// <summary>
        /// A value indicating whether the input operation reads only a single character.
        /// </summary>
        private readonly bool readCharacter;

        /// <summary>
        /// The text buffer address.
        /// </summary>
        private readonly ushort textBuffer;

        /// <summary>
        /// The timeout inerval in milliseconds.
        /// </summary>
        private readonly int timeoutInterval;

        /// <summary>
        /// Timeout routine address.
        /// </summary>
        private readonly ushort timeoutRoutine;

        /// <summary>
        /// The cursor position.
        /// </summary>
        private DisplayPosition cursor;

        /// <summary>
        /// The elapsed time.
        /// </summary>
        private long elapsedTime;

        /// <summary>
        /// The input text.
        /// </summary>
        private ImmutableStack<char> inputText;

        /// <summary>
        /// The number of milliseconds elapsed during any child input operations.
        /// </summary>
        private long timeInChildOperations;

        /// <summary>
        /// The timeout threshold.
        /// </summary>
        private long timeoutThreshold;

        /// <summary>
        /// The unprocessed input values.
        /// </summary>
        private ImmutableQueue<InputValue> unprocessedInputValues;

        /// <summary>
        /// The display window.
        /// </summary>
        private DisplayWindow window;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputOperation"/> class.
        /// </summary>
        /// <param name="textBuffer">
        /// The text buffer address.
        /// </param>
        /// <param name="parseBuffer">
        /// The parse buffer address.
        /// </param>
        /// <param name="maxCharacters">
        /// The maximum characters that can be input.
        /// </param>
        internal InputOperation(ushort textBuffer, ushort parseBuffer, byte maxCharacters) : this(textBuffer, parseBuffer, maxCharacters, null, 0, 0, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputOperation"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <param name="catchValue">
        /// The call stack catch value when the input operation began.
        /// </param>
        /// <param name="timeoutInterval">
        /// The timeout interval.
        /// </param>
        /// <param name="timeoutRoutine">
        /// The timeout routine.
        /// </param>
        internal InputOperation(InputOperation parent, int catchValue, ushort timeoutInterval, ushort timeoutRoutine) : this(0, 0, 0, parent, catchValue, timeoutInterval, timeoutRoutine)
        {
            this.readCharacter = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputOperation"/> class.
        /// </summary>
        /// <param name="textBuffer">
        /// The text buffer address.
        /// </param>
        /// <param name="parseBuffer">
        /// The parse buffer address.
        /// </param>
        /// <param name="maxCharacters">
        /// The maximum characters that can be input.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <param name="catchValue">
        /// The call stack catch value when the input operation began.
        /// </param>
        /// <param name="timeoutInterval">
        /// The timeout interval.
        /// </param>
        /// <param name="timeoutRoutine">
        /// The timeout routine.
        /// </param>
        internal InputOperation(ushort textBuffer, ushort parseBuffer, byte maxCharacters, InputOperation parent, int catchValue, ushort timeoutInterval, ushort timeoutRoutine)
        {
            this.textBuffer = textBuffer;
            this.parseBuffer = parseBuffer;
            this.maxCharacters = maxCharacters;
            this.parent = parent;
            this.catchValue = catchValue;
            this.timeoutThreshold = this.timeoutInterval = timeoutInterval * 100;
            this.timeoutRoutine = timeoutRoutine;
        }

        /// <summary>
        /// Gets the call stack catch value when the input operation began.
        /// </summary>
        /// <value>
        /// The call stack catch value when the input operation began.
        /// </value>
        internal int CatchValue
        {
            get
            {
                return this.catchValue;
            }
        }

        /// <summary>
        /// Gets the cursor position.
        /// </summary>
        /// <value>
        /// The cursor position.
        /// </value>
        internal DisplayPosition Cursor
        {
            get
            {
                return this.cursor;
            }
        }

        /// <summary>
        /// Gets or sets the elapsed time.
        /// </summary>
        /// <value>
        /// The elapsed time.
        /// </value>
        internal long ElapsedTime
        {
            get
            {
                return this.elapsedTime;
            }

            set
            {
                if (this.Timed && value > this.elapsedTime)
                {
                    this.elapsedTime = value;
                    if (this.parent != null)
                    {
                        this.parent.timeInChildOperations = this.elapsedTime;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the finished input text.
        /// </summary>
        /// <value>
        /// The finished input text.
        /// </value>
        internal string InputText
        {
            get
            {
                return this.inputText.Reverse().StackToString();
            }
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        internal InputOperation Parent
        {
            get
            {
                return this.parent;
            }
        }

        /// <summary>
        /// Gets the parse buffer address.
        /// </summary>
        /// <value>
        /// The parse buffer.
        /// </value>
        internal ushort ParseBuffer
        {
            get
            {
                return this.parseBuffer;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the input operation reads only a single character.
        /// </summary>
        /// <value>
        /// A value indicating whether the input operation reads only a single character.
        /// </value>
        internal bool ReadCharacter
        {
            get
            {
                return this.readCharacter;
            }
        }

        /// <summary>
        /// Gets the text buffer address.
        /// </summary>
        /// <value>
        /// The text buffer.
        /// </value>
        internal ushort TextBuffer
        {
            get
            {
                return this.textBuffer;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the operation is timed.
        /// </summary>
        /// <value>
        /// A value indicating whether the operation is timed.
        /// </value>
        internal bool Timed
        {
            get
            {
                return this.timeoutInterval != 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the input operation has timed out.
        /// </summary>
        /// <value>
        /// A value indicating whether the input operation has timed out.
        /// </value>
        internal bool TimedOut
        {
            get
            {
                return this.Timed && this.elapsedTime - this.timeInChildOperations >= this.timeoutThreshold;
            }
        }

        /// <summary>
        /// Gets the timeout routine.
        /// </summary>
        /// <value>
        /// The timeout routine.
        /// </value>
        internal ushort TimeoutRoutine
        {
            get
            {
                return this.timeoutRoutine;
            }
        }

        /// <summary>
        /// Gets the unprocessed input values.
        /// </summary>
        /// <value>
        /// The unprocessed input values.
        /// </value>
        internal ImmutableQueue<InputValue> UnprocessedInputValues
        {
            get
            {
                return this.unprocessedInputValues;
            }
        }

        /// <summary>
        /// Gets the display window.
        /// </summary>
        /// <value>
        /// The display window.
        /// </value>
        internal DisplayWindow Window
        {
            get
            {
                return this.window;
            }
        }

        /// <summary>
        /// Adds a character to the input buffer if the maximum character count has not been reached.
        /// </summary>
        /// <param name="character">
        /// The character.
        /// </param>
        /// <returns>
        /// A value indicating whether the character was added.
        /// </returns>
        internal bool AddCharacter(char character)
        {
            if (this.readCharacter || this.inputText.Count() == this.maxCharacters)
            {
                return false;
            }

            this.inputText = this.inputText.Add(character);
            return true;
        }

        /// <summary>
        /// Deletes a character from the input buffer if any are present.
        /// </summary>
        /// <returns>
        /// A value indicating whether the character was deleted.
        /// </returns>
        internal bool DeleteCharacter()
        {
            if (this.readCharacter || this.inputText == null)
            {
                return false;
            }

            this.inputText = this.inputText.Tail;
            return true;
        }

        /// <summary>
        /// Resets an input operation which has timed out.
        /// </summary>
        internal void ResetTimeout()
        {
            this.timeoutThreshold += this.timeoutInterval;
        }

        /// <summary>
        /// Updates the display status of the input operation.
        /// </summary>
        /// <param name="displayWindow">
        /// The display window.
        /// </param>
        /// <param name="cursorPosition">
        /// The cursor position.
        /// </param>
        internal void UpdateDisplayStatus(DisplayWindow displayWindow, DisplayPosition cursorPosition)
        {
            this.window = displayWindow;
            this.cursor = cursorPosition;
        }

        /// <summary>
        /// Updates the input.
        /// </summary>
        /// <param name="inputValues">
        /// The input values.
        /// </param>
        internal void UpdateInput(ImmutableQueue<InputValue> inputValues)
        {
            this.unprocessedInputValues = inputValues;
        }
    }
}