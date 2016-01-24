using IFInterfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFInterfaces.Support
{
    public interface IDebugInfo
    {
        bool SupportsLogging { get;  }

        IDebugService Debug { get; set; }
    }
}
