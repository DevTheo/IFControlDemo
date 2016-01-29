// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZmachineV3.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   The zmachine version 3.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// The zmachine version 3.
    /// </summary>
    /// <remarks>
    /// Version 3 changes the modifier for calculating the story length and adds support for two more sets of abbreviations.
    ///   It also adds operations to:
    ///   Play sounds.
    ///   Update the status line.
    ///   Split the display into upper and lower windows.
    ///   Select the current display window.
    ///   Open or close output streams.
    ///   Select the current input stream.
    ///   Verify the the story matches the checksum in the header.
    /// </remarks>
    internal class ZmachineV3 : ZmachineV2, IZmachine
    {
        /// <summary>
        /// A value indicating whether the display stream is open.
        /// </summary>
        private bool displayStreamOpen = true;

        /// <summary>
        /// The last sound used.
        /// </summary>
        private ushort lastSound;

        /// <summary>
        /// The memory streams currently open.
        /// </summary>
        private ImmutableStack<MemoryStream> memoryStreams;

        /// <summary>
        /// The current music effect.
        /// </summary>
        private SoundEffect musicEffect;

        /// <summary>
        /// A value indicating whether to queue sounds as they are played.
        /// </summary>
        private bool queueSounds;

        /// <summary>
        /// The sound effects queued to play.
        /// </summary>
        private ImmutableQueue<SoundEffect> soundEffects;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZmachineV3"/> class.
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
        internal ZmachineV3(FrontEnd frontEnd, ImmutableArray<byte> story, IRandomNumberGenerator random) : base(frontEnd, story, random)
        {
        }

        /// <summary>
        /// Gets the Flags1 address.
        /// </summary>
        /// <value>
        /// The flags 1 address.
        /// </value>
        protected static byte Flags1Address
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Gets the upper window default cursor position.
        /// </summary>
        /// <value>
        /// The upper window default cursor position.
        /// </value>
        protected static DisplayPosition UpperWindowDefaultCursorPosition
        {
            get
            {
                return new DisplayPosition(0, 0);
            }
        }

        /// <summary>
        /// Gets or sets the active display window.
        /// </summary>
        /// <value>
        /// The active display window.
        /// </value>
        protected DisplayWindow ActiveDisplayWindow
        {
            get;
            set;
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
                return this.ActiveDisplayWindow == DisplayWindow.Lower && base.BufferOutput;
            }
        }

        /// <summary>
        /// Gets or sets the inactive lower window cursor position.
        /// </summary>
        /// <value>
        /// The inactive lower window cursor position.
        /// </value>
        protected DisplayPosition InactiveLowerWindowCursorPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the lower window area.
        /// </summary>
        /// <value>
        /// The lower window area.
        /// </value>
        protected DisplayArea LowerWindowArea
        {
            get
            {
                return new DisplayArea(new DisplayPosition(0, this.UpperWindowHeight), new DisplayAreaSize(this.FrontEnd.DisplayColumnCount, this.FrontEnd.DisplayRowCount - this.UpperWindowHeight));
            }
        }

        /// <summary>
        /// Gets the memory streams currently open.
        /// </summary>
        /// <value>
        /// The memory streams currently open.
        /// </value>
        protected ImmutableStack<MemoryStream> MemoryStreams
        {
            get
            {
                return this.memoryStreams;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the display is scrollable.
        /// </summary>
        /// <value>
        /// A value indicating whether the display is scrollable.
        /// </value>
        protected override bool Scrollable
        {
            get
            {
                return this.ActiveDisplayWindow == DisplayWindow.Lower;
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
                return this.ActiveDisplayWindow == DisplayWindow.Upper || this.ReadHeaderFlag(Memory.Flags2LowByteAddress, HeaderFlags.Flags2LowForceFixedPitch) ? TextStyles.Fixed : TextStyles.None;
            }
        }

        /// <summary>
        /// Gets the upper window area.
        /// </summary>
        /// <value>
        /// The upper window area.
        /// </value>
        protected DisplayArea UpperWindowArea
        {
            get
            {
                return new DisplayArea(new DisplayPosition(0, 0), new DisplayAreaSize(this.FrontEnd.DisplayColumnCount, this.UpperWindowHeight));
            }
        }

        /// <summary>
        /// Gets or sets the upper window height.
        /// </summary>
        /// <value>
        /// The upper window height.
        /// </value>
        protected byte UpperWindowHeight
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the writable area of the display.
        /// </summary>
        /// <value>
        /// The writable area of the display.
        /// </value>
        protected override DisplayArea WritableArea
        {
            get
            {
                return this.ActiveDisplayWindow == DisplayWindow.Upper ? this.UpperWindowArea : this.LowerWindowArea;
            }
        }

        /// <summary>
        /// Indicates a sound has finished playing.
        /// </summary>
        /// <param name="sound">
        /// The sound.
        /// </param>
        void IZmachine.SoundFinished(int sound)
        {
            lock (this.Memory)
            {
                this.ProcessFinishedSound(sound);
            }
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
        protected override void AddAlphabetShiftToZCharacters(byte neededAlphabet, ImmutableStack<Zscii> zsciiText, ref byte lockedAlphabet, ref ImmutableStack<byte> characters)
        {
            characters = characters.Add((byte)(neededAlphabet + 3));
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
            this.queueSounds = false;
        }

        /// <summary>
        /// Closes a memory stream.
        /// </summary>
        /// <param name="memoryStream">
        /// The memory stream.
        /// </param>
        protected virtual void CloseMemoryStream(MemoryStream memoryStream)
        {
            // todo: Error if stream has too many characters.
            this.Memory.WriteWord(memoryStream.Table, (ushort)memoryStream.CharacterCount);
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
        protected virtual SoundEffect CreateSoundEffect(ushort sound, byte volume)
        {
            return new SoundEffect(sound, volume, this.FrontEnd.SoundLoops(this.lastSound));
        }

        /// <summary>
        /// Discovers the zmachine capabilities.
        /// </summary>
        protected override void DiscoverCapabilities()
        {
            base.DiscoverCapabilities();
            this.SetHeaderFlags();
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
        protected virtual bool LoopSound(SoundEffect soundEffect)
        {
            return soundEffect.Loop();
        }

        /// <summary>
        /// Opens a memory stream.
        /// </summary>
        /// <returns>
        /// The memory stream.
        /// </returns>
        protected virtual MemoryStream OpenMemoryStream()
        {
            return new MemoryStream(this.Operands.Second);
        }

        /// <summary>
        /// Operation 188.
        /// </summary>
        /// <remarks>
        /// Infocom name: USL
        ///   Inform name: show_status
        ///   This operation updates the status line.
        /// </remarks>
        protected override void Operation188()
        {
            this.UpdateStatusLine();
        }

        /// <summary>
        /// Operation 189.
        /// </summary>
        /// <remarks>
        /// Infocom name: VERIFY
        ///   Inform name: verify
        ///   This operation branches if the story checksum is correct.
        /// </remarks>
        protected override void Operation189()
        {
            var storyLength = this.StoryLength;
            ushort checksum = 0;
            const int HeaderLength = 64;
            for (var storyPosition = HeaderLength; storyPosition < storyLength; storyPosition++)
            {
                checksum += this.Memory.ReadStoryByte(storyPosition);
            }

            this.Branch(checksum == this.Checksum);
        }

        /// <summary>
        /// Operation 234.
        /// </summary>
        /// <remarks>
        /// Infocom name: SPLIT
        ///   Inform name: split_window
        ///   This operation splits the display into upper and lower windows. It then erases the upper window if it has a non-zero height, resetting the cursor if the upper window was already active.
        ///   Operands:
        ///   0) Upper window height.
        /// </remarks>
        protected override void Operation234()
        {
            this.SplitWindow(this.Operands.First);
            if (this.UpperWindowHeight > 0)
            {
                this.FrontEnd.EraseDisplayArea(this.UpperWindowArea, this.BackgroundColour);
                if (this.ActiveDisplayWindow == DisplayWindow.Upper)
                {
                    this.FrontEnd.CursorPosition = UpperWindowDefaultCursorPosition;
                }
            }
        }

        /// <summary>
        /// Operation 235.
        /// </summary>
        /// <remarks>
        /// Infocom name: SCREEN
        ///   Inform name: set_window
        ///   This operation makes a given display window active.
        ///   Operands:
        ///   0) Window number.
        /// </remarks>
        protected override void Operation235()
        {
            this.FlushBufferedOutput();
            var window = (DisplayWindow)this.Operands.First;
            switch (window)
            {
                case DisplayWindow.Lower:
                    if (this.ActiveDisplayWindow != DisplayWindow.Lower)
                    {
                        this.ActiveDisplayWindow = window;
                        this.FrontEnd.CursorPosition = this.InactiveLowerWindowCursorPosition;
                    }

                    break;
                case DisplayWindow.Upper:
                    if (this.ActiveDisplayWindow == DisplayWindow.Lower)
                    {
                        this.InactiveLowerWindowCursorPosition = this.FrontEnd.CursorPosition;
                    }

                    this.ActiveDisplayWindow = window;
                    this.FrontEnd.CursorPosition = UpperWindowDefaultCursorPosition;
                    break;
                default:
                    this.FrontEnd.ErrorNotification(ErrorCondition.InvalidWindow, "Tried to set invalid window (" + window + ").");
                    break;
            }
        }

        /// <summary>
        /// Operation 243.
        /// </summary>
        /// <remarks>
        /// Infocom name: DIROUT
        ///   Inform name: output_stream
        ///   This operation changes controls the zmachine output streams.
        ///   Operands:
        ///   0) Stream number (signed byte).
        ///   1) Memory stream address (optional).
        /// </remarks>
        protected override void Operation243()
        {
            var stream = (sbyte)this.Operands.First;
            var state = true;
            if (stream < 0)
            {
                state = false;
                stream *= -1;
            }

            switch (stream)
            {
                case 0:
                    break;
                case 1:
                    this.displayStreamOpen = state;
                    break;
                case 2:
                    this.TranscriptOpen = state;
                    break;
                case 3:
                    if (state)
                    {
                        if (this.Operands.Count < 2)
                        {
                            this.FrontEnd.ErrorNotification(ErrorCondition.InvalidStream, "Tried to open a memory stream but no address was provided.");
                            break;
                        }

                        if (this.memoryStreams.Count() == 16)
                        {
                            this.FrontEnd.ErrorNotification(ErrorCondition.InvalidStream, "Tried to open too many memory streams.");
                            break;
                        }

                        this.memoryStreams = this.memoryStreams.Add(this.OpenMemoryStream());
                        break;
                    }

                    if (this.memoryStreams == null)
                    {
                        this.FrontEnd.ErrorNotification(ErrorCondition.InvalidStream, "Tried to close a memory stream, but none are open.");
                        break;
                    }

                    this.CloseMemoryStream(this.memoryStreams.Top);
                    this.memoryStreams = this.memoryStreams.Tail;
                    break;
                case 4:
                    this.InputLogOpen = state;
                    this.FrontEnd.ControlOutputStream(state, OutputStream.InputLog);
                    break;
                default:
                    this.FrontEnd.ErrorNotification(ErrorCondition.InvalidStream, "Tried to " + (state ? "open" : "close") + " an invalid output stream (" + stream + ").");
                    break;
            }
        }

        /// <summary>
        /// Operation 244.
        /// </summary>
        /// <remarks>
        /// Infocom name: DIRIN
        ///   Inform name: input_stream
        ///   This operation sets the active input stream.
        ///   Operands:
        ///   0) Stream number.
        /// </remarks>
        protected override void Operation244()
        {
            var stream = (InputStream)this.Operands.First;
            switch (stream)
            {
                case InputStream.InputLog:
                case InputStream.Keyboard:
                    this.FrontEnd.OpenInputStream(stream);
                    break;
                default:
                    this.FrontEnd.ErrorNotification(ErrorCondition.InvalidStream, "Tried to open invalid input stream (" + stream + ".)");
                    break;
            }
        }

        /// <summary>
        /// Operation 245.
        /// </summary>
        /// <remarks>
        /// Infocom name: SOUND
        ///   Inform name: sound_effect
        ///   This operation manipulates sound effects.
        ///   The standard 1.1 interpretation of 'stop all' and 'unload all' when providing operands (0,3) and (0,4) respectively is followed despite the evidence that this was not Infocom's intent.
        ///   Due to the addition of a second channel for music in standard 1.1, changing this now would be a breaking change.
        ///   This implementation expands upon standard 1.1 by following the behavior of Infocom's sound capable interpreters.
        ///   Sepcifically:
        ///   Omitting the third operand is the same as specifying one repetition at the default volume.
        ///   Omitting the second operand is the same as specifying play.
        ///   Omitting the first operand or providing it with a zero value is the same as specifying the last used sound number.
        ///   The last used sound number is updated on all actions except unload.
        ///   Actions performed with sound numbers one and two (high beep and low beep) never update the last used sound number.
        ///   For two reasons this implementation resets the last sound number on restart or restore and issues an error if an attempt is made to use the last sound number before a valid sound is used:
        ///   1) Infocom interpreters vary in their behavior when attempting to use the last sound before a valid sound is used.
        ///   2) Infocom interpreters do not reset the last sound number during a restart or restore, leading to inconsistent behavior versus initial start up.
        ///   Operands:
        ///   0) Sound number
        ///   1) Action number
        ///   2) Volume.
        /// </remarks>
        protected override void Operation245()
        {
            var sound = this.Operands.First;
            switch (sound)
            {
                case 1:
                    this.FrontEnd.PlayBeep(Beep.High);
                    return;
                case 2:
                    this.FrontEnd.PlayBeep(Beep.Low);
                    return;
            }

            var action = this.Operands.Count > 1 ? this.Operands.Second : (ushort)2;
            if (sound != 0 && action != 4)
            {
                this.lastSound = sound;
            }

            switch (action)
            {
                case 1:
                    if (this.lastSound == 0)
                    {
                        this.FrontEnd.ErrorNotification(ErrorCondition.InvalidSoundAction, "Tried to load last sound before a vaild sound has been used.");
                        break;
                    }

                    this.FrontEnd.LoadSound(this.lastSound);
                    break;
                case 2:
                    if (this.lastSound == 0)
                    {
                        this.FrontEnd.ErrorNotification(ErrorCondition.InvalidSoundAction, "Tried to play last sound before a vaild sound has been used.");
                        break;
                    }

                    this.PlaySound(this.CreateSoundEffect(this.lastSound, this.Operands.Count > 2 ? (byte)this.Operands.Third : byte.MaxValue));
                    break;
                case 3:
                    if (sound == 0)
                    {
                        this.musicEffect = null;
                        this.soundEffects = null;
                        this.FrontEnd.StopAllSounds();
                        break;
                    }

                    this.StopSound(this.lastSound);
                    break;
                case 4:
                    if (sound == 0)
                    {
                        this.FrontEnd.UnloadAllSounds();
                        break;
                    }

                    this.FrontEnd.UnloadSound(sound);
                    break;
                default:
                    this.FrontEnd.ErrorNotification(ErrorCondition.InvalidSoundAction, "Tried to use sound " + this.lastSound + " with action " + action + ".");
                    break;
            }
        }

        /// <summary>
        /// Releases external resources used by the zmachine.
        /// </summary>
        protected override void ReleaseExternalResources()
        {
            base.ReleaseExternalResources();
            this.FrontEnd.StopAllSounds();
            this.FrontEnd.UnloadAllSounds();
        }

        /// <summary>
        /// Restarts the zmachine.
        /// </summary>
        protected override void Restart()
        {
            this.musicEffect = null;
            this.soundEffects = null;
            this.memoryStreams = null;
            this.displayStreamOpen = true;
            this.ActiveDisplayWindow = DisplayWindow.Lower;
            this.UpperWindowHeight = 0;
            this.lastSound = 0;
            base.Restart();
        }

        /// <summary>
        /// Sets the header flags.
        /// </summary>
        protected virtual void SetHeaderFlags()
        {
            this.WriteHeaderFlag(Flags1Address, HeaderFlags.Flags1TandyComputer, this.FrontEnd.TandyComputer);
            this.WriteHeaderFlag(Flags1Address, HeaderFlags.Flags1StatusLineUnavailable, !this.FrontEnd.StatusLineAvailable);
            this.WriteHeaderFlag(Flags1Address, HeaderFlags.Flags1DisplaySplitAvailable, this.FrontEnd.DisplaySplitAvailable);
            this.WriteHeaderFlag(Flags1Address, HeaderFlags.Flags1DefaultToProportionalFont, this.FrontEnd.DefaultToVariablePitchFont);
            this.SetHeaderFlagsForSound();
        }

        /// <summary>
        /// Sets the header flags for sound.
        /// </summary>
        protected virtual void SetHeaderFlagsForSound()
        {
            this.WriteHeaderFlag(Memory.Flags2LowByteAddress, HeaderFlags.Flags2LowSoundsAvailableOld, this.FrontEnd.SoundsAvailable);
        }

        /// <summary>
        /// Splits the display into upper and lower windows.
        /// </summary>
        /// <param name="upperWindowHeight">
        /// The upper window height.
        /// </param>
        protected void SplitWindow(ushort upperWindowHeight)
        {
            // todo: Revisit this mess when working on V6 cursor handling.
            var displayRowCount = this.FrontEnd.DisplayRowCount;
            if (upperWindowHeight > displayRowCount)
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidWindowSize, "Attempted to set upper window size (" + upperWindowHeight + ") greater than display size (" + displayRowCount + ").");
                return;
            }

            this.FlushBufferedOutput();
            this.UpperWindowHeight = (byte)upperWindowHeight;
            if (this.ActiveDisplayWindow == DisplayWindow.Lower && upperWindowHeight > this.FrontEnd.CursorPosition.Row)
            {
                this.FrontEnd.CursorPosition = this.DefaultCursorPosition;
            }
            else
            {
                if (upperWindowHeight > this.InactiveLowerWindowCursorPosition.Row)
                {
                    this.InactiveLowerWindowCursorPosition = this.DefaultCursorPosition;
                }
            }

            if (this.ActiveDisplayWindow == DisplayWindow.Upper && upperWindowHeight <= this.FrontEnd.CursorPosition.Row)
            {
                this.FrontEnd.CursorPosition = UpperWindowDefaultCursorPosition;
            }
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
        /// The number of moves.
        /// </param>
        protected override void UpdateFrontEndStatusLine(string locationName, short score, short moves)
        {
            if (this.ReadHeaderFlag(Flags1Address, HeaderFlags.Flags1TimeBased))
            {
                this.FrontEnd.UpdateStatusLineForTimedGame(locationName, score, moves);
                return;
            }

            base.UpdateFrontEndStatusLine(locationName, score, moves);
        }

        /// <summary>
        /// Writes zscii text.
        /// </summary>
        /// <param name="zsciiText">
        /// The zscii text.
        /// </param>
        protected override void Write(ImmutableStack<Zscii> zsciiText)
        {
            this.WriteToMemoryStream(zsciiText);
            base.Write(zsciiText);
        }

        /// <summary>
        /// Writes to the display.
        /// </summary>
        /// <param name="text">
        /// The text to write.
        /// </param>
        protected override void WriteToDisplay(string text)
        {
            if (this.displayStreamOpen && this.memoryStreams == null)
            {
                base.WriteToDisplay(text);
            }
        }

        /// <summary>
        /// Writes zscii text to a memory stream.
        /// </summary>
        /// <param name="zsciiText">
        /// The zscii text.
        /// </param>
        protected virtual void WriteToMemoryStream(ImmutableStack<Zscii> zsciiText)
        {
            if (this.memoryStreams != null)
            {
                var memoryStream = this.memoryStreams.Top;
                this.WriteZsciiToMemory(memoryStream.Table + memoryStream.CharacterCount + 2, zsciiText);
                memoryStream.CharacterCount += zsciiText.Count();
            }
        }

        /// <summary>
        /// Writes to the transcript.
        /// </summary>
        /// <param name="text">
        /// The text to write.
        /// </param>
        protected override void WriteToTranscript(string text)
        {
            if (this.ActiveDisplayWindow == DisplayWindow.Lower && this.memoryStreams == null)
            {
                base.WriteToTranscript(text);
            }
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
        protected override void ZCharactersToZscii(bool calledRecursively, byte zcharacter, byte currentAlphabet, ref byte nextAlphabet, ref byte lockedAlphabet, ref ImmutableStack<byte> zcharacters, ref ImmutableStack<Zscii> zsciiText)
        {
            switch (zcharacter)
            {
                case 1:
                case 2:
                case 3:
                    this.AppendAbbreviation(zcharacter, calledRecursively, ref zcharacters, ref zsciiText);
                    break;
                case 4:
                case 5:
                    if (currentAlphabet == 0)
                    {
                        nextAlphabet = (byte)(zcharacter % 3);
                        break;
                    }

                    nextAlphabet = lockedAlphabet = (byte)(currentAlphabet - ((zcharacter - currentAlphabet) % 3));
                    break;
                default:
                    base.ZCharactersToZscii(calledRecursively, zcharacter, currentAlphabet, ref nextAlphabet, ref lockedAlphabet, ref zcharacters, ref zsciiText);
                    break;
            }
        }

        /// <summary>
        /// Plays the next sound effect.
        /// </summary>
        private void PlayNextSound()
        {
            if (this.soundEffects != null)
            {
                var effect = this.soundEffects.Front;
                this.FrontEnd.PlaySound(effect.Sound, effect.Volume);
            }
        }

        /// <summary>
        /// Plays a sound effect.
        /// </summary>
        /// <param name="soundEffect">
        /// The sound effect.
        /// </param>
        /// <remarks>
        /// This manages the sound queue and current music effect whenever a sound is played.
        ///   To enable correct behavior for 'The Lurking Horror' the sound is queued if an input operation has not occurred since the last sound was played.
        ///   Music is never queued, as specified in the Blorb standard.
        /// </remarks>
        private void PlaySound(SoundEffect soundEffect)
        {
            if (this.FrontEnd.SoundIsMusic(soundEffect.Sound))
            {
                if (this.musicEffect != null)
                {
                    this.FrontEnd.StopSound(this.musicEffect.Sound);
                }

                this.musicEffect = soundEffect;
                this.FrontEnd.PlaySound(this.musicEffect.Sound, this.musicEffect.Volume);
                return;
            }

            if (!this.queueSounds && this.soundEffects != null)
            {
                var sound = this.soundEffects.Front.Sound;
                this.soundEffects = null;
                this.FrontEnd.StopSound(sound);
            }

            this.queueSounds = true;
            this.soundEffects = this.soundEffects.Add(soundEffect);
            if (this.soundEffects.Count() == 1)
            {
                this.PlayNextSound();
            }
        }

        /// <summary>
        /// Processes a sound that has finished playing.
        /// </summary>
        /// <param name="sound">
        /// The sound.
        /// </param>
        private void ProcessFinishedSound(int sound)
        {
            if (this.musicEffect.Sound == sound)
            {
                if (!this.LoopSound(this.musicEffect))
                {
                    this.musicEffect = null;
                }

                return;
            }

            if (this.soundEffects != null)
            {
                var soundEffect = this.soundEffects.Front;
                if (sound == soundEffect.Sound)
                {
                    if (!this.LoopSound(soundEffect) || this.soundEffects.Tail != null)
                    {
                        this.soundEffects = this.soundEffects.Tail;
                    }

                    this.PlayNextSound();
                }
            }
        }

        /// <summary>
        /// Stops a sound that is playing.
        /// </summary>
        /// <param name="sound">
        /// The sound.
        /// </param>
        private void StopSound(ushort sound)
        {
            if (this.musicEffect.Sound == sound)
            {
                this.musicEffect = null;
                this.FrontEnd.StopSound(sound);
                return;
            }

            if (this.soundEffects != null && this.soundEffects.Front.Sound == sound)
            {
                this.FrontEnd.StopSound(this.soundEffects.Front.Sound);
                this.soundEffects = this.soundEffects.Tail;
                this.PlayNextSound();
            }
        }
    }
}