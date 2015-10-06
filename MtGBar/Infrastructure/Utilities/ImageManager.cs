using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Melek.Domain;

namespace MtGBar.Infrastructure.Utilities
{
    public static class ImageManager
    {
        private static async Task DownloadImage(string url, string localPath)
        {
            // check if the file is even a real thing
            HttpWebRequest request = HttpWebRequest.CreateHttp(url);
            request.Method = "HEAD";

            try {
                using (HttpWebResponse response = (await request.GetResponseAsync() as HttpWebResponse)) {
                    if (response.StatusCode == HttpStatusCode.OK) {
                        await Task.Factory.StartNew(() => {
                            Uri webUri = new Uri(url);
                            WebClient client = new WebClient();
                            client.DownloadFileAsync(webUri, localPath);
                        });
                    }
                }
            }
            catch (WebException) {
                // this is cool, sometimes we don't have images. we'll make it.
            }
        }

        private static string GetSetSymbolFileName(Set set, CardRarity rarity)
        {
            return set.Code.ToLower() + "-" + rarity.ToString().ToCharArray()[0].ToString().ToLower() + ".png";
        }

        public static async Task DownloadSetSymbol(Set set, CardRarity rarity)
        {
            string setSymbolFileName = GetSetSymbolFileName(set, rarity);
            Uri localUri = new Uri(Path.Combine(FileSystemManager.SetSymbolsDirectory, setSymbolFileName));

            if (!File.Exists(localUri.ToString())) {
                await DownloadImage(AppConstants.SETSYMBOL_URL_BASE + setSymbolFileName, localUri.LocalPath);
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

        public static async void DownloadImageData()
        {
            if (AppState.Instance.Settings.LastImageCheck == null || DateTime.Now - AppState.Instance.Settings.LastImageCheck >= TimeSpan.FromHours(24)) {
                // maybe special per-set arts?
                // we need to know which set has the most recent date so we can know what hip background to make the default
                DateTime maxDate = DateTime.MinValue;
                string localPathToUse = null;

                foreach (Set set in AppState.Instance.MelekClient.GetSets()) {
                    string fileName = set.Code.ToLower() + ".jpg";
                    string localPath = Path.Combine(FileSystemManager.SetArtDirectory, fileName);

                    if (!File.Exists(localPath)) {
                        await DownloadImage(AppConstants.SET_BACKGROUND_URL_BASE + fileName, localPath);
                    }

                    if (set.Date != null && set.Date > maxDate && File.Exists(localPath)) {
                        maxDate = set.Date.Value;
                        localPathToUse = localPath;
                    }
                }

                if (!string.IsNullOrEmpty(localPathToUse)) {
                    try {
                        File.Copy(localPathToUse, Path.Combine(FileSystemManager.SetArtDirectory, "default.jpg"), true);

                        AppState.Instance.Settings.LastImageCheck = DateTime.Now;
                        AppState.Instance.Settings.Save();
                    }
                    catch(IOException) {
                        // TODO: be nonterrible
                        // couldn't write the new set art over the default. try again later?
                    }
                }
            }
        }
    }
}