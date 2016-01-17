using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using IFControls.Internal;
using IFInterfaces.Support;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;

namespace IFControls.Services
{
    public sealed partial class SimpleRuntimeInterface
    {
        class FileStreamInfo
        {
            internal StorageFile file;
            internal Stream stream;
            internal bool readOnly;
            internal bool isArchive;
            internal int archiveNum;
            internal string fileName;
        }
        List<KeyValuePair<string, byte[]>> archiveData;

        List<FileStreamInfo> files = new List<FileStreamInfo>();

        public bool SupportsSaveLoadGame { get { return true; } }

        public string GetFile(uint fileNum)
        {
            if (files.Count <= files.Count)
                return "";
            return files[(int)fileNum].fileName;
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

            var fs = await openPicker.PickSingleFileAsync();
            return await addFileStreamInfoFromFileAndGetNumber(fs, true);
        }

        public IAsyncOperation<int> SaveFileAsync(string fileName, [ReadOnlyArray] string[] filters, int defaultFilterIdx)
        {
            return SaveFileAsyncTask(fileName, filters, defaultFilterIdx).AsAsyncOperation();
        }
        private async Task<int> SaveFileAsyncTask(string fileName, string[] filters, int defaultFilterIdx)
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();

            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            savePicker.SuggestedFileName = fileName;
            foreach (var filter in filters)
                savePicker.FileTypeChoices.Add(filterToKvp(filter));

            var fs = await savePicker.PickSaveFileAsync();
            return await addFileStreamInfoFromFileAndGetNumber(fs);
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

        public IAsyncOperation<IRandomAccessStream> GetStreamAsync(int fileStream, bool readOnly)
        {
            return GetRAStreamAsyncTask(fileStream, readOnly).AsAsyncOperation();
        }
        private async Task<IRandomAccessStream> GetRAStreamAsyncTask(int fileStream, bool readOnly)
        {
            var stream = await GetStreamAsyncTask(fileStream, readOnly);
            return stream.AsRandomAccessStream();
        }
        private async Task<Stream> GetStreamAsyncTask(int fileStream, bool readOnly)
        {
            if (files.Count <= fileStream)
                return await Task.FromResult<Stream>(null);

            var fi = files[fileStream];

            if (fi.readOnly != readOnly && fi.readOnly)
                return await Task.FromResult<Stream>(null);

            if (fi.stream != null)
                return fi.stream;

            if (readOnly)
                fi.stream = await fi.file.OpenStreamForReadAsync();
            else
                fi.stream = await fi.file.OpenStreamForWriteAsync();
            return fi.stream;
        }

        public IAsyncOperation<IRandomAccessStream> GetStreamReaderAsync(int fileStream)
        {
            return GetRAStreamReaderAsync(fileStream).AsAsyncOperation();
        }
        private async Task<IRandomAccessStream> GetRAStreamReaderAsync(int fileStream)
        {
            var stream = await GetStreamReaderAsyncTask(fileStream);
            return stream.AsRandomAccessStream();
        }
        private async Task<Stream> GetStreamReaderAsyncTask(int fileStream)
        {
            if (files.Count <= fileStream)
                return await Task.FromResult<Stream>(null);

            var fi = files[fileStream];
            if (!fi.readOnly)
                return await Task.FromResult<Stream>(null);

            if (fi.stream != null)
                return fi.stream;

            fi.stream = await fi.file.OpenStreamForReadAsync();
            return fi.stream;
        }

        public IAsyncOperation<IRandomAccessStream> GetStreamWriterAsync(int fileStream)
        {
            return GetRAStreamWriterAsync(fileStream).AsAsyncOperation();
        }
        private async Task<IRandomAccessStream> GetRAStreamWriterAsync(int fileStream)
        {
            var stream = await GetStreamWriterAsyncTask(fileStream);
            return stream.AsRandomAccessStream();
        }
        private async Task<Stream> GetStreamWriterAsyncTask(int fileStream)
        {
            if (files.Count <= fileStream)
                return await Task.FromResult<Stream>(null);
            var fi = files[fileStream];
            if (fi.readOnly)
                return await Task.FromResult<Stream>(null);

            if (fi.stream != null)
                return fi.stream;

            fi.stream = await fi.file.OpenStreamForWriteAsync();
            return fi.stream;
        }

        public IAsyncOperation<bool> CloseStreamAsync(int fileStream)
        {
            return CloseStreamAsyncTask(fileStream).AsAsyncOperation();
        }
        private async Task<bool> CloseStreamAsyncTask(int fileStream)
        {
            if (files.Count <= fileStream)
                return await Task.FromResult<bool>(false);
            var fi = files[fileStream];
            if (fi.stream != null)
            {
                await fi.stream.FlushAsync();
                fi.stream.Dispose();
            }
            fi.stream = null;
            return await Task.FromResult<bool>(true);
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
                if (fi != null && fi.stream != null)
                    return await Task.FromResult(files.IndexOf(fi));
                else if (fi != null && fi.file != null)
                {
                    fi.readOnly = true;
                    fi.stream = await fi.file.OpenStreamForReadAsync();
                    return files.IndexOf(fi);
                }

                var item = await getFileFullAsync(fileName);
                return await addFileStreamInfoFromFileAndGetNumber(item);
            }
            catch (Exception ex)
            {

            }
            return -1;
        }

        public IAsyncOperation<int> CreateTempFileForWrite()
        {
            return CreateTempFileForWriteAsyncTask().AsAsyncOperation();
        }
        private async Task<int> CreateTempFileForWriteAsyncTask()
        {
            var fn = "ifngn_" + Guid.NewGuid().ToString().Replace("-", "");
            var item = await Windows.Storage.ApplicationData.Current.TemporaryFolder.CreateFileAsync(fn, CreationCollisionOption.GenerateUniqueName);
            return await addFileStreamInfoFromFileAndGetNumber(item);
        }
        private async Task<int> addFileStreamInfoFromFileAndGetNumber(StorageFile item, bool isReadOnly = false)
        {
            await Task.Run(() =>
            {

                files.Add(new FileStreamInfo
                {
                    //stream = await item.OpenStreamForWriteAsync(),
                    fileName = item.Name,
                    file = item,
                    readOnly = isReadOnly
                });
            });
            return files.Count - 1;
        }

        public IAsyncOperation<int> CreateLocalFileForWriteAsync(string fileName)
        {
            return CreateLocalFileForWriteAsyncTask(fileName).AsAsyncOperation();
        }
        private async Task<int> CreateLocalFileForWriteAsyncTask(string fileName)
        {
            try
            {
                var item = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                return await addFileStreamInfoFromFileAndGetNumber(item);
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
        private async Task<int> OpenLocalFileForWriteAsyncTask(string fileName)
        {
            try
            {

                FileStreamInfo fi = getFileStream(fileName);
                if (fi != null && fi.stream != null)
                    return await Task.FromResult(files.IndexOf(fi));
                else if (fi != null && fi.file != null)
                {
                    fi.readOnly = false;
                    fi.stream = await fi.file.OpenStreamForWriteAsync();
                    return files.IndexOf(fi);
                }
                var item = await getFileFullAsync(fileName);
                return await addFileStreamInfoFromFileAndGetNumber(item);
            }
            catch (Exception ex)
            {

            }
            return -1;
        }
        private FileStreamInfo getFileStream(string fileName)
        {
            foreach (FileStreamInfo item in files)
            {
                if (item.fileName.StartsWith(fileName))
                    return item;
            }
            return null;
        }
        private async Task<StorageFile> getFileFullAsync(string fileName)
        {
            // TODO: check for existing registered file
            var fs = getFileStream(fileName);
            if (fs != null)
                return fs.file;

            // TODO: detect file in archive
            return await getFileAsync(fileName);
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

        public IAsyncOperation<bool> DeleteFileAsync(string fileName)
        {
            return DeleteFileAsyncTask(fileName).AsAsyncOperation();
        }
        private async Task<bool> DeleteFileAsyncTask(string fileName)
        {
            try
            {
                StorageFile item;
                var fi = files.FirstOrDefault(i => i.fileName.Equals(fileName, StringComparison.OrdinalIgnoreCase) && files != null);
                if (fi != null)
                {
                    item = fi.file;
                    if (fi.stream != null)
                        fi.stream.Dispose();
                    fi.stream = null;
                    fi.file = null;
                    fi.fileName = "\t\t"; // Should be looking for a file name with this pattern (the idea is that this file is dead)
                }
                else
                {
                    item = await getFileAsync(fileName);
                }
                await item.DeleteAsync();
                return true;
            }
            catch (Exception ex) { }
            return false;
        }
        private Stream getStreamFor(int fileNum)
        {
            if (files.Count > fileNum)
            {
                return files[fileNum].stream;
            }
            return null;
        }

        public int FWrite([ReadOnlyArray] byte[] buf, int pos, int len, int fileNum)
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

        public string FGets([ReadOnlyArray] byte[] cbuf, int len, int fileNum)
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
            var chars = result.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                cbuf[i] = (byte) chars[i];
            }
            return result;
        }

        public void Seek(int fileNum, long pos, FSeekOrigin seekOrigin)
        {
            var strm = getStreamFor(fileNum);
            if (strm == null)
                return;
            strm.Seek(pos, (SeekOrigin) ((int) seekOrigin));
        }
        public long GetPosition(int filenum)
        {
            var strm = getStreamFor(filenum);
            if (strm == null)
                return -1;
            // else Error??
            return strm.Position;
        }

        public IAsyncAction AppendArchiveEntriesAsync([ReadOnlyArray] StringKeyByteArrayVal[] fileData)
        {
            return AppendArchiveEntriesAsyncTask(fileData).AsAsyncAction();
        }
        private async Task AppendArchiveEntriesAsyncTask(StringKeyByteArrayVal[] fileData)
        {
            await Task.Run(() =>
            {
                foreach (var entry in files.Where(i => i.isArchive))
                {
                    if (entry.stream != null)
                        entry.stream.Dispose();
                }
                files.RemoveAll(i => i.isArchive);

                archiveData = fileData.Select(i => new KeyValuePair<string, byte[]>(i.Key, i.Value)).ToList();

                for (var i = 0; i < archiveData.Count; i++)
                {
                    var item = archiveData[i];

                    files.Add(new FileStreamInfo
                    {
                        fileName = item.Key,
                        isArchive = true,
                        archiveNum = i
                    });
                }
            });
        }

        public void Flush(int fileNum)
        {
            var strm = getStreamFor(fileNum);
            strm.Flush();
        }

        public long SizeOfFile(int fp)
        {
            var strm = getStreamFor(fp);
            return strm.Length;
        }
    }
}
