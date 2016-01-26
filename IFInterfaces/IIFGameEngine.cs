using IFInterfaces.Services;
using IFInterfaces.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace IFInterfaces
{
    public enum CanRunResult
    {
        Unkown,
        No,
        Maybe,
        Yes        
    }

    public interface IIFGameEngine
    {
        string Identifier { get; }

        CanRunResult CanRun(IFileService fileIO);

        IAsyncOperation<ExecutionResult> Start(IIFRuntime runtime);

        IAsyncOperation<ExecutionResult> Start(IIFRuntime runtime, bool debugMessages);

        string[] KnownExtensions { get; }
    }
}
