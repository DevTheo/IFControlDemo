// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectField.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   Object fields.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// Object fields.
    /// </summary>
    internal enum ObjectField
    {
        /// <summary>
        /// Child field.
        /// </summary>
        Child = 2, 

        /// <summary>
        /// Parent field.
        /// </summary>
        Parent = 0, 

        /// <summary>
        /// Sibling field.
        /// </summary>
        Sibling = 1
    }
}