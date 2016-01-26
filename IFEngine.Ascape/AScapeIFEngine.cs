using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFInterfaces;
using IFInterfaces.Services;
using IFInterfaces.Support;
using Windows.Foundation;
using IFInterfaces.Helpers;

namespace IFEngine.Ascape
{
    public sealed class AScapeIFEngine : IIFGameEngine
    {
        #region data structures
        class objecttype
        {

            internal string name = "";
            internal string noun = "";
            internal string description = "";
            internal int location;
            internal int score;
            internal int gettable;
            internal int wearable;
            internal int carried;
            internal int worn;
        }

        class puzzletype
        {
            internal int[] condition = new int[10];
            internal int[] action = new int[10];
        }

        class blockedexittype
        {
            internal int room;
            internal int exit;
            internal int message;
        }

        struct flagtype
        {
            internal int state;
            internal int score;
        }

        class roomtype
        {
            internal string description = new string(new char[255]);
            internal int n;
            internal int e;
            internal int w;
            internal int s;
            internal int u;
            internal int d;
        }

        class playertype
        {
            internal int maxcarry;
            internal int currentroom;
            internal int numcarry;
            internal bool moved;
            internal bool won;
            internal int currverb;
            internal int currnoun;
            internal bool dead;
        }

        class headertype
        {
            internal string name = "";
            internal int verbs;
            internal int objects;
            internal int maxobjects;
            internal int puzzles;
            internal int maxpuzzles;
            internal int flags;
            internal int blockedexits;
            internal int rooms;
            internal int maxphrases;
            internal int maxscore;
            internal int messages;
            internal int winscore;
            internal int startmessage;
        }

        const int EXIT_NORTH = 1;
        const int EXIT_EAST = 2;
        const int EXIT_WEST = 3;
        const int EXIT_SOUTH = 4;
        const int EXIT_UP = 5;
        const int EXIT_DOWN = 6;
        const int WORD_VERB = 1;
        const int WORD_NOUN = 2;
        #endregion

        #region IIFGameEngine
        public string Identifier { get { return "ASCAPE";  } }

        public string[] KnownExtensions { get { return null; } }

        public CanRunResult CanRun(IFileService fileIO)
        {
            var filesNames = fileIO.GetFileNames();

            return !String.IsNullOrEmpty(getDATFileName(fileIO)) && !String.IsNullOrEmpty(getINTFileName(fileIO)) ?
                    CanRunResult.Yes:
                    CanRunResult.No;
        }

        public IAsyncOperation<ExecutionResult> Start(IIFRuntime runtime)
        {
            return Run(runtime).AsAsyncOperation();
        }

        public IAsyncOperation<ExecutionResult> Start(IIFRuntime runtime, bool debugMessages)
        {
            return Run(runtime, debugMessages).AsAsyncOperation();
        }
        #endregion

        IIFRuntime runtime { get; set; }
        IFileService fileIO
        {
            get
            {
                return runtime != null ? runtime.FileIO : null;
            }
        }

        readonly objecttype[] objects = Arrays.InitializeWithDefaultInstances<objecttype>(255);
        readonly puzzletype[] puzzle = Arrays.InitializeWithDefaultInstances<puzzletype>(255);
        readonly blockedexittype[] blockedexit = Arrays.InitializeWithDefaultInstances<blockedexittype>(255);
        readonly flagtype[] flag = Arrays.InitializeWithDefaultInstances<flagtype>(255);
        readonly roomtype[] room = Arrays.InitializeWithDefaultInstances<roomtype>(255);
        readonly playertype player = new playertype();
        readonly headertype header = new headertype();
        readonly string[] verb = new string[255];
        readonly string[] message = new string[255];

        int numscoreobj { get; set; } = 0;
        int numscoreflags { get; set; } = 0;
        string locationfile { get; set; } = "";
        string messagefile { get; set; } = "";

        string getDATFileName(IFileService fileIO)
        {
            return fileIO.GetFileNames().Where(fn => fn.Trim().EndsWith("dat", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }
        string getINTFileName(IFileService fileIO)
        {
            return fileIO.GetFileNames().Where(fn => fn.Trim().EndsWith("int", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        // Main Entry point
        async Task<ExecutionResult> Run(IIFRuntime runtime, bool debugMessages = false)
        {
            this.runtime = runtime;

            var datFileName = getDATFileName(runtime.FileIO);
            var intFileName = getINTFileName(runtime.FileIO);

            if (String.IsNullOrEmpty(datFileName) || String.IsNullOrEmpty(intFileName))
            {
                return await Task.FromResult(ExecutionResult.ERR_INVALID_STATE);
            }

            var result = ExecutionResult.ERR_NO_ERRORS;
            bool finished = false;
            string verb = new string(new char[255]);
            string noun = new string(new char[255]);

            result = await loaddatabaseAsync(datFileName, intFileName);
            player.moved = true;
            player.won = false;

            // Start the game!
            if (debugMessages)
            {
                printdatabase();
            }
            else
            {
                logDebug("Starting\n");
                start();

                while (!finished)
                {
                    if (player.moved)
                    {
                        displayroom(player.currentroom);
                    }
                    player.moved = false;

                    var inp = await getInputAsync();
                    verb = inp.Item1;
                    noun = inp.Item2;

                    finished = parse(verb, noun) == ExecutionResult.ERR_STATE_QUIT;

                    if (player.dead || player.won)
                    {
                        score();
                        finished = true;
                    }
                }
            }

            return await Task.FromResult(result);
        }

        async Task<ExecutionResult> loaddatabaseAsync(string datFileName, string intFileName)
        {
            int infile;
            int scrap;
            var retval = ExecutionResult.ERR_NO_ERRORS;
            int version = 1;

            // First thing - is this version 2 or version 1?
            infile = fileIO.FOpen(datFileName, "rb");

            try
            {
                if (infile == null)
                {
                    logError("Could not open data file {0}\n", datFileName);
                    return await Task.FromResult(ExecutionResult.ERR_BADFILE);
                }

                fileIO.FSeek(infile, 0x3f, FSeekOffset.SEEK_SET);
                scrap = fileIO.FGetc(infile);
                if (scrap == 0x40)
                {
                    version = 2;
                }

                fileIO.FSeek(infile, 0, FSeekOffset.SEEK_SET);
                scrap = fileIO.FGetc(infile);
                if (scrap == 0x0)
                {
                    version = 1;
                }

                if (version == 2)
                {
                    logDebug("Database version 2\n");
                    retval = await loadv2databaseAsync(infile, intFileName);
                }
                else
                {
                    logDebug("Database version 1\n");
                    retval = await loadv1databaseAsync(infile, intFileName);
                }
                return retval;
            }
            finally
            {
                fileIO.FClose(infile);
            }
            return ExecutionResult.ERR_UNKNOWN;
        }

        async Task<ExecutionResult> loadv2databaseAsync(int datFile, string intFileName)
        {
            int infile = datFile;
            int locfileptr;
            int messfileptr;
            int initptr;
            int i;
            int j;
            int scrap;
            int objscore;
            int flagscore;
            int[] startptr = new int[5];
            int[] inc = new int[4];


            //infile = fopen(filename, "rb");
            fileIO.Rewind(infile);

            initptr = fileIO.FOpen(intFileName, "rb");

            // read the header
            header.verbs = read_int_data(infile);
            scrap = read_int_data(infile);
            header.objects = read_int_data(infile);
            header.maxobjects = read_int_data(infile);
            header.puzzles = read_int_data(infile);
            header.maxpuzzles = read_int_data(infile);
            header.flags = read_int_data(infile);
            header.blockedexits = read_int_data(infile);
            scrap = read_int_data(infile);

            locationfile = read_char_data(infile, locationfile);
            messagefile = read_char_data(infile, messagefile);

            locfileptr = fileIO.FOpen(locationfile, "rb");
            if (locfileptr == null)
            {
                logDebug("Could not open location file {0}\n", locationfile);
            }
            scrap = read_int_data(locfileptr);
            header.rooms = read_int_data(locfileptr);

            messfileptr = fileIO.FOpen(messagefile, "rb");
            if (messfileptr == null)
            {
                logDebug("Could not open message file {0}\n", messagefile);
            }
            scrap = read_int_data(messfileptr);
            header.messages = read_int_data(messfileptr);

            for (i = 0; i < 5; i++)
            {
                startptr[i] = read_int_data(infile);
            }
            for (i = 0; i < 4; i++)
            {
                inc[i] = read_int_data(infile);
            }
            header.maxphrases = read_int_data(infile);

            // Now read objects
            for (i = 1; i <= header.objects; i++)
            {
                int baseoffset = startptr[0] + ((i - 1) * inc[0]);

                fileIO.FSeek(infile, baseoffset, FSeekOffset.SEEK_SET);
                objects[i].name = read_char_data(infile, objects[i].name);
                fileIO.FSeek(infile, baseoffset + header.maxphrases + 2, FSeekOffset.SEEK_SET);
                objects[i].noun = read_char_data(infile, objects[i].noun);
                fileIO.FSeek(infile, baseoffset + header.maxphrases + 7, FSeekOffset.SEEK_SET);
                objects[i].description = read_char_data(infile, objects[i].description);
            }

            // Now read verbs
            for (i = 1; i <= header.verbs; i++)
            {
                int baseoffset = startptr[1] + ((i - 1) * inc[1]);
                fileIO.FSeek(infile, baseoffset, FSeekOffset.SEEK_SET);
                verb[i] = read_char_data(infile, verb[i]);
            }

            // Read rooms now, so we can use the room name later + exits are overwritten by state
            for (i = 1; i <= header.rooms; i++)
            {
                fileIO.FSeek(locfileptr, 0x0a + ((i - 1) * 0x10e), FSeekOffset.SEEK_SET);
                room[i].n = read_int_data(locfileptr);
                room[i].e = read_int_data(locfileptr);
                room[i].w = read_int_data(locfileptr);
                room[i].s = read_int_data(locfileptr);
                room[i].u = read_int_data(locfileptr);
                room[i].d = read_int_data(locfileptr);
                room[i].description = read_char_data(locfileptr, room[i].description);
            }
            fileIO.FClose(locfileptr);

            // Read messages now - so we can get it over with!
            for (i = 1; i <= header.messages; i++)
            {
                fileIO.FSeek(messfileptr, 0x0a + (240 * (i - 1)), FSeekOffset.SEEK_SET);
                message[i] = read_char_data(messfileptr, message[i]);
            }
            fileIO.FClose(messfileptr);

            // Puzzles - this is really screwed up!
            for (i = 1; i <= header.puzzles; i++)
            {
                int baseoffset = startptr[2] - header.maxpuzzles;
                int newoffset;
                int n;
                int dummy;

                fileIO.FSeek(infile, baseoffset, FSeekOffset.SEEK_SET);
                n = 0;
                dummy = 0;
                while (dummy != i)
                {
                    n++;
                    dummy = fileIO.FGetc(infile);
                }
                newoffset = startptr[2] + ((n - 1) * inc[2]) + 10;
                fileIO.FSeek(infile, newoffset, FSeekOffset.SEEK_SET);
                for (j = 0; j < 10; j++)
                {
                    puzzle[i].condition[j] = fileIO.FGetc(infile);
                }
                for (j = 0; j < 10; j++)
                {
                    puzzle[i].action[j] = fileIO.FGetc(infile);
                }
            }

            // Blocked exits
            fileIO.FSeek(infile, startptr[3], FSeekOffset.SEEK_SET);
            for (i = 1; i <= header.blockedexits; i++)
            {
                blockedexit[i].room = fileIO.FGetc(infile);
                blockedexit[i].exit = fileIO.FGetc(infile);
                blockedexit[i].message = fileIO.FGetc(infile);
            }

            // Scoring conditions
            fileIO.FSeek(infile, startptr[4], FSeekOffset.SEEK_SET);
            objscore = fileIO.FGetc(infile);
            flagscore = fileIO.FGetc(infile);
            header.winscore = fileIO.FGetc(infile);
            for (i = 1; i <= header.maxobjects; i++)
            {
                scrap = fileIO.FGetc(infile);
                if (scrap > 0)
                {
                    numscoreobj++;
                    objects[i].score = objscore;
                }
            }
            for (i = 1; i <= header.flags; i++)
            {
                scrap = fileIO.FGetc(infile);
                if (scrap > 0)
                {
                    numscoreflags++;
                    flag[i].score = flagscore;
                }
            }
            header.maxscore = (numscoreobj * objscore) + (numscoreflags * flagscore) + header.winscore;

            // Close main datafile
            //fclose(infile);

            // Load up the initial state
            header.name = read_char_data(initptr, header.name);
            header.startmessage = read_int_data(initptr);
            player.maxcarry = read_int_data(initptr);
            player.currentroom = read_int_data(initptr);
            player.numcarry = read_int_data(initptr);

            for (i = 1; i <= header.flags; i++)
            {
                flag[i].state = fileIO.FGetc(initptr);
            }
            for (i = 1; i <= header.objects; i++)
            {
                objects[i].location = fileIO.FGetc(initptr);
                scrap = fileIO.FGetc(initptr);
                switch (scrap)
                {
                    case 0:
                        objects[i].gettable = 1;
                        objects[i].wearable = 0;
                        objects[i].carried = 0;
                        objects[i].worn = 0;
                        break;
                    case 1:
                        objects[i].gettable = 1;
                        objects[i].wearable = 1;
                        objects[i].carried = 0;
                        objects[i].worn = 0;
                        break;
                    case 2:
                        objects[i].gettable = 0;
                        objects[i].wearable = 0;
                        objects[i].carried = 0;
                        objects[i].worn = 0;
                        break;
                    case 3:
                    case 5:
                        objects[i].gettable = 1;
                        objects[i].wearable = 0;
                        objects[i].carried = 1;
                        objects[i].worn = 0;
                        break;
                    case 4:
                        objects[i].gettable = 1;
                        objects[i].wearable = 1;
                        objects[i].carried = 1;
                        objects[i].worn = 1;
                        break;
                }
            }
            for (i = 1; i <= header.rooms; i++)
            {
                room[i].n = fileIO.FGetc(initptr);
                room[i].e = fileIO.FGetc(initptr);
                room[i].w = fileIO.FGetc(initptr);
                room[i].s = fileIO.FGetc(initptr);
                room[i].u = fileIO.FGetc(initptr);
                room[i].d = fileIO.FGetc(initptr);
            }

            return 0;
        }

        async Task<ExecutionResult> loadv1databaseAsync(int datFile, string intFileName)
        {
            var result = ExecutionResult.ERR_NO_ERRORS;
            int infile = datFile;
            int initptr = 0;
            int messptr = 0;
            int locptr = 0;
            int messptrptr = 0;
            int locptrptr = 0; ;
            int i;
            int j;
            int scrap;
            int objscore;
            int flagscore;
            int nso;
            int nsf;
            int disc = 0;
            string messagefile = new string(new char[255]);
            string locationfile = new string(new char[255]);
            string mangle = new string(new char[255]);

            fileIO.Rewind(infile); //fopen(filename, "rb");
            initptr = fileIO.FOpen(intFileName, "rb");

            // Check whether it the messages/locs are read from disc
            scrap = fileIO.FGetc(infile);
            if (scrap == 0)
            {
                disc = 1;
                fileIO.Rewind(infile);
                locationfile = read_char_data(infile, locationfile);
                messagefile = read_char_data(infile, messagefile);
                messptr = fileIO.FOpen(messagefile, "rb");
                locptr = fileIO.FOpen(locationfile, "rb");
                mangle = "I.";
                mangle += messagefile;
                messptrptr = fileIO.FOpen(mangle, "rb");
                mangle = "I.";
                mangle += locationfile;
                locptrptr = fileIO.FOpen(mangle, "rb");
            }
            else
            {
                fileIO.Rewind(infile);
            }

            // read the header
            header.rooms = read_int_data(infile);
            player.currentroom = read_int_data(infile);
            header.verbs = read_int_data(infile);
            header.objects = read_int_data(infile);
            player.maxcarry = read_int_data(infile);
            header.flags = read_int_data(infile);
            header.messages = read_int_data(infile);
            header.puzzles = read_int_data(infile);

            if (disc == 0)
            {
                // Now read objects
                for (i = 1; i <= header.objects; i++)
                {
                    objects[i].location = fileIO.FGetc(infile);
                }

                for (i = 1; i <= header.objects; i++)
                {
                    //object[i].state=fgetc(infile);
                    scrap = fileIO.FGetc(infile);
                }
            }

            numscoreobj = read_int_data(infile);
            objscore = read_int_data(infile);
            if (numscoreobj > 0)
            {
                for (i = 1; i <= numscoreobj; i++)
                {
                    scrap = fileIO.FGetc(infile);
                    if (scrap > 0)
                    {
                        objects[scrap].score = objscore;
                    }
                }
            }
            numscoreflags = read_int_data(infile);
            flagscore = read_int_data(infile);
            if (numscoreflags > 0)
            {
                for (i = 1; i <= numscoreflags; i++)
                {
                    scrap = fileIO.FGetc(infile);
                    if (scrap > 0)
                    {
                        flag[scrap].score = flagscore;
                    }
                }
            }
            header.winscore = fileIO.FGetc(infile);
            if (header.winscore == 0x40)
            {
                // It uses an integer rather than a byte for the winning score
                fileIO.FSeek(infile, -1, FSeekOffset.SEEK_CUR);
                header.winscore = read_int_data(infile);
            }
            header.maxscore = (numscoreobj * objscore) + (numscoreflags * flagscore) + header.winscore;

            // Now read verbs
            for (i = 1; i <= header.verbs; i++)
            {
                verb[i] = read_char_data(infile, verb[i]);
            }

            for (i = 1; i <= header.objects; i++)
            {
                objects[i].name = read_char_data(infile, objects[i].name);
            }

            for (i = 1; i <= header.objects; i++)
            {
                objects[i].noun = read_char_data(infile, objects[i].noun);
            }

            if (disc != 0)
            {
                header.name = read_char_data(infile, header.name);
                message[0] = read_char_data(infile, message[0]);
                header.startmessage = 0;
            }

            // Read rooms now, so we can use the room name later + exits are overwritten by state
            for (i = 1; i <= header.rooms; i++)
            {
                if (disc != 0)
                {
                    scrap = (i - 1) * 5;
                    if (locptrptr > 0)
                    {
                        fileIO.FSeek(locptrptr, scrap, FSeekOffset.SEEK_SET);
                        scrap = read_int_data(locptrptr);

                        if (locptr > 0)
                        {
                            fileIO.FSeek(locptr, scrap, FSeekOffset.SEEK_SET);
                            room[i].description = read_char_data(locptr, room[i].description);
                        }
                    }
                }
                else
                {
                    room[i].description = read_char_data(infile, room[i].description);
                    room[i].n = fileIO.FGetc(infile);
                    room[i].e = fileIO.FGetc(infile);
                    room[i].w = fileIO.FGetc(infile);
                    room[i].s = fileIO.FGetc(infile);
                    room[i].u = fileIO.FGetc(infile);
                    room[i].d = fileIO.FGetc(infile);
                }
            }

            if (disc == 0)
            {
                header.name = read_char_data(infile, header.name);
            }

            // Read messages now - so we can get it over with!
            for (i = 0; i <= header.messages; i++)
            {
                if (disc != 0)
                {
                    if (i < header.messages)
                    {
                        scrap = i * 5;
                        if (messptrptr > 0)
                        {
                            fileIO.FSeek(messptrptr, scrap, FSeekOffset.SEEK_SET);
                            scrap = read_int_data(messptrptr);

                            if (messptr > 0)
                            {
                                fileIO.FSeek(messptr, scrap, FSeekOffset.SEEK_SET);
                                message[i + 1] = read_char_data(messptr, message[i + 1]);
                            }
                        }
                    }
                }
                else
                {
                    message[i] = read_char_data(infile, message[i]);
                }
            }

            for (i = 1; i <= header.objects; i++)
            {
                objects[i].description = read_char_data(infile, objects[i].description);
            }
            // Puzzles - this is really screwed up!
            for (i = 1; i <= header.puzzles; i++)
            {
                for (j = 0; j < 10; j++)
                {
                    puzzle[i].condition[j] = fileIO.FGetc(infile);
                }
                for (j = 0; j < 10; j++)
                {
                    puzzle[i].action[j] = fileIO.FGetc(infile);
                }
            }

            if (disc != 0)
            {
                // Blocked exits
                for (i = 1; i <= header.blockedexits; i++)
                {
                    blockedexit[i].room = fileIO.FGetc(infile);
                    blockedexit[i].exit = fileIO.FGetc(infile);
                    blockedexit[i].message = fileIO.FGetc(infile);
                }
            }

            // Close main datafile
            //fclose(infile);

            // Load up the initial state
            player.currentroom = read_int_data(initptr);
            player.numcarry = read_int_data(initptr);

            for (i = 1; i <= header.objects; i++)
            {
                objects[i].location = fileIO.FGetc(initptr);
            }
            for (i = 1; i <= header.objects; i++)
            {
                scrap = fileIO.FGetc(initptr);
                switch (scrap)
                {
                    case 0:
                        objects[i].gettable = 1;
                        objects[i].wearable = 0;
                        objects[i].carried = 0;
                        objects[i].worn = 0;
                        break;
                    case 1:
                        objects[i].gettable = 1;
                        objects[i].wearable = 1;
                        objects[i].carried = 0;
                        objects[i].worn = 0;
                        break;
                    case 2:
                        objects[i].gettable = 0;
                        objects[i].wearable = 0;
                        objects[i].carried = 0;
                        objects[i].worn = 0;
                        break;
                    case 3:
                    case 5:
                        objects[i].gettable = 1;
                        objects[i].wearable = 0;
                        objects[i].carried = 1;
                        objects[i].worn = 0;
                        break;
                    case 4:
                        objects[i].gettable = 1;
                        objects[i].wearable = 1;
                        objects[i].carried = 1;
                        objects[i].worn = 1;
                        break;
                }
            }
            if (header.flags > 0)
            {
                for (i = 1; i <= header.flags; i++)
                {
                    flag[i].state = fileIO.FGetc(initptr);
                }
            }
            else
            { // BBC Basic always seems to execute a loop even when the start is greater than the end!
                scrap = fileIO.FGetc(initptr);
            }
            for (i = 1; i <= header.rooms; i++)
            {
                room[i].n = fileIO.FGetc(initptr);
                room[i].e = fileIO.FGetc(initptr);
                room[i].w = fileIO.FGetc(initptr);
                room[i].s = fileIO.FGetc(initptr);
                room[i].u = fileIO.FGetc(initptr);
                room[i].d = fileIO.FGetc(initptr);
            }

            fileIO.FClose(initptr);

            return await Task.FromResult(result);
        }

        int read_int_data(int inFile)
        {
            int byt = 0;
            int scrap = 0;

            scrap = fileIO.FGetc(inFile);
            if (scrap != 0x40)
            {
                logError("Cannot find an int type when expecting one!\n");
                return -1;
            }

            byt = fileIO.FGetc(inFile) << 24;
            byt += fileIO.FGetc(inFile) << 16;
            byt += fileIO.FGetc(inFile) << 8;
            byt += fileIO.FGetc(inFile);
            return byt;
        }

        string read_char_data(int inFile, string _result)
        {
            var result = "";
            var strscrap = new char[255];
            int scrap;
            int length;
            int i;

            scrap = fileIO.FGetc(inFile);
            if (scrap != 0x00)
            {
                logError("Cannot find a char type when expecting one!\n");
                return null;
            }

            length = fileIO.FGetc(inFile);
            var readResult = fileIO.FRead(strscrap, 1, length, inFile);  //length, 1, inFile);
            strscrap = (char[])readResult.Data;
            var sb = new StringBuilder(_result);
            for (i = 0; i < length; i++)
            {
                sb.Append(strscrap[length - i - 1]);
            }
            result = sb.ToString();

            return result;
        }

        void logError(string messageFormat, params object[] parms)
        {
            if (runtime.SupportsLogging)
            {
                runtime.Debug.LogError(messageFormat, parms);
            }
        }

        void logDebug(string messageFormat, params object[] parms)
        {
            if (runtime.SupportsLogging)
            {
                runtime.Debug.LogDebug(messageFormat, parms);
            }
        }

        void printexit(int roomnum, int roomexit, int exit)
        {
            int i;
            int blocked;
            string exitname = new string(new char[8]);

            exitname = (exit == 1) ? "North" : (exit == 2) ? "East" : (exit == 3) ? "West" : (exit == 4) ? "South" : (exit == 5) ? "Up" : "Down";

            if (roomexit > 0)
            {
                blocked = 0;
                if (roomexit == 255)
                {
                    for (i = 1; i <= header.blockedexits; i++)
                    {
                        if (blockedexit[i].room == roomnum && blockedexit[i].exit == exit)
                        {
                            blocked = 1;
                            logDebug(" {0} is a blocked exit with a message of ", exitname);
                            logDebug("\"{0}\"\n", message[blockedexit[i].message]);
                        }
                    }
                }
                if (blocked == 0)
                {
                    logDebug(" {0} leads to \"{1}\"\n", exitname, room[roomexit].description);
                }
            }
        }
        void printdatabase()
        {
            int i;
            int j;
            // Prints the whole database

            // First off the header
            logDebug("Game: {0}\n", header.name);
            logDebug("Numbers of objects:\n");
            logDebug(" Verbs: {0:D}\n", header.verbs);
            logDebug(" Objects: {0:D}\n", header.objects);
            logDebug(" Maximum Objects: {0:D}\n", header.maxobjects);
            logDebug(" Puzzles: {0:D}\n", header.puzzles);
            logDebug(" Maximum Puzzles: {0:D}\n", header.maxpuzzles);
            logDebug(" Flags: {0:D}\n", header.flags);
            logDebug(" Blocked Exits: {0:D}\n", header.blockedexits);
            logDebug(" Rooms: {0:D}\n", header.rooms);
            logDebug(" Maximum phrases: {0:D}\n", header.maxphrases);
            logDebug(" Messages: {0:D}\n", header.messages);
            logDebug("Maxmimum score: {0:D}\n", header.maxscore);
            logDebug("Score for winning: {0:D}\n", header.winscore);
            logDebug("Starting Message: {0}\n", message[header.startmessage]);

            logDebug("\n");
            for (i = 1; i <= header.rooms; i++)
            {
                logDebug("Room {0:D}: {1}\n", i, room[i].description);
                printexit(i, room[i].n, 1);
                printexit(i, room[i].e, 2);
                printexit(i, room[i].w, 3);
                printexit(i, room[i].s, 4);
                printexit(i, room[i].u, 5);
                printexit(i, room[i].d, 6);
            }

            logDebug("\n");
            for (i = 1; i <= header.objects; i++)
            {
                logDebug("Object {0:D}: {1}\n", i, objects[i].name);
                logDebug(" has a noun of {0}\n", objects[i].noun);
                logDebug(" a description of {0}\n", objects[i].description);
                logDebug(" and a score of {0:D}\n", objects[i].score);
                logDebug(" it can be: ");
                if (objects[i].gettable != 0)
                {
                    logDebug("picked up ");
                }
                if (objects[i].wearable != 0)
                {
                    logDebug("worn ");
                }
                logDebug("\n");
            }

            logDebug("\n");
            for (i = 1; i <= header.flags; i++)
            {
                if (flag[i].score > 0)
                {
                    logDebug("Flag {0:D} has a score of {1:D}\n", i, flag[i].score);
                }
            }

            logDebug("\n");
            for (i = 1; i <= header.verbs; i++)
            {
                logDebug("Verb {0:D}: {1}\n", i, verb[i]);
            }

            logDebug("\n");
            for (i = 1; i <= header.messages; i++)
            {
                logDebug("Message {0:D}: \"{1}\"\n", i, message[i]);
            }

            logDebug("\n");
            for (i = 1; i <= header.puzzles; i++)
            {
                logDebug("Puzzle {0:D}\n", i);
                logDebug("IF {\n");
                if (puzzle[i].condition[0] != 0)
                {
                    logDebug(" PLAYERIN {0}\n", room[puzzle[i].condition[0]].description);
                }
                if (puzzle[i].condition[1] != 0)
                {
                    logDebug(" VERB {0}\n", verb[puzzle[i].condition[1]]);
                }
                if (puzzle[i].condition[2] != 0)
                {
                    logDebug(" OBJECT {0}\n", objects[puzzle[i].condition[2]].name);
                }
                if (puzzle[i].condition[3] != 0)
                {
                    logDebug(" FLAG {0:D}\n", puzzle[i].condition[3]);
                }
                if (puzzle[i].condition[4] != 0)
                {
                    logDebug(" FLAGSET {0:D}\n", puzzle[i].condition[4]);
                }
                if (puzzle[i].condition[5] != 0)
                {
                    logDebug(" 2FLAG {0:D}\n", puzzle[i].condition[5]);
                }
                if (puzzle[i].condition[6] != 0)
                {
                    logDebug(" 2FLAGSET {0:D}\n", puzzle[i].condition[6]);
                }
                if (puzzle[i].condition[7] != 0)
                {
                    logDebug(" CARRYOBJ {0}\n", objects[puzzle[i].condition[7]].name);
                }
                if (puzzle[i].condition[8] != 0)
                {
                    logDebug(" 2CARRYOBJ {0}\n", objects[puzzle[i].condition[8]].name);
                }
                if (puzzle[i].condition[9] != 0)
                {
                    logDebug(" OBJWORN {0}\n", objects[puzzle[i].condition[9]].name);
                }
                logDebug("}\nTHEN {\n");
                if (puzzle[i].action[0] != 0)
                {
                    logDebug(" SETFLAG {0:D}\n", puzzle[i].action[0]);
                }
                if (puzzle[i].action[1] != 0)
                {
                    logDebug(" PRINTMESS {0}\n", message[puzzle[i].action[1]]);
                }
                if (puzzle[i].action[2] != 0)
                {
                    logDebug(" ALTEREXIT {0}\n", (puzzle[i].action[2] == 1) ? "North" : (puzzle[i].action[2] == 2) ? "East" : (puzzle[i].action[2] == 3) ? "West" : (puzzle[i].action[2] == 4) ? "South" : (puzzle[i].action[2] == 5) ? "Up" : "Down");
                }
                if (puzzle[i].action[3] != 0)
                {
                    logDebug(" NEWLOC {0}\n", room[puzzle[i].action[3]].description);
                }
                if (puzzle[i].action[4] != 0)
                {
                    logDebug(" ALTEROBJ {0}\n", objects[puzzle[i].action[4]].name);
                }
                if (puzzle[i].action[5] != 0)
                {
                    switch (puzzle[i].action[5])
                    {
                        case 1:
                            logDebug(" NEWOBJ MAKE_GETTABLE\n");
                            break;
                        case 2:
                            logDebug(" NEWOBJ DISAPPEAR\n");
                            break;
                        case 3:
                            logDebug(" NEWOBJ MOVE_OBJECT\n");
                            break;
                        case 4:
                            logDebug(" NEWOBJ MAKE_GET_WEAR\n");
                            break;
                        case 5:
                            logDebug(" NEWOBJ APPEAR\n");
                            break;
                        default:
                            logDebug(" NEWOBJ UNKNOWN {0:D}\n", puzzle[i].action[5]);
                            break;
                    }
                }
                if (puzzle[i].action[6] != 0)
                {
                    logDebug(" NEWOBJLOC {0}\n", room[puzzle[i].action[6]].description);
                }
                if (puzzle[i].action[7] != 0)
                {
                    if (puzzle[i].action[7] == 255)
                    {
                        logDebug(" MOVEPLAYER {0}\n", room[puzzle[i].action[8]].description);
                    }
                    else
                    {
                        logDebug(" 2ALTEROBJ {0}\n", objects[puzzle[i].action[7]].name);
                    }
                }
                if (puzzle[i].action[8] != 0)
                {
                    if (puzzle[i].action[7] != 255)
                    {
                        switch (puzzle[i].action[8])
                        {
                            case 1:
                                logDebug(" 2NEWOBJ MAKE_GETTABLE\n");
                                break;
                            case 2:
                                logDebug(" 2NEWOBJ DISAPPEAR\n");
                                break;
                            case 3:
                                logDebug(" 2NEWOBJ MOVE_OBJECT\n");
                                break;
                            case 4:
                                logDebug(" 2NEWOBJ MAKE_GET_WEAR\n");
                                break;
                            case 5:
                                logDebug(" 2NEWOBJ APPEAR\n");
                                break;
                            default:
                                logDebug(" 2NEWOBJ UNKNOWN {0:D}\n", puzzle[i].action[8]);
                                break;
                        }
                    }

                }
                if (puzzle[i].action[9] != 0)
                {
                    logDebug(" NEWOBJLOC {0}\n", room[puzzle[i].action[9]].description);
                }
                logDebug("}\n");
            }
        }
        void start()
        {
            runtime.Write("{0}\n\n", header.name);
            runtime.Write("{0}\n\n", message[header.startmessage]);
            runtime.Write("For help type HELP.\n\n");
        }

        void displayroom(int roomnum)
        {
            int i = 0;

            runtime.Write("You are {0}\n\n", room[roomnum].description);

            runtime.Write("Visible exits: ");
            if (room[roomnum].n != 0)
            {
                runtime.Write("north ");
            }
            if (room[roomnum].e != 0)
            {
                runtime.Write("east ");
            }
            if (room[roomnum].w != 0)
            {
                runtime.Write("west ");
            }
            if (room[roomnum].s != 0)
            {
                runtime.Write("south ");
            }
            if (room[roomnum].u != 0)
            {
                runtime.Write("up ");
            }
            if (room[roomnum].d != 0)
            {
                runtime.Write("down ");
            }
            runtime.Write("\n\n");

            for (i = 1; i <= header.objects; i++)
            {
                if (objects[i].location == roomnum)
                {
                    runtime.Write("You see {0}\n", objects[i].name);
                }
            }
            runtime.Write("\n");
        }

        async Task<Tuple<string, string>> getInputAsync()
        {
            var noun = "";
            var verb = "";
            int space;
            var commandSB = new StringBuilder();
            string command;
            int i;

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'memset' has no equivalent in C#:
            //memset(verb, '\0', 255);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'memset' has no equivalent in C#:
            //memset(noun, '\0', 255);
            //do
            //{
                runtime.Write("What now? ");
                var data = (await runtime.GetLineAsync()).Trim();
                commandSB.Append(data.Length > 250 ? data.Substring(0, 250) : data);
            //} while (commandSB.Length < 2);

            for (i = 0; i < commandSB.Length; i++)
            {
                commandSB[i] = char.ToUpper(commandSB[i]);
            }

            command = commandSB.ToString();
            space = command.IndexOf(" ");
            if (space == -1)
            {
                verb = command.Trim();
            }
            else if (space != -1)
            {
                verb = command.Substring(0, space).Trim();
                noun = command.Substring(space).Trim();
            }

            return new Tuple<string, string>(verb, noun);
        }

        ExecutionResult parse(string currverb, string noun)
        {
            ExecutionResult retval = ExecutionResult.ERR_NO_ERRORS;
            int i;
            int objnumber = 0;
            int verbnumber = 0;
            string scrap = ""; // new string(new char[10]);
            string shortverb = ""; // new string(new char[5]);
            string shortnoun = ""; // new string(new char[5]);

            shortnoun = noun.Length > 3 ? noun.Substring(0, 3) : noun;
            shortverb = currverb.Length > 4 ? currverb.Substring(0, 4) : currverb;
            player.currnoun = 0;
            player.currverb = 0;

            // if there's a noun, check that it exists
            for (i = 1; i <= header.objects; i++)
            {
                if (shortnoun.Equals(objects[i].noun))
                {
                    objnumber = i;
                }
            }
            if (objnumber == 0 && shortnoun.Length != 0)
            {
                unknownword(noun, WORD_NOUN);
                return retval;
            }
            player.currnoun = objnumber;

            // first check for builtins
            if (shortverb.Length == 1)
            {
                switch (shortverb.ToCharArray()[0])
                {
                    case 'N':
                        moveplayer(room[player.currentroom].n, 1);
                        break;
                    case 'E':
                        moveplayer(room[player.currentroom].e, 2);
                        break;
                    case 'W':
                        moveplayer(room[player.currentroom].w, 3);
                        break;
                    case 'S':
                        moveplayer(room[player.currentroom].s, 4);
                        break;
                    case 'U':
                        moveplayer(room[player.currentroom].u, 5);
                        break;
                    case 'D':
                        moveplayer(room[player.currentroom].d, 6);
                        break;
                    case 'I':
                        showinventory();
                        break;
                    case 'L':
                        player.moved = true;
                        break;
                    case 'Q':
                        retval = ExecutionResult.ERR_STATE_QUIT;
                        break;
                    default:
                        unknownword(shortverb, WORD_VERB);
                        break;
                }
            }
            else if (shortverb.Equals("LOOK"))
            {
                player.moved = true;
            }
            else if (shortverb.Equals("INV"))
            {
                showinventory();
            }
            else if (shortverb.Equals("HELP"))
            {
                showhelp();
            }
            else if (shortverb.Equals("QUIT"))
            {
                retval = ExecutionResult.ERR_STATE_QUIT;
            }
            else if (shortverb.Equals("SAVE"))
            {
                savegame();
            }
            else if (shortverb.Equals("LOAD"))
            {
                loadgame();
            }
            else if (shortverb.Equals("GO"))
            {
                if (noun.Length > 0)
                {
                    scrap = noun.Length > 1 ? noun.Substring(0, 1) : noun;
                    parse(scrap, "");
                }
            }
            else if (shortverb.Equals("EXAM"))
            {
                examine(objnumber);
            }
            else if (shortverb.Equals("WEAR"))
            {
                wear(objnumber);
            }
            else if (shortverb.Equals("GET") || !shortverb.Equals("TAKE"))
            {
                get(objnumber);
            }
            else if (shortverb.Equals("DROP") || !shortverb.Equals("THRO"))
            {
                drop(objnumber);
            }
            else if (shortverb.Equals("SCOR"))
            {
                score();
            }
            else if (shortverb.Equals("FLAG"))
            {
                for (i = 1; i < header.flags; i++)
                {
                    logDebug("Flag {0:D}: {1:D}\n", i, flag[i].state);
                }
            }
            else
            {
                // other verbs check here
                verbnumber = 0;
                for (i = 1; i <= header.verbs; i++)
                {
                    if (shortverb.Equals(verb[i]))
                    {
                        verbnumber = i;
                    }
                }
                if (verbnumber != 0)
                {
                    player.currverb = verbnumber;
                    checkpuzzles();
                }
                else
                {
                    unknownword(currverb, WORD_VERB);
                }
            }

            return retval;
        }

        #region actions
        void unknownword(string word, int type)
        {
            if (type == WORD_VERB)
            {
                runtime.Write("I don't know how to {0}.\n", word);
            }
            else
            {
                runtime.Write("I don't know what {0} is.\n", word);
            }
        }

        void savegame()
        {
        }

        void loadgame()
        {
        }

        void showhelp()
        {
            runtime.Write("I understand one or two word instructions such as CLIMB TREE or QUIT.");
            runtime.Write(" Some of the words I know are GET, DROP, INV (to list objects carried), LOOK, EXAMINE and QUIT.\n\n");
            runtime.Write("To move around you can type GO NORTH, GO DOWN etc or just N,E,W,S,U and D.");
            runtime.Write(" Some other commands can be abbreviated too, eg L for LOOK.\n\n");
            runtime.Write("SAVE and LOAD are used to save your game and reload it.");
            runtime.Write(" Other words you will have to discover for yourself.\n\n");
        }

        void examine(int noun)
        {
            if (objects[noun].worn != 0 || objects[noun].carried != 0 || objects[noun].location == player.currentroom)
            {
                if (Convert.ToString(objects[noun].description).Length == 0)
                {
                    runtime.Write("You see nothing special.\n\n");
                }
                else
                {
                    runtime.Write("{0}\n\n", objects[noun].description);
                }
            }
            else
            {
                runtime.Write("You can't see {0}.\n\n", objects[noun].name);
            }
        }

        void wear(int noun)
        {
            if (objects[noun].carried != 0 && objects[noun].wearable != 0)
            {
                runtime.Write("You put on the {0}.\n\n", objects[noun].name);
                objects[noun].worn = 1;
            }
            else if (objects[noun].carried == 0)
            {
                runtime.Write("You are not carrying {0}.\n\n", objects[noun].name);
            }
            else
            {
                runtime.Write("You can't wear that!\n\n");
            }
        }

        void get(int noun)
        {
            if (objects[noun].carried != 0)
            {
                runtime.Write("You are already carrying {0}.\n\n", objects[noun].name);
            }
            else if (objects[noun].location != player.currentroom)
            {
                runtime.Write("I can't see {0}.\n\n", objects[noun].name);
            }
            else if (player.numcarry == player.maxcarry)
            {
                runtime.Write("Your hands are full\n\n");
            }
            else if (objects[noun].gettable == 0)
            {
                runtime.Write("You can't.\n\n");
            }
            else
            {
                runtime.Write("You get {0}.\n\n", objects[noun].name);
                objects[noun].location = 0;
                objects[noun].carried = 1;
                player.numcarry++;
            }
        }

        void drop(int noun)
        {
            if (objects[noun].carried == 0)
            {
                runtime.Write("You are not carrying {0}.\n\n", objects[noun].name);
            }
            else
            {
                runtime.Write("You drop {0}.\n\n", objects[noun].name);
                objects[noun].location = player.currentroom;
                objects[noun].carried = 0;
                player.numcarry--;
            }
        }

        void score()
        {
            int i;
            int score = 0;

            for (i = 1; i <= header.objects; i++)
            {
                if (objects[i].carried != 0)
                {
                    score += objects[i].score;
                }
            }
            for (i = 1; i <= header.flags; i++)
            {
                if (flag[i].state != 0)
                {
                    score += flag[i].score;
                }
            }
            if (player.won)
            {
                score += header.winscore;
            }
            runtime.Write("You have score {0:D} points out of {1:D}.\n\n", score, header.maxscore);
        }

        void moveplayer(int newloc, int direction)
        {
            int i;

            switch (newloc)
            {
                case 0:
                    runtime.Write("You cannot move that way.\n\n");
                    break;
                case 255:
                    // It's a blocked exit
                    for (i = 1; i < header.blockedexits; i++)
                    {
                        if (blockedexit[i].room == player.currentroom && blockedexit[i].exit == direction)
                        {
                            runtime.Write("{0}\n\n", message[blockedexit[i].message]);
                        }
                    }
                    break;
                default:
                    player.currentroom = newloc;
                    player.moved = true;
                    checkpuzzles();
                    break;
            }
        }

        void showinventory()
        {
            int i;
            int wearing = 0;

            if (player.numcarry == 0)
            {
                runtime.Write("You are carrying nothing.\n\n");
            }
            else
            {
                runtime.Write("You are carrying:\n");
                for (i = 1; i <= header.objects; i++)
                {
                    if (objects[i].carried != 0)
                    {
                        runtime.Write("   {0}\n", objects[i].name);
                    }
                }
                for (i = 1; i <= header.objects; i++)
                {
                    if (objects[i].worn != 0)
                    {
                        wearing = 1;
                    }
                }
                if (wearing != 0)
                {
                    runtime.Write("You are wearing:\n");
                    for (i = 1; i <= header.objects; i++)
                    {
                        if (objects[i].worn != 0)
                        {
                            runtime.Write("   {0}\n", objects[i].name);
                        }
                    }
                }
            }
        }

        int checkcondition(int puzzlenum)
        {
            if (puzzle[puzzlenum].condition[0] != player.currentroom && puzzle[puzzlenum].condition[0] > 0)
            {
                return 0;
            }
            if (puzzle[puzzlenum].condition[1] != player.currverb && puzzle[puzzlenum].condition[1] > 0)
            {
                return 0;
            }
            if (puzzle[puzzlenum].condition[2] != player.currnoun && puzzle[puzzlenum].condition[2] > 0)
            {
                return 0;
            }
            if (puzzle[puzzlenum].condition[3] > 0 && flag[puzzle[puzzlenum].condition[3]].state != puzzle[puzzlenum].condition[4])
            {
                return 0;
            }
            if (puzzle[puzzlenum].condition[5] > 0 && flag[puzzle[puzzlenum].condition[5]].state != puzzle[puzzlenum].condition[6])
            {
                return 0;
            }
            if (puzzle[puzzlenum].condition[7] > 0 && objects[puzzle[puzzlenum].condition[7]].carried == 0)
            {
                return 0;
            }
            if (puzzle[puzzlenum].condition[8] > 0 && objects[puzzle[puzzlenum].condition[8]].carried == 0)
            {
                return 0;
            }
            if (puzzle[puzzlenum].condition[9] > 0 && objects[puzzle[puzzlenum].condition[9]].worn == 0)
            {
                return 0;
            }
            return puzzlenum;
        }

        void checkpuzzles()
        {
            int i = 1;
            int j;
            int doaction = 0;

            i = 1;
            do
            {
                doaction = checkcondition(i);
                i++;
            } while (i <= header.puzzles && doaction == 0);

            if (doaction > 0)
            {
                performaction(doaction);
            }
            else
            {
                if (!player.moved)
                {
                    runtime.Write("You cannot do that.\n\n");
                }
            }
        }

        void performaction(int puzzlenum)
        {
            if (puzzle[puzzlenum].action[0] == 255)
            {
                player.dead = true;
            }
            else if (puzzle[puzzlenum].action[0] == 254)
            {
                player.won = true;
            }
            else if (puzzle[puzzlenum].action[0] > 0)
            {
                flag[puzzle[puzzlenum].action[0]].state = flag[puzzle[puzzlenum].action[0]].state == 0 ? 1 : 0;
            }
            if (puzzle[puzzlenum].action[1] > 0)
            {
                runtime.Write("{0}\n\n", message[puzzle[puzzlenum].action[1]]);
            }
            switch (puzzle[puzzlenum].action[2])
            {
                case 1:
                    room[player.currentroom].n = puzzle[puzzlenum].action[3];
                    break;
                case 2:
                    room[player.currentroom].e = puzzle[puzzlenum].action[3];
                    break;
                case 3:
                    room[player.currentroom].w = puzzle[puzzlenum].action[3];
                    break;
                case 4:
                    room[player.currentroom].s = puzzle[puzzlenum].action[3];
                    break;
                case 5:
                    room[player.currentroom].u = puzzle[puzzlenum].action[3];
                    break;
                case 6:
                    room[player.currentroom].d = puzzle[puzzlenum].action[3];
                    break;
            }
            if (puzzle[puzzlenum].action[4] > 0)
            {
                switch (puzzle[puzzlenum].action[5])
                {
                    case 1:
                        objects[puzzle[puzzlenum].action[4]].gettable = 1;
                        break;
                    case 2:
                        if (objects[puzzle[puzzlenum].action[4]].carried != 0)
                        {
                            player.numcarry--;
                        }
                        objects[puzzle[puzzlenum].action[4]].carried = 0;
                        objects[puzzle[puzzlenum].action[4]].location = 0;
                        break;
                    case 3:
                        objects[puzzle[puzzlenum].action[4]].location = puzzle[puzzlenum].action[6];
                        break;
                    case 4:
                        objects[puzzle[puzzlenum].action[4]].gettable = 1;
                        objects[puzzle[puzzlenum].action[4]].wearable = 1;
                        break;
                    case 5:
                        objects[puzzle[puzzlenum].action[4]].location = player.currentroom;
                        break;
                }
            }
            if (puzzle[puzzlenum].action[7] == 255)
            {
                player.currentroom = puzzle[puzzlenum].action[8];
                player.moved = true;
            }
            else if (puzzle[puzzlenum].action[7] > 0)
            {
                switch (puzzle[puzzlenum].action[8])
                {
                    case 1:
                        objects[puzzle[puzzlenum].action[7]].gettable = 1;
                        break;
                    case 2:
                        objects[puzzle[puzzlenum].action[7]].carried = 0;
                        objects[puzzle[puzzlenum].action[7]].location = 0;
                        break;
                    case 3:
                        objects[puzzle[puzzlenum].action[7]].location = puzzle[puzzlenum].action[9];
                        break;
                    case 4:
                        objects[puzzle[puzzlenum].action[7]].gettable = 1;
                        objects[puzzle[puzzlenum].action[7]].wearable = 1;
                        break;
                    case 5:
                        objects[puzzle[puzzlenum].action[7]].location = player.currentroom;
                        break;
                }
            }
        }
        #endregion

    }
}
