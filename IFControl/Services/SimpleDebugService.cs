using IFInterfaces.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFCore.Services
{
    class SimpleDebugService : IDebugService
    {
        public void LogDebug(string message)
        {
            Debug.WriteLine(message, "debug");
        }

        public void LogDebug(string messageFormat, params object[] parms)
        {
            Debug.WriteLine(String.Format(messageFormat, parms), "debug");
        }

        public void LogError(string message)
        {
            Debug.WriteLine(message, "error");
        }

        public void LogError(string messageFormat, params object[] parms)
        {
            Debug.WriteLine(String.Format(messageFormat, parms), "error");
        }

        public void LogInfo(string message)
        {
            Debug.WriteLine(message, "info");
        }

        public void LogInfo(string messageFormat, params object[] parms)
        {
            Debug.WriteLine(String.Format(messageFormat, parms), "info");
        }
    }
}
