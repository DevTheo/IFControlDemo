using IFEngine_UWP.Controls;
using IFInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFEngine_UWP.Support
{
    public class UIControlBuilder : IControlCreator
    {
        public IIFTextGridControl CreateGridTextControl(bool scrollable, bool allowInput)
        {
            return new TextGridControl()
            {
                IsScrollable = scrollable,
                AllowInput = allowInput
            };
        }

        public IIFTextControl CreateTextControl(bool scrollable, bool allowInput)
        {
            return new ConsoleControl()
            {
                IsScrollable = scrollable,
                AllowInput = allowInput
            };
        }
    }
}
