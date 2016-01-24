using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SharpCompress.Archive;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using IFInterfaces.Support;
using IFInterfaces.Services;

namespace IFCore.Services
{
    internal class BaseImplemention : IIFRuntime
    {
        protected IArchive archive = null;
        public BaseImplemention()
        {
            controls = new List<FrameworkElement>();
        }
        #region IScreenSetup
        public virtual int CharHeight { get { return 1; } }
        public virtual int CharWidth { get { return 1; } }

        public virtual bool SupportsMultiWin { get { return false; } }
        public Grid RootUI { get; set; }
        internal List<FrameworkElement> controls { get; set; }
        public virtual string ControlQLocation(int controlNum) { return "5"; }
        
        public virtual int CreateRegion(WindowType winType, string qlocation, int width, int height, bool scrollable)
        {
            return CreateRegion(winType, qlocation, width, height, scrollable, -1, -1);
        }
        public virtual int CreateRegion(WindowType winType, string qlocation, int width, int height, bool scrollable, int rows)
        {
            return CreateRegion(winType, qlocation, width, height, scrollable, rows, -1);
        }
        public virtual int CreateRegion(WindowType winType, string qlocation, int width, int height, bool scrollable, int rows, int cols)
        {
            return -1;
        }
        public virtual int CreateMainText(string qlocation)
        {
            return -1;
        }
        public virtual TupleIntInt GetRowsColumns(int controlNum)
        {
            return new TupleIntInt(23, 40);
        }

        public virtual int SetTextRegion(string qlocation, int width, int height, bool scrollable)
        {
            return SetTextRegion(qlocation, width, height, scrollable, -1, -1);
        }

        public virtual int SetTextRegion(string qlocation, int width, int height, bool scrollable, int rows)
        {
            return SetTextRegion(qlocation, width, height, scrollable, rows, -1);
        }
        public virtual int SetTextRegion(string qlocation, int width, int height, bool scrollable, int rows , int cols)
        {
            return -1;
        }

        public virtual int SetGridTextRegion(string qlocation, int width, int height, bool scrollable)
        {
            return SetGridTextRegion(qlocation, width, height, scrollable, -1, -1);
        }
        public virtual int SetGridTextRegion(string qlocation, int width, int height, bool scrollable, int rows)
        {
            return SetGridTextRegion(qlocation, width, height, scrollable, rows, -1);
        }
        public virtual int SetGridTextRegion(string qlocation, int width, int height, bool scrollable, int rows = -1, int cols = -1)
        {
            return -1;
        }

        public virtual int SetGraphicRegion(string qlocation, int width, int height)
        {
            return -1;
        }

        public virtual void DestroyRegion(int regionNum)
        {
            return;
        }

        #endregion

        #region ISimpleTextOnlySupport
        public virtual bool SupportsBasicTextInputOutput { get { return false; } }

        public virtual IAsyncOperation<char> InputCharAsync()
        {
            return InputCharAsyncTask().AsAsyncOperation();
        }
        public virtual IAsyncOperation<char> InputCharAsync(int timeout)
        {
            return InputCharAsyncTask(timeout).AsAsyncOperation();
        }
        internal async Task<char> InputCharAsyncTask(int timeout = -1)
        {
            await Task.Delay(1);
            return (char)0;
        }

        public virtual char InputChar()
        {
            return InputChar(-1);
        }
        public virtual char InputChar(int timeout)
        {
            return (char)0;
        }

        public virtual IAsyncOperation<string> GetLineAsync()
        {
            return GetLineAsync(-1);
        }
        public virtual  IAsyncOperation<string> GetLineAsync(int timeout)
        {
            return GetLineAsyncTask(timeout).AsAsyncOperation();
        }
        internal async Task<string> GetLineAsyncTask(int timeout = -1)
        {
            await Task.Delay(1);
            return "";
        }

        public virtual string GetLine()
        {
            return GetLine(-1);
        }
        public virtual string GetLine(int timeout)
        {
            return "";
        }

        public virtual void Write(char c)
        {

        }
        public virtual void Write(string str, params object[] parms)
        {
        }

        public virtual void WriteLine(string text, params object[] parms)
        {

        }
        public virtual void Log(string text, params object[] parms)
        {
            if (this is IDebugInfo && ((IDebugInfo) this).SupportsLogging)
            {
                ((IDebugInfo) this).Debug.LogDebug(text, parms);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(text, parms);
            }
        }
        public virtual void SetTextColor(Windows.UI.Color c)
        {

        }

        public virtual void SetBgColor(Windows.UI.Color c)
        {

        }
        #endregion

        #region IComplexTextSupport
        public virtual bool SupportsComplexTextOutput { get { return false; } }
        public virtual bool SupportsComplexTextInput { get { return false; } }

        public virtual bool HideTextInput { get; set; }

        internal List<Windows.UI.Xaml.Media.FontFamily> _Fonts = new List<FontFamily>();
        public virtual Windows.UI.Xaml.Media.FontFamily[] Fonts { get { return _Fonts.ToArray(); } }
        public virtual int AddFont(FontFamily f) { _Fonts.Add(f); return _Fonts.Count - 1; }
        public virtual void SetFont(int displayRegion, int fontNum) { }
        public virtual void SetFontSize(int displayRegion, int fontSize) { }
        public virtual void SetBackgroundColor(int displayRegion, Color color) { }
        public virtual void SetForegroundColor(int displayRegion, Color color) { }
        public virtual void SetIsUnderline(int displayRegion, bool value) { }
        public virtual void SetIsBold(int displayRegion, bool value) { }
        public virtual void SetIsItalic(int displayRegion, bool value) { }
        public virtual void SetIsStrikethrough(int displayRegion, bool value) { }
        public virtual void MoveControlToGrid(int regionNum) { }
        public virtual void SetBlockQuoteOn(int ctrlNum, bool value) { }
        public virtual void SetFixedWidth(int displayRegion, bool value) { }
        public virtual bool IsGridText(int controlNum)
        {
            return false;
        }
        public virtual void SetCursorLocation(int controlNum, int row, int col)
        {
        }
        public virtual void Write(int displayRegion, char c)
        {

        }
        public virtual void Write(int displayRegion, string s)
        {

        }
        public virtual void WriteLine(int displayRegion, string text)
        {

        }
        #endregion

        #region IBasicImageSupport
        public virtual bool SupportsImageDisplay { get { return false; } }

        public virtual List<byte[]> Images
        {
            get { return null; }
        }

        public virtual void SetImage(int imageNum, byte[] imageData)
        {
        }

        public virtual void DrawPictureByNum(int imageNum)
        {
            DrawPictureByNum(imageNum, -1, -1, -1, -1);
        }
        public virtual void DrawPictureByNum(int imageNum, int x)
        {
            DrawPictureByNum(imageNum, x, -1, -1, -1);
        }
        public virtual void DrawPictureByNum(int imageNum, int x, int y)
        {
            DrawPictureByNum(imageNum, x, y, -1, -1);
        }
        public virtual void DrawPictureByNum(int imageNum, int x, int y, int width)
        {
            DrawPictureByNum(imageNum, x, y, width, -1);
        }
        public virtual void DrawPictureByNum(int imageNum, int x, int y, int width, int height)
        {
        }

        public virtual void DrawPicture(byte[] imageData)
        {
            DrawPicture(imageData, -1, -1, -1, -1);
        }
        public virtual void DrawPicture(byte[] imageData, int x)
        {
            DrawPicture(imageData, x, -1, -1, -1);
        }
        public virtual void DrawPicture(byte[] imageData, int x, int y)
        {
            DrawPicture(imageData, x, y, -1, -1);
        }
        public virtual void DrawPicture(byte[] imageData, int x, int y, int width)
        {
            DrawPicture(imageData, x, y, width, -1);
        }
        public virtual void DrawPicture(byte[] imageData, int x = -1, int y = -1, int width = -1, int height = -1)
        {
        }
        #endregion

        #region IAnimationSupport
        public virtual bool SupportsAnimations { get { return false; } }

        protected List<AnimationFrame[]> Animations
        {
            get { return null; }
        }

        public virtual void SetAnimation(int animationNum, AnimationFrame[] AnimationFrames)
        {

        }

        public virtual IAsyncAction PlayAnimationByNumAsync(int animationNum)
        {
            return PlayAnimationByNumAsyncTask(animationNum).AsAsyncAction();
        }
        internal Task PlayAnimationByNumAsyncTask(int animationNum)
        {
            return new Task(() => { });
        }

        public virtual IAsyncAction PlayAnimationAsync(AnimationFrame[] AnimationFrames)
        {
            return PlayAnimationAsyncTask(AnimationFrames).AsAsyncAction();
        }
        internal Task PlayAnimationAsyncTask(AnimationFrame[] AnimationFrames)
        {
            return new Task(() => { });
        }
        #endregion

        #region IBasicSoundSupport
        public virtual bool SupportsSoundPlayback { get { return false; } }

        public virtual void DefineSound(int soundNum, byte[] buffer)
        {

        }

        public virtual IAsyncAction PlayAsync(int soundNum)
        {
            return PlayAsyncTask(soundNum).AsAsyncAction();
        }
        internal virtual Task PlayAsyncTask(int soundNum)
        {
            return new Task(() => { });
        }

        public virtual IAsyncAction PlayBuffer(byte[] buffer)
        {
            return PlayBufferTask(buffer).AsAsyncAction();
        }
        internal virtual Task PlayBufferTask(byte[] buffer)
        {
            return new Task(() => { });
        }
        #endregion

        #region IFileStorageSupport
        public virtual bool SupportsSaveLoadGame { get { return false; } }
        public virtual IFInterfaces.Services.IFileService FileIO { get; set; }

        #endregion

        #region IOtherInputSupport
        public virtual bool SupportsMouseInput { get { return false; } }
        public virtual bool SupportsJoystickInput { get { return false; } }

        public virtual MouseInputData GetCurrentMouseData()
        {
            return new MouseInputData();
        }

        public virtual IAsyncOperation<MouseInputData> GetAnyMouseClickAsync()
        {
            return GetAnyMouseClickAsyncTask().AsAsyncOperation();
        }
        public virtual IAsyncOperation<MouseInputData> GetAnyMouseClickAsync(int timeout)
        {
            return GetAnyMouseClickAsyncTask(timeout).AsAsyncOperation();
        }
        internal virtual Task<MouseInputData> GetAnyMouseClickAsyncTask(int timeout = -1)
        {
            return new Task<MouseInputData>(() => new MouseInputData());
        }

        public virtual IAsyncOperation<MouseInputData> GetLeftMouseClickAsync()
        {
            return GetLeftMouseClickAsyncTask().AsAsyncOperation();
        }
        public virtual IAsyncOperation<MouseInputData> GetLeftMouseClickAsync(int timeout)
        {
            return GetLeftMouseClickAsyncTask(timeout).AsAsyncOperation();
        }
        internal virtual Task<MouseInputData> GetLeftMouseClickAsyncTask(int timeout = -1)
        {
            return new Task<MouseInputData>(() => new MouseInputData());
        }

        public virtual IAsyncOperation<MouseInputData> GetMiddleMouseClickAsync()
        {
            return GetMiddleMouseClickAsyncTask().AsAsyncOperation();
        }
        public virtual IAsyncOperation<MouseInputData> GetMiddleMouseClickAsync(int timeout = -1)
        {
            return GetMiddleMouseClickAsyncTask(timeout).AsAsyncOperation();
        }
        internal virtual Task<MouseInputData> GetMiddleMouseClickAsyncTask(int timeout = -1)
        {
            return new Task<MouseInputData>(() => new MouseInputData());
        }

        public virtual IAsyncOperation<MouseInputData> GetRightMouseClickAsync()
        {
            return GetRightMouseClickAsyncTask().AsAsyncOperation();
        }
        public virtual IAsyncOperation<MouseInputData> GetRightMouseClickAsync(int timeout = -1)
        {
            return GetRightMouseClickAsyncTask(timeout).AsAsyncOperation();
        }
        public virtual Task<MouseInputData> GetRightMouseClickAsyncTask(int timeout = -1)
        {
            return new Task<MouseInputData>(() => new MouseInputData());
        }

        public virtual MouseInputData GetLeftMouseClick()
        {
            return GetLeftMouseClick(-1);
        }
        public virtual MouseInputData GetLeftMouseClick(int timeout)
        {
            return new MouseInputData();
        }

        public virtual MouseInputData GetRightMouseClick()
        {
            return GetRightMouseClick(-1);
        }
        public virtual MouseInputData GetRightMouseClick(int timeout)
        {
            return new MouseInputData();
        }

        public virtual MouseInputData GetMiddleMouseClick()
        {
            return GetMiddleMouseClick(-1);
        }
        public virtual MouseInputData GetMiddleMouseClick(int timeout)
        {
            return new MouseInputData();
        }
        public virtual MouseInputData GetAnyMouseClick()
        {
            return GetAnyMouseClick(-1);
        }
        public virtual MouseInputData GetAnyMouseClick(int timeout)
        {
            return new MouseInputData();
        }

        public virtual JoystickInputData[] GetCurrentJoystickData()
        {
            return null;
        }

        public virtual IAsyncOperation<JoystickInputData> GetJoystickAnyButtonClickAsync(int joyStickNum)
        {
            return GetJoystickAnyButtonClickAsyncTask(joyStickNum).AsAsyncOperation();
        }
        public virtual IAsyncOperation<JoystickInputData> GetJoystickAnyButtonClickAsync(int joyStickNum, int timeout)
        {
            return GetJoystickAnyButtonClickAsyncTask(joyStickNum, timeout).AsAsyncOperation();
        }
        internal virtual Task<JoystickInputData> GetJoystickAnyButtonClickAsyncTask(int joyStickNum, int timeout = -1)
        {
            return new Task<JoystickInputData>(() => new JoystickInputData());
        }

        public virtual IAsyncOperation<JoystickInputData> GetJoystickButtonClickAsync(int joyStickNum, int buttonNum)
        {
            return GetJoystickButtonClickAsyncTask(joyStickNum, buttonNum).AsAsyncOperation();
        }
        public virtual IAsyncOperation<JoystickInputData> GetJoystickButtonClickAsync(int joyStickNum, int buttonNum, int timeout = -1)
        {
            return GetJoystickButtonClickAsyncTask(joyStickNum, buttonNum, timeout).AsAsyncOperation();
        }
        internal virtual Task<JoystickInputData> GetJoystickButtonClickAsyncTask(int joyStickNum, int buttonNum, int timeout = -1)
        {
            return new Task<JoystickInputData>(() => new JoystickInputData());
        }

        public virtual JoystickInputData GetJoystickAnyButtonClick(int joyStickNum)
        {
            return GetJoystickAnyButtonClick(joyStickNum, - 1);
        }
        public virtual JoystickInputData GetJoystickAnyButtonClick(int joyStickNum, int timeout)
        {
            return new JoystickInputData();
        }

        public virtual JoystickInputData GetJoystickButtonClick(int joyStickNum, int buttonNum)
        {
            return GetJoystickButtonClick(joyStickNum, buttonNum, -1);
        }
        public virtual JoystickInputData GetJoystickButtonClick(int joyStickNum, int buttonNum, int timeout = -1)
        {
            return new JoystickInputData();
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
            get;set;
        }
        #endregion

    }
}
