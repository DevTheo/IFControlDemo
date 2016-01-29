// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZmachineV1.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   The zmachine version 1.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// The zmachine version 1.
    /// </summary>
    /// <remarks>
    /// Implements the zmachine in its earliest form.
    /// </remarks>
    internal class ZmachineV1 : IZmachine
    {
        /// <summary>
        /// Unicode sentence space.
        /// </summary>
        protected const char UnicodeSentenceSpace = '\u2002';

        /// <summary>
        /// The maximum number of local variables in a routine.
        /// </summary>
        private const byte MaximumLocalVariables = 15;

        /// <summary>
        /// The operations.
        /// </summary>
        private static readonly ImmutableArray<Operation> operations = InitializeOperations();

        /// <summary>
        /// The unicode characters.
        /// </summary>
        private static readonly ImmutableArray<char> unicodeCharacters = new ImmutableArray<char>(new[] { 'ä', 'ö', 'ü', 'Ä', 'Ö', 'Ü', 'ß', '»', '«', 'ë', 'ï', 'ÿ', 'Ë', 'Ï', 'á', 'é', 'í', 'ó', 'ú', 'ý', 'Á', 'É', 'Í', 'Ó', 'Ú', 'Ý', 'à', 'è', 'ì', 'ò', 'ù', 'À', 'È', 'Ì', 'Ò', 'Ù', 'â', 'ê', 'î', 'ô', 'û', 'Â', 'Ê', 'Î', 'Ô', 'Û', 'å', 'Å', 'ø', 'Ø', 'ã', 'ñ', 'õ', 'Ã', 'Ñ', 'Õ', 'æ', 'Æ', 'ç', 'Ç', 'þ', 'ð', 'Þ', 'Ð', '£', 'œ', 'Œ', '¡', '¿' });

        /// <summary>
        /// The zscii alphabet characters.
        /// </summary>
        private static readonly ImmutableArray<Zscii> zsciiAlphabetCharacters = new ImmutableArray<Zscii>(new[] { Zscii.LowercaseA, Zscii.LowercaseB, Zscii.LowercaseC, Zscii.LowercaseD, Zscii.LowercaseE, Zscii.LowercaseF, Zscii.LowercaseG, Zscii.LowercaseH, Zscii.LowercaseI, Zscii.LowercaseJ, Zscii.LowercaseK, Zscii.LowercaseL, Zscii.LowercaseM, Zscii.LowercaseN, Zscii.LowercaseO, Zscii.LowercaseP, Zscii.LowercaseQ, Zscii.LowercaseR, Zscii.LowercaseS, Zscii.LowercaseT, Zscii.LowercaseU, Zscii.LowercaseV, Zscii.LowercaseW, Zscii.LowercaseX, Zscii.LowercaseY, Zscii.LowercaseZ, Zscii.UppercaseA, Zscii.UppercaseB, Zscii.UppercaseC, Zscii.UppercaseD, Zscii.UppercaseE, Zscii.UppercaseF, Zscii.UppercaseG, Zscii.UppercaseH, Zscii.UppercaseI, Zscii.UppercaseJ, Zscii.UppercaseK, Zscii.UppercaseL, Zscii.UppercaseM, Zscii.UppercaseN, Zscii.UppercaseO, Zscii.UppercaseP, Zscii.UppercaseQ, Zscii.UppercaseR, Zscii.UppercaseS, Zscii.UppercaseT, Zscii.UppercaseU, Zscii.UppercaseV, Zscii.UppercaseW, Zscii.UppercaseX, Zscii.UppercaseY, Zscii.UppercaseZ, Zscii.Null, Zscii.Number0, Zscii.Number1, Zscii.Number2, Zscii.Number3, Zscii.Number4, Zscii.Number5, Zscii.Number6, Zscii.Number7, Zscii.Number8, Zscii.Number9, Zscii.Period, Zscii.Comma, Zscii.ExclamationPoint, Zscii.QuestionMark, Zscii.Underscore, Zscii.NumberSign, Zscii.SingleQuote, Zscii.DoubleQuote, Zscii.ForwardSlash, Zscii.BackSlash, Zscii.LessThan, Zscii.Hyphen, Zscii.Colon, Zscii.LeftParenthesis, Zscii.RightParenthesis });

        /// <summary>
        /// The zmachine call stack.
        /// </summary>
        private readonly CallStack callStack;

        /// <summary>
        /// The zmachine front end.
        /// </summary>
        private readonly FrontEnd frontEnd;

        /// <summary>
        /// The zmachine memory.
        /// </summary>
        private readonly Memory memory;

        /// <summary>
        /// The operands for the current operation.
        /// </summary>
        private readonly Operands operands = new Operands();

        /// <summary>
        /// The zmachine random number generator.
        /// </summary>
        private readonly RandomNumberGenerator randomNumberGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZmachineV1"/> class.
        /// </summary>
        /// <param name="frontEnd">
        /// The zmachine front end.
        /// </param>
        /// <param name="story">
        /// The story.
        /// </param>
        /// <param name="random">
        /// The random number generator.
        /// </param>
        internal ZmachineV1(FrontEnd frontEnd, ImmutableArray<byte> story, IRandomNumberGenerator random)
        {
            this.frontEnd = frontEnd;
            this.memory = new Memory(story, this.FrontEnd);
            this.randomNumberGenerator = new RandomNumberGenerator(random);
            this.callStack = new CallStack(this.FrontEnd);
        }

        /// <summary>
        /// A zmachine operation.
        /// </summary>
        /// <param name="machine">
        /// The zmachine to run the operation.
        /// </param>
        private delegate void Operation(ZmachineV1 machine);

        /// <summary>
        /// Gets or sets a value indicating whether the input log is open.
        /// </summary>
        /// <value>
        /// A value indicating whether the input log is open.
        /// </value>
        bool IZmachine.InputLogOpen
        {
            get
            {
                lock (this.Memory)
                {
                    return this.InputLogOpen;
                }
            }

            set
            {
                lock (this.Memory)
                {
                    if (this.InputLogOpen != value)
                    {
                        this.FrontEnd.ControlOutputStream(value, OutputStream.InputLog);
                    }

                    this.InputLogOpen = value;
                }
            }
        }

        /// <summary>
        /// Gets the interactive fiction identifier.
        /// </summary>
        /// <value>
        /// The interactive fiction identifier.
        /// </value>
        /// <remarks>
        /// This returns an interactive fiction identifier if one is found in the zmachine's memory, otherwise one is generated.
        /// </remarks>
        string IZmachine.InteractiveFictionIdentifier
        {
            get
            {
                lock (this.Memory)
                {
                    return this.FindEmbeddedIfid() ?? this.GenerateIfid();
                }
            }
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
                return true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the transcript is open.
        /// </summary>
        /// <value>
        /// A value indicating whether the transcript is open.
        /// </value>
        bool IZmachine.TranscriptOpen
        {
            get
            {
                lock (this.Memory)
                {
                    return this.TranscriptOpen;
                }
            }

            set
            {
                lock (this.Memory)
                {
                    this.TranscriptOpen = value;
                }
            }
        }

        /// <summary>
        /// Gets the current background colour.
        /// </summary>
        /// <value>
        /// The current background colour.
        /// </value>
        protected virtual ColorStruct? BackgroundColour
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the output is buffered to allow word wrapping at the end of each line.
        /// </summary>
        /// <value>
        /// A value indicating whether the output is buffered to allow word wrapping at the end of each line.
        /// </value>
        protected virtual bool BufferOutput
        {
            get
            {
                return this.State == MachineState.Running;
            }
        }

        /// <summary>
        /// Gets the zmachine call stack.
        /// </summary>
        /// <value>
        /// The zmachine call stack.
        /// </value>
        protected CallStack CallStack
        {
            get
            {
                return this.callStack;
            }
        }

        /// <summary>
        /// Gets the checksum.
        /// </summary>
        /// <value>
        /// The checksum.
        /// </value>
        protected ushort Checksum
        {
            get
            {
                return this.Memory.ReadStoryWord(28);
            }
        }

        /// <summary>
        /// Gets the default cursor position.
        /// </summary>
        /// <value>
        /// The default cursor position.
        /// </value>
        protected virtual DisplayPosition DefaultCursorPosition
        {
            get
            {
                return new DisplayPosition(0, this.FrontEnd.DisplayRowCount - 1);
            }
        }

        /// <summary>
        /// Gets the dictionary table address.
        /// </summary>
        /// <value>
        /// The dictionary table address.
        /// </value>
        protected int DictionaryTableAddress
        {
            get
            {
                return this.Memory.ReadWord(8);
            }
        }

        /// <summary>
        /// Gets the dictionary word length (in words, not bytes).
        /// </summary>
        /// <value>
        /// The dictionary word length (in words, not bytes).
        /// </value>
        protected virtual byte DictionaryWordLength
        {
            get
            {
                return 2;
            }
        }

        /// <summary>
        /// Gets the entire display area.
        /// </summary>
        /// <value>
        /// The entire display area.
        /// </value>
        protected DisplayArea EntireDisplayArea
        {
            get
            {
                return new DisplayArea(new DisplayPosition(0, 0), new DisplayAreaSize(this.FrontEnd.DisplayColumnCount, this.FrontEnd.DisplayRowCount));
            }
        }

        /// <summary>
        /// Gets the display font.
        /// </summary>
        /// <value>
        /// The display font.
        /// </value>
        protected virtual Font Font
        {
            get
            {
                return Font.Normal;
            }
        }

        /// <summary>
        /// Gets the current foreground colour.
        /// </summary>
        /// <value>
        /// The current foreground colour.
        /// </value>
        protected virtual ColorStruct? ForegroundColour
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the zmachine front end.
        /// </summary>
        /// <value>
        /// The zmachine front end.
        /// </value>
        protected FrontEnd FrontEnd
        {
            get
            {
                return this.frontEnd;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the input log is open.
        /// </summary>
        /// <value>
        /// A value indicating whether the input log is open.
        /// </value>
        protected bool InputLogOpen
        {
            private get;
            set;
        }

        /// <summary>
        /// Gets or sets the current input operation.
        /// </summary>
        /// <value>
        /// The current input operation.
        /// </value>
        protected InputOperation InputOperation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the maximum attribute number.
        /// </summary>
        /// <value>
        /// The maximum attribute number.
        /// </value>
        protected virtual byte MaximumAttributeNumber
        {
            get
            {
                return 31;
            }
        }

        /// <summary>
        /// Gets the maximum object number.
        /// </summary>
        /// <value>
        /// The maximum object number.
        /// </value>
        protected virtual ushort MaximumObjectNumber
        {
            get
            {
                return byte.MaxValue;
            }
        }

        /// <summary>
        /// Gets the maximum property number.
        /// </summary>
        /// <value>
        /// The maximum property number.
        /// </value>
        protected virtual byte MaximumPropertyNumber
        {
            get
            {
                return 31;
            }
        }

        /// <summary>
        /// Gets the zmachine memory.
        /// </summary>
        /// <value>
        /// The zmachine memory.
        /// </value>
        protected Memory Memory
        {
            get
            {
                return this.memory;
            }
        }

        /// <summary>
        /// Gets the object field length.
        /// </summary>
        /// <value>
        /// The object field length.
        /// </value>
        protected virtual byte ObjectFieldLength
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets the operands for the current operation.
        /// </summary>
        /// <value>
        /// The operands for the current operation.
        /// </value>
        protected Operands Operands
        {
            get
            {
                return this.operands;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the display is scrollable.
        /// </summary>
        /// <value>
        /// A value indicating whether the display is scrollable.
        /// </value>
        protected virtual bool Scrollable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the zmachine state.
        /// </summary>
        /// <value>
        /// The zmachine state.
        /// </value>
        protected MachineState State
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the story length.
        /// </summary>
        /// <value>
        /// The story length.
        /// </value>
        protected int StoryLength
        {
            get
            {
                var storyLengthFromHeader = this.StoryLengthMultiplier * this.Memory.ReadStoryWord(26);
                return storyLengthFromHeader > 0 ? storyLengthFromHeader : this.Memory.StoryLength;
            }
        }

        /// <summary>
        /// Gets the story length multiplier.
        /// </summary>
        /// <value>
        /// The story length multiplier.
        /// </value>
        protected virtual int StoryLengthMultiplier
        {
            get
            {
                return 2;
            }
        }

        /// <summary>
        /// Gets the text buffer header length.
        /// </summary>
        /// <value>
        /// The text buffer header length.
        /// </value>
        protected virtual byte TextBufferHeaderLength
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets the current text styles.
        /// </summary>
        /// <value>
        /// The current text styles.
        /// </value>
        protected virtual TextStyles TextStyles
        {
            get
            {
                return TextStyles.None;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the transcript is open.
        /// </summary>
        /// <value>
        /// A value indicating whether the transcript is open.
        /// </value>
        protected bool TranscriptOpen
        {
            get
            {
                return this.ReadHeaderFlag(Memory.Flags2LowByteAddress, HeaderFlags.Flags2LowTranscriptOpen);
            }

            set
            {
                this.WriteHeaderFlag(Memory.Flags2LowByteAddress, HeaderFlags.Flags2LowTranscriptOpen, value);
            }
        }

        /// <summary>
        /// Gets the unicode character count.
        /// </summary>
        /// <value>
        /// The unicode character count.
        /// </value>
        protected virtual byte UnicodeCharacterCount
        {
            get
            {
                return 69;
            }
        }

        /// <summary>
        /// Gets the writable area of the display.
        /// </summary>
        /// <value>
        /// The writable area of the display.
        /// </value>
        protected virtual DisplayArea WritableArea
        {
            get
            {
                return this.EntireDisplayArea;
            }
        }

        /// <summary>
        /// Gets the attribute field length.
        /// </summary>
        /// <value>
        /// The attribute field length.
        /// </value>
        private byte AttributeFieldlength
        {
            get
            {
                return (byte)((this.MaximumAttributeNumber + 1) / 8);
            }
        }

        /// <summary>
        /// Gets the objects table address.
        /// </summary>
        /// <value>
        /// The objects table address.
        /// </value>
        private int ObjectsTableAddress
        {
            get
            {
                return this.Memory.ReadWord(10);
            }
        }

        /// <summary>
        /// Gets the release number.
        /// </summary>
        /// <value>
        /// The release number.
        /// </value>
        private ushort ReleaseNumber
        {
            get
            {
                return this.Memory.ReadStoryWord(2);
            }
        }

        /// <summary>
        /// Gets the serial code.
        /// </summary>
        /// <value>
        /// The serial code.
        /// </value>
        private string SerialCode
        {
            get
            {
                const byte SerialAddress = 18;
                const byte SerialLength = 6;
                var serial = new char[SerialLength];
                for (byte serialCharacterNumber = 0; serialCharacterNumber < SerialLength; serialCharacterNumber++)
                {
                    var serialCharacter = (Zscii)this.Memory.ReadStoryByte(SerialAddress + serialCharacterNumber);
                    serial[serialCharacterNumber] = (char)(IsStandardZscii(serialCharacter) ? serialCharacter : Zscii.Hyphen);
                }

                return new string(serial);
            }
        }

        /// <summary>
        /// Indicates the display should be redrawn.
        /// </summary>
        void IZmachine.RedrawDisplay()
        {
            // Do nothing.
        }

        /// <summary>
        /// Restarts the zmachine.
        /// </summary>
        void IZmachine.Restart()
        {
            lock (this.Memory)
            {
                this.Restart();
            }
        }

        /// <summary>
        /// Restores a saved state.
        /// </summary>
        /// <param name="saveState">
        /// The saved state to restore.
        /// </param>
        /// <returns>
        /// A value indicating whether the restore succeeded.
        /// </returns>
        bool IZmachine.Restore(ZmachineSaveState saveState)
        {
            lock (this.Memory)
            {
                if (this.RestoreState(saveState))
                {
                    this.FinishRestoreOperation(true);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Runs one cycle of the zmachine.
        /// </summary>
        async Task IZmachine.Run()
        {
            lock (this.Memory)
            {
                switch (this.State)
                {
                    case MachineState.Initializing:
                        this.Initialize();
                        break;
                    case MachineState.Running:
                        this.ExecuteInstruction();
                        break;
                    case MachineState.ReadingInput:
                        this.ReadInput();
                        break;
                }
            }
        }

        /// <summary>
        /// To be called by the frontend when a sound has finished playing.
        /// </summary>
        /// <param name="sound">
        /// The sound.
        /// </param>
        void IZmachine.SoundFinished(int sound)
        {
            // Do nothing.
        }

        /// <summary>
        /// Adds an alphabet shift to z-characters.
        /// </summary>
        /// <param name="neededAlphabet">
        /// The needed alphabet.
        /// </param>
        /// <param name="zsciiText">
        /// The zscii text following the alphabet shift.
        /// </param>
        /// <param name="lockedAlphabet">
        /// The locked alphabet.
        /// </param>
        /// <param name="characters">
        /// The z-characters.
        /// </param>
        protected virtual void AddAlphabetShiftToZCharacters(byte neededAlphabet, ImmutableStack<Zscii> zsciiText, ref byte lockedAlphabet, ref ImmutableStack<byte> characters)
        {
            // bug: if next character is newline, space, etc...
            var shiftCharacter = (byte)(((3 + neededAlphabet - lockedAlphabet) % 3) + 1);
            if (zsciiText != null)
            {
                var nextIndex = this.GetZsciiAlphabetIndex(zsciiText.Top);
                var nextNeededAlphabet = (byte)((nextIndex == -1) ? 2 : nextIndex / 26);
                if (nextNeededAlphabet == neededAlphabet)
                {
                    shiftCharacter += 2;
                    lockedAlphabet = neededAlphabet;
                }
            }

            characters = characters.Add(shiftCharacter);
        }

        /// <summary>
        /// Begins a new input operation.
        /// </summary>
        /// <param name="inputOperation">
        /// The input operation.
        /// </param>
        protected virtual void BeginInputOperation(InputOperation inputOperation)
        {
            this.State = MachineState.ReadingInput;
            this.InputOperation = inputOperation;
            this.FlushBufferedOutput();
            this.FrontEnd.ResetMorePromptCounts();
        }

        /// <summary>
        /// Determines whether an operation branches.
        /// </summary>
        /// <param name="condition">
        /// Branch condition.
        /// </param>
        protected void Branch(bool condition)
        {
            var branchData = this.Memory.ReadByte(this.CallStack.ProgramCounter++);
            var offset = (ushort)(branchData & 63);
            if ((branchData & 64) != 64)
            {
                offset = (ushort)((offset << 8) + this.Memory.ReadByte(this.CallStack.ProgramCounter++));
            }

            if (condition ^ ((branchData & 128) != 128))
            {
                if (offset > 1)
                {
                    this.Jump((short)(offset > 8191 ? offset - 16384 : offset));
                }
                else
                {
                    this.Return(offset, this.CallStack.EndRoutine());
                }
            }
        }

        /// <summary>
        /// Calls a routine.
        /// </summary>
        /// <param name="routineType">
        /// The routine type.
        /// </param>
        /// <param name="packedAddress">
        /// The packed address of the routine.
        /// </param>
        /// <param name="arguments">
        /// Routine arguments.
        /// </param>
        protected void CallRoutine(RoutineType routineType, ushort packedAddress, ImmutableStack<ushort> arguments)
        {
            if (packedAddress == 0)
            {
                this.Return(0, routineType);
                return;
            }

            var argumentCount = (byte)arguments.Count();
            var programCounter = this.UnpackRoutineAddress(packedAddress);
            var localVariableCount = this.Memory.ReadByte(programCounter);
            if (localVariableCount > MaximumLocalVariables)
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidRoutine, "Called a routine at address " + programCounter + " with " + localVariableCount + " local variables.");
            }

            this.CallStack.BeginRoutine(routineType, programCounter + 1, argumentCount, localVariableCount);
            this.InitializeLocalVariables(localVariableCount);
            byte localVariableNumber = 0;
            foreach (var argument in arguments.Enumerable())
            {
                if (localVariableNumber >= localVariableCount)
                {
                    break;
                }

                this.CallStack.WriteLocalVariable(localVariableNumber++, argument);
            }
        }

        /// <summary>
        /// Calls a routine with the address and arguments supplied by operands.
        /// </summary>
        /// <param name="type">
        /// The routine type to call.
        /// </param>
        protected void CallRoutineFromOperation(RoutineType type)
        {
            if (this.Operands.Count == 0)
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidRoutine, "Tried to a call a routine but no address was given.");
            }

            ImmutableStack<ushort> arguments = null;
            var operandNumber = this.Operands.Count;
            while (operandNumber > 1)
            {
                arguments = arguments.Add(this.Operands[--operandNumber]);
            }

            this.CallRoutine(type, this.Operands.First, arguments);
        }

        /// <summary>
        /// Discovers the zmachine capabilities.
        /// </summary>
        protected virtual void DiscoverCapabilities()
        {
            this.Memory.WriteByte(50, this.FrontEnd.StandardRevisionMajor);
            this.Memory.WriteByte(51, this.FrontEnd.StandardRevisionMinor);
        }

        /// <summary>
        /// Encodes zscii text for comparison with a dictionary entry.
        /// </summary>
        /// <param name="zsciiText">
        /// The zscii text.
        /// </param>
        /// <returns>
        /// The encoded z-characters.
        /// </returns>
        protected ImmutableStack<ushort> EncodeWord(ImmutableStack<Zscii> zsciiText)
        {
            var characters = this.ZsciiToZCharacters(zsciiText);
            ImmutableStack<ushort> encodedText = null;
            var wordLength = this.DictionaryWordLength;
            while (encodedText.Count() < wordLength)
            {
                var encodedCharacter = (ushort)(encodedText.Count() + 1 == wordLength ? 32768 : 0);
                for (var characterOffset = 2; characterOffset > -1; characterOffset--)
                {
                    byte character = 5;
                    if (characters != null)
                    {
                        character = characters.Top;
                        characters = characters.Tail;
                    }

                    encodedCharacter += (ushort)(character << (characterOffset * 5));
                }

                encodedText = encodedText.Add(encodedCharacter);
            }

            return encodedText.Reverse();
        }

        /// <summary>
        /// Decodes encoded text into z-characters.
        /// </summary>
        /// <param name="address">
        /// The address of the encoded text. After returning, the address points past the end of the encoded text.
        /// </param>
        /// <returns>
        /// The z-characters.
        /// </returns>
        protected ImmutableStack<byte> EncodedTextToZCharacters(ref int address)
        {
            ushort character;
            ImmutableStack<byte> characters = null;
            var storyLength = this.StoryLength;
            do
            {
                if (address >= storyLength)
                {
                    this.FrontEnd.ErrorNotification(ErrorCondition.InvalidAddress, "Encoded text does not end before the end of memory.");
                    break;
                }

                character = this.Memory.ReadWord(address);
                characters = characters.Add((byte)(character >> 10 & 31));
                characters = characters.Add((byte)(character >> 5 & 31));
                characters = characters.Add((byte)(character & 31));
                address += 2;
            }
            while ((character & 32768) == 0);
            return characters.Reverse();
        }

        /// <summary>
        /// Finishes an input operation which reads a line of text.
        /// </summary>
        /// <param name="inputText">
        /// The input text.
        /// </param>
        /// <param name="terminator">
        /// The terminator.
        /// </param>
        protected virtual void FinishInputLineOperation(string inputText, InputValue terminator)
        {
            var zsciiText = this.UnicodeToZscii(inputText);
            var textStartAddress = this.InputOperation.TextBuffer + this.TextBufferHeaderLength;
            this.WriteZsciiToMemory(textStartAddress, zsciiText);
            this.TerminateTextBuffer(textStartAddress, (byte)zsciiText.Count());
            this.LexicalAnalysis(zsciiText, this.InputOperation.ParseBuffer, this.DictionaryTableAddress, true);
            this.FinishInputOperation(inputText, terminator);
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
        protected virtual void FinishInputOperation(string inputText, InputValue terminator)
        {
            if (this.InputLogOpen)
            {
                ImmutableQueue<InputValue> inputValues = null;
                foreach (var character in inputText)
                {
                    inputValues = inputValues.Add(new InputValue(character));
                }

                inputValues = inputValues.Add(terminator);
                this.WriteToInputLog(inputValues);
            }

            this.InputOperation = null;
            this.State = MachineState.Running;
        }

        /// <summary>
        /// Finishes a restore operation.
        /// </summary>
        /// <param name="restoreSucceeded">
        /// A value which indicates whether a restore succeeded.
        /// </param>
        protected virtual void FinishRestoreOperation(bool restoreSucceeded)
        {
            this.Branch(restoreSucceeded);
        }

        /// <summary>
        /// Finishes a save operation.
        /// </summary>
        /// <param name="saveSucceeded">
        /// A value which indicates a save succeeded.
        /// </param>
        protected virtual void FinishSaveOperation(bool saveSucceeded)
        {
            this.Branch(saveSucceeded);
        }

        /// <summary>
        /// Flushes any buffered output.
        /// </summary>
        protected void FlushBufferedOutput()
        {
            if (this.BufferOutput)
            {
                this.FrontEnd.WriteBufferedTextToDisplay();
            }
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
        protected virtual int GetPropertyDataAddress(int propertyAddress)
        {
            return propertyAddress + 1;
        }

        /// <summary>
        /// Gets a unicode character by index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The unicode character.
        /// </returns>
        protected virtual char GetUnicodeCharacter(int index)
        {
            return unicodeCharacters[index];
        }

        /// <summary>
        /// Gets the zscii alphabet character indicated by the given index.
        /// </summary>
        /// <param name="index">
        /// The character index.
        /// </param>
        /// <returns>
        /// The zscii alphabet character.
        /// </returns>
        protected virtual Zscii GetZsciiAlphabetCharacter(byte index)
        {
            return zsciiAlphabetCharacters[index];
        }

        /// <summary>
        /// Initializes the call stack.
        /// </summary>
        /// <param name="initialProgramCounter">
        /// The initial program counter.
        /// </param>
        protected virtual void InitializeCallStack(ushort initialProgramCounter)
        {
            this.callStack.Initialize(initialProgramCounter, 0);
        }

        /// <summary>
        /// Initializes local variables at the start of a routine.
        /// </summary>
        /// <param name="localVariableCount">
        /// The local variable count.
        /// </param>
        protected virtual void InitializeLocalVariables(byte localVariableCount)
        {
            for (byte variableNumber = 0; variableNumber < localVariableCount; variableNumber++, this.CallStack.ProgramCounter += 2)
            {
                this.CallStack.WriteLocalVariable(variableNumber, this.Memory.ReadWord(this.CallStack.ProgramCounter));
            }
        }

        /// <summary>
        /// An invalid operation.
        /// </summary>
        protected void InvalidOperation()
        {
            this.FrontEnd.ErrorNotification(ErrorCondition.InvalidOperationCode, "Invalid operation code.");
        }

        /// <summary>
        /// Performs lexical analysis on zscii text and writes the results to the parse buffer.
        /// </summary>
        /// <param name="zsciiText">
        /// The zscii text.
        /// </param>
        /// <param name="parseBuffer">
        /// The parse buffer address.
        /// </param>
        /// <param name="dictionary">
        /// The dictionary address.
        /// </param>
        /// <param name="parseUnknownWords">
        /// A value indicating whether to parse unknown words.
        /// </param>
        protected void LexicalAnalysis(ImmutableStack<Zscii> zsciiText, int parseBuffer, int dictionary, bool parseUnknownWords)
        {
            if (parseBuffer == 0)
            {
                return;
            }

            byte wordCount = 0;
            var wordOffset = this.TextBufferHeaderLength;
            var maxWords = this.Memory.ReadByte(parseBuffer);
            var wordSeparators = this.ReadZsciiFromMemory(dictionary).Add(Zscii.Space);
            while (zsciiText != null)
            {
                if (wordCount >= maxWords)
                {
                    this.FrontEnd.ErrorNotification(ErrorCondition.ParseBufferOverflow, "Too many words typed, discarding: " + this.ZsciiToUnicode(zsciiText));
                    break;
                }

                var word = GetNextWord(ref zsciiText, wordSeparators);
                var wordlength = (byte)word.Count();
                if (word.Top != Zscii.Space)
                {
                    var dictionaryEntryAddress = this.DictionarySearch(dictionary, this.EncodeWord(word));
                    if (dictionaryEntryAddress != 0 || parseUnknownWords)
                    {
                        var parseBufferEntry = parseBuffer + 2 + (wordCount * 4);
                        this.Memory.WriteWord(parseBufferEntry, (ushort)dictionaryEntryAddress);
                        this.Memory.WriteByte(parseBufferEntry + 2, wordlength);
                        this.Memory.WriteByte(parseBufferEntry + 3, wordOffset);
                    }

                    wordCount++;
                }

                wordOffset += wordlength;
            }

            this.Memory.WriteByte(parseBuffer + 1, wordCount);
        }

        /// <summary>
        /// Loads operands from a byte containing the operand types.
        /// </summary>
        /// <param name="operandTypes">
        /// The operands.
        /// </param>
        protected void LoadOperands(byte operandTypes)
        {
            this.LoadOperand((OperandType)(operandTypes >> 6 & 3));
            this.LoadOperand((OperandType)(operandTypes >> 4 & 3));
            this.LoadOperand((OperandType)(operandTypes >> 2 & 3));
            this.LoadOperand((OperandType)(operandTypes & 3));
        }

        /// <summary>
        /// Operation 0.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation0()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 1.
        /// </summary>
        /// <remarks>
        /// Infocom name: EQUAL?
        ///   Inform name: je
        ///   This operation branches if the first value is equal to any subsequent ones.
        ///   Standard 1.0 says do not branch if there is only one operand.
        ///   Standard 1.1 says it is illegal to have only one operand.
        ///   Infocom interpreters vary in their behavior if there are zero or one operands.
        ///   This implementation follows both standards; it issues an error message and then does not branch in the case of too few operands.
        ///   Operands:
        ///   0) Value to compare.
        ///   1) Value to compare.
        ///   2) Value to compare (optional).
        ///   3) Value to compare (optional).
        /// </remarks>
        protected virtual void Operation1()
        {
            var operandCount = this.Operands.Count;
            for (byte operandNumber = 1; operandNumber < operandCount; operandNumber++)
            {
                if (this.Operands.First == this.Operands[operandNumber])
                {
                    this.Branch(true);
                    return;
                }
            }

            if (operandCount < 2)
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidOperandCount, "EQUAL? Too few operands.");
            }

            this.Branch(false);
        }

        /// <summary>
        /// Operation 10.
        /// </summary>
        /// <remarks>
        /// Infocom name: FSET?
        ///   Inform name: test_attr
        ///   This operation branches if the given object has the given attribute set.
        ///   Operands:
        ///   0) Object number.
        ///   1) Attribute Number.
        /// </remarks>
        protected virtual void Operation10()
        {
            var objectNumber = this.Operands.First;
            var attributeNumber = this.Operands.Second;
            if (this.Operands.Count < 2)
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidOperandCount, "FSET? Too few operands.");
                this.Branch(false);
                return;
            }

            if (this.InvalidObject(objectNumber))
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidObject, "Tried to read attribute " + attributeNumber + " on invalid object " + objectNumber + ".");
                this.Branch(false);
                return;
            }

            if (this.InvalidAttribute(attributeNumber))
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidAttribute, "Tried to read invalid attribute " + attributeNumber + " on object " + objectNumber + ".");
                this.Branch(false);
                return;
            }

            this.Branch(this.Memory.ReadFlags(this.GetAttributeAddress(objectNumber, attributeNumber), GetAttributeBitMask(attributeNumber)));
        }

        /// <summary>
        /// Operation 11.
        /// </summary>
        /// <remarks>
        /// Infocom name: FSET
        ///   Inform name: set_attr
        ///   This operation sets an attribute on an object.
        ///   Operands:
        ///   0) Object number.
        ///   1) Attribute number.
        /// </remarks>
        protected virtual void Operation11()
        {
            this.WriteAttribute(this.Operands.First, this.Operands.Second, true);
        }

        /// <summary>
        /// Operation 12.
        /// </summary>
        /// <remarks>
        /// Infocom name: FCLEAR
        ///   Inform name: clear_attr
        ///   This operation clears an attribute on an object.
        ///   Operands:
        ///   0) Object number.
        ///   1) Attribute number.
        /// </remarks>
        protected virtual void Operation12()
        {
            this.WriteAttribute(this.Operands.First, this.Operands.Second, false);
        }

        /// <summary>
        /// Operation 128.
        /// </summary>
        /// <remarks>
        /// Infocom name: ZERO?
        ///   Inform name: jz
        ///   This operation branches if a value is zero.
        ///   Operands:
        ///   0) Value to compare.
        /// </remarks>
        protected virtual void Operation128()
        {
            this.Branch(this.Operands.First == 0);
        }

        /// <summary>
        /// Operation 129.
        /// </summary>
        /// <remarks>
        /// Infocom name: NEXT?
        ///   Inform name: get_sibling
        ///   This operation stores the sibling of an object and then branches if it is not zero.
        ///   Operands:
        ///   0) Object number.
        /// </remarks>
        protected virtual void Operation129()
        {
            var sibling = this.ReadField(this.Operands.First, ObjectField.Sibling);
            this.Store(sibling);
            this.Branch(sibling != 0);
        }

        /// <summary>
        /// Operation 13.
        /// </summary>
        /// <remarks>
        /// Infocom name: SET
        ///   Inform name: store
        ///   This operation sets the value of a variable.
        ///   If the variable indicated is the evaluation stack (variable '0'), then the value is written in place instead of pushing onto the stack.
        ///   Operands:
        ///   0) Variable number.
        ///   1) Value to set.
        /// </remarks>
        protected virtual void Operation13()
        {
            this.ReadVariable(this.Operands.First);
            this.WriteVariable(this.Operands.First, this.Operands.Second);
        }

        /// <summary>
        /// Operation 130.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIRST?
        ///   Inform name: get_child
        ///   This operation stores the child of an object and then branches if it is not zero.
        ///   Operands:
        ///   0) Object number.
        /// </remarks>
        protected virtual void Operation130()
        {
            var child = this.ReadField(this.Operands.First, ObjectField.Child);
            this.Store(child);
            this.Branch(child != 0);
        }

        /// <summary>
        /// Operation 131.
        /// </summary>
        /// <remarks>
        /// Infocom name: LOC
        ///   Inform name: get_parent
        ///   This operation stores the parent of an object.
        ///   Operands:
        ///   0) Object number.
        /// </remarks>
        protected virtual void Operation131()
        {
            this.Store(this.ReadField(this.Operands.First, ObjectField.Parent));
        }

        /// <summary>
        /// Operation 132.
        /// </summary>
        /// <remarks>
        /// Infocom name: PTSIZE
        ///   Inform name: get_prop_len
        ///   This operation stores the data length of an object property.
        ///   Operands:
        ///   0) Property data address.
        /// </remarks>
        protected virtual void Operation132()
        {
            var propertyDataAddress = this.Operands.First;
            if (propertyDataAddress == 0)
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidProperty, "Tried to read the length of a property with a data address of zero.");
                this.Store(0);
                return;
            }

            this.Store(this.PropertyLength(propertyDataAddress));
        }

        /// <summary>
        /// Operation 133.
        /// </summary>
        /// <remarks>
        /// Infocom name: INC
        ///   Inform name: inc
        ///   This operation increments the value of a variable.
        ///   Operands:
        ///   0) Variable number.
        /// </remarks>
        protected virtual void Operation133()
        {
            var value = (short)this.ReadVariable(this.Operands.First);
            this.WriteVariable(this.Operands.First, (ushort)(++value));
        }

        /// <summary>
        /// Operation 134.
        /// </summary>
        /// <remarks>
        /// Infocom name: DEC
        ///   Inform name: dec
        ///   This operation decrements the value of a variable.
        ///   Operands:
        ///   0) Variable number.
        /// </remarks>
        protected virtual void Operation134()
        {
            var value = (short)this.ReadVariable(this.Operands.First);
            this.WriteVariable(this.Operands.First, (ushort)(--value));
        }

        /// <summary>
        /// Operation 135.
        /// </summary>
        /// <remarks>
        /// Infocom name: PRINTB
        ///   Inform name: print_addr
        ///   This operation prints encoded text.
        ///   Operands:
        ///   0) Address of text.
        /// </remarks>
        protected virtual void Operation135()
        {
            this.Write(this.DecodeText(this.Operands.First));
        }

        /// <summary>
        /// Operation 136.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation136()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 137.
        /// </summary>
        /// <remarks>
        /// Infocom name: REMOVE
        ///   Inform name: remove_obj
        ///   This operation removes an object from its parent.
        ///   Operands:
        ///   0) Object number.
        /// </remarks>
        protected virtual void Operation137()
        {
            var objectNumber = this.Operands.First;
            if (this.InvalidObject(objectNumber))
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidObject, "Tried to remove invalid object " + objectNumber + " from its parent.");
                return;
            }

            this.RemoveObjectFromSiblingList(objectNumber);
            this.WriteField(objectNumber, ObjectField.Parent, 0);
            this.WriteField(objectNumber, ObjectField.Sibling, 0);
        }

        /// <summary>
        /// Operation 138.
        /// </summary>
        /// <remarks>
        /// Infocom name: PRINTD
        ///   Inform name: print_obj
        ///   This operation prints the name of an object.
        ///   Operands:
        ///   0) Object number.
        /// </remarks>
        protected virtual void Operation138()
        {
            this.Write(this.ReadObjectName(this.Operands.First));
        }

        /// <summary>
        /// Operation 139.
        /// </summary>
        /// <remarks>
        /// Infocom name: RETURN
        ///   Inform name: ret
        ///   This operation returns a value from the current routine.
        ///   Operands:
        ///   0) Return value.
        /// </remarks>
        protected virtual void Operation139()
        {
            this.Return(this.Operands.First, this.CallStack.EndRoutine());
        }

        /// <summary>
        /// Operation 14.
        /// </summary>
        /// <remarks>
        /// Infocom name: MOVE
        ///   Inform name: insert_obj
        ///   This operation moves an object into another object.
        ///   The object is first removed from any existing parent.
        ///   Any previous child of the new parent becomes the sibling of the moved object.
        ///   Inserting an object into its current parent is not a nop because the list of siblings is reordered as a side effect.
        ///   Operands:
        ///   0) Object number to move.
        ///   1) Object number of new parent.
        /// </remarks>
        protected virtual void Operation14()
        {
            var newChild = this.Operands.First;
            var newParent = this.Operands.Second;
            if (this.InvalidObject(newChild))
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidObject, "Tried to insert invalid object " + newChild + " into parent " + newParent + ".");
                return;
            }

            if (this.InvalidObject(newParent))
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidObject, "Tried to insert object " + newChild + " into invalid parent " + newParent + ".");
                return;
            }

            this.RemoveObjectFromSiblingList(newChild);
            this.WriteField(newChild, ObjectField.Parent, newParent);
            this.WriteField(newChild, ObjectField.Sibling, this.ReadField(newParent, ObjectField.Child));
            this.WriteField(newParent, ObjectField.Child, newChild);
        }

        /// <summary>
        /// Operation 140.
        /// </summary>
        /// <remarks>
        /// Infocom name: JUMP
        ///   Inform name: jump
        ///   This operation causes the program counter to jump based on a given offset.
        ///   Operands:
        ///   0) Signed offset value.
        /// </remarks>
        protected virtual void Operation140()
        {
            this.Jump((short)this.Operands.First);
        }

        /// <summary>
        /// Operation 141.
        /// </summary>
        /// <remarks>
        /// Infocom name: PRINT
        ///   Inform name: print_paddr
        ///   This operation prints encoded text.
        ///   Operands:
        ///   0) Packed address of text.
        /// </remarks>
        protected virtual void Operation141()
        {
            this.Write(this.DecodeText(this.UnpackStringAddress(this.Operands.First)));
        }

        /// <summary>
        /// Operation 142.
        /// </summary>
        /// <remarks>
        /// Infocom name: VALUE
        ///   Inform name: load
        ///   This operation stores the value of a variable.
        ///   If the variable indicated is the evaluation stack (variable zero), then the value is read in place instead of popping off the stack.
        ///   Operands:
        ///   0) Variable number.
        /// </remarks>
        protected virtual void Operation142()
        {
            var value = this.ReadVariable(this.Operands.First);
            this.WriteVariable(this.Operands.First, value);
            this.Store(value);
        }

        /// <summary>
        /// Operation 143.
        /// </summary>
        /// <remarks>
        /// Infocom name: BCOM
        ///   Inform name: not
        ///   This operation stores the bitwise complement of a value.
        ///   Operands:
        ///   0) Bitmap.
        /// </remarks>
        protected virtual void Operation143()
        {
            this.Store((ushort)(~this.Operands.First));
        }

        /// <summary>
        /// Operation 15.
        /// </summary>
        /// <remarks>
        /// Infocom name: GET
        ///   Inform name: loadw
        ///   This operation stores a value from an array of words.
        ///   Operands:
        ///   0) Array address.
        ///   1) Array element number.
        /// </remarks>
        protected virtual void Operation15()
        {
            this.Store(this.Memory.ReadWord((ushort)(this.Operands.First + (this.Operands.Second * 2))));
        }

        /// <summary>
        /// Operation 16.
        /// </summary>
        /// <remarks>
        /// Infocom name: GETB
        ///   Inform name: loadb
        ///   This operation stores a value from an array of bytes.
        ///   Operands:
        ///   0) Array address.
        ///   1) Array element number.
        /// </remarks>
        protected virtual void Operation16()
        {
            this.Store(this.Memory.ReadByte((ushort)(this.Operands.First + this.Operands.Second)));
        }

        /// <summary>
        /// Operation 17.
        /// </summary>
        /// <remarks>
        /// Infocom name: GETP
        ///   Inform name: get_prop
        ///   This operation stores the value of an object property.
        ///   Operands:
        ///   0) Object number.
        ///   1) Property number.
        /// </remarks>
        protected virtual void Operation17()
        {
            var objectNumber = this.Operands.First;
            var propertyNumber = this.Operands.Second;
            if (this.InvalidObject(objectNumber))
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidObject, "Tried to read property " + propertyNumber + " on invalid object " + objectNumber + ".");
                this.Store(0);
                return;
            }

            if (this.InvalidProperty(propertyNumber))
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidProperty, "Tried to read invalid property " + propertyNumber + " on object " + objectNumber + ".");
                this.Store(0);
                return;
            }

            var propertyDataAddress = this.GetPropertyDataAddress(objectNumber, propertyNumber);
            if (propertyDataAddress == 0)
            {
                this.Store(this.GetPropertyDefaultValue(propertyNumber));
                return;
            }

            switch (this.PropertyLength(propertyDataAddress))
            {
                case 1:
                    this.Store(this.Memory.ReadByte(propertyDataAddress));
                    break;
                case 2:
                    this.Store(this.Memory.ReadWord(propertyDataAddress));
                    break;
                default:
                    this.FrontEnd.ErrorNotification(ErrorCondition.InvalidPropertyLength, "Tried to read property " + propertyNumber + " with length other than 1 or 2 on object " + objectNumber + ".");
                    this.Store(0);
                    break;
            }
        }

        /// <summary>
        /// Operation 176.
        /// </summary>
        /// <remarks>
        /// Infocom name: RTRUE
        ///   Inform name: rtrue
        ///   This operation returns true from the current routine.
        /// </remarks>
        protected virtual void Operation176()
        {
            this.Return(1, this.CallStack.EndRoutine());
        }

        /// <summary>
        /// Operation 177.
        /// </summary>
        /// <remarks>
        /// Infocom name: RFALSE
        ///   Inform name: rfalse
        ///   This operation returns false from the current routine.
        /// </remarks>
        protected virtual void Operation177()
        {
            this.Return(0, this.CallStack.EndRoutine());
        }

        /// <summary>
        /// Operation 178.
        /// </summary>
        /// <remarks>
        /// Infocom name: PRINTI
        ///   Inform name: print
        ///   This operation prints encoded text.
        /// </remarks>
        protected virtual void Operation178()
        {
            var address = this.CallStack.ProgramCounter;
            this.Write(this.ZCharactersToZscii(false, this.EncodedTextToZCharacters(ref address)));
            this.CallStack.ProgramCounter = address;
        }

        /// <summary>
        /// Operation 179.
        /// </summary>
        /// <remarks>
        /// Infocom name: PRINTR
        ///   Inform name: print_ret
        ///   This operation prints encoded text followed by a NewLine and then returns true from the current routine.
        /// </remarks>
        protected virtual void Operation179()
        {
            this.Write(this.DecodeText(this.CallStack.ProgramCounter));
            this.Write(new ImmutableStack<Zscii>(Zscii.NewLine));
            this.Return(1, this.CallStack.EndRoutine());
        }

        /// <summary>
        /// Operation 18.
        /// </summary>
        /// <remarks>
        /// Infocom name: GETPT
        ///   Inform name: get_prop_addr
        ///   This operation stores the data address of an object property.
        ///   Operands:
        ///   0) Object number.
        ///   1) Property number.
        /// </remarks>
        protected virtual void Operation18()
        {
            var objectNumber = this.Operands.First;
            var propertyNumber = this.Operands.Second;
            if (this.InvalidObject(objectNumber))
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidObject, "Tried to get the data address of property " + propertyNumber + " on invalid object " + objectNumber + ".");
                return;
            }

            if (this.InvalidProperty(propertyNumber))
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidProperty, "Tried to get the data address of property " + propertyNumber + " on object " + objectNumber + ".");
                return;
            }

            var propertyDataAddress = this.GetPropertyDataAddress(objectNumber, propertyNumber);
            if (propertyDataAddress > ushort.MaxValue)
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidAddress, "Data address of property " + propertyNumber + " on object " + objectNumber + " is outside 16-bit addressable memory.");
            }

            this.Store((ushort)propertyDataAddress);
        }

        /// <summary>
        /// Operation 180.
        /// </summary>
        /// <remarks>
        /// Infocom name: NOOP
        ///   Inform name: nop
        ///   This operation does nothing.
        /// </remarks>
        protected virtual void Operation180()
        {
        }

        /// <summary>
        /// Operation 181.
        /// </summary>
        /// <remarks>
        /// Infocom name: SAVE
        ///   Inform name: save
        ///   This operation saves the state of the zmachine.
        /// </remarks>
        protected virtual void Operation181()
        {
            this.FinishSaveOperation(this.FrontEnd.Save(this.SaveState()));
        }

        /// <summary>
        /// Operation 182.
        /// </summary>
        /// <remarks>
        /// Infocom name: RESTORE
        ///   Inform name: restore
        ///   This operation restores the zmachine to a previously saved state.
        /// </remarks>
        protected virtual void Operation182()
        {
            this.FinishRestoreOperation(this.RestoreState(this.FrontEnd.Restore()));
        }

        /// <summary>
        /// Operation 183.
        /// </summary>
        /// <remarks>
        /// Infocom name: RESTART
        ///   Inform name: restart
        ///   This operation restarts the zmachine.
        /// </remarks>
        protected virtual void Operation183()
        {
            this.Restart();
        }

        /// <summary>
        /// Operation 184.
        /// </summary>
        /// <remarks>
        /// Infocom name: RSTACK
        ///   Inform name: ret_popped
        ///   This operation pops a value off the evaluation stack and returns it from the current routine.
        /// </remarks>
        protected virtual void Operation184()
        {
            this.Return(this.CallStack.Pop(), this.CallStack.EndRoutine());
        }

        /// <summary>
        /// Operation 185.
        /// </summary>
        /// <remarks>
        /// Infocom name: FSTACK
        ///   Inform name: pop
        ///   This operation pops a value off the evaluation stack.
        /// </remarks>
        protected virtual void Operation185()
        {
            this.CallStack.Pop();
        }

        /// <summary>
        /// Operation 186.
        /// </summary>
        /// <remarks>
        /// Infocom name: QUIT
        ///   Inform name: quit
        ///   This operations halts the zmachine.
        /// </remarks>
        protected virtual void Operation186()
        {
            this.ReleaseExternalResources();
            this.State = MachineState.Halted;
            this.FrontEnd.Halted();
        }

        /// <summary>
        /// Operation 187.
        /// </summary>
        /// <remarks>
        /// Infocom name: CRLF
        ///   Inform name: new_line
        ///   This operation prints a zscii NewLine character.
        /// </remarks>
        protected virtual void Operation187()
        {
            this.Write(new ImmutableStack<Zscii>(Zscii.NewLine));
        }

        /// <summary>
        /// Operation 188.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation188()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 189.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation189()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 19.
        /// </summary>
        /// <remarks>
        /// Infocom name: NEXTP
        ///   Inform name: get_next_prop
        ///   This operation stores the next property on an object after a given property.
        ///   Operands:
        ///   0) Object number.
        ///   1) Property number.
        /// </remarks>
        protected virtual void Operation19()
        {
            var objectNumber = this.Operands.First;
            var propertyNumber = this.Operands.Second;
            if (this.InvalidObject(objectNumber))
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidObject, "Tried to get the next property after property " + propertyNumber + " on invalid object " + objectNumber + ".");
                this.Store(0);
                return;
            }

            if (propertyNumber == 0)
            {
                this.Store(this.GetPropertyNumber(this.GetFirstPropertyAddress(objectNumber)));
                return;
            }

            if (this.InvalidProperty(propertyNumber))
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidProperty, "Tried to get the next property after invalid property " + propertyNumber + " on object " + objectNumber + ".");
                this.Store(0);
                return;
            }

            var propertyDataAddress = this.GetPropertyDataAddress(objectNumber, propertyNumber);
            if (propertyDataAddress == 0)
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.PropertyNotFound, "Tried to get the next property after property " + propertyNumber + " not posessed by object " + objectNumber + ".");
                this.Store(0);
                return;
            }

            var nextPropertyAddress = propertyDataAddress + this.PropertyLength(propertyDataAddress);
            this.Store(this.GetPropertyNumber(nextPropertyAddress));
        }

        /// <summary>
        /// Operation 190.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation190()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 191.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation191()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 2.
        /// </summary>
        /// <remarks>
        /// Infocom name: LESS?
        ///   Inform name: jl
        ///   This operation branches if the first number is less than the second.
        ///   Operands:
        ///   0) Signed number
        ///   1) Signed number.
        /// </remarks>
        protected virtual void Operation2()
        {
            this.Branch((short)this.Operands.First < (short)this.Operands.Second);
        }

        /// <summary>
        /// Operation 20.
        /// </summary>
        /// <remarks>
        /// Infocom name: ADD
        ///   Inform name: add
        ///   This operation stores the sum of two numbers.
        ///   Operands:
        ///   0) Signed number.
        ///   1) Signed number.
        /// </remarks>
        protected virtual void Operation20()
        {
            this.Store((ushort)((short)this.Operands.First + (short)this.Operands.Second));
        }

        /// <summary>
        /// Operation 21.
        /// </summary>
        /// <remarks>
        /// Infocom name: SUB
        ///   Inform name: sub
        ///   This operation stores the difference of two numbers.
        ///   Operands:
        ///   0) Signed number.
        ///   1) Signed number.
        /// </remarks>
        protected virtual void Operation21()
        {
            this.Store((ushort)((short)this.Operands.First - (short)this.Operands.Second));
        }

        /// <summary>
        /// Operation 22.
        /// </summary>
        /// <remarks>
        /// Infocom name: MUL
        ///   Inform name: mul
        ///   This operation stores the product of two numbers.
        ///   Operands:
        ///   0) Signed number.
        ///   1) Signed number.
        /// </remarks>
        protected virtual void Operation22()
        {
            this.Store((ushort)((short)this.Operands.First * (short)this.Operands.Second));
        }

        /// <summary>
        /// Operation 224.
        /// </summary>
        /// <remarks>
        /// Infocom name: CALL
        ///   Inform name: call (Version 1-3), call_vs (Version 4+)
        ///   This operation calls a function with zero to three arguments.
        ///   Operands:
        ///   0) Routine packed address.
        ///   1) Routine argument (optional).
        ///   2) Routine argument (optional).
        ///   3) Routine argument (optional).
        /// </remarks>
        protected virtual void Operation224()
        {
            this.CallRoutineFromOperation(RoutineType.Function);
        }

        /// <summary>
        /// Operation 225.
        /// </summary>
        /// <remarks>
        /// Infocom name: PUT
        ///   Inform name: storew
        ///   This operation writes a value into an array of words.
        ///   Operands:
        ///   0) Array address.
        ///   1) Array element number.
        ///   2) Value to write.
        /// </remarks>
        protected virtual void Operation225()
        {
            this.Memory.WriteWord((ushort)(this.Operands.First + (this.Operands.Second * 2)), this.Operands.Third);
        }

        /// <summary>
        /// Operation 226.
        /// </summary>
        /// <remarks>
        /// Infocom name: PUTB
        ///   Inform name: storeb
        ///   This operation writes a value into an array of bytes.
        ///   Operands:
        ///   0) Array address.
        ///   1) Array element number.
        ///   2) Value to write.
        /// </remarks>
        protected virtual void Operation226()
        {
            this.Memory.WriteByte((ushort)(this.Operands.First + this.Operands.Second), (byte)this.Operands.Third);
        }

        /// <summary>
        /// Operation 227.
        /// </summary>
        /// <remarks>
        /// Infocom name: PUTP
        ///   Inform name: put_prop
        ///   This operation writes a value to an object property.
        ///   Operands:
        ///   0) Object number.
        ///   1) Property number.
        ///   2) Value to write.
        /// </remarks>
        protected virtual void Operation227()
        {
            var objectNumber = this.Operands.First;
            var propertyNumber = this.Operands.Second;
            var propertyValue = this.Operands.Third;
            if (this.InvalidObject(objectNumber))
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidObject, "Tried to write property " + propertyNumber + " on invalid object " + objectNumber + ".");
                return;
            }

            if (this.InvalidProperty(propertyNumber))
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidProperty, "Tried to write invalid property " + propertyNumber + " on object " + objectNumber + ".");
                return;
            }

            var propertyDataAddress = this.GetPropertyDataAddress(objectNumber, propertyNumber);
            if (propertyDataAddress == 0)
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.PropertyNotFound, "Tried to write property " + propertyNumber + " not possessed by object " + objectNumber + ".");
                return;
            }

            switch (this.PropertyLength(propertyDataAddress))
            {
                case 1:
                    this.Memory.WriteByte(propertyDataAddress, (byte)propertyValue);
                    break;
                case 2:
                    this.Memory.WriteWord(propertyDataAddress, propertyValue);
                    break;
                default:
                    this.FrontEnd.ErrorNotification(ErrorCondition.InvalidPropertyLength, "Tried to write property " + propertyNumber + " with length other than 1 or 2 on object " + objectNumber + ".");
                    break;
            }
        }

        /// <summary>
        /// Operation 228.
        /// </summary>
        /// <remarks>
        /// Infocom name: READ
        ///   Inform name: sread (Version 1-4), aread (Version 5+)
        ///   This operation reads a line of input.
        ///   Operands:
        ///   0) Text buffer address.
        ///   1) Parse buffer address.
        /// </remarks>
        protected virtual void Operation228()
        {
            this.UpdateStatusLine();
            this.BeginInputOperation(new InputOperation(this.Operands.First, this.Operands.Second, (byte)(this.Memory.ReadByte(this.Operands.First) - 1)));
        }

        /// <summary>
        /// Operation 229.
        /// </summary>
        /// <remarks>
        /// Infocom name: PRINTC
        ///   Inform name: print_char
        ///   This operation prints a zscii character.
        ///   Operands:
        ///   0) Zscii character.
        /// </remarks>
        protected virtual void Operation229()
        {
            this.Write(new ImmutableStack<Zscii>((Zscii)this.Operands.First));
        }

        /// <summary>
        /// Operation 23.
        /// </summary>
        /// <remarks>
        /// Infocom name: DIV
        ///   Inform name: div
        ///   This operation stores the quotient of two numbers.
        ///   Operands:
        ///   0) Signed number.
        ///   1) Signed number.
        /// </remarks>
        protected virtual void Operation23()
        {
            var numerator = (short)this.Operands.First;
            var denominator = (short)this.Operands.Second;
            if (denominator == 0)
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.DivisionByZero, "The second operand of the DIV instruction cannot be zero.");
                denominator = 1;
            }

            this.Store((ushort)(numerator / denominator));
        }

        /// <summary>
        /// Operation 230.
        /// </summary>
        /// <remarks>
        /// Infocom name: PRINTN
        ///   Inform name: print_num
        ///   This operation prints a number.
        ///   Operands:
        ///   0) Signed number.
        /// </remarks>
        protected virtual void Operation230()
        {
            var number = (short)this.Operands.First;
            this.Write(number < 0 ? NumberToZscii((ushort)(-number)).Add(Zscii.Hyphen) : NumberToZscii((ushort)number));
        }

        /// <summary>
        /// Operation 231.
        /// </summary>
        /// <remarks>
        /// Infocom name: RANDOM
        ///   Inform name: random
        ///   This operation stores a random number between one and a given positive range. A zero or negative range seeds the generator and stores zero.
        ///   Operands:
        ///   0) Signed range or seed value.
        /// </remarks>
        protected virtual void Operation231()
        {
            var range = (short)this.Operands.First;
            if (range > 0)
            {
                this.Store((ushort)(((ushort)this.randomNumberGenerator.Generate() % range) + 1));
                return;
            }

            var newSeed = range == 0 ? this.FrontEnd.GetRandomSeed() : -range;
            var countingMode = range < 0 && range > -1000;
            this.randomNumberGenerator.Seed(newSeed, countingMode);
            this.Store(0);
        }

        /// <summary>
        /// Operation 232.
        /// </summary>
        /// <remarks>
        /// Infocom name: PUSH
        ///   Inform name: push
        ///   This operation pushes a value onto the evaluation stack.
        ///   Operands:
        ///   0) Value to push.
        /// </remarks>
        protected virtual void Operation232()
        {
            this.CallStack.Push(this.Operands.First);
        }

        /// <summary>
        /// Operation 233.
        /// </summary>
        /// <remarks>
        /// Infocom name: POP
        ///   Inform name: pull
        ///   This operation pops a value from the evaluation stack and writes it to a variable.
        ///   If the variable indicated is the evaluation stack (variable '0'), then the value is written in place instead of pushing onto the stack.
        ///   Operands:
        ///   0) Variable number.
        /// </remarks>
        protected virtual void Operation233()
        {
            var value = this.CallStack.Pop();
            this.ReadVariable(this.Operands.First);
            this.WriteVariable(this.Operands.First, value);
        }

        /// <summary>
        /// Operation 234.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation234()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 235.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation235()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 236.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation236()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 237.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation237()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 238.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation238()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 239.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation239()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 24.
        /// </summary>
        /// <remarks>
        /// Infocom name: MOD
        ///   Inform name: mod
        ///   This operation stores the remainder after division of two numbers.
        ///   Operands:
        ///   0) Signed number.
        ///   1) Signed number.
        /// </remarks>
        protected virtual void Operation24()
        {
            var numerator = (short)this.Operands.First;
            var denominator = (short)this.Operands.Second;
            if (denominator == 0)
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.DivisionByZero, "The second operand of the MOD instruction cannot be zero.");
                denominator = 1;
            }

            this.Store((ushort)(numerator % denominator));
        }

        /// <summary>
        /// Operation 240.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation240()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 241.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation241()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 242.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation242()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 243.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation243()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 244.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation244()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 245.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation245()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 246.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation246()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 247.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation247()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 248.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation248()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 249.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation249()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 25.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation25()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 250.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation250()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 251.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation251()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 252.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation252()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 253.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation253()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 254.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation254()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 255.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation255()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 26.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation26()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 27.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation27()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 28.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation28()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 29.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation29()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 3.
        /// </summary>
        /// <remarks>
        /// Infocom name: GRTR?
        ///   Inform name: jg
        ///   This operation branches if the first number is greater than the second.
        ///   Operands:
        ///   0) Signed number.
        ///   1) Signed number.
        /// </remarks>
        protected virtual void Operation3()
        {
            var firstNumber = (short)this.Operands.First;
            var secondNumber = (short)this.Operands.Second;
            this.Branch(firstNumber > secondNumber);
        }

        /// <summary>
        /// Operation 30.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation30()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 31.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void Operation31()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 4.
        /// </summary>
        /// <remarks>
        /// Infocom name: DLESS?
        ///   Inform name: dec_chk
        ///   This operation decrements the value of a variable and then branches if the new value is less than a threshold.
        ///   Operands:
        ///   0) Variable number.
        ///   1) Signed threshold value.
        /// </remarks>
        protected virtual void Operation4()
        {
            var value = this.ReadVariable(this.Operands.First);
            this.WriteVariable(this.Operands.First, --value);
            this.Branch((short)value < (short)this.Operands.Second);
        }

        /// <summary>
        /// Operation 5.
        /// </summary>
        /// <remarks>
        /// Infocom name: IGRTR?
        ///   Inform name: inc_chk
        ///   This operation increments the value of a variable and then branches if the new value is greater than a threshold.
        ///   Operands:
        ///   0) Variable number.
        ///   1) Signed threshold value.
        /// </remarks>
        protected virtual void Operation5()
        {
            var value = this.ReadVariable(this.Operands.First);
            this.WriteVariable(this.Operands.First, ++value);
            this.Branch((short)value > (short)this.Operands.Second);
        }

        /// <summary>
        /// Operation 6.
        /// </summary>
        /// <remarks>
        /// Infocom name: IN?
        ///   Inform name: jin
        ///   This operation branches if an object has a specific parent object.
        ///   Operands:
        ///   0) Object number.
        ///   1) Parent object number.
        /// </remarks>
        protected virtual void Operation6()
        {
            this.Branch(this.Operands.Second == this.ReadField(this.Operands.First, ObjectField.Parent));
        }

        /// <summary>
        /// Operation 7.
        /// </summary>
        /// <remarks>
        /// Infocom name: BTST
        ///   Inform name: test
        ///   This operation branches if all the bits set in the second value are set in the first.
        ///   Operands:
        ///   0) Bitmap.
        ///   1) Bitmap.
        /// </remarks>
        protected virtual void Operation7()
        {
            this.Branch((this.Operands.First & this.Operands.Second) == this.Operands.Second);
        }

        /// <summary>
        /// Operation 8.
        /// </summary>
        /// <remarks>
        /// Infocom name: BOR
        ///   Inform name: or
        ///   This operation performs a bitwise 'or' operation on two values and stores the result.
        ///   Operands:
        ///   0) Bitmap.
        ///   1) Bitmap.
        /// </remarks>
        protected virtual void Operation8()
        {
            this.Store((ushort)(this.Operands.First | this.Operands.Second));
        }

        /// <summary>
        /// Operation 9.
        /// </summary>
        /// <remarks>
        /// Infocom name: BAND
        ///   Inform name: and
        ///   This operation performs a bitwise 'and' operation on two values and stores the result.
        ///   Operands:
        ///   0) Bitmap.
        ///   1) Bitmap.
        /// </remarks>
        protected virtual void Operation9()
        {
            this.Store((ushort)(this.Operands.First & this.Operands.Second));
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
        protected virtual bool ProcessCharacter(char character)
        {
            // todo: Implement my own culture invariant lower case conversion.
            var lowerCaseCharacter = char.ToLowerInvariant(character);
            if (this.UnicodeToZscii(lowerCaseCharacter) != Zscii.Null)
            {
                if (this.InputOperation.AddCharacter(lowerCaseCharacter))
                {
                    this.WriteToDisplay(new string(character, 1));
                }
                else
                {
                    this.FrontEnd.InputBufferFull();
                }
            }

            return false;
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
        protected virtual bool ProcessInputKey(InputKey inputKey)
        {
            switch (inputKey)
            {
                case InputKey.NewLine:
                    var inputText = this.InputOperation.InputText;
                    this.WriteToTranscript(inputText);
                    this.Write(new ImmutableStack<Zscii>(Zscii.NewLine));
                    this.FinishInputLineOperation(inputText, new InputValue(inputKey));
                    return true;
                case InputKey.Delete:
                    if (this.InputOperation.DeleteCharacter())
                    {
                        this.FrontEnd.DeleteCharacterFromDisplay(this.BackgroundColour);
                    }

                    break;
            }

            return false;
        }

        /// <summary>
        /// Processes an input value.
        /// </summary>
        /// <param name="inputValue">
        /// The input value.
        /// </param>
        /// <returns>
        /// A value indicating whether the input operation terminated.
        /// </returns>
        protected virtual bool ProcessInputValue(InputValue inputValue)
        {
            var value = inputValue.Value;
            if (value is char)
            {
                return this.ProcessCharacter((char)value);
            }

            if (value is InputKey)
            {
                return this.ProcessInputKey((InputKey)value);
            }

            return false;
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
        protected virtual byte PropertyLength(byte propertyHeader)
        {
            return (byte)((propertyHeader >> 5) + 1);
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
        protected virtual ushort ReadField(int fieldAddress)
        {
            return this.Memory.ReadByte(fieldAddress);
        }

        /// <summary>
        /// Reads a header flag.
        /// </summary>
        /// <param name="address">
        /// Memory address.
        /// </param>
        /// <param name="flag">
        /// The header flag.
        /// </param>
        /// <returns>
        /// Flag value.
        /// </returns>
        protected bool ReadHeaderFlag(int address, HeaderFlags flag)
        {
            return this.Memory.ReadFlags(address, (byte)flag);
        }

        /// <summary>
        /// Reads input.
        /// </summary>
        protected virtual void ReadInput()
        {
            foreach (var inputValue in this.InputOperation.UnprocessedInputValues.Enumerable())
            {
                if (this.ProcessInputValue(inputValue))
                {
                    return;
                }
            }

            this.UpdateInput();
        }

        /// <summary>
        /// Reads a zscii string from memory preceeded by a byte indicating the string length.
        /// </summary>
        /// <param name="zsciiStringAddress">
        /// The zscii string address.
        /// </param>
        /// <returns>
        /// The zscii text.
        /// </returns>
        protected ImmutableStack<Zscii> ReadZsciiFromMemory(int zsciiStringAddress)
        {
            return this.ReadZsciiFromMemory(zsciiStringAddress + 1, this.Memory.ReadByte(zsciiStringAddress));
        }

        /// <summary>
        /// Reads a zscii string from memory.
        /// </summary>
        /// <param name="zsciiStringAddress">
        /// The zscii string address.
        /// </param>
        /// <param name="characterCount">
        /// The character count.
        /// </param>
        /// <returns>
        /// The zscii text.
        /// </returns>
        protected ImmutableStack<Zscii> ReadZsciiFromMemory(int zsciiStringAddress, int characterCount)
        {
            ImmutableStack<Zscii> zsciiText = null;
            while (zsciiText.Count() < characterCount)
            {
                zsciiText = zsciiText.Add((Zscii)this.Memory.ReadByte(zsciiStringAddress++));
            }

            return zsciiText.Reverse();
        }

        /// <summary>
        /// Releases external resources used by the zmachine.
        /// </summary>
        protected virtual void ReleaseExternalResources()
        {
            this.FlushBufferedOutput();
        }

        /// <summary>
        /// Restarts the zmachine.
        /// </summary>
        protected virtual void Restart()
        {
            if (this.State != MachineState.Halted)
            {
                this.ReleaseExternalResources();
            }

            this.InputOperation = null;
            this.State = MachineState.Initializing;
        }

        /// <summary>
        /// Restores a saved state.
        /// </summary>
        /// <param name="saveState">
        /// The save state.
        /// </param>
        /// <returns>
        /// A value which indicates whether the restore was successful.
        /// </returns>
        protected bool RestoreState(ZmachineSaveState saveState)
        {
            if (saveState.Memory == null || saveState.CallStack == null)
            {
                return false;
            }

            this.Restart();
            this.Memory.Restore(saveState.Memory);
            this.CallStack.Restore(saveState.CallStack);
            this.DiscoverCapabilities();
            this.State = MachineState.Running;
            return true;
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
        protected virtual void Return(ushort returnValue, RoutineType type)
        {
            if (type == RoutineType.Function)
            {
                this.Store(returnValue);
            }
        }

        /// <summary>
        /// Saves the zmachine state.
        /// </summary>
        /// <returns>
        /// The zmachine state.
        /// </returns>
        protected ZmachineSaveState SaveState()
        {
            return new ZmachineSaveState(this.Memory.Save(), this.CallStack.Save());
        }

        /// <summary>
        /// Stores a value in the variable indicated by the byte located at the program counter address.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        protected void Store(ushort value)
        {
            this.WriteVariable(this.Memory.ReadByte(this.CallStack.ProgramCounter++), value);
        }

        /// <summary>
        /// Writes a zero to the byte following the text in the buffer.
        /// </summary>
        /// <param name="textStartAddress">
        /// The text start address.
        /// </param>
        /// <param name="zsciiCharacterCount">
        /// The zscii character count.
        /// </param>
        protected virtual void TerminateTextBuffer(int textStartAddress, byte zsciiCharacterCount)
        {
            this.Memory.WriteByte(textStartAddress + zsciiCharacterCount, 0);
        }

        /// <summary>
        /// Converts a unicode character to zscii.
        /// </summary>
        /// <param name="character">
        /// The unicode character.
        /// </param>
        /// <returns>
        /// The zscii character.
        /// </returns>
        protected Zscii UnicodeToZscii(char character)
        {
            var zsciiCharacter = (Zscii)character;
            if (IsStandardZscii(zsciiCharacter))
            {
                return zsciiCharacter;
            }

            var unicodeCharacterCount = this.UnicodeCharacterCount;
            for (byte characterNumber = 0; characterNumber < unicodeCharacterCount; characterNumber++)
            {
                if (character == this.GetUnicodeCharacter(characterNumber))
                {
                    return Zscii.FirstUnicodeCharacter + characterNumber;
                }
            }

            return Zscii.Null;
        }

        /// <summary>
        /// Converts unicode to zscii.
        /// </summary>
        /// <param name="text">
        /// The unicode text.
        /// </param>
        /// <returns>
        /// The zscii text.
        /// </returns>
        protected ImmutableStack<Zscii> UnicodeToZscii(string text)
        {
            ImmutableStack<Zscii> zsciiText = null;
            foreach (var character in text)
            {
                var zsciiCharacter = this.UnicodeToZscii(character);
                zsciiText = zsciiText.Add(zsciiCharacter == Zscii.Null ? Zscii.QuestionMark : zsciiCharacter);
            }

            return zsciiText.Reverse();
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
        protected virtual int UnpackRoutineAddress(ushort packedAddress)
        {
            return packedAddress * 2;
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
        protected virtual int UnpackStringAddress(ushort packedAddress)
        {
            return packedAddress * 2;
        }

        /// <summary>
        /// Updates the front end's status line.
        /// </summary>
        /// <param name="locationName">
        /// The location name.
        /// </param>
        /// <param name="score">
        /// The score.
        /// </param>
        /// <param name="moves">
        /// The moves.
        /// </param>
        protected virtual void UpdateFrontEndStatusLine(string locationName, short score, short moves)
        {
            this.FrontEnd.UpdateStatusLineForScoredGame(locationName, score, moves);
        }

        /// <summary>
        /// Updates input.
        /// </summary>
        protected virtual void UpdateInput()
        {
            this.InputOperation.UpdateInput(this.FrontEnd.GetInput());
        }

        /// <summary>
        /// Updates the status line.
        /// </summary>
        protected void UpdateStatusLine()
        {
            var locationName = this.ZsciiToUnicode(this.ReadObjectName(this.ReadVariable(MaximumLocalVariables + 1)));
            var score = (short)this.ReadVariable(MaximumLocalVariables + 2);
            var moves = (short)this.ReadVariable(MaximumLocalVariables + 3);
            this.UpdateFrontEndStatusLine(locationName, score, moves);
        }

        /// <summary>
        /// Writes zscii text.
        /// </summary>
        /// <param name="zsciiText">
        /// The zscii text.
        /// </param>
        protected virtual void Write(ImmutableStack<Zscii> zsciiText)
        {
            var text = this.ZsciiToUnicode(zsciiText);
            this.WriteToDisplay(text);
            this.WriteToTranscript(text);
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
        protected virtual void WriteField(int fieldAddress, ushort fieldValue)
        {
            this.Memory.WriteByte(fieldAddress, (byte)fieldValue);
        }

        /// <summary>
        /// Writes to a header flag.
        /// </summary>
        /// <param name="address">
        /// The memory address.
        /// </param>
        /// <param name="flag">
        /// The header flag.
        /// </param>
        /// <param name="value">
        /// The value to write.
        /// </param>
        protected void WriteHeaderFlag(int address, HeaderFlags flag, bool value)
        {
            this.Memory.WriteFlags(address, (byte)flag, value);
        }

        /// <summary>
        /// Writes to the display.
        /// </summary>
        /// <param name="text">
        /// The text to write.
        /// </param>
        protected virtual void WriteToDisplay(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                this.FrontEnd.WriteToDisplay(text, this.WritableArea, this.Scrollable, this.BufferOutput, this.TextStyles, this.Font, this.ForegroundColour, this.BackgroundColour);
            }
        }

        /// <summary>
        /// Writes input values to the input log.
        /// </summary>
        /// <param name="inputValues">
        /// The input values.
        /// </param>
        protected virtual void WriteToInputLog(ImmutableQueue<InputValue> inputValues)
        {
            this.FrontEnd.WriteToInputLog(inputValues);
        }

        /// <summary>
        /// Writes to the transcript.
        /// </summary>
        /// <param name="text">
        /// The text to write.
        /// </param>
        protected virtual void WriteToTranscript(string text)
        {
            if (this.TranscriptOpen && !string.IsNullOrEmpty(text))
            {
                this.FrontEnd.WriteToTranscript(text);
            }
        }

        /// <summary>
        /// Writes zscii text to memory.
        /// </summary>
        /// <param name="address">
        /// The memory address to begin writing.
        /// </param>
        /// <param name="zsciiText">
        /// The zscii text.
        /// </param>
        protected void WriteZsciiToMemory(int address, ImmutableStack<Zscii> zsciiText)
        {
            foreach (var zsciiCharacter in zsciiText.Enumerable())
            {
                this.Memory.WriteByte(address++, (byte)zsciiCharacter);
            }
        }

        /// <summary>
        /// Converts z-characters to zscii text.
        /// </summary>
        /// <param name="calledRecursively">
        /// A value which indicates whether the method was called recursively.
        /// </param>
        /// <param name="zcharacters">
        /// The z-characters.
        /// </param>
        /// <returns>
        /// The zscii text.
        /// </returns>
        protected ImmutableStack<Zscii> ZCharactersToZscii(bool calledRecursively, ImmutableStack<byte> zcharacters)
        {
            byte lockedAlphabet = 0;
            byte nextAlphabet = 0;
            ImmutableStack<Zscii> zsciiText = null;
            while (zcharacters != null)
            {
                var currentAlphabet = nextAlphabet;
                nextAlphabet = lockedAlphabet;
                var zcharacter = zcharacters.Top;
                zcharacters = zcharacters.Tail;
                this.ZCharactersToZscii(calledRecursively, zcharacter, currentAlphabet, ref nextAlphabet, ref lockedAlphabet, ref zcharacters, ref zsciiText);
            }

            return zsciiText.Reverse();
        }

        /// <summary>
        /// Converts z-characters to zscii text.
        /// </summary>
        /// <param name="calledRecursively">
        /// A value which indicates whether the method was called recursively.
        /// </param>
        /// <param name="zcharacter">
        /// The zcharacter.
        /// </param>
        /// <param name="currentAlphabet">
        /// The current alphabet.
        /// </param>
        /// <param name="nextAlphabet">
        /// The next alphabet.
        /// </param>
        /// <param name="lockedAlphabet">
        /// The locked alphabet.
        /// </param>
        /// <param name="zcharacters">
        /// The zcharacters.
        /// </param>
        /// <param name="zsciiText">
        /// The zscii text.
        /// </param>
        protected virtual void ZCharactersToZscii(bool calledRecursively, byte zcharacter, byte currentAlphabet, ref byte nextAlphabet, ref byte lockedAlphabet, ref ImmutableStack<byte> zcharacters, ref ImmutableStack<Zscii> zsciiText)
        {
            switch (zcharacter)
            {
                case 0:
                    zsciiText = zsciiText.Add(Zscii.Space);
                    break;
                case 1:
                    zsciiText = zsciiText.Add(Zscii.NewLine);
                    break;
                case 2:
                case 3:
                    nextAlphabet = (byte)((lockedAlphabet + zcharacter - 1) % 3);
                    break;
                case 4:
                case 5:
                    nextAlphabet = lockedAlphabet = (byte)((lockedAlphabet + zcharacter) % 3);
                    break;
                default:
                    if (zcharacter == 6 && currentAlphabet == 2)
                    {
                        if (zcharacters.Count() > 1)
                        {
                            zsciiText = zsciiText.Add((Zscii)((zcharacters.Top * 32) + zcharacters.Tail.Top));
                            zcharacters = zcharacters.Tail.Tail;
                        }
                        else
                        {
                            zcharacters = null;
                        }

                        break;
                    }

                    zsciiText = zsciiText.Add(this.GetZsciiAlphabetCharacter((byte)((currentAlphabet * 26) + zcharacter - 6)));
                    break;
            }
        }

        /// <summary>
        /// Converts a zscii alphabet character to z-characters.
        /// </summary>
        /// <param name="zsciiCharacter">
        /// The zscii character.
        /// </param>
        /// <param name="zsciiText">
        /// The zscii text.
        /// </param>
        /// <param name="lockedAlphabet">
        /// The locked alphabet.
        /// </param>
        /// <param name="characters">
        /// The z-characters.
        /// </param>
        protected void ZsciiAlphabetCharacterToZCharacters(Zscii zsciiCharacter, ImmutableStack<Zscii> zsciiText, ref byte lockedAlphabet, ref ImmutableStack<byte> characters)
        {
            var zsciiAlphabetIndex = this.GetZsciiAlphabetIndex(zsciiCharacter);
            var neededAlphabet = (byte)((zsciiAlphabetIndex == -1) ? 2 : zsciiAlphabetIndex / 26);
            if (neededAlphabet != lockedAlphabet)
            {
                this.AddAlphabetShiftToZCharacters(neededAlphabet, zsciiText, ref lockedAlphabet, ref characters);
            }

            if (zsciiAlphabetIndex > -1)
            {
                characters = characters.Add((byte)((zsciiAlphabetIndex % 26) + 6));
            }
            else
            {
                characters = characters.Add((byte)6);
                characters = characters.Add((byte)((ushort)zsciiCharacter / 32));
                characters = characters.Add((byte)((ushort)zsciiCharacter & 31));
            }
        }

        /// <summary>
        /// Converts a zscii character to z-characters.
        /// </summary>
        /// <param name="zsciiCharacter">
        /// The zscii character.
        /// </param>
        /// <param name="zsciiText">
        /// The zscii text following the zscii character.
        /// </param>
        /// <param name="lockedAlphabet">
        /// The locked alphabet.
        /// </param>
        /// <param name="characters">
        /// The z-characters to add the results to.
        /// </param>
        /// <remarks>
        /// Newline is now part of the zscii alphabet.
        /// </remarks>
        protected virtual void ZsciiCharacterToZCharacters(Zscii zsciiCharacter, ImmutableStack<Zscii> zsciiText, ref byte lockedAlphabet, ref ImmutableStack<byte> characters)
        {
            switch (zsciiCharacter)
            {
                case Zscii.Null:
                    break;
                case Zscii.Space:
                    characters = characters.Add((byte)0);
                    break;
                case Zscii.NewLine:
                    characters = characters.Add((byte)1);
                    break;
                default:
                    this.ZsciiAlphabetCharacterToZCharacters(zsciiCharacter, zsciiText, ref lockedAlphabet, ref characters);
                    break;
            }
        }

        /// <summary>
        /// Converts zscii text to unicode text.
        /// </summary>
        /// <param name="zsciiText">
        /// The zscii text.
        /// </param>
        /// <returns>
        /// The unicode text.
        /// </returns>
        protected string ZsciiToUnicode(ImmutableStack<Zscii> zsciiText)
        {
            ImmutableStack<char> unicodeText = null;
            foreach (var zsciiCharacter in zsciiText.Enumerable())
            {
                if (zsciiCharacter != Zscii.Null)
                {
                    unicodeText = unicodeText.Add(this.ZsciiToUnicode(zsciiCharacter));
                }
            }

            return unicodeText.Reverse().StackToString();
        }

        /// <summary>
        /// Converts a zscii character to a unicode character.
        /// </summary>
        /// <param name="zsciiCharacter">
        /// The zscii character.
        /// </param>
        /// <returns>
        /// The unicode character.
        /// </returns>
        protected char ZsciiToUnicode(Zscii zsciiCharacter)
        {
            switch (zsciiCharacter)
            {
                case Zscii.Tab:
                case Zscii.NewLine:
                    return (char)zsciiCharacter;
                case Zscii.SentenceSpace:
                    return UnicodeSentenceSpace;
                default:
                    if (IsStandardZscii(zsciiCharacter))
                    {
                        return (char)zsciiCharacter;
                    }

                    if (zsciiCharacter >= Zscii.FirstUnicodeCharacter && zsciiCharacter < Zscii.FirstUnicodeCharacter + this.UnicodeCharacterCount)
                    {
                        // todo: prevent control character output.
                        return this.GetUnicodeCharacter(zsciiCharacter - Zscii.FirstUnicodeCharacter);
                    }

                    this.FrontEnd.ErrorNotification(ErrorCondition.InvalidOutput, "Invalid character (" + zsciiCharacter + ").");
                    return (char)Zscii.QuestionMark;
            }
        }

        /// <summary>
        /// Gets a bitmask based on an attribute number.
        /// </summary>
        /// <param name="attributeNumber">
        /// The attribute number.
        /// </param>
        /// <returns>
        /// The bit mask.
        /// </returns>
        private static byte GetAttributeBitMask(ushort attributeNumber)
        {
            return (byte)(128 >> (attributeNumber & 7));
        }

        /// <summary>
        /// Gets the next word in the zscii text.
        /// </summary>
        /// <param name="zsciiText">
        /// The zscii text.
        /// </param>
        /// <param name="wordSeparators">
        /// The word separators.
        /// </param>
        /// <returns>
        /// The next word in the zscii text.
        /// </returns>
        private static ImmutableStack<Zscii> GetNextWord(ref ImmutableStack<Zscii> zsciiText, ImmutableStack<Zscii> wordSeparators)
        {
            ImmutableStack<Zscii> word = null;
            while (zsciiText != null)
            {
                var zsciiCharacter = zsciiText.Top;
                foreach (var wordSeparator in wordSeparators.Enumerable())
                {
                    if (zsciiCharacter == wordSeparator)
                    {
                        if (word == null)
                        {
                            word = word.Add(zsciiCharacter);
                            zsciiText = zsciiText.Tail;
                        }

                        return word.Reverse();
                    }
                }

                word = word.Add(zsciiCharacter);
                zsciiText = zsciiText.Tail;
            }

            return word.Reverse();
        }

        /// <summary>
        /// Initializes the operations.
        /// </summary>
        /// <returns>
        /// The operations.
        /// </returns>
        
        private static ImmutableArray<Operation> InitializeOperations()
        {
            var op = new Operation[256];
            op[0] = op[32] = op[64] = op[96] = op[192] = z => z.Operation0();
            op[1] = op[33] = op[65] = op[97] = op[193] = z => z.Operation1();
            op[2] = op[34] = op[66] = op[98] = op[194] = z => z.Operation2();
            op[3] = op[35] = op[67] = op[99] = op[195] = z => z.Operation3();
            op[4] = op[36] = op[68] = op[100] = op[196] = z => z.Operation4();
            op[5] = op[37] = op[69] = op[101] = op[197] = z => z.Operation5();
            op[6] = op[38] = op[70] = op[102] = op[198] = z => z.Operation6();
            op[7] = op[39] = op[71] = op[103] = op[199] = z => z.Operation7();
            op[8] = op[40] = op[72] = op[104] = op[200] = z => z.Operation8();
            op[9] = op[41] = op[73] = op[105] = op[201] = z => z.Operation9();
            op[10] = op[42] = op[74] = op[106] = op[202] = z => z.Operation10();
            op[11] = op[43] = op[75] = op[107] = op[203] = z => z.Operation11();
            op[12] = op[44] = op[76] = op[108] = op[204] = z => z.Operation12();
            op[13] = op[45] = op[77] = op[109] = op[205] = z => z.Operation13();
            op[14] = op[46] = op[78] = op[110] = op[206] = z => z.Operation14();
            op[15] = op[47] = op[79] = op[111] = op[207] = z => z.Operation15();
            op[16] = op[48] = op[80] = op[112] = op[208] = z => z.Operation16();
            op[17] = op[49] = op[81] = op[113] = op[209] = z => z.Operation17();
            op[18] = op[50] = op[82] = op[114] = op[210] = z => z.Operation18();
            op[19] = op[51] = op[83] = op[115] = op[211] = z => z.Operation19();
            op[20] = op[52] = op[84] = op[116] = op[212] = z => z.Operation20();
            op[21] = op[53] = op[85] = op[117] = op[213] = z => z.Operation21();
            op[22] = op[54] = op[86] = op[118] = op[214] = z => z.Operation22();
            op[23] = op[55] = op[87] = op[119] = op[215] = z => z.Operation23();
            op[24] = op[56] = op[88] = op[120] = op[216] = z => z.Operation24();
            op[25] = op[57] = op[89] = op[121] = op[217] = z => z.Operation25();
            op[26] = op[58] = op[90] = op[122] = op[218] = z => z.Operation26();
            op[27] = op[59] = op[91] = op[123] = op[219] = z => z.Operation27();
            op[28] = op[60] = op[92] = op[124] = op[220] = z => z.Operation28();
            op[29] = op[61] = op[93] = op[125] = op[221] = z => z.Operation29();
            op[30] = op[62] = op[94] = op[126] = op[222] = z => z.Operation30();
            op[31] = op[63] = op[95] = op[127] = op[223] = z => z.Operation31();
            op[128] = op[144] = op[160] = z => z.Operation128();
            op[129] = op[145] = op[161] = z => z.Operation129();
            op[130] = op[146] = op[162] = z => z.Operation130();
            op[131] = op[147] = op[163] = z => z.Operation131();
            op[132] = op[148] = op[164] = z => z.Operation132();
            op[133] = op[149] = op[165] = z => z.Operation133();
            op[134] = op[150] = op[166] = z => z.Operation134();
            op[135] = op[151] = op[167] = z => z.Operation135();
            op[136] = op[152] = op[168] = z => z.Operation136();
            op[137] = op[153] = op[169] = z => z.Operation137();
            op[138] = op[154] = op[170] = z => z.Operation138();
            op[139] = op[155] = op[171] = z => z.Operation139();
            op[140] = op[156] = op[172] = z => z.Operation140();
            op[141] = op[157] = op[173] = z => z.Operation141();
            op[142] = op[158] = op[174] = z => z.Operation142();
            op[143] = op[159] = op[175] = z => z.Operation143();
            op[176] = z => z.Operation176();
            op[177] = z => z.Operation177();
            op[178] = z => z.Operation178();
            op[179] = z => z.Operation179();
            op[180] = z => z.Operation180();
            op[181] = z => z.Operation181();
            op[182] = z => z.Operation182();
            op[183] = z => z.Operation183();
            op[184] = z => z.Operation184();
            op[185] = z => z.Operation185();
            op[186] = z => z.Operation186();
            op[187] = z => z.Operation187();
            op[188] = z => z.Operation188();
            op[189] = z => z.Operation189();
            op[190] = z => z.Operation190();
            op[191] = z => z.Operation191();
            op[224] = z => z.Operation224();
            op[225] = z => z.Operation225();
            op[226] = z => z.Operation226();
            op[227] = z => z.Operation227();
            op[228] = z => z.Operation228();
            op[229] = z => z.Operation229();
            op[230] = z => z.Operation230();
            op[231] = z => z.Operation231();
            op[232] = z => z.Operation232();
            op[233] = z => z.Operation233();
            op[234] = z => z.Operation234();
            op[235] = z => z.Operation235();
            op[236] = z => z.Operation236();
            op[237] = z => z.Operation237();
            op[238] = z => z.Operation238();
            op[239] = z => z.Operation239();
            op[240] = z => z.Operation240();
            op[241] = z => z.Operation241();
            op[242] = z => z.Operation242();
            op[243] = z => z.Operation243();
            op[244] = z => z.Operation244();
            op[245] = z => z.Operation245();
            op[246] = z => z.Operation246();
            op[247] = z => z.Operation247();
            op[248] = z => z.Operation248();
            op[249] = z => z.Operation249();
            op[250] = z => z.Operation250();
            op[251] = z => z.Operation251();
            op[252] = z => z.Operation252();
            op[253] = z => z.Operation253();
            op[254] = z => z.Operation254();
            op[255] = z => z.Operation255();
            return new ImmutableArray<Operation>(op);
        }

        /// <summary>
        /// Determines if a character is standard zscii.
        /// </summary>
        /// <param name="zsciiCharacter">
        /// The value to check.
        /// </param>
        /// <returns>
        /// A value indicating whether the value is standard zscii.
        /// </returns>
        private static bool IsStandardZscii(Zscii zsciiCharacter)
        {
            return zsciiCharacter >= Zscii.Space && zsciiCharacter <= Zscii.Tilde;
        }

        /// <summary>
        /// Converts a number into zscii text.
        /// </summary>
        /// <param name="number">
        /// The number.
        /// </param>
        /// <returns>
        /// The zscii text.
        /// </returns>
        private static ImmutableStack<Zscii> NumberToZscii(ushort number)
        {
            ImmutableStack<Zscii> zsciiText = null;
            do
            {
                zsciiText = zsciiText.Add(Zscii.Number0 + (ushort)(number % 10));
                number /= 10;
            }
            while (number != 0);
            return zsciiText;
        }

        /// <summary>
        /// Decodes encoded z-characters into zscii text.
        /// </summary>
        /// <param name="address">
        /// The address of the encoded text.
        /// </param>
        /// <returns>
        /// The zscii text.
        /// </returns>
        private ImmutableStack<Zscii> DecodeText(int address)
        {
            var zcharacters = this.EncodedTextToZCharacters(ref address);
            return this.ZCharactersToZscii(false, zcharacters);
        }

        /// <summary>
        /// Searches a dictionary for a word using a binary search.
        /// </summary>
        /// <param name="entryLength">
        /// The entry length.
        /// </param>
        /// <param name="entryCount">
        /// The entry count.
        /// </param>
        /// <param name="firstEntryAddress">
        /// The first entry address.
        /// </param>
        /// <param name="encodedWord">
        /// The encoded word.
        /// </param>
        /// <returns>
        /// The address of the dictionary entry for the word or zero if the word is not found.
        /// </returns>
        private int DictionaryBinarySearch(byte entryLength, ushort entryCount, int firstEntryAddress, ImmutableStack<ushort> encodedWord)
        {
            ushort firstEntryNumber = 0;
            while (firstEntryNumber < entryCount)
            {
                var middleEntryNumber = (ushort)((firstEntryNumber + entryCount - 1) / 2);
                var entryAddress = firstEntryAddress + (entryLength * middleEntryNumber);
                switch (this.DictionaryCompare(entryAddress, encodedWord))
                {
                    case 1:
                        entryCount = middleEntryNumber;
                        break;
                    case -1:
                        firstEntryNumber = (ushort)(middleEntryNumber + 1);
                        break;
                    default:
                        return entryAddress;
                }
            }

            return 0;
        }

        /// <summary>
        /// Compares a dictionary entry to a word.
        /// </summary>
        /// <param name="dictionaryEntryAddress">
        /// The dictionary entry address.
        /// </param>
        /// <param name="encodedWord">
        /// The encoded word.
        /// </param>
        /// <returns>
        /// Returns 0 if the entry and word are equal, -1 if the entry comes before the word alphabetically and 1 if the entry comes after the word alphabetically.
        /// </returns>
        private int DictionaryCompare(int dictionaryEntryAddress, ImmutableStack<ushort> encodedWord)
        {
            var entryPosition = 0;
            foreach (var encodedValue in encodedWord.Enumerable())
            {
                var entryPositionValue = this.Memory.ReadWord(dictionaryEntryAddress + entryPosition);
                if (entryPositionValue < encodedValue)
                {
                    return -1;
                }

                if (entryPositionValue > encodedValue)
                {
                    return 1;
                }

                entryPosition += 2;
            }

            return 0;
        }

        /// <summary>
        /// Searches a dictionary for a word using a linear search.
        /// </summary>
        /// <param name="entryLength">
        /// The entry length.
        /// </param>
        /// <param name="entryCount">
        /// The entry count.
        /// </param>
        /// <param name="firstEntryAddress">
        /// The first entry address.
        /// </param>
        /// <param name="encodedWord">
        /// The encoded word.
        /// </param>
        /// <returns>
        /// The address of the dictionary entry for the word or zero if the word is not found.
        /// </returns>
        private int DictionaryLinearSearch(byte entryLength, ushort entryCount, int firstEntryAddress, ImmutableStack<ushort> encodedWord)
        {
            for (ushort entryNumber = 0; entryNumber < entryCount; entryNumber++)
            {
                var entryAddress = firstEntryAddress + (entryLength * entryNumber);
                if (this.DictionaryCompare(entryAddress, encodedWord) == 0)
                {
                    return entryAddress;
                }
            }

            return 0;
        }

        /// <summary>
        /// Searches the dictionary for a word.
        /// </summary>
        /// <param name="dictionaryAddress">
        /// The dictionary address.
        /// </param>
        /// <param name="encodedWord">
        /// The encoded word.
        /// </param>
        /// <returns>
        /// The address of the dictionary entry for the word or zero if the word is not found.
        /// </returns>
        private int DictionarySearch(int dictionaryAddress, ImmutableStack<ushort> encodedWord)
        {
            var separatorCount = this.Memory.ReadByte(dictionaryAddress);
            var entryLengthAddress = dictionaryAddress + separatorCount + 1;
            var entryLength = this.Memory.ReadByte(entryLengthAddress);
            if (entryLength < this.DictionaryWordLength * 2)
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidDictionaryEntryLength, "Dictionary entry length " + entryLength + " is too small.");
                return 0;
            }

            var entryCount = (short)this.Memory.ReadWord(entryLengthAddress + 1);
            var firstEntryAddress = entryLengthAddress + 3;
            return entryCount < 0 ? this.DictionaryLinearSearch(entryLength, (ushort)(-entryCount), firstEntryAddress, encodedWord) : this.DictionaryBinarySearch(entryLength, (ushort)entryCount, firstEntryAddress, encodedWord);
        }

        /// <summary>
        /// Executes an instruction.
        /// </summary>
        private void ExecuteInstruction()
        {
            this.Operands.Reset();
            var operationCode = this.Memory.ReadByte(this.CallStack.ProgramCounter++);
            if (operationCode < 128)
            {
                this.LoadOperand((OperandType)((operationCode >> 6 & 1) + 1));
                this.LoadOperand((OperandType)((operationCode >> 5 & 1) + 1));
            }
            else
            {
                if (operationCode < 192)
                {
                    this.LoadOperand((OperandType)(operationCode >> 4 & 3));
                }
                else
                {
                    var firstFourOperandTypes = this.Memory.ReadByte(this.CallStack.ProgramCounter++);
                    var lastFourOperandTypes = (operationCode == 236 || operationCode == 250) ? this.Memory.ReadByte(this.CallStack.ProgramCounter++) : byte.MaxValue;
                    this.LoadOperands(firstFourOperandTypes);
                    this.LoadOperands(lastFourOperandTypes);
                }
            }

            operations[operationCode](this);
        }

        /// <summary>
        /// Finds an embedded interactive fiction identifier.
        /// </summary>
        /// <returns>
        /// The embedded interactive fiction identifier or null if not found.
        /// </returns>
        /// <remarks>
        /// The constants given are from section 2.2 of the Treaty of Babel.
        ///   See http://babel.ifarchive.org/ for more information.
        /// </remarks>
        private string FindEmbeddedIfid()
        {
            // todo: This is messy. Refactor. Also, need to verify the body of the ifid only contains digits, capital letters or hyphens.
            const int IdMinimumLength = 8;
            const int IdMaximumLength = 63;
            const string IdHeader = "UUID://";
            const string IdTrailer = "//";
            var ifidMinimumLength = IdHeader.Length + IdMinimumLength + IdTrailer.Length;
            var ifidMaximumLength = IdHeader.Length + IdMaximumLength + IdTrailer.Length;
            var storyLength = this.StoryLength;
            var endSearchAddress = storyLength - ifidMinimumLength;
            for (var address = 0; address <= endSearchAddress; address++)
            {
                if (this.MatchStoryText(IdHeader, address))
                {
                    var maxIdEndPosition = storyLength - address - 1;
                    if (maxIdEndPosition > ifidMaximumLength - 1)
                    {
                        maxIdEndPosition = ifidMaximumLength - 1;
                    }

                    for (var index = IdHeader.Length + IdMinimumLength; index < maxIdEndPosition; index++)
                    {
                        if (this.MatchStoryText(IdTrailer, address + index))
                        {
                            var ifidStart = address + IdHeader.Length;
                            var ifidLength = (byte)(index - IdHeader.Length);
                            var ifid = new char[ifidLength];
                            for (var ifidIndex = 0; ifidIndex < ifidLength; ifidIndex++)
                            {
                                var character = (Zscii)this.Memory.ReadStoryByte(ifidStart + ifidIndex);
                                ifid[ifidIndex] = (char)(IsStandardZscii(character) ? character : Zscii.Hyphen);
                            }

                            return new string(ifid);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Generates an interactive fiction identifier.
        /// </summary>
        /// <returns>
        /// An interactive fiction identifier.
        /// </returns>
        /// <remarks>
        /// The identifier is generated according to section 2.2.2.1 of the Treaty of Babel.
        ///   See http://babel.ifarchive.org/ for more information.
        /// </remarks>
        private string GenerateIfid()
        {
            var serialCode = this.SerialCode;
            var firstSerialCharacter = serialCode[0];
            var omitChecksum = serialCode == "000000" || firstSerialCharacter < '0' || firstSerialCharacter == '8' || firstSerialCharacter > '9';
            var generatedIfid = "ZCODE-" + this.ZsciiToUnicode(NumberToZscii(this.ReleaseNumber)) + "-" + serialCode;
            return omitChecksum ? generatedIfid : generatedIfid + "-" + this.ZsciiToUnicode(NumberToZscii(this.Checksum));
        }

        /// <summary>
        /// Gets the address of an object attribute.
        /// </summary>
        /// <param name="objectNumber">
        /// The object number.
        /// </param>
        /// <param name="attributeNumber">
        /// The attribute number.
        /// </param>
        /// <returns>
        /// The address of the object attribute.
        /// </returns>
        private int GetAttributeAddress(ushort objectNumber, ushort attributeNumber)
        {
            return this.GetObjectAddress(objectNumber) + (attributeNumber / 8);
        }

        /// <summary>
        /// Gets an object's field addresses.
        /// </summary>
        /// <param name="objectNumber">
        /// The object number.
        /// </param>
        /// <param name="objectField">
        /// The object field.
        /// </param>
        /// <returns>
        /// The object's field addresses.
        /// </returns>
        private int GetFieldAddress(ushort objectNumber, ObjectField objectField)
        {
            return this.GetObjectAddress(objectNumber) + this.AttributeFieldlength + (this.ObjectFieldLength * (byte)objectField);
        }

        /// <summary>
        /// Gets the address of an object's first property.
        /// </summary>
        /// <param name="objectNumber">
        /// The object number.
        /// </param>
        /// <returns>
        /// The address of the object's first property.
        /// </returns>
        private int GetFirstPropertyAddress(ushort objectNumber)
        {
            var propertyTableAddress = this.GetPropertyTableAddress(objectNumber);
            var objectShortNameLength = this.Memory.ReadByte(propertyTableAddress) * 2;
            return propertyTableAddress + 1 + objectShortNameLength;
        }

        /// <summary>
        /// Gets the address of a global variable.
        /// </summary>
        /// <param name="variableNumber">
        /// The variable number.
        /// </param>
        /// <returns>
        /// The address of the global variable.
        /// </returns>
        private int GetGlobalVariableAddress(ushort variableNumber)
        {
            var globalVariablesTableAddress = this.Memory.ReadWord(12);
            return globalVariablesTableAddress + ((variableNumber - MaximumLocalVariables - 1) * 2);
        }

        /// <summary>
        /// Gets an object's address.
        /// </summary>
        /// <param name="objectNumber">
        /// The object number.
        /// </param>
        /// <returns>
        /// The object's address.
        /// </returns>
        private int GetObjectAddress(ushort objectNumber)
        {
            const byte ObjectFieldCount = 3;
            const byte PropertyFieldLength = 2;
            const byte PropertyDefaultValueLength = 2;
            var firstObjectAddress = this.ObjectsTableAddress + (this.MaximumPropertyNumber * PropertyDefaultValueLength);
            var objectLength = this.AttributeFieldlength + (this.ObjectFieldLength * ObjectFieldCount) + PropertyFieldLength;
            return firstObjectAddress + ((objectNumber - 1) * objectLength);
        }

        /// <summary>
        /// Gets a property's data address.
        /// </summary>
        /// <param name="objectNumber">
        /// Object number.
        /// </param>
        /// <param name="propertyNumber">
        /// Property number.
        /// </param>
        /// <returns>
        /// The property's data address or zero if the property is not found on the object.
        /// </returns>
        private int GetPropertyDataAddress(ushort objectNumber, ushort propertyNumber)
        {
            var propertyAddress = this.GetFirstPropertyAddress(objectNumber);
            byte currentPropertyNumber;
            while ((currentPropertyNumber = this.GetPropertyNumber(propertyAddress)) != 0)
            {
                var propertyDataAddress = this.GetPropertyDataAddress(propertyAddress);
                if (currentPropertyNumber == propertyNumber)
                {
                    return propertyDataAddress;
                }

                propertyAddress = propertyDataAddress + this.PropertyLength(propertyDataAddress);
            }

            return 0;
        }

        /// <summary>
        /// Gets the default value of a property.
        /// </summary>
        /// <param name="propertyNumber">
        /// The property number.
        /// </param>
        /// <returns>
        /// The default value of the property.
        /// </returns>
        private ushort GetPropertyDefaultValue(ushort propertyNumber)
        {
            const byte PropertyDefaultValueLength = 2;
            return this.Memory.ReadWord(this.ObjectsTableAddress + ((propertyNumber - 1) * PropertyDefaultValueLength));
        }

        /// <summary>
        /// Gets the number of a property.
        /// </summary>
        /// <param name="propertyAddress">
        /// The property address.
        /// </param>
        /// <returns>
        /// The property number.
        /// </returns>
        private byte GetPropertyNumber(int propertyAddress)
        {
            return (byte)(this.Memory.ReadByte(propertyAddress) & this.MaximumPropertyNumber);
        }

        /// <summary>
        /// Gets an object's property table address.
        /// </summary>
        /// <param name="objectNumber">
        /// The object number.
        /// </param>
        /// <returns>
        /// The object's property table address.
        /// </returns>
        private int GetPropertyTableAddress(ushort objectNumber)
        {
            const ObjectField PropertyField = (ObjectField)3;
            return this.Memory.ReadWord(this.GetFieldAddress(objectNumber, PropertyField));
        }

        /// <summary>
        /// Gets the index of a character in the zscii alphabet.
        /// </summary>
        /// <param name="zsciiCharacter">
        /// The character.
        /// </param>
        /// <returns>
        /// The index of the character or -1 if the character is not found.
        /// </returns>
        private int GetZsciiAlphabetIndex(Zscii zsciiCharacter)
        {
            const byte ZsciiAlphabetCharacterCount = 78;
            for (byte zsciiAlphabetIndex = 0; zsciiAlphabetIndex < ZsciiAlphabetCharacterCount; zsciiAlphabetIndex++)
            {
                if (zsciiCharacter == this.GetZsciiAlphabetCharacter(zsciiAlphabetIndex))
                {
                    return zsciiAlphabetIndex;
                }
            }

            return -1;
        }

        /// <summary>
        /// Initializes the zmachine.
        /// </summary>
        private void Initialize()
        {
            this.Memory.Initialize();
            this.InitializeCallStack(this.Memory.ReadWord(6));
            this.randomNumberGenerator.Seed(this.FrontEnd.GetRandomSeed(), false);
            this.DiscoverCapabilities();
            this.FrontEnd.ResetMorePromptCounts();
            this.FrontEnd.EraseDisplayArea(this.EntireDisplayArea, this.BackgroundColour);
            this.FrontEnd.CursorPosition = this.DefaultCursorPosition;
            this.State = MachineState.Running;
        }

        /// <summary>
        /// Determines if an attribute is invalid.
        /// </summary>
        /// <param name="attributeNumber">
        /// The attribute number.
        /// </param>
        /// <returns>
        /// A value indicating whether the attribute is invalid.
        /// </returns>
        private bool InvalidAttribute(ushort attributeNumber)
        {
            return attributeNumber > this.MaximumAttributeNumber;
        }

        /// <summary>
        /// Determines if an object is invalid.
        /// </summary>
        /// <param name="objectNumber">
        /// The object number.
        /// </param>
        /// <returns>
        /// A value indicating whether the object is invalid.
        /// </returns>
        private bool InvalidObject(ushort objectNumber)
        {
            return objectNumber == 0 || objectNumber > this.MaximumObjectNumber;
        }

        /// <summary>
        /// Determines if a property is invalid.
        /// </summary>
        /// <param name="propertyNumber">
        /// The property number.
        /// </param>
        /// <returns>
        /// A value indicating whether the property is invalid.
        /// </returns>
        private bool InvalidProperty(ushort propertyNumber)
        {
            return propertyNumber == 0 || propertyNumber > this.MaximumPropertyNumber;
        }

        /// <summary>
        /// Causes the program counter to jump.
        /// </summary>
        /// <param name="offset">
        /// The offset of the jump.
        /// </param>
        private void Jump(short offset)
        {
            this.CallStack.ProgramCounter += offset - 2;
        }

        /// <summary>
        /// Loads an operand.
        /// </summary>
        /// <param name="type">
        /// The operand type.
        /// </param>
        private void LoadOperand(OperandType type)
        {
            switch (type)
            {
                case OperandType.SmallConstant:
                    this.Operands.Load(this.Memory.ReadByte(this.CallStack.ProgramCounter++));
                    break;
                case OperandType.Variable:
                    this.Operands.Load(this.ReadVariable(this.Memory.ReadByte(this.CallStack.ProgramCounter++)));
                    break;
                case OperandType.LargeConstant:
                    this.Operands.Load(this.Memory.ReadWord(this.CallStack.ProgramCounter));
                    this.CallStack.ProgramCounter += 2;
                    break;
            }
        }

        /// <summary>
        /// Determines if some text matches the story at a given address.
        /// </summary>
        /// <param name="text">
        /// The text to find.
        /// </param>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// A value indicating whether the text matches the story.
        /// </returns>
        private bool MatchStoryText(string text, int address)
        {
            if (text != null)
            {
                for (var index = 0; index < text.Length; index++)
                {
                    if (this.Memory.ReadStoryByte(address + index) != text[index])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Reads an object property length.
        /// </summary>
        /// <param name="propertyDataAddress">
        /// The property data address.
        /// </param>
        /// <returns>
        /// The property length.
        /// </returns>
        private byte PropertyLength(int propertyDataAddress)
        {
            var propertyHeader = this.Memory.ReadByte(propertyDataAddress - 1);
            return this.PropertyLength(propertyHeader);
        }

        /// <summary>
        /// Reads an object field.
        /// </summary>
        /// <param name="objectNumber">
        /// The object number.
        /// </param>
        /// <param name="objectField">
        /// The object field.
        /// </param>
        /// <returns>
        /// The field value.
        /// </returns>
        private ushort ReadField(ushort objectNumber, ObjectField objectField)
        {
            if (this.InvalidObject(objectNumber))
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidObject, "Tried to read " + objectField + " of invalid object " + objectNumber + ".");
                return 0;
            }

            return this.ReadField(this.GetFieldAddress(objectNumber, objectField));
        }

        /// <summary>
        /// Reads an object name.
        /// </summary>
        /// <param name="objectNumber">
        /// Object number.
        /// </param>
        /// <returns>
        /// The object name.
        /// </returns>
        private ImmutableStack<Zscii> ReadObjectName(ushort objectNumber)
        {
            if (this.InvalidObject(objectNumber))
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidObject, "Tried to read the name of invalid object " + objectNumber + ".");
                return null;
            }

            return this.DecodeText(this.GetPropertyTableAddress(objectNumber) + 1);
        }

        /// <summary>
        /// Reads a variable.
        /// </summary>
        /// <param name="variableNumber">
        /// The variable number.
        /// </param>
        /// <returns>
        /// The variable value.
        /// </returns>
        private ushort ReadVariable(ushort variableNumber)
        {
            if (variableNumber > MaximumLocalVariables)
            {
                return this.Memory.ReadWord(this.GetGlobalVariableAddress(variableNumber));
            }

            return variableNumber > 0 ? this.CallStack.ReadLocalVariable((byte)(variableNumber - 1)) : this.CallStack.Pop();
        }

        /// <summary>
        /// Removes an object from the list of its siblings.
        /// </summary>
        /// <param name="objectToRemove">
        /// The object to remove.
        /// </param>
        private void RemoveObjectFromSiblingList(ushort objectToRemove)
        {
            var parent = this.ReadField(objectToRemove, ObjectField.Parent);
            if (parent == 0)
            {
                return;
            }

            var sibling = this.ReadField(objectToRemove, ObjectField.Sibling);
            var previousSibling = this.ReadField(parent, ObjectField.Child);
            if (previousSibling == objectToRemove)
            {
                this.WriteField(parent, ObjectField.Child, sibling);
                return;
            }

            var previousSiblingCount = 0;
            while (previousSibling != 0 && ++previousSiblingCount < this.MaximumObjectNumber)
            {
                var nextSibling = this.ReadField(previousSibling, ObjectField.Sibling);
                if (nextSibling == objectToRemove)
                {
                    this.WriteField(previousSibling, ObjectField.Sibling, sibling);
                    return;
                }

                previousSibling = nextSibling;
            }

            this.FrontEnd.ErrorNotification(ErrorCondition.InvalidObjectTree, "Tried to remove object " + objectToRemove + " from the children of its parent " + parent + ", but it was not found.");
        }

        /// <summary>
        /// Writes an object attribute.
        /// </summary>
        /// <param name="objectNumber">
        /// Object number.
        /// </param>
        /// <param name="attributeNumber">
        /// Attribute number.
        /// </param>
        /// <param name="attributeValue">
        /// Attribute value.
        /// </param>
        private void WriteAttribute(ushort objectNumber, ushort attributeNumber, bool attributeValue)
        {
            if (this.InvalidObject(objectNumber))
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidObject, "Tried to set attribute " + attributeNumber + " on invalid object " + objectNumber + " to " + attributeValue + ".");
                return;
            }

            if (this.InvalidAttribute(attributeNumber))
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidAttribute, "Tried to set invalid attribute " + attributeNumber + " on object " + objectNumber + " to " + attributeValue + ".");
                return;
            }

            this.Memory.WriteFlags(this.GetAttributeAddress(objectNumber, attributeNumber), GetAttributeBitMask(attributeNumber), attributeValue);
        }

        /// <summary>
        /// Writes to an object field.
        /// </summary>
        /// <param name="objectNumber">
        /// The object number.
        /// </param>
        /// <param name="objectField">
        /// The object field.
        /// </param>
        /// <param name="fieldValue">
        /// The field value.
        /// </param>
        private void WriteField(ushort objectNumber, ObjectField objectField, ushort fieldValue)
        {
            if (this.InvalidObject(objectNumber))
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidObject, "Tried to write " + objectField + " of invalid object " + objectNumber + ".");
                return;
            }

            this.WriteField(this.GetFieldAddress(objectNumber, objectField), fieldValue);
        }

        /// <summary>
        /// Writes to a variable.
        /// </summary>
        /// <param name="variableNumber">
        /// The variable number.
        /// </param>
        /// <param name="value">
        /// The value to write.
        /// </param>
        private void WriteVariable(ushort variableNumber, ushort value)
        {
            if (variableNumber > MaximumLocalVariables)
            {
                this.Memory.WriteWord(this.GetGlobalVariableAddress(variableNumber), value);
                return;
            }

            if (variableNumber > 0)
            {
                this.CallStack.WriteLocalVariable((byte)(variableNumber - 1), value);
                return;
            }

            this.CallStack.Push(value);
        }

        /// <summary>
        /// Converts zscii text to z-characters.
        /// </summary>
        /// <param name="zsciiText">
        /// The zscii text.
        /// </param>
        /// <returns>
        /// The z-characters.
        /// </returns>
        private ImmutableStack<byte> ZsciiToZCharacters(ImmutableStack<Zscii> zsciiText)
        {
            ImmutableStack<byte> characters = null;
            byte lockedAlphabet = 0;
            while (zsciiText != null)
            {
                var zsciiCharacter = zsciiText.Top;
                zsciiText = zsciiText.Tail;
                this.ZsciiCharacterToZCharacters(zsciiCharacter, zsciiText, ref lockedAlphabet, ref characters);
            }

            return characters.Reverse();
        }
    }
}