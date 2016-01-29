using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IFInterfaces;
using IFInterfaces.Services;
using IFInterfaces.Support;
using Windows.Foundation;

namespace IFEngine.Grue
{
    public sealed class GrueEngine : IIFGameEngine
    {

        #region IIFGameEngine
        public string Identifier
        {
            get
            {
                return "GRUE";
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
            this.runtime = runtime;
            return StartAsyncTask(debugMessages).AsAsyncOperation();
        }
        #endregion

        #region private members
        private IIFRuntime runtime = null;
        private IFileService fileIO
        {
            get
            {
                if (runtime == null)
                    return null;
                return runtime.FileIO;
            }
        }
        ExecutionResult execresult = ExecutionResult.ERR_NO_ERRORS;
        #endregion

        async Task<ExecutionResult> StartAsyncTask(bool debugMessages)
        {
            execresult = ExecutionResult.ERR_NO_ERRORS;

            var fileExt = fileIO.GetFileNames().Where(i => i.Contains(".")).Select(i => i.Substring(i.LastIndexOf(".")))
                .First(i => KnownExtensions.Contains(i.ToLower()));

            var fnames = fileIO.GetFileNames();

            var story_name = fnames.First(i => i.ToLower().EndsWith(fileExt.ToLower()));

            if (story_name.Length == 0)
            {
                return await Task.FromResult(ExecutionResult.ERR_BADFILE);
            }

            var mem = new List<byte>();
            var fd = fileIO.FOpen(story_name, "rb");
            using (var strm = (await fileIO.GetStreamReaderAsync(fd)).AsStream())
            {
                while (true)
                {
                    var buffer = new byte[4096];
                    var countRead = strm.Read(buffer, 0, 4096);
                    mem.AddRange(buffer);
                    if (countRead <= 0)
                        break;
                }
            }
            
            
            var fe = new Zmachine.FrontEnd(runtime);

            fe.LoadStory(new Zmachine.ImmutableArray<byte>(mem.ToArray()));

            //execresult = await game_begin();
            //if (execresult != ExecutionResult.ERR_NO_ERRORS)
            //    return execresult;

            fe.Zmachine.Restart();
            //execresult = await game_restart();
            //if (execresult != ExecutionResult.ERR_NO_ERRORS)
            //    return execresult;

            for (;;)
            {
                await fe.Zmachine.Run();
                if (!fe.Done)
                    break;
            }

            return await Task.FromResult(execresult);
        }

    }
}
