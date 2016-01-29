// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZmachineV6.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   The zmachine version 6.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// The zmachine version 6.
    /// </summary>
    internal class ZmachineV6 : ZmachineV5, IZmachine
    {
        /// <summary>
        /// Custom colours.
        /// </summary>
        private readonly ColorStruct?[] customColours = new ColorStruct?[240];

        /// <summary>
        /// The custom colour number to assign.
        /// </summary>
        private byte nextCustomColourNumber = 16;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZmachineV6"/> class.
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
        internal ZmachineV6(FrontEnd frontEnd, ImmutableArray<byte> story, IRandomNumberGenerator random) : base(frontEnd, story, random)
        {
        }

        /// <summary>
        /// Gets the font height header address.
        /// </summary>
        /// <value>
        /// The font height header address.
        /// </value>
        protected override int FontHeightHeaderAddress
        {
            get
            {
                return base.FontWidthHeaderAddress;
            }
        }

        /// <summary>
        /// Gets the font width header address.
        /// </summary>
        /// <value>
        /// The font width header address.
        /// </value>
        protected override int FontWidthHeaderAddress
        {
            get
            {
                return base.FontHeightHeaderAddress;
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
                return 8;
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
                // todo: V6 sets text style per display window.
                return base.TextStyles;
            }
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
                // todo: fix.
                return base.WritableArea;
            }
        }

        /// <summary>
        /// Gets or sets the mouse window.
        /// </summary>
        /// <value>
        /// The mouse window.
        /// </value>
        private DisplayArea MouseWindow
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the display should be redrawn.
        /// </summary>
        void IZmachine.RedrawDisplay()
        {
            lock (this.Memory)
            {
                this.WriteHeaderFlag(Memory.Flags2LowByteAddress, HeaderFlags.Flags2LowDisplayRedrawNeeded, true);
            }
        }

        /// <summary>
        /// Closes a memory stream.
        /// </summary>
        /// <param name="memoryStream">
        /// The memory stream.
        /// </param>
        protected override void CloseMemoryStream(MemoryStream memoryStream)
        {
            // todo: Get width from where?
            base.CloseMemoryStream(memoryStream);
            ushort widthOfStream = 0;
            this.Memory.WriteWord(48, widthOfStream);
        }

        /// <summary>
        /// Gets colour from colour number.
        /// </summary>
        /// <param name="number">
        /// Colour number.
        /// </param>
        /// <returns>
        /// True colour.
        /// </returns>
        protected ColorStruct ColourNumberToColour(byte number)
        {
            var colour = this.customColours[number];
            if (colour == null)
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidColor, "Tried to use undefined colour (" + number + ").");
            }

            return colour ?? ColorStruct.Black;
        }

        /// <summary>
        /// Discovers the zmachine capabilities.
        /// </summary>
        protected override void DiscoverCapabilities()
        {
            base.DiscoverCapabilities();
            if (this.FrontEnd.TransparencyAvailable)
            {
                var flags3 = this.ReadHeaderExtensionTableEntry(HeaderExtension.Flags3);
                this.WriteHeaderExtensionTableEntry(HeaderExtension.Flags3, (ushort)(flags3 & 1));
            }
        }

        /// <summary>
        /// Extended operation 13.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void ExtendedOperation13()
        {
            // todo: true_colour
            // ushort window = this.CurrentWindow;
            // ushort foreground = operands.first;
            // ushort background = operands.second;
            // if (window >= this.Windows.Length)
            // {
            // this.FrontEnd.ErrorNotification(this, ErrorCondition.InvalidWindow, "Tried to set true colour on invalid window (" + window + ").");
            // return;
            // }
            // Window w = this.Windows[window];
            // if (background < 32768)
            // {
            // w.Background = FifteenBitToColour(background);
            // }
            // else
            // {
            // switch (background)
            // {
            // case 65535:
            // w.Background = this.ColourNumberToColour(this.Memory.ReadByte(MemoryAddress.DefaultBackground));
            // break;
            // case 65533:
            // w.Background = this.GetPixelColour(w.AbsoluteCursorRow, w.AbsoluteCursorColumn);
            // break;
            // case 65532:
            // break;
            // }
            // }
            // if (foreground < 32768)
            // {
            // w.Foreground = FifteenBitToColour(foreground);
            // }
            // else
            // {
            // switch (foreground)
            // {
            // case 65535:
            // w.Foreground = this.ColourNumberToColour(this.Memory.ReadByte(MemoryAddress.DefaultForeground));
            // break;
            // case 65533:
            // w.Foreground = this.GetPixelColour(w.AbsoluteCursorRow, w.AbsoluteCursorColumn);
            // break;
            // case 65532:
            // break;
            // }
            // }
        }

        /// <summary>
        /// Extended operation 16.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void ExtendedOperation16()
        {
            // todo: move_window
            // ushort window = operands.first;
            // ushort row = operands.second;
            // ushort column = operands.third;
            // if (window == 65533)
            // {
            // window = this.CurrentWindow;
            // }
            // if (window >= this.Windows.Length)
            // {
            // this.FrontEnd.ErrorNotification(this, ErrorCondition.InvalidWindow, "Tried to move invalid window (" + window + ").");
            // return;
            // }
            // Window w = this.Windows[window];
            // w.Row = row;
            // w.Column = column;
        }

        /// <summary>
        /// Extended operation 17.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void ExtendedOperation17()
        {
            // todo: resize_window
            // ushort window = operands.first;
            // ushort height = operands.second;
            // ushort width = operands.third;
            // if (window == 65533)
            // {
            // window = this.CurrentWindow;
            // }
            // if (window >= this.Windows.Length)
            // {
            // this.FrontEnd.ErrorNotification(this, ErrorCondition.InvalidWindow, "Tried to resize invalid window (" + window + ").");
            // return;
            // }
            // Window w = this.Windows[window];
            // w.Height = height;
            // w.Width = width;
            // if (w.CursorRow <= height && w.CursorColumn <= width)
            // {
            // return;
            // }
            // w.CursorRow = 1;
            // w.CursorColumn = (ushort)(w.LeftMargin + 1);
        }

        /// <summary>
        /// Extended operation 18.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void ExtendedOperation18()
        {
            // todo: set_window_style. How to deal with wrapping? If we limit the writable area, how does that affect more prompts and writing of newlines?
            // ushort window = operands.first;
            // ushort flags = operands.second;
            // ushort operation = operands.third;
            // if (window == 65533)
            // {
            // window = this.CurrentWindow;
            // }
            // if (window >= this.Windows.Length)
            // {
            // this.FrontEnd.ErrorNotification(this, ErrorCondition.InvalidWindow, "Tried to set style on invalid window (" + window + ").");
            // return;
            // }
            // Window w = this.Windows[window];
            // switch (operation)
            // {
            // case 0:
            // w.Attributes = flags;
            // return;
            // case 1:
            // w.Attributes |= flags;
            // return;
            // case 2:
            // w.Attributes &= (ushort)(~flags);
            // return;
            // case 3:
            // w.Attributes ^= flags;
            // return;
            // }
            // this.FrontEnd.ErrorNotification(this, ErrorCondition.InvalidWindowStyleOperation, "Tried to set window style using an invalid oepration (" + operation + ").");
        }

        /// <summary>
        /// Extended operation 19.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void ExtendedOperation19()
        {
            this.Store(this.ReadProperty(this.Operands.First, this.Operands.Second));
        }

        /// <summary>
        /// Extended operation 20.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void ExtendedOperation20()
        {
            // Todo : scroll_window
            // ushort window = operands.first;
            // if (window == 65533)
            // {
            // window = this.CurrentWindow;
            // }
            // if (window < this.Windows.Length)
            // {
            // return;
            // }
            // this.FrontEnd.ErrorNotification(this, ErrorCondition.InvalidWindow, "Tried to scroll invalid window (" + window + ").");
        }

        /// <summary>
        /// Extended operation 21.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void ExtendedOperation21()
        {
            var itemCount = this.Operands.First;
            if (this.Operands.Count > 1)
            {
                var userStackAddress = this.Operands.Second;
                var freeSlots = this.Memory.ReadWord(userStackAddress);
                this.Memory.WriteWord(userStackAddress, (ushort)(freeSlots + itemCount));
            }
            else
            {
                while (itemCount-- > 0)
                {
                    this.CallStack.Pop();
                }
            }
        }

        /// <summary>
        /// Extended operation 22.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void ExtendedOperation22()
        {
            var address = this.Operands.First;
            var mouseState = this.FrontEnd.GetMouseState();
            this.Memory.WriteWord(address, (ushort)(mouseState.CursorPosition.Row + 1));
            this.Memory.WriteWord(address + 2, (ushort)(mouseState.CursorPosition.Column + 1));
            this.Memory.WriteWord(address + 4, (ushort)mouseState.ButtonsPressed);
            this.Memory.WriteWord(address + 6, (ushort)((mouseState.MenuSelected << 8) + mouseState.MenuItemSelected));
        }

        /// <summary>
        /// Extended operation 23.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void ExtendedOperation23()
        {
            // todo: Mouse_window
            // if (operands.first == 65535)
            // {
            // if (!this.MouseWindow.AreaIsZero)
            // {
            // this.FrontEnd.FreeMouse(this);
            // this.MouseWindow = new Rectangle();
            // }
            // }
            // else
            // {
            // Rectangle windowDimensions = this.WindowDimensions(operands.first);
            // if (!windowDimensions.AreaIsZero)
            // {
            // this.MouseWindow = windowDimensions;
            // this.FrontEnd.ConfineMouse(this, new Rectangle(this.MouseWindow.Row - 1, this.MouseWindow.Column - 1, this.MouseWindow.Height, this.MouseWindow.Width));
            // }
            // }
        }

        /// <summary>
        /// Extended operation 24.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void ExtendedOperation24()
        {
            var value = this.Operands.First;
            var userStackAddress = this.Operands.Second;
            var freeSlots = this.Memory.ReadWord(userStackAddress);
            var hasFreeSlot = freeSlots > 0;
            if (hasFreeSlot)
            {
                this.Memory.WriteWord(userStackAddress + (freeSlots * 2), value);
                this.Memory.WriteWord(userStackAddress, --freeSlots);
            }

            this.Branch(hasFreeSlot);
        }

        /// <summary>
        /// Extended operation 25.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void ExtendedOperation25()
        {
            // todo: set_window_property
            // ushort window = operands.first;
            // ushort property = operands.second;
            // ushort value = operands.third;
            // if (window == 65533)
            // {
            // window = this.CurrentWindow;
            // }
            // if (window >= this.Windows.Length)
            // {
            // this.FrontEnd.ErrorNotification(this, ErrorCondition.InvalidWindow, "Tried to write property (" + property + ") on invalid window (" + window + ").");
            // return;
            // }
            // Window w = this.Windows[window];
            // switch (property)
            // {
            // case 0:
            // w.Row = value;
            // return;
            // case 1:
            // w.Column = value;
            // return;
            // case 2:
            // w.Height = value;
            // return;
            // case 3:
            // w.Width = value;
            // return;
            // case 4:
            // w.CursorRow = value;
            // return;
            // case 5:
            // w.CursorColumn = value;
            // return;
            // case 6:
            // w.LeftMargin = value;
            // return;
            // case 7:
            // w.RightMargin = value;
            // return;
            // case 8:
            // w.InterruptRoutine = value;
            // return;
            // case 9:
            // w.InterruptCountdown = value;
            // return;
            // case 10:
            // w.TextStyle = value;
            // return;
            // case 11:
            // var background = (byte)(value >> 8);
            // var foreground = (byte)value;
            // w.Background = this.ColourNumberToColour(background);
            // w.Foreground = this.ColourNumberToColour(foreground);
            // return;
            // case 12:
            // w.FontNumber = value;
            // return;
            // case 13:
            // w.FontHeight = (byte)(value >> 8);
            // w.FontWidth = (byte)value;
            // return;
            // case 14:
            // w.Attributes = value;
            // return;
            // case 15:
            // w.LineCount = value;
            // return;
            // }
            // this.FrontEnd.ErrorNotification(this, ErrorCondition.InvalidWindowProperty, "Tried to write invalid property (" + property + ") on window (" + window + ").");
        }

        /// <summary>
        /// Extended operation 26.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void ExtendedOperation26()
        {
            // todo: Flush buffer.
            var startingAddress = this.Operands.First;
            var address = startingAddress;
            var cursorStart = this.FrontEnd.CursorPosition;
            var row = 0;
            ushort characterCount;
            while (0 != (characterCount = this.Memory.ReadWord(address)))
            {
                if (address != startingAddress)
                {
                    this.FrontEnd.CursorPosition = new DisplayPosition(cursorStart.Column, ++row + cursorStart.Row);
                }

                address += 2;
                this.Write(this.ReadZsciiFromMemory(address, characterCount));
                address += characterCount;
            }
        }

        /// <summary>
        /// Extended operation 27.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void ExtendedOperation27()
        {
            // todo: Clean this up.
            var menuNumber = this.Operands.First;
            if (menuNumber < 3)
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidMenu, "Menus 0, 1, and 2 are reserved.");
                this.Branch(false);
                return;
            }

            int entryTableAddress = this.Operands.Second;
            if (entryTableAddress == 0)
            {
                this.Branch(this.FrontEnd.RemoveMenu(menuNumber));
                return;
            }

            var entryCount = this.Memory.ReadWord(entryTableAddress);
            if (entryCount < 2)
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.InvalidMenu, "At least two entries are required to create a menu.");
                this.Branch(false);
                return;
            }

            entryTableAddress += 2;
            var entries = new string[entryCount];
            for (ushort entryNumber = 0; entryNumber < entryCount; entryNumber++)
            {
                entries[entryNumber] = this.ZsciiToUnicode(this.ReadZsciiFromMemory(this.Memory.ReadWord(entryTableAddress)));
                entryTableAddress += 2;
            }

            var menu = new MenuEntry(entries[0], menuNumber);
            var itemCount = (byte)(entries.Length - 1);
            var items = new MenuEntry[itemCount];
            for (byte itemNumber = 1; itemNumber <= itemCount; itemNumber++)
            {
                items[itemNumber - 1] = new MenuEntry(entries[itemNumber], itemNumber);
            }

            this.Branch(this.FrontEnd.AddMenu(menu, new ImmutableArray<MenuEntry>(items)));
        }

        /// <summary>
        /// Extended operation 28.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void ExtendedOperation28()
        {
            var pictureTable = this.Operands.First;
            var pictureCount = this.Memory.ReadWord(pictureTable);
            ImmutableStack<int> pictures = null;
            while (pictures.Count() < pictureCount)
            {
                pictureTable += 2;
                pictures = pictures.Add(this.Memory.ReadWord(pictureTable));
            }

            this.FrontEnd.LoadPictures(pictures);
        }

        /// <summary>
        /// Extended operation 29.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void ExtendedOperation29()
        {
            // todo: display_buffer_mode
            // bool previousMode = this.DisplayBufferEnabled;
            // switch (operands.first)
            // {
            // case 0:
            // this.DisplayBufferEnabled = false;
            // break;
            // case 1:
            // this.DisplayBufferEnabled = true;
            // break;
            // case 65535:
            // this.DisplayBufferEnabled = false;
            // this.UpdateDisplay();
            // this.DisplayBufferEnabled = previousMode;
            // break;
            // }
            // this.Store((ushort)(previousMode ? 1 : 0));
        }

        /// <summary>
        /// Extended operation 4.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void ExtendedOperation4()
        {
        }

        /// <summary>
        /// Extended Operation 5.
        /// </summary>
        /// <remarks>
        /// Infocom name: DISPLAY
        ///   Inform name: draw_picture
        ///   This operation draws a picture on the display.
        ///   The first operand is the resource number of the picture.
        ///   The second and third operands are the row and column offsets of the picture's top left corner with respect to the current display window's top left corner.
        ///   A value of zero for either the row or column offset is special and indicates that the current cursor value is used instead.
        ///   The picture must be cropped to the current display window boundaries.
        /// </remarks>
        protected override void ExtendedOperation5()
        {
            // todo: Calculate picture origin and calculate crop area based on current window and screen size.
            this.FrontEnd.DrawPicture(this.Operands.First, new DisplayArea());
        }

        /// <summary>
        /// Extended Operation 6.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void ExtendedOperation6()
        {
            var picture = this.Operands.First;
            var address = this.Operands.Second;
            if (picture != 0)
            {
                if (this.FrontEnd.IsValidPicture(picture))
                {
                    var pictureSize = this.FrontEnd.GetPictureSize(picture);
                    this.Memory.WriteWord(address, (ushort)pictureSize.Height);
                    this.Memory.WriteWord(address + 2, (ushort)pictureSize.Width);
                    this.Branch(true);
                }
                else
                {
                    this.FrontEnd.ErrorNotification(ErrorCondition.InvalidPicture, "PICTURE DATA called with invalid picture number (" + picture + ").");
                    this.Branch(false);
                }
            }
            else
            {
                // todo: The zip manual says we should get the highest picture id number, not the picture count, which contradicts the zmachine standard.
                var pictureCount = this.FrontEnd.GetPictureCount();
                this.Memory.WriteWord(address, (ushort)pictureCount);
                this.Memory.WriteWord(address + 2, (ushort)this.FrontEnd.GetPicturesReleaseNumber());
                this.Branch(pictureCount > 0);
            }
        }

        /// <summary>
        /// Extended Operation 7.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void ExtendedOperation7()
        {
            // Todo : fix. also clipped to current window
            // ushort picture = operands.first;
            // PictureSize pictureSize = this.FrontEnd.GetPictureSize(this, picture);
            // if (!pictureSize.ValidPicture)
            // {
            // this.FrontEnd.ErrorNotification(this, ErrorCondition.InvalidPicture, "ERASE PICTURE called with invalid picture number (" + picture + ").");
            // return;
            // }
            // ushort row = operands.second;
            // ushort column = operands.third;
            // Window w = this.Windows[this.CurrentWindow];
            // row = row == 0 ? w.AbsoluteCursorRow : (ushort)(w.Row + row - 1);
            // column = column == 0 ? w.AbsoluteCursorColumn : (ushort)(w.Column + column - 1);
            // this.ColourArea(row, column, (ushort)pictureSize.Height, (ushort)pictureSize.Width, w.Background);
        }

        /// <summary>
        /// Extended Operation 8.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void ExtendedOperation8()
        {
            // todo: according to zip manual, setting margin on a non-wrapping window does nothing. it must be executed before any text is buffered for the current line.
            // ushort window = operands.third;
            // ushort left = operands.first;
            // ushort right = operands.second;
            // if (window == 65533)
            // {
            // window = this.CurrentWindow;
            // }
            // if (window >= this.Windows.Length)
            // {
            // this.FrontEnd.ErrorNotification(this, ErrorCondition.InvalidWindow, "Tried to set margins on invalid window (" + window + ").");
            // return;
            // }
            // Window w = this.Windows[window];
            // w.LeftMargin = left;
            // w.RightMargin = right;
            // if (w.CursorColumn <= w.LeftMargin || w.CursorColumn > w.Width - w.RightMargin)
            // {
            // w.CursorColumn = (ushort)(w.LeftMargin + 1);
            // }
        }

        /// <summary>
        /// Initializes the call stack.
        /// </summary>
        /// <param name="initialProgramCounter">
        /// The initial program counter.
        /// </param>
        protected override void InitializeCallStack(ushort initialProgramCounter)
        {
            var programCounter = this.UnpackRoutineAddress(initialProgramCounter);
            var localVariableCount = this.Memory.ReadByte(programCounter++);
            this.CallStack.Initialize(programCounter, localVariableCount);
        }

        /// <summary>
        /// Opens a memory stream.
        /// </summary>
        /// <returns>
        /// The memory stream.
        /// </returns>
        protected override MemoryStream OpenMemoryStream()
        {
            // todo: V6 - zip manual says if window argument is not supplied, we zero out the stream 3 width word in the header here and keep track of the pixel width of each character written.
            ushort width = 0;
            if (this.Operands.Count > 2)
            {
                var window = this.Operands.Third;
                var windowNumber = (short)window;
                if (windowNumber < 0)
                {
                    width = (ushort)-windowNumber;
                }

                // else
                // {
                // Rectangle r = this.WindowDimensions(window);
                // if (!r.AreaIsZero)
                // {
                // width = (ushort)r.Width;
                // }
                // }
            }

            return new MemoryStream(this.Operands.Second, width);
        }

        /// <summary>
        /// Operation 233.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void Operation233()
        {
            if (this.Operands.Count == 0)
            {
                this.Store(this.CallStack.Pop());
                return;
            }

            var userStack = this.Operands.First;
            var freeSlots = (ushort)(this.Memory.ReadWord(userStack) + 1);
            var value = this.Memory.ReadWord(userStack + (freeSlots * 2));
            this.Memory.WriteWord(userStack, freeSlots);
            this.Store(value);
        }

        /// <summary>
        /// Operation 238.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void Operation238()
        {
            // todo: erase_line needs current colour
            // ushort value = operands.first;
            // Window w = this.Windows[this.CurrentWindow];
            // var maxWidth = (ushort)(1 + w.Width - w.CursorColumn);
            // if (value != 1)
            // {
            // maxWidth -= w.RightMargin;
            // if (maxWidth > --value)
            // {
            // maxWidth = value;
            // }
            // }
            // this.ColourArea(w.AbsoluteCursorRow, w.AbsoluteCursorColumn, 1, maxWidth, w.Background);
        }

        /// <summary>
        /// Operation 239.
        /// </summary>
        /// <remarks>
        /// Infocom name: FIX_THIS
        ///   Inform name: FIX_THIS.
        /// </remarks>
        protected override void Operation239()
        {
            // todo: set_cursor
            // ushort row = operands.first;
            // ushort column = operands.second;
            // ushort window = operands.Count == 3 ? operands.third : this.CurrentWindow;
            // if (window == 65533)
            // {
            // window = this.CurrentWindow;
            // }
            // if (window >= this.Windows.Length)
            // {
            // this.FrontEnd.ErrorNotification(this, ErrorCondition.InvalidWindow, "Tried to set cursor on invalid window (" + window + ").");
            // return;
            // }
            // Window w = this.Windows[window];
            // if (row == 65535)
            // {
            // this.CharacterModeDisplay.CursorVisible = false;
            // return;
            // }
            // if (row == 65534)
            // {
            // this.CharacterModeDisplay.CursorVisible = true;
            // return;
            // }
            // if (row > w.Height || column > w.Width)
            // {
            // return;
            // }
            // if (column <= w.LeftMargin || column > w.Width - w.RightMargin)
            // {
            // column = (ushort)(w.LeftMargin + 1);
            // }
            // w.CursorRow = row;
            // w.CursorColumn = column;
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
            // todo: if the colour header bit is not set, this does nothing according to zip manual.
            // ushort window = this.CurrentWindow;
            // if (operands.Count == 3 && operands.third != 65533)
            // {
            // window = operands.third;
            // }
            // ushort foreground = operands.first;
            // ushort background = operands.second;
            // if (window >= this.Windows.Length)
            // {
            // this.FrontEnd.ErrorNotification(this, ErrorCondition.InvalidWindow, "Tried to set colour on invalid window (" + window + ").");
            // return;
            // }
            // Window w = this.Windows[window];
            // if (background != 0)
            // {
            // if (background > 255)
            // {
            // if (background == 65535)
            // {
            // w.Background = this.GetPixelColour(w.AbsoluteCursorRow, w.AbsoluteCursorColumn);
            // }
            // }
            // else
            // {
            // switch (background)
            // {
            // case 1:
            // w.Background = this.ColourNumberToColour(this.Memory.ReadByte(MemoryAddress.DefaultBackground));
            // break;
            // case 15:
            // break;
            // default:
            // w.Background = this.ColourNumberToColour((byte)background);
            // break;
            // }
            // }
            // }
            // if (foreground == 0)
            // {
            // return;
            // }
            // if (foreground > 255)
            // {
            // if (foreground == 65535)
            // {
            // w.Foreground = this.GetPixelColour(w.AbsoluteCursorRow, w.AbsoluteCursorColumn);
            // }
            // }
            // else
            // {
            // switch (foreground)
            // {
            // case 1:
            // w.Foreground = this.ColourNumberToColour(this.Memory.ReadByte(MemoryAddress.DefaultForeground));
            // break;
            // case 15:
            // break;
            // default:
            // w.Foreground = this.ColourNumberToColour((byte)foreground);
            // break;
            // }
            // }
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
        protected override bool ProcessMouseClick(MouseClick mouseClick)
        {
            return this.MouseWindow.ContainsPosition(mouseClick.ClickPosition) && base.ProcessMouseClick(mouseClick);
        }

        /// <summary>
        /// Releases external resources used by the zmachine.
        /// </summary>
        protected override void ReleaseExternalResources()
        {
            // todo: moving or resizing a window must resize the restricted mouse area according to the zip manual!
            // todo: mouse should be restricted to window 1 at startup according to zip manual. That means we need to do this in init?
            // Todo: Should menus be reset as well? Test Journey for Mac on Infocom terp.
            base.ReleaseExternalResources();
            this.FrontEnd.FreeMouse();
        }

        /// <summary>
        /// Restarts the zmachine.
        /// </summary>
        protected override void Restart()
        {
            // todo: reset custom colours.
            this.nextCustomColourNumber = 16;
            this.MouseWindow = new DisplayArea();
            base.Restart();
        }

        /// <summary>
        /// Sets the header flags.
        /// </summary>
        protected override void SetHeaderFlags()
        {
            base.SetHeaderFlags();
            const byte Flags2HighByteAddress = 16;
            this.WriteHeaderFlag(Flags2HighByteAddress, HeaderFlags.Flags2HighMenusAvailable, this.FrontEnd.MenusAvailable);
            this.WriteHeaderFlag(Flags1Address, HeaderFlags.Flags1PicturesAvailable, this.FrontEnd.PicturesAvailable);
        }

        /// <summary>
        /// Sets the header flags for sound.
        /// </summary>
        /// <remarks>
        /// Version 6 repurposed the version 3 display split flag to indicate sound support. It also continued to use the flag from previous versions.
        /// </remarks>
        protected override void SetHeaderFlagsForSound()
        {
            this.WriteHeaderFlag(Flags1Address, HeaderFlags.Flags1SoundsAvailable, this.FrontEnd.SoundsAvailable);
            base.SetHeaderFlagsForSound();
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
            var routinesoffset = this.Memory.ReadWord(40);
            return (packedAddress * 4) + (routinesoffset * 8);
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
            var stringsOffset = this.Memory.ReadWord(42);
            return (packedAddress * 4) + (stringsOffset * 8);
        }

        /// <summary>
        /// Determines if a mouse click type is valid.
        /// </summary>
        /// <param name="zsciiCharacter">
        /// The zscii character.
        /// </param>
        /// <returns>
        /// A value indicating whether the mouse click type is valid.
        /// </returns>
        protected override bool ValidMouseClick(Zscii zsciiCharacter)
        {
            return zsciiCharacter == Zscii.MenuClick || base.ValidMouseClick(zsciiCharacter);
        }

        /// <summary>
        /// Writes zscii text to a memory stream if one is open.
        /// </summary>
        /// <param name="zsciiText">
        /// The zscii text.
        /// </param>
        protected override void WriteToMemoryStream(ImmutableStack<Zscii> zsciiText)
        {
            // todo: Is it correct to fill in zeros after the text? It seems to fix Arthur.
            // int finalAddress = address + (memoryStream.Width / 8); // Todo :  fix - need current font width
            // if (base.WriteToMemoryStream(text))
            // {
            // this.Memory.WriteWord(MemoryAddress.MemoryStreamWidthUnits, (ushort)(memoryStream.CharacterCount * 8)); // Todo :  fix - need current font width
            // }
        }

        /// <summary>
        /// Writes to the transcript.
        /// </summary>
        /// <param name="text">
        /// The text to write.
        /// </param>
        protected override void WriteToTranscript(string text)
        {
            // todo: write to transcript
            // if ((this.Windows[this.CurrentWindow].Attributes & WindowAttributes.ScriptingEnabled) == WindowAttributes.ScriptingEnabled)
            // {
            // base.WriteToTranscript(unicode);
            // }
        }

        /// <summary>
        /// Converts true colour to 15-bit.
        /// </summary>
        /// <param name="colour">
        /// True colour.
        /// </param>
        /// <returns>
        /// 15-bit colour.
        /// </returns>
        protected static short ColorToFifteenBit(ColorStruct colour)
        {
            return (byte)((colour.RedComponent >> 3) + ((colour.GreenComponent >> 3) << 5) + ((colour.BlueComponent >> 3) << 10));
        }

        /// <summary>
        /// Get colour number from colour.
        /// </summary>
        /// <param name="colour">
        /// True colour.
        /// </param>
        /// <returns>
        /// Colour number.
        /// </returns>
        protected byte ColorToColorNumber(ColorStruct colour)
        {
            for (byte colourNumber = 0; colourNumber < this.customColours.Length; colourNumber++)
            {
                var definedColour = this.customColours[colourNumber];
                if (definedColour.HasValue && colour.Equals(definedColour.Value))
                {
                    return colourNumber;
                }
            }

            var result = this.nextCustomColourNumber;
            this.customColours[result] = colour;
            if (++this.nextCustomColourNumber == 0)
            {
                this.nextCustomColourNumber = 16;
            }

            return result;
        }

        /// <summary>
        /// Reads a window property.
        /// </summary>
        /// <param name="window">
        /// Window number.
        /// </param>
        /// <param name="property">
        /// Property number.
        /// </param>
        /// <returns>
        /// Window property value.
        /// </returns>
        private ushort ReadProperty(ushort window, ushort property)
        {
            // todo: read property
            // if (this.Memory.ReadByte(MemoryAddress.Version) == 6 && window == 65533)
            // {
            // window = this.CurrentWindow;
            // }
            // if (window >= this.Windows.Length)
            // {
            // this.FrontEnd.ErrorNotification(this, ErrorCondition.InvalidWindow, "Tried to read property (" + property + ") on invalid window (" + window + ").");
            // return 0;
            // }
            // Window w = this.Windows[window];
            // switch (property)
            // {
            // case 0:
            // return w.Row;
            // case 1:
            // return w.Column;
            // case 2:
            // return w.Height;
            // case 3:
            // return w.Width;
            // case 4:
            // return w.CursorRow;
            // case 5:
            // return w.CursorColumn;
            // case 6:
            // return w.LeftMargin;
            // case 7:
            // return w.RightMargin;
            // case 8:
            // return w.InterruptRoutine;
            // case 9:
            // return w.InterruptCountdown;
            // case 10:
            // return w.TextStyle;
            // case 11:
            // byte background = this.ColourToColourNumber(w.Background);
            // byte foreground = this.ColourToColourNumber(w.Foreground);
            // return (ushort)((background << 8) + foreground);
            // case 12:
            // return w.FontNumber;
            // case 13:
            // return (ushort)((w.FontHeight << 8) + w.FontWidth);
            // case 14:
            // return w.Attributes;
            // case 15:
            // return w.LineCount;
            // case 16:
            // return ColourToFifteenBit(w.Foreground);
            // case 17:
            // return ColourToFifteenBit(w.Background);
            // }
            // this.FrontEnd.ErrorNotification(this, ErrorCondition.InvalidWindowProperty, "Tried to read invalid property (" + property + ") on window (" + window + ").");
            return 0;
        }
    }
}