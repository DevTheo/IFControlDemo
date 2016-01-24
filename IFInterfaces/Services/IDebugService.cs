using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFInterfaces.Services
{
    public interface IDebugService
    {
        void LogInfo(string message);
        void LogInfo(string messageFormat, params object[] parms);
        void LogDebug(string message);
        void LogDebug(string messageFormat, params object[] parms);
        void LogError(string message);
        void LogError(string messageFormat, params object[] parms);
    }
}
