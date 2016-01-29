// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimedInput.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   The zmachine timed input.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// The zmachine timed input.
    /// </summary>
    internal struct TimedInput
    {
        /// <summary>
        /// The elapsed time since the last time marker minus the time spent at any [MORE] prompts.
        /// </summary>
        private readonly long elapsedTime;

        /// <summary>
        /// The input values.
        /// </summary>
        private readonly ImmutableQueue<InputValue> inputValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedInput"/> struct.
        /// </summary>
        /// <param name="inputValues">
        /// The input values.
        /// </param>
        /// <param name="elapsedTime">
        /// The elapsed time since the last time marker minus the time spent at any [MORE] prompts.
        /// </param>
        internal TimedInput(ImmutableQueue<InputValue> inputValues, long elapsedTime)
        {
            this.elapsedTime = elapsedTime;
            this.inputValues = inputValues;
        }

        /// <summary>
        /// Gets the elapsed time since the last time marker minus the time spent at any [MORE] prompts.
        /// </summary>
        /// <value>
        /// The elapsed time since the last time marker minus the time spent at any [MORE] prompts.
        /// </value>
        internal long ElapsedTime
        {
            get
            {
                return this.elapsedTime;
            }
        }

        /// <summary>
        /// Gets the input values.
        /// </summary>
        /// <value>
        /// The input values.
        /// </value>
        internal ImmutableQueue<InputValue> InputValues
        {
            get
            {
                return this.inputValues;
            }
        }

        /// <summary>
        /// Operator ==.
        /// </summary>
        /// <param name="first">
        /// The first input to compare.
        /// </param>
        /// <param name="second">
        /// The second input to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two inputs are equal.
        /// </returns>
        public static bool operator ==(TimedInput first, TimedInput second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Operator !=.
        /// </summary>
        /// <param name="first">
        /// The first input to compare.
        /// </param>
        /// <param name="second">
        /// The second input to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two inputs are not equal.
        /// </returns>
        public static bool operator !=(TimedInput first, TimedInput second)
        {
            return !first.Equals(second);
        }

        /// <summary>
        /// Determines whether two inputs are equal.
        /// </summary>
        /// <param name="obj">
        /// The object to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two inputs are equal.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is TimedInput && this.Equals((TimedInput)obj);
        }

        /// <summary>
        /// Determines whether two inputs are equal.
        /// </summary>
        /// <param name="timedInput">
        /// The input to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two inputs are equal.
        /// </returns>
        public bool Equals(TimedInput timedInput)
        {
            return this.inputValues == timedInput.inputValues && this.elapsedTime == timedInput.elapsedTime;
        }

        /// <summary>
        /// Calculates the hashcode.
        /// </summary>
        /// <returns>
        /// The hashcode.
        /// </returns>
        public override int GetHashCode()
        {
            return this.inputValues.GetHashCode();
        }
    }
}