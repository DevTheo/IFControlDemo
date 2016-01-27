using IFInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFInterfaces.Services;
using IFInterfaces.Support;
using Windows.Foundation;
using System.IO;

namespace IFEngine.ZMachineF
{
    public sealed class ZMachine1To5And7PlusIFEngine : IIFGameEngine
    {
        #region IIFGameEngine
        public string Identifier
        {
            get
            {
                return "Z1-5&7+";
            }
        }
        readonly string[] zfileExt = new[] { ".z1", ".z2", ".z3", ".z4", ".z5", ".z7", ".z8", ".z9", ".z10" };

        public string[] KnownExtensions
        {
            get
            {
                return new[] { ".bin", ".z1", ".z2", ".z3", ".z4", ".z5", ".z7", ".z8", ".z9", ".z10" };
            }
        }

        public CanRunResult CanRun(IFileService fileIO)
        {
            if(fileIO.GetFileNames().Any( i=>
                                zfileExt.Contains(i.Substring(i.Length-3, 3).ToLower()) || 
                                i.Substring(i.Length - 4, 4).ToLower() == ".z10"))
            {
                return CanRunResult.Yes;
            }
            else if (fileIO.GetFileNames().Any(i => i.ToLower().EndsWith(".bin")))
            {
                // parse file version.. for now return undetermined
                return CanRunResult.Maybe;
            }
            return CanRunResult.No;
        }

        public IAsyncOperation<ExecutionResult> Start(IIFRuntime runtime)
        {
            return Start(runtime, false);
        }

        public IAsyncOperation<ExecutionResult> Start(IIFRuntime runtime, bool debugMessages)
        {
            this.runtime = runtime;
            return StartAsyncTask(bool debugMessages);
        }
        #endregion

        #region data structures
        internal class StackFrame
        {
            public uint pc;
            public ushort start;
            public byte argc;
            public bool stored;
        }

        internal class Typechecker
        {
            //C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
            //  int var8[sizeof(byte NamelessParameter)==1?1:-9];
            //C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
            //  int var16[sizeof(ushort NamelessParameter)==2?1:-9];
            //C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
            //  int var32[sizeof(uint NamelessParameter)==4?1:-9];
            //C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
            //  int var64[sizeof(ulong NamelessParameter)==8?1:-9];
        }
        #endregion

        #region private members
        private IIFRuntime runtime = null;
        private IFileService fileIO
        {
            get
            {
                if (runtime == null)
                    return null;
                return runtime.FileIO;
            }
        }
        ExecutionResult execresult = ExecutionResult.ERR_NO_ERRORS;
        const string VERSION = "0.8.4";

        //internal readonly char[] zscii_conv_1 = new [] {[155 - 128] = 'a','o','u','A','O','U','s','>','<','e','i','y','E','I','a','e','i','o','u','y','A','E','I','O','U','Y', 'a','e','i','o','u','A','E','I','O','U','a','e','i','o','u','A','E','I','O','U','a','A','o','O','a','n', 'o','A','N','O','a','A','c','C','t','t','T','T','L','o','O','!','?'};

        //internal readonly string zscii_conv_2 = {[155 - 128] = 'e','e','e', [161 - 128] = 's','>','<', [211 - 128] = 'e','E', [215 - 128] = 'h','h','h','h', [220 - 128] = 'e','E'};
//        const char zscii_conv_1[128] ={
//  [155-128]=
//  'a','o','u','A','O','U','s','>','<','e','i','y','E','I','a','e','i','o','u','y','A','E','I','O','U','Y',
//  'a','e','i','o','u','A','E','I','O','U','a','e','i','o','u','A','E','I','O','U','a','A','o','O','a','n',
//  'o','A','N','O','a','A','c','C','t','t','T','T','L','o','O','!','?'
//};

//    const char zscii_conv_2[128] ={
//  [155-128]='e','e','e', [161-128]='s','>','<', [211-128]='e','E', [215-128]='h','h','h','h', [220-128]='e','E'
//};

internal string v1alpha = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789.,!?_#'\"/\\<-:()";
        internal string v2alpha = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ ^0123456789.,!?_#'\"/\\-:()";

        //internal const string optarg = {['a'] = 1,['e'] = 1,['g'] = 1,['i'] = 1,['o'] = 1,['r'] = 1,['s'] = 1};
        internal string[] opts = new string[128];

        internal string story_name;
        internal int story = 0;
        internal int transcript = 0;
        internal int inlog = 0;
        internal int outlog = 0;
        internal int auxfile = 0;
        internal byte[] auxname = new byte[11];
        internal bool original = true;
        internal sbyte verified = 0;
        internal sbyte allow_undo = 1;
        internal sbyte escape_code = 0;
        internal int sc_rows = 25;
        internal int sc_columns = 80;

        internal uint routine_start;
        internal uint text_start;
        internal int packed_shift;
        internal int address_shift;

        internal uint object_table;
        internal uint dictionary_table;
        internal uint restart_address;
        internal uint synonym_table;
        internal uint alphabet_table;
        internal uint static_start;
        internal uint global_table;

        internal byte[] memory = new byte[0x200000];
        internal int memoryIndex = 0;
        internal byte zmachineVersion = 0;
        //C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
        //ORIGINAL LINE: #define version (*memory)
        //#define version
        internal byte[] undomem = new byte[0x40000];
        internal uint program_counter;
        internal StackFrame[] frames = new StackFrame[256];
        internal int[] stack = new int[1024];
        internal int frameptr;
        internal int stackptr;
        internal StackFrame[] u_frames = new StackFrame[256];
        internal ushort[] u_stack = new ushort[1024];
        internal int u_frameptr;
        internal int u_stackptr;
        internal uint u_program_counter;
        //C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
        //ORIGINAL LINE: #define copy_array(d,s) memcpy(d,s,sizeof(s))
        //#define copy_array

        internal ushort[] stream3addr = new ushort[16];
        internal int stream3ptr = -1;
        internal bool texting = true;
        internal bool window = false;
        internal bool buffering = true;
        internal bool logging = false;
        internal bool from_log = false;
        internal int cur_row = 2;
        internal int cur_column;
        internal int lmargin = 0;
        internal int rmargin = 0;

        internal byte arcfour_i;
        internal byte arcfour_j;
        internal byte[] arcfour_s = new byte[256];
        internal ushort predictable_max;
        internal ushort predictable_value;
        internal bool unpredictable;

        internal ushort[] inst_args = new ushort[8];
        internal int inst_args_index;
        //C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
        //ORIGINAL LINE: #define inst_sargs ((S16*)inst_args)
        //#define inst_sargs
        internal string text_buffer = "";
        internal int textptr;
        internal byte cur_prop_size;

        internal int zch_shift;
        internal int zch_shiftlock;
        internal int zch_code;

        internal byte[] instruction_use = new byte[256];
        internal sbyte break_on;
        internal string instruction_bkpt = new string(new char[256]);
        internal uint[] address_bkpt = new uint[16];
        internal uint continuing;
        internal bool lastdebug = false;
        internal ushort oldscore = 0;
        #endregion

        #region macros
        Func<ushort, int, ushort> obj_tree_get = (o, f) => (ushort)0;
        Func<ushort, ushort> parent = (o) => (ushort) 0; //obj_tree_get(o, 0);
        Func<ushort, ushort> sibling = (o) => (ushort) 0; //obj_tree_get(o, 1);
        Func<ushort, ushort> child = (o) => (ushort) 0; //obj_tree_get(o, 2);
        Action<ushort, int, ushort> obj_tree_put = (obj, f, v) => { };
        Action<ushort, ushort> set_parent = (o, v) => {};
        Action<ushort, ushort> set_sibling = (o, v) => { };
        Action<ushort, ushort> set_child = (o, v) => { };
        Func<uint, uint> attribute = (o) =>  0;
        Func<uint, int> obj_prop_addr = (o) => 0;

        /*void obj_tree_put(ushort obj, int f, ushort v)
        {
            if (zmachineVersion > 3) write16(object_table + 118 + obj * 14 + f * 2, v);
            else memory[object_table + 57 + obj * 9 + f] = v;
        }*/

        void setupMacros()
        {
            if (zmachineVersion > 3)
            {
                obj_tree_get = (o, f) => read16((int)(object_table + 118 + ((uint)o) * 14 + (f) * 2));
                obj_tree_put = (obj, f, v) => write16((int)(object_table + 118 + obj * 14 + f * 2), v);
                attribute = (o) => object_table + 112 + (o) * 14;
                obj_prop_addr = (o) => read16((int)(object_table + 124 + (o) * 14)) << address_shift;
            }
            else
            {
                obj_tree_get = (o, f) => memory[(int)(object_table + 57 + ((uint)o) * 9 + (f))];
                obj_tree_put = (obj, f, v) => memory[(int) (object_table + 57 + obj * 9 + f)] = (byte)v;
                attribute = (o) => object_table + 53 + (o) * 9;
                obj_prop_addr = (o) => read16((int) (object_table + 60 + (o) * 9)) << address_shift;
            }
            parent = (o) => obj_tree_get(o, 0);
            sibling = (o) => obj_tree_get(o, 1);
            child = (o) => obj_tree_get(o, 2);
            set_parent = (o, v) => obj_tree_put(o, 0, v);
            set_sibling = (o, v) => obj_tree_put(o, 1, v);
            set_child = (o, v) => obj_tree_put(o, 2, v);
        }
        #endregion

        #region Instructions

        short get_random(short max)
        {
            int k;
            int v;
            int m;
            if (predictable_max != 0)
            {
                predictable_value = (ushort) ((predictable_value + 1) % predictable_max);
                return (short) ((predictable_value % max) + 1);
            }
            else
            {
                m = max - 1;
                m |= m >> 1;
                m |= m >> 2;
                m |= m >> 4;
                m |= m >> 8;
                if (unpredictable)
                {
                    arcfour_i ^= (byte) DateTime.Now.Ticks;
                }
                for (;;)
                {
                    arcfour_i++;
                    arcfour_j += (byte) (k = arcfour_s[arcfour_i]);
                    arcfour_s[arcfour_i] = arcfour_s[arcfour_j];
                    arcfour_s[arcfour_j] = (byte) k;
                    arcfour_i++;
                    arcfour_j += (byte) (k = arcfour_s[arcfour_i]);
                    arcfour_s[arcfour_i] = arcfour_s[arcfour_j];
                    arcfour_s[arcfour_j] = (byte) k;
                    v = arcfour_s[(arcfour_s[arcfour_i] + arcfour_s[arcfour_j]) & 255] << 8;
                    arcfour_i++;
                    arcfour_j += (byte) (k = arcfour_s[arcfour_i]);
                    arcfour_s[arcfour_i] = arcfour_s[arcfour_j];
                    arcfour_s[arcfour_j] = (byte) k;
                    v |= arcfour_s[(arcfour_s[arcfour_i] + arcfour_s[arcfour_j]) & 255];
                    v &= m;
                    if (v < max)
                    {
                        return (short) (v + 1);
                    }
                }
            }
        }

        void randomize(ushort seed)
        {
            int i;
            int j;
            int k;
            unpredictable = seed == (ushort) 0;
            predictable_value = 0;
            if (seed < 1000 && seed != 0)
            {
                predictable_max = seed;
                return;
            }
            predictable_max = 0;
            if (seed == 0)
            {
                seed = (ushort) (DateTime.Now.Ticks - DateTime.Today.Ticks);// (ushort) time(0);
            }
            for (i = 0; i < 256; i++)
            {
                arcfour_s[i] = (byte) i;
            }
            arcfour_i = 0;
            arcfour_j = (byte) (seed & 255);
            for (i = 0, j = 0; i < 256; i++)
            {
                j = (j + arcfour_s[i] + (seed >> (i & 7))) & 255;
                k = arcfour_s[i];
                arcfour_s[i] = arcfour_s[j];
                arcfour_s[j] = (byte) k;
            }
            get_random(4);
            get_random(4);
            get_random(4);
        }

        ushort read16(int address)
        {
            return (ushort) ((memory[address] << 8) | memory[address + 1]);
        }

        void write16(int address, int value)
        {
            memory[address] = (byte) (value >> 8);
            memory[address + 1] = (byte) (value & 255);
        }

        string text_flush_junk = "";
        async Task text_flush()
        {
            text_buffer = text_buffer.Substring(0, textptr);
            if (textptr + cur_column >= sc_columns - rmargin)
            {
                runtime.Write('\n');
                cur_row++;
                cur_column = 0;
                while (cur_column < lmargin)
                {
                    runtime.Write((char)32);
                    cur_column++;
                }
            }
            if (cur_row >= sc_rows && sc_rows != 255 && !from_log)
            {
                runtime.Write("[MORE]");
                //fflush(stdout);
                text_flush_junk = await runtime.GetLineAsync();
                //fileIO.FGets(text_flush_junk, 256, stdin);
                cur_row = 2;
            }

            runtime.Write(text_buffer);
            //fileIO.FPuts(text_buffer, stdout);
            cur_column += textptr;
            //fflush(stdout);
            textptr = 0;
        }

        ushort fetch(byte byteVal)
        {
            if ((byteVal & 0xF0) != 0)
            {
                return read16((int) (global_table + ((byteVal - 16) << 1)));
            }
            else if (byteVal != 0)
            {
                return (ushort) stack[frames[frameptr].start + byteVal - 1];
            }
            else
            {
                return (ushort) stack[--stackptr];
            }
        }

        void store(byte byteVal, int value)
        {
            if ((byteVal & 0xF0) != 0)
            {
                write16((int) (global_table + ((byteVal - 16) << 1)), value);
            }
            else if (byteVal != 0)
            {
                stack[(int) (frames[(int) frameptr].start + byteVal - 1)] = value;
            }
            else
            {
                stack[(int) (stackptr++)] = value;
            }
        }

        void storei(int value)
        {
            store(memory[program_counter++], value);
        }

        ushort aux_restore()
        {
            /*
            int i;
            int p;
            int s;
            byte[] fn = new byte[11];
            if (auxfile == null)
            {
                return 0;
            }
            clearerr(auxfile);
            aux_filename();
            for (i = 0; i < 32; i++)
            {
                fseek(auxfile, i * 13, SEEK_SET);
                fread(fn, 1, 11, auxfile);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcmp' has no equivalent in C#:
                if (!memcmp(fn, auxname, 11))
                {
                    p = fgetc(auxfile) << 8;
                    p |= fgetc(auxfile);
                    fseek(auxfile, (p << 6) + 416, SEEK_SET);
                    s = fgetc(auxfile) << 8;
                    s |= fgetc(auxfile);
                    if (s > inst_args[2])
                    {
                        s = inst_args[2];
                    }
                    fread(memory + inst_args[1], 1, s, auxfile);
                    return s;
                }
            }*/
            return 0;
        }

        ushort aux_save()
        {
            /*
            int i;
            int j;
            int p;
            int s;
            byte[] fn = new byte[11];
            byte[] bl = { 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32 };
            ushort[] pos = new ushort[32];
            ushort[] size = new ushort[32];
            if (auxfile > 0)
            {
                return 0;
            }
            fileIO.ClearErr(auxfile);
            aux_filename();
            // Find the free file number
            for (i = 0; i < 32; i++)
            {
                fseek(auxfile, i * 13, SEEK_SET);
                fread(fn, 1, 11, auxfile);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcmp' has no equivalent in C#:
                if (memcmp(fn, bl, 11))
                {
                    pos[i] = fgetc(auxfile) << 8;
                    pos[i] |= fgetc(auxfile);
                    fseek(auxfile, (p << 6) + 416, SEEK_SET);
                    size[i] = fgetc(auxfile) << 8;
                    size[i] |= fgetc(auxfile);
                }
                else
                {
                    pos[i] = size[i] = 0;
                }
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcmp' has no equivalent in C#:
                if (!memcmp(fn, auxname, 11))
                {
                    // Delete it
                    fseek(auxfile, i * 13, SEEK_SET);
                    for (j = 0; j < 11; j++)
                    {
                        fputc(32, auxfile);
                    }
                }
            }
            for (i = 0; i < 32; i++)
            {
                fseek(auxfile, i * 13, SEEK_SET);
                fread(fn, 1, 11, auxfile);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcmp' has no equivalent in C#:
                if (!memcmp(fn, bl, 11))
                    break;
            }
            fflush(auxfile);
            s = inst_args[2];
            if (i == 32 || s == 0)
            {
                return 0;
            }
            // Figure out what position to save
            p = aux_free_chunk(pos, size, (s + 65) >> 6);
            // Write filename
            fseek(auxfile, i * 13, SEEK_SET);
            fwrite(auxname, 1, 11, auxfile);
            fputc(p >> 8, auxfile);
            fputc(p & 255, auxfile);
            // Write data
            fseek(auxfile, (p << 6) + 416, SEEK_SET);
            fputc(s >> 8, auxfile);
            fputc(s & 255, auxfile);
            fwrite(memory + inst_args[1], 1, s, auxfile);
            return 1;
            */
            return 0;
        }

        void game_save_many(int fp, int count)
        {
            int i;
            while (count > 0)
            {
                fileIO.FPutc(0, fp);
                if (count >= 129)
                {
                    i = count;
                    if (i > 0x3FFF)
                    {
                        i = 0x3FFF;
                    }
                    fileIO.FPutc(((i - 1) & 0x7F) | 0x80, fp);
                    fileIO.FPutc((i - ((i - 1) & 0x7F) - 0x80) >> 7, fp);
                    count -= i;
                }
                else
                {
                    fileIO.FPutc(count - 1, fp);
                    count = 0;
                }
            }
        }

        async Task game_restore()
        {
            int fp;
            int i;
            int c;
            int d;
            int o;

            fp = await fileIO.PickFileForReadAsync(Path.GetFileNameWithoutExtension(story_name) + ".sav", new[] { ".sav" }, 0);

            if (fp <= 0)
                return;
            frameptr = fileIO.FGetc(fp) - 1;
            stackptr = 0;
            for (i = 0; i <= frameptr; i++)
            {
                c = fileIO.FGetc(fp);
                d = fileIO.FGetc(fp);
                frames[i].start = (ushort) stackptr;
                stackptr += (c << 1) | (d >> 7);
                frames[i].stored = (d & 0x40) != 0;
                frames[i].pc = (uint) ((d & 0x3F) << 16);
                frames[i].pc |= (uint) (fileIO.FGetc(fp) << 8);
                frames[i].pc |= (uint) (fileIO.FGetc(fp));
            }
            for (i = 0; i < stackptr; i++)
            {
                stack[i] = (ushort) (fileIO.FGetc(fp) << 8);
                stack[i] |= (ushort) (fileIO.FGetc(fp));
            }
            fileIO.ClearErr(story);
            fileIO.FSeek(story, o = 0x38, FSeekOffset.SEEK_SET);
            i = 0;
            while (o < static_start)
            {
                d = fileIO.FGetc(fp);
                if (d < FileIOConstants.EOF)
                    break;
                if (d != 0)
                {
                    memory[o++] = (byte) (fileIO.FGetc(story) ^ d);
                }
                else
                {
                    c = fileIO.FGetc(fp);
                    if ((c & 0x80) != 0)
                    {
                        c += fileIO.FGetc(fp) << 7;
                    }
                    while (c-- >= 0)
                    {
                        memory[o++] = (byte) fileIO.FGetc(story);
                    }
                }
            }
            fileIO.FClose(fp);
            while (o < static_start)
            {
                memory[o++] = (byte) fileIO.FGetc(story);
            }
            program_counter = frames[frameptr].pc;
        }

        async Task game_save(byte storage)
        {
            int fp;
            int i;
            byte c;
            int o;
            int q;
            if (from_log)
            {
                store(storage, 0);
                return;
            }

            fp = await fileIO.PickFileForWriteAsync(Path.GetFileNameWithoutExtension(story_name) + ".sav", new[] { ".sav" }, 0);
            cur_column = 0;

            if (fp <= 0)
            {
                if (memory[0] < 4)
                {
                    branch(false);
                }
                else
                {
                    store(storage, 0);
                }
                return;
            }
            if (memory[0] < 4)
            {
                branch(true);
            }
            else
            {
                store(storage, 2);
            }
            frames[frameptr].pc = program_counter;
            frames[frameptr + 1].start = (ushort) stackptr;
            fileIO.FPutc(frameptr + 1, fp);
            for (i = 0; i <= frameptr; i++)
            {
                fileIO.FPutc((frames[i + 1].start - frames[i].start) >> 1, fp);
                fileIO.FPutc((int) ((((frames[i + 1].start - frames[i].start) & 1) << 7) | (ushort) ((!frames[i].stored ? 1 : 0) << 6) | (frames[i].pc >> 16)), fp);
                fileIO.FPutc((int) ((frames[i].pc >> 8) & 255), fp);
                fileIO.FPutc((int) (frames[i].pc & 255), fp);
            }
            for (i = 0; i < stackptr; i++)
            {
                fileIO.FPutc(stack[i] >> 8, fp);
                fileIO.FPutc(stack[i] & 255, fp);
            }
            fileIO.ClearErr(story);
            fileIO.FSeek(story, o = 0x38, FSeekOffset.SEEK_SET);
            q = 0;
            while (o < static_start)
            {
                c = (byte) fileIO.FGetc(story);
                if (memory[o] == c)
                {
                    q++;
                }
                else
                {
                    game_save_many(fp, q);
                    q = 0;
                    fileIO.FPutc(memory[o] ^ c, fp);
                }
                o++;
            }
            fileIO.FClose(fp);
            if (memory[0] < 4)
                return;
            fetch(storage);
            store(storage, 1);
        }

        void enter_routine(uint address, bool stored, int argc)
        {
            int c = memory[address];
            int i;
            frames[frameptr].pc = program_counter;
            frames[++frameptr].argc = (byte)argc;
            frames[frameptr].start = (ushort)stackptr;
            frames[frameptr].stored = stored;
            program_counter = address + 1;
            if (zmachineVersion < 5)
            {
                for (i = 0; i < c; i++)
                {
                    stack[stackptr++] = read16((int)program_counter);
                    program_counter += 2;
                }
            }
            else
            {
                for (i = 0; i < c; i++)
                {
                    stack[stackptr++] = 0;
                }
            }
            if (argc > c)
            {
                argc = c;
            }
            for (i = 0; i < argc; i++)
            {
                stack[frames[frameptr].start + i] = inst_args[i + 1];
            }
        }

        void exit_routine(ushort result)
        {
            stackptr = frames[frameptr].start;
            program_counter = frames[--frameptr].pc;
            if (frames[frameptr + 1].stored)
            {
                store(memory[program_counter - 1], result);
            }
        }

        void branch(bool cond)
        {
            int v = memory[program_counter++];
            if ((v & 0x80) == 0)
            {
                cond = !cond;
            }
            if ((v & 0x40) != 0)
            {
                v &= 0x3F;
            }
            else
            {
                v = ((v & 0x3F) << 8) | memory[program_counter++];
            }
            if (cond)
            {
                if (v == 0 || v == 1)
                {
                    exit_routine((ushort) v);
                }
                else
                {
                    program_counter += (uint) ((v & 0x1FFF) - ((v & 0x2000) | 2));
                }
            }
        }

        uint property_address(ushort obj, byte p)
        {
            uint a = (uint)obj_prop_addr(obj);
            byte n = 1;
            a += ((uint)(memory[a] << 1) + 1);
            //if (opts['d'] != 0)
            //{
            //    Console.Error.Write("\n** Finding property {0:D} of object {1:D}.\n", p, obj);
            //}
            while (memory[a] != 0)
            {
                if (zmachineVersion < 4)
                {
                    n = (byte) (memory[a] & 31);
                    cur_prop_size = (byte) ((memory[a] >> 5) + 1);
                }
                else if ((memory[a] & 0x80) != 0)
                {
                    n = (byte) (memory[a] & (zmachineVersion > 8 ? 127 : 63));
                    if ((memory[++a] & 63) == 0)
                        cur_prop_size = 64;
                }
                else
                {
                    n = (byte) (memory[a] & 63);
                    cur_prop_size = (byte) ((memory[a] >> 6) + 1);
                }
                a++;
                //if(n<p) return 0;
                if (n == p)
                {
                    return a;
                }
                a += cur_prop_size;
            }
            return 0;
        }

        void char_print(char zscii)
        {
            char_print((byte)zscii);
        }

        void char_print(byte zscii)
        {
            if (zscii == 0)
                return;
            if (stream3ptr != -1)
            {
                ushort w = read16(stream3addr[stream3ptr]);
                memory[stream3addr[stream3ptr] + 2 + w] = zscii;
                write16(stream3addr[stream3ptr], w + 1);
                return;
            }
            if ((memory[0x11] & 1) != 0 && !window)
            {
                if (transcript > 0)
                {
                    fileIO.FPutc(zscii, transcript);
                }
                else
                {
                    memory[0x10] |= 4;
                }
            }
            var sb = new StringBuilder(text_buffer);
            if (texting && !window)
            {
                if ((zscii & 0x80) != 0)
                {
                    sb[textptr++] = zscii_conv_1[zscii & 0x7F];
                    if (zscii_conv_2[zscii & 0x7F])
                    {
                        sb[textptr++] = zscii_conv_2[zscii & 0x7F];
                    }
                }
                else if ((zscii & 0x6F) != 0)
                {
                    sb[textptr++] = zscii;
                }
                if (zscii <= 32 || textptr > 1000 || !buffering)
                {
                    text_flush();
                }
                if (zscii == 13)
                {
                    runtime.Write('\n');
                    cur_row++;
                    cur_column = 0;
                    while (cur_column < lmargin)
                    {
                        runtime.Write((char)32);
                        cur_column++;
                    }
                }
            }
            text_buffer = sb.ToString();
        }

        void zch_print(int z)
        {
            int zsl;
            if (zch_shift == 3)
            {
                zch_code = z << 5;
                zch_shift = 4;
            }
            else if (zch_shift == 4)
            {
                zch_code |= z;
                char_print(zch_code);
                zch_shift = zch_shiftlock;
            }
            else if (zch_shift >= 5)
            {
                zsl = zch_shiftlock;
                text_print(read16((int) (synonym_table + (z << 1) + ((zch_shift - 5) << 6))) << 1);
                zch_shift = zch_shiftlock = zsl;
            }
            else if (z == 0)
            {
                char_print(32);
                zch_shift = zch_shiftlock;
            }
            else if (z == 1 && zmachineVersion == 1)
            {
                char_print(13);
                zch_shift = zch_shiftlock;
            }
            else if (z == 1)
            {
                zch_shift = 5;
            }
            else if ((z == 4 || z == 5) && (zmachineVersion > 2 && zmachineVersion < 9) && (zch_shift == 1 || zch_shift == 2))
            {
                zch_shift = zch_shiftlock = zch_shift & (z - 3);
            }
            else if (z == 4 && (zmachineVersion < 3 || zmachineVersion > 8))
            {
                zch_shift = zch_shiftlock = (zch_shift + 1) % 3;
            }
            else if (z == 5 && (zmachineVersion < 3 || zmachineVersion > 8))
            {
                zch_shift = zch_shiftlock = (zch_shift + 2) % 3;
            }
            else if ((z == 2 && (zmachineVersion < 3 || zmachineVersion > 8)) || z == 4)
            {
                zch_shift = (zch_shift + 1) % 3;
            }
            else if ((z == 3 && (zmachineVersion < 3 || zmachineVersion > 8)) || z == 5)
            {
                zch_shift = (zch_shift + 2) % 3;
            }
            else if (z == 2)
            {
                zch_shift = 6;
            }
            else if (z == 3)
            {
                zch_shift = 7;
            }
            else if (z == 6 && zch_shift == 2)
            {
                zch_shift = 3;
            }
            else if (z == 7 && zch_shift == 2 && zmachineVersion != 1)
            {
                char_print(13);
                zch_shift = zch_shiftlock;
            }
            else
            {
                if (alphabet_table != 0)
                {
                    char_print(memory[alphabet_table + z + (zch_shift * 26) - 6]);
                }
                else if (zmachineVersion == 1)
                {
                    char_print(v1alpha[z + (zch_shift * 26) - 6]);
                }
                else
                {
                    char_print(v2alpha[z + (zch_shift * 26) - 6]);
                }
                zch_shift = zch_shiftlock;
            }
        }

        uint text_print(uint address)
        {
            ushort t;
            zch_shift = zch_shiftlock = 0;
            for (;;)
            {
                t = read16(address);
                address += 2;
                zch_print((t >> 10) & 31);
                zch_print((t >> 5) & 31);
                zch_print(t & 31);
                if ((t & 0x8000) != 0)
                {
                    return address;
                }
            }
        }

        void insert_object(ushort obj, ushort dest)
        {
            ushort p = parent(obj); 
            ushort s = sibling(obj);
            ushort x;
            if (p != 0)
            {
                //if (opts['d'] != 0)
                //{
                //    Console.Error.Write("\n** Detaching {0:D} from {1:D}.\n", obj, p);
                //}
                // Detach object from parent
                x = child(p);
                if (x == obj)
                {
                    set_child(p, sibling(x));
                }
                else
                {
                    while (sibling(x) != 0)
                    {
                        if (sibling(x) == obj)
                        {
                            set_sibling(x, sibling(sibling(x)));
                            break;
                        }
                        x = sibling(x);
                    }
                }
            }
            if (dest != 0)
            {
                //if (opts['d'] != 0)
                //{
                //    Console.Error.Write("\n** Attaching {0:D} to {1:D}.\n", obj, dest);
                //}
                // Attach object to new parent
                set_sibling(obj, child(dest));
                set_child(dest, obj);
            }
            else
            {
                set_sibling(obj, 0);
            }
            set_parent(obj, dest);
        }

        bool verify_checksum()
        {
            return true;
            //uint size = read16(0x1A);
            //ushort sum = 0;
            //if (verified != 0)
            //{
            //    return 1;
            //}
            //if (zmachineVersion < 4)
            //{
            //    size <<= 1;
            //}
            //else if (zmachineVersion < 6)
            //{
            //    size <<= 2;
            //}
            //else if (zmachineVersion < 10)
            //{
            //    size <<= 3;
            //}
            //else
            //{
            //    size <<= 4;
            //}
            //fileIO.ClearErr(story);
            //if (size != 0)
            //{
            //    size -= 0x40;
            //}
            //fileIO.FSeek(story, 0x40, FSeekOffset.SEEK_SET);
            //while (size-- != 0)
            //{
            //    sum += (ushort)fileIO.FGetc(story);
            //}
            //return (sum == read16(0x1C));
        }

        byte system_input(string[] @out)
        {
            string p;
            uint i;
            input_again:
            GlobalMembersFweep.text_flush();
            cur_row = 2;
            cur_column = 0;
            if (!fgets(text_buffer, 1023, stdin))
            {
                Console.Error.Write("*** Unable to continue.\n");
                Environment.Exit(1);
            }
            p = text_buffer.Substring(text_buffer.Length);
            while (p != text_buffer && p[-1] < 32)
            {
                *--p = 0; // Let's removing "CRLF", etc
            }
            if (escape_code != 0 && *text_buffer == escape_code)
            {
                @out = text_buffer.Substring(2);
                switch (text_buffer[1])
                {
                    case '"':
                        text_buffer[1] = escape_code;
                        @out = text_buffer.Substring(1);
                        break;
                    case '1'... '9':
			return text_buffer[1] + 133 - '1';
                    case ';':
                        if (transcript)
                        {
                            fputc(0, transcript);
                            fputs(text_buffer.Substring(2), transcript);
                            fputc(13, transcript);
                        }
                        goto input_again;
                    case '<':
                        return 131;
                    case '>':
                        return 132;
                    case '?':
                        fputs("Arrow keys: ^v<>    Function keys: 123456789\n" + "Other keys: x (delete) s (space) e (escape) \" (escape_code)\n" + " B0 = breakpoints off   B1 = breakpoints on\n" + " F0 = read keyboard   F1 = read logged input\n" + " L0 = disable input logging   L1 = enable input logging\n" + " S0 = transcript off   S1 = transcript on\n" + " U = instruction usage   U* = save instruction usage to file\n" + " Y = clear instruction usage\n" + " ;* = send comment to transcript\n" + " b = break into debugger\n" + " q = quit\n" + " r* = initialize random number generator\n" + " y = show status line (version 1, 2, 3 only)\n", stderr);
                        goto input_again;
                    case 'B':
                        break_on = text_buffer[2] & 1;
                        goto input_again;
                    case 'F':
                        from_log = text_buffer[2] & 1;
                        goto input_again;
                    case 'L':
                        logging = text_buffer[2] & 1;
                        goto input_again;
                    case 'S':
                        memory[0x11] &= 0xFE;
                        memory[0x11] |= text_buffer[2] & 1;
                        goto input_again;
                    case 'U':
                        if (text_buffer[2])
                        {
                            FILE fp = fopen(text_buffer.Substring(2), "wb");
                            if (fp == null)
                            {
                                goto input_again;
                            }
                            fwrite(instruction_use, 1, 256, fp);
                            fclose(fp);
                        }
                        else
                        {
                            for (i = 0; i < 256; i++)
                            {
                                if ((instruction_use[i] & 0x06) != 0)
                                {
                                    Console.Write(".{0:X2}.\n", i);
                                }
                            }
                        }
                        goto input_again;
                    case 'Y':
                        //C++ TO C# CONVERTER TODO TASK: The memory management function 'memset' has no equivalent in C#:
                        memset(instruction_use, 0, 256);
                        goto input_again;
                    case '^':
                        return 129;
                    case 'b':
                        GlobalMembersFweep.debugger();
                        if (lastdebug != 0)
                        {
                            return 0;
                        }
                        goto input_again;
                    case 'e':
                        return 27;
                    case 'q':
                        Environment.Exit(0);
                        break;
                    case 'r':
                        GlobalMembersFweep.randomize(strtol(text_buffer.Substring(2), 0, 0));
                        goto input_again;
                    case 's':
                        text_buffer[1] = ' ';
                        @out = text_buffer.Substring(1);
                        break;
                    case 'v':
                        return 130;
                    case 'x':
                        return 8;
                    case 'y':
                        if (memory > 3)
                        {
                            Console.Error.Write("*** Status line not available in this Z-machine version.\n");
                        }
                        else
                        {
                            GlobalMembersFweep.text_print((GlobalMembersFweep.read16(memory > 3 ? (object_table + 124 + (GlobalMembersFweep.fetch(16)) * 14) : (object_table + 60 + (GlobalMembersFweep.fetch(16)) * 9)) << address_shift) + 1);
                            GlobalMembersFweep.char_print(13);
                            printf(memory == 3 && ((memory[0x01] & 2) != 0) ? "Time: %02u:%02u\n" : "Score: %d\nTurns: %d\n", (short)GlobalMembersFweep.fetch(17), GlobalMembersFweep.fetch(18));
                        }
                        goto input_again;
                }
            }
            else
            {
                @out = text_buffer;
            }
            return 13;
        }

        async Task<byte> line_input()
        {
            string ptr;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            StringBuilder p;
            int c;
            int cmax;
            byte res;
            //if (from_log != 0 && inlog != null && feof(inlog))
            //{
            //    from_log = 0;
            //}
            //if (from_log != 0)
            //{
            //    p = text_buffer;
            //    for (;;)
            //    {
            //        c = fgetc(inlog);
            //        if (c == 8)
            //        {
            //            if (p > text_buffer)
            //            {
            //                p--;
            //            }
            //        }
            //        else if (c == 0 || c == 13 || c == 27 || (c > 128 && c < 145) || c > 251 || c == EOF)
            //        {
            //            res = c;
            //            if (c == EOF)
            //            {
            //                from_log = 0;
            //            }
            //            break;
            //        }
            //        else if ((c > 31 && c < 127) || (c > 154 && c < 252))
            //        {
            //            *p++ = c;
            //        }
            //    }
            //    *p = 0;
            //}
            //else
            //{
                //if (opts['n'] != 0 && memory < 4 && ((short)GlobalMembersFweep.fetch(17)) != oldscore)
                //{
                //    c = ((short)GlobalMembersFweep.fetch(17)) - oldscore;
                //    Console.Write("\n[The score has been {0}creased by {1:D}.]\n", c >= 0 ? "in" : "de", c >= 0 ? c : -c);
                //    oldscore = (short)GlobalMembersFweep.fetch(17);
                //}
                res = await system_input(ptr);
                if (lastdebug)
                    return await Task.FromResult((byte) 0);
            //}
            if (logging && outlog > 0)
            {
                fileIO.FPrintf(outlog, "{0}", ptr);
                fileIO.FPutc(res, outlog);
            }
            if ((memory[0x11] & 1) != 0)
            {
                if (transcript != 0)
                {
                    fileIO.FPuts(ptr, transcript);
                    fileIO.FPutc(13, transcript);
                }
                else
                {
                    memory[0x10] |= 4;
                }
            }
            var loc = 0;
            if (zmachineVersion < 9)
            {
                p = new StringBuilder(ptr);
                while (p[loc] != (char)0)
                {
                    if (p[loc] >= 'A' && p[loc] <= 'Z')
                    {
                        p[loc] |= (char)0x20;
                    }
                    loc++;
                }
                ptr = p.ToString();
            }

            loc = 0;
            p = new StringBuilder(ptr);
            c = 0;
            cmax = memory[inst_args[0]];
            if (zmachineVersion > 4)
            {
                // "Left over" characters are not implemented.
                while (p[loc] != (char)0 && c < cmax)
                {
                    memory[inst_args[0] + c + 2] = (byte)p[loc++];
                    ++c;
                }

                memory[inst_args[0] + 1] = (byte)c;
                if (inst_args[1] != 0)
                {
                    tokenise(inst_args[0] + 2, 0, inst_args[1], c, 0);
                }
            }
            else
            {
                while (p[loc] != (char)0 && c < cmax)
                {
                    memory[inst_args[0] + c + 1] = (byte)p[loc++];
                    ++c;
                }
                memory[c + 1] = 0;
                tokenise(inst_args[0] + 1, 0, inst_args[1], c, 0);
            }
            return res;
        }
        #endregion

        async Task<ExecutionResult> StartAsyncTask(bool debugMessages)
        {
            execresult = ExecutionResult.ERR_NO_ERRORS;

            var fileExt = fileIO.GetFileNames().Where(i => i.Contains(".")).Select(i => i.Substring(i.LastIndexOf(".")))
                .First(i => KnownExtensions.Contains(i.ToLower()));

            story_name = fileIO.GetFileNames().First(i => i.ToLower().EndsWith(fileExt));
                        
            if (story_name.Length == 0)
            {
                help();
                return await Task.FromResult(ExecutionResult.ERR_BADFILE);
            }
            execresult = await game_begin();
            if (execresult != ExecutionResult.ERR_NO_ERRORS)
                return execresult;

            execresult = await game_restart();
            if (execresult != ExecutionResult.ERR_NO_ERRORS)
                return execresult;

            if (debugMessages)
            {
                debugger();
            }
            for (;;)
            {
                execresult = await execute_instruction();
                if (execresult != ExecutionResult.ERR_NO_ERRORS)
                    break;
            }

            return await Task.FromResult(execresult);
        }

        void help()
        {
            //runtime.WriteLine("Fweep -- a Z-machine interpreter for versions 1 to 10 except 6.\nVersion {0}.\nThis program comes with ABSOLUTELY NO WARRANTY; for details see the\nCOPYING file. This is free software, and you are welcome to\nredistribute it under certain conditions; see the COPYING file for\ndetails.\n\n" 
            //+ "usage: fweep [options] story\n" 
            //+ "\n" 
            //+ " -a *  = Set auxiliary file.\n" 
            //+ " -b    = Break into debugger.\n" 
            //+ " -d    = Object and parser debugging.\n" 
            //+ " -e *  = Escape code.\n" 
            //+ " -g *  = Set screen geometry by rows,columns.\n" 
            //+ " -i *  = Set command log file for input.\n" 
            //+ " -n    = Enable score notification.\n" 
            //+ " -o *  = Set command log file for output.\n" 
            //+ " -p    = Assume game disc is not original.\n" 
            //+ " -q    = Convert question marks to spaces before lexical analysis.\n" 
            //+ " -r *  = Restore save game.\n" 
            //+ " -s *  = Set transcript file.\n" 
            //+ " -t    = Select Tandy mode.\n" 
            //+ " -u    = Disable undo.\n" 
            //+ " -v    = Assume the checksum is correct without checking.\n");
        }

        async Task<ExecutionResult> game_begin()
        {
            var done = false;
            await Task.Run(() =>
            {
                int i;
                if (story == 0)
                {
                    story = fileIO.FOpen(story_name, "rb");
                }
                if (story == 0 || story < 1)
                {
                    runtime.WriteLine("\n*** Unable to load story file: {0}", story_name);
                    execresult = ExecutionResult.ERR_BADFILE;
                    done = true;
                }
                if(!done)
                { 
                    //if (opts['a'] != 0 && auxfile == null)
                    //{
                    //    auxfile = fopen(opts['a'], "r+b");
                    //    if (auxfile == null)
                    //    {
                    //        auxfile = fopen(opts['a'], "w+b");
                    //        for (i = 0; i < 416; i++)
                    //        {
                    //            fputc(32, auxfile);
                    //        }
                    //        fclose(auxfile);
                    //        auxfile = fopen(opts['a'], "r+b");
                    //    }
                    //    if (auxfile == null)
                    //    {
                    //        Console.Error.Write("\n*** Unable to create auxiliary file: {0}\n", opts['a']);
                    //        Environment.Exit(1);
                    //    }
                    //}
                    //if (opts['s'] != 0 && transcript == null)
                    //{
                    //    transcript = fopen(opts['s'], "wb");
                    //}
                    //if (opts['i'] != 0 && inlog == null)
                    //{
                    //    inlog = fopen(opts['i'], "rb");
                    //}
                    //if (opts['o'] != 0 && outlog == null)
                    //{
                    //    outlog = fopen(opts['o'], "wb");
                    //}
                    fileIO.Rewind(story);
                    fileIO.FRead(memory, 64, 1, story);
                    memoryIndex = 0;
                    zmachineVersion = memory[memoryIndex];
                    setupMacros();

                    switch (zmachineVersion)
                    {
                        case 1:
                            packed_shift = 1;
                            memory[0x01] = 0x10;
                            break;
                        case 2:
                            packed_shift = 1;
                            memory[0x01] = 0x10;
                            break;
                        case 3:
                            packed_shift = 1;
                            memory[0x01] &= 0x8F;
                            memory[0x01] |= 0x10;
                            //if (opts['t'] != 0)
                            //{
                            //    memory[0x01] |= 0x08;
                            //}
                            break;
                        case 4:
                            packed_shift = 2;
                            memory[0x01] = 0x00;
                            break;
                        case 5:
                            packed_shift = 2;
                            alphabet_table = read16(0x34);
                            break;
                        case 7:
                            packed_shift = 2;
                            routine_start = (uint) (read16(0x28) << 3);
                            text_start = (uint) (read16(0x2A) << 3);
                            alphabet_table = read16(0x34);
                            break;
                        case 8:
                            packed_shift = 3;
                            alphabet_table = read16(0x34);
                            break;
                        case 9:
                            packed_shift = 3;
                            address_shift = 1;
                            routine_start = (uint) (read16(0x28) << 3);
                            text_start = (uint) (read16(0x2A) << 3);
                            alphabet_table = (uint) (read16(0x34) << 1);
                            break;
                        case 10:
                            packed_shift = 4;
                            address_shift = 2;
                            routine_start = (uint) (read16(0x28) << 4);
                            text_start = (uint) (read16(0x2A) << 4);
                            alphabet_table = (uint) (read16(0x34) << 2);
                            break;
                        default:
                            unsupported:
                            runtime.Debug.LogError("\n*** Unsupported Z-machine version: {0:D}\n", memory[0]);
                            done = true;
                            break;
                    }
                    if (!done)
                    {
                        restart_address = (uint)(read16(0x06) << address_shift);
                        dictionary_table = (uint) (read16(0x08) << address_shift);
                        object_table = (uint) (read16(0x0A) << address_shift);
                        global_table = (uint) (read16(0x0C) << address_shift);
                        static_start = (uint) (read16(0x0E) << address_shift);
                        memory[0x11] &= 0x53;
                        if (zmachineVersion > 1)
                        {
                            synonym_table = (uint) (read16(0x18) << address_shift);
                        }
                        if (zmachineVersion > 3)
                        {
                            memory[0x1E] = 1; // opts['t'] != 0 ? 11 : 1;
                            memory[0x20] = (byte) sc_rows;
                            memory[0x21] = (byte) sc_columns;
                        }
                        if (zmachineVersion > 4)
                        {
                            memory[0x01] = 0x10;
                            memory[0x23] = (byte) sc_columns;
                            memory[0x25] = (byte) sc_rows;
                            memory[0x26] = 1;
                            memory[0x27] = 1;
                            memory[0x2C] = 2;
                            memory[0x2D] = 9;
                        }
                        if ((memory[memoryIndex + 2] & 128) == 0)
                        {
                            write16(0x02, (ushort) (auxfile > 0 ? 0x0A02 : 0x0802));
                        }
                        //if (opts['b'] != 0)
                        //{
                        //    break_on = 1;
                        //}
                        //if (opts['e'] != 0)
                        //{
                        //    if (opts['e'][0] >= '0' && opts['e'][0] <= '9')
                        //    {
                        //        escape_code = strtol(opts['e'], 0, 0);
                        //    }
                        //    else
                        //    {
                        //        escape_code = opts['e'][0];
                        //    }
                        //}
                        //if (opts['g'] != 0)
                        //{
                        //    string p = opts['g'];
                        //    sc_rows = sc_columns = 0;
                        //}
                        //if (opts['p'] != 0)
                        //{
                        //    original = 0;
                        //}
                        //if (opts['u'] != 0)
                        //{
                        //    allow_undo = 0;
                        //    memory[0x11] &= 0x43;
                        //}
                        //if (opts['v'] != 0)
                        //{
                        //    verified = 1;
                        //}
                        cur_row = 2;
                        cur_column = 0;
                        randomize(0);
                        runtime.Write('\n');
                    }
                }
            });

            return execresult;
        }

        async Task<ExecutionResult> game_restart()
        {
            execresult = ExecutionResult.ERR_NO_ERRORS;
            await Task.Run(() =>
            {
                int addr = 64;
                stackptr = frameptr = 0;
                program_counter = restart_address;
                fileIO.ClearErr(story);
                fileIO.FSeek(story, 64, FSeekOffset.SEEK_SET);
                var buffer = new byte[1024];
                while (!fileIO.FEof(story))
                {
                    var readBytes = fileIO.FRead(buffer, 1024, 1, story);
                    Array.Copy((byte[])readBytes.Data, 0, memory, addr, readBytes.Length);
                    if (readBytes.Length == 0)
                        break;
                    addr += 1024;
                }
            });
            return execresult;
        }
        ulong y;
        async Task<ExecutionResult> execute_instruction()
        {
            execresult = ExecutionResult.ERR_NO_ERRORS;

            byte currentInstruction = memory[program_counter++];
            ushort at;
            short m;
            short n;
            uint u = program_counter - 1;

            var nbuf = new byte[5];
            int argc;
            instruction_use[currentInstruction] |= 0x01;
            if ((currentInstruction & 0x80) != 0)
	        {
                if (currentInstruction >= 0xC0 || currentInstruction == 0xBE)
		        {
                    // variable
                    if (currentInstruction == 0xBE)
		            {
                        currentInstruction = memory[program_counter++];
                    }
                    at = (ushort)(memory[program_counter++] << 8);
                    if (currentInstruction == 0xEC || currentInstruction == 0xFA)
		            {
                        at |= memory[program_counter++];
                    }
		            else
		            {
                        at |= 0x00FF;
                    }
                    if ((at & 0xC000) == 0xC000)
                    {
                        argc = 0;
                    }
                    else if ((at & 0x3000) == 0x3000)
                    {
                        argc = 1;
                    }
                    else if ((at & 0x0C00) == 0x0C00)
                    {
                        argc = 2;
                    }
                    else if ((at & 0x0300) == 0x0300)
                    {
                        argc = 3;
                    }
                    else if ((at & 0x00C0) == 0x00C0)
                    {
                        argc = 4;
                    }
                    else if ((at & 0x0030) == 0x0030)
                    {
                        argc = 5;
                    }
                    else if ((at & 0x000C) == 0x000C)
                    {
                        argc = 6;
                    }
                    else if ((at & 0x0003) == 0x0003)
                    {
                        argc = 7;
                    }
                    else
                    {
                        argc = 8;
                    }
                }
		        else
		        {
                    // short
                    at = (ushort)((currentInstruction << 10) | 0x3FFF);
                    argc = (currentInstruction < 0xB0) ? 1 : 0;
                    if (argc != 0)
                    {
                        currentInstruction &= 0x8F;
                    }
                }
            }
	        else
	        {
                // long
                at = 0x5FFF;
                if ((currentInstruction & 0x20) != 0)
		        {
                    at ^= 0x3000;
                }
                if ((currentInstruction & 0x40) != 0)
		        {
                    at ^= 0xC000;
                }
                currentInstruction &= 0x1F;
                currentInstruction |= 0xC0;
                argc = 2;
            }
            if (break_on != 0)
            {
                if (continuing == u || continuing == program_counter)
                {
                    continuing = 0;
                }
                else
                {
                    if (instruction_bkpt[currentInstruction] != 0)
                    {
                        program_counter = u;
                        await debugger();
                        return execresult;
                    }
                    for (n = 0; n < 16; n++)
                    {
                        if (address_bkpt[n] == u)
                        {
                            program_counter = u;
                            await debugger();
                            return execresult;
                        }
                    }
                }
            }
            for (n = 0; n < 8; n++)
            {
                switch ((at >> (14 - n * 2)) & 3)
                {
                    case 0: // large
                        inst_args[n] = (ushort)(memory[program_counter++] << 8);
                        inst_args[n] |= memory[program_counter++];
                        break;
                    case 1: // small
                        inst_args[n] = memory[program_counter++];
                        break;
                    case 2: // variable
                        inst_args[n] = fetch(memory[program_counter++]);
                        break;
                    case 3: // omit
                        inst_args[n] = 0;
                        break;
                }
            }
            instruction_use[currentInstruction] |= (byte)(argc != 0 ? 0x04 : 0x02);
            lastdebug = 0;
            switch (currentInstruction)
	        {
		        case 0x00: // Save game or auxiliary file
		            if (argc != 0)
                    {
                        storei(aux_save());
                    }
                    else
                    {
                        await game_save(memory[program_counter++]);
                    }
                    break;
		        case 0x01: // Restore game or auxiliary file
		            storei(argc != 0 && auxfile != null ? aux_restore() : 0);
                    if (argc == 0)
                    {
                        await game_restore();
                    }
                    break;
		        case 0x02: // Logical shift
		        if ((short) inst_args[1] > 0)
                {
                    storei(inst_args[0] << inst_args[1]);
                }
                else
                {
                    storei(inst_args[0] >> -inst_args[1]);
                }
                break;
		        case 0x03: // Arithmetic shift
		            if ((short) inst_args[1] > 0)
                    {
                        storei((short) inst_args[0] << (short) inst_args[1]);
                    }
                    else
                    {
                        storei((short) inst_args[0] >> -(short) inst_args[1]);
                    }
                    break;
        		case 0x04: // Set font
		            await text_flush();
                    storei((inst_args[inst_args_index] == 1 || inst_args[inst_args_index] == 4) ? 4 : 0);
                    //if (opts['t'] == 0)
                    //{
                    //    Console.Write(inst_args == 3 ? 14 : 15);
                    //}
                    break;
    		    case 0x08: // Set margins
		            if (!window )
                    {
                        lmargin = inst_args[0];
                        rmargin = inst_args[1];
                        if (zmachineVersion == 5)
                        {
                            write16(40, inst_args[0]);
                        }
                        if (zmachineVersion == 5)
                        {
                            write16(41, inst_args[1]);
                        }
                        while (cur_column < (int)inst_args[0])
                        {
                             runtime.Write(' ');
                            //Console.Write(32);
                            cur_column++;
                        }
                    }
                    break;
		        case 0x09: // Save undo buffer
		            if (allow_undo != 0)
                    {
                        //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
                        //memcpy(u_frames, frames, sizeof(StackFrame));
                        Array.Copy(u_frames, frames, 1);
                        //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
                        //memcpy(u_stack, stack, sizeof(ushort));
                        Array.Copy(u_stack, stack, 1);
                        //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
                        //memcpy(undomem, memory + 0x40, static_start - 0x40);
                        Array.Copy(undomem, 0, memory, 0x40, (int)static_start - 0x40);
                        u_frameptr = frameptr;
                        u_stackptr = stackptr;
                        u_program_counter = program_counter;
                        storei(1);
                    }
                    else
                    {
                        storei(-1);
                    }
                    break;
		            case 0x0A: // Restore undo buffer
		                if (allow_undo != 0)
                        {
                            //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
                            //memcpy(frames, u_frames, sizeof(StackFrame));
                            Array.Copy(frames, u_frames, 1);

                            //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
                            //memcpy(stack, u_stack, sizeof(ushort));
                            Array.Copy(stack, u_stack, 1);

                            //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
                            //memcpy(memory + 0x40, undomem, static_start - 0x40);
                            Array.Copy(memory, 0x40, undomem, 0, (int) static_start - 0x40);
                            frameptr = u_frameptr;
                            stackptr = u_stackptr;
                            program_counter = u_program_counter;
                            storei((argc != 0 && zmachineVersion > 8) ? inst_args[0] : 2);
                        }
                        else
                        {
                            storei(-1);
                        }
                        break;
		            case 0x0B: // Call byte address
		                program_counter++;
                        enter_routine(inst_args[inst_args_index], true, argc - 1);
                        break;
		            case 0x0C: // Get reference to stack or local variables
		                if (inst_args != null)
                        {
                            storei(stackptr - 1);
                        }
                        else
                        {
                            storei(frames[(int)frameptr].start + inst_args[inst_args_index - 1]);
                        }
                        break;
		            case 0x0D: // Read through stack/locals reference
		                storei(stack[inst_args[inst_args_index]]);
                        break;
		            case 0x0E: // Write through stack/locals reference
		                if (inst_args[inst_args_index] < 1024)
                        {
                            stack[inst_args[inst_args_index]] = inst_args[1];
                        }
                        break;
		            case 0x0F: // Read byte from long property
		                u = property_address(inst_args[0], (byte)inst_args[1]);
                        storei(memory[u]);
                        break;
		            case 0x1D: // Read word from long property
		                u = property_address(inst_args[0], (byte) inst_args[1]);
                        storei(read16((int)u));
                        break;
		            case 0x80: // Jump if zero
		                branch(inst_args[inst_args_index] == 0 );
                        break;
		            case 0x81: // Sibling
		                storei(sibling(inst_args[inst_args_index]));
                        branch(sibling(inst_args[inst_args_index]) != 0);
                    break;
		            case 0x82: // Child
                        storei(child(inst_args[inst_args_index]));
                        branch(child(inst_args[inst_args_index]) != 0);
                        break;
		            case 0x83: // Parent
                        storei(parent(inst_args[inst_args_index]));
                        break;
		            case 0x84: // Property length
                        currentInstruction = memory[inst_args[inst_args_index] - 1];
                        storei(zmachineVersion < 4 ? 
                            (currentInstruction >> 5) + 1:
                            ( (currentInstruction & 0x80) != 0 ? 
                                ( (currentInstruction & 63) == 0 ? 
                                    64 :
                                    ((currentInstruction >> 6) +1)
                                ) : ((currentInstruction >> 6) + 1)
                            )
                        );
                        break;
		            case 0x85: // Increment
		                store((byte) inst_args[inst_args_index], (byte)(fetch((byte) inst_args[inst_args_index]) + 1));
                        break;
		            case 0x86: // Decrement
		                store((byte) inst_args[inst_args_index], (byte) fetch((byte) inst_args[inst_args_index]) - 1);
                        break;
		            case 0x87: // Print by byte address
		                text_print(inst_args[inst_args_index]);
                        break;
		            case 0x88: // Call routine
		                if (inst_args[inst_args_index] != 0)
                        {
                            program_counter++;
                            enter_routine((uint)((inst_args[inst_args_index] << packed_shift) + routine_start), true, argc - 1);
                        }
                        else
                        {
                            storei(0);
                        }
                        break;
		            case 0x89: // Remove object
		                insert_object(inst_args[inst_args_index], 0);
                        break;
		            case 0x8A: // Print short name of object
		                text_print((uint)obj_prop_addr((uint)(inst_args[inst_args_index] + 1)));
                        break;
		            case 0x8B: // Return
		                exit_routine(inst_args[inst_args_index]);
                        break;
		            case 0x8C: // Unconditional jump
		                program_counter += (uint)((short) inst_args[inst_args_index] - 2);
                        break;
		            case 0x8D: // Print by packed address
		                text_print((uint)((inst_args[inst_args_index] << packed_shift) + text_start));
                        break;
		            case 0x8E: // Load variable
		                at = fetch((byte) inst_args[inst_args_index]);
                        store((byte) inst_args[inst_args_index], at); // if it popped from the stack, please put it back on
                        storei(at);
                        break;
		            case 0x8F: // Not // Call routine and discard result
		                if (zmachineVersion > 4)
                        {
                            if (inst_args[inst_args_index] != 0)
                            {
                                enter_routine((uint) ((inst_args[inst_args_index] << packed_shift) + routine_start), false, argc - 1);
                            }
                        }
                        else
                        {
                            storei(~inst_args[inst_args_index]);
                        }
                        break;
		            case 0xB0: // Return 1
		                exit_routine(1);
                        break;
		            case 0xB1: // Return 0
		                exit_routine(0);
                        break;
		            case 0xB2: // Print literal
		                program_counter = text_print(program_counter);
                        break;
		            case 0xB3: // Print literal and return
		                program_counter = text_print(program_counter);
                        char_print(13);
                        exit_routine(1);
                        break;
		            case 0xB4: // No operation
		                //NOP
		                break;
		            case 0xB5: // Save
		                if (zmachineVersion > 3)
                        {
                            await game_save(memory[program_counter++]);
                        }
                        else
                        {
                            await game_save(0);
                        }
                        break;
		        case 0xB6: // Restore
		            if (zmachineVersion > 3)
                    {
                        storei(0);
                    }
                    else
                    {
                        branch(false);
                    }
                    await game_restore();
                    break;
        		case 0xB7: // Restart
		            game_restart();
                    break;
		        case 0xB8: // Return from stack
		            exit_routine((ushort) stack[stackptr - 1]);
                    break;
		        case 0xB9: // Discard from stack // Catch
		            if (zmachineVersion > 4)
                    {
                        storei(frameptr);
                    }
                    else
                    {
                        stackptr--;
                    }
                    break;
		        case 0xBA: // Quit
		            await text_flush();
                    return await Task.FromResult(ExecutionResult.ERR_STATE_QUIT);
                    break;
		        case 0xBB: // Line break
		            char_print(13);
                    break;
		        case 0xBC: // Show status
		            //NOP
		            break;
		        case 0xBD: // Verify checksum
		            branch(verify_checksum());
                    break;
		        case 0xBF: // Check if game disc is original
		            branch(original);
                    break;
		        case 0xC1: // Branch if equal
		            for (n = 1; n < argc; n++)
                    {
                        if (inst_args[inst_args_index] == inst_args[n])
                        {
                            branch(true);
                            break;
                        }
                    }
                    if (n == argc)
                    {
                        branch(false);
                    }
                    break;
		        case 0xC2: // Jump if less
		            branch((short) inst_args[0] < (short) inst_args[1]);
                    break;
		        case 0xC3: // Jump if greater
		            branch((short) inst_args[0] > (short) inst_args[1]);
                    break;
		        case 0xC4: // Decrement and branch if less
		            store((byte)inst_args[inst_args_index], n = (short)(fetch((byte)inst_args[inst_args_index]) - 1));
                    branch(n < (short) inst_args[1]);
                    break;
		        case 0xC5: // Increment and branch if greater
		            store((byte)inst_args[inst_args_index], n = (short)(fetch((byte) inst_args[inst_args_index]) + 1));
                    branch(n > (short) inst_args[1]);
                    break;
		        case 0xC6: // Check if one object is the parent of the other
		            branch(parent(inst_args[0]) == inst_args[1]);
                    break;
		        case 0xC7: // Test bitmap
		            branch((inst_args[0] & inst_args[1]) == inst_args[1]);
                    break;
		        case 0xC8: // Bitwise OR
		            storei(inst_args[0] | inst_args[1]);
                    break;
		        case 0xC9: // Bitwise AND
		            storei(inst_args[0] & inst_args[1]);
                    break;
		        case 0xCA: // Test attributes
		            branch( (memory[attribute(inst_args[inst_args_index]) + (inst_args[1] >> 3)] & (0x80 >> (inst_args[1] & 7))) != 0);
                    break;
		        case 0xCB: // Set attribute
                    memory[(int)(attribute(inst_args[inst_args_index]) + (inst_args[1] >> 3))] |= (byte)(0x80 >> (inst_args[1] & 7));
                    break;
        		case 0xCC: // Clear attribute
                    memory[(int)(attribute(inst_args[inst_args_index]) + (inst_args[1] >> 3))] &= (byte)(~(0x80 >> (inst_args[1] & 7)));
                    break;
    		    case 0xCD: // Store to variable
		            fetch((byte) inst_args[0]);
                    store((byte) inst_args[0], inst_args[1]);
                    break;
		        case 0xCE: // Insert object
		            insert_object(inst_args[0], inst_args[1]);
                    break;
		        case 0xCF: // Read 16-bit number from RAM/ROM
		            storei(read16(inst_args[0] + ((short) inst_args[1] << 1)));
                    break;
		        case 0xD0: // Read 8-bit number from RAM/ROM
		            storei(memory[inst_args[0] + (short) inst_args[1]]);
                    break;
		        case 0xD1: // Read property
		            if ((u = (uint) property_address(inst_args[0], (byte) inst_args[1])) != 0)
                    {
                        storei(cur_prop_size == 1 ? memory[u] : read16(u));
                    }
                    else
                    {
                        storei(read16((int)(object_table + (inst_args[1] << 1) - 2)));
                    }
                    break;
		        case 0xD2: // Get address of property
		            storei((int) property_address(inst_args[0], (byte) inst_args[1]));
                    break;
		        case 0xD3: // Find next property
		            if (inst_args[1] != 0)
                    {
                        u = property_address(inst_args[0], (byte) inst_args[1]);
                        u += cur_prop_size;
                        storei(memory[u] & (zmachineVersion > 8 && ((memory[u] & 128) != 0) ? 127 : zmachineVersion > 3 ? 63 : 31));
                    }
                    else
                    {
                        u = (uint)obj_prop_addr((uint)inst_args[0]);
                        u += (uint) ((memory[(int)u] << 1) + 1);
                        storei(memory[u] & (zmachineVersion > 8 && ((memory[u] & 128) != 0) ? 127 : zmachineVersion > 3 ? 63 : 31));
                    }
                    break;
		        case 0xD4: // Addition
		            storei((short) inst_args[0] + (short) inst_args[1]);
                    break;
		        case 0xD5: // Subtraction
		            storei((short) inst_args[0] - (short) inst_args[1]);
                    break;
		        case 0xD6: // Multiplication
		            storei((short) inst_args[0] * (short) inst_args[1]);
                    break;
		        case 0xD7: // Division
		            if (inst_args[1] != 0)
                    {
                        n = (short)((short) inst_args[0] / (short) inst_args[1]);
                    }
                    else
                    {
                        runtime.Debug.LogError("\n*** Division by zero\n{0}\n", currentInstruction);
                    }
                    storei(n);
                    break;
		        case 0xD8: // Modulo
		            if (inst_args[1] != 0)
                    {
                        n = (short) ((short) inst_args[0] % (short) inst_args[1]);
                    }
                    else
                    {
                        runtime.Debug.LogError("\n*** Division by zero\n{0}\n", currentInstruction);
                    }
                    storei(n);
                    break;
		        case 0xD9: // Call routine
		            if (inst_args[inst_args_index] != 0)
                    {
                        program_counter++;
                        enter_routine((uint)(inst_args[inst_args_index] << packed_shift) + routine_start, true, argc - 1);
                    }
                    else
                    {
                        storei(0);
                    }
                    break;
		        case 0xDA: // Call routine and discard result
		            if (inst_args[inst_args_index] != 0)
                    {
                        enter_routine((uint)(inst_args[inst_args_index] << packed_shift) + routine_start, false, argc - 1);
                    }
                    break;
		        case 0xDB: // Set colors
		            //NOP
		            break;
		        case 0xDC: // Throw
		            frameptr = inst_args[1];
                    exit_routine(inst_args[inst_args_index]);
                    break;
		        case 0xDD: // Bitwise XOR
		            storei(inst_args[0] ^ inst_args[1]);
                    break;
		        case 0xE0: // Call routine // Read from extended RAM
		            if (zmachineVersion > 8)
                    {
                        u = (uint)((inst_args[0] << address_shift) + (inst_args[1] << 1) + inst_args[2)];
                        storei(read16((int)u));
                    }
                    else if (inst_args[inst_args_index] != 0)
                    {
                        program_counter++;
                        enter_routine((uint)(inst_args[inst_args_index] << packed_shift) + routine_start, true, argc - 1);
                    }
                    else
                    {
                        storei(0);
                    }
                    break;
		        case 0xE1: // Write 16-bit number to RAM
		            write16(inst_args[0] + ((short) inst_args[1] << 1), inst_args[2]);
                    break;
		        case 0xE2: // Write 8-bit number to RAM
		            memory[inst_args[0] + (short) inst_args[1]] = (byte) inst_args[2];
                    break;
		        case 0xE3: // Write property
		            u = property_address(inst_args[0], (byte) inst_args[1]);
                    if (cur_prop_size == 1)
                    {
                        memory[u] = (byte) inst_args[2];
                    }
                    else
                    {
                        write16((int)u, inst_args[2]);
                    }
                    break;
		        case 0xE4: // Read line of input
		            n = line_input();
                    if (zmachineVersion > 4 && lastdebug == 0)
                    {
                        storei(n);
                    }
                    break;
		        case 0xE5: // Print character
		            char_print((byte)inst_args[inst_args_index]);
                    break;
		        case 0xE6: // Print number
		            n = (short) inst_args[inst_args_index];
                    if (n == -32768)
                    {
                        char_print('-');
                        char_print('3');
                        char_print('2');
                        char_print('7');
                        char_print('6');
                        char_print('8');
                    }
                    else
                    {
                        nbuf[0] = nbuf[1] = nbuf[2] = nbuf[3] = nbuf[4] = 0;
                        if (n < 0)
                        {
                            char_print('-');
                            n *= -1;
                        }
                        nbuf[4] = (byte)( (n % 10) | '0');
                        if ((n /= 10) != 0)
                        {
                            nbuf[3] = (byte) ((n % 10) | '0');
                        }
                        if ((n /= 10) != 0)
                        {
                            nbuf[2] = (byte) ((n % 10) | '0');
                        }
                        if ((n /= 10) != 0)
                        {
                            nbuf[1] = (byte) ((n % 10) | '0');
                        }
                        if ((n /= 10) != 0)
                        {
                            nbuf[0] = (byte) ((n % 10) | '0');
                        }
                        char_print(nbuf[0]);
                        char_print(nbuf[1]);
                        char_print(nbuf[2]);
                        char_print(nbuf[3]);
                        char_print(nbuf[4]);
                    }
                    break;
		        case 0xE7: // Random number generator
                    if ((short) inst_args[inst_args_index] > 0)
                    {
                        storei(get_random((short) inst_args[inst_args_index]));
                    }
                    else
                    {
                        randomize((ushort)(-(short) inst_args[inst_args_index]));
                        storei(0);
                    }
                    break;
		        case 0xE8: // Push to stack
		            stack[stackptr++] = inst_args[inst_args_index];
                    break;
		        case 0xE9: // Pop from stack
		            if (inst_args[inst_args_index] != 0)
                    {
                        store((byte) inst_args[inst_args_index], stack[--stackptr]);
                    }
                    else
                    {
                        stack[stackptr - 2] = stack[stackptr - 1],stackptr--;
                    }
                    break;
		        case 0xEA: // Split window
		            //NOP
		            break;
		        case 0xEB: // Set active window
		            window = inst_args[inst_args_index] != 0;
                    break;
		        case 0xEC: // Call routine
		            if (inst_args[inst_args_index] != 0)
                    {
                        program_counter++;
                        enter_routine((uint)(inst_args[inst_args_index] << packed_shift) + routine_start, true, argc - 1);
                    }
                    else
                    {
                        storei(0);
                    }
                    break;
		        case 0xED: // Clear window
		            if (inst_args[inst_args_index] != 1)
                    {
                        runtime.Write('\n');
                        textptr = 0;
                        cur_row = 2;
                        cur_column = 0;
                        while (cur_column < lmargin)
                        {
                            runtime.Write((char)32);
                            cur_column++;
                        }
                    }
                    break;
		        case 0xEE: // Erase line
		            //NOP
		            break;
		        case 0xEF: // Set cursor position
		            //NOP
		            break;
		        case 0xF0: // Get cursor position
		            if (window)
                    {
                        memory[inst_args[inst_args_index]] = (byte)sc_rows;
                        memory[inst_args[inst_args_index + 1]] = (byte) (cur_column + 1);
                    }
                    else
                    {
                        memory[inst_args[inst_args_index]] = 0;
                        memory[inst_args[inst_args_index + 1]] = 0;
                    }
                    break;
		        case 0xF1: // Set text style
		            //NOP
		            break;
		        case 0xF2: // Buffer mode
		            buffering = inst_args[inst_args_index] != 0;
                    break;
		        case 0xF3: // Select output stream
		            switch_output((short) inst_args[inst_args_index]);
                    break;
		        case 0xF4: // Select input stream
		            if (inlog)
                    {
                        from_log = inst_args[inst_args_index] != 0;
                    }
                    break;
		        case 0xF5: // Sound effects
                    runtime.Write((char)7); //Console.Write(7);
                    break;
		        case 0xF6: // Read a single character
		            n = char_input();
                    if (lastdebug == 0)
                    {
                        storei(n);
                    }
                    break;
		        case 0xF7: // Scan a table
		            if (argc < 4)
                    {
                        inst_args[3] = 0x82;
                    }
                    u = inst_args[1];
                    while (inst_args[2] != 0)
                    {
                        if (inst_args == ((inst_args[3] & 0x80) != 0 ? read16(u) : memory[u]))
                            break;
                        u += (uint)inst_args[3] & 0x7F;
                        inst_args[2]--;
                    }
                    storei((int) (inst_args[2] != 0 ? u : 0));
                    branch(inst_args[2] != 0);
                    break;
		        case 0xF8: // Not
		            storei(~inst_args[inst_args_index]);
                    break;
		        case 0xF9: // Call routine and discard results // Write extended RAM
		            if (zmachineVersion > 8)
                    {
                        u = ((uint)inst_args[0] << address_shift) + ((uint) inst_args[1] << 1) + (uint)inst_args[2];
                        write16(u, inst_args[3]);
                    }
                    else if (inst_args[inst_args_index] != 0)
                    {
                        enter_routine((uint) ((inst_args[inst_args_index] << packed_shift) + routine_start), false, argc - 1);
                    }
                    break;
		        case 0xFA: // Call routine and discard results
		            if (inst_args[inst_args_index] != 0)
                    {
                        enter_routine((uint) ((inst_args[inst_args_index] << packed_shift) + routine_start), false, argc - 1);
                    }
                    break;
		        case 0xFB: // Tokenise text
		            if (argc < 4)
                    {
                        inst_args[3] = 0;
                    }
                    if (argc < 3)
                    {
                        inst_args[2] = 0;
                    }
                    tokenise(inst_args[0] + 2, inst_args[2], inst_args[1], memory[inst_args[0] + 1], inst_args[3]);
                    break;
		        case 0xFC: // Encode text in dictionary format
		            ulong execute_instruction_y = dictionary_encode(zmachineVersion + inst_args[0] + inst_args[2], inst_args[1]);
                    write16(inst_args[3], (ushort)(execute_instruction_y >> 16));
                    write16(inst_args[3] + 2, (ushort) (execute_instruction_y >> 8));
                    write16(inst_args[3] + 4, (ushort) execute_instruction_y);
                    break;
		        case 0xFD: // Copy a table
		            if (inst_args[1] == 0)
                    {
                        // zero!
                        while (inst_args[2] != 0)
                        {
                            memory[inst_args[0] + --inst_args[2]] = 0;
                        }
                    }
                    else if ((short) inst_args[2] > 0 && inst_args[1] > inst_args[0])
                    {
                        // backward!
                        m = (short) inst_args[2];
                        while (m-- != 0)
                        {
                            memory[inst_args[1] + m] = memory[inst_args[0] + m];
                        }
                    }
                    else
                    {
                        // forward!
                        if ((short) inst_args[2] < 0)
                        {
                            inst_args[2] = (ushort) (inst_args[2] * -1);
                        }
                        m = 0;
                        while (m < (short) inst_args[2])
                        {
                            memory[inst_args[1] + m] = memory[inst_args[0] + m],m++;
                        }
                    }
                    break;
		        case 0xFE: // Print a rectangle of text
		            make_rectangle(inst_args[0], inst_args[1], argc > 2 ? inst_args[2] : 1, argc > 3 ? (short) inst_args[3] : 0);
                    // (I assume the skip is signed, since many other things are, and +32768 isn't useful anyways.)
                    break;
		        case 0xFF: // Check argument count
		            branch(frames[frameptr].argc >= (byte) inst_args[inst_args_index]);
                    break;
                default:
                    runtime.Debug.LogError("\n*** Invalid instruction: {0:X2} (near {1:X6})\n", currentInstruction, program_counter);
                    return await Task.FromResult(ExecutionResult.ERR_UNKNOWN);
                    break;
            }
            return await Task.FromResult(result);
        }

        Task debugger()
        {
            return Task.FromResult(0);
        }
    }
}
