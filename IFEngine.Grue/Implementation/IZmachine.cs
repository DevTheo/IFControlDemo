// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IZmachine.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   The zmachine interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// The zmachine interface.
    /// </summary>
    internal interface IZmachine
    {
        /// <summary>
        /// Gets or sets a value indicating whether the input log is open.
        /// </summary>
        /// <value>
        /// A value indicating whether the input log is open.
        /// </value>
        bool InputLogOpen
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the interactive fiction identifier.
        /// </summary>
        /// <value>
        /// The interactive fiction identifier.
        /// </value>
        string InteractiveFictionIdentifier
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether a status line is used.
        /// </summary>
        /// <value>
        /// A value indicating whether a status line is used.
        /// </value>
        bool StatusLineUsed
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the transcript is open.
        /// </summary>
        /// <value>
        /// A value indicating whether the transcript is open.
        /// </value>
        bool TranscriptOpen
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates the display should be redrawn.
        /// </summary>
        void RedrawDisplay();

        /// <summary>
        /// Restarts the zmachine.
        /// </summary>
        void Restart();

        /// <summary>
        /// Restores a saved state.
        /// </summary>
        /// <param name="saveState">
        /// The saved state to restore.
        /// </param>
        /// <returns>
        /// A value indicating whether the restore succeeded.
        /// </returns>
        bool Restore(ZmachineSaveState saveState);

        /// <summary>
        /// Runs one cycle of the zmachine.
        /// </summary>
        Task Run();

        /// <summary>
        /// Indicates a sound has finished playing.
        /// </summary>
        /// <param name="sound">
        /// The sound.
        /// </param>
        void SoundFinished(int sound);
    }
}