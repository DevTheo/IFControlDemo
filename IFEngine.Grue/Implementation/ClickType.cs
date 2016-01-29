// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClickType.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   Mouse click types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// Mouse click types.
    /// </summary>
    internal enum ClickType
    {
        /// <summary>
        /// Default value.
        /// </summary>
        None = Zscii.Null, 

        /// <summary>
        /// A single click.
        /// </summary>
        SingleClick = Zscii.SingleClick, 

        /// <summary>
        /// A double click.
        /// </summary>
        DoubleClick = Zscii.DoubleClick, 

        /// <summary>
        /// A menu click.
        /// </summary>
        MenuClick = Zscii.MenuClick
    }
}