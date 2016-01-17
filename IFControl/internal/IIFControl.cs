using System;
using System.Linq;
using Windows.Foundation.Metadata;

namespace IFControls.Interfaces
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
    }

}
