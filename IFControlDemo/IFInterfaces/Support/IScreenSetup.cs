using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;

namespace IFInterfaces.Support
{
    public enum WindowType
    {
        TextRegion,
        MainTextRegion,
        TextGridRegion,
        GraphicRegion
    }

    public sealed  class TupleIntInt
    {
        public TupleIntInt(int v1, int v2)
        {
            Value1 = v1;
            Value2 = v2;
        }

        public int Value1 { get; set; }

        public int Value2 { get; set; }
    }

    public interface IScreenSetup
    {
        int CharHeight { get; }
        int CharWidth { get; }
        void SetCursorLocation(int controlNum, int row, int col);
        bool SupportsMultiWin { get; }
        string ControlQLocation(int controlNum);

        [DefaultOverloadAttribute]
        int CreateRegion(WindowType winType, string qlocation, int width, int height, bool scrollable /*, int rows = -1, int cols = -1*/);
        int CreateRegion(WindowType winType, string qlocation, int width, int height, bool scrollable, int rows /*, int cols = -1*/);
        int CreateRegion(WindowType winType, string qlocation, int width, int height, bool scrollable, int rows, int cols);

        [DefaultOverloadAttribute]
        int SetTextRegion(string qlocation, int width, int height, bool scrollable /*, int rows = -1, int cols = -1*/);
        int SetTextRegion(string qlocation, int width, int height, bool scrollable, int rows /*, int cols = -1*/);
        int SetTextRegion(string qlocation, int width, int height, bool scrollable, int rows, int cols);


        [DefaultOverloadAttribute]
        int SetGridTextRegion(string qlocation, int width, int height, bool scrollable /*, int rows = -1, int cols = -1*/);
        int SetGridTextRegion(string qlocation, int width, int height, bool scrollable, int rows /*, int cols = -1*/);
        int SetGridTextRegion(string qlocation, int width, int height, bool scrollable, int rows, int cols);

        int SetGraphicRegion(string qlocation, int width, int height);
        int CreateMainText(string qlocation);
        bool IsGridText(int controlNum);
        TupleIntInt GetRowsColumns(int controlNum);
        void DestroyRegion(int regionNum);
        void MoveControlToGrid(int regionNum);
    }
}
