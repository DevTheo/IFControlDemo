using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using IFInterfaces.Support;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media;
using IFInterfaces.Services;

namespace IFCore.Services
{
    public sealed class EmptyImplementation : IIFRuntime
    {
        readonly BaseImplemention baseImpl = new BaseImplemention();

        #region IScreenSetup
        public int CharHeight
        {
            get
            {
                return baseImpl.CharHeight;
            }
        }

        public int CharWidth
        {
            get
            {
                return baseImpl.CharWidth;
            }
        }


        public bool SupportsMultiWin
        {
            get
            {
                return baseImpl.SupportsMultiWin;
            }
        }

        public string ControlQLocation(int controlNum)
        {
            return baseImpl.ControlQLocation(controlNum);
        }

        public int CreateMainText(string qlocation)
        {
            return baseImpl.CreateMainText(qlocation);
        }

        public int CreateRegion(WindowType winType, string qlocation, int width, int height, bool scrollable)
        {
            return baseImpl.CreateRegion(winType, qlocation, width, height, scrollable);
        }

        public int CreateRegion(WindowType winType, string qlocation, int width, int height, bool scrollable, int rows)
        {
            return baseImpl.CreateRegion(winType, qlocation, width, height, scrollable, rows);
        }

        public int CreateRegion(WindowType winType, string qlocation, int width, int height, bool scrollable, int rows, int cols)
        {
            return baseImpl.CreateRegion(winType, qlocation, width, height, scrollable, rows, cols);
        }

        public void DestroyRegion(int regionNum)
        {
            baseImpl.DestroyRegion(regionNum);
        }
        public TupleIntInt GetRowsColumns(int controlNum)
        {
            return baseImpl.GetRowsColumns(controlNum);
        }

        public bool IsGridText(int controlNum)
        {
            return baseImpl.IsGridText(controlNum);
        }

        public void MoveControlToGrid(int regionNum)
        {
            baseImpl.MoveControlToGrid(regionNum);
        }

        public void SetCursorLocation(int controlNum, int row, int col)
        {
            baseImpl.SetCursorLocation(controlNum, row, col);
        }

        public int SetGraphicRegion(string qlocation, int width, int height)
        {
            return baseImpl.SetGraphicRegion(qlocation, width, height);
        }

        public int SetGridTextRegion(string qlocation, int width, int height, bool scrollable)
        {
            return baseImpl.SetGridTextRegion(qlocation, width, height, scrollable);
        }

        public int SetGridTextRegion(string qlocation, int width, int height, bool scrollable, int rows)
        {
            return baseImpl.SetGridTextRegion(qlocation, width, height, scrollable, rows);
        }

        public int SetGridTextRegion(string qlocation, int width, int height, bool scrollable, int rows, int cols)
        {
            return baseImpl.SetGridTextRegion(qlocation, width, height, scrollable, rows, cols);
        }

        public int SetTextRegion(string qlocation, int width, int height, bool scrollable)
        {
            return baseImpl.SetTextRegion(qlocation, width, height, scrollable);
        }

        public int SetTextRegion(string qlocation, int width, int height, bool scrollable, int rows)
        {
            return baseImpl.SetTextRegion(qlocation, width, height, scrollable, rows);
        }

        public int SetTextRegion(string qlocation, int width, int height, bool scrollable, int rows, int cols)
        {
            return baseImpl.SetTextRegion(qlocation, width, height, scrollable, rows, cols);
        }
        #endregion

        #region ISimpleTextOnlySupport
        public bool SupportsBasicTextInputOutput
        {
            get
            {
                return baseImpl.SupportsBasicTextInputOutput;
            }
        }


        public string GetLine()
        {
            return baseImpl.GetLine();
        }

        public string GetLine(int timeout)
        {
            return baseImpl.GetLine(timeout);
        }

        public IAsyncOperation<string> GetLineAsync()
        {
            return baseImpl.GetLineAsync();
        }

        public IAsyncOperation<string> GetLineAsync(int timeout)
        {
            return baseImpl.GetLineAsync(timeout);
        }

        public char InputChar()
        {
            return baseImpl.InputChar();
        }

        public char InputChar(int timeout)
        {
            return baseImpl.InputChar(timeout);
        }

        public IAsyncOperation<char> InputCharAsync()
        {
            return baseImpl.InputCharAsync();
        }

        public IAsyncOperation<char> InputCharAsync(int timeout)
        {
            return baseImpl.InputCharAsync(timeout);
        }

        public void Log(string text, params object[] parms)
        {
            baseImpl.Log(text, parms);
        }

        public void SetBgColor(Color c)
        {
            baseImpl.SetBgColor(c);
        }

        public void SetTextColor(Color c)
        {
            baseImpl.SetTextColor(c);
        }

        public void Write(char c)
        {
            baseImpl.Write(c);
        }

        public void Write(string str, params object[] parms)
        {
            baseImpl.Write(str, parms);
        }

        public void WriteLine(string text, params object[] parms)
        {
            baseImpl.WriteLine(text, parms);
        }

        #endregion

        #region IComplexTextSupport
        public bool SupportsComplexTextOutput
        {
            get
            {
                return baseImpl.SupportsComplexTextOutput;
            }
        }

        public bool SupportsComplexTextInput
        {
            get
            {
                return baseImpl.SupportsComplexTextInput;
            }
        }

        public bool HideTextInput
        {
            get
            {
                return baseImpl.HideTextInput;
            }

            set
            {
                baseImpl.HideTextInput = value;
            }
        }

        public FontFamily[] Fonts
        {
            get
            {
                return baseImpl.Fonts;
            }
        }

        public int AddFont(FontFamily f)
        {
            return baseImpl.AddFont(f);
        }

        public void SetFont(int displayRegion, int fontNum)
        {
            baseImpl.SetFont(displayRegion, fontNum);
        }

        public void SetFontSize(int displayRegion, int fontSize)
        {
            baseImpl.SetFontSize(displayRegion, fontSize);
        }

        public void SetBackgroundColor(int displayRegion, Color color)
        {
            baseImpl.SetBackgroundColor(displayRegion, color);
        }

        public void SetForegroundColor(int displayRegion, Color color)
        {
            baseImpl.SetForegroundColor(displayRegion, color);
        }

        public void SetIsUnderline(int displayRegion, bool value)
        {
            baseImpl.SetIsUnderline(displayRegion, value);
        }

        public void SetIsBold(int displayRegion, bool value)
        {
            baseImpl.SetIsBold(displayRegion, value);
        }

        public void SetIsItalic(int displayRegion, bool value)
        {
            baseImpl.SetIsItalic(displayRegion, value);
        }

        public void SetIsStrikethrough(int displayRegion, bool value)
        {
            baseImpl.SetIsStrikethrough(displayRegion, value);
        }

        public void SetBlockQuoteOn(int ctrlNum, bool value)
        {
            baseImpl.SetBlockQuoteOn(ctrlNum, value);
        }

        public void SetFixedWidth(int displayRegion, bool value)
        {
            baseImpl.SetBlockQuoteOn(displayRegion, value);
        }

        public void Write(int displayRegion, char c)
        {
            baseImpl.Write(displayRegion, c);
        }

        public void Write(int displayRegion, string s)
        {
            baseImpl.Write(displayRegion, s);
        }

        public void WriteLine(int displayRegion, string text)
        {
            baseImpl.WriteLine(displayRegion, text);
        }
        #endregion

        #region IBasicImageSupport
        public bool SupportsImageDisplay
        {
            get
            {
                return baseImpl.SupportsImageDisplay;
            }
        }

        public void SetImage(int imageNum, [ReadOnlyArray] byte[] imageData)
        {
            baseImpl.SetImage(imageNum, imageData);
        }

        public void DrawPictureByNum(int imageNum)
        {
            baseImpl.DrawPictureByNum(imageNum);
        }

        public void DrawPictureByNum(int imageNum, int x)
        {
            baseImpl.DrawPictureByNum(imageNum, x);
        }

        public void DrawPictureByNum(int imageNum, int x, int y)
        {
            baseImpl.DrawPictureByNum(imageNum, x, y);
        }

        public void DrawPictureByNum(int imageNum, int x, int y, int width)
        {
            baseImpl.DrawPictureByNum(imageNum, x, y, width);
        }

        public void DrawPictureByNum(int imageNum, int x, int y, int width, int height)
        {
            baseImpl.DrawPictureByNum(imageNum, x, y, width, height);
        }

        public void DrawPicture([ReadOnlyArray] byte[] imageData)
        {
            baseImpl.DrawPicture(imageData);
        }

        public void DrawPicture([ReadOnlyArray] byte[] imageData, int x)
        {
            baseImpl.DrawPicture(imageData, x);
        }

        public void DrawPicture([ReadOnlyArray] byte[] imageData, int x, int y)
        {
            baseImpl.DrawPicture(imageData, x, y);
        }

        public void DrawPicture([ReadOnlyArray] byte[] imageData, int x, int y, int width)
        {
            baseImpl.DrawPicture(imageData, x, y, width);
        }

        public void DrawPicture([ReadOnlyArray] byte[] imageData, int x, int y, int width, int height)
        {
            baseImpl.DrawPicture(imageData, x, y, width, height);
        }
        #endregion

        #region IAnimationSupport
        public bool SupportsAnimations
        {
            get
            {
                return baseImpl.SupportsAnimations;
            }
        }

        public void SetAnimation(int animationNum, [ReadOnlyArray] AnimationFrame[] AnimationFrames)
        {
            baseImpl.SetAnimation(animationNum, AnimationFrames);
        }

        public IAsyncAction PlayAnimationByNumAsync(int animationNum)
        {
            return baseImpl.PlayAnimationByNumAsync(animationNum); ;
        }

        public IAsyncAction PlayAnimationAsync([ReadOnlyArray] AnimationFrame[] AnimationFrames)
        {
            return baseImpl.PlayAnimationAsync(AnimationFrames);
        }
        #endregion

        #region IBasicSoundSupport
        public bool SupportsSoundPlayback
        {
            get
            {
                return baseImpl.SupportsSoundPlayback;
            }
        }

        public void DefineSound(int soundNum, [ReadOnlyArray] byte[] buffer)
        {
            baseImpl.DefineSound(soundNum, buffer);
        }

        public IAsyncAction PlayAsync(int soundNum)
        {
            return baseImpl.PlayAsync(soundNum);
        }

        public IAsyncAction PlayBuffer([ReadOnlyArray] byte[] buffer)
        {
            return baseImpl.PlayBuffer(buffer);
        }
        #endregion

        #region IFileStorageSupport
        public bool SupportsSaveLoadGame
        {
            get
            {
                return baseImpl.SupportsSaveLoadGame;
            }
        }

        public IFileService FileIO { get; set; }
        #endregion

        #region IOtherInputSupport

        public bool SupportsMouseInput
        {
            get
            {
                return baseImpl.SupportsMouseInput;
            }
        }

        public bool SupportsJoystickInput
        {
            get
            {
                return baseImpl.SupportsJoystickInput;
            }
        }

        public MouseInputData GetCurrentMouseData()
        {
            return baseImpl.GetCurrentMouseData();
        }

        public IAsyncOperation<MouseInputData> GetAnyMouseClickAsync()
        {
            return baseImpl.GetAnyMouseClickAsync();
        }

        public IAsyncOperation<MouseInputData> GetAnyMouseClickAsync(int timeout)
        {
            return baseImpl.GetAnyMouseClickAsync(timeout);
        }

        public IAsyncOperation<MouseInputData> GetLeftMouseClickAsync()
        {
            return baseImpl.GetLeftMouseClickAsync();
        }

        public IAsyncOperation<MouseInputData> GetLeftMouseClickAsync(int timeout)
        {
            return baseImpl.GetLeftMouseClickAsync(timeout);
        }

        public IAsyncOperation<MouseInputData> GetMiddleMouseClickAsync()
        {
            return baseImpl.GetMiddleMouseClickAsync();
        }

        public IAsyncOperation<MouseInputData> GetMiddleMouseClickAsync(int timeout)
        {
            return baseImpl.GetMiddleMouseClickAsync(timeout);
        }

        public IAsyncOperation<MouseInputData> GetRightMouseClickAsync()
        {
            return baseImpl.GetRightMouseClickAsync();
        }

        public IAsyncOperation<MouseInputData> GetRightMouseClickAsync(int timeout)
        {
            return baseImpl.GetRightMouseClickAsync(timeout);
        }

        public MouseInputData GetLeftMouseClick()
        {
            return baseImpl.GetLeftMouseClick();
        }

        public MouseInputData GetLeftMouseClick(int timeout)
        {
            return baseImpl.GetLeftMouseClick(timeout);
        }

        public MouseInputData GetRightMouseClick()
        {
            return baseImpl.GetRightMouseClick();
        }

        public MouseInputData GetRightMouseClick(int timeout)
        {
            return baseImpl.GetRightMouseClick(timeout);
        }

        public MouseInputData GetMiddleMouseClick()
        {
            return baseImpl.GetMiddleMouseClick();
        }

        public MouseInputData GetMiddleMouseClick(int timeout)
        {
            return baseImpl.GetMiddleMouseClick(timeout);
        }

        public MouseInputData GetAnyMouseClick()
        {
            return baseImpl.GetAnyMouseClick();
        }

        public MouseInputData GetAnyMouseClick(int timeout)
        {
            return baseImpl.GetAnyMouseClick(timeout);
        }

        public JoystickInputData[] GetCurrentJoystickData()
        {
            return baseImpl.GetCurrentJoystickData();
        }

        public IAsyncOperation<JoystickInputData> GetJoystickAnyButtonClickAsync(int joyStickNum, int timeout)
        {
            return baseImpl.GetJoystickAnyButtonClickAsync(joyStickNum, timeout);
        }

        public IAsyncOperation<JoystickInputData> GetJoystickAnyButtonClickAsync(int joyStickNum)
        {
            return baseImpl.GetJoystickAnyButtonClickAsync(joyStickNum);
        }

        public IAsyncOperation<JoystickInputData> GetJoystickButtonClickAsync(int joyStickNum, int buttonNum, int timeout)
        {
            return baseImpl.GetJoystickButtonClickAsync(joyStickNum, buttonNum, timeout);
        }

        public IAsyncOperation<JoystickInputData> GetJoystickButtonClickAsync(int joyStickNum, int buttonNum)
        {
            return baseImpl.GetJoystickButtonClickAsync(joyStickNum, buttonNum);
        }

        public JoystickInputData GetJoystickAnyButtonClick(int joyStickNum)
        {
            return baseImpl.GetJoystickAnyButtonClick(joyStickNum);
        }

        public JoystickInputData GetJoystickAnyButtonClick(int joyStickNum, int timeout)
        {
            return baseImpl.GetJoystickAnyButtonClick(joyStickNum, timeout);
        }

        public JoystickInputData GetJoystickButtonClick(int joyStickNum, int buttonNum)
        {
            return baseImpl.GetJoystickButtonClick(joyStickNum, buttonNum);
        }

        public JoystickInputData GetJoystickButtonClick(int joyStickNum, int buttonNum, int timeout)
        {
            return baseImpl.GetJoystickButtonClick(joyStickNum, buttonNum, timeout);
        }

        #endregion

        #region IDebugInfo
        public bool SupportsLogging
        {
            get
            {
                return false;
            }
        }
        public IDebugService Debug
        {
            get; set;
        }
        #endregion

    }
}
