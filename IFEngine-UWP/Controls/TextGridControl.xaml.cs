using System;
using System.Linq;
using System.Threading.Tasks;
using IFCore;
using IFInterfaces;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace IFEngine_UWP.Controls
{
    public sealed partial class TextGridControl : UserControl, IIFTextGridControl
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

        public bool IsScrollable
        {
            get { return false; }
            internal set { }
        }

        public bool AllowInput
        {
            get { return false; }
            internal set { }
        }

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

        //public int ColumnCount
        //{
        //    get;
        //    set;
        //}

        //public int RowCount
        //{
        //    get;
        //    set;
        //}

        //public int CurrentRow
        //{
        //    get { return -1; }
        //    set { }
        //}

        //public int CurrentColumn
        //{
        //    get { return -1; }
        //    set { }
        //}

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

        public IAsyncOperation<char> GetCharAsync()
        {
            return GetCharAsyncTask().AsAsyncOperation();
        }
        private Task<char> GetCharAsyncTask() { return Task.FromResult('\0'); }

        public IAsyncOperation<string> GetInputAsync()
        {
            return GetInputAsyncTask().AsAsyncOperation();
        }
        private Task<string> GetInputAsyncTask()
        {
            return Task.FromResult("\0");
        }

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

        public TextGridControl()
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
            var BgColor = (Application.Current.RequestedTheme == ApplicationTheme.Dark) ? Colors.Black : Colors.White;

            CurrentTextColor = BgColor == Colors.White ? Colors.Black : Colors.White;
            CurrentBGColor = BgColor;
            FontSize = this.FontSize;
            FontFamily = this.FontFamily;

            rootPanel = textGridLayout;
            var fontSzRatio = this.FontSize / (double)10;
            WidthOfAChar = (int)Math.Floor((CONSOLAS_FONT.WidthAt10Px * fontSzRatio) + .5);
            HeightOfAChar = (int)Math.Floor((CONSOLAS_FONT.HeightAt10Px * fontSzRatio) + .5);

            this.SizeChanged += TextGridControl_SizeChanged;
        }

        public int ColumnCount
        {

            get
            {
                return textGridLayout.ColumnDefinitions.Count == 0 ? 1 : textGridLayout.ColumnDefinitions.Count;
            }
            set { }
        }

        public int RowCount
        {

            get
            {
                return textGridLayout.RowDefinitions.Count == 0 ? 1 : textGridLayout.RowDefinitions.Count;
            }
            set { }
        }

        int _CurrentRow = 0;
        public int CurrentRow
        {
            get { return _CurrentRow; }
            set
            {
                _CurrentRow = (value >= RowCount ? 0 : value);
            }
        }
        int _CurrentCol = 0;
        public int CurrentColumn
        {
            get { return _CurrentCol; }
            set
            {
                if (value >= ColumnCount)
                {
                    _CurrentCol = 0;
                    CurrentRow++;
                }
                else
                    _CurrentCol = value;
            }
        }

        public void ReCalcCells()
        {
            var actualWidth = this.Width > 0 ? this.Width : this.ActualWidth <= 0 ? findWidth() : this.ActualWidth;
            var actualHeight = this.Height > 0 ? this.Height : this.ActualHeight <= 0 ? findHeight() : this.ActualHeight;
            var numCols = Math.Min((int)Math.Floor(actualWidth / WidthOfAChar), 120);
            var numRows = Math.Min((int)Math.Floor(actualHeight / HeightOfAChar), 23);
            textGridLayout.ColumnDefinitions.Clear();
            for (var i = 0; i < numCols; i++)
                textGridLayout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            textGridLayout.RowDefinitions.Clear();
            for (var i = 0; i < numRows; i++)
                textGridLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
        }

        void TextGridControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ReCalcCells();
        }

        public void ClearRegion()
        {
            textGridLayout.Children.Clear();
        }

        const string kCtlPrefix = "ctl_";
        private void RemoveControlAtCursor()
        {
            var ctl = (UIElement)textGridLayout.FindName(getCtlName());
            if (ctl != null)
                textGridLayout.Children.Remove(ctl);
        }

        private string getCtlName()
        {
            return kCtlPrefix + CurrentRow + "_" + CurrentColumn;
        }

        public void Write(char ch)
        {
            if (ch == '\n' || ch == '\r')
            {
                CurrentRow++;
                CurrentColumn = 0;
                return;
            }
            RemoveControlAtCursor();
            var ctl = getFormattedText(new String(new char[] { ch }));
            FrameworkElement tb = null;
            if (ctl is FrameworkElement)
            {
                tb = (FrameworkElement)ctl;
                ((FrameworkElement)tb).Name = getCtlName();
            }
            else if (ctl is Inline)
            {
                tb = new TextBlock();
                ((TextBlock)tb).Name = getCtlName();
                ((TextBlock)tb).Inlines.Add((Inline)ctl);
            }
            if (tb != null)
            {
                tb.SetValue(Grid.ColumnProperty, CurrentColumn);
                tb.SetValue(Grid.RowProperty, CurrentRow);

                textGridLayout.Children.Add(tb);
            }
            CurrentColumn++;
        }
        public void Write(string text)
        {
            foreach (var ch in text.ToCharArray())
                Write(ch);
        }
        public void WriteLine(string text)
        {
            foreach (var ch in text.ToCharArray())
                Write(ch);
            if (!text.EndsWith("\n"))
                Write('\n');
        }

    }
}
