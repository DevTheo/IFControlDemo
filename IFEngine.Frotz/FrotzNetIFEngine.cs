using IFInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using IFInterfaces.Services;
using IFInterfaces.Support;
using System.IO;
using IFInterfaces.Helpers;
using Windows.Foundation;

namespace IFEngine.Frotz
{
    public sealed class FrotzNetIFEngine : IIFGameEngine
    {

        #region IIFGameEngine
        public string Identifier
        {
            get
            {
                return "Z1-5&7+";
            }
        }
        readonly string[] zfileExt = new[] { ".z1", ".z2", ".z3", ".z4", ".z5", ".z7", ".z8", ".z9", ".z10" };

        public string[] KnownExtensions
        {
            get
            {
                return new[] { ".bin", ".z1", ".z2", ".z3", ".z4", ".z5", ".z7", ".z8", ".z9", ".z10" };
            }
        }

        public CanRunResult CanRun(IFileService fileIO)
        {
            if (fileIO.GetFileNames().Any(i =>
                                zfileExt.Contains(i.Substring(i.Length - 3, 3).ToLower()) ||
                                i.Substring(i.Length - 4, 4).ToLower() == ".z10"))
            {
                return CanRunResult.Yes;
            }
            else if (fileIO.GetFileNames().Any(i => i.ToLower().EndsWith(".bin")))
            {
                // parse file version.. for now return undetermined
                return CanRunResult.Maybe;
            }
            return CanRunResult.No;
        }

        public IAsyncOperation<ExecutionResult> Start(IIFRuntime runtime)
        {
            return Start(runtime, false);
        }

        public IAsyncOperation<ExecutionResult> Start(IIFRuntime runtime, bool debugMessages)
        {
            //this.runtime = runtime;
            return StartAsyncTask(debugMessages).AsAsyncOperation();
        }

        private async Task<ExecutionResult> StartAsyncTask(bool debugMessages)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
