// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryStream.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   A stream for writing to memory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// A stream for writing to memory.
    /// </summary>
    internal sealed class MemoryStream
    {
        /// <summary>
        /// The table address.
        /// </summary>
        private readonly ushort table;

        /// <summary>
        /// The stream width.
        /// </summary>
        private readonly ushort width;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryStream"/> class.
        /// </summary>
        /// <param name="tableAddress">
        /// The table address.
        /// </param>
        /// <param name="width">
        /// The stream width.
        /// </param>
        internal MemoryStream(ushort tableAddress, ushort width) : this(tableAddress)
        {
            this.width = width;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryStream"/> class.
        /// </summary>
        /// <param name="tableAddress">
        /// The table address.
        /// </param>
        internal MemoryStream(ushort tableAddress)
        {
            this.table = tableAddress;
        }

        /// <summary>
        /// Gets or sets the character count.
        /// </summary>
        /// <value>
        /// The character count.
        /// </value>
        internal int CharacterCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the table address.
        /// </summary>
        /// <value>
        /// The table address.
        /// </value>
        internal ushort Table
        {
            get
            {
                return this.table;
            }
        }

        /// <summary>
        /// Gets the stream width.
        /// </summary>
        /// <value>
        /// The stream width.
        /// </value>
        internal ushort Width
        {
            get
            {
                return this.width;
            }
        }
    }
}