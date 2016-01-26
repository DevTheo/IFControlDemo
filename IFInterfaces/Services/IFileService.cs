using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using IFInterfaces.Support;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.Storage.Streams;

namespace IFInterfaces.Services
{
    public static class FileIOConstants
    {
        public const int EOF = -1;
    }

    public enum FSeekOrigin : int
    {
        Begin = 0,
        Current = 1,
        End = 2
    }

    public enum FSeekOffset : int
    {
        SEEK_SET = 0,
        SEEK_CUR = 1,
        SEEK_END = 2
    }

    public interface IFileEntry
    {
        string Key
        {
            get; set;
        }

        byte[] Buffer
        {
            get; set;
        }

        IStorageFile File
        {
            get; set;
        }
    }

    public sealed class FPos_T
    {
        public long off { get; set; }
        //_Mbstatet _wstate
    }

    public sealed class FReadResult
    {
        public int Length { get; set; }
        public object Data { get; set; }
    }
    public interface IFileService
    {
        String[] GetFileNames();

        void ResetService();
        IAsyncAction InitFileServiceAsync([ReadOnlyArray] IStorageFile[] _files);
        IAsyncAction AddFilesAsync([ReadOnlyArray] IStorageFile[] _files);

        IAsyncOperation<int> PickFileForReadAsync(string fileName, [ReadOnlyArray] string[] filters, int defaultFilterIdx);
        IAsyncOperation<int> PickFileForWriteAsync(string fileName, [ReadOnlyArray] string[] filters, int defaultFilterIdx);

        IAsyncOperation<IRandomAccessStream> GetStreamAsync(int fileStream, bool readOnly);

        IAsyncOperation<IRandomAccessStream> GetStreamReaderAsync(int fileStream);

        IAsyncOperation<IRandomAccessStream> GetStreamWriterAsync(int fileStream);

        IAsyncOperation<bool> CloseStreamAsync(int fileStream);

        IAsyncOperation<int> OpenLocalFileForReadAsync(string fileName);

        IAsyncOperation<int> CreateTempFileForWrite();

        IAsyncOperation<int> CreateLocalFileForWriteAsync(string fileName);

        IAsyncOperation<int> CreateLocalFileForWriteAsync(string fileName, bool deleteIfExists);

        IAsyncOperation<int> OpenLocalFileForWriteAsync(string fileName);
        IAsyncOperation<int> OpenLocalFileForWriteAsync(string fileName, bool createIfNotExists);

        IAsyncOperation<bool> FileExistsAsync(string fileName);

        IAsyncOperation<bool> DeleteFileAsync(string fileName);

        string GetFile(uint fileNum);

        int FWrite(object buf, int pos, int len, int fileNum);

        char Putc(char chr, int fileNum);

        [DefaultOverloadAttribute]
        char[] Getc(int fileNum/*, int max = 1*/);
        char[] Getc(int fileNum, int max);

        FReadResult FRead(object temp, int sizeOfValue, int len, int fileNum);

        string FGets([WriteOnlyArray] char[] str, int len, int fileNum);
        int FGetc(int fileNum);

        void Seek(int fileNum, long pos, FSeekOrigin seekOrigin);
        long GetPosition(int filenum);

        void Flush(int fileNum);

        long SizeOfFile(int fileNum);

        int FOpen(string name, string mode);
        void ClearErr(int fileNum);
        void FClose(int fileNum);
        bool FEof(int fileNum);
        void FError(int fileNum);
        int FFlush(int fileNum);
        int FGetPos(int fileNum, FPos_T pos);
        int FPrintf(int fileNum, string format, params object[] parms);
        int FPutc(int character, int fileNum);
        int FPuts(string str, int fileNum);
        int FReopen(string filename, string mode, int fileNum);
        int FScanf(int fileNum, string format, params object[] parms);
        int FSeek(int fileNum, long offset, FSeekOffset origin);
        int FSetPos(int fileNum, FPos_T pos);
        long FTell(int fileNum);
        int Remove(string filename);
        int Rename(string oldname, string newname);
        void Rewind(int filenum);
        void SetBuf(int filenum, [WriteOnlyArray] byte[] buffer);
        int SetVBuf(int filenum, [WriteOnlyArray] byte[] buffer, int mode, long size);
        int TmpFile();
        string TmpNam();
        int Ungetc(int character, int filenum);
    }
}
