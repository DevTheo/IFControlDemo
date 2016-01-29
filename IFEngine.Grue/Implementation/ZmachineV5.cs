// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZmachineV5.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   The zmachine version 5.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// The zmachine version 5.
    /// </summary>
    internal class ZmachineV5 : ZmachineV4
    {
        /// <summary>
        /// Indicates the 'Undo' feature is not available.
        /// </summary>
        private const ushort UndoNotAvailable = ushort.MaxValue;

        /// <summary>
        /// The extended operations.
        /// </summary>
        private static readonly ImmutableArray<ExtendedOperation> extendedOperations = InitializeExtendedOperations();

        /// <summary>
        /// The standard colours.
        /// </summary>
        private static readonly ImmutableArray<ColorStruct> standardColours = new ImmutableArray<ColorStruct>(new[] { ColorStruct.Black, ColorStruct.Red, ColorStruct.Green, ColorStruct.Yellow, ColorStruct.Blue, ColorStruct.Magenta, ColorStruct.Cyan, ColorStruct.White, ColorStruct.LightGrey, ColorStruct.MediumGrey, ColorStruct.DarkGrey });

        /// <summary>
        /// The background colour.
        /// </summary>
        private ColorStruct? backgroundColour;

        /// <summary>
        /// The display font.
        /// </summary>
        private Font font = Font.Normal;

        /// <summary>
        /// The foreground colour.
        /// </summary>
        private ColorStruct? foregroundColour;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZmachineV5"/> class.
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
        internal ZmachineV5(FrontEnd frontEnd, ImmutableArray<byte> story, IRandomNumberGenerator random) : base(frontEnd, story, random)
        {
        }

        /// <summary>
        /// A zmachine extended operation.
        /// </summary>
        /// <param name="machine">
        /// The zmachine to run the operation.
        /// </param>
        private delegate void ExtendedOperation(ZmachineV5 machine);

        /// <summary>
        /// Gets the current background colour.
        /// </summary>
        /// <value>
        /// The current background colour.
        /// </value>
        protected override ColorStruct? BackgroundColour
        {
            get
            {
                return this.backgroundColour;
            }
        }

        /// <summary>
        /// Gets the default cursor position.
        /// </summary>
        /// <value>
        /// The default cursor position.
        /// </value>
        /// <remarks>
        /// Infocom's version 5 interpreters varied in their placement of the cursor at the start or restart of a story.
        ///   These differences are not usually seen as most version 5 games erase the lower window or entire display on start up, moving the cursor to the upper left.
        /// </remarks>
        protected override DisplayPosition DefaultCursorPosition
        {
            get
            {
                return new DisplayPosition(0, this.UpperWindowHeight);
            }
        }

        /// <summary>
        /// Gets or sets the display font.
        /// </summary>
        /// <value>
        /// The display font.
        /// </value>
        protected override Font Font
        {
            get
            {
                return this.font;
            }
        }

        /// <summary>
        /// Gets the font height header address.
        /// </summary>
        /// <value>
        /// The font height header address.
        /// </value>
        protected virtual int FontHeightHeaderAddress
        {
            get
            {
                return 39;
            }
        }

        /// <summary>
        /// Gets the font width header address.
        /// </summary>
        /// <value>
        /// The font width header address.
        /// </value>
        protected virtual int FontWidthHeaderAddress
        {
            get
            {
                return 38;
            }
        }

        /// <summary>
        /// Gets the current foreground colour.
        /// </summary>
        /// <value>
        /// The current foreground colour.
        /// </value>
        protected override ColorStruct? ForegroundColour
        {
            get
            {
                return this.foregroundColour;
            }
        }

        /// <summary>
        /// Gets the line height.
        /// </summary>
        /// <value>
        /// The line height.
        /// </value>
        protected override byte LineHeight
        {
            get
            {
                return this.Memory.ReadByte(this.FontHeightHeaderAddress);
            }
        }

        /// <summary>
        /// Gets the text buffer header length.
        /// </summary>
        /// <value>
        /// The text buffer header length.
        /// </value>
        protected override byte TextBufferHeaderLength
        {
            get
            {
                return 2;
            }
        }

        /// <summary>
        /// Gets the unicode character count.
        /// </summary>
        /// <value>
        /// The unicode character count.
        /// </value>
        protected override byte UnicodeCharacterCount
        {
            get
            {
                var unicodeTranslationTable = this.ReadHeaderExtensionTableEntry(HeaderExtension.UnicodeTranslationTable);
                return unicodeTranslationTable == 0 ? base.UnicodeCharacterCount : this.Memory.ReadByte(unicodeTranslationTable);
            }
        }

        /// <summary>
        /// Gets the header extension table address.
        /// </summary>
        /// <value>
        /// The header extension table address.
        /// </value>
        private int HeaderExtensionTableAddress
        {
            get
            {
                return this.Memory.ReadWord(54);
            }
        }

        /// <summary>
        /// Creates a new sound effect.
        /// </summary>
        /// <param name="sound">
        /// The sound.
        /// </param>
        /// <param name="volume">
        /// The volume.
        /// </param>
        /// <returns>
        /// A new sound effect.
        /// </returns>
        protected override SoundEffect CreateSoundEffect(ushort sound, byte volume)
        {
            return new SoundEffect(sound, volume, (byte)(this.Operands.Third >> 8), this.Operands.Fourth);
        }

        /// <summary>
        /// Discovers the zmachine capabilities.
        /// </summary>
        protected override void DiscoverCapabilities()
        {
            base.DiscoverCapabilities();
            this.Memory.WriteWord(34, (ushort)this.FrontEnd.DisplayWidthInUnits);
            this.Memory.WriteWord(36, (ushort)this.FrontEnd.DisplayHeightInUnits);
            this.Memory.WriteByte(this.FontWidthHeaderAddress, this.FrontEnd.FontWidth);
            this.Memory.WriteByte(this.FontHeightHeaderAddress, this.FrontEnd.FontHeight);
            this.Memory.WriteByte(44, 1);
            this.Memory.WriteByte(45, 1);
            this.WriteHeaderExtensionTableEntry(HeaderExtension.Flags3, 0);
            this.WriteHeaderExtensionTableEntry(HeaderExtension.DefaultTrueBackgroundColour, ushort.MaxValue);
            this.WriteHeaderExtensionTableEntry(HeaderExtension.DefaultTrueForegroundColour, ushort.MaxValue);
        }

        /// <summary>
        /// Extended Operation 0.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected virtual void ExtendedOperation0()
        {
            if (this.Operands.Count == 0)
            {
                this.FinishSaveOperation(this.FrontEnd.Save(this.SaveState()));
                return;
            }

            var saveAddress = this.Operands.First;
            var saveLength = this.Operands.Second;
            var savedMemory = new byte[saveLength];
            for (ushort addressOffset = 0; addressOffset < saveLength; addressOffset++)
            {
                savedMemory[addressOffset] = this.Memory.ReadByte(saveAddress + addressOffset);
            }

            // todo: Validate savedLength.
            var defaultName = this.ZsciiToUnicode(this.ReadZsciiFromMemory(this.Operands.Third));
            var promptForName = this.Operands.Count < 4 || this.Operands.Fourth > 0;
            var savedLength = (ushort)this.FrontEnd.PartialSave(defaultName, promptForName, new ImmutableArray<byte>(savedMemory));
            this.Store(savedLength);
        }

        /// <summary>
        /// Extended Operation 1.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected virtual void ExtendedOperation1()
        {
            if (this.Operands.Count == 0)
            {
                this.FinishRestoreOperation(this.RestoreState(this.FrontEnd.Restore()));
                return;
            }

            // todo: Validate restoredLength.
            var defaultName = this.ZsciiToUnicode(this.ReadZsciiFromMemory(this.Operands.Third));
            var promptForName = this.Operands.Count < 4 || this.Operands.Fourth > 0;
            var restoreLength = this.Operands.Second;
            var restoredMemory = this.FrontEnd.PartialRestore(defaultName, promptForName, restoreLength) ?? new ImmutableArray<byte>(new byte[0]);
            var restoredLength = restoredMemory.Length < restoreLength ? restoredMemory.Length : restoreLength;
            var restoreAddress = this.Operands.First;
            for (ushort addressOffset = 0; addressOffset < restoredLength; addressOffset++)
            {
                this.Memory.WriteByte(restoreAddress + addressOffset, restoredMemory[addressOffset]);
            }

            this.Store((ushort)restoredLength);
        }

        /// <summary>
        /// Extended Operation 10.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected virtual void ExtendedOperation10()
        {
            if (this.ReadHeaderFlag(Memory.Flags2LowByteAddress, HeaderFlags.Flags2LowUndoAvailable))
            {
                this.FinishRestoreOperation(this.RestoreState(this.FrontEnd.RestoreUndo()));
                return;
            }

            this.Store(UndoNotAvailable);
        }

        /// <summary>
        /// Extended Operation 100.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation100()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 101.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation101()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 102.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation102()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 103.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation103()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 104.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation104()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 105.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation105()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 106.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation106()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 107.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation107()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 108.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation108()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 109.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation109()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 11.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected virtual void ExtendedOperation11()
        {
            var text = new string((char)this.Operands.First, 1);
            this.WriteToMemoryStream(this.UnicodeToZscii(text));
            this.WriteToDisplay(text);
            this.WriteToTranscript(text);
        }

        /// <summary>
        /// Extended Operation 110.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation110()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 111.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation111()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 112.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation112()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 113.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation113()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 114.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation114()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 115.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation115()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 116.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation116()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 117.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation117()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 118.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation118()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 119.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation119()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 12.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected virtual void ExtendedOperation12()
        {
            this.Store((ushort)this.FrontEnd.GetCharacterCapabilities((char)this.Operands.First));
        }

        /// <summary>
        /// Extended Operation 120.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation120()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 121.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation121()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 122.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation122()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 123.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation123()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 124.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation124()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 125.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation125()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 126.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation126()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 127.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation127()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 128.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation128()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 129.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation129()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 13.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected virtual void ExtendedOperation13()
        {
            var foreground = (short)this.Operands.First;
            switch (foreground)
            {
                case -2:
                    break;
                case -1:
                    this.foregroundColour = null;
                    break;
                default:
                    if (foreground < 0)
                    {
                        this.FrontEnd.ErrorNotification(ErrorCondition.InvalidColor, "Tried to use invalid foreground colour (" + foreground + ").");
                    }
                    else
                    {
                        this.foregroundColour = FifteenBitToColour(foreground);
                    }

                    break;
            }

            var background = (short)this.Operands.Second;
            switch (background)
            {
                case -2:
                    break;
                case -1:
                    this.backgroundColour = null;
                    break;
                default:
                    if (background < 0)
                    {
                        this.FrontEnd.ErrorNotification(ErrorCondition.InvalidColor, "Tried to use invalid background colour (" + background + ").");
                    }
                    else
                    {
                        this.backgroundColour = FifteenBitToColour(background);
                    }

                    break;
            }
        }

        /// <summary>
        /// Extended Operation 130.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation130()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 131.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation131()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 132.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation132()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 133.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation133()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 134.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation134()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 135.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation135()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 136.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation136()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 137.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation137()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 138.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation138()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 139.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation139()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 14.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation14()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 140.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation140()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 141.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation141()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 142.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation142()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 143.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation143()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 144.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation144()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 145.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation145()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 146.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation146()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 147.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation147()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 148.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation148()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 149.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation149()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 15.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation15()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 150.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation150()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 151.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation151()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 152.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation152()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 153.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation153()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 154.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation154()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 155.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation155()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 156.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation156()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 157.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation157()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 158.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation158()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 159.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation159()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 16.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation16()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 160.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation160()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 161.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation161()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 162.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation162()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 163.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation163()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 164.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation164()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 165.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation165()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 166.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation166()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 167.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation167()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 168.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation168()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 169.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation169()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 17.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation17()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 170.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation170()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 171.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation171()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 172.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation172()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 173.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation173()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 174.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation174()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 175.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation175()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 176.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation176()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 177.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation177()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 178.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation178()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 179.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation179()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 18.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation18()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 180.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation180()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 181.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation181()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 182.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation182()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 183.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation183()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 184.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation184()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 185.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation185()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 186.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation186()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 187.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation187()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 188.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation188()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 189.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation189()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 19.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation19()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 190.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation190()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 191.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation191()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 192.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation192()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 193.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation193()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 194.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation194()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 195.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation195()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 196.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation196()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 197.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation197()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 198.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation198()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 199.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation199()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 2.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected virtual void ExtendedOperation2()
        {
            this.BitShiftOperation(this.Operands.First, (short)this.Operands.Second);
        }

        /// <summary>
        /// Extended Operation 20.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation20()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 200.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation200()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 201.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation201()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 202.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation202()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 203.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation203()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 204.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation204()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 205.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation205()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 206.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation206()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 207.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation207()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 208.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation208()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 209.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation209()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 21.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation21()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 210.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation210()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 211.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation211()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 212.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation212()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 213.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation213()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 214.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation214()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 215.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation215()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 216.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation216()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 217.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation217()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 218.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation218()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 219.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation219()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 22.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation22()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 220.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation220()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 221.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation221()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 222.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation222()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 223.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation223()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 224.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation224()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 225.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation225()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 226.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation226()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 227.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation227()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 228.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation228()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 229.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation229()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 23.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation23()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 230.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation230()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 231.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation231()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 232.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation232()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 233.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation233()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 234.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation234()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 235.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation235()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 236.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation236()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 237.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation237()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 238.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation238()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 239.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation239()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 24.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation24()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 240.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation240()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 241.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation241()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 242.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation242()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 243.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation243()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 244.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation244()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 245.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation245()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 246.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation246()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 247.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation247()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 248.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation248()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 249.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation249()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 25.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation25()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 250.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation250()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 251.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation251()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 252.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation252()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 253.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation253()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 254.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation254()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 255.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation255()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 26.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation26()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 27.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation27()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 28.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation28()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 29.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation29()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 3.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected virtual void ExtendedOperation3()
        {
            this.BitShiftOperation((short)this.Operands.First, (short)this.Operands.Second);
        }

        /// <summary>
        /// Extended Operation 30.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation30()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 31.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation31()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 32.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation32()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 33.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation33()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 34.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation34()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 35.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation35()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 36.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation36()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 37.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation37()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 38.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation38()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 39.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation39()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 4.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected virtual void ExtendedOperation4()
        {
            var newFont = (Font)this.Operands.First;
            if (newFont != Font.Invalid)
            {
                // todo: Need to deal with set_font 0.
                // todo: The standard says we should not change the font if the frontend says it is not supported, however Infocom games do not do this because it is an addition to the zmachine.
                this.font = newFont;
                this.Store(this.FrontEnd.SetFont(newFont) ? (ushort)newFont : (ushort)Font.Invalid);
            }
        }

        /// <summary>
        /// Extended Operation 40.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation40()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 41.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation41()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 42.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation42()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 43.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation43()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 44.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation44()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 45.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation45()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 46.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation46()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 47.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation47()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 48.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation48()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 49.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation49()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 5.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation5()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 50.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation50()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 51.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation51()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 52.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation52()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 53.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation53()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 54.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation54()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 55.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation55()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 56.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation56()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 57.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation57()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 58.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation58()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 59.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation59()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 6.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation6()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 60.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation60()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 61.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation61()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 62.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation62()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 63.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation63()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 64.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation64()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 65.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation65()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 66.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation66()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 67.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation67()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 68.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation68()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 69.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation69()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 7.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation7()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 70.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation70()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 71.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation71()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 72.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation72()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 73.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation73()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 74.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation74()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 75.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation75()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 76.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation76()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 77.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation77()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 78.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation78()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 79.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation79()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 8.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation8()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 80.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation80()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 81.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation81()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 82.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation82()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 83.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation83()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 84.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation84()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 85.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation85()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 86.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation86()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 87.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation87()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 88.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation88()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 89.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation89()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 9.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected virtual void ExtendedOperation9()
        {
            if (this.ReadHeaderFlag(Memory.Flags2LowByteAddress, HeaderFlags.Flags2LowUndoAvailable))
            {
                this.FinishSaveOperation(this.FrontEnd.SaveUndo(this.SaveState()));
                return;
            }

            this.Store(UndoNotAvailable);
        }

        /// <summary>
        /// Extended Operation 90.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation90()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 91.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation91()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 92.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation92()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 93.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation93()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 94.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation94()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 95.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation95()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 96.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation96()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 97.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation97()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 98.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation98()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Extended Operation 99.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected virtual void ExtendedOperation99()
        {
            this.InvalidOperation();
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
        protected override void FinishInputLineOperation(string inputText, InputValue terminator)
        {
            this.Store((ushort)this.InputValueToZscii(terminator));
            base.FinishInputLineOperation(inputText, terminator);
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
        protected override char GetUnicodeCharacter(int index)
        {
            var unicodeTranslationTable = this.ReadHeaderExtensionTableEntry(HeaderExtension.UnicodeTranslationTable);
            if (unicodeTranslationTable != 0)
            {
                return (char)this.Memory.ReadWord(unicodeTranslationTable + 1 + (index * 2));
            }

            return base.GetUnicodeCharacter(index);
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
        protected override Zscii GetZsciiAlphabetCharacter(byte index)
        {
            var alphabetTable = this.Memory.ReadWord(52);
            if (alphabetTable != 0)
            {
                switch (index)
                {
                    case 52:
                        return Zscii.Null;
                    case 53:
                        return Zscii.NewLine;
                    default:
                        return (Zscii)this.Memory.ReadByte(alphabetTable + index);
                }
            }

            return base.GetZsciiAlphabetCharacter(index);
        }

        /// <summary>
        /// Initializes local variables at the start of a routine.
        /// </summary>
        /// <param name="localVariableCount">
        /// The local variable count.
        /// </param>
        /// <remarks>
        /// Version 5 removed the initialization of local variables.
        /// </remarks>
        protected override void InitializeLocalVariables(byte localVariableCount)
        {
            // Do nothing.
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
        protected override Zscii InputValueToZscii(InputValue inputValue)
        {
            if (inputValue.Value is MouseClick)
            {
                return (Zscii)((MouseClick)inputValue.Value).ClickType;
            }

            return base.InputValueToZscii(inputValue);
        }

        /// <summary>
        /// Determines if a sound effect should loop.
        /// </summary>
        /// <param name="soundEffect">
        /// The sound effect.
        /// </param>
        /// <returns>
        /// A value indicating whether the sound effect should loop.
        /// </returns>
        protected override bool LoopSound(SoundEffect soundEffect)
        {
            var loop = soundEffect.Loop();
            if (!loop && soundEffect.Routine != 0)
            {
                this.CallInterrupt(soundEffect.Routine);
            }

            return loop;
        }

        /// <summary>
        /// Operation 143.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void Operation143()
        {
            this.CallRoutineFromOperation(RoutineType.Procedure);
        }

        /// <summary>
        /// Operation 181.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected override void Operation181()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 182.
        /// </summary>
        /// <remarks>
        /// This operation is not used in this version of the zmachine.
        /// </remarks>
        protected override void Operation182()
        {
            this.InvalidOperation();
        }

        /// <summary>
        /// Operation 185.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void Operation185()
        {
            this.Store((ushort)this.CallStack.CatchValue);
        }

        /// <summary>
        /// Operation 190.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void Operation190()
        {
            var operationCode = this.Memory.ReadByte(this.CallStack.ProgramCounter++);
            var operandTypes = this.Memory.ReadByte(this.CallStack.ProgramCounter++);
            this.LoadOperands(operandTypes);
            extendedOperations[operationCode](this);
        }

        /// <summary>
        /// Operation 191.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void Operation191()
        {
            this.Branch(!this.FrontEnd.PiracyDetected);
        }

        /// <summary>
        /// Operation 228.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void Operation228()
        {
            this.BeginInputOperation(new InputOperation(this.Operands.First, this.Operands.Second, this.Memory.ReadByte(this.Operands.First), this.InputOperation, this.CallStack.CatchValue, this.Operands.Third, this.Operands.Fourth));
            var preloadedInputText = this.ZsciiToUnicode(this.ReadZsciiFromMemory(this.InputOperation.TextBuffer + 1));
            foreach (var character in preloadedInputText)
            {
                this.InputOperation.AddCharacter(character);
            }
        }

        /// <summary>
        /// Operation 248.
        /// </summary>
        protected override void Operation248()
        {
            this.Store((ushort)(~this.Operands.First));
        }

        /// <summary>
        /// Operation 249.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void Operation249()
        {
            this.CallRoutineFromOperation(RoutineType.Procedure);
        }

        /// <summary>
        /// Operation 250.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void Operation250()
        {
            this.CallRoutineFromOperation(RoutineType.Procedure);
        }

        /// <summary>
        /// Operation 251.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void Operation251()
        {
            var textBuffer = this.Operands.First;
            var parseBuffer = this.Operands.Second;
            var dictionaryAddress = this.Operands.Third == 0 ? this.DictionaryTableAddress : this.Operands.Third;
            var parseUnknownWords = this.Operands.Fourth == 0;
            this.LexicalAnalysis(this.ReadZsciiFromMemory(textBuffer + 1), parseBuffer, dictionaryAddress, parseUnknownWords);
        }

        /// <summary>
        /// Operation 252.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void Operation252()
        {
            // todo: Add to remarks that Zip manual allows the character count to be optional, instead relying on word breaks or a zero byte, but that would greatly complicate implementation.
            var zsciiStringAddress = this.Operands.First + this.Operands.Third;
            var characterCount = this.Operands.Second;
            var destinationAddress = this.Operands.Fourth;
            var encodedWord = this.EncodeWord(this.ReadZsciiFromMemory(zsciiStringAddress, characterCount));
            foreach (var encodedValue in encodedWord.Enumerable())
            {
                this.Memory.WriteWord(destinationAddress, encodedValue);
                destinationAddress += 2;
            }
        }

        /// <summary>
        /// Operation 253.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void Operation253()
        {
            var sourceTable = this.Operands.First;
            var destinationTable = this.Operands.Second;
            int tableLength = (short)this.Operands.Third;
            var backwardCopy = false;
            if (tableLength < 0)
            {
                tableLength = -tableLength;
            }
            else
            {
                backwardCopy = sourceTable < destinationTable;
            }

            if (destinationTable == 0)
            {
                while (tableLength-- > 0)
                {
                    this.Memory.WriteByte(sourceTable++, 0);
                }
            }
            else
            {
                if (backwardCopy)
                {
                    while (tableLength-- > 0)
                    {
                        this.Memory.WriteByte(destinationTable + tableLength, this.Memory.ReadByte(sourceTable + tableLength));
                    }
                }
                else
                {
                    for (ushort tableOffset = 0; tableOffset < tableLength; tableOffset++)
                    {
                        this.Memory.WriteByte(destinationTable + tableOffset, this.Memory.ReadByte(sourceTable + tableOffset));
                    }
                }
            }
        }

        /// <summary>
        /// Operation 254.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void Operation254()
        {
            this.FlushBufferedOutput();
            int zsciiStringAddress = this.Operands.First;
            var tableWidth = this.Operands.Second;
            var tableHeight = this.Operands.Count > 2 ? this.Operands.Third : (ushort)1;
            var charactersToSkip = this.Operands.Fourth;
            var cursorStart = this.FrontEnd.CursorPosition;
            for (ushort row = 0; row < tableHeight; row++)
            {
                if (row != 0)
                {
                    this.FlushBufferedOutput();
                    this.FrontEnd.CursorPosition = new DisplayPosition(cursorStart.Column, cursorStart.Row + row);
                }

                this.Write(this.ReadZsciiFromMemory(zsciiStringAddress, tableWidth));
                zsciiStringAddress += tableWidth + charactersToSkip;
            }
        }

        /// <summary>
        /// Operation 255.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void Operation255()
        {
            this.Branch(this.CallStack.ArgumentCount >= this.Operands.First);
        }

        /// <summary>
        /// Operation 26.
        /// </summary>
        /// <remarks>
        /// Infocom name: ICALL2
        ///   Inform name: call_2n
        ///   This operation calls a procedure with one argument. Up to three arguments are allowed in VAR form.
        ///   Operands:
        ///   0) Routine packed address.
        ///   1) Routine argument.
        /// </remarks>
        protected override void Operation26()
        {
            this.CallRoutineFromOperation(RoutineType.Procedure);
        }

        /// <summary>
        /// Operation 27.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void Operation27()
        {
            var foreground = this.Operands.First;
            switch (foreground)
            {
                case 0:
                    break;
                case 1:
                    this.foregroundColour = null;
                    break;
                default:
                    if (foreground < 13)
                    {
                        this.foregroundColour = standardColours[foreground - 2];
                    }
                    else
                    {
                        this.FrontEnd.ErrorNotification(ErrorCondition.InvalidColor, "Tried to use invalid foreground colour (" + foreground + ").");
                    }

                    break;
            }

            var background = this.Operands.Second;
            switch (background)
            {
                case 0:
                    break;
                case 1:
                    this.backgroundColour = null;
                    break;
                default:
                    if (background < 13)
                    {
                        this.backgroundColour = standardColours[background - 2];
                    }
                    else
                    {
                        this.FrontEnd.ErrorNotification(ErrorCondition.InvalidColor, "Tried to use invalid background colour (" + background + ").");
                    }

                    break;
            }
        }

        /// <summary>
        /// Operation 28.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void Operation28()
        {
            this.CallStack.Throw(this.Operands.Second);
            this.Return(this.Operands.First, this.CallStack.EndRoutine());
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
            var inputOperation = this.InputOperation;
            if (!inputOperation.ReadCharacter)
            {
                var zsciiCharacter = (Zscii)inputKey;
                if (ValidFunctionKeyCode(zsciiCharacter) && this.IsTerminator(zsciiCharacter))
                {
                    this.FinishInputLineOperation(inputOperation.InputText, new InputValue(inputKey));
                    return true;
                }
            }

            return base.ProcessInputKey(inputKey);
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
        protected override bool ProcessInputValue(InputValue inputValue)
        {
            var value = inputValue.Value;
            if (value is MouseClick)
            {
                return this.ProcessMouseClick((MouseClick)value);
            }

            return base.ProcessInputValue(inputValue);
        }

        /// <summary>
        /// Processes a mouse click.
        /// </summary>
        /// <param name="mouseClick">
        /// The mouse click.
        /// </param>
        /// <returns>
        /// A value indicating whether the input operation terminated.
        /// </returns>
        protected virtual bool ProcessMouseClick(MouseClick mouseClick)
        {
            var zsciiClick = (Zscii)mouseClick.ClickType;
            if (this.ValidMouseClick(zsciiClick))
            {
                this.WriteMouseClickPosition(mouseClick.ClickPosition);
                var inputOperation = this.InputOperation;
                if (inputOperation.ReadCharacter)
                {
                    this.FinishInputCharacterOperation(new InputValue(mouseClick));
                    return true;
                }

                if (this.IsTerminator(zsciiClick))
                {
                    this.FinishInputLineOperation(inputOperation.InputText, new InputValue(mouseClick));
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Reads from a header extension table entry.
        /// </summary>
        /// <param name="entry">
        /// The header extension table entry.
        /// </param>
        /// <returns>
        /// The value read.
        /// </returns>
        protected ushort ReadHeaderExtensionTableEntry(HeaderExtension entry)
        {
            // todo: Is this needed in V6 or make this private?
            var extensionTableAddress = this.HeaderExtensionTableAddress;
            if (extensionTableAddress > 0)
            {
                var entryCount = this.Memory.ReadWord(extensionTableAddress);
                var entryNumber = (ushort)entry;
                if (entryNumber <= entryCount)
                {
                    return this.Memory.ReadWord(extensionTableAddress + (entryNumber * 2));
                }
            }

            return 0;
        }

        /// <summary>
        /// Restarts the zmachine.
        /// </summary>
        protected override void Restart()
        {
            this.font = Font.Normal;
            this.backgroundColour = null;
            this.foregroundColour = null;
            base.Restart();
        }

        /// <summary>
        /// Sets the header flags.
        /// </summary>
        protected override void SetHeaderFlags()
        {
            base.SetHeaderFlags();
            this.WriteHeaderFlag(Flags1Address, HeaderFlags.Flags1ColoursAvailable, this.FrontEnd.ColorsAvailable);
            this.WriteHeaderFlag(Memory.Flags2LowByteAddress, HeaderFlags.Flags2LowPicturesAvailable, this.FrontEnd.PicturesAvailable);
            this.WriteHeaderFlag(Memory.Flags2LowByteAddress, HeaderFlags.Flags2LowUndoAvailable, this.FrontEnd.UndoAvailable);
            this.WriteHeaderFlag(Memory.Flags2LowByteAddress, HeaderFlags.Flags2LowMouseAvailable, this.FrontEnd.MouseAvailable);
        }

        /// <summary>
        /// Writes the character count to the byte preceeding the text in the buffer.
        /// </summary>
        /// <param name="textStartAddress">
        /// The text start address.
        /// </param>
        /// <param name="zsciiCharacterCount">
        /// The zscii character count.
        /// </param>
        protected override void TerminateTextBuffer(int textStartAddress, byte zsciiCharacterCount)
        {
            this.Memory.WriteByte(textStartAddress - 1, zsciiCharacterCount);
        }

        /// <summary>
        /// Determines if a mouse click is valid.
        /// </summary>
        /// <param name="zsciiCharacter">
        /// The zscii character.
        /// </param>
        /// <returns>
        /// A value indicating whether the mouse click is valid.
        /// </returns>
        protected virtual bool ValidMouseClick(Zscii zsciiCharacter)
        {
            return zsciiCharacter == Zscii.SingleClick || zsciiCharacter == Zscii.DoubleClick;
        }

        /// <summary>
        /// Writes to a header extension table entry.
        /// </summary>
        /// <param name="entry">
        /// The header extension table entry.
        /// </param>
        /// <param name="value">
        /// The value to write.
        /// </param>
        protected void WriteHeaderExtensionTableEntry(HeaderExtension entry, ushort value)
        {
            var extensionTableAddress = this.HeaderExtensionTableAddress;
            if (extensionTableAddress > 0)
            {
                var entryCount = this.Memory.ReadWord(extensionTableAddress);
                var entryNumber = (ushort)entry;
                if (entryNumber <= entryCount)
                {
                    this.Memory.WriteWord(extensionTableAddress + (entryNumber * 2), value);
                }
            }
        }

        /// <summary>
        /// Converts 15-bit colour to true colour.
        /// </summary>
        /// <param name="fifteenBitColour">
        /// The 15-bit colour to convert.
        /// </param>
        /// <returns>
        /// The converted colour.
        /// </returns>
        private static ColorStruct FifteenBitToColour(short fifteenBitColour)
        {
            var red = (byte)((fifteenBitColour & 31) << 3);
            var green = (byte)(((fifteenBitColour >> 5) & 31) << 3);
            var blue = (byte)(((fifteenBitColour >> 10) & 31) << 3);
            return new ColorStruct(red, green, blue);
        }

        /// <summary>
        /// Initializes the extended operations.
        /// </summary>
        /// <returns>
        /// The extended operations.
        /// </returns>
        
        private static ImmutableArray<ExtendedOperation> InitializeExtendedOperations()
        {
            var ext = new ExtendedOperation[256];
           
            ext[0] = z => z.ExtendedOperation0();
            ext[1] = z => z.ExtendedOperation1();
            ext[2] = z => z.ExtendedOperation2();
            ext[3] = z => z.ExtendedOperation3();
            ext[4] = z => z.ExtendedOperation4();
            ext[5] = z => z.ExtendedOperation5();
            ext[6] = z => z.ExtendedOperation6();
            ext[7] = z => z.ExtendedOperation7();
            ext[8] = z => z.ExtendedOperation8();
            ext[9] = z => z.ExtendedOperation9();
            ext[10] = z => z.ExtendedOperation10();
            ext[11] = z => z.ExtendedOperation11();
            ext[12] = z => z.ExtendedOperation12();
            ext[13] = z => z.ExtendedOperation13();
            ext[14] = z => z.ExtendedOperation14();
            ext[15] = z => z.ExtendedOperation15();
            ext[16] = z => z.ExtendedOperation16();
            ext[17] = z => z.ExtendedOperation17();
            ext[18] = z => z.ExtendedOperation18();
            ext[19] = z => z.ExtendedOperation19();
            ext[20] = z => z.ExtendedOperation20();
            ext[21] = z => z.ExtendedOperation21();
            ext[22] = z => z.ExtendedOperation22();
            ext[23] = z => z.ExtendedOperation23();
            ext[24] = z => z.ExtendedOperation24();
            ext[25] = z => z.ExtendedOperation25();
            ext[26] = z => z.ExtendedOperation26();
            ext[27] = z => z.ExtendedOperation27();
            ext[28] = z => z.ExtendedOperation28();
            ext[29] = z => z.ExtendedOperation29();
            ext[30] = z => z.ExtendedOperation30();
            ext[31] = z => z.ExtendedOperation31();
            ext[32] = z => z.ExtendedOperation32();
            ext[33] = z => z.ExtendedOperation33();
            ext[34] = z => z.ExtendedOperation34();
            ext[35] = z => z.ExtendedOperation35();
            ext[36] = z => z.ExtendedOperation36();
            ext[37] = z => z.ExtendedOperation37();
            ext[38] = z => z.ExtendedOperation38();
            ext[39] = z => z.ExtendedOperation39();
            ext[40] = z => z.ExtendedOperation40();
            ext[41] = z => z.ExtendedOperation41();
            ext[42] = z => z.ExtendedOperation42();
            ext[43] = z => z.ExtendedOperation43();
            ext[44] = z => z.ExtendedOperation44();
            ext[45] = z => z.ExtendedOperation45();
            ext[46] = z => z.ExtendedOperation46();
            ext[47] = z => z.ExtendedOperation47();
            ext[48] = z => z.ExtendedOperation48();
            ext[49] = z => z.ExtendedOperation49();
            ext[50] = z => z.ExtendedOperation50();
            ext[51] = z => z.ExtendedOperation51();
            ext[52] = z => z.ExtendedOperation52();
            ext[53] = z => z.ExtendedOperation53();
            ext[54] = z => z.ExtendedOperation54();
            ext[55] = z => z.ExtendedOperation55();
            ext[56] = z => z.ExtendedOperation56();
            ext[57] = z => z.ExtendedOperation57();
            ext[58] = z => z.ExtendedOperation58();
            ext[59] = z => z.ExtendedOperation59();
            ext[60] = z => z.ExtendedOperation60();
            ext[61] = z => z.ExtendedOperation61();
            ext[62] = z => z.ExtendedOperation62();
            ext[63] = z => z.ExtendedOperation63();
            ext[64] = z => z.ExtendedOperation64();
            ext[65] = z => z.ExtendedOperation65();
            ext[66] = z => z.ExtendedOperation66();
            ext[67] = z => z.ExtendedOperation67();
            ext[68] = z => z.ExtendedOperation68();
            ext[69] = z => z.ExtendedOperation69();
            ext[70] = z => z.ExtendedOperation70();
            ext[71] = z => z.ExtendedOperation71();
            ext[72] = z => z.ExtendedOperation72();
            ext[73] = z => z.ExtendedOperation73();
            ext[74] = z => z.ExtendedOperation74();
            ext[75] = z => z.ExtendedOperation75();
            ext[76] = z => z.ExtendedOperation76();
            ext[77] = z => z.ExtendedOperation77();
            ext[78] = z => z.ExtendedOperation78();
            ext[79] = z => z.ExtendedOperation79();
            ext[80] = z => z.ExtendedOperation80();
            ext[81] = z => z.ExtendedOperation81();
            ext[82] = z => z.ExtendedOperation82();
            ext[83] = z => z.ExtendedOperation83();
            ext[84] = z => z.ExtendedOperation84();
            ext[85] = z => z.ExtendedOperation85();
            ext[86] = z => z.ExtendedOperation86();
            ext[87] = z => z.ExtendedOperation87();
            ext[88] = z => z.ExtendedOperation88();
            ext[89] = z => z.ExtendedOperation89();
            ext[90] = z => z.ExtendedOperation90();
            ext[91] = z => z.ExtendedOperation91();
            ext[92] = z => z.ExtendedOperation92();
            ext[93] = z => z.ExtendedOperation93();
            ext[94] = z => z.ExtendedOperation94();
            ext[95] = z => z.ExtendedOperation95();
            ext[96] = z => z.ExtendedOperation96();
            ext[97] = z => z.ExtendedOperation97();
            ext[98] = z => z.ExtendedOperation98();
            ext[99] = z => z.ExtendedOperation99();
            ext[100] = z => z.ExtendedOperation100();
            ext[101] = z => z.ExtendedOperation101();
            ext[102] = z => z.ExtendedOperation102();
            ext[103] = z => z.ExtendedOperation103();
            ext[104] = z => z.ExtendedOperation104();
            ext[105] = z => z.ExtendedOperation105();
            ext[106] = z => z.ExtendedOperation106();
            ext[107] = z => z.ExtendedOperation107();
            ext[108] = z => z.ExtendedOperation108();
            ext[109] = z => z.ExtendedOperation109();
            ext[110] = z => z.ExtendedOperation110();
            ext[111] = z => z.ExtendedOperation111();
            ext[112] = z => z.ExtendedOperation112();
            ext[113] = z => z.ExtendedOperation113();
            ext[114] = z => z.ExtendedOperation114();
            ext[115] = z => z.ExtendedOperation115();
            ext[116] = z => z.ExtendedOperation116();
            ext[117] = z => z.ExtendedOperation117();
            ext[118] = z => z.ExtendedOperation118();
            ext[119] = z => z.ExtendedOperation119();
            ext[120] = z => z.ExtendedOperation120();
            ext[121] = z => z.ExtendedOperation121();
            ext[122] = z => z.ExtendedOperation122();
            ext[123] = z => z.ExtendedOperation123();
            ext[124] = z => z.ExtendedOperation124();
            ext[125] = z => z.ExtendedOperation125();
            ext[126] = z => z.ExtendedOperation126();
            ext[127] = z => z.ExtendedOperation127();
            ext[128] = z => z.ExtendedOperation128();
            ext[129] = z => z.ExtendedOperation129();
            ext[130] = z => z.ExtendedOperation130();
            ext[131] = z => z.ExtendedOperation131();
            ext[132] = z => z.ExtendedOperation132();
            ext[133] = z => z.ExtendedOperation133();
            ext[134] = z => z.ExtendedOperation134();
            ext[135] = z => z.ExtendedOperation135();
            ext[136] = z => z.ExtendedOperation136();
            ext[137] = z => z.ExtendedOperation137();
            ext[138] = z => z.ExtendedOperation138();
            ext[139] = z => z.ExtendedOperation139();
            ext[140] = z => z.ExtendedOperation140();
            ext[141] = z => z.ExtendedOperation141();
            ext[142] = z => z.ExtendedOperation142();
            ext[143] = z => z.ExtendedOperation143();
            ext[144] = z => z.ExtendedOperation144();
            ext[145] = z => z.ExtendedOperation145();
            ext[146] = z => z.ExtendedOperation146();
            ext[147] = z => z.ExtendedOperation147();
            ext[148] = z => z.ExtendedOperation148();
            ext[149] = z => z.ExtendedOperation149();
            ext[150] = z => z.ExtendedOperation150();
            ext[151] = z => z.ExtendedOperation151();
            ext[152] = z => z.ExtendedOperation152();
            ext[153] = z => z.ExtendedOperation153();
            ext[154] = z => z.ExtendedOperation154();
            ext[155] = z => z.ExtendedOperation155();
            ext[156] = z => z.ExtendedOperation156();
            ext[157] = z => z.ExtendedOperation157();
            ext[158] = z => z.ExtendedOperation158();
            ext[159] = z => z.ExtendedOperation159();
            ext[160] = z => z.ExtendedOperation160();
            ext[161] = z => z.ExtendedOperation161();
            ext[162] = z => z.ExtendedOperation162();
            ext[163] = z => z.ExtendedOperation163();
            ext[164] = z => z.ExtendedOperation164();
            ext[165] = z => z.ExtendedOperation165();
            ext[166] = z => z.ExtendedOperation166();
            ext[167] = z => z.ExtendedOperation167();
            ext[168] = z => z.ExtendedOperation168();
            ext[169] = z => z.ExtendedOperation169();
            ext[170] = z => z.ExtendedOperation170();
            ext[171] = z => z.ExtendedOperation171();
            ext[172] = z => z.ExtendedOperation172();
            ext[173] = z => z.ExtendedOperation173();
            ext[174] = z => z.ExtendedOperation174();
            ext[175] = z => z.ExtendedOperation175();
            ext[176] = z => z.ExtendedOperation176();
            ext[177] = z => z.ExtendedOperation177();
            ext[178] = z => z.ExtendedOperation178();
            ext[179] = z => z.ExtendedOperation179();
            ext[180] = z => z.ExtendedOperation180();
            ext[181] = z => z.ExtendedOperation181();
            ext[182] = z => z.ExtendedOperation182();
            ext[183] = z => z.ExtendedOperation183();
            ext[184] = z => z.ExtendedOperation184();
            ext[185] = z => z.ExtendedOperation185();
            ext[186] = z => z.ExtendedOperation186();
            ext[187] = z => z.ExtendedOperation187();
            ext[188] = z => z.ExtendedOperation188();
            ext[189] = z => z.ExtendedOperation189();
            ext[190] = z => z.ExtendedOperation190();
            ext[191] = z => z.ExtendedOperation191();
            ext[192] = z => z.ExtendedOperation192();
            ext[193] = z => z.ExtendedOperation193();
            ext[194] = z => z.ExtendedOperation194();
            ext[195] = z => z.ExtendedOperation195();
            ext[196] = z => z.ExtendedOperation196();
            ext[197] = z => z.ExtendedOperation197();
            ext[198] = z => z.ExtendedOperation198();
            ext[199] = z => z.ExtendedOperation199();
            ext[200] = z => z.ExtendedOperation200();
            ext[201] = z => z.ExtendedOperation201();
            ext[202] = z => z.ExtendedOperation202();
            ext[203] = z => z.ExtendedOperation203();
            ext[204] = z => z.ExtendedOperation204();
            ext[205] = z => z.ExtendedOperation205();
            ext[206] = z => z.ExtendedOperation206();
            ext[207] = z => z.ExtendedOperation207();
            ext[208] = z => z.ExtendedOperation208();
            ext[209] = z => z.ExtendedOperation209();
            ext[210] = z => z.ExtendedOperation210();
            ext[211] = z => z.ExtendedOperation211();
            ext[212] = z => z.ExtendedOperation212();
            ext[213] = z => z.ExtendedOperation213();
            ext[214] = z => z.ExtendedOperation214();
            ext[215] = z => z.ExtendedOperation215();
            ext[216] = z => z.ExtendedOperation216();
            ext[217] = z => z.ExtendedOperation217();
            ext[218] = z => z.ExtendedOperation218();
            ext[219] = z => z.ExtendedOperation219();
            ext[220] = z => z.ExtendedOperation220();
            ext[221] = z => z.ExtendedOperation221();
            ext[222] = z => z.ExtendedOperation222();
            ext[223] = z => z.ExtendedOperation223();
            ext[224] = z => z.ExtendedOperation224();
            ext[225] = z => z.ExtendedOperation225();
            ext[226] = z => z.ExtendedOperation226();
            ext[227] = z => z.ExtendedOperation227();
            ext[228] = z => z.ExtendedOperation228();
            ext[229] = z => z.ExtendedOperation229();
            ext[230] = z => z.ExtendedOperation230();
            ext[231] = z => z.ExtendedOperation231();
            ext[232] = z => z.ExtendedOperation232();
            ext[233] = z => z.ExtendedOperation233();
            ext[234] = z => z.ExtendedOperation234();
            ext[235] = z => z.ExtendedOperation235();
            ext[236] = z => z.ExtendedOperation236();
            ext[237] = z => z.ExtendedOperation237();
            ext[238] = z => z.ExtendedOperation238();
            ext[239] = z => z.ExtendedOperation239();
            ext[240] = z => z.ExtendedOperation240();
            ext[241] = z => z.ExtendedOperation241();
            ext[242] = z => z.ExtendedOperation242();
            ext[243] = z => z.ExtendedOperation243();
            ext[244] = z => z.ExtendedOperation244();
            ext[245] = z => z.ExtendedOperation245();
            ext[246] = z => z.ExtendedOperation246();
            ext[247] = z => z.ExtendedOperation247();
            ext[248] = z => z.ExtendedOperation248();
            ext[249] = z => z.ExtendedOperation249();
            ext[250] = z => z.ExtendedOperation250();
            ext[251] = z => z.ExtendedOperation251();
            ext[252] = z => z.ExtendedOperation252();
            ext[253] = z => z.ExtendedOperation253();
            ext[254] = z => z.ExtendedOperation254();
            ext[255] = z => z.ExtendedOperation255();
            return new ImmutableArray<ExtendedOperation>(ext);
        }

        /// <summary>
        /// Performs a bit shift on the given value and stores the result.
        /// </summary>
        /// <param name="value">
        /// The value to shift.
        /// </param>
        /// <param name="shift">
        /// The number of places to shift to the left (positive) or right (negative).
        /// </param>
        private void BitShiftOperation(int value, short shift)
        {
            if (shift < -15 || shift > 15)
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.ShiftOutOfRange, "Shift out of range +15 to -15 (" + shift + ").");
            }

            this.Store((ushort)((shift < 0) ? (value >> -shift) : (value << shift)));
        }

        /// <summary>
        /// Determines whether a zscii character is in the terminating characters table.
        /// </summary>
        /// <param name="zsciiCharacter">
        /// The zscii character.
        /// </param>
        /// <returns>
        /// A value indicating whether the zscii character is in the terminating characters table.
        /// </returns>
        /// <remarks>
        /// A value of 255 in the table will match any value.
        /// </remarks>
        private bool IsTerminator(Zscii zsciiCharacter)
        {
            int terminatingCharactersTable = this.Memory.ReadWord(46);
            if (terminatingCharactersTable != 0)
            {
                byte terminator;
                while ((terminator = this.Memory.ReadByte(terminatingCharactersTable++)) != 0)
                {
                    if ((Zscii)terminator == zsciiCharacter || terminator == byte.MaxValue)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Writes a mouse click position to memory.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        private void WriteMouseClickPosition(DisplayPosition position)
        {
            this.WriteHeaderExtensionTableEntry(HeaderExtension.MouseColumn, (ushort)(position.Column + 1));
            this.WriteHeaderExtensionTableEntry(HeaderExtension.MouseRow, (ushort)(position.Row + 1));
        }
    }
}