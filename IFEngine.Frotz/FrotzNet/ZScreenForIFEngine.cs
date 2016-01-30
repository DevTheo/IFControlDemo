using Frotz.Screen;
using IFInterfaces.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frotz.Blorb;

namespace IFEngine.ZMachineF.FrotzNet
{
    class ZScreenForIFEngine : IZScreen
    {
        IIFRuntime runtime;
        public ZScreenForIFEngine(IIFRuntime runtime)
        {
            runtime = runtime;
        }

        public event EventHandler<ZKeyPressEventArgs> KeyPressed;

        public void addInputChar(char c)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            
        }

        public void ClearArea(int top, int left, int bottom, int right)
        {
            
        }

        public void DisplayChar(char c)
        {
            runtime.Write(c);
        }

        public void DisplayMessage(string Message, string Caption)
        {
            
        }

        public void DrawPicture(int picture, byte[] Image, int y, int x)
        {
            if (picture > 0)
            {
                if (Image.Length > 0)
                {
                    runtime.SetImage(picture, Image);
                }
                runtime.DrawPictureByNum(picture, x, y);
            }
            else if (Image.Length > 0)
            {
                runtime.DrawPicture(Image, x, y);
            }
        }

        public void FinishWithSample(int number)
        {
            
        }

        public void GetColor(out int foreground, out int background)
        {
            foreground = 0xffffff; // runtime.GetForeground();
            background = 0x000000; // runtime.GetBackground();
        }

        public ZPoint GetCursorPosition()
        {
            
        }

        public bool GetFontData(int font, ref ushort height, ref ushort width)
        {
            throw new NotImplementedException();
        }

        public ZSize GetImageInfo(byte[] Image)
        {
            throw new NotImplementedException();
        }

        public ScreenMetrics GetScreenMetrics()
        {
            throw new NotImplementedException();
        }

        public int GetStringWidth(string s, CharDisplayInfo Font)
        {
            throw new NotImplementedException();
        }

        public void HandleFatalError(string Message)
        {
            throw new NotImplementedException();
        }

        public string OpenExistingFile(string defaultName, string Title, string Filter)
        {
            throw new NotImplementedException();
        }

        public string OpenNewOrExistingFile(string defaultName, string Title, string Filter, string defaultExtension)
        {
            throw new NotImplementedException();
        }

        public ushort PeekColor()
        {
            throw new NotImplementedException();
        }

        public void PrepareSample(int number)
        {
            throw new NotImplementedException();
        }

        public void RefreshScreen()
        {
            throw new NotImplementedException();
        }

        public void RemoveChars(int count)
        {
            throw new NotImplementedException();
        }

        public void ScrollArea(int top, int bottom, int left, int right, int units)
        {
            throw new NotImplementedException();
        }

        public void ScrollLines(int top, int height, int lines)
        {
            throw new NotImplementedException();
        }

        public string SelectGameFile(out byte[] filedata)
        {
            throw new NotImplementedException();
        }

        public void SetActiveWindow(int win)
        {
            throw new NotImplementedException();
        }

        public void SetColor(int new_foreground, int new_background)
        {
            throw new NotImplementedException();
        }

        public void SetCursorPosition(int x, int y)
        {
            throw new NotImplementedException();
        }

        public void SetFont(int font)
        {
            throw new NotImplementedException();
        }

        public void SetInputColor()
        {
            throw new NotImplementedException();
        }

        public void SetInputMode(bool InputMode, bool CursorVisibility)
        {
            throw new NotImplementedException();
        }

        public void SetTextStyle(int new_style)
        {
            throw new NotImplementedException();
        }

        public void SetWindowSize(int win, int top, int left, int height, int width)
        {
            throw new NotImplementedException();
        }

        public bool ShouldWrap()
        {
            throw new NotImplementedException();
        }

        public void StartSample(int number, int volume, int repeats, ushort eos)
        {
            throw new NotImplementedException();
        }

        public void StopSample(int number)
        {
            throw new NotImplementedException();
        }

        public void StoryStarted(string StoryName, Blorb BlorbFile)
        {
            throw new NotImplementedException();
        }
    }
}
