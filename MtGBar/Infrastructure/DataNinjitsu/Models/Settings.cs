using System;
using System.IO;
using System.Xml.Linq;
using Bazam.KeyAdept.Infrastructure;
using Bazam.Modules;
using Microsoft.Win32;
using MtGBar.Infrastructure.Utilities;

namespace MtGBar.Infrastructure.DataNinjitsu.Models
{
    public class Settings
    {
        #region Events
        public event EventHandler Updated;
        #endregion

        #region Fields
        private string _SettingsFile = Path.Combine(FileSystemManager.AppDataDirectory, "settings.xml");
        #endregion

        #region Properties
        public bool DismissOnFocusLoss { get; set; }
        public int? DisplayIndex { get; set; }
        public DateTime LastImageCheck { get; set; }
        public HotkeyDescription Hotkey { get; set; }
        public string MelekDevAuthkey { get; private set; }
        public bool SaveCardImageData { get; set; }
        public bool ShowPricingData { get; set; }
        public bool StartOnSignIn { get; set; }
        #endregion

        public Settings()
        {
            _SettingsFile = Path.Combine(FileSystemManager.AppDataDirectory, "settings.xml");
            XElement settingsData = null;

            bool firstRun = false;
            using (FileStream stream = File.Open(_SettingsFile, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
                if (stream.Length > 0) {
                    settingsData = XDocument.Load(stream).Element("settings");
                    DismissOnFocusLoss = (settingsData.Attribute("dismissOnFocusLoss") != null ? XMLPal.GetBool(settingsData.Attribute("dismissOnFocusLoss")) : true);
                    DisplayIndex = (settingsData.Attribute("displayIndex") != null ? (int?)XMLPal.GetInt(settingsData.Attribute("displayIndex")) : null);
                    Hotkey = (settingsData.Attribute("hotkey") != null ? new HotkeyDescription(XMLPal.GetString(settingsData.Attribute("hotkey"))) : null);
                    LastImageCheck = XMLPal.GetDate(settingsData.Attribute("lastImageCheck"));
                    MelekDevAuthkey = (settingsData.Attribute("melekDevAuthKey") != null ? XMLPal.GetString(settingsData.Attribute("melekDevAuthKey")) : null);
                    SaveCardImageData = XMLPal.GetBool(settingsData.Attribute("saveCardImageData"));
                    ShowPricingData = XMLPal.GetBool(settingsData.Attribute("showPricingData"));
                    StartOnSignIn = XMLPal.GetBool(settingsData.Attribute("startOnSignIn"));
                }
                else {
                    firstRun = true;
                }
            }

            if (firstRun) {
                DismissOnFocusLoss = true;
                Hotkey = new HotkeyDescription(new Modifier[] { Modifier.Ctrl }, Key.Space);
                LastImageCheck = DateTime.MinValue;
                SaveCardImageData = true;
                ShowPricingData = true;
                StartOnSignIn = true;
                Save();
            }
        }

        private XDocument CreateDoc()
        {
            XDocument doc = new XDocument(
                new XElement("settings",
                    new XAttribute("dismissOnFocusLoss", DismissOnFocusLoss.ToString()),
                    (DisplayIndex != null ? new XAttribute("displayIndex", DisplayIndex.Value) : null),
                    (Hotkey != null ? new XAttribute("hotkey", Hotkey.ToString().Replace(" ", "")) : null),
                    new XAttribute("lastImageCheck", LastImageCheck.ToString()),
                    (MelekDevAuthkey != null ? new XAttribute("melekDevAuthKey", MelekDevAuthkey) : null),
                    new XAttribute("saveCardImageData", SaveCardImageData.ToString()),
                    new XAttribute("showPricingData", ShowPricingData.ToString()),
                    new XAttribute("startOnSignIn", StartOnSignIn.ToString())
                )
            );

            return doc;
        }

        public void Save()
        {
            CreateDoc().Save(_SettingsFile);

            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            string publisherName = "Jammerware";
            string productName = "MtGBar";
            string allProgramsPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
            string shortcutPath = Path.Combine(allProgramsPath, publisherName);
            shortcutPath = Path.Combine(shortcutPath, productName) + ".appref-ms";
            regKey.DeleteSubKey("MtGBar", false);

            if (StartOnSignIn) {
                regKey.SetValue("MtGBar", shortcutPath);
                regKey.Close();
            }

            if (Updated != null) {
                Updated(this, EventArgs.Empty);
            }
        }
    }
}