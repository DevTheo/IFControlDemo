// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Zscii.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   Zscii characters.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// Zscii characters.
    /// </summary>
    internal enum Zscii : ushort
    {
        /// <summary>
        /// The character ampersand.
        /// </summary>
        Ampersand = 38, 

        /// <summary>
        /// The character asterisk.
        /// </summary>
        Asterisk = 42, 

        /// <summary>
        /// The character back slash.
        /// </summary>
        BackSlash = 92, 

        /// <summary>
        /// The character caret.
        /// </summary>
        Caret = 94, 

        /// <summary>
        /// The character colon.
        /// </summary>
        Colon = 58, 

        /// <summary>
        /// The character comma.
        /// </summary>
        Comma = 44, 

        /// <summary>
        /// The character commercial at.
        /// </summary>
        CommercialAt = 64, 

        /// <summary>
        /// The character cursor down.
        /// </summary>
        CursorDown = 130, 

        /// <summary>
        /// The character cursor left.
        /// </summary>
        CursorLeft = 131, 

        /// <summary>
        /// The character cursor right.
        /// </summary>
        CursorRight = 132, 

        /// <summary>
        /// The character cursor up.
        /// </summary>
        CursorUp = 129, 

        /// <summary>
        /// The character delete.
        /// </summary>
        Delete = 8, 

        /// <summary>
        /// The character dollar sign.
        /// </summary>
        DollarSign = 36, 

        /// <summary>
        /// The character double click.
        /// </summary>
        DoubleClick = 253, 

        /// <summary>
        /// The character double quote.
        /// </summary>
        DoubleQuote = 34, 

        /// <summary>
        /// The character equal sign.
        /// </summary>
        EqualSign = 61, 

        /// <summary>
        /// The character escape.
        /// </summary>
        Escape = 27, 

        /// <summary>
        /// The character exclamation point.
        /// </summary>
        ExclamationPoint = 33, 

        /// <summary>
        /// The character F1.
        /// </summary>
        F1 = 133, 

        /// <summary>
        /// The character F10.
        /// </summary>
        F10 = 142, 

        /// <summary>
        /// The character F11.
        /// </summary>
        F11 = 143, 

        /// <summary>
        /// The character F12.
        /// </summary>
        F12 = 144, 

        /// <summary>
        /// The character F2.
        /// </summary>
        F2 = 134, 

        /// <summary>
        /// The character F3.
        /// </summary>
        F3 = 135, 

        /// <summary>
        /// The character F4.
        /// </summary>
        F4 = 136, 

        /// <summary>
        /// The character F5.
        /// </summary>
        F5 = 137, 

        /// <summary>
        /// The character F6.
        /// </summary>
        F6 = 138, 

        /// <summary>
        /// The character F7.
        /// </summary>
        F7 = 139, 

        /// <summary>
        /// The character F8.
        /// </summary>
        F8 = 140, 

        /// <summary>
        /// The character F9.
        /// </summary>
        F9 = 141, 

        /// <summary>
        /// The first unicode character.
        /// </summary>
        FirstUnicodeCharacter = 155, 

        /// <summary>
        /// The character forward slash.
        /// </summary>
        ForwardSlash = 47, 

        /// <summary>
        /// The character grave accent.
        /// </summary>
        GraveAccent = 96, 

        /// <summary>
        /// The character greater than.
        /// </summary>
        GreaterThan = 62, 

        /// <summary>
        /// The character hyphen.
        /// </summary>
        Hyphen = 45, 

        /// <summary>
        /// The character left brace.
        /// </summary>
        LeftBrace = 123, 

        /// <summary>
        /// The character left parentheis.
        /// </summary>
        LeftParenthesis = 40, 

        /// <summary>
        /// The character left square bracket.
        /// </summary>
        LeftSquareBracket = 91, 

        /// <summary>
        /// The character less than.
        /// </summary>
        LessThan = 60, 

        /// <summary>
        /// The character lowercase a.
        /// </summary>
        LowercaseA = 97, 

        /// <summary>
        /// The character lowercase b.
        /// </summary>
        LowercaseB = 98, 

        /// <summary>
        /// The character lowercase c.
        /// </summary>
        LowercaseC = 99, 

        /// <summary>
        /// The character lowercase d.
        /// </summary>
        LowercaseD = 100, 

        /// <summary>
        /// The character lowercase e.
        /// </summary>
        LowercaseE = 101, 

        /// <summary>
        /// The character lowercase f.
        /// </summary>
        LowercaseF = 102, 

        /// <summary>
        /// The character lowercase g.
        /// </summary>
        LowercaseG = 103, 

        /// <summary>
        /// The character lowercase h.
        /// </summary>
        LowercaseH = 104, 

        /// <summary>
        /// The character lowercase i.
        ///   The character i.
        /// </summary>
        LowercaseI = 105, 

        /// <summary>
        /// The character lowercase j.
        /// </summary>
        LowercaseJ = 106, 

        /// <summary>
        /// The character lowercase k.
        /// </summary>
        LowercaseK = 107, 

        /// <summary>
        /// The character lowercase l.
        /// </summary>
        LowercaseL = 108, 

        /// <summary>
        /// The character lowercase m.
        /// </summary>
        LowercaseM = 109, 

        /// <summary>
        /// The character lowercase n.
        /// </summary>
        LowercaseN = 110, 

        /// <summary>
        /// The character lowercase o.
        /// </summary>
        LowercaseO = 111, 

        /// <summary>
        /// The character lowercase p.
        /// </summary>
        LowercaseP = 112, 

        /// <summary>
        /// The character lowercase q.
        /// </summary>
        LowercaseQ = 113, 

        /// <summary>
        /// The character lowercase r.
        /// </summary>
        LowercaseR = 114, 

        /// <summary>
        /// The character lowercase s.
        /// </summary>
        LowercaseS = 115, 

        /// <summary>
        /// The character lowercase t.
        /// </summary>
        LowercaseT = 116, 

        /// <summary>
        /// The character lowercase u.
        /// </summary>
        LowercaseU = 117, 

        /// <summary>
        /// The character lowercase v.
        /// </summary>
        LowercaseV = 118, 

        /// <summary>
        /// The character lowercase w.
        /// </summary>
        LowercaseW = 119, 

        /// <summary>
        /// The character lowercase x.
        /// </summary>
        LowercaseX = 120, 

        /// <summary>
        /// The character lowercase y.
        /// </summary>
        LowercaseY = 121, 

        /// <summary>
        /// The character lowercase z.
        /// </summary>
        LowercaseZ = 122, 

        /// <summary>
        /// The character menu click.
        /// </summary>
        MenuClick = 252, 

        /// <summary>
        /// The character new line.
        /// </summary>
        NewLine = 13, 

        /// <summary>
        /// The character null.
        /// </summary>
        Null = 0, 

        /// <summary>
        /// The character zero.
        /// </summary>
        Number0 = 48, 

        /// <summary>
        /// The character one.
        /// </summary>
        Number1 = 49, 

        /// <summary>
        /// The character two.
        /// </summary>
        Number2 = 50, 

        /// <summary>
        /// The character three.
        /// </summary>
        Number3 = 51, 

        /// <summary>
        /// The character four.
        /// </summary>
        Number4 = 52, 

        /// <summary>
        /// The character five.
        /// </summary>
        Number5 = 53, 

        /// <summary>
        /// The character six.
        /// </summary>
        Number6 = 54, 

        /// <summary>
        /// The character seven.
        /// </summary>
        Number7 = 55, 

        /// <summary>
        /// The character eight.
        /// </summary>
        Number8 = 56, 

        /// <summary>
        /// The character nine.
        /// </summary>
        Number9 = 57, 

        /// <summary>
        /// The character number pad zero.
        /// </summary>
        NumberPad0 = 145, 

        /// <summary>
        /// The character number pad one.
        /// </summary>
        NumberPad1 = 146, 

        /// <summary>
        /// The character number pad two.
        /// </summary>
        NumberPad2 = 147, 

        /// <summary>
        /// The character number pad three.
        /// </summary>
        NumberPad3 = 148, 

        /// <summary>
        /// The character number pad four.
        /// </summary>
        NumberPad4 = 149, 

        /// <summary>
        /// The character number pad five.
        /// </summary>
        NumberPad5 = 150, 

        /// <summary>
        /// The character number pad six.
        /// </summary>
        NumberPad6 = 151, 

        /// <summary>
        /// The character number pad seven.
        /// </summary>
        NumberPad7 = 152, 

        /// <summary>
        /// The character number pad eight.
        /// </summary>
        NumberPad8 = 153, 

        /// <summary>
        /// The character number pad nine.
        /// </summary>
        NumberPad9 = 154, 

        /// <summary>
        /// The character number sign.
        /// </summary>
        NumberSign = 35, 

        /// <summary>
        /// The character percent sign.
        /// </summary>
        PercentSign = 37, 

        /// <summary>
        /// The character period.
        /// </summary>
        Period = 46, 

        /// <summary>
        /// The character plus.
        /// </summary>
        Plus = 43, 

        /// <summary>
        /// The character question mark.
        /// </summary>
        QuestionMark = 63, 

        /// <summary>
        /// The character right brace.
        /// </summary>
        RightBrace = 125, 

        /// <summary>
        /// The character right parenthesis.
        /// </summary>
        RightParenthesis = 41, 

        /// <summary>
        /// The character right square bracket.
        /// </summary>
        RightSquareBracket = 93, 

        /// <summary>
        /// The character semicolon.
        /// </summary>
        Semicolon = 59, 

        /// <summary>
        /// The character sentence space.
        /// </summary>
        SentenceSpace = 11, 

        /// <summary>
        /// The character single click.
        /// </summary>
        SingleClick = 254, 

        /// <summary>
        /// The character single quote.
        /// </summary>
        SingleQuote = 39, 

        /// <summary>
        /// The character space.
        /// </summary>
        Space = 32, 

        /// <summary>
        /// The character tab.
        /// </summary>
        Tab = 9, 

        /// <summary>
        /// The character tilde.
        /// </summary>
        Tilde = 126, 

        /// <summary>
        /// The character underscore.
        /// </summary>
        Underscore = 95, 

        /// <summary>
        /// The character uppercase A.
        /// </summary>
        UppercaseA = 65, 

        /// <summary>
        /// The character uppercase B.
        /// </summary>
        UppercaseB = 66, 

        /// <summary>
        /// The character uppercase C.
        /// </summary>
        UppercaseC = 67, 

        /// <summary>
        /// The character uppercase D.
        /// </summary>
        UppercaseD = 68, 

        /// <summary>
        /// The character uppercase E.
        /// </summary>
        UppercaseE = 69, 

        /// <summary>
        /// The character uppercase F.
        /// </summary>
        UppercaseF = 70, 

        /// <summary>
        /// The character uppercase G.
        /// </summary>
        UppercaseG = 71, 

        /// <summary>
        /// The character uppercase H.
        /// </summary>
        UppercaseH = 72, 

        /// <summary>
        /// The character uppercase I.
        /// </summary>
        UppercaseI = 73, 

        /// <summary>
        /// The character uppercase J.
        /// </summary>
        UppercaseJ = 74, 

        /// <summary>
        /// The character uppercase K.
        /// </summary>
        UppercaseK = 75, 

        /// <summary>
        /// The character uppercase L.
        /// </summary>
        UppercaseL = 76, 

        /// <summary>
        /// The character uppercase M.
        /// </summary>
        UppercaseM = 77, 

        /// <summary>
        /// The character uppercase N.
        /// </summary>
        UppercaseN = 78, 

        /// <summary>
        /// The character uppercase O.
        /// </summary>
        UppercaseO = 79, 

        /// <summary>
        /// The character uppercase P.
        /// </summary>
        UppercaseP = 80, 

        /// <summary>
        /// The character uppercase Q.
        /// </summary>
        UppercaseQ = 81, 

        /// <summary>
        /// The character uppercase R.
        /// </summary>
        UppercaseR = 82, 

        /// <summary>
        /// The character uppercase S.
        /// </summary>
        UppercaseS = 83, 

        /// <summary>
        /// The character uppercase T.
        /// </summary>
        UppercaseT = 84, 

        /// <summary>
        /// The character uppercase U.
        /// </summary>
        UppercaseU = 85, 

        /// <summary>
        /// The character uppercase V.
        /// </summary>
        UppercaseV = 86, 

        /// <summary>
        /// The character uppercase W.
        /// </summary>
        UppercaseW = 87, 

        /// <summary>
        /// The character uppercase X.
        /// </summary>
        UppercaseX = 88, 

        /// <summary>
        /// The character uppercase Y.
        /// </summary>
        UppercaseY = 89, 

        /// <summary>
        /// The character uppercase Z.
        /// </summary>
        UppercaseZ = 90, 

        /// <summary>
        /// The character vertical line.
        /// </summary>
        VerticalLine = 124
    }
}