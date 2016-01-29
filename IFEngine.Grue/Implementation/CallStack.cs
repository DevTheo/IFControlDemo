// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CallStack.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   The zmachine call stack.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// The zmachine call stack.
    /// </summary>
    internal sealed class CallStack
    {
        /// <summary>
        /// The zmachine front end.
        /// </summary>
        private readonly FrontEnd frontEnd;

        /// <summary>
        /// The calling routines on the call stack.
        /// </summary>
        private ImmutableStack<ActivationRecord> callingRoutines;

        /// <summary>
        /// The evaluation stack belonging to the current routine.
        /// </summary>
        private ImmutableStack<int> evaluationStack;

        /// <summary>
        /// The local variables belonging to the current routine.
        /// </summary>
        private int[] localVariables;

        /// <summary>
        /// The type of the current routine.
        /// </summary>
        private RoutineType type;

        /// <summary>
        /// Initializes a new instance of the <see cref="CallStack"/> class.
        /// </summary>
        /// <param name="frontEnd">
        /// The zmachine front end.
        /// </param>
        public CallStack(FrontEnd frontEnd)
        {
            this.frontEnd = frontEnd;
        }

        /// <summary>
        /// Gets the argument count of the current routine.
        /// </summary>
        /// <value>
        /// The argument count of the current routine.
        /// </value>
        public byte ArgumentCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the catch value of the current routine.
        /// </summary>
        /// <value>
        /// The catch value of the current routine.
        /// </value>
        public int CatchValue
        {
            get
            {
                return this.callingRoutines.Count();
            }
        }

        /// <summary>
        /// Gets or sets the program counter of the current routine.
        /// </summary>
        /// <value>
        /// The program counter of the current routine.
        /// </value>
        public int ProgramCounter
        {
            get;
            set;
        }

        /// <summary>
        /// Places a new routine on call stack.
        /// </summary>
        /// <param name="routineType">
        /// The type of the new routine.
        /// </param>
        /// <param name="programCounter">
        /// The program counter of the new routine.
        /// </param>
        /// <param name="argumentCount">
        /// The argument count of the new routine.
        /// </param>
        /// <param name="localVariableCount">
        /// The local variable count of the new routine.
        /// </param>
        public void BeginRoutine(RoutineType routineType, int programCounter, byte argumentCount, byte localVariableCount)
        {
            this.callingRoutines = this.callingRoutines.Add(new ActivationRecord(this.type, this.ProgramCounter, this.ArgumentCount, new ImmutableArray<int>(this.localVariables), this.evaluationStack));
            this.SetCurrentRoutine(routineType, programCounter, argumentCount, new int[localVariableCount], null);
        }

        /// <summary>
        /// Removes a routine from the call stack.
        /// </summary>
        /// <returns>
        /// The type of the removed routine.
        /// </returns>
        public RoutineType EndRoutine()
        {
            var routineType = this.type;
            if (this.callingRoutines != null)
            {
                var routine = this.callingRoutines.Top;
                this.SetCurrentRoutine(routine.RType, routine.ProgramCounter, routine.ArgumentCount, routine.LocalVariables.ToArray(), routine.EvaluationStack);
                this.callingRoutines = this.callingRoutines.Tail;
            }
            else
            {
                this.frontEnd.ErrorNotification(ErrorCondition.InvalidReturn, "Tried to return from the main routine.");
            }

            return routineType;
        }

        /// <summary>
        /// Initializes the call stack.
        /// </summary>
        /// <param name="initialProgramCounter">
        /// The initial program counter for the main routine.
        /// </param>
        /// <param name="localVariableCount">
        /// The local variable count of the main routine.
        /// </param>
        public void Initialize(int initialProgramCounter, byte localVariableCount)
        {
            this.callingRoutines = null;
            this.SetCurrentRoutine(RoutineType.Procedure, initialProgramCounter, 0, new int[localVariableCount], null);
        }

        /// <summary>
        /// Removes a value from the evaluation stack.
        /// </summary>
        /// <returns>
        /// The removed value.
        /// </returns>
        public ushort Pop()
        {
            if (this.evaluationStack == null)
            {
                this.frontEnd.ErrorNotification(ErrorCondition.StackUnderflow, "Tried to read from an empty stack.");
                return 0;
            }

            var top = (ushort)this.evaluationStack.Top;
            this.evaluationStack = this.evaluationStack.Tail;
            return top;
        }

        /// <summary>
        /// Adds a value to the evaluation stack.
        /// </summary>
        /// <param name="value">
        /// The added value.
        /// </param>
        public void Push(ushort value)
        {
            this.evaluationStack = this.evaluationStack.Add(value);
        }

        /// <summary>
        /// Reads the value of a local variable.
        /// </summary>
        /// <param name="localVariableNumber">
        /// The local variable number.
        /// </param>
        /// <returns>
        /// The local variable value.
        /// </returns>
        public ushort ReadLocalVariable(byte localVariableNumber)
        {
            if (localVariableNumber < this.localVariables.Length)
            {
                return (ushort)this.localVariables[localVariableNumber];
            }

            this.frontEnd.ErrorNotification(ErrorCondition.InvalidLocalVariable, "Tried to read from a nonexistent local variable " + localVariableNumber + ".");
            return 0;
        }

        /// <summary>
        /// Restores the call stack.
        /// </summary>
        /// <param name="restoredStack">
        /// The restored stack.
        /// </param>
        public void Restore(ImmutableStack<ActivationRecord> restoredStack)
        {
            this.callingRoutines = restoredStack.Tail;
            var currentRoutine = restoredStack.Top;
            this.SetCurrentRoutine(currentRoutine.RType, currentRoutine.ProgramCounter, currentRoutine.ArgumentCount, currentRoutine.LocalVariables.ToArray(), currentRoutine.EvaluationStack);
        }

        /// <summary>
        /// Saves the call stack.
        /// </summary>
        /// <returns>
        /// The saved stack.
        /// </returns>
        public ImmutableStack<ActivationRecord> Save()
        {
            return this.callingRoutines.Add(new ActivationRecord(this.type, this.ProgramCounter, this.ArgumentCount, new ImmutableArray<int>(this.localVariables), this.evaluationStack));
        }

        /// <summary>
        /// Ends routines until a routine with the given catch value is found.
        /// </summary>
        /// <param name="catchValue">
        /// The catch value to find.
        /// </param>
        public void Throw(int catchValue)
        {
            if ((uint)catchValue <= this.callingRoutines.Count())
            {
                while (this.callingRoutines.Count() != catchValue)
                {
                    this.EndRoutine();
                }

                return;
            }

            this.frontEnd.ErrorNotification(ErrorCondition.InvalidThrow, "Attempted THROW to a nonexistent routine: " + catchValue + ".");
        }

        /// <summary>
        /// Writes a value to a local variable.
        /// </summary>
        /// <param name="localVariableNumber">
        /// The local variable number.
        /// </param>
        /// <param name="localVariableValue">
        /// The local variable value.
        /// </param>
        public void WriteLocalVariable(byte localVariableNumber, ushort localVariableValue)
        {
            if (localVariableNumber < this.localVariables.Length)
            {
                this.localVariables[localVariableNumber] = localVariableValue;
                return;
            }

            this.frontEnd.ErrorNotification(ErrorCondition.InvalidLocalVariable, "Tried to write to a nonexistent local variable " + localVariableNumber + ".");
        }

        /// <summary>
        /// Sets the current routine.
        /// </summary>
        /// <param name="routineType">
        /// The routine type.
        /// </param>
        /// <param name="programCounter">
        /// The program counter.
        /// </param>
        /// <param name="argumentCount">
        /// The argument count.
        /// </param>
        /// <param name="variables">
        /// The localVariables.
        /// </param>
        /// <param name="stack">
        /// The stack.
        /// </param>
        private void SetCurrentRoutine(RoutineType routineType, int programCounter, byte argumentCount, int[] variables, ImmutableStack<int> stack)
        {
            this.ArgumentCount = argumentCount;
            this.localVariables = variables;
            this.evaluationStack = stack;
            this.ProgramCounter = programCounter;
            this.type = routineType;
        }
    }
}