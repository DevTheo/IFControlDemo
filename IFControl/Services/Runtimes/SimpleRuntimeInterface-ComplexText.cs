using System;
using System.Linq;
using Windows.UI;

namespace IFCore.Services
{
    public sealed partial class SimpleRuntimeInterface
    {
        #region IComplexTextSupport
        public bool SupportsComplexTextOutput { get { return true; } }

        public void SetBlockQuoteOn(int ctrlNum, bool value)
        {
            actionOnIFControl(ctrlNum, (cc) => cc.BlockQuoteOn = value);
        }

        public void SetFixedWidth(int displayRegion, bool value)
        {
            actionOnIFControl(displayRegion, (cc) => cc.FixedWidthFontOn = value);
        }
                
        public void SetBackgroundColor(int displayRegion, Color color)
        {
            actionOnIFControl(displayRegion, (cc) => cc.CurrentBGColor = color);
        }

        public void SetForegroundColor(int displayRegion, Color color)
        {
            actionOnIFControl(displayRegion, (cc) => cc.CurrentTextColor = color);
        }

        public void SetFont(int displayRegion, int fontNum)
        {
            if (Fonts == null || fontNum >= baseImpl._Fonts.Count)
                return;
            actionOnIFControl(displayRegion, (cc) => cc.CurrentFont = baseImpl._Fonts[fontNum]);
        }

        public void SetFontSize(int displayRegion, int fontSize)
        {
            actionOnIFControl(displayRegion, (cc) => cc.CurrentFontSize = fontSize);
        }

        public void SetIsUnderline(int displayRegion, bool value)
        {
            actionOnIFControl(displayRegion, (cc) => cc.IsUnderline = value);
        }

        public void SetIsBold(int displayRegion, bool value)
        {
            actionOnIFControl(displayRegion, (cc) => cc.IsBold = value);
        }

        public void SetIsItalic(int displayRegion, bool value)
        {
            actionOnIFControl(displayRegion, (cc) => cc.IsItalic = value);
        }

        public void SetIsStrikethrough(int displayRegion, bool value)
        {
            actionOnIFControl(displayRegion, (cc) => cc.IsStrikethrough = value);
        }

        public void Write(int displayRegion, char c)
        {
            actionOnIFControl(displayRegion, (cc) => cc.Write(c));
        }
        public void Write(int displayRegion, string s)
        {
            actionOnIFControl(displayRegion, (cc) => cc.Write(s));
        }

        public void WriteLine(int displayRegion, string text)
        {
            actionOnIFControl(displayRegion, (cc) => cc.WriteLine(text));
        }
        #endregion
    }
}
