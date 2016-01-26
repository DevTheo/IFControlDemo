using System;
using System.Linq;
using System.Threading.Tasks;
using IFCore.Services.IOC;
using IFEngine.Ascape;
using IFInterfaces;
using Windows.Storage;
using Windows.Storage.Pickers;
using IFInterfaces.Support;
using IFCore.Services;
using System.Collections.Generic;

namespace IFEngine_UWP.Support
{
    public class AppCore
    {
        #region singleton
        public static AppCore Inst
        {
            get; private set;
        }
        private AppCore() { }
        static AppCore()
        {
            Inst = new AppCore();
            Inst.SetupEngines();
        }
        #endregion singleton

        public IOCContainer IOC
        {
            get
            {
                return IOCContainer.Inst;
            }
        }

        public void SetupEngines()
        {
            var engines = new[]
            {
                (IIFGameEngine)new AScapeIFEngine()
            };
            IOC.AddGameEngines(engines);
        }

        public async Task<bool> OpenFileAndCreateFileServiceAsync()
        {
            try
            {

                var fileOpen = new FileOpenPicker()
                {
                    SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                    ViewMode = PickerViewMode.List
                };

                // TODO: make this extensible via the engines
                var fileTypes = new List<string>()
                {
                    ".zip",
                    ".rar",
                    ".7z",
                    ".tar",
                    ".gz",
                    ".tgz"
                };
                
                foreach (var engine in IOC.GameEngines)
                {
                    if (engine.KnownExtensions != null && engine.KnownExtensions.Length > 0)
                    {
                        engine.KnownExtensions.ToList().ForEach(i => 
                        {
                            if (!fileTypes.Contains(i.ToLower()))
                            {
                                fileTypes.Add(i.ToLower());
                            }
                        });
                    }
                }

                fileTypes = fileTypes.OrderBy(i => i).ToList();

                fileTypes.ForEach(i => fileOpen.FileTypeFilter.Add(i));

                StorageFile file = null;

                try
                {
                    file = await fileOpen.PickSingleFileAsync();
                }
                catch (Exception ex)
                {
                    var t = ex;
                }
                if (file != null)
                {
                    await IOC.FileService.InitFileServiceAsync(new[] { file });
                }

                return IOC.FileService.GetFileNames().Length > 0;
            }
            catch (Exception ex)
            {
                var t = ex;
            }
            return false;
        }

        public async Task LoadAndRunAsync(IIFTextControl consoleControl)
        {
            SetupEngines();
            if(await OpenFileAndCreateFileServiceAsync())
            {
                var engines = IOC.GetValidGameEngines();
                var engine = engines.FirstOrDefault(i => i.CanRun == CanRunResult.Yes);

                // We don't have a clear cut engine, but there is only one potential match
                if (engine == null && engines.Length == 1)
                {
                    engine = engines[0];
                }
                // We don't have a clear cut engine, and there are several potential matches
                else if (engine == null && engines.Length > 1)
                {
                    // TODO: launch engine chooser
                }

                // We have an engine, so run
                if (engine != null)
                {
                    var runtime = new SimpleRuntimeInterface(consoleControl);
                    await engine.GameEngine.Start(runtime).AsTask();
                }
            }
        }

    }
}
