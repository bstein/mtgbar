using System;
using System.IO;
using System.Net;
using Bazam.Modules;
using Melek.Models;

namespace MtGBar.Infrastructure.Utilities
{
    public static class ImageManager
    {
        private static void DownloadImage(string url, string localPath)
        {
            Uri webUri = new Uri(url);
            BackgroundBuddy.RunAsync(() => {
                try {
                    new WebClient().DownloadFile(webUri.AbsoluteUri, localPath);
                }
                catch (WebException) {
                    // idc
                }
            });
        }

        private static string GetSetSymbolFileName(Set set, CardRarity rarity)
        {
            return set.Code.ToLower() + "-" + rarity.ToString().ToCharArray()[0].ToString().ToLower() + ".png";
        }

        public static void DownloadSetSymbol(Set set, CardRarity rarity)
        {
            string setSymbolFileName = GetSetSymbolFileName(set, rarity);
            Uri localUri = new Uri(Path.Combine(FileSystemManager.SetSymbolsDirectory, setSymbolFileName));

            if (!File.Exists(localUri.ToString())) {
                BackgroundBuddy.RunAsync(() => { DownloadImage(AppConstants.SETSYMBOL_URL_BASE + setSymbolFileName, localUri.LocalPath); });
            }
        }

        public static string GetSetSymbolPath(Set set, CardRarity rarity)
        {
            return Path.Combine(FileSystemManager.SetSymbolsDirectory, GetSetSymbolFileName(set, rarity));
        }

        public static string GetSetSymbolUrl(Set set, CardRarity rarity)
        {
            return AppConstants.SETSYMBOL_URL_BASE + GetSetSymbolFileName(set, rarity);
        }

        public static void DownloadImageData()
        {
            if (DateTime.Now - AppState.Instance.Settings.LastImageCheck >= TimeSpan.FromHours(24)) {
                BackgroundBuddy.RunAsync(() => {
                    if (AppState.Instance.MelekDataStore != null) {
                   
                        // maybe package arts?
                        // we need to know which package has the most recent date so we can know what hip background to make the default
                        DateTime maxDate = DateTime.MinValue;
                        string localPathToUse = null;

                        foreach (Package package in AppState.Instance.MelekDataStore.GetPackages()) {
                            string fileName = package.ID + ".jpg";
                            string localPath = Path.Combine(FileSystemManager.PackageArtDirectory, fileName);

                            if (!File.Exists(localPath)) {
                                DownloadImage(AppConstants.PACKAGEBACKGROUND_URL_BASE + fileName, localPath);
                            }

                            if (package.CardsReleased > maxDate && File.Exists(localPath)) {
                                maxDate = package.DataUpdated;
                                localPathToUse = localPath;
                            }
                        }

                        if (!string.IsNullOrEmpty(localPathToUse)) {
                            File.Copy(localPathToUse, Path.Combine(FileSystemManager.PackageArtDirectory, "default.jpg"), true);
                        }

                        AppState.Instance.Settings.LastImageCheck = DateTime.Now;
                        AppState.Instance.Settings.Save();
                    }
                });
            }
        }
    }
}