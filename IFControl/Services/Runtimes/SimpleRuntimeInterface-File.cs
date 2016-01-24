using System;
using System.Linq;
using IFInterfaces.Services;
using IoC = IFCore.Services.IOC.IOCContainer;

namespace IFCore.Services
{
    public sealed partial class SimpleRuntimeInterface
    {
        private IFileService fileService = null;

        public bool SupportsSaveLoadGame { get { return true; } }
        public IFileService FileIO
        {
            get
            {
                if (fileService == null)
                {
                    fileService = IoC.Inst.FileService;
                }

                return fileService;
            }

            set
            {
                this.fileService = value;
            }
        }
    }
}
