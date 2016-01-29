// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeaderFlags.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   Header flag bitmasks.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    using System;

    /// <summary>
    /// Header flag bitmasks.
    /// </summary>
    [Flags]
    internal enum HeaderFlags
    {
        /// <summary>
        /// Bold style is available.
        /// </summary>
        Flags1BoldStyleAvailable = 4, 

        /// <summary>
        /// Colours are available.
        /// </summary>
        Flags1ColoursAvailable = 1, 

        /// <summary>
        /// Default to a proportional font.
        /// </summary>
        Flags1DefaultToProportionalFont = 64, 

        /// <summary>
        /// Fixed style is available.
        /// </summary>
        Flags1FixedStyleAvailable = 16, 

        /// <summary>
        /// Italic style is available.
        /// </summary>
        Flags1ItalicStyleAvailable = 8, 

        /// <summary>
        /// Pictures are available.
        /// </summary>
        Flags1PicturesAvailable = 2, 

        /// <summary>
        /// Display split is available.
        /// </summary>
        Flags1DisplaySplitAvailable = 32, 

        /// <summary>
        /// Sounds are available.
        /// </summary>
        Flags1SoundsAvailable = 32, 

        /// <summary>
        /// Story is split across multiple files.
        /// </summary>
        /// <remarks>
        /// This flag is not used in this implementation as all story files are assumed to be complete.
        /// </remarks>
        Flags1SplitStory = 4, 

        /// <summary>
        /// Status line is unavailable.
        /// </summary>
        Flags1StatusLineUnavailable = 16, 

        /// <summary>
        /// Tandy computer.
        /// </summary>
        Flags1TandyComputer = 8, 

        /// <summary>
        /// Story is time based.
        /// </summary>
        Flags1TimeBased = 2, 

        /// <summary>
        /// Timed input is available.
        /// </summary>
        Flags1TimedInputAvailable = 128, 

        /// <summary>
        /// Story needs colours.
        /// </summary>
        /// <remarks>
        /// This flag indicates the story can use colours, but is never altered by the interpreter.
        ///   This implementation does not rule out colour use if the flag is not set and so does not check it.
        /// </remarks>
        Flags2LowColoursUsed = 64, 

        /// <summary>
        /// Force fixed pitch.
        /// </summary>
        Flags2LowForceFixedPitch = 2, 

        /// <summary>
        /// Mouse is available.
        /// </summary>
        Flags2LowMouseAvailable = 32, 

        /// <summary>
        /// Pictures are available.
        /// </summary>
        Flags2LowPicturesAvailable = 8, 

        /// <summary>
        /// Display redraw needed.
        /// </summary>
        Flags2LowDisplayRedrawNeeded = 4, 

        /// <summary>
        /// Sounds are available.
        /// </summary>
        Flags2LowSoundsAvailableNew = 128, 

        /// <summary>
        /// Sounds are available.
        /// </summary>
        Flags2LowSoundsAvailableOld = 16, 

        /// <summary>
        /// Transcript is open.
        /// </summary>
        Flags2LowTranscriptOpen = 1, 

        /// <summary>
        /// Undo is available.
        /// </summary>
        Flags2LowUndoAvailable = 16, 

        /// <summary>
        /// Menus are available.
        /// </summary>
        Flags2HighMenusAvailable = 1, 
    }
}