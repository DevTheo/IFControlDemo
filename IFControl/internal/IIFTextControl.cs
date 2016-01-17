using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFControls.Interfaces
{
    public interface IIFTextControl
    {
        int ColumnCount { get; set; }
        int CurrentColumn { get; set; }
        int CurrentRow { get; set; }
        int RowCount { get; set; }
    }
}
