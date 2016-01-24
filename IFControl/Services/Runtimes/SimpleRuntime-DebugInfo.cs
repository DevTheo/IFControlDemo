using IFCore.Services.IOC;
using IFInterfaces.Services;

namespace IFCore.Services
{
    public sealed partial class SimpleRuntimeInterface
    {
        private IOCContainer IoC { get; set; } = IOCContainer.Inst;
        #region IDebugInfo
        public bool SupportsLogging
        {
            get
            {
                return Debug != null;
            }
        }

        IDebugService debug;
        public IDebugService Debug
        {
            get
            {
                if(debug == null)
                {
                    debug = IoC.DebugService;
                }
                return debug;
            }
            set
            {
                debug = value;
            }
        }
        #endregion

    }
}