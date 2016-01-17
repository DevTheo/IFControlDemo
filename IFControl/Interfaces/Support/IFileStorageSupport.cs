using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Storage.Streams;

namespace IFControl.Interfaces.Support
{
    public enum FSeekOrigin
    {
        Begin = 0,
        Current = 1,
        End = 2
    }

    public sealed class StringKeyByteArrayVal
    {
        public string Key
        {
            get; set;
        }

        public byte[] Value
        {
            get; set;
        }
    }

    public interface IFileStorageSupport
    {
        bool SupportsSaveLoadGame { get; }
        IAsyncOperation<int> OpenFileAsync([ReadOnlyArray] string[] filters, int defaultFilterIdx);
        IAsyncOperation<int> SaveFileAsync(string fileName, [ReadOnlyArray] string[] filters, int defaultFilterIdx);
        IAsyncOperation<IBuffer> GetStreamAsync(int fileStream, bool readOnly);
        IAsyncOperation<IBuffer> GetStreamReaderAsync(int fileStream);
        IAsyncOperation<IBuffer> GetStreamWriterAsync(int fileStream);
        IAsyncOperation<bool> CloseStreamAsync(int fileStream);
        IAsyncOperation<int> OpenLocalFileForReadAsync(string fileName);
        IAsyncOperation<int> CreateTempFileForWrite();
        IAsyncOperation<int> CreateLocalFileForWriteAsync(string fileName);
        IAsyncOperation<int> OpenLocalFileForWriteAsync(string fileName);
        IAsyncOperation<bool> FileExistsAsync(string fileName);
        IAsyncOperation<bool> DeleteFileAsync(string fileName);
        string GetFile(uint fileNum);

        int FWrite([ReadOnlyArray] byte[] buf, int pos, int len, int fileNum);
        char Putc(char chr, int fileNum);

        [DefaultOverloadAttribute]
        char[] Getc(int fileNum/*, int max = 1*/);
        char[] Getc(int fileNum, int max);

        int FRead(object temp, int pos, int len, int fileNum);

        string FGets([ReadOnlyArray] byte[] cbuf, int len, int fileNum);

        void Seek(int fileNum, long pos, FSeekOrigin seekOrigin);
        long GetPosition(int filenum);

        IAsyncAction AppendArchiveEntriesAsync([ReadOnlyArray] StringKeyByteArrayVal[] fileData);

        void Flush(int fileNum);

        int SizeOfFile(int fp);
    }
}
