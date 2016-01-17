using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Metadata;

namespace IFControl.Interfaces.Support
{
    public struct MouseInputData
    {
        public int X;
        public int Y;
        public bool LeftButtonIsDown;
        public bool MiddleButtonIsDown;
        public bool RightButtonIsDown;
        public int ScrollPos;
    }

    public sealed class JoystickInputData
    {
        public bool[] ButtonState { get; set; }
        public int[] XAxisPos { get; set; }
        public int[] YAxisPos { get; set; }
    }

    public interface IOtherInputSupport
    {
        bool SupportsMouseInput { get; }
        bool SupportsJoystickInput { get; }

        MouseInputData GetCurrentMouseData();

        [DefaultOverloadAttribute]
        IAsyncOperation<MouseInputData> GetAnyMouseClickAsync(/*int timeout = -1*/);
        IAsyncOperation<MouseInputData> GetAnyMouseClickAsync(int timeout);

        [DefaultOverloadAttribute]
        IAsyncOperation<MouseInputData> GetLeftMouseClickAsync(/*int timeout = -1*/);
        IAsyncOperation<MouseInputData> GetLeftMouseClickAsync(int timeout);

        [DefaultOverloadAttribute]
        IAsyncOperation<MouseInputData> GetMiddleMouseClickAsync(/*int timeout = -1*/);
        IAsyncOperation<MouseInputData> GetMiddleMouseClickAsync(int timeout);

        [DefaultOverloadAttribute]
        IAsyncOperation<MouseInputData> GetRightMouseClickAsync(/*int timeout = -1*/);
        IAsyncOperation<MouseInputData> GetRightMouseClickAsync(int timeout);

        [DefaultOverloadAttribute]
        MouseInputData GetLeftMouseClick(/*int timeout = -1*/);
        MouseInputData GetLeftMouseClick(int timeout);

        [DefaultOverloadAttribute]
        MouseInputData GetRightMouseClick(/*int timeout = -1*/);
        MouseInputData GetRightMouseClick(int timeout);

        [DefaultOverloadAttribute]
        MouseInputData GetMiddleMouseClick(/*int timeout = -1*/);
        MouseInputData GetMiddleMouseClick(int timeout);

        [DefaultOverloadAttribute]
        MouseInputData GetAnyMouseClick(/*int timeout = -1*/);
        MouseInputData GetAnyMouseClick(int timeout);

        JoystickInputData[] GetCurrentJoystickData();
        [DefaultOverloadAttribute]
        IAsyncOperation<JoystickInputData> GetJoystickAnyButtonClickAsync(int joyStickNum, int timeout);
        IAsyncOperation<JoystickInputData> GetJoystickAnyButtonClickAsync(int joyStickNum /*, int timeout = -1*/);

        [DefaultOverloadAttribute]
        IAsyncOperation<JoystickInputData> GetJoystickButtonClickAsync(int joyStickNum, int buttonNum, int timeout);
        IAsyncOperation<JoystickInputData> GetJoystickButtonClickAsync(int joyStickNum, int buttonNum/*, int timeout = -1*/);

        [DefaultOverloadAttribute]
        JoystickInputData GetJoystickAnyButtonClick(int joyStickNum /*, int timeout = -1*/);
        JoystickInputData GetJoystickAnyButtonClick(int joyStickNum, int timeout);

        [DefaultOverloadAttribute]
        JoystickInputData GetJoystickButtonClick(int joyStickNum, int buttonNum /*, int timeout = -1*/);
        JoystickInputData GetJoystickButtonClick(int joyStickNum, int buttonNum, int timeout);
    }
}

