// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrontEnd.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   The zmachine front end.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using IFInterfaces.Support;
using System;
using System.Threading.Tasks;

namespace Zmachine
{
    /// <summary>
    /// The zmachine front end.
    /// </summary>
    internal class FrontEnd
    {
        IIFRuntime runtime;
        /// <summary>
        /// The zmachine.
        /// </summary>
        private IZmachine zmachine;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrontEnd"/> class.
        /// </summary>
        internal FrontEnd(IIFRuntime runtime)
        {
            this.runtime = runtime;
            this.FontHeight = 1;
            this.FontWidth = 1;
            this.StandardRevisionMinor = 1;
            this.StandardRevisionMajor = 1;
            this.Interpreter = Interpreter.DEC20;
        }

        /// <summary>
        /// Gets or sets a value indicating whether colours are available.
        /// </summary>
        /// <value>
        /// A value indicating whether colours are available.
        /// </value>
        internal bool ColorsAvailable
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets or sets the cursor position.
        /// </summary>
        /// <value>
        /// The cursor position.
        /// </value>
        internal DisplayPosition CursorPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the default font is variable pitch.
        /// </summary>
        /// <value>
        /// A value indicating whether the default font is variable pitch.
        /// </value>
        internal bool DefaultToVariablePitchFont
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the display column count.
        /// </summary>
        /// <value>
        /// The display column count.
        /// </value>
        internal byte DisplayColumnCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the display height in units.
        /// </summary>
        /// <value>
        /// The display height in units.
        /// </value>
        internal int DisplayHeightInUnits
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the display row count.
        /// </summary>
        /// <value>
        /// The display row count.
        /// </value>
        internal byte DisplayRowCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether display splitting is available.
        /// </summary>
        /// <value>
        /// A value indicating whether display splitting is available.
        /// </value>
        internal bool DisplaySplitAvailable
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets or sets the display width in units.
        /// </summary>
        /// <value>
        /// The display width in units.
        /// </value>
        internal int DisplayWidthInUnits
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the font height.
        /// </summary>
        /// <value>
        /// The font height.
        /// </value>
        internal byte FontHeight
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the font width.
        /// </summary>
        /// <value>
        /// The font width.
        /// </value>
        internal byte FontWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the interpreter.
        /// </summary>
        /// <value>
        /// The interpreter.
        /// </value>
        internal Interpreter Interpreter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the interpreter version.
        /// </summary>
        /// <value>
        /// The interpreter version.
        /// </value>
        internal byte InterpreterVersion
        {
            get;
            set;
        } = 5;

        /// <summary>
        /// Gets or sets a value indicating whether menus are available.
        /// </summary>
        /// <value>
        /// A value indicating whether menus are available.
        /// </value>
        internal bool MenusAvailable
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets or sets a value indicating whether a mouse is available.
        /// </summary>
        /// <value>
        /// A value indicating whether a mouse is available.
        /// </value>
        internal bool MouseAvailable
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets or sets a value indicating whether pictures are available.
        /// </summary>
        /// <value>
        /// A value indicating whether pictures are available.
        /// </value>
        internal bool PicturesAvailable
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets or sets a value indicating whether piracy has been detected.
        /// </summary>
        /// <value>
        /// A value indicating whether piracy has been detected.
        /// </value>
        internal bool PiracyDetected
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets or sets a value indicating whether sounds are available.
        /// </summary>
        /// <value>
        /// A value indicating whether sounds are available.
        /// </value>
        internal bool SoundsAvailable
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets or sets the standard revison major number.
        /// </summary>
        /// <value>
        /// The standard revision major number.
        /// </value>
        internal byte StandardRevisionMajor
        {
            get;
            set;
        } = 5;

        /// <summary>
        /// Gets or sets the standard revision minor number.
        /// </summary>
        /// <value>
        /// The standard revision minor number.
        /// </value>
        internal byte StandardRevisionMinor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether a status line is available.
        /// </summary>
        /// <value>
        /// A value indicating whether a status line is available.
        /// </value>
        internal bool StatusLineAvailable
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets or sets the supported text styles.
        /// </summary>
        /// <value>
        /// The supported text styles.
        /// </value>
        internal TextStyles SupportedTextStyles
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is a Tandy computer.
        /// </summary>
        /// <value>
        /// A value indicating whether this is a Tandy computer.
        /// </value>
        internal bool TandyComputer
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets or sets a value indicating whether timed input is available.
        /// </summary>
        /// <value>
        /// A value indicating whether timed input is available.
        /// </value>
        internal bool TimedInputAvailable
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets or sets a value indicating whether transparency is available.
        /// </summary>
        /// <value>
        /// A value indicating whether transparency is available.
        /// </value>
        internal bool TransparencyAvailable
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets or sets a value indicating whether undo is available.
        /// </summary>
        /// <value>
        /// A value indicating whether undo is available.
        /// </value>
        internal bool UndoAvailable
        {
            get;
            set;
        } = false;

        /// <summary>
        /// Gets the zmachine.
        /// </summary>
        /// <value>
        /// The zmachine.
        /// </value>
        internal IZmachine Zmachine
        {
            get
            {
                return this.zmachine;
            }
        }

        public bool Done { get; internal set; }

        /// <summary>
        /// Adds a menu.
        /// </summary>
        /// <param name="menu">
        /// The menu to add.
        /// </param>
        /// <param name="items">
        /// The menu items.
        /// </param>
        /// <returns>
        /// A value indicating whether the menu was added.
        /// </returns>
        internal virtual bool AddMenu(MenuEntry menu, ImmutableArray<MenuEntry> items)
        {
            return false;
        }

        /// <summary>
        /// Confines mouse movement.
        /// </summary>
        /// <param name="confinedArea">
        /// The confined area.
        /// </param>
        internal virtual void ConfineMouse(DisplayArea confinedArea)
        {
        }

        /// <summary>
        /// Opens or closes an output stream.
        /// </summary>
        /// <param name="open">
        /// A value indicating whether to open or close the selected output stream.
        /// </param>
        /// <param name="outputStream">
        /// The output stream.
        /// </param>
        internal virtual void ControlOutputStream(bool open, OutputStream outputStream)
        {
        }

        /// <summary>
        /// Deletes the character behind the cursor with the given background colour and moves the cursor to that location.
        /// </summary>
        /// <param name="backgroundColor">
        /// The background colour.
        /// </param>
        /// <remarks>
        /// This is never buffered.
        /// </remarks>
        internal void DeleteCharacterFromDisplay(ColorStruct? backgroundColor)
        {

        }

        /// <summary>
        /// Draws a picture.
        /// </summary>
        /// <param name="picture">
        /// The picture to draw.
        /// </param>
        /// <param name="cropArea">
        /// The area to crop the picture to.
        /// </param>
        internal virtual void DrawPicture(int picture, DisplayArea cropArea)
        {
        }

        /// <summary>
        /// Erases an area of the display with the given background colour.
        /// </summary>
        /// <param name="area">
        /// The area to erase.
        /// </param>
        /// <param name="backgroundColor">
        /// The background colour.
        /// </param>
        /// <remarks>
        /// This does not move the cursor.
        /// </remarks>
        internal void EraseDisplayArea(DisplayArea area, ColorStruct? backgroundColor)
        {

        }

        /// <summary>
        /// Indicates an error has occurred.
        /// </summary>
        /// <param name="condition">
        /// The error condition.
        /// </param>
        /// <param name="description">
        /// The error description.
        /// </param>
        internal void ErrorNotification(ErrorCondition condition, string description)
        {

        }

        /// <summary>
        /// Frees the mouse from movement restrictions.
        /// </summary>
        internal virtual void FreeMouse()
        {
        }

        /// <summary>
        /// Gets the capabilites for a character.
        /// </summary>
        /// <param name="character">
        /// The character.
        /// </param>
        /// <returns>
        /// Character capabilities.
        /// </returns>
        internal virtual CharacterCapabilities GetCharacterCapabilities(char character)
        {
            return CharacterCapabilities.None;
        }

        private ImmutableQueue<InputValue> input = null;
        async Task ReadInput()
        {

        }

        /// <summary>
        /// Gets input.
        /// </summary>
        /// <returns>
        /// The input.
        /// </returns>
        internal ImmutableQueue<InputValue> GetInput()
        {
            return new ImmutableQueue<InputValue>(new InputValue());
        }

        /// <summary>
        /// Gets the mouse state.
        /// </summary>
        /// <returns>
        /// The mouse state.
        /// </returns>
        internal virtual MouseState GetMouseState()
        {
            return new MouseState();
        }

        /// <summary>
        /// Gets the picture count.
        /// </summary>
        /// <returns>
        /// The picture count.
        /// </returns>
        internal virtual int GetPictureCount()
        {
            return 0;
        }

        /// <summary>
        /// Gets the size of a picture.
        /// </summary>
        /// <param name="picture">
        /// The picture.
        /// </param>
        /// <returns>
        /// The picture size.
        /// </returns>
        internal virtual DisplayAreaSize GetPictureSize(int picture)
        {
            return new DisplayAreaSize();
        }

        /// <summary>
        /// Gets the release number of the picture resources.
        /// </summary>
        /// <returns>
        /// The release number of the picture resources.
        /// </returns>
        internal virtual int GetPicturesReleaseNumber()
        {
            return 0;
        }

        /// <summary>
        /// Gets a random seed value for the random number generator.
        /// </summary>
        /// <returns>
        /// The random seed.
        /// </returns>
        internal int GetRandomSeed()
        {
            return (int) DateTime.Now.Ticks;
        }

        /// <summary>
        /// Gets timed input.
        /// </summary>
        /// <returns>
        /// The timed input.
        /// </returns>
        internal virtual TimedInput GetTimedInput()
        {
            return new TimedInput(this.GetInput(), 0);
        }

        /// <summary>
        /// The zmachine has halted.
        /// </summary>
        internal virtual void Halted()
        {
            Done = true;
        }

        /// <summary>
        /// Indicates the input buffer is full.
        /// </summary>
        internal virtual void InputBufferFull()
        {
        }

        /// <summary>
        /// Determines if a picture is valid.
        /// </summary>
        /// <param name="picture">
        /// The picture number.
        /// </param>
        /// <returns>
        /// A value indicating whether the picture is valid.
        /// </returns>
        internal virtual bool IsValidPicture(int picture)
        {
            return false;
        }

        /// <summary>
        /// Loads pictures.
        /// </summary>
        /// <param name="pictures">
        /// The pictures to load.
        /// </param>
        internal virtual void LoadPictures(ImmutableStack<int> pictures)
        {
        }

        /// <summary>
        /// Loads a sound.
        /// </summary>
        /// <param name="sound">
        /// The sound to load.
        /// </param>
        internal virtual void LoadSound(int sound)
        {
        }

        /// <summary>
        /// Loads a story.
        /// </summary>
        /// <param name="story">
        /// The story.
        /// </param>
        /// <returns>
        /// A new Zmachine.
        /// </returns>
        internal bool LoadStory(ImmutableArray<byte> story)
        {
            this.zmachine = null;
            var randomNumberGenerator = this.NewRandomNumberGenerator();
            if (randomNumberGenerator != null && story != null && story.Length > 0)
            {
                this.zmachine = this.NewZmachine(story, randomNumberGenerator);
            }

            return this.zmachine != null;
        }

        /// <summary>
        /// Start receiving input from the selected input stream.
        /// </summary>
        /// <param name="inputStream">
        /// The input stream.
        /// </param>
        internal virtual void OpenInputStream(InputStream inputStream)
        {
        }

        /// <summary>
        /// Restores part of the zmachine memory.
        /// </summary>
        /// <param name="defaultName">
        /// The default name of the file to restore.
        /// </param>
        /// <param name="promptForName">
        /// A value indicating whether to prompt for a file name.
        /// </param>
        /// <param name="restoreSize">
        /// The number of bytes to restore.
        /// </param>
        /// <returns>
        /// The restored memory.
        /// </returns>
        internal virtual ImmutableArray<byte> PartialRestore(string defaultName, bool promptForName, int restoreSize)
        {
            return null;
        }

        /// <summary>
        /// Saves part of the zmachine memory.
        /// </summary>
        /// <param name="defaultName">
        /// The default name of the file to save.
        /// </param>
        /// <param name="promptForName">
        /// A value indicating whether to prompt for a file name.
        /// </param>
        /// <param name="memory">
        /// The saved memory.
        /// </param>
        /// <returns>
        /// The number of bytes saved.
        /// </returns>
        internal virtual int PartialSave(string defaultName, bool promptForName, ImmutableArray<byte> memory)
        {
            return 0;
        }

        /// <summary>
        /// Plays a beep sound.
        /// </summary>
        /// <param name="beep">
        /// The beep sound.
        /// </param>
        internal virtual void PlayBeep(Beep beep)
        {
        }

        /// <summary>
        /// Plays a sound.
        /// </summary>
        /// <param name="sound">
        /// The sound.
        /// </param>
        /// <param name="volume">
        /// The volume.
        /// </param>
        internal virtual void PlaySound(int sound, byte volume)
        {
        }

        /// <summary>
        /// Releases a time marker.
        /// </summary>
        internal virtual void ReleaseTimeMarker()
        {
        }

        /// <summary>
        /// Removes a menu.
        /// </summary>
        /// <param name="menu">
        /// The menu to remove.
        /// </param>
        /// <returns>
        /// A value indicating whether the menu was removed.
        /// </returns>
        internal virtual bool RemoveMenu(int menu)
        {
            return false;
        }

        /// <summary>
        /// Resets any counts controlling MORE prompts.
        /// </summary>
        internal virtual void ResetMorePromptCounts()
        {
        }

        /// <summary>
        /// Restores a saved state.
        /// </summary>
        /// <returns>
        /// The restored state.
        /// </returns>
        internal virtual ZmachineSaveState Restore()
        {
            return new ZmachineSaveState();
        }

        /// <summary>
        /// Restores a saved state to undo the last input.
        /// </summary>
        /// <returns>
        /// The restored state.
        /// </returns>
        internal virtual ZmachineSaveState RestoreUndo()
        {
            return new ZmachineSaveState();
        }

        /// <summary>
        /// Saves the z-machne state.
        /// </summary>
        /// <param name="saveState">
        /// The save state.
        /// </param>
        /// <returns>
        /// A value indicating whether the save was successful.
        /// </returns>
        internal virtual bool Save(ZmachineSaveState saveState)
        {
            return false;
        }

        /// <summary>
        /// Saves the zmachine state so the next input can be undone if needed.
        /// </summary>
        /// <param name="saveState">
        /// The save state.
        /// </param>
        /// <returns>
        /// A value indicating whether the save was successful.
        /// </returns>
        internal virtual bool SaveUndo(ZmachineSaveState saveState)
        {
            return false;
        }

        /// <summary>
        /// Sets the display font.
        /// </summary>
        /// <param name="newFont">
        /// The new font.
        /// </param>
        /// <returns>
        /// A value indicating whether the font was set.
        /// </returns>
        internal bool SetFont(Font newFont)
        {
            return false;
        }

        /// <summary>
        /// Sets a time marker.
        /// </summary>
        internal virtual void SetTimeMarker()
        {
        }

        /// <summary>
        /// Determines if a sound is considered music.
        /// </summary>
        /// <param name="sound">
        /// The sound.
        /// </param>
        /// <returns>
        /// A value indicating whether the sound is considered music.
        /// </returns>
        internal virtual bool SoundIsMusic(int sound)
        {
            return false;
        }

        /// <summary>
        /// Determines whether the sound loops continuously.
        /// </summary>
        /// <param name="sound">
        /// The sound.
        /// </param>
        /// <returns>
        /// A value indicating whether the sound loops continuously.
        /// </returns>
        internal virtual bool SoundLoops(int sound)
        {
            return false;
        }

        /// <summary>
        /// Stops playing all sounds.
        /// </summary>
        internal virtual void StopAllSounds()
        {
        }

        /// <summary>
        /// Stops playing a sound.
        /// </summary>
        /// <param name="sound">
        /// The sound to stop.
        /// </param>
        internal virtual void StopSound(int sound)
        {
        }

        /// <summary>
        /// Unloads all sounds.
        /// </summary>
        internal virtual void UnloadAllSounds()
        {
        }

        /// <summary>
        /// Unloads a sound.
        /// </summary>
        /// <param name="sound">
        /// The sound to unload.
        /// </param>
        internal virtual void UnloadSound(int sound)
        {
        }

        /// <summary>
        /// Updates the status line for a scored game.
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
        internal virtual void UpdateStatusLineForScoredGame(string locationName, short score, short moves)
        {
        }

        /// <summary>
        /// Updates the status line for a timed game.
        /// </summary>
        /// <param name="locationName">
        /// The location name.
        /// </param>
        /// <param name="hours">
        /// The hours.
        /// </param>
        /// <param name="minutes">
        /// The minutes.
        /// </param>
        internal virtual void UpdateStatusLineForTimedGame(string locationName, short hours, short minutes)
        {
        }

        /// <summary>
        /// Writes any buffered text to the display.
        /// </summary>
        internal void WriteBufferedTextToDisplay()
        {

        }

        /// <summary>
        /// Writes to the display.
        /// </summary>
        /// <param name="text">
        /// The text to write.
        /// </param>
        /// <param name="writableArea">
        /// The writable area of the display.
        /// </param>
        /// <param name="scrollable">
        /// A value indicating whether the writable area is scrollable.
        /// </param>
        /// <param name="buffered">
        /// A value indicating whether the output is buffered to allow word wrapping at the end of each line.
        /// </param>
        /// <param name="textStyles">
        /// The text styles.
        /// </param>
        /// <param name="font">
        /// The font used.
        /// </param>
        /// <param name="foregroundColor">
        /// The foreground colour.
        /// </param>
        /// <param name="backgroundColor">
        /// The background colour.
        /// </param>
        internal void WriteToDisplay(string text, DisplayArea writableArea, bool scrollable, bool buffered, TextStyles textStyles, Font font, ColorStruct? foregroundColor, ColorStruct? backgroundColor)
        {

        }

        /// <summary>
        /// Writes to the input log.
        /// </summary>
        /// <param name="inputValues">
        /// The input values.
        /// </param>
        internal virtual void WriteToInputLog(ImmutableQueue<InputValue> inputValues)
        {
        }

        /// <summary>
        /// Writes to the input log.
        /// </summary>
        /// <param name="inputValues">
        /// The input values.
        /// </param>
        /// <param name="elapsedTime">
        /// The elapsed time.
        /// </param>
        internal virtual void WriteToInputLog(ImmutableQueue<InputValue> inputValues, long elapsedTime)
        {
        }

        /// <summary>
        /// Writes to the transcript.
        /// </summary>
        /// <param name="text">
        /// The text to write.
        /// </param>
        internal virtual void WriteToTranscript(string text)
        {
        }

        /// <summary>
        /// Creates a new random number generator.
        /// </summary>
        /// <returns>
        /// A new random number generator.
        /// </returns>
        /// <remarks>
        /// The default random number generator is an implementation of the Mersenne Twister algorithm. Overriding this method allows an alternate implementation.
        /// </remarks>
        protected virtual IRandomNumberGenerator NewRandomNumberGenerator()
        {
            return new MersenneTwister();
        }

        /// <summary>
        /// Creates a new Zmachine.
        /// </summary>
        /// <param name="story">
        /// The story.
        /// </param>
        /// <param name="randomNumberGenerator">
        /// The random number generator.
        /// </param>
        /// <returns>
        /// A new Zmachine.
        /// </returns>
        /// <remarks>
        /// Versions 1 through 8 are supported. Overriding this method allows support for new versions or custom implementations.
        /// </remarks>
        protected virtual IZmachine NewZmachine(ImmutableArray<byte> story, IRandomNumberGenerator randomNumberGenerator)
        {
            switch (story[0])
            {
                case 1:
                    return new ZmachineV1(this, story, randomNumberGenerator);
                case 2:
                    return new ZmachineV2(this, story, randomNumberGenerator);
                case 3:
                    return new ZmachineV3(this, story, randomNumberGenerator);
                case 4:
                    return new ZmachineV4(this, story, randomNumberGenerator);
                case 5:
                    return new ZmachineV5(this, story, randomNumberGenerator);
                case 6:
                    return new ZmachineV6(this, story, randomNumberGenerator);
                case 7:
                    return new ZmachineV7(this, story, randomNumberGenerator);
                case 8:
                    return new ZmachineV8(this, story, randomNumberGenerator);
            }

            return null;
        }
    }
}