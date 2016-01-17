using System;
using System.Linq;
using IFInterfaces;
using IFInterfaces.Support;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace IFControls.Services
{
    internal static class ControlPositions
    {
        public const string PosTopLeft = "7";
        public const string PosTopCenter = "8";
        public const string PosTopRight = "9";
        public const string PosMiddleLeft = "4";
        public const string PosMiddleCenter = "5";
        public const string PosMiddleRight = "6";
        public const string PosBottomLeft = "1";
        public const string PosBottomCenter = "2";
        public const string PosBottomRight = "3";
    }

    public sealed partial class SimpleRuntimeInterface
    {
        const string kGridName = "IFGrid";

        const string kCtlName = "Ctl";
        #region IScreenSetup
        public bool SupportsMultiWin { get { return true; } }
        public int CharHeight { get { return 20; } }
        public int CharWidth { get { return 20; } }

        private bool GotMainWin = false;

        private void splitCell(Grid parent, int controlNum, FrameworkElement newItem, string newElQloc)
        {
            var existing = controls[controlNum];

            parent.Children.Remove(existing);
            string qLoc = existing.Name.Replace(kCtlName, "");
            if (newElQloc == qLoc || string.IsNullOrEmpty(newElQloc))
            {
                switch (qLoc.Substring(qLoc.Length - 1, 1))
                {
                    case ControlPositions.PosTopLeft:
                        newElQloc = ControlPositions.PosTopRight;
                        break;
                    case ControlPositions.PosTopCenter:
                        newElQloc = ControlPositions.PosBottomCenter;
                        break;
                    case ControlPositions.PosTopRight:
                        newElQloc = ControlPositions.PosTopLeft;
                        break;
                    case ControlPositions.PosMiddleLeft:
                        newElQloc = ControlPositions.PosMiddleRight;
                        break;
                    case ControlPositions.PosMiddleRight:
                        newElQloc = ControlPositions.PosMiddleLeft;
                        break;
                    case ControlPositions.PosBottomLeft:
                        newElQloc = ControlPositions.PosTopRight;
                        break;
                    case ControlPositions.PosBottomCenter:
                        newElQloc = ControlPositions.PosTopCenter;
                        break;
                    case ControlPositions.PosBottomRight:
                        newElQloc = ControlPositions.PosTopLeft;
                        break;
                    default: // We could potentially keep looping until 
                        //we had some idea where we are.. but this is good.. for now
                        newElQloc = ControlPositions.PosMiddleRight; // This handles where someone else wants the center
                        break;
                }
            }
            existing.Name += ControlPositions.PosMiddleCenter; // updating the name
            //existing.Parent = null;
            var width = 0;
            try { width = (int)existing.Width; } catch { }
            var height = 0;
            try { height = (int)existing.Height; } catch { }
            applyQuadrant(existing, qLoc + ControlPositions.PosMiddleCenter, width, height, true);

            width = 0;
            try { width = (int)newItem.Width; } catch { }
            height = 0;
            try { height = (int)newItem.Height; }
            catch { }
            applyQuadrant(newItem, qLoc + newElQloc, width, height, true);
        }

        public string ControlQLocation(int controlNum)
        {
            var qloc = "5";
            actionOnControl(controlNum, (ctl) => qloc = ctl.Name.Replace(kCtlName, ""));
            return qloc;
        }

        public bool IsGridText(int controlNum)
        {
            if (controls == null || controls.Count <= controlNum)
                return false;

            var ctl = controls[controlNum];
            return ctl is TextGridControl;
        }

        public TupleIntInt GetRowsColumns(int controlNum)
        {
            if (!IsGridText(controlNum))
                return baseImpl.GetRowsColumns(controlNum);

            var ctl = (IIFTextControl)controls[controlNum];
            return new TupleIntInt(ctl.RowCount, ctl.ColumnCount);
        }

        public void SetCursorLocation(int controlNum, int row, int col)
        {
            if (controls == null || controls.Count <= controlNum)
                return;

            var ctl = controls[controlNum];
            if (ctl is IIFTextControl)
            {
                var txtCtl = (IIFTextControl) ctl;
                txtCtl.CurrentRow = row;
                txtCtl.CurrentColumn = col;
            }
        }

        public int CreateRegion(WindowType winType, string qlocation, int width, int height, bool scrollable)
        {
            return CreateRegion(winType, qlocation, width, height, scrollable, -1, -1);
        }
        public int CreateRegion(WindowType winType, string qlocation, int width, int height, bool scrollable, int rows)
        {
            return CreateRegion(winType, qlocation, width, height, scrollable, rows, -1);
        }
        public int CreateRegion(WindowType winType, string qlocation, int width, int height, bool scrollable, int rows, int cols)
        {
            if (!SupportsMultiWin && GotMainWin)
                return -1;

            if (String.IsNullOrEmpty(qlocation))
                qlocation = "5"; // center

            if (winType == WindowType.MainTextRegion)
            {
                _ui = CreateConsoleTextControl(true, true);

                applyQuadrant(_ui, qlocation, -1, -1);
                GotMainWin = true;
                controls.Add(_ui);
                return controls.Count - 1;
            }
            if (winType == WindowType.TextRegion)
            {
                var ctl = CreateConsoleTextControl(scrollable, false);
                applyQuadrant(ctl, qlocation, width, height);
                controls.Add(ctl);
                return controls.Count - 1;
            }

            if (winType == WindowType.TextGridRegion)
            {
                var ctl = CreateConsoleGridTextControl(scrollable, false);
                applyQuadrant(ctl, qlocation, width, height);
                controls.Add(ctl);
                ctl.ReCalcCells();
                return controls.Count - 1;
            }

            if (winType == WindowType.GraphicRegion)
            {
                var ctl = new Canvas();
                applyQuadrant(ctl, qlocation, width, height);
                controls.Add(ctl);
                return controls.Count - 1;
            }
            return -1;
        }

        public void DestroyRegion(int regionNum)
        {
            if (regionNum > controls.Count)
                return;
            var ctl = controls[regionNum];
            if (ctl == null)
                return;
            var parent = ctl.Parent;
            if (parent == null)
                return;
            ((Grid)parent).Children.Remove(ctl);
            controls[regionNum] = null;
        }

        //public void MoveControlToGrid(int regionNum)
        //{
        //    if (controls.Count <= regionNum)
        //        return;

        //    var ctl = controls[regionNum];
        //    moveControlToGrid(ctl);

        //}
        //void moveControlToGrid(FrameworkElement ctl)
        //{
        //    var ctlName = ctl.Name;
        //    var qloc = ctlName.Substring(kCtlName.Length);
        //    qloc = qloc.Substring(0, qloc.Length - 3);


        //    ctl.Name = ctl.Name.Replace("__0", "") + "5" + "__0";
        //    var row = (int)ctl.GetValue(Grid.RowProperty);
        //    var col = (int)ctl.GetValue(Grid.ColumnProperty);

        //    var pgrid = (Grid)ctl.Parent;
        //    pgrid.Children.RemoveAt(0);

        //    var grid = FindOrCreateGridForLayout(pgrid, qloc);

        //    EnsureGrid(grid);
        //    qloc = qloc + "5";
        //    grid.Children.Add(ctl);
        //    ctl.SetValue(Grid.RowProperty, 1);
        //    ctl.SetValue(Grid.ColumnProperty, 1);

        //    //pgrid.Children.Add(grid);
        //    //grid.SetValue(Grid.ColumnProperty, col);
        //    //grid.SetValue(Grid.RowProperty, row);
        //}

        private void applyQuadrant(FrameworkElement ctl, string qlocation, int width, int height, bool preSized = false)
        {
            Grid parent;
            int row;
            int col;
            string loc;
            Tuple<int, int> quad;

            parent = FindParentOrCreate(qlocation);

            // Now we will be able to add the new control to the right place.
            loc = qlocation.Substring(qlocation.Length - 1, 1);
            quad = TranslateQLoc(loc);
            row = quad.Item1;
            col = quad.Item2;

            if (width > 0 && !preSized)
                ctl.Width = width * ((IIFControl)ctl).WidthOfAChar;
            else if (!preSized)
                ctl.HorizontalAlignment = HorizontalAlignment.Stretch;

            if (height > 0 && !preSized)
                ctl.Height = height * ((IIFControl)ctl).HeightOfAChar;
            else if (!preSized)
                ctl.VerticalAlignment = VerticalAlignment.Stretch;

            ctl.Name = kCtlName + qlocation; // +"__" + parent.Children.Count;

            if (parent.Children.Count > 0)
            {
                var existing = parent.Children.FirstOrDefault(i => (int)i.GetValue(Grid.ColumnProperty) == col && (int)i.GetValue(Grid.RowProperty) == row);
                // Need to find an existing element that is in our spot
                if (existing is Grid)
                {
                    // tried to place where there is a grid.
                    // Try to place in exact spot in the grid we found
                    applyQuadrant(ctl, qlocation + qlocation.Substring(qlocation.Length - 1, 1), width, height, true);
                    return;
                }
                if (existing != null)
                {
                    var ctlNum = controls.IndexOf((FrameworkElement)existing);

                    splitCell(parent, ctlNum, ctl, "");
                    return;
                }
                // else fall through to insert

                //    moveControlToGrid((FrameworkElement)parent.Children[0]);
                //    var newparent = (Grid)parent.Children[0];

                //    newparent.Children.Add(ctl);
                //    ctl.SetValue(Grid.RowProperty, 1);
                //    ctl.SetValue(Grid.ColumnProperty, 0);
            }
            parent.Children.Add(ctl);
            ctl.SetValue(Grid.RowProperty, row);
            ctl.SetValue(Grid.ColumnProperty, col);
        }
        private Grid FindParentOrCreate(string qlocation)
        {
            var parent = baseImpl.RootUI;
            EnsureGrid(parent);

            string loc = "";

            for (var idx = 0; idx < qlocation.Length - 1; idx++)
            {
                loc = qlocation.Substring(idx, 1);
                Grid pctl = FindOrCreateGridForLayout(parent, loc);

                if (pctl == null)
                    throw new Exception("Unable to create ui layout");

                parent = pctl;
            }
            return parent;
        }

        private Grid FindOrCreateGridForLayout(Grid parent, string loc)
        {
            Tuple<int, int> quad;

            int row = 0;
            int col = 0;
            var gridName = (parent == baseImpl.RootUI ? kGridName : parent.Name) + "_" + loc;
            var pctl = (Grid)parent.FindName(gridName);
            if (pctl == null)
            {
                quad = TranslateQLoc(loc);
                row = quad.Item1;
                col = quad.Item2;

                var grid = new Grid();
                grid.Name = gridName;
                grid.HorizontalAlignment = HorizontalAlignment.Stretch;
                grid.VerticalAlignment = VerticalAlignment.Stretch;
                EnsureGrid(grid);
                grid.SetValue(Grid.RowProperty, row);
                grid.SetValue(Grid.ColumnProperty, col);

                parent.Children.Add(grid);
                pctl = grid;
            }
            return pctl;
        }

        private void EnsureGrid(Grid grid)
        {
            if (grid.ColumnDefinitions.Count == 0)
            {
                grid.ColumnDefinitions.Add(new Windows.UI.Xaml.Controls.ColumnDefinition() { Width = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Auto) });
                grid.ColumnDefinitions.Add(new Windows.UI.Xaml.Controls.ColumnDefinition() { Width = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new Windows.UI.Xaml.Controls.ColumnDefinition() { Width = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Auto) });
            }
            if (grid.RowDefinitions.Count == 0)
            {
                grid.RowDefinitions.Add(new Windows.UI.Xaml.Controls.RowDefinition() { Height = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Auto) });
                grid.RowDefinitions.Add(new Windows.UI.Xaml.Controls.RowDefinition() { Height = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Star) });
                grid.RowDefinitions.Add(new Windows.UI.Xaml.Controls.RowDefinition() { Height = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Auto) });
            }
        }

        private Tuple<int, int> TranslateQLoc(string loc)
        {
            var row = 1; var col = 1;
            if (loc == ControlPositions.PosTopLeft || loc == ControlPositions.PosTopCenter || loc == ControlPositions.PosTopRight)
                row = 0;
            if (loc == ControlPositions.PosBottomLeft || loc == ControlPositions.PosBottomCenter || loc == ControlPositions.PosBottomRight)
                row = 2;
            if (loc == ControlPositions.PosTopLeft || loc == ControlPositions.PosMiddleLeft || loc == ControlPositions.PosBottomLeft)
                col = 0;
            if (loc == ControlPositions.PosTopRight || loc == ControlPositions.PosMiddleRight || loc == ControlPositions.PosBottomRight)
                col = 2;
            return new Tuple<int, int>(row, col);
        }

        private ConsoleControl CreateConsoleTextControl(bool scrollable, bool allowInput)
        {
            return new ConsoleControl
            {
                IsScrollable = scrollable,
                AllowInput = allowInput
            };
        }
        private TextGridControl CreateConsoleGridTextControl(bool scrollable, bool allowInput)
        {
            return new TextGridControl
            {
                IsScrollable = scrollable,
                AllowInput = allowInput
            };
        }

        public int CreateMainText(string qlocation)
        {
            var id = CreateRegion(WindowType.MainTextRegion, qlocation, -1, -1, true);
            return id;
        }

        public int SetTextRegion(string qlocation, int width, int height, bool scrollable)
        {
            return SetTextRegion(qlocation, width, height, scrollable, -1, -1);
        }
        public int SetTextRegion(string qlocation, int width, int height, bool scrollable, int rows)
        {
            return SetTextRegion(qlocation, width, height, scrollable, rows, -1);
        }
        public int SetTextRegion(string qlocation, int width, int height, bool scrollable, int rows, int cols)
        {
            return CreateRegion(WindowType.TextRegion, qlocation, width, height, scrollable, rows, cols);
        }

        public int SetGraphicRegion(string qlocation, int width, int height)
        {
            return CreateRegion(WindowType.GraphicRegion, qlocation, width, height, false);
        }

        public int SetGridTextRegion(string qlocation, int width, int height, bool scrollable)
        {
            return SetGridTextRegion(qlocation, width, height, scrollable, -1, -1);
        }
        public int SetGridTextRegion(string qlocation, int width, int height, bool scrollable, int rows)
        {
            return SetGridTextRegion(qlocation, width, height, scrollable, rows, -1);
        }
        public int SetGridTextRegion(string qlocation, int width, int height, bool scrollable, int rows, int cols)
        {
            return CreateRegion(WindowType.TextGridRegion, qlocation, width, height, false);
        }

        #endregion

    }
}
