using System;
using System.IO;

namespace MtGBar.Infrastructure.Utilities
{
    public static class FileSystemManager
    {
        public static string AppDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Jammerware.MtGBar");
        public static string LogFileName = Path.Combine(AppDataDirectory, "errors.log");
        public static string MelekDataDirectory = AppDataDirectory;
        public static string PackageArtDirectory = Path.Combine(AppDataDirectory, "packageArt");
        public static string SetSymbolsDirectory = Path.Combine(AppDataDirectory, "setSymbols");

        public static void Init()
        {
            if (!Directory.Exists(AppDataDirectory)) {
                Directory.CreateDirectory(AppDataDirectory);
            }

            if (!Directory.Exists(PackageArtDirectory)) {
                Directory.CreateDirectory(PackageArtDirectory);
            }

            if (!Directory.Exists(SetSymbolsDirectory)) {
                Directory.CreateDirectory(SetSymbolsDirectory);
            }
        }
    }
}