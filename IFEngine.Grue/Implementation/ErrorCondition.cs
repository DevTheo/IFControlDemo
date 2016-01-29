// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorCondition.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   Error conditions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// Error conditions.
    /// </summary>
    internal enum ErrorCondition
    {
        /// <summary>
        /// Division by zero.
        /// </summary>
        DivisionByZero, 

        /// <summary>
        /// Invalid address.
        /// </summary>
        InvalidAddress, 

        /// <summary>
        /// Invalid attribute.
        /// </summary>
        InvalidAttribute, 

        /// <summary>
        /// Invalid colour.
        /// </summary>
        InvalidColor, 

        /// <summary>
        /// Invalid dictionary entry length.
        /// </summary>
        InvalidDictionaryEntryLength, 

        /// <summary>
        /// Invalid local variable.
        /// </summary>
        InvalidLocalVariable, 

        /// <summary>
        /// Invalid menu.
        /// </summary>
        InvalidMenu, 

        /// <summary>
        /// Invalid object.
        /// </summary>
        InvalidObject, 

        /// <summary>
        /// Invalid object tree.
        /// </summary>
        InvalidObjectTree, 

        /// <summary>
        /// Invalid operand count.
        /// </summary>
        InvalidOperandCount, 

        /// <summary>
        /// Invalid operation code.
        /// </summary>
        InvalidOperationCode, 

        /// <summary>
        /// Invalid output.
        /// </summary>
        InvalidOutput, 

        /// <summary>
        /// Invalid picture.
        /// </summary>
        InvalidPicture, 

        /// <summary>
        /// Invalid property.
        /// </summary>
        InvalidProperty, 

        /// <summary>
        /// Invalid property length.
        /// </summary>
        InvalidPropertyLength, 

        /// <summary>
        /// Invalid return.
        /// </summary>
        InvalidReturn, 

        /// <summary>
        /// Invalid routine.
        /// </summary>
        InvalidRoutine, 

        /// <summary>
        /// Invalid sound action.
        /// </summary>
        InvalidSoundAction, 

        /// <summary>
        /// Invalid stream.
        /// </summary>
        InvalidStream, 

        /// <summary>
        /// Invalid throw.
        /// </summary>
        InvalidThrow, 

        /// <summary>
        /// Invalid window.
        /// </summary>
        InvalidWindow, 

        /// <summary>
        /// Invalid window property.
        /// </summary>
        InvalidWindowProperty, 

        /// <summary>
        /// Invalid window size.
        /// </summary>
        InvalidWindowSize, 

        /// <summary>
        /// Invalid window style operation.
        /// </summary>
        InvalidWindowStyleOperation, 

        /// <summary>
        /// Nested abbreviation.
        /// </summary>
        NestedAbbreviation, 

        /// <summary>
        /// Parse buffer overflow.
        /// </summary>
        ParseBufferOverflow, 

        /// <summary>
        /// Property not found.
        /// </summary>
        PropertyNotFound, 

        /// <summary>
        /// Shift out of range.
        /// </summary>
        ShiftOutOfRange, 

        /// <summary>
        /// Stack underflow.
        /// </summary>
        StackUnderflow
    }
}