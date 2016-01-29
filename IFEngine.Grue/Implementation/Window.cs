// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Window.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   Display window.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// Display window.
    /// </summary>
    internal sealed class Window
    {
        /// <summary>
        /// The window id.
        /// </summary>
        private readonly ushort id;

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        /// <param name="id">
        /// The window id.
        /// </param>
        /// <param name="fontHeight">
        /// Font height.
        /// </param>
        /// <param name="fontWidth">
        /// Font width.
        /// </param>
        /// <param name="background">
        /// Background colour.
        /// </param>
        /// <param name="foreground">
        /// Foreground colour.
        /// </param>
        internal Window(ushort id, byte fontHeight, byte fontWidth, ColorStruct background, ColorStruct foreground)
        {
            this.id = id;
            this.Attributes = WindowAttributes.BufferingEnabled;
            this.Column = 1;
            this.Row = 1;
            this.CursorColumn = 1;
            this.CursorRow = 1;
            this.FontNumber = 1;
            this.FontHeight = fontHeight;
            this.FontWidth = fontWidth;
            this.Background = background;
            this.Foreground = foreground;
        }

        /// <summary>
        /// Gets AbsoluteCursorColumn.
        /// </summary>
        /// <value>
        /// The absolute cursor column.
        /// </value>
        internal ushort AbsoluteCursorColumn
        {
            get
            {
                return (ushort)(this.Column + this.CursorColumn - 1);
            }
        }

        /// <summary>
        /// Gets AbsoluteCursorRow.
        /// </summary>
        /// <value>
        /// The absolute cursor row.
        /// </value>
        internal ushort AbsoluteCursorRow
        {
            get
            {
                return (ushort)(this.Row + this.CursorRow - 1);
            }
        }

        /// <summary>
        /// Gets or sets Attributes.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        internal ushort Attributes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Background.
        /// </summary>
        /// <value>
        /// The background.
        /// </value>
        internal ColorStruct Background
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the column number.
        /// </summary>
        /// <value>
        /// The column.
        /// </value>
        internal ushort Column
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets CursorColumn.
        /// </summary>
        /// <value>
        /// The cursor column.
        /// </value>
        internal ushort CursorColumn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets CursorRow.
        /// </summary>
        /// <value>
        /// The cursor row.
        /// </value>
        internal ushort CursorRow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets FontHeight.
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
        /// Gets or sets FontNumber.
        /// </summary>
        /// <value>
        /// The font number.
        /// </value>
        internal ushort FontNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets FontWidth.
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
        /// Gets or sets Foreground.
        /// </summary>
        /// <value>
        /// The foreground.
        /// </value>
        internal ColorStruct Foreground
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        internal ushort Height
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the window id.
        /// </summary>
        /// <value>
        /// The window id.
        /// </value>
        internal ushort Id
        {
            get
            {
                return this.id;
            }
        }

        /// <summary>
        /// Gets or sets InterruptCountdown.
        /// </summary>
        /// <value>
        /// The interrupt countdown.
        /// </value>
        internal ushort InterruptCountdown
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets InterruptRoutine.
        /// </summary>
        /// <value>
        /// The interrupt routine.
        /// </value>
        internal ushort InterruptRoutine
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets LeftMargin.
        /// </summary>
        /// <value>
        /// The left margin.
        /// </value>
        internal ushort LeftMargin
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets LineCount.
        /// </summary>
        /// <value>
        /// The line count.
        /// </value>
        internal ushort LineCount
        {
            // -999 = 64537
            get;
            set;
        }

        /// <summary>
        /// Gets or sets RightMargin.
        /// </summary>
        /// <value>
        /// The right margin.
        /// </value>
        internal ushort RightMargin
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the row number.
        /// </summary>
        /// <value>
        /// The row number.
        /// </value>
        internal ushort Row
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets TextStyle.
        /// </summary>
        /// <value>
        /// The text style.
        /// </value>
        internal ushort TextStyle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        internal ushort Width
        {
            get;
            set;
        }
    }
}