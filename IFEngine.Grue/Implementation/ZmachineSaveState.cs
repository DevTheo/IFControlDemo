// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZmachineSaveState.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   A zmachine saved state.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// A zmachine saved state.
    /// </summary>
    internal struct ZmachineSaveState
    {
        /// <summary>
        /// The zmachine call stack.
        /// </summary>
        private readonly ImmutableStack<ActivationRecord> callStack;

        /// <summary>
        /// The zmachine memory.
        /// </summary>
        private readonly ImmutableArray<byte> memory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZmachineSaveState"/> struct.
        /// </summary>
        /// <param name="memory">
        /// The zmachine memory.
        /// </param>
        /// <param name="callStack">
        /// The zmachine call stack.
        /// </param>
        internal ZmachineSaveState(ImmutableArray<byte> memory, ImmutableStack<ActivationRecord> callStack)
        {
            this.memory = memory;
            this.callStack = callStack;
        }

        /// <summary>
        /// Gets the zmachine call stack.
        /// </summary>
        /// <value>
        /// The zmachine call stack.
        /// </value>
        internal ImmutableStack<ActivationRecord> CallStack
        {
            get
            {
                return this.callStack;
            }
        }

        /// <summary>
        /// Gets the zmachine memory.
        /// </summary>
        /// <value>
        /// The zmachine memory.
        /// </value>
        internal ImmutableArray<byte> Memory
        {
            get
            {
                return this.memory;
            }
        }

        /// <summary>
        /// Operator ==.
        /// </summary>
        /// <param name="first">
        /// The first save state to compare.
        /// </param>
        /// <param name="second">
        /// The second save state to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two save states are equal.
        /// </returns>
        public static bool operator ==(ZmachineSaveState first, ZmachineSaveState second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Operator !=.
        /// </summary>
        /// <param name="first">
        /// The first save state to compare.
        /// </param>
        /// <param name="second">
        /// The second save state to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two save states are not equal.
        /// </returns>
        public static bool operator !=(ZmachineSaveState first, ZmachineSaveState second)
        {
            return !first.Equals(second);
        }

        /// <summary>
        /// Determines whether two save states are equal.
        /// </summary>
        /// <param name="obj">
        /// The object to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two save states are equal.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is ZmachineSaveState && this.Equals((ZmachineSaveState)obj);
        }

        /// <summary>
        /// Determines whether two save states are equal.
        /// </summary>
        /// <param name="zmachineSaveState">
        /// The save state to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two save states are equal.
        /// </returns>
        public bool Equals(ZmachineSaveState zmachineSaveState)
        {
            return this.memory == zmachineSaveState.memory && this.callStack == zmachineSaveState.callStack;
        }

        /// <summary>
        /// Calculates the hashcode.
        /// </summary>
        /// <returns>
        /// The hashcode.
        /// </returns>
        public override int GetHashCode()
        {
            return this.memory.GetHashCode() ^ this.callStack.GetHashCode();
        }
    }
}