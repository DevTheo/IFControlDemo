// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Memory.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   The zmachine memory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// The zmachine memory.
    /// </summary>
    /// <remarks>
    /// Memory is separated into three regions: dynamic, static, and high.
    ///   Dynamic can be read and written to, while static and high are read only.
    ///   As used by Infocom, high memory indicated parts of the story which did not have to be kept in memory but could be loaded from disk as needed.
    ///   This implementaion does not distinguish between static and high memory.
    ///   The first 64 bytes of dynamic memory are known as the header and contain fields and flags which indicate the zmachine's capabilities and state.
    ///   One particular bit in the header must be able to control the state of the transcript stream on the zmachine.
    /// </remarks>
    internal sealed class Memory
    {
        /// <summary>
        /// The dynamic memory.
        /// </summary>
        private readonly byte[] dynamicMemory;

        /// <summary>
        /// The dynamic memory length.
        /// </summary>
        /// <remarks>
        /// This is used to enhance the performance of memory reads and writes.
        /// </remarks>
        private readonly int dynamicMemoryLength;

        /// <summary>
        /// The zmachine front end.
        /// </summary>
        private readonly FrontEnd frontEnd;

        /// <summary>
        /// The story.
        /// </summary>
        /// <remarks>
        /// This is used for reads from static and high memory and the initialization and reset of dynamic memory.
        /// </remarks>
        private readonly ImmutableArray<byte> story;

        /// <summary>
        /// The story length.
        /// </summary>
        /// <remarks>
        /// This is used to enhance the performance of memory reads and writes.
        /// </remarks>
        private readonly int storyLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="Memory"/> class.
        /// </summary>
        /// <param name="story">
        /// The story data.
        /// </param>
        /// <param name="frontEnd">
        /// The zmachine front end.
        /// </param>
        /// <remarks>
        /// We have to initialize the low byte of Flags 2 here because the first two bits in it must always survive initialization.
        /// </remarks>
        internal Memory(ImmutableArray<byte> story, FrontEnd frontEnd)
        {
            this.story = story;
            this.storyLength = this.story.Length;
            this.frontEnd = frontEnd;
            this.dynamicMemoryLength = this.ReadStoryWord(14);
            this.dynamicMemory = new byte[this.dynamicMemoryLength];
            this.WriteByte(Flags2LowByteAddress, this.ReadStoryByte(Flags2LowByteAddress));
        }

        /// <summary>
        /// Gets the flags2 low byte address.
        /// </summary>
        /// <value>
        /// The flags2 low byte address.
        /// </value>
        internal static byte Flags2LowByteAddress
        {
            get
            {
                return 17;
            }
        }

        /// <summary>
        /// Gets the story length.
        /// </summary>
        /// <value>
        /// The story length.
        /// </value>
        internal int StoryLength
        {
            get
            {
                return this.storyLength;
            }
        }

        /// <summary>
        /// Initializes dynamic memory.
        /// </summary>
        internal void Initialize()
        {
            this.Restore(null);
        }

        /// <summary>
        /// Reads a byte from memory.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// The value.
        /// </returns>
        internal byte ReadByte(int address)
        {
            return (uint)address < this.dynamicMemoryLength ? this.dynamicMemory[address] : this.ReadStoryByte(address);
        }

        /// <summary>
        /// Reads the flags which match a bitmask.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="bitMask">
        /// The bit mask.
        /// </param>
        /// <returns>
        /// A value which indicates whether all the flags which match the bit mask are set.
        /// </returns>
        internal bool ReadFlags(int address, byte bitMask)
        {
            return (this.ReadByte(address) & bitMask) == bitMask;
        }

        /// <summary>
        /// Reads a byte from the story.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// The value.
        /// </returns>
        internal byte ReadStoryByte(int address)
        {
            if ((uint)address < this.storyLength)
            {
                return this.story[address];
            }

            this.frontEnd.ErrorNotification(ErrorCondition.InvalidAddress, "Tried to read invalid address (" + address + ").");
            return 0;
        }

        /// <summary>
        /// Reads a word from the story.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// The value.
        /// </returns>
        internal ushort ReadStoryWord(int address)
        {
            return (ushort)((this.ReadStoryByte(address) << 8) + this.ReadStoryByte(address + 1));
        }

        /// <summary>
        /// Reads a word from memory.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <returns>
        /// The value.
        /// </returns>
        internal ushort ReadWord(int address)
        {
            return (ushort)((this.ReadByte(address) << 8) + this.ReadByte(address + 1));
        }

        /// <summary>
        /// Restores the dynamic memory.
        /// </summary>
        /// <param name="restoredMemory">
        /// The restored memory.
        /// </param>
        /// <remarks>
        /// If values are missing, the original story values will be used.
        ///   If there are extra values, they will be ignored.
        ///   This allows restores which were truncated by the frontend to save space or were saved from a different version of the story.
        ///   The restore may not be valid, but we always try.
        /// </remarks>
        internal void Restore(ImmutableArray<byte> restoredMemory)
        {
            restoredMemory = restoredMemory ?? new ImmutableArray<byte>(new byte[0]);
            const byte PreservedFlags = (byte)(HeaderFlags.Flags2LowTranscriptOpen | HeaderFlags.Flags2LowForceFixedPitch);
            var preservedFlagValues = this.dynamicMemory[Flags2LowByteAddress] & PreservedFlags;
            for (var address = 0; address < this.dynamicMemoryLength; address++)
            {
                this.dynamicMemory[address] = (address < restoredMemory.Length) ? restoredMemory[address] : this.ReadStoryByte(address);
            }

            this.dynamicMemory[Flags2LowByteAddress] = (byte)(this.dynamicMemory[Flags2LowByteAddress] & ~PreservedFlags | preservedFlagValues);
        }

        /// <summary>
        /// Saves the dynamic memory.
        /// </summary>
        /// <returns>
        /// The saved memory.
        /// </returns>
        internal ImmutableArray<byte> Save()
        {
            var saveMemory = new byte[this.dynamicMemoryLength];
            for (var address = 0; address < this.dynamicMemoryLength; address++)
            {
                saveMemory[address] = this.dynamicMemory[address];
            }

            return new ImmutableArray<byte>(saveMemory);
        }

        /// <summary>
        /// Writes a byte to memory.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        internal void WriteByte(int address, byte value)
        {
            if ((uint)address < this.dynamicMemoryLength)
            {
                if (address == Flags2LowByteAddress)
                {
                    var transcriptState = ((HeaderFlags)this.dynamicMemory[Flags2LowByteAddress] & HeaderFlags.Flags2LowTranscriptOpen) == HeaderFlags.Flags2LowTranscriptOpen;
                    var newTranscriptState = ((HeaderFlags)value & HeaderFlags.Flags2LowTranscriptOpen) == HeaderFlags.Flags2LowTranscriptOpen;
                    if (newTranscriptState != transcriptState)
                    {
                        this.frontEnd.ControlOutputStream(newTranscriptState, OutputStream.Transcript);
                    }
                }

                this.dynamicMemory[address] = value;
                return;
            }

            this.frontEnd.ErrorNotification(ErrorCondition.InvalidAddress, "Tried to write to invalid dynamic memory address (" + address + ").");
        }

        /// <summary>
        /// Writes a value into the flags which match a bit mask.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="bitMask">
        /// The bit mask.
        /// </param>
        /// <param name="flagValue">
        /// The flag value.
        /// </param>
        internal void WriteFlags(int address, byte bitMask, bool flagValue)
        {
            var oldValue = this.ReadByte(address);
            this.WriteByte(address, (byte)(flagValue ? oldValue | bitMask : oldValue & ~bitMask));
        }

        /// <summary>
        /// Writes a word to memory.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        internal void WriteWord(int address, ushort value)
        {
            this.WriteByte(address, (byte)(value >> 8));
            this.WriteByte(address + 1, (byte)value);
        }
    }
}