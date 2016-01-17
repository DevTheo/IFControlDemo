using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;

namespace IFInterfaces.Support
{
    public sealed class AnimationFrame
    {
        public int Step { get; set; }
        public byte[] ImageData { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public interface IAnimationSupport
    {
        bool SupportsAnimations { get; }
        //IList Animations { get; } // List<AnimationFrame[]>

        void SetAnimation(int animationNum, [ReadOnlyArray] AnimationFrame[] AnimationFrames);
        IAsyncAction PlayAnimationByNumAsync(int animationNum);
        IAsyncAction PlayAnimationAsync([ReadOnlyArray] AnimationFrame[] AnimationFrames);
    }
}
