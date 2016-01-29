// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MouseButtons.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   Mouse buttons.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    using System;

    /// <summary>
    /// Mouse buttons.
    /// </summary>
    [Flags]
    internal enum MouseButtons
    {
        /// <summary>
        /// No buttons.
        /// </summary>
        None = 0, 

        /// <summary>
        /// Button one.
        /// </summary>
        Button1 = 1, 

        /// <summary>
        /// Button two.
        /// </summary>
        Button2 = Button1 << 1, 

        /// <summary>
        /// Button three.
        /// </summary>
        Button3 = Button2 << 1, 

        /// <summary>
        /// Button four.
        /// </summary>
        Button4 = Button3 << 1, 

        /// <summary>
        /// Button five.
        /// </summary>
        Button5 = Button4 << 1, 

        /// <summary>
        /// Button six.
        /// </summary>
        Button6 = Button5 << 1, 

        /// <summary>
        /// Button seven.
        /// </summary>
        Button7 = Button6 << 1, 

        /// <summary>
        /// Button eight.
        /// </summary>
        Button8 = Button7 << 1, 

        /// <summary>
        /// Button nine.
        /// </summary>
        Button9 = Button8 << 1, 

        /// <summary>
        /// Button ten.
        /// </summary>
        Button10 = Button9 << 1, 

        /// <summary>
        /// Button eleven.
        /// </summary>
        Button11 = Button10 << 1, 

        /// <summary>
        /// Button twelve.
        /// </summary>
        Button12 = Button11 << 1, 

        /// <summary>
        /// Button thirteen.
        /// </summary>
        Button13 = Button12 << 1, 

        /// <summary>
        /// Button fourteen.
        /// </summary>
        Button14 = Button13 << 1, 

        /// <summary>
        /// Button fifteen.
        /// </summary>
        Button15 = Button14 << 1, 

        /// <summary>
        /// Button sixteen.
        /// </summary>
        Button16 = Button15 << 1, 
    }
}