// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivationRecord.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   A zmachine call stack activation record.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// A zmachine call stack activation record.
    /// </summary>
    internal sealed class ActivationRecord
    {
        /// <summary>
        /// The argument count.
        /// </summary>
        private readonly byte argumentCount;

        /// <summary>
        /// The evaluation stack.
        /// </summary>
        private readonly ImmutableStack<int> evaluationStack;

        /// <summary>
        /// The local variables.
        /// </summary>
        private readonly ImmutableArray<int> localVariables;

        /// <summary>
        /// The program counter.
        /// </summary>
        private readonly int programCounter;

        /// <summary>
        /// The routine type.
        /// </summary>
        private readonly RoutineType type;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivationRecord"/> class.
        /// </summary>
        /// <param name="type">
        /// The routine type.
        /// </param>
        /// <param name="programCounter">
        /// The program counter.
        /// </param>
        /// <param name="argumentCount">
        /// The argument count.
        /// </param>
        /// <param name="localVariables">
        /// The local variables.
        /// </param>
        /// <param name="evaluationStack">
        /// The evaluation stack.
        /// </param>
        public ActivationRecord(RoutineType type, int programCounter, byte argumentCount, ImmutableArray<int> localVariables, ImmutableStack<int> evaluationStack)
        {
            this.argumentCount = argumentCount;
            this.type = type;
            this.localVariables = localVariables;
            this.programCounter = programCounter;
            this.evaluationStack = evaluationStack;
        }

        /// <summary>
        /// Gets the argument count.
        /// </summary>
        /// <value>
        /// The argument count.
        /// </value>
        public byte ArgumentCount
        {
            get
            {
                return this.argumentCount;
            }
        }

        /// <summary>
        /// Gets the evaluation stack.
        /// </summary>
        /// <value>
        /// The evaluation stack.
        /// </value>
        public ImmutableStack<int> EvaluationStack
        {
            get
            {
                return this.evaluationStack;
            }
        }

        /// <summary>
        /// Gets the local variables.
        /// </summary>
        /// <value>
        /// The local variables.
        /// </value>
        public ImmutableArray<int> LocalVariables
        {
            get
            {
                return this.localVariables;
            }
        }

        /// <summary>
        /// Gets the program counter.
        /// </summary>
        /// <value>
        /// The program counter.
        /// </value>
        public int ProgramCounter
        {
            get
            {
                return this.programCounter;
            }
        }

        /// <summary>
        /// Gets the routine type.
        /// </summary>
        /// <value>
        /// The routine type.
        /// </value>
        public RoutineType RType
        {
            get
            {
                return this.type;
            }
        }
    }
}