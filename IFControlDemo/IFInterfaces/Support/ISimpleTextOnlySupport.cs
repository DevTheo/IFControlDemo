using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI;

namespace IFInterfaces.Support
{
    public interface ISimpleTextOnlySupport
    {
        bool SupportsBasicTextInputOutput { get; }

        [DefaultOverloadAttribute]
        IAsyncOperation<char> InputCharAsync(/* int timeout = -1 */);
        IAsyncOperation<char> InputCharAsync(int timeout);

        [DefaultOverloadAttribute]
        char InputChar(/*int timeout = -1*/);
        char InputChar(int timeout);

        [DefaultOverloadAttribute]
        IAsyncOperation<string> GetLineAsync(/*int timeout = -1*/);
        IAsyncOperation<string> GetLineAsync(int timeout);

        [DefaultOverloadAttribute]
        string GetLine(/*int timeout = -1*/);
        string GetLine(int timeout);

        void Write(char c);
        void Write(string str, params object[] parms);
        void WriteLine(string text, params object[] parms);
        void Log(string text, params object[] parms);
        void SetTextColor(Color c);
        void SetBgColor(Color c);
    }
}
