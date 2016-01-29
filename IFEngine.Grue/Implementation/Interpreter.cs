// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interpreter.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   Interpreter types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// Interpreter types.
    /// </summary>
    internal enum Interpreter
    {
        /// <summary>
        /// Invalid interpreter.
        /// </summary>
        Invalid = 0, 

        /// <summary>
        /// DEC System 20 interpreter.
        /// </summary>
        DEC20 = 1, 

        /// <summary>
        /// Apple IIe interpreter.
        /// </summary>
        AppleIIe = 2, 

        /// <summary>
        /// Apple Macintosh interpreter.
        /// </summary>
        Macintosh = 3, 

        /// <summary>
        /// Commodore Amiga interpreter.
        /// </summary>
        Amiga = 4, 

        /// <summary>
        /// Atari ST interpreter.
        /// </summary>
        AtariST = 5, 

        /// <summary>
        /// IBM PC interpreter.
        /// </summary>
        IBM = 6, 

        /// <summary>
        /// Commodore 128 interpreter.
        /// </summary>
        Commodore128 = 7, 

        /// <summary>
        /// Commodore 64 interpreter.
        /// </summary>
        Commodore64 = 8, 

        /// <summary>
        /// Apple IIc interpreter.
        /// </summary>
        AppleIIc = 9, 

        /// <summary>
        /// Apple IIgs interpreter.
        /// </summary>
        AppleIIgs = 10, 

        /// <summary>
        /// Tandy Color Computer interpreter.
        /// </summary>
        TandyColorComputer = 11
    }
}