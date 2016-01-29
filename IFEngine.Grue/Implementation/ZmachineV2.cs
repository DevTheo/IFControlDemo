// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZmachineV2.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   The zmachine version 2.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// The zmachine version 2.
    /// </summary>
    /// <remarks>
    /// Version 2 modifies the zscii alphabet, and adds the ability to store abbreviations for commonly used zscii text.
    ///   These changes are reflected in a slightly modified algorithm for converting from zcharacters to zscii.
    /// </remarks>
    internal class ZmachineV2 : ZmachineV1
    {
        /// <summary>
        /// The zscii alphabet characters.
        /// </summary>
        private static readonly ImmutableArray<Zscii> zsciiAlphabetCharacters = new ImmutableArray<Zscii>(new[] { Zscii.LowercaseA, Zscii.LowercaseB, Zscii.LowercaseC, Zscii.LowercaseD, Zscii.LowercaseE, Zscii.LowercaseF, Zscii.LowercaseG, Zscii.LowercaseH, Zscii.LowercaseI, Zscii.LowercaseJ, Zscii.LowercaseK, Zscii.LowercaseL, Zscii.LowercaseM, Zscii.LowercaseN, Zscii.LowercaseO, Zscii.LowercaseP, Zscii.LowercaseQ, Zscii.LowercaseR, Zscii.LowercaseS, Zscii.LowercaseT, Zscii.LowercaseU, Zscii.LowercaseV, Zscii.LowercaseW, Zscii.LowercaseX, Zscii.LowercaseY, Zscii.LowercaseZ, Zscii.UppercaseA, Zscii.UppercaseB, Zscii.UppercaseC, Zscii.UppercaseD, Zscii.UppercaseE, Zscii.UppercaseF, Zscii.UppercaseG, Zscii.UppercaseH, Zscii.UppercaseI, Zscii.UppercaseJ, Zscii.UppercaseK, Zscii.UppercaseL, Zscii.UppercaseM, Zscii.UppercaseN, Zscii.UppercaseO, Zscii.UppercaseP, Zscii.UppercaseQ, Zscii.UppercaseR, Zscii.UppercaseS, Zscii.UppercaseT, Zscii.UppercaseU, Zscii.UppercaseV, Zscii.UppercaseW, Zscii.UppercaseX, Zscii.UppercaseY, Zscii.UppercaseZ, Zscii.Null, Zscii.NewLine, Zscii.Number0, Zscii.Number1, Zscii.Number2, Zscii.Number3, Zscii.Number4, Zscii.Number5, Zscii.Number6, Zscii.Number7, Zscii.Number8, Zscii.Number9, Zscii.Period, Zscii.Comma, Zscii.ExclamationPoint, Zscii.QuestionMark, Zscii.Underscore, Zscii.NumberSign, Zscii.SingleQuote, Zscii.DoubleQuote, Zscii.ForwardSlash, Zscii.BackSlash, Zscii.Hyphen, Zscii.Colon, Zscii.LeftParenthesis, Zscii.RightParenthesis });

        /// <summary>
        /// Initializes a new instance of the <see cref="ZmachineV2"/> class.
        /// </summary>
        /// <param name="frontEnd">
        /// The front end.
        /// </param>
        /// <param name="story">
        /// The story.
        /// </param>
        /// <param name="random">
        /// The random number generator.
        /// </param>
        internal ZmachineV2(FrontEnd frontEnd, ImmutableArray<byte> story, IRandomNumberGenerator random) : base(frontEnd, story, random)
        {
        }

        /// <summary>
        /// Appends an abbreviation to the given zscii text.
        /// </summary>
        /// <param name="zcharacter">
        /// The zcharacter.
        /// </param>
        /// <param name="calledRecursively">
        /// A value which indicates whether the method was called recursively.
        /// </param>
        /// <param name="zcharacters">
        /// The zcharacters.
        /// </param>
        /// <param name="zsciiText">
        /// The zscii text.
        /// </param>
        protected void AppendAbbreviation(byte zcharacter, bool calledRecursively, ref ImmutableStack<byte> zcharacters, ref ImmutableStack<Zscii> zsciiText)
        {
            if (calledRecursively)
            {
                this.FrontEnd.ErrorNotification(ErrorCondition.NestedAbbreviation, "Nested abbreviation detected.");
                return;
            }

            if (zcharacters != null)
            {
                var abbreviationNumber = ((zcharacter - 1) * 32) + zcharacters.Top;
                zcharacters = zcharacters.Tail;
                var abbreviationsTableAddress = this.Memory.ReadWord(24);
                var abbreviationAddress = 2 * this.Memory.ReadWord(abbreviationsTableAddress + (2 * abbreviationNumber));
                var abbreviation = this.ZCharactersToZscii(true, this.EncodedTextToZCharacters(ref abbreviationAddress));
                foreach (var zsciiCharacter in abbreviation.Enumerable())
                {
                    zsciiText = zsciiText.Add(zsciiCharacter);
                }
            }
        }

        /// <summary>
        /// Gets the zscii alphabet character indicated by the given index.
        /// </summary>
        /// <param name="index">
        /// The character index.
        /// </param>
        /// <returns>
        /// The zscii alphabet character.
        /// </returns>
        protected override Zscii GetZsciiAlphabetCharacter(byte index)
        {
            return zsciiAlphabetCharacters[index];
        }

        /// <summary>
        /// Converts z-characters to zscii text.
        /// </summary>
        /// <param name="calledRecursively">
        /// A value which indicates whether the method was called recursively.
        /// </param>
        /// <param name="zcharacter">
        /// The zcharacter.
        /// </param>
        /// <param name="currentAlphabet">
        /// The current alphabet.
        /// </param>
        /// <param name="nextAlphabet">
        /// The next alphabet.
        /// </param>
        /// <param name="lockedAlphabet">
        /// The locked alphabet.
        /// </param>
        /// <param name="zcharacters">
        /// The zcharacters.
        /// </param>
        /// <param name="zsciiText">
        /// The zscii text.
        /// </param>
        /// <remarks>
        /// Zcharacter 1 represents an abbreviation.
        /// </remarks>
        protected override void ZCharactersToZscii(bool calledRecursively, byte zcharacter, byte currentAlphabet, ref byte nextAlphabet, ref byte lockedAlphabet, ref ImmutableStack<byte> zcharacters, ref ImmutableStack<Zscii> zsciiText)
        {
            if (zcharacter == 1)
            {
                this.AppendAbbreviation(zcharacter, calledRecursively, ref zcharacters, ref zsciiText);
                return;
            }

            base.ZCharactersToZscii(calledRecursively, zcharacter, currentAlphabet, ref nextAlphabet, ref lockedAlphabet, ref zcharacters, ref zsciiText);
        }

        /// <summary>
        /// Converts a zscii character to z-characters.
        /// </summary>
        /// <param name="zsciiCharacter">
        /// The zscii character.
        /// </param>
        /// <param name="zsciiText">
        /// The zscii text following the zscii character.
        /// </param>
        /// <param name="lockedAlphabet">
        /// The locked alphabet.
        /// </param>
        /// <param name="characters">
        /// The z-characters to add the results to.
        /// </param>
        /// <remarks>
        /// Newline is now part of the zscii alphabet.
        /// </remarks>
        protected override void ZsciiCharacterToZCharacters(Zscii zsciiCharacter, ImmutableStack<Zscii> zsciiText, ref byte lockedAlphabet, ref ImmutableStack<byte> characters)
        {
            if (zsciiCharacter == Zscii.NewLine)
            {
                this.ZsciiAlphabetCharacterToZCharacters(zsciiCharacter, zsciiText, ref lockedAlphabet, ref characters);
                return;
            }

            base.ZsciiCharacterToZCharacters(zsciiCharacter, zsciiText, ref lockedAlphabet, ref characters);
        }
    }
}