using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;

namespace IFInterfaces.Support
{
    public interface IBasicSoundSupport
    {
        bool SupportsSoundPlayback { get; }
        void DefineSound(int soundNum, [ReadOnlyArray] byte[] buffer);
        IAsyncAction PlayAsync(int soundNum);
        IAsyncAction PlayBuffer([ReadOnlyArray] byte[] buffer);
    }
}
