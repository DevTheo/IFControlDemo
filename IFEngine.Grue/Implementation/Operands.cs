// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Operands.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   The operands for a zmachine instruction.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// The operands for a zmachine instruction.
    /// </summary>
    internal sealed class Operands
    {
        /// <summary>
        /// The operand count.
        /// </summary>
        private byte count;

        /// <summary>
        /// The eighth operand.
        /// </summary>
        private ushort eighth;

        /// <summary>
        /// The fifth operand.
        /// </summary>
        private ushort fifth;

        /// <summary>
        /// The first operand.
        /// </summary>
        private ushort first;

        /// <summary>
        /// The fourth operand.
        /// </summary>
        private ushort fourth;

        /// <summary>
        /// The second operand.
        /// </summary>
        private ushort second;

        /// <summary>
        /// The seventh operand.
        /// </summary>
        private ushort seventh;

        /// <summary>
        /// The sixth operand.
        /// </summary>
        private ushort sixth;

        /// <summary>
        /// The third operand.
        /// </summary>
        private ushort third;

        /// <summary>
        /// Gets the operand count.
        /// </summary>
        /// <value>
        /// The operand count.
        /// </value>
        internal byte Count
        {
            get
            {
                return this.count;
            }
        }

        /// <summary>
        /// Gets the eighth operand.
        /// </summary>
        /// <value>
        /// The eighth operand.
        /// </value>
        internal ushort Eighth
        {
            get
            {
                return this.eighth;
            }
        }

        /// <summary>
        /// Gets the fifth operand.
        /// </summary>
        /// <value>
        /// The fifth operand.
        /// </value>
        internal ushort Fifth
        {
            get
            {
                return this.fifth;
            }
        }

        /// <summary>
        /// Gets the first operand.
        /// </summary>
        /// <value>
        /// The first operand.
        /// </value>
        internal ushort First
        {
            get
            {
                return this.first;
            }
        }

        /// <summary>
        /// Gets the fourth operand.
        /// </summary>
        /// <value>
        /// The fourth operand.
        /// </value>
        internal ushort Fourth
        {
            get
            {
                return this.fourth;
            }
        }

        /// <summary>
        /// Gets the second operand.
        /// </summary>
        /// <value>
        /// The second operand.
        /// </value>
        internal ushort Second
        {
            get
            {
                return this.second;
            }
        }

        /// <summary>
        /// Gets the seventh operand.
        /// </summary>
        /// <value>
        /// The seventh operand.
        /// </value>
        internal ushort Seventh
        {
            get
            {
                return this.seventh;
            }
        }

        /// <summary>
        /// Gets the sixth operand.
        /// </summary>
        /// <value>
        /// The sixth operand.
        /// </value>
        internal ushort Sixth
        {
            get
            {
                return this.sixth;
            }
        }

        /// <summary>
        /// Gets the third operand.
        /// </summary>
        /// <value>
        /// The third operand.
        /// </value>
        internal ushort Third
        {
            get
            {
                return this.third;
            }
        }

        /// <summary>
        /// Gets an operand by index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The operand at the given index.
        /// </returns>
        internal ushort this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.First;
                    case 1:
                        return this.Second;
                    case 2:
                        return this.Third;
                    case 3:
                        return this.Fourth;
                    case 4:
                        return this.Fifth;
                    case 5:
                        return this.Sixth;
                    case 6:
                        return this.Seventh;
                    case 7:
                        return this.Eighth;
                    default:
                        return 0;
                }
            }
        }

        /// <summary>
        /// Loads an operand.
        /// </summary>
        /// <param name="value">
        /// The operand value.
        /// </param>
        internal void Load(ushort value)
        {
            switch (this.count++)
            {
                case 0:
                    this.first = value;
                    break;
                case 1:
                    this.second = value;
                    break;
                case 2:
                    this.third = value;
                    break;
                case 3:
                    this.fourth = value;
                    break;
                case 4:
                    this.fifth = value;
                    break;
                case 5:
                    this.sixth = value;
                    break;
                case 6:
                    this.seventh = value;
                    break;
                case 7:
                    this.eighth = value;
                    break;
            }
        }

        /// <summary>
        /// Resets the operands.
        /// </summary>
        internal void Reset()
        {
            this.count = 0;
            this.first = 0;
            this.second = 0;
            this.third = 0;
            this.fourth = 0;
            this.fifth = 0;
            this.sixth = 0;
            this.seventh = 0;
            this.eighth = 0;
        }
    }
}