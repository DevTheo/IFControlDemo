using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using IFInterfaces.Services;
using IFInterfaces.Support;
using SharpCompress.Archive;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;

namespace IFControls.Services
{
    public partial class ArchiveAndLocalBasedFileService
    {
        class FileEntry : IFileEntry
        {
            public FileEntry(string fileName, IStorageFile file)
            {
                Key = fileName;
                File = file;
                Buffer = null;
            }

            public FileEntry(string fileName, byte[] buffer)
            {
                Key = fileName;
                Buffer = buffer;
                File = null;
            }

            public byte[] Buffer { get; set; }

            public IStorageFile File { get; set; }

            public string Key { get; set; }

            internal async Task<Stream> GetStreamAsync()
            {
                if (Buffer != null)
                {
                    return new MemoryStream(Buffer);
                }
                else if (File != null)
                {
                    return (await File.OpenAsync(FileAccessMode.ReadWrite)).AsStream();
                }
                throw new ArgumentNullException();
            }

            internal async Task<Stream> GetReadOnlyStreamAsync()
            {
                if (Buffer != null)
                {
                    return new MemoryStream(Buffer);
                }
                else if (File != null)
                {
                    return (await File.OpenAsync(FileAccessMode.Read)).AsStream();
                }
                throw new ArgumentNullException();
            }
        }
    }
}