using System;
using System.IO;
using static System.Environment;

namespace MtGBar.Infrastructure.Utilities
{
    public static class FileSystemManager
    {
        public static string AppDataDirectory { get; private set; }
        public static string LogFileName { get; private set; }
        public static string MelekDataDirectory { get; private set; }
        public static string SetArtDirectory { get; private set; }
        public static string SetSymbolsDirectory { get; private set; }

        public static void Init()
        {
            // this is kind of urky. Had reports of some users having problems on startup when their PATH variable is possibly nonstandard and/or
            // corrupted. not sure if this'll fix it since i can't repro locally, but we'll try this.
            string appDataRoot = null;
            appDataRoot = ResolveSpecialFolder(SpecialFolder.ApplicationData) ?? ResolveSpecialFolder(SpecialFolder.LocalApplicationData);
            if(appDataRoot == null) { appDataRoot = Environment.ExpandEnvironmentVariables("%appdata%");  }

            if(appDataRoot == null) {
                AppState.Instance.LoggingNinja.LogMessage("Unable to resolve app data root. The PATH variable might be corrupted or gone or something.");
            }

            AppDataDirectory = Path.Combine(appDataRoot, "Jammerware.MtGBar");
            LogFileName = Path.Combine(AppDataDirectory, "errors.log");
            MelekDataDirectory = AppDataDirectory;
            SetArtDirectory = Path.Combine(AppDataDirectory, "setArt");
            SetSymbolsDirectory = Path.Combine(AppDataDirectory, "setSymbols");

            if (!Directory.Exists(AppDataDirectory)) {
                Directory.CreateDirectory(AppDataDirectory);
            }

            if (!Directory.Exists(SetArtDirectory)) {
                Directory.CreateDirectory(SetArtDirectory);
            }

            if (!Directory.Exists(SetSymbolsDirectory)) {
                Directory.CreateDirectory(SetSymbolsDirectory);
            }
        }

        private static string ResolveSpecialFolder(SpecialFolder specialFolder)
        {
            string environmentPath = null;
            try { environmentPath = Environment.GetFolderPath(specialFolder); }
            catch (Exception) { }

            if (!string.IsNullOrEmpty(environmentPath)) {
                return environmentPath;
            }

            return null;
        }
    }
}