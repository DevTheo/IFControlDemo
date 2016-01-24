using System;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace IFInterfaces
{
    public interface IIFControl
    {
        int WidthOfAChar
        {
            get;
        }

        int HeightOfAChar
        {
            get;
        }

        void Write(char ch);

        [DefaultOverloadAttribute]
        void Write(string text);

        void WriteLine(string text);

        IAsyncOperation<string> GetInputAsync();

        Color CurrentTextColor { get; set;}
        Color CurrentBGColor { get; set; }
        FontFamily CurrentFont { get; set; }
        double CurrentFontSize { get; set; }
        bool BlockQuoteOn { get; set; }
        bool FixedWidthFontOn { get; set; }
        bool IsUnderline { get; set; }
        bool IsBold { get; set; }
        bool IsItalic { get; set; }
        bool IsStrikethrough { get; set; }
        IAsyncOperation<char> GetCharAsync();
        bool IsScrollable { get; }
        bool AllowInput { get; }

    }

}
