using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFInterfaces
{
    public interface IControlCreator
    {
        IIFTextControl CreateTextControl(bool scrollable, bool allowInput);
        IIFTextGridControl CreateGridTextControl(bool scrollable, bool allowInput);
    }
}
