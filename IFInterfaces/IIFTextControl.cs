using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFInterfaces
{
    public interface IIFTextControl : IIFControl
    {
        int ColumnCount { get; set; }
        int CurrentColumn { get; set; }
        int CurrentRow { get; set; }
        int RowCount { get; set; }
    }

    public interface IIFTextGridControl : IIFTextControl
    {
        void ReCalcCells();
    }
}
