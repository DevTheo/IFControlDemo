using IFInterfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace IFCore.Services
{
    public sealed partial class SimpleRuntimeInterface
    {
        IIFTextControl _ui;
        public SimpleRuntimeInterface(IIFTextControl ui)
        {
            _ui = ui;
        }

        #region ISimpleTextOnlySupport
        public bool SupportsBasicTextInputOutput { get { return true; } }
        internal async Task<char> InputCharAsyncTask(int timeout = -1)
        {
            if (timeout == -1)
            {
                return await _ui.GetCharAsync().AsTask();
            }
            var task = _ui.GetCharAsync().AsTask();
            var current = DateTime.Now;
            while (!task.IsCompleted || !task.IsCanceled || !task.IsFaulted || current.Subtract(DateTime.Now).Ticks <= timeout)
            {
                await Task.Delay(10);
            }
            if (task.IsCompleted)
                return task.Result;
            return (char)0;
        }
        public char InputChar(/*int timeout = -1*/)
        {
            return InputChar(-1);
        }
        public char InputChar(int timeout)
        {
            var t = InputCharAsyncTask(timeout);
            t.Wait(timeout);
            if (t.IsCompleted)
                return t.Result;
            return (char)0;
        }
        
        internal async Task<string> GetLineAsyncTask(int timeout = -1)
        {
            if (timeout == -1)
            {
                return await _ui.GetInputAsync().AsTask();
            }
            var task = _ui.GetInputAsync().AsTask();
            var current = DateTime.Now;
            while (!task.IsCompleted || !task.IsCanceled || !task.IsFaulted || current.Subtract(DateTime.Now).Ticks <= timeout)
            {
                await Task.Delay(10);
            }
            if (task.IsCompleted)
                return task.Result;
            return "";
        }
        public string GetLine(/* int timeout = -1 */)
        {
            return GetLine(-1);
        }
        public string GetLine(int timeout)
        {
            var t = GetLineAsyncTask(timeout);
            t.Wait(timeout);
            if (t.IsCompleted)
                return t.Result;
            return "";
        }

        public void Write(char c)
        {
            if (c == '\n')
                WriteLine("");
            else
                _ui.Write(c);
        }
        public void Write(string str, params object[] parms)
        {
            if (parms.Length > 0)
                str = String.Format(str, parms);
            var lines = str.Split('\n');
            for (int i = 0; i < lines.Length - 1; i++)
            {
                WriteLine(lines[i]);
            }
            if (str.EndsWith("\n"))
                WriteLine(lines[lines.Length - 1]);
            else
            {
                foreach (var chr in lines[lines.Length - 1])
                {
                    Write(chr);
                }
            }
        }

        public void WriteLine(string text, params object[] parms)
        {
            if (parms.Length > 0)
                text = String.Format(text, parms);
            _ui.WriteLine(text);
        }

        public void SetTextColor(Windows.UI.Color c)
        {
            _ui.CurrentTextColor = c;
        }

        public void SetBgColor(Windows.UI.Color c)
        {
            _ui.CurrentBGColor = c;
        }
        #endregion

    }
}
