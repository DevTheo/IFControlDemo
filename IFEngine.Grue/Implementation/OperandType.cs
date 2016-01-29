// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperandType.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   Operand types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// Operand types.
    /// </summary>
    internal enum OperandType
    {
        /// <summary>
        /// Large constant operand.
        /// </summary>
        LargeConstant = 0, 

        /// <summary>
        /// Small constant operand.
        /// </summary>
        SmallConstant = 1, 

        /// <summary>
        /// Variable operand.
        /// </summary>
        Variable = 2
    }
}