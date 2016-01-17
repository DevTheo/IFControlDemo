using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using IFControls.Internal;
using IFInterfaces.Services;
using SharpCompress.Archive;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;

namespace IFControls.Services
{
    public sealed partial class ArchiveAndLocalBasedFileService : IFileService
    {
        const int EOF = -1;

        class FileStreamInfo
        {
            internal IStorageFile file;
            internal Stream stream;
            internal bool readOnly;
            //internal bool isArchive;
            //internal int archiveNum;
            internal string fileName;
            internal int fileNum;
            internal byte[] buffer;
        }

        internal List<IFileEntry> Files = new List<IFileEntry>();
        internal Dictionary<int, Tuple<Stream, string>> OpenStreams = new Dictionary<int, Tuple<Stream, string>>();

        public IAsyncAction InitFileServiceAsync([ReadOnlyArray] IStorageFile[] _files)
        {
            Files.Clear();
            return AddFilesAsyncTask(_files).AsAsyncAction();
        }
        public IAsyncAction AddFilesAsync([ReadOnlyArray] IStorageFile[] _files)
        {
            return AddFilesAsyncTask(_files).AsAsyncAction();
        }

        private async Task AddFilesAsyncTask(IStorageFile[] _files)
        {
            foreach (var item in _files)
            {
                if(item is IStorageFile)
                {
                    var file = (IStorageFile) item;
                    Files.Add(new FileEntry(file.Name, file));
                    try
                    {
                        using (var strm = (await file.OpenReadAsync()).AsStream())
                        {
                            var arch = ArchiveFactory.Open(strm);
                            foreach (var entry in arch.Entries)
                            {
                                if (!entry.IsDirectory)
                                {                                    
                                    using (var ms = new MemoryStream())
                                    {
                                        try
                                        {
                                            entry.WriteTo(ms);
                                        }
                                        catch (Exception ex) { }
                                        ms.Seek(0, SeekOrigin.Begin);
                                        var buffer = new ArraySegment<byte>();
                                        ms.TryGetBuffer(out buffer);
                                        
                                        Files.Add(new FileEntry(entry.Key, buffer.Array));
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        public IAsyncOperation<bool> CloseStreamAsync(int fileStream)
        {
            var stream = OpenStreams.Where(i => i.Key == fileStream).Select(i => i.Value.Item1).FirstOrDefault();
            if (stream == null)
            {
                return Task.FromResult(false).AsAsyncOperation();
            }

            return Task.Run(() =>
               {
                   stream.Dispose();
                   return true;
               }).AsAsyncOperation();
        }

        private int getFileNum()
        {
            return OpenStreams.Count + 10;
        }

        private Stream getStreamFor(int fileNum)
        {
            if (OpenStreams.Count > 0 && OpenStreams.ContainsKey(fileNum))
            {
                return OpenStreams[fileNum].Item1;
            }
            return null;
        }
        private FileStreamInfo getFileStream(string fileName)
        {
            if (OpenStreams.Count > 0)
            {
                var item = OpenStreams.FirstOrDefault(i => i.Value.Item2.ToLowerInvariant().StartsWith(fileName.ToLowerInvariant()));

                if (item.Value != null && item.Value.Item2.ToLowerInvariant().StartsWith(fileName.ToLowerInvariant()))
                {
                    var _file = Files.FirstOrDefault(i => i.Key.Equals(item.Value.Item2, StringComparison.OrdinalIgnoreCase));
                    return new FileStreamInfo
                    {
                        fileNum = item.Key,
                        file = _file.File == null ? null : _file.File,
                        buffer = _file.Buffer,
                        readOnly = _file.File == null,
                        stream = item.Value.Item1,
                        fileName = item.Value.Item2
                    };
                }

            }
            else if (Files.Any(i => i.Key.Equals(fileName, StringComparison.OrdinalIgnoreCase)))
            {
                var _file = Files.FirstOrDefault(i => i.Key.Equals(fileName, StringComparison.OrdinalIgnoreCase));
                return new FileStreamInfo
                {
                    fileNum = -1,
                    file = _file.File,
                    buffer = _file.Buffer,
                    readOnly = _file.File == null,
                    stream = null,
                    fileName = _file.Key
                };
            }
            return null;
        }
        private async Task<IStorageFile> getFileFullAsync(string fileName)
        {
            // TODO: check for existing registered file
            var fs = getFileStream(fileName);
            if (fs != null && fs.file != null)
                return fs.file;

            // TODO: detect file in archive
            return await getFileAsync(fileName);
        }

        public IAsyncOperation<int> CreateLocalFileForWriteAsync(string fileName)
        {
            return CreateLocalFileForWriteAsyncTask(fileName, false).AsAsyncOperation();
        }
        public IAsyncOperation<int> CreateLocalFileForWriteAsync(string fileName, bool deleteIfExists)
        {
            return CreateLocalFileForWriteAsyncTask(fileName, deleteIfExists).AsAsyncOperation();
        }
        private async Task<int> CreateLocalFileForWriteAsyncTask(string fileName, bool deleteIfExists)
        {
            var item = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, deleteIfExists ? CreationCollisionOption.ReplaceExisting : CreationCollisionOption.GenerateUniqueName);
            Files.Add(new FileEntry(item.Name, item));

            var fs = (await item.OpenAsync(FileAccessMode.ReadWrite)).AsStream();
            var filenum = getFileNum();
            OpenStreams.Add(filenum, new Tuple<Stream, string>(fs, fileName));

            return filenum;
        }

        public IAsyncOperation<int> CreateTempFileForWrite()
        {
            return CreateTempFileForWriteAsyncTask().AsAsyncOperation();
        }
        private async Task<int> CreateTempFileForWriteAsyncTask()
        {
            var fn = "ifngn_" + Guid.NewGuid().ToString().Replace("-", "");
            var item = await Windows.Storage.ApplicationData.Current.TemporaryFolder.CreateFileAsync(fn, CreationCollisionOption.GenerateUniqueName);
            
            // Not sure I should save temp files
            Files.Add(new FileEntry(item.Name, item));

            var fs = (await item.OpenAsync(FileAccessMode.ReadWrite)).AsStream();
            var filenum = getFileNum(); 
            OpenStreams.Add(filenum, new Tuple<Stream, string>(fs, fn));

            return filenum;
        }

        public IAsyncOperation<bool> DeleteFileAsync(string fileName)
        {
            return DeleteFileAsyncTask(fileName).AsAsyncOperation();
        }
        private async Task<bool> DeleteFileAsyncTask(string fileName)
        {
            try
            {
                IStorageFile item;
                var fi = Files.FirstOrDefault(i => i.Key.Equals(fileName, StringComparison.OrdinalIgnoreCase));
                if (fi != null)
                {
                    item = fi.File;
                    var openStreamRecord = OpenStreams.FirstOrDefault(i => i.Value.Item2.Equals(fileName, StringComparison.OrdinalIgnoreCase));
                    if (openStreamRecord.Value != null)
                    {
                        OpenStreams.Remove(openStreamRecord.Key);
                        openStreamRecord.Value.Item1.Dispose();
                    }
                    fi.File = null;
                    fi.Key = "\t\t"; // Should be looking for a file name with this pattern (the idea is that this file is dead)
                }
                else
                {
                    item = await getFileAsync(fileName);
                }
                if(item != null)
                    await item.DeleteAsync();
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        private async Task<StorageFile> getFileAsync(string fileName)
        {
            StorageFile item = null;
            var slashPos = fileName.IndexOf(@"\");
            if (slashPos > -1 && slashPos < 3)
                item = await StorageFile.GetFileFromPathAsync(fileName);
            else
            {
                item = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
            }
            return item;
        }

        /// <summary>
        /// Get string from stream
        /// Reads characters from stream and stores them as a C string into str until(num-1) characters have been read or 
        /// either a newline or the end-of-file is reached, whichever happens first.
        /// 
        /// A newline character makes fgets stop reading, but it is considered a valid character by the function and 
        /// included in the string copied to str.
        /// 
        /// A terminating null character is automatically appended after the characters copied to str.
        /// 
        /// Notice that fgets is quite different from gets: not only fgets accepts a stream argument, but also allows 
        /// to specify the maximum size of str and includes in the string any ending newline character.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <param name="fileNum"></param>
        /// <returns>
        /// On success, the function returns str.
        /// If the end-of-file is encountered while attempting to read a character, the eof indicator is set(feof). 
        /// If this happens before any characters could be read, the pointer returned is a null pointer (and the 
        /// contents of str remain unchanged).
        /// If a read error occurs, the error indicator (ferror) is set and a null pointer is also returned (but 
        /// the contents pointed by str may have changed).
        /// </returns>
        public string FGets([WriteOnlyArray] char[] str, int len, int fileNum)
        {
            var strm = getStreamFor(fileNum);
            if (strm == null)
                return null;
            var sb = new StringBuilder();
            while (true)
            {
                char ch = '\0';
                var read = 1;
                try
                {
                    ch = (char) strm.ReadByte();
                }
                catch { read = 0; }
                if (read != 0)
                    sb.Append(ch);
                if (ch == '\n')
                    break;
                if ((len > 0 && sb.Length >= len) || read == 0) // last one is eof/eos
                    break;
            }
            //sb.Append('\0');
            var result = sb.ToString();
            str = result.ToCharArray();

            return result;
        }

        public IAsyncOperation<bool> FileExistsAsync(string fileName)
        {
            return FileExistsAsyncTask(fileName).AsAsyncOperation();
        }
        private async Task<bool> FileExistsAsyncTask(string fileName)
        {
            try
            {
                var fi = getFileStream(fileName);
                if (fi != null && (fi.stream != null || fi.file != null))
                    return true;

                var item = await getFileFullAsync(fileName);
                var props = await item.GetBasicPropertiesAsync();

                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public void Flush(int fileNum)
        {
            var strm = getStreamFor(fileNum);
            strm.Flush();
        }

        /// <summary>
        /// Read block of data from stream
        /// Reads an array of count elements, each one with a size of size bytes, 
        /// from the stream and stores them in the block of memory specified by ptr.
        /// 
        /// The position indicator of the stream is advanced by the total amount of bytes read.
        /// 
        /// The total amount of bytes read if successful is (size* count).
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="sizeOfValue"></param>
        /// <param name="len"></param>
        /// <param name="fileNum"></param>
        /// <returns>
        /// The total number of elements successfully read is returned.
        /// 
        /// If this number differs from the count parameter, either a 
        /// reading error occurred or the end-of-file was reached while 
        /// reading.In both cases, the proper indicator is set, which 
        /// can be checked with ferror and feof, respectively.
        /// 
        /// If either size or count is zero, the function returns zero 
        /// and both the stream state and the content pointed by ptr remain unchanged.
        /// 
        /// size_t is an unsigned integral type.
        /// </returns>
        public int FRead(object temp, int sizeOfValue, int len, int fileNum)
        {
            var strm = getStreamFor(fileNum);
            if (strm == null)
                return -1;
            if (temp is byte[])
            {
                return strm.Read((byte[]) temp, 0, len);
            }
            else if (temp is uint[])
            {
                byte[] buffer = null;
                var result = strm.Read(buffer, 0, len * sizeOfValue);
                for (var i = 0; i < buffer.Length; i += sizeOfValue)
                {
                    ((uint[]) temp)[i / 2] = buffer.GetUIntFromBufferLoc(i);
                }
                return result / 2;
            }
            else
            {
                throw new NotImplementedException("Binary object serialization is not built as of yet.");
            }
        }

        /// <summary>
        /// Write block of data to stream
        /// Writes an array of count elements, each one with a size of size bytes, from the block of memory pointed 
        /// by ptr to the current position in the stream.
        /// 
        /// The position indicator of the stream is advanced by the total number of bytes written.
        /// 
        /// Internally, the function interprets the block pointed by ptr as if it was an array of (size* count) elements 
        /// of type unsigned char, and writes them sequentially to stream as if fputc was called for each byte.
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="pos"></param>
        /// <param name="len"></param>
        /// <param name="fileNum"></param>
        /// <returns>
        /// The total number of elements successfully written is returned.
        /// 
        /// If this number differs from the count parameter, a writing error prevented the function from completing. 
        /// In this case, the error indicator (ferror) will be set for the stream.
        /// 
        /// If either size or count is zero, the function returns zero and the error indicator remains unchanged.
        /// size_t is an unsigned integral type.
        /// </returns>
        public int FWrite(object buf, int pos, int len, int fileNum)
        {
            var strm = getStreamFor(fileNum);
            if (strm == null)
                return 0;
            byte[] buffer;
            if (buf is byte[])
            {
                buffer = (byte[]) buf;
                strm.Write(buffer, 0, len);
                strm.Flush();
                return len;
            }
            else
            {
                throw new NotImplementedException("Binary object serialization is not built as of yet.");
            }
        }

        public char[] Getc(int fileNum)
        {
            var strm = getStreamFor(fileNum);
            if (strm == null)
                return null;

            char ch = (char) 0;
            try { ch = (char) strm.ReadByte(); } catch { /*read=0;*/}
            return new[] { ch };
        }

        public char[] Getc(int fileNum, int max)
        {
            List<char> chars = new List<char>();
            var strm = getStreamFor(fileNum);
            if (strm == null)
                return null;

            char ch = (char) 0;
            while (strm.CanRead && chars.Count < max)
            {
                try { ch = (char) strm.ReadByte(); } catch { break; /*read=0;*/}
                chars.Add(ch);
            }
            return chars.ToArray();
        }

        public string GetFile(uint fileNum)
        {
            if (Files.Count <= 1)
                return "";
            return Files[(int) fileNum].Key;
        }

        public long GetPosition(int filenum)
        {
            var strm = getStreamFor(filenum);
            if (strm == null)
                return -1;
            // else Error??
            return strm.Position;
        }

        public IAsyncOperation<IRandomAccessStream> GetStreamAsync(int fileStream, bool readOnly)
        {
            return GetStreamAsyncTask(fileStream, readOnly).AsAsyncOperation();
        }
        private async Task<IRandomAccessStream> GetStreamAsyncTask(int fileStream, bool readOnly)
        {
            if(OpenStreams.Count > 0 && OpenStreams.ContainsKey(fileStream))
            {
                if (OpenStreams[fileStream].Item1 != null)
                    return await Task.FromResult<IRandomAccessStream>(OpenStreams[fileStream].Item1.AsRandomAccessStream());
                else if (!String.IsNullOrEmpty(OpenStreams[fileStream].Item2))
                {
                    var item = Files.FirstOrDefault(i => i.Key.Equals(OpenStreams[fileStream].Item2, StringComparison.OrdinalIgnoreCase));
                    if (item == null ||
                        (item.File == null && item.Buffer != null && !readOnly)) // can't write to a buffer
                    {
                        return await Task.FromResult<IRandomAccessStream>(null);
                    }
                    Stream stream = null;
                    if (item.Buffer != null)
                        stream = new MemoryStream(item.Buffer);
                    else if (item.File != null)
                        stream = await (readOnly ?
                            item.File.OpenStreamForReadAsync() :
                            item.File.OpenStreamForWriteAsync());

                    if(stream != null)
                    {
                        OpenStreams.Add(fileStream, new Tuple<Stream, string>(stream, item.Key));
                        return await Task.FromResult<IRandomAccessStream>(stream.AsRandomAccessStream());
                    }

                }
            }
            return await Task.FromResult<IRandomAccessStream>(null);
        }

        public IAsyncOperation<IRandomAccessStream> GetStreamReaderAsync(int fileStream)
        {
            return GetStreamAsync(fileStream, true);
        }

        public IAsyncOperation<IRandomAccessStream> GetStreamWriterAsync(int fileStream)
        {
            return GetStreamAsync(fileStream, false);
        }

        public IAsyncOperation<int> OpenFileAsync([ReadOnlyArray] string[] filters, int defaultFilterIdx)
        {
            return OpenFileAsyncTask(filters, defaultFilterIdx).AsAsyncOperation();
        }
        private async Task<int> OpenFileAsyncTask(string[] filters, int defaultFilterIdx)
        {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
            openPicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            foreach (var filter in filters)
                foreach (var val in filterToKvp(filter).Value)
                    openPicker.FileTypeFilter.Add(val);

            openPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

            var file = await openPicker.PickSingleFileAsync();
            var filenum = getFileNum();
            var stream = await file.OpenStreamForWriteAsync();
            Files.Add(new FileEntry(file.Name, file));

            OpenStreams.Add(filenum, new Tuple<Stream, string>(stream, file.Name));

            return filenum;
        }

        private static KeyValuePair<string, IList<string>> filterToKvp(string filter)
        {
            var key = "";
            var val = "";
            if (filter.Contains("|"))
            {
                var vals = filter.Split(new[] { '|' }, 2);
                key = vals[0];
                val = vals.Last();
            }
            else
            {
                key = val = filter;
            }
            var kvp = new KeyValuePair<string, IList<String>>(key, new List<string> { val });
            return kvp;
        }

        public IAsyncOperation<int> OpenLocalFileForReadAsync(string fileName)
        {
            return OpenLocalFileForReadAsyncTask(fileName).AsAsyncOperation();
        }
        private async Task<int> OpenLocalFileForReadAsyncTask(string fileName)
        {
            try
            {
                FileStreamInfo fi = getFileStream(fileName);
                if (fi != null && fi.fileNum < 1)
                    fi.fileNum = getFileNum();
                if (fi != null && fi.stream != null)
                    return await Task.FromResult(fi.fileNum);
                else if (fi != null && fi.file != null)
                {
                    fi.readOnly = true;
                    
                    fi.stream = await fi.file.OpenStreamForReadAsync();
                    OpenStreams.Add(fi.fileNum, new Tuple<Stream, string>(fi.stream, fi.fileName));
                    return await Task.FromResult(fi.fileNum);
                }
                else if (fi != null && fi.buffer != null)
                {
                    fi.stream = new MemoryStream(fi.buffer);
                    OpenStreams.Add(fi.fileNum, new Tuple<Stream, string>(fi.stream, fi.fileName));
                    return await Task.FromResult(fi.fileNum);
                }

                var _fil = await getFileFullAsync(fileName);
                var fileNum = (fi != null && fi.fileNum > 0) ? fi.fileNum : getFileNum();
                Files.Add(new FileEntry(_fil.Name, _fil));
                var stream = await _fil.OpenStreamForReadAsync();
                OpenStreams.Add(fileNum, new Tuple<Stream, string>(stream, _fil.Name));
                return fileNum;
            }
            catch (Exception ex)
            {

            }
            return -1;
        }

        public IAsyncOperation<int> OpenLocalFileForWriteAsync(string fileName)
        {
            return OpenLocalFileForWriteAsyncTask(fileName).AsAsyncOperation();
        }
        public IAsyncOperation<int> OpenLocalFileForWriteAsync(string fileName, bool createIfNotExists)
        {
            return OpenLocalFileForWriteAsyncTask(fileName, createIfNotExists).AsAsyncOperation();
        }
        private async Task<int> OpenLocalFileForWriteAsyncTask(string fileName, bool createIfNotExists = false)
        {
            try
            {
                FileStreamInfo fi = getFileStream(fileName);
                if (fi != null && fi.fileNum < 1)
                    fi.fileNum = getFileNum();
                if (fi != null && fi.stream != null)
                    return await Task.FromResult(fi.fileNum);
                else if (fi != null && fi.file != null)
                {
                    fi.readOnly = true;
                    
                    fi.stream = await fi.file.OpenStreamForWriteAsync();
                    OpenStreams.Add(fi.fileNum, new Tuple<Stream, string>(fi.stream, fi.fileName));
                    return await Task.FromResult(fi.fileNum);
                }
                else if (fi != null && fi.buffer != null)
                {
                    fi.stream = new MemoryStream(fi.buffer);
                    OpenStreams.Add(fi.fileNum, new Tuple<Stream, string>(fi.stream, fi.fileName));
                    return await Task.FromResult(fi.fileNum);
                }

                var _fil = await getFileFullAsync(fileName);
                // TODO: Detect non-existent file and create if the flag says to do so
                if(!createIfNotExists && _fil.GetBasicPropertiesAsync().GetResults().Size == 0)
                {
                    return -1;
                }

                var fileNum = (fi != null && fi.fileNum > 0) ? fi.fileNum : getFileNum();
                Files.Add(new FileEntry(_fil.Name, _fil));
                var stream = await _fil.OpenStreamForWriteAsync();
                OpenStreams.Add(fileNum, new Tuple<Stream, string>(stream, _fil.Name));
                return fileNum;
            }
            catch (Exception ex)
            {

            }
            return -1;
        }

        public char Putc(char chr, int fileNum)
        {
            var strm = getStreamFor(fileNum);

            if (strm != null)
            {
                strm.WriteByte((byte) chr);
                strm.Flush();
                return chr;
            }
            // else Error??
            return (char) 0;
        }

        public IAsyncOperation<int> SaveFileAsync(string fileName, [ReadOnlyArray] string[] filters, int defaultFilterIdx)
        {
            return SaveFileAsyncTask(fileName, filters, defaultFilterIdx).AsAsyncOperation();
        }
        private async Task<int> SaveFileAsyncTask(string fileName, string[] filters, int defaultFilterIdx)
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();

            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            savePicker.SuggestedFileName = fileName;
            foreach (var filter in filters)
                savePicker.FileTypeChoices.Add(filterToKvp(filter));

            var file = await savePicker.PickSaveFileAsync();
            var fileNum = getFileNum();
            Files.Add(new FileEntry(file.Name, file));
            var stream = await file.OpenStreamForWriteAsync();
            OpenStreams.Add(fileNum, new Tuple<Stream, string>(stream, file.Name));
            return fileNum;
        }

        public void Seek(int fileNum, long pos, FSeekOrigin seekOrigin)
        {
            var strm = getStreamFor(fileNum);
            if (strm == null)
                return;
            strm.Seek(pos, (SeekOrigin) ((int) seekOrigin));
        }

        public long SizeOfFile(int fileNum)
        {
            var strm = getStreamFor(fileNum);
            if (strm == null)
                return -1;
            return strm.Length;
        }

        /// <summary>
        /// Opens the file whose name is specified in the parameter filename and associates it with a stream 
        /// that can be identified in future operations by the FILE pointer returned.
        /// 
        /// The operations that are allowed on the stream and how these are performed are defined by the mode parameter.
        /// The returned stream is fully buffered by default if it is known to not refer to an interactive device(see setbuf).
        /// 
        /// The returned pointer can be disassociated from the file by calling fclose or freopen.All opened files are 
        /// automatically closed on normal program termination.
        /// 
        /// The running environment supports at least FOPEN_MAX files open simultaneously.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mode">
        /// C string containing a file access mode.It can be:
        /// "r" -  read: Open file for input operations. The file must exist.
        /// "w" -  write: Create an empty file for output operations. If a 
        ///        file with the same name already exists, its contents are discarded 
        ///        and the file is treated as a new empty file. 
        /// "a" -  append: Open file for output at the end of a file. Output operations 
        ///        always write data at the end of the file, expanding it. Repositioning 
        ///        operations (fseek, fsetpos, rewind) are ignored. The file is created if it does not exist.
        /// "r+" - read/update: Open a file for update (both for input and output). The file must exist.
        /// "w+" - write/update: Create an empty file and open it for update (both for input and output). If 
        ///        a file with the same name already exists its contents are discarded and the file is treated 
        ///        as a new empty file.
        /// "a+" - append/update: Open a file for update (both for input and output) with all output operations 
        ///        writing data at the end of the file. Repositioning operations (fseek, fsetpos, rewind) affects 
        ///        the next input operations, but output operations move the position back to the end of file. 
        ///        The file is created if it does not exist.
        /// "b"  - Open in binary mode (ignored here)
        /// </param>
        /// <returns></returns>
        public int FOpen(string name, string mode)
        {
            var lmode = mode.ToLowerInvariant();
            if (lmode.Contains("w") || lmode.Contains("r+"))
                return CreateLocalFileForWriteAsyncTask(name, true).Result;
            if (lmode.Contains("a"))
                return OpenLocalFileForWriteAsyncTask(name, true).Result;

            // else "r"
            return OpenLocalFileForReadAsyncTask(name).Result;
        }

        /// <summary>
        /// Clear error indicators
        /// Resets both the error and the eof indicators of the stream.
        /// When a i/o function fails either because of an error or because the end of the file has been reached, one of these 
        /// internal indicators may be set for the stream.The state of these indicators is cleared by a call to this function, 
        /// or by a call to any of: rewind, fseek, fsetpos and freopen.
        /// </summary>
        /// <param name="fileNum"></param>
        public void ClearErr(int fileNum)
        {
            // Does nothing right now
            throw new NotImplementedException("Not Implemented");
        }

        /// <summary>
        /// Close file
        /// Closes the file associated with the stream and disassociates it.
        ///
        /// All internal buffers associated with the stream are disassociated from it and flushed: the content of any 
        /// unwritten output buffer is written and the content of any unread input buffer is discarded.
        ///
        /// Even if the call fails, the stream passed as parameter will no longer be associated with the file nor its buffers.
        /// </summary>
        /// <param name="fileNum"></param>
        /// <returns>
        /// If the stream is successfully closed, a zero value is returned.
        /// On failure, EOF is returned.
        /// </returns>
        public void FClose(int fileNum)
        {
            // Does nothing right now
            throw new NotImplementedException("Not Implemented");
        }

        /// <summary>
        /// Check end-of-file indicator
        /// Checks whether the end-of-File indicator associated with stream is set, returning a value different from zero 
        /// if it is.
        /// 
        /// This indicator is generally set by a previous operation on the stream that attempted to read at or past the 
        /// end-of-file.
        /// 
        /// Notice that stream's internal position indicator may point to the end-of-file for the next operation, but 
        /// still, the end-of-file indicator may not be set until an operation attempts to read at that point.
        /// 
        /// This indicator is cleared by a call to clearerr, rewind, fseek, fsetpos or freopen.Although if the position 
        /// indicator is not repositioned by such a call, the next i/o operation is likely to set the indicator again.
        /// </summary>
        /// <param name="fileNum"></param>
        /// <returns>
        /// A non-zero value is returned in the case that the end-of-file indicator associated with the stream is set.
        /// Otherwise, zero is returned.
        /// </returns>
        public void FEof(int fileNum)
        {
            // Does nothing right now
            throw new NotImplementedException("Not Implemented");
        }

        /// <summary>
        /// Check error indicator
        /// Checks if the error indicator associated with stream is set, returning a value different from zero if it is.
        /// 
        /// This indicator is generally set by a previous operation on the stream that failed, and is cleared by a call 
        /// to clearerr, rewind or freopen.
        /// </summary>
        /// <param name="fileNum"></param>
        /// <returns>
        /// A non-zero value is returned in the case that the error indicator associated with the stream is set.
        /// Otherwise, zero is returned.
        /// </returns>
        public void FError(int fileNum)
        {
            // Does nothing right now
            throw new NotImplementedException("Not Implemented");
        }

        /// <summary>
        /// Flush stream
        /// If the given stream was open for writing(or if it was open for updating and the last i/o operation was an 
        /// output operation) any unwritten data in its output buffer is written to the file.
        /// 
        /// If stream is a null pointer, all such streams are flushed.
        /// 
        /// In all other cases, the behavior depends on the specific library implementation. In some implementations, 
        /// flushing a stream open for reading causes its input buffer to be cleared (but this is not portable 
        /// expected behavior).
        /// 
        /// The stream remains open after this call.
        /// 
        /// When a file is closed, either because of a call to fclose or because the program terminates, all the 
        /// buffers associated with it are automatically flushed.
        /// </summary>
        /// <param name="fileNum"></param>
        /// <returns>
        /// A zero value indicates success.
        /// If an error occurs, EOF is returned and the error indicator is set(see ferror).
        /// </returns>
        public int FFlush(int fileNum)
        {
            try
            {
                Flush(fileNum);
                return 0; // flushed
            } catch (Exception ex)
            {
            }
            return EOF;
        }

        /// <summary>
        /// Get current position in stream
        /// Retrieves the current position in the stream.
        /// 
        /// The function fills the fpos_t object pointed by pos with the information needed from the stream's position 
        /// indicator to restore the stream to its current position (and multibyte state, if wide-oriented) with a call 
        /// to fsetpos.
        /// 
        /// The ftell function can be used to retrieve the current position in the stream as an integer value.
        /// </summary>
        /// <param name="fileNum"></param>
        /// <param name="pos"></param>
        /// <returns>
        /// On success, the function returns zero.
        /// In case of error, errno is set to a platform-specific positive value and the function returns a non-zero value.
        /// </returns>
        public int FGetPos(int fileNum, FPos_T pos)
        {
            if (pos == null)
                pos = new FPos_T();

            try
            {
                pos.off = GetPosition(fileNum);
                return 0;
            }
            catch (Exception ex)
            {
                
            }
            return -1;
        }

        /// <summary>
        /// Write formatted data to stream
        /// Writes the C string pointed by format to the stream.If format includes format 
        /// specifiers(subsequences beginning with %), the additional arguments following 
        /// format are formatted and inserted in the resulting string replacing their respective 
        /// specifiers.
        /// 
        /// After the format parameter, the function expects at least as many additional arguments as specified by fo
        /// </summary>
        /// <param name="fileNum"></param>
        /// <param name="format"></param>
        /// <param name="parms"></param>
        /// <returns>
        /// On success, the total number of characters written is returned.
        /// 
        /// If a writing error occurs, the error indicator(ferror) is set and a negative number is returned.
        /// 
        /// If a multibyte character encoding error occurs while writing wide characters, errno is set to EILSEQ 
        /// and a negative number is returned.
        /// </returns>
        public int FPrintf(int fileNum, string format, params object[] parms)
        {
            var strm = getStreamFor(fileNum);
            if (strm == null)
                return -1;

            var printThis = string.Format(format, parms);
            var data = Encoding.UTF8.GetBytes(printThis.ToCharArray(), 0, printThis.Length);

            strm.Write(data, 0, data.Length);

            return data.Length;
        }

        /// <summary>
        /// Write character to stream
        /// Writes a character to the stream and advances the position indicator.
        /// 
        /// The character is written at the position indicated by the internal 
        /// position indicator of the stream, which is then automatically advanced by one.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="fileNum"></param>
        /// <returns>
        /// On success, the character written is returned.
        /// If a writing error occurs, EOF is returned and the error indicator(ferror) is set.
        /// </returns>
        public int FPutc(int character, int fileNum)
        {
            // Does nothing right now
            throw new NotImplementedException("Not Implemented");
        }

        /// <summary>
        /// Write string to stream
        /// Writes the C string pointed by str to the stream.
        /// 
        /// The function begins copying from the address specified(str) 
        /// until it reaches the terminating null character('\0'). This 
        /// terminating null-character is not copied to the stream.
        /// 
        /// Notice that fputs not only differs from puts in that the 
        /// destination stream can be specified, but also fputs does 
        /// not write additional characters, while puts appends a 
        /// newline character at the end automatically.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="fileNum"></param>
        /// <returns>
        /// On success, a non-negative value is returned.
        /// On error, the function returns EOF and sets the error indicator(ferror).
        /// </returns>
        public int FPuts(string str, int fileNum)
        {
            // Does nothing right now
            throw new NotImplementedException("Not Implemented");
        }

        /// <summary>
        /// Reopen stream with different file or mode
        /// Reuses stream to either open the file specified by filename or to change its access mode.
        /// 
        /// If a new filename is specified, the function first attempts to close any file already 
        /// associated with stream(third parameter) and disassociates it.Then, independently of 
        /// whether that stream was successfuly closed or not, freopen opens the file specified by 
        /// filename and associates it with the stream just as fopen would do using the specified mode.
        /// 
        /// If filename is a null pointer, the function attempts to change the mode of the stream. 
        /// Although a particular library implementation is allowed to restrict the changes permitted, 
        /// and under which circumstances.
        /// 
        /// The error indicator and eof indicator are automatically cleared (as if clearerr was called).
        /// 
        /// This function is especially useful for redirecting predefined streams like stdin, stdout and 
        /// stderr to specific files(see the example below).
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="mode"></param>
        /// <param name="fileNum"></param>
        /// <returns>
        /// If the file is successfully reopened, the function returns the pointer passed as parameter 
        /// stream, which can be used to identify the reopened stream.
        /// 
        /// Otherwise, a null pointer is returned.
        /// 
        /// On most library implementations, the errno variable is also set to a system-specific error code on failure.
        /// </returns>
        public int FReopen(string filename, string mode, int fileNum)
        {
            // Does nothing right now
            throw new NotImplementedException("Not Implemented");
        }

        /// <summary>
        /// Read formatted data from stream
        /// Reads data from the stream and stores them according to the parameter format into the locations 
        /// pointed by the additional arguments.
        /// 
        /// The additional arguments should point to already allocated objects of the type specified by 
        /// their corresponding format specifier within the format string.
        /// </summary>
        /// <param name="fileNum"></param>
        /// <param name="format"></param>
        /// <param name="parms"></param>
        /// <returns>
        /// On success, the function returns the number of items of the argument list successfully filled. 
        /// This count can match the expected number of items or be less (even zero) due to a matching failure, 
        /// a reading error, or the reach of the end-of-file.
        /// 
        /// If a reading error happens or the end-of-file is reached while reading, the proper indicator is 
        /// set(feof or ferror). And, if either happens before any data could be successfully read, EOF is returned.
        /// 
        /// If an encoding error happens interpreting wide characters, the function sets errno to EILSEQ.
        /// </returns>
        public int FScanf(int fileNum, string format, params object[] parms)
        {
            // Does nothing right now
            throw new NotImplementedException("Not Implemented");
        }

        /// <summary>
        /// Reposition stream position indicator
        /// Sets the position indicator associated with the stream to a new position.
        /// 
        /// For streams open in binary mode, the new position is defined by adding offset to a reference 
        /// position specified by origin.
        /// 
        /// For streams open in text mode, offset shall either be zero or a value returned by a previous 
        /// call to ftell, and origin shall necessarily be SEEK_SET.
        /// 
        /// If the function is called with other values for these arguments, support depends on the 
        /// particular system and library implementation(non-portable).
        /// 
        /// The end-of-file internal indicator of the stream is cleared after a successful call to this 
        /// function, and all effects from previous calls to ungetc on this stream are dropped.
        /// 
        /// On streams open for update (read+write), a call to fseek allows to switch between reading and writing.
        /// </summary>
        /// <param name="fileNum"></param>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns>
        /// If successful, the function returns zero.
        /// Otherwise, it returns non-zero value.
        /// If a read or write error occurs, the error indicator (ferror) is set.
        /// </returns>
        public int FSeek(int fileNum, long offset, FSeekOffset origin)
        {
            // Does nothing right now
            throw new NotImplementedException("Not Implemented");
        }

        /// <summary>
        /// Set position indicator of stream
        /// Restores the current position in the stream to pos.
        /// 
        /// The internal file position indicator associated with stream is set to the position represented by 
        /// pos, which is a pointer to an fpos_t object whose value shall have been previously obtained by a call 
        /// to fgetpos.
        /// 
        /// The end-of-file internal indicator of the stream is cleared after a successful call to this function, 
        /// and all effects from previous calls to ungetc on this stream are dropped.
        /// 
        /// On streams open for update (read+write), a call to fsetpos allows to switch between reading and writing.
        /// 
        /// A similar function, fseek, can be used to set arbitrary positions on streams open in binary mode.
        /// </summary>
        /// <param name="fileNum"></param>
        /// <param name="pos"></param>
        /// <returns>
        /// If successful, the function returns zero.
        /// On failure, a non-zero value is returned and errno is set to a system-specific positive value.
        /// </returns>
        public int FSetPos(int fileNum, FPos_T pos)
        {
            // Does nothing right now
            throw new NotImplementedException("Not Implemented");
        }

        /// <summary>
        /// Get current position in stream
        /// Returns the current value of the position indicator of the stream.
        /// 
        /// For binary streams, this is the number of bytes from the beginning of the file.
        /// 
        /// For text streams, the numerical value may not be meaningful but can still be used to restore the 
        /// position to the same position later using fseek (if there are characters put back using ungetc 
        /// still pending of being read, the behavior is undefined).
        /// </summary>
        /// <param name="fileNum"></param>
        /// <returns>
        /// On success, the current value of the position indicator is returned.
        /// On failure, -1L is returned, and errno is set to a system-specific positive value.
        /// </returns>
        public long FTell(int fileNum)
        {
            // Does nothing right now
            throw new NotImplementedException("Not Implemented");
        }

        /// <summary>
        /// Remove file
        /// Deletes the file whose name is specified in filename.
        /// 
        /// This is an operation performed directly on a file identified by its filename; No streams are 
        /// involved in the operation.
        /// 
        /// Proper file access shall be available.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>
        /// If the file is successfully deleted, a zero value is returned.
        /// On failure, a nonzero value is returned.
        /// On most library implementations, the errno variable is also set to a system-specific error code on failure.
        /// </returns>
        public int Remove(string filename)
        {
            // Does nothing right now
            throw new NotImplementedException("Not Implemented");
        }

        /// <summary>
        /// Rename file
        /// Changes the name of the file or directory specified by oldname to newname.
        /// 
        /// This is an operation performed directly on a file; No streams are involved in the operation.
        /// 
        /// If oldname and newname specify different paths and this is supported by the system, the file 
        /// is moved to the new location.
        /// 
        /// If newname names an existing file, the function may either fail or override the existing file, 
        /// depending on the specific system and library implementation.
        /// 
        /// Proper file access shall be available.
        /// </summary>
        /// <param name="oldname"></param>
        /// <param name="newname"></param>
        /// <returns>
        /// If the file is successfully renamed, a zero value is returned.
        /// On failure, a nonzero value is returned.
        /// On most library implementations, the errno variable is also set to a system-specific error code on failure.
        /// </returns>
        public int Rename(string oldname, string newname)
        {
            // Does nothing right now
            throw new NotImplementedException("Not Implemented");
        }

        /// <summary>
        /// Set position of stream to the beginning
        /// Sets the position indicator associated with stream to the beginning of the file.
        /// 
        /// The end-of-file and error internal indicators associated to the stream are cleared after a successful 
        /// call to this function, and all effects from previous calls to ungetc on this stream are dropped.
        /// 
        /// On streams open for update (read+write), a call to rewind allows to switch between reading and writing.
        /// </summary>
        /// <param name="filenum"></param>
        public void Rewind(int filenum)
        {
            Seek(filenum, 0, FSeekOrigin.Begin);
        }

        /// <summary>
        /// Set stream buffer
        /// Specifies the buffer to be used by the stream for I/O operations, which becomes a fully buffered stream.
        /// Or, alternatively, if buffer is a null pointer, buffering is disabled for the stream, which becomes an 
        /// unbuffered stream.
        /// 
        /// This function should be called once the stream has been associated with an open file, but before any input 
        /// or output operation is performed with it.
        /// 
        /// The buffer is assumed to be at least BUFSIZ bytes in size (see setvbuf to specify a size of the buffer).
        /// 
        /// A stream buffer is a block of data that acts as intermediary between the i/o operations and the physical 
        /// file associated to the stream: For output buffers, data is output to the buffer until its maximum capacity 
        /// is reached, then it is flushed(i.e.: all data is sent to the physical file at once and the buffer cleared). 
        /// Likewise, input buffers are filled from the physical file, from which data is sent to the operations until 
        /// exhausted, at which point new data is acquired from the file to fill the buffer again.
        /// 
        /// Stream buffers can be explicitly flushed by calling fflush.They are also automatically flushed by fclose 
        /// and freopen, or when the program terminates normally.
        /// 
        /// A full buffered stream uses the entire size of the buffer as buffer whenever enough data is available 
        /// (see setvbuf for other buffer modes).
        /// 
        /// All files are opened with a default allocated buffer(fully buffered) if they are known to not refer to 
        /// an interactive device.This function can be used to either set a specific memory block to be used as buffer 
        /// or to disable buffering for the stream.
        /// 
        /// The default streams stdin and stdout are fully buffered by default if they are known to not refer to an 
        /// interactive device. Otherwise, they may either be line buffered or unbuffered by default, depending on 
        /// the system and library implementation.The same is true for stderr, which is always either line buffered 
        /// or unbuffered by default.
        /// 
        /// A call to this function is equivalent to calling setvbuf with _IOFBF as mode and BUFSIZ as size (when 
        /// buffer is not a null pointer), or equivalent to calling it with _IONBF as mode(when it is a null pointer).
        /// </summary>
        /// <param name="filenum"></param>
        /// <param name="buffer"></param>
        public void SetBuf(int filenum, [WriteOnlyArray] byte[] buffer)
        {
            // Does nothing right now
            throw new NotImplementedException("Not Implemented");
        }

        /// <summary>
        /// Change stream buffering
        /// Specifies a buffer for stream.The function allows to specify the mode and size of the buffer(in bytes).
        /// 
        /// If buffer is a null pointer, the function automatically allocates a buffer(using size as a hint on the 
        /// size to use). Otherwise, the array pointed by buffer may be used as a buffer of size bytes.
        /// 
        /// This function should be called once the stream has been associated with an open file, but before any input 
        /// or output operation is performed with it.
        /// 
        /// A stream buffer is a block of data that acts as intermediary between the i/o operations and the physical 
        /// file associated to the stream: For output buffers, data is output to the buffer until its maximum capacity 
        /// is reached, then it is flushed(i.e.: all data is sent to the physical file at once and the buffer cleared). 
        /// Likewise, input buffers are filled from the physical file, from which data is sent to the operations until 
        /// exhausted, at which point new data is acquired from the file to fill the buffer again.
        /// 
        /// Stream buffers can be explicitly flushed by calling fflush.They are also automatically flushed by fclose 
        /// and freopen, or when the program terminates normally.
        /// 
        /// All files are opened with a default allocated buffer (fully buffered) if they are known to not refer to 
        /// an interactive device.This function can be used to either redefine the buffer size or mode, to define a 
        /// user-allocated buffer or to disable buffering for the stream.
        /// 
        /// The default streams stdin and stdout are fully buffered by default if they are known to not refer to an 
        /// interactive device. Otherwise, they may either be line buffered or unbuffered by default, depending on 
        /// the system and library implementation.The same is true for stderr, which is always either line buffered or 
        /// unbuffered by default.
        /// </summary>
        /// <param name="filenum"></param>
        /// <param name="buffer"></param>
        /// <param name="mode"></param>
        /// <param name="size"></param>
        /// <returns>
        /// If the buffer is correctly assigned to the file, a zero value is returned.
        /// Otherwise, a non-zero value is returned; This may be due to an invalid mode parameter or to some other 
        /// error allocating or assigning the buffer.
        /// </returns>
        public int SetVBuf(int filenum, [WriteOnlyArray] byte[] buffer, int mode, long size)
        {
            // Does nothing right now
            throw new NotImplementedException("Not Implemented");
        }

        /// <summary>
        /// Open a temporary file
        /// 
        /// Creates a temporary binary file, open for update("wb+" mode, see fopen for details) with a filename 
        /// guaranteed to be different from any other existing file.
        /// 
        /// The temporary file created is automatically deleted when the stream is closed (fclose) or when the 
        /// program terminates normally.If the program terminates abnormally, whether the file is deleted depends 
        /// on the specific system and library implementation.
        /// </summary>
        /// <returns>
        /// If successful, the function returns a stream pointer to the temporary file created.
        /// On failure, NULL is returned.
        /// </returns>
        public int TmpFile()
        {
            // Does nothing right now
            throw new NotImplementedException("Not Implemented");
        }

        /// <summary>
        /// Generate temporary filename
        /// Returns a string containing a file name different from the name of any existing file, and thus suitable to safely create a temporary file without risking to overwrite an existing file.
        /// If str is a null pointer, the resulting string is stored in an internal static array that can be accessed by the return value.The content of this string is preserved at least until a subsequent call to this same function, which may overwrite it.
        /// If str is not a null pointer, it shall point to an array of at least L_tmpnam characters that will be filled with the proposed temporary file name.
        /// The file name returned by this function can be used to create a regular file using fopen to be used as a temporary file.The file created this way, unlike those created with tmpfile is not automatically deleted when closed; A program shall call remove to delete this file once closed.
        /// </summary>
        /// <returns>
        /// On success, a pointer to the C string containing the proposed name for a temporary file:
        /// 
        ///     - If str was a null pointer, this points to an internal buffer(whose content is preserved at least until the 
        ///       next call to this function).
        ///     - If str was not a null pointer, str is returned.
        /// If the function fails to create a suitable filename, it returns a null pointer.
        /// </returns>
        public string TmpNam()
        { 
            // Does nothing right now
            throw new NotImplementedException("Not Implemented");
        }

        /// <summary>
        /// Unget character from stream
        /// A character is virtually put back into an input stream, decreasing its internal file position as if a previous getc operation was undone.
        /// 
        /// This character may or may not be the one read from the stream in the preceding input operation. In any case, the next character retrieved from stream is the character passed to this function, independently of the original one.
        /// 
        /// Notice though, that this only affects further input operations on that stream, and not the content of the physical file associated with it, which is not modified by any calls to this function.
        /// 
        /// Some library implementations may support this function to be called multiple times, making the characters available in the reverse order in which they were put back.Although this behavior has no standard portability guarantees, and further calls may simply fail after any number of calls beyond the first.
        /// 
        /// If successful, the function clears the end-of-file indicator of stream (if it was currently set), and decrements its internal file position indicator if it operates in binary mode; In text mode, the position indicator has unspecified value until all characters put back with ungetc have been read or discarded.
        /// 
        /// A call to fseek, fsetpos or rewind on stream will discard any characters previously put back into it with this function.
        /// 
        /// If the argument passed for the character parameter is EOF, the operation fails and the input stream remains unchanged.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="filenum"></param>
        /// <returns>
        /// On success, the character put back is returned.
        /// If the operation fails, EOF is returned.
        /// </returns>
        public int Ungetc(int character, int filenum)
        {
            // Does nothing right now
            throw new NotImplementedException("Not Implemented");
        }
        
    }
}
