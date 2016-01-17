using System;
using System.Linq;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace IFControl.Interfaces.Support
{
    public interface IComplexTextSupport : IScreenSetup
    {
        bool SupportsComplexTextOutput { get; }
        bool SupportsComplexTextInput { get; }
        bool HideTextInput { get; set; }
        FontFamily[] Fonts { get; }
        int AddFont(FontFamily f);
        void SetFont(int displayRegion, int fontNum);
        void SetFontSize(int displayRegion, int fontSize);
        void SetBackgroundColor(int displayRegion, Color color);
        void SetForegroundColor(int displayRegion, Color color);
        void SetIsUnderline(int displayRegion, bool value);
        void SetIsBold(int displayRegion, bool value);
        void SetIsItalic(int displayRegion, bool value);
        void SetIsStrikethrough(int displayRegion, bool value);
        void SetBlockQuoteOn(int ctrlNum, bool value);
        void SetFixedWidth(int displayRegion, bool value);
        void Write(int displayRegion, char c);
        [DefaultOverloadAttribute]
        void Write(int displayRegion, string s);
        void WriteLine(int displayRegion, string text);
    }
}