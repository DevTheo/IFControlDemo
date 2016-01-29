// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZmachineV4.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   The zmachine version 4.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// The zmachine version 4.
    /// </summary>
    /// <remarks>
    /// Version 4 changes the modifiers for calculating routine and string addresses and the story length.
    ///   It also increases the number of objects, attributes, properties, and the dictionary entry length.
    ///   The upper window completely replaces the status line.
    ///   Some operations are modified:
    ///   Updating the status line is now an invalid operation.
    ///   Beginning an input operation no longer updates the status line.
    ///   Save and restore were changed from branch to store operations.
    ///   Splitting the display no longer erases the upper window.
    ///   It also adds operations to:
    ///   Read a single character of input.
    ///   Scan a table for a value.
    ///   Enable or disable buffering on the current window.
    ///   Get the cursor position.
    ///   Set the cursor position.
    ///   Erase a line.
    ///   Erase a window.
    ///   Change the current text style.
    ///   Call a function with no arguments.
    ///   Call a function with one argument.
    ///   Call a function with zero to seven arguments.
    ///   Infocom added timed input support in version 5, but some Inform version 4 games use it so it is implemented here.
    /// </remarks>
    internal class ZmachineV4 : ZmachineV3, IZmachine
    {
        /// <summary>
        /// A value indicating whether the output is buffered to allow word wrapping at the end of each line.
        /// </summary>
        private bool bufferOutput = true;

        /// <summary>
        /// The current text styles.
        /// </summary>
        private TextStyles textStyles;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZmachineV4"/> class.
        /// </summary>
        /// <param name="frontEnd">
        /// The front end.
        /// </param>
        /// <param name="story">
        /// The story.
        /// </param>
        /// <param name="random">
        /// The random number generator.
        /// </param>
        internal ZmachineV4(FrontEnd frontEnd, ImmutableArray<byte> story, IRandomNumberGenerator random) : base(frontEnd, story, random)
        {
        }

        /// <summary>
        /// Gets a value indicating whether a status line is used.
        /// </summary>
        /// <value>
        /// A value indicating whether a status line is used.
        /// </value>
        bool IZmachine.StatusLineUsed
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the output is buffered to allow word wrapping at the end of each line.
        /// </summary>
        /// <value>
        /// A value indicating whether the output is buffered to allow word wrapping at the end of each line.
        /// </value>
        protected override bool BufferOutput
        {
            get
            {
                return this.bufferOutput && base.BufferOutput;
            }
        }

        /// <summary>
        /// Gets the dictionary word length (in words, not bytes).
        /// </summary>
        /// <value>
        /// The dictionary word length in (in words, not bytes).
        /// </value>
        protected override byte DictionaryWordLength
        {
            get
            {
                return 3;
            }
        }

        /// <summary>
        /// Gets the line height.
        /// </summary>
        /// <value>
        /// The line height.
        /// </value>
        protected virtual byte LineHeight
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets the maximum attribute number.
        /// </summary>
        /// <value>
        /// The maximum attribute number.
        /// </value>
        protected override byte MaximumAttributeNumber
        {
            get
            {
                return 47;
            }
        }

        /// <summary>
        /// Gets the maximum object number.
        /// </summary>
        /// <value>
        /// The maximum object number.
        /// </value>
        protected override ushort MaximumObjectNumber
        {
            get
            {
                return ushort.MaxValue;
            }
        }

        /// <summary>
        /// Gets the maximum property number.
        /// </summary>
        /// <value>
        /// The maximum property number.
        /// </value>
        protected override byte MaximumPropertyNumber
        {
            get
            {
                return 63;
            }
        }

        /// <summary>
        /// Gets the object field length.
        /// </summary>
        /// <value>
        /// The object field length.
        /// </value>
        protected override byte ObjectFieldLength
        {
            get
            {
                return 2;
            }
        }

        /// <summary>
        /// Gets the story length multiplier.
        /// </summary>
        /// <value>
        /// The story length multiplier.
        /// </value>
        protected override int StoryLengthMultiplier
        {
            get
            {
                return 4;
            }
        }

        /// <summary>
        /// Gets the current text styles.
        /// </summary>
        /// <value>
        /// The current text styles.
        /// </value>
        protected override TextStyles TextStyles
        {
            get
            {
                return this.textStyles | base.TextStyles;
            }
        }

        /// <summary>
        /// Determines whether a zscii character is a valid function key code.
        /// </summary>
        /// <param name="zsciiCharacter">
        /// The zscii character.
        /// </param>
        /// <returns>
        /// A value indicating whether the zscii character is a valid function key code.
        /// </returns>
        protected static bool ValidFunctionKeyCode(Zscii zsciiCharacter)
        {
            return zsciiCharacter >= Zscii.CursorUp && zsciiCharacter <= Zscii.NumberPad9;
        }

        /// <summary>
        /// Begins a new input operation.
        /// </summary>
        /// <param name="inputOperation">
        /// The input operation.
        /// </param>
        protected override void BeginInputOperation(InputOperation inputOperation)
        {
            base.BeginInputOperation(inputOperation);
            inputOperation.UpdateDisplayStatus(this.ActiveDisplayWindow, this.FrontEnd.CursorPosition);
            if (inputOperation.Timed)
            {
                // todo: Only set if no parent is timed, otherwise update starting time of operation.
                this.FrontEnd.SetTimeMarker();
            }
        }

        /// <summary>
        /// Calls an interrupt routine.
        /// </summary>
        /// <param name="packedAddress">
        /// The packed address of the interrupt routine.
        /// </param>
        /// <remarks>
        /// The state is changed before the routine call because a call to address zero returns immediately.
        ///   This is important in the case of an input timeout interrupt where the state after returning needs to be ReadingInput, not Running.
        /// </remarks>
        protected void CallInterrupt(ushort packedAddress)
        {
            if (this.State == MachineState.ReadingInput)
            {
                this.InputOperation.UpdateDisplayStatus(this.ActiveDisplayWindow, this.FrontEnd.CursorPosition);
            }

            this.State = MachineState.Running;
            this.CallRoutine(RoutineType.Procedure, packedAddress, null);
        }

        /// <summary>
        /// Discovers the zmachine capabilities.
        /// </summary>
        protected override void DiscoverCapabilities()
        {
            // todo: V4 and up require display sections of the header to be updated whenever the display is resized. (use event?)
            base.DiscoverCapabilities();
            this.Memory.WriteByte(30, (byte)this.FrontEnd.Interpreter);
            this.Memory.WriteByte(31, this.FrontEnd.InterpreterVersion);
            this.Memory.WriteByte(32, this.FrontEnd.DisplayRowCount);
            this.Memory.WriteByte(33, this.FrontEnd.DisplayColumnCount);
        }

        /// <summary>
        /// Finishes an input operation which reads a single character.
        /// </summary>
        /// <param name="terminator">
        /// The terminator.
        /// </param>
        protected void FinishInputCharacterOperation(InputValue terminator)
        {
            this.Store((ushort)this.InputValueToZscii(terminator));
            this.FinishInputOperation(string.Empty, terminator);
        }

        /// <summary>
        /// Finishes an input operation.
        /// </summary>
        /// <param name="inputText">
        /// The input text.
        /// </param>
        /// <param name="terminator">
        /// The terminator.
        /// </param>
        protected override void FinishInputOperation(string inputText, InputValue terminator)
        {
            if (this.InputOperation.Timed)
            {
                // todo: Only release if no parent is also timed.
                this.FrontEnd.ReleaseTimeMarker();
            }

            var parentOperation = this.InputOperation.Parent;
            base.FinishInputOperation(inputText, terminator);
            this.InputOperation = parentOperation;
        }

        /// <summary>
        /// Finishes a restore operation.
        /// </summary>
        /// <param name="restoreSucceeded">
        /// A value which indicates whether a restore succeeded.
        /// </param>
        protected override void FinishRestoreOperation(bool restoreSucceeded)
        {
            const ushort RestoreFailed = 0;
            const ushort RestoreSucceeded = 2;
            this.Store(restoreSucceeded ? RestoreSucceeded : RestoreFailed);
        }

        /// <summary>
        /// Finishes a save operation.
        /// </summary>
        /// <param name="saveSucceeded">
        /// A value which indicates a save succeeded.
        /// </param>
        protected override void FinishSaveOperation(bool saveSucceeded)
        {
            const ushort SaveFailed = 0;
            const ushort SaveSucceeded = 1;
            this.Store(saveSucceeded ? SaveSucceeded : SaveFailed);
        }

        /// <summary>
        /// Finds the data address of a property.
        /// </summary>
        /// <param name="propertyAddress">
        /// The property address.
        /// </param>
        /// <returns>
        /// The data address.
        /// </returns>
        protected override int GetPropertyDataAddress(int propertyAddress)
        {
            return propertyAddress + (this.Memory.ReadFlags(propertyAddress, 128) ? 2 : 1);
        }

        /// <summary>
        /// Converts an input value to a zscii character.
        /// </summary>
        /// <param name="inputValue">
        /// The input value.
        /// </param>
        /// <returns>
        /// The zscii character.
        /// </returns>
        protected virtual Zscii InputValueToZscii(InputValue inputValue)
        {
            if (inputValue.Value is char)
            {
                // todo: // should this write null or question mark for unknown unicode characters?
                return this.UnicodeToZscii((char)inputValue.Value);
            }

            if (inputValue.Value is InputKey)
            {
                return (Zscii)(InputKey)inputValue.Value;
            }

            return Zscii.Null;
        }

        /// <summary>
        /// Operation 136.
        /// </summary>
        /// <remarks>
        /// Infocom name: CALL1
        ///   Inform name: call_1s
        ///   This operation calls a function with no arguments.
        ///   Operands:
        ///   0) Routine packed address.
        /// </remarks>
        protected override void Operation136()
        {
            this.CallRoutineFromOperation(RoutineType.Function);
        }

        /// <summary>
        /// Operation 188.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected override void Operation188()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 228.
        /// </summary>
        /// <remarks>
        /// Infocom name: READ
        ///   Inform name: sread (Version 1-4), aread (Version 5+)
        ///   This operation reads a line of input and optionally provides a routine to be called every time a timeout interval has elapsed.
        ///   Operands:
        ///   0) Text buffer address.
        ///   1) Parse buffer address.
        ///   2) Timeout interval (optional).
        ///   3) Timeout routine (optional).
        /// </remarks>
        protected override void Operation228()
        {
            this.BeginInputOperation(new InputOperation(this.Operands.First, this.Operands.Second, (byte)(this.Memory.ReadByte(this.Operands.First) - 1), this.InputOperation, this.CallStack.CatchValue, this.Operands.Third, this.Operands.Fourth));
        }

        /// <summary>
        /// Operation 234.
        /// </summary>
        /// <remarks>
        /// Infocom name: SPLIT
        ///   Inform name: split_window
        ///   This operation splits the display into upper and lower windows. It does not erase the upper window as in version 3.
        ///   Operands:
        ///   0) Upper window height.
        /// </remarks>
        protected override void Operation234()
        {
            this.SplitWindow(this.Operands.First);
        }

        /// <summary>
        /// Operation 236.
        /// </summary>
        /// <remarks>
        /// Infocom name: XCALL
        ///   Inform name: call_vs2
        ///   This operation calls a function with zero to seven arguments.
        ///   Operands:
        ///   0) Routine packed address.
        ///   1) Routine argument (optional).
        ///   2) Routine argument (optional).
        ///   3) Routine argument (optional).
        ///   4) Routine argument (optional).
        ///   5) Routine argument (optional).
        ///   6) Routine argument (optional).
        ///   7) Routine argument (optional).
        /// </remarks>
        protected override void Operation236()
        {
            this.CallRoutineFromOperation(RoutineType.Function);
        }

        /// <summary>
        /// Operation 237.
        /// </summary>
        /// <remarks>
        /// Infocom name: CLEAR
        ///   Inform name: erase_window
        ///   This operation erases a specified window, or the entire display.
        ///   Operands:
        ///   0) Display window (signed byte). A value of -1 or -2 erases the entire display. Additionally, -1 unsplits the display and makes the lower window active.
        /// </remarks>
        protected override void Operation237()
        {
            // todo: Where did I get sbyte from? Test on Infocom interpreters.
            this.FlushBufferedOutput();
            var windowNumber = (sbyte)this.Operands.First;
            switch (windowNumber)
            {
                case -1:
                    this.UpperWindowHeight = 0;
                    this.ActiveDisplayWindow = DisplayWindow.Lower;
                    this.FrontEnd.CursorPosition = this.DefaultCursorPosition;
                    goto case -2;
                case -2:
                    this.FrontEnd.EraseDisplayArea(this.EntireDisplayArea, this.BackgroundColour);
                    break;
                default:
                    var window = (DisplayWindow)windowNumber;
                    switch (window)
                    {
                        case DisplayWindow.Upper:
                            if (this.ActiveDisplayWindow == DisplayWindow.Upper)
                            {
                                this.FrontEnd.CursorPosition = UpperWindowDefaultCursorPosition;
                            }

                            this.FrontEnd.EraseDisplayArea(this.UpperWindowArea, this.BackgroundColour);
                            break;
                        case DisplayWindow.Lower:
                            if (this.ActiveDisplayWindow == DisplayWindow.Lower)
                            {
                                this.FrontEnd.CursorPosition = this.DefaultCursorPosition;
                            }
                            else
                            {
                                this.InactiveLowerWindowCursorPosition = this.DefaultCursorPosition;
                            }

                            this.FrontEnd.EraseDisplayArea(this.LowerWindowArea, this.BackgroundColour);
                            break;
                        default:
                            this.FrontEnd.ErrorNotification(ErrorCondition.InvalidWindow, "Tried to erase invalid window (" + window + ").");
                            break;
                    }

                    break;
            }
        }

        /// <summary>
        /// Operation 238.
        /// </summary>
        /// <remarks>
        /// Infocom name: ERASE
        ///   Inform name: erase_line
        ///   This operation erases from the cursor position to the end of the line.
        ///   Operands:
        ///   0) A value of 1 erases the line. Any other value does nothing.
        /// </remarks>
        protected override void Operation238()
        {
            if (this.Operands.First == 1)
            {
                this.FlushBufferedOutput();
                var cursorPosition = this.FrontEnd.CursorPosition;
                this.FrontEnd.EraseDisplayArea(new DisplayArea(cursorPosition, new DisplayAreaSize(this.FrontEnd.DisplayColumnCount - cursorPosition.Column, this.LineHeight)), this.BackgroundColour);
            }
        }

        /// <summary>
        /// Operation 239.
        /// </summary>
        /// <remarks>
        /// Infocom name: CURSET
        ///   Inform name: set_cursor
        ///   This operation sets the cursor position if the upper window is selected, otherwise it does nothing.
        ///   Operands:
        ///   0) Display row.
        ///   1) Display column.
        /// </remarks>
        protected override void Operation239()
        {
            if (this.ActiveDisplayWindow == DisplayWindow.Upper)
            {
                // todo: It is illegal to move the cursor outside the upper window except for the case below.
                var row = this.Operands.First;
                var column = this.Operands.Second;
                if (row > this.UpperWindowHeight)
                {
                    // todo: Standard 1.1 states diagnostics should be produced in this case. It is technically an error, although allowed.
                    this.SplitWindow(row);
                }

                this.FrontEnd.CursorPosition = new DisplayPosition(column - 1, row - 1);
            }
        }

        /// <summary>
        /// Operation 240.
        /// </summary>
        /// <remarks>
        /// Infocom name: CURGET
        ///   Inform name: get_cursor
        ///   This operation writes the cursor position to memory.
        ///   Operands:
        ///   0) Address to write cursor position.
        /// </remarks>
        protected override void Operation240()
        {
            // todo: Test if this opcode works on Infocom interpreters. It may be limited to the upper window like CURSET which removes the need to write out any buffered text. The standard doesn't say.
            this.FlushBufferedOutput();
            var address = this.Operands.First;
            var cursorPosition = this.FrontEnd.CursorPosition;
            this.Memory.WriteWord(address, (ushort)(cursorPosition.Row + 1));
            this.Memory.WriteWord(address + 2, (ushort)(cursorPosition.Column + 1));
        }

        /// <summary>
        /// Operation 241.
        /// </summary>
        /// <remarks>
        /// Infocom name: HLIGHT
        ///   Inform name: set_text_style
        ///   This operation sets the text style used for output.
        ///   Operands:
        ///   0) Text style.
        /// </remarks>
        protected override void Operation241()
        {
            var style = (TextStyles)this.Operands.First;
            if (style == TextStyles.None)
            {
                this.textStyles = style;
                return;
            }

            this.textStyles |= style;
        }

        /// <summary>
        /// Operation 242.
        /// </summary>
        /// <remarks>
        /// Infocom name: BUFOUT
        ///   Inform name: buffer_mode
        ///   This operation enables or disables output buffering, writing any buffered text in the latter case.
        ///   Operands:
        ///   0) Non-zero values enable buffering.
        /// </remarks>
        protected override void Operation242()
        {
            this.bufferOutput = this.Operands.First != 0;
            if (!this.bufferOutput)
            {
                this.FrontEnd.WriteBufferedTextToDisplay();
            }
        }

        /// <summary>
        /// Operation 246.
        /// </summary>
        /// <remarks>
        /// Infocom name: INPUT
        ///   Inform name: read_char
        ///   This operation reads a single keypress of input.
        ///   Operands:
        ///   0) Not used (must be 1).
        ///   1) Timeout interval (optional).
        ///   2) Timeout routine (optional).
        /// </remarks>
        protected override void Operation246()
        {
            this.BeginInputOperation(new InputOperation(this.InputOperation, this.CallStack.CatchValue, this.Operands.Second, this.Operands.Third));
        }

        /// <summary>
        /// Operation 247.
        /// </summary>
        /// <remarks>
        /// Infocom name: INTBL?
        ///   Inform name: scan_table
        ///   This operation searches a table in memory for a value. It stores the address where the value is found, or zero if it is not. It then branches if the value is found.
        ///   Operands:
        ///   0) Value to compare.
        ///   1) Address of table.
        ///   2) Entry count.
        ///   3) Entry length in the bottom seven bits and whether to read a byte or a word from each entry in the eighth bit (optional).
        /// </remarks>
        protected override void Operation247()
        {
            var byteForm = false;
            byte entryLength = 2;
            var searchValue = this.Operands.First;
            var searchAddress = this.Operands.Second;
            if (this.Operands.Count > 3)
            {
                byteForm = (this.Operands.Fourth & 128) == 0;
                entryLength = (byte)(this.Operands.Fourth & 127);
            }

            var endOfTable = (ushort)(searchAddress + (this.Operands.Third * entryLength));
            while (searchAddress < endOfTable)
            {
                if (searchValue == (byteForm ? this.Memory.ReadByte(searchAddress) : this.Memory.ReadWord(searchAddress)))
                {
                    this.Store(searchAddress);
                    this.Branch(true);
                    return;
                }

                searchAddress += entryLength;
            }

            this.Store(0);
            this.Branch(false);
        }

        /// <summary>
        /// Operation 25.
        /// </summary>
        /// <remarks>
        /// Infocom name: CALL2
        ///   Inform name: call_2s
        ///   This operation calls a function with one argument. Up to three arguments are allowed in VAR form.
        ///   Operands:
        ///   0) Routine packed address.
        ///   1) Routine argument.
        /// </remarks>
        protected override void Operation25()
        {
            this.CallRoutineFromOperation(RoutineType.Function);
        }

        /// <summary>
        /// Processes a character.
        /// </summary>
        /// <param name="character">
        /// The character.
        /// </param>
        /// <returns>
        /// A value indicating whether the input operation terminated.
        /// </returns>
        protected override bool ProcessCharacter(char character)
        {
            if (this.InputOperation.ReadCharacter)
            {
                if (this.UnicodeToZscii(character) != Zscii.Null)
                {
                    this.FinishInputCharacterOperation(new InputValue(character));
                    return true;
                }

                return false;
            }

            return base.ProcessCharacter(character);
        }

        /// <summary>
        /// Processes an input key.
        /// </summary>
        /// <param name="inputKey">
        /// The input key.
        /// </param>
        /// <returns>
        /// A value indicating whether the input operation terminated.
        /// </returns>
        protected override bool ProcessInputKey(InputKey inputKey)
        {
            if (this.InputOperation.ReadCharacter)
            {
                var zsciiCharacter = (Zscii)inputKey;
                if (zsciiCharacter == Zscii.Delete || zsciiCharacter == Zscii.NewLine || zsciiCharacter == Zscii.Escape || ValidFunctionKeyCode(zsciiCharacter))
                {
                    this.FinishInputCharacterOperation(new InputValue(inputKey));
                    return true;
                }

                return false;
            }

            return base.ProcessInputKey(inputKey);
        }

        /// <summary>
        /// Reads an object property length.
        /// </summary>
        /// <param name="propertyHeader">
        /// The property header.
        /// </param>
        /// <returns>
        /// The property length.
        /// </returns>
        protected override byte PropertyLength(byte propertyHeader)
        {
            if ((propertyHeader & 128) == 128)
            {
                var propertyLength = (byte)(propertyHeader & 63);
                return propertyLength == 0 ? (byte)64 : propertyLength;
            }

            return (byte)((propertyHeader >> 6 & 1) + 1);
        }

        /// <summary>
        /// Reads an object field.
        /// </summary>
        /// <param name="fieldAddress">
        /// The field address.
        /// </param>
        /// <returns>
        /// The field value.
        /// </returns>
        protected override ushort ReadField(int fieldAddress)
        {
            return this.Memory.ReadWord(fieldAddress);
        }

        /// <summary>
        /// Reads input.
        /// </summary>
        protected override void ReadInput()
        {
            if (this.InputOperation.TimedOut)
            {
                this.CallInterrupt(this.InputOperation.TimeoutRoutine);
                return;
            }

            base.ReadInput();
        }

        /// <summary>
        /// Releases external resources used by the zmachine.
        /// </summary>
        protected override void ReleaseExternalResources()
        {
            base.ReleaseExternalResources();
            var inputOperation = this.InputOperation;
            while (inputOperation != null)
            {
                if (inputOperation.Timed)
                {
                    this.FrontEnd.ReleaseTimeMarker();
                }

                inputOperation = inputOperation.Parent;
            }
        }

        /// <summary>
        /// Restarts the zmachine.
        /// </summary>
        protected override void Restart()
        {
            this.bufferOutput = true;
            this.textStyles = TextStyles.None;
            base.Restart();
        }

        /// <summary>
        /// Returns a value from a routine.
        /// </summary>
        /// <param name="returnValue">
        /// The return value.
        /// </param>
        /// <param name="type">
        /// The routine type.
        /// </param>
        protected override void Return(ushort returnValue, RoutineType type)
        {
            base.Return(returnValue, type);
            if (this.InputOperation != null && this.InputOperation.CatchValue == this.CallStack.CatchValue)
            {
                this.State = MachineState.ReadingInput;
                this.FlushBufferedOutput();
                if (this.InputOperation.TimedOut)
                {
                    if (returnValue == 0)
                    {
                        this.InputOperation.ResetTimeout();
                        if (!this.InputOperation.ReadCharacter && (this.ActiveDisplayWindow != this.InputOperation.Window || this.FrontEnd.CursorPosition != this.InputOperation.Cursor))
                        {
                            this.WriteToDisplay(this.InputOperation.InputText);
                        }
                    }
                    else
                    {
                        if (this.InputOperation.ReadCharacter)
                        {
                            this.FinishInputCharacterOperation(new InputValue());
                        }
                        else
                        {
                            // todo: should we re-display input text when returning from an interrupt which terminates the input, before clearing it?
                            this.FinishInputLineOperation(string.Empty, new InputValue());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the header flags.
        /// </summary>
        protected override void SetHeaderFlags()
        {
            var supportedTextStyles = this.FrontEnd.SupportedTextStyles;
            this.WriteHeaderFlag(Flags1Address, HeaderFlags.Flags1BoldStyleAvailable, (supportedTextStyles & TextStyles.Bold) == TextStyles.Bold);
            this.WriteHeaderFlag(Flags1Address, HeaderFlags.Flags1ItalicStyleAvailable, (supportedTextStyles & TextStyles.Italic) == TextStyles.Italic);
            this.WriteHeaderFlag(Flags1Address, HeaderFlags.Flags1FixedStyleAvailable, (supportedTextStyles & TextStyles.Fixed) == TextStyles.Fixed);
            this.WriteHeaderFlag(Flags1Address, HeaderFlags.Flags1TimedInputAvailable, this.FrontEnd.TimedInputAvailable);
            this.SetHeaderFlagsForSound();
        }

        /// <summary>
        /// Sets the header flags for sound.
        /// </summary>
        /// <remarks>
        /// This new flag was first used by Infocom in version 5 as no version 4 games to date have used sound effects. It is introduced here to allow for the possibility.
        /// </remarks>
        protected override void SetHeaderFlagsForSound()
        {
            this.WriteHeaderFlag(Memory.Flags2LowByteAddress, HeaderFlags.Flags2LowSoundsAvailableNew, this.FrontEnd.SoundsAvailable);
        }

        /// <summary>
        /// Unpacks a routine address.
        /// </summary>
        /// <param name="packedAddress">
        /// Packed address.
        /// </param>
        /// <returns>
        /// Unpacked address.
        /// </returns>
        protected override int UnpackRoutineAddress(ushort packedAddress)
        {
            return packedAddress * 4;
        }

        /// <summary>
        /// Unpacks a string address.
        /// </summary>
        /// <param name="packedAddress">
        /// Packed address.
        /// </param>
        /// <returns>
        /// Unpacked address.
        /// </returns>
        protected override int UnpackStringAddress(ushort packedAddress)
        {
            return packedAddress * 4;
        }

        /// <summary>
        /// Updates input.
        /// </summary>
        protected override void UpdateInput()
        {
            if (this.InputOperation.Timed)
            {
                var timedInput = this.FrontEnd.GetTimedInput();
                this.InputOperation.ElapsedTime = timedInput.ElapsedTime;
                this.InputOperation.UpdateInput(timedInput.InputValues);
            }
            else
            {
                base.UpdateInput();
            }
        }

        /// <summary>
        /// Writes to an object field.
        /// </summary>
        /// <param name="fieldAddress">
        /// The field address.
        /// </param>
        /// <param name="fieldValue">
        /// The field value.
        /// </param>
        protected override void WriteField(int fieldAddress, ushort fieldValue)
        {
            this.Memory.WriteWord(fieldAddress, fieldValue);
        }

        /// <summary>
        /// Writes input values to the input log.
        /// </summary>
        /// <param name="inputValues">
        /// The input values.
        /// </param>
        protected override void WriteToInputLog(ImmutableQueue<InputValue> inputValues)
        {
            var inputOperation = this.InputOperation;
            if (inputOperation.Timed)
            {
                this.FrontEnd.WriteToInputLog(inputValues, inputOperation.ElapsedTime);
            }
            else
            {
                base.WriteToInputLog(inputValues);
            }
        }
    }
}