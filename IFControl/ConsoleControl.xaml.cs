using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using IFInterfaces;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace IFControls
{
    public sealed partial class ConsoleControl : UserControl, IIFControl, IIFTextControl
    {
        #region from BaseIFControl
        private double findWidth()
        {
            Func<FrameworkElement, double> recurs = (el) => 0d;
            recurs = (FrameworkElement parent) =>
            {
                if (parent == null)
                    return Windows.UI.Core.CoreWindow.GetForCurrentThread().Bounds.Width;
                return parent.Width > 0 ? parent.Width : parent.ActualWidth <= 0 ? recurs(parent.Parent as FrameworkElement) : parent.ActualWidth;
            };
            return recurs(this.Parent as FrameworkElement);
        }

        private double findHeight()
        {
            Func<FrameworkElement, double> recurs = (el) => 0d;
            recurs = (FrameworkElement parent) =>
            {
                if (parent == null)
                    return Windows.UI.Core.CoreWindow.GetForCurrentThread().Bounds.Height;
                return parent.Height > 0 ? parent.Height : parent.ActualHeight <= 0 ? recurs(parent.Parent as FrameworkElement) : parent.ActualHeight;
            };
            return recurs(this.Parent as FrameworkElement);
        }

        private Panel rootPanel { get; set; }

        public int WidthOfAChar
        {
            get;
            private set;
        }

        public int HeightOfAChar
        {
            get;
            private set;
        }

        //public bool IsScrollable
        //{
        //    get { return false; }
        //    set { }
        //}

        //public bool AllowInput
        //{
        //    get { return false; }
        //    set { }
        //}

        public Color CurrentTextColor
        {
            get;
            set;
        }

        Color _CurrentBGColor = Colors.Black;
        public Color CurrentBGColor
        {
            get
            {
                return _CurrentBGColor;
            }
            set
            {
                _CurrentBGColor = value;
            }
        }

        public FontFamily CurrentFont
        {
            get;
            set;
        }

        public double CurrentFontSize
        {
            get;
            set;
        }

        public int ColumnCount
        {
            get;
            set;
        }

        public int RowCount
        {
            get;
            set;
        }

        public int CurrentRow
        {
            get { return -1; }
            set { }
        }

        public int CurrentColumn
        {
            get { return -1; }
            set { }
        }

        public bool BlockQuoteOn
        {
            get;
            set;
        }

        public bool FixedWidthFontOn
        {
            get;
            set;
        }

        //protected abstract void ReCalcCells();

        public bool IsUnderline { get; set; }

        public bool IsBold { get; set; }

        public bool IsItalic { get; set; }

        public bool IsStrikethrough { get; set; }

        //public void Write(char ch) { }

        //public void Write(string text) { }

        //public void WriteLine(string text) { }

        //public Task<char> GetCharAsync() { return Task.FromResult('\0'); }

        //public Task<string> GetInputAsync() { return Task.FromResult("\0"); }

        private object getFormattedText(string text)
        {
            Inline runResult;
            var run = new Run()
            {
                Text = text,
                FontFamily = CurrentFont,
                Foreground = new SolidColorBrush(CurrentTextColor),
                FontWeight = IsBold ? FontWeights.Bold : FontWeights.Normal,
                FontStyle = IsItalic ? FontStyle.Italic : FontStyle.Normal,
            };
            if (CurrentFontSize > 0)
                run.FontSize = CurrentFontSize;
            runResult = run;
            if (IsUnderline)
            {
                Span underline = new Underline();
                underline.Inlines.Add(run);
                runResult = underline;
            }


            if (!(rootPanel.Background is SolidColorBrush) || CurrentBGColor != ((SolidColorBrush)rootPanel.Background).Color)
            {
                var tb = new TextBlock();
                tb.TextWrapping = TextWrapping.WrapWholeWords;
                tb.Inlines.Add(runResult);

                var border = new Border();
                border.Background = new SolidColorBrush(CurrentBGColor);
                border.Child = tb;

                ////if(IsStrikethrough)
                ////{
                ////    var b = new Border();
                ////    b.BorderThickness = new Thickness(0,1,0,0);
                ////    b.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
                ////    b.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
                ////    b.BorderBrush = new SolidColorBrush(CurrentTextColor);
                ////    innerGrid.Children.Add(b);
                ////}

                return border;
            }
            return (Inline)runResult;
        }
        #endregion

        public ConsoleControl()
        {
            #region from BaseIFControl
            CurrentTextColor = ((SolidColorBrush)this.Foreground).Color;
            CurrentFont = this.FontFamily;
            CurrentFontSize = this.FontSize;
            rootPanel = (Panel)this.Content;
            WidthOfAChar = 1;
            HeightOfAChar = 1;
            #endregion
            this.InitializeComponent();
            CurrentTextColor = ((SolidColorBrush)tbInput.Foreground).Color;
            CurrentBGColor = ((SolidColorBrush)rootLayout.Background).Color;
            CurrentFont = tbInput.FontFamily;
            CurrentFontSize = 20; // this.FontSize;
            tbInput.KeyUp += tbInput_KeyUp;
            tbInput.GotFocus += tbInput_GotFocus;

            var fontSzRatio = CurrentFontSize / (double)10;
            WidthOfAChar = (int)Math.Floor((CONSOLAS_FONT.WidthAt10Px * fontSzRatio) + .5);
            HeightOfAChar = (int)Math.Floor((CONSOLAS_FONT.HeightAt10Px * fontSzRatio) + .5);

            rootLayout.RowDefinitions[0].MaxHeight = CoreWindow.GetForCurrentThread().Bounds.Height * .66;

            SizeChanged += ConsoleControl_SizeChanged;
        }

        void ConsoleControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ReCalcCells();
        }

        public void ReCalcCells()
        {
            var actualWidth = Width > 0 ? Width : ActualWidth <= 0 ? findWidth() : ActualWidth;
            var actualHeight = Height > 0 ? Height : ActualHeight <= 0 ? findHeight() : ActualHeight;
            ColumnCount = (int)Math.Floor(actualWidth / WidthOfAChar);
            RowCount = (int)Math.Floor(actualHeight / HeightOfAChar);
           
        }

        public bool IsScrollable
        {
            get
            {
                return //sv.HorizontalScrollBarVisibility != ScrollBarVisibility.Hidden && sv.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled &&
                       sv.VerticalScrollBarVisibility != ScrollBarVisibility.Hidden && sv.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled;
            }
            set
            {
                //sv.HorizontalScrollBarVisibility = value ? ScrollBarVisibility.Visible : ScrollBarVisibility.Hidden;
                sv.VerticalScrollBarVisibility = value ? ScrollBarVisibility.Visible : ScrollBarVisibility.Hidden;
            }
        }
        public bool AllowInput
        {
            get
            {
                return txtPrompt.Visibility != Windows.UI.Xaml.Visibility.Collapsed;
            }
            set
            {
                tbInput.Visibility = txtPrompt.Visibility = (!value ? Windows.UI.Xaml.Visibility.Collapsed : Windows.UI.Xaml.Visibility.Visible);
            }
        }
        void tbInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!inEditMode)
            {
                delayedBlur();
            }
        }

        bool inEditMode = false;

        const int blankLines = 2;
        //private void outp(Run r)
        //{
        //    var count = rtbConsole.Blocks.Count - blankLines; // ignore blank lines at end
        //    var idx = count - 1;
        //    ((Paragraph)rtbConsole.Blocks[idx]).Inlines.Add(r);
        //    CauseScroll();
        //}
        private void outp(Inline r)
        {
            var count = rtbConsole.Blocks.Count - blankLines; // ignore blank lines at end
            var idx = count - 1;
            var p = ((Paragraph)rtbConsole.Blocks[0]);
            var loc = p.Inlines.Count - 3;
            p.Inlines.Insert(loc, r);
            CauseScroll();

        }

        //private void outp(Paragraph p) 
        //{
        //    var idx = rtbConsole.Blocks.Count - blankLines; // ignore blank line at end
        //    rtbConsole.Blocks.Insert(idx, p);
        //    CauseScroll();
        //}

        public void Write(char ch)
        {
            if (ch == '\n' || ch == '\r')
            {
                // create new line
                outp(new LineBreak());
                return;
            }

            var newRun = createRun(new string(new[] { ch }));
            if (newRun != null)
                outp(newRun);
            CauseScroll();
        }
        
        public void Write(string text)
        {
            if (text.Length <= 0)
                return;
            var lines = text.Split('\n');
            for (var lidx = 0; lidx < lines.Length - 1; lidx++)
            {
                var ln = lines[lidx];
                WriteLine(ln);
            }
            var line = lines[lines.Length - 1];
            if (text.EndsWith("\n"))
                WriteLine(line);
            else
            {
                var newRun = createRun(line);
                if (newRun != null)
                    outp(newRun);
            }
            CauseScroll();
        }
        public void WriteLine(string text)
        {
            if (text.Trim('\n').IndexOf("\n") > -1)
            {
                var lines = text.Split('\n');
                foreach (var line in lines)
                {
                    WriteLine(text);
                }
                return;
            }
            if (text.Equals(" "))
            {
                var spcRun = createRun(" ");
                if (spcRun != null)
                    outp(spcRun);
                outp(new LineBreak());
                return;
            }
            if (BlockQuoteOn)
                text = "     " + text;
            var run = createRun(text);
            if (run != null)
                outp(run);
            outp(new LineBreak());
            CauseScroll();
        }

        private Inline createRun(string text)
        {
            var ctl = getFormattedText(text);
            if (ctl is Inline)
                return (Inline)ctl;
            else if (ctl is UIElement)
            {
                var result = new InlineUIContainer();
                result.Child = (UIElement)ctl;
                return result;
            }
            return null;
        }

        void CauseScroll()
        {
            if (IsScrollable)
            {
                if (sv.ScrollableHeight > 0)
                    sv.ChangeView(null, sv.ScrollableHeight, null);
                else if (rtbConsole.ActualHeight > sv.ActualHeight)
                    sv.ChangeView(null, rtbConsole.ActualHeight - sv.ActualHeight, null);
            }
        }

        //string lastInput = "";
        //Stack<char> buffer;

        public IAsyncOperation<char> GetCharAsync()
        {
            return GetCharAsyncTask().AsAsyncOperation();
            
        }

        private async Task<char> GetCharAsyncTask()
        {
            if (!AllowInput)
                return '\0';
            tbInput.Text = "";
            inEditMode = true;
            tbInput.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            while (tbInput.Text.Length == 0)
            {
                //await Task.Yield();
                await Task.Delay(20);
            }
            inEditMode = false;

            var result = tbInput.Text.ToArray()[0];
            blur();
            return result;
        }

        bool gotEnter = false;

        public IAsyncOperation<string> GetInputAsync()
        {
            return GetInputAsyncTask().AsAsyncOperation();
        }

        private async Task<string> GetInputAsyncTask()
        {
            if (!AllowInput)
                return await Task.FromResult("\0");

            inEditMode = true;
            gotEnter = false;
            tbInput.Text = "";
            tbInput.Focus(FocusState.Programmatic);
            while (!gotEnter)
            {
                //                await Task.Yield();
                await Task.Delay(20);
            }
            var result = tbInput.Text;
            inEditMode = false;
            tbInput.Text = "";
            blur();
            return result;
        }
        async void delayedBlur()
        {
            await Task.Delay(10);
            blur();
        }

        void blur()
        {
            sv.Focus(FocusState.Programmatic);
        }

        void tbInput_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (!inEditMode)
            {
                e.Handled = true;
                return;
            }
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                gotEnter = true;
            }
            e.Handled = false;
        }
    }
}
