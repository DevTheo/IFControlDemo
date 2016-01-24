using IFInterfaces.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Storage.Streams;

namespace IFInterfaces.Support
{    
    public interface IFileStorageSupport
    {
        bool SupportsSaveLoadGame { get; }
        IFileService FileIO { get; set; }
    }
}
