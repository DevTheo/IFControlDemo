using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using IFInterfaces;
using IFInterfaces.Services;

namespace IFCore.Services.IOC
{
    public sealed class PossibleGameEngines
    {
        public PossibleGameEngines(CanRunResult canRun, IIFGameEngine gameEngine)
        {
            CanRun = canRun;
            GameEngine = gameEngine;
        }

        public CanRunResult CanRun { get; private set; }
        public IIFGameEngine GameEngine { get; private set; }
    }

    public sealed class IOCContainer
    {

        #region singleton
        public static IOCContainer Inst
        {
            get; private set;
        }
        private IOCContainer()
        {
            FileService = new ArchiveAndLocalBasedFileService();
            DebugService = new SimpleDebugService();
            gameEngines = new List<IIFGameEngine>();
        }
        static IOCContainer()
        {
            Inst = new IOCContainer();
        }
        #endregion singleton

        public IFileService FileService { get; set; }

        public IDebugService DebugService { get; set; }

        public IControlCreator ControlCreator { get; set; }

        List<IIFGameEngine> gameEngines;

        public IIFGameEngine[] GameEngines { get { return gameEngines.ToArray(); } }

        public void AddGameEngine(IIFGameEngine engine)
        {
            if(gameEngines.Any(i => i.Identifier.Equals(engine.Identifier)))
            {
                var existing = gameEngines.First(i => i.Identifier.Equals(engine.Identifier));
                gameEngines.Remove(existing);
            }

            gameEngines.Add(engine);
        }

        public void AddGameEngines([ReadOnlyArray]IIFGameEngine[] engines)
        {
            foreach (var engine in engines)
            {
                AddGameEngine(engine);
            }
        }

        public PossibleGameEngines[] GetValidGameEngines()
        {
            var list = new List<PossibleGameEngines>();
            gameEngines.ForEach(i =>
            {
                var result = i.CanRun(FileService);
                if(result != CanRunResult.No && result != CanRunResult.Unkown)
                {
                    list.Add(new PossibleGameEngines(result, i));
                }
            });

            return list.ToArray();
        }
    }
}
