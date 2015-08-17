using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Bazam.KeyAdept.Infrastructure;
using Bazam.Modules;
using Melek.Domain;
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
        private List<ICard> _RecentCards = new List<ICard>();
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
        public bool ShowWelcomeScreen { get; set; }
        public bool StartOnSignIn { get; set; }

        public ICard[] RecentCards 
        {
            get { return _RecentCards.ToArray(); }
        }
        #endregion

        public Settings()
        {
            XElement settingsData = null;
            _RecentCards = new List<ICard>();

            bool firstRun = false;
            settingsData = GetSettingsData();

            if (settingsData != null) {
                DismissOnFocusLoss = (settingsData.Attribute("dismissOnFocusLoss") != null ? XmlPal.GetBool(settingsData.Attribute("dismissOnFocusLoss")) : true);
                DisplayIndex = (settingsData.Attribute("displayIndex") != null ? (int?)XmlPal.GetInt(settingsData.Attribute("displayIndex")) : null);
                Hotkey = (settingsData.Attribute("hotkey") != null ? new HotkeyDescription(XmlPal.GetString(settingsData.Attribute("hotkey"))) : null);
                LastImageCheck = XmlPal.GetDate(settingsData.Attribute("lastImageCheck"));
                MelekDevAuthkey = (settingsData.Attribute("melekDevAuthKey") != null ? XmlPal.GetString(settingsData.Attribute("melekDevAuthKey")) : null);
                SaveCardImageData = XmlPal.GetBool(settingsData.Attribute("saveCardImageData"));
                ShowPricingData = XmlPal.GetBool(settingsData.Attribute("showPricingData"));
                ShowWelcomeScreen = (settingsData.Attribute("showWelcomeScreen") != null ? XmlPal.GetBool(settingsData.Attribute("showWelcomeScreen")) : true);
                StartOnSignIn = XmlPal.GetBool(settingsData.Attribute("startOnSignIn"));
            }
            else {
                firstRun = true;
            }

            if (firstRun) {
                DismissOnFocusLoss = true;
                Hotkey = new HotkeyDescription(new Modifier[] { Modifier.Ctrl }, Key.Space);
                LastImageCheck = DateTime.MinValue;
                SaveCardImageData = true;
                ShowPricingData = true;
                ShowWelcomeScreen = true;
                StartOnSignIn = true;
                Save();
            }
        }

        private XDocument CreateDoc()
        {
            XElement recentCards = new XElement("recentCards");
            foreach (Card card in RecentCards) {
                recentCards.Add(new XElement("card", new XAttribute("multiverseID", card.Printings[0].MultiverseId)));
            }

            XDocument doc = new XDocument(
                new XElement("settings",
                    new XAttribute("dismissOnFocusLoss", DismissOnFocusLoss.ToString()),
                    (DisplayIndex != null ? new XAttribute("displayIndex", DisplayIndex.Value) : null),
                    (Hotkey != null ? new XAttribute("hotkey", Hotkey.ToString().Replace(" ", "")) : null),
                    new XAttribute("lastImageCheck", LastImageCheck.ToString()),
                    (MelekDevAuthkey != null ? new XAttribute("melekDevAuthKey", MelekDevAuthkey) : null),
                    new XAttribute("saveCardImageData", SaveCardImageData.ToString()),
                    new XAttribute("showPricingData", ShowPricingData.ToString()),
                    new XAttribute("showWelcomeScreen", ShowWelcomeScreen.ToString()),
                    new XAttribute("startOnSignIn", StartOnSignIn.ToString()),
                    recentCards
                )
            );

            return doc;
        }

        private XElement GetSettingsData()
        {
            XElement settingsData = null;
            using (Stream stream = GetSettingsFileStream()) {
                if(stream.Length > 0) settingsData = XDocument.Load(stream).Element("settings");
            }

            return settingsData;
        }

        private Stream GetSettingsFileStream()
        {
            return File.Open(_SettingsFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

        public void LoadRecentCards()
        {
            XElement settingsData = GetSettingsData();
            IEnumerable<ICard> recentCards = null;

            if (settingsData.Element("recentCards") != null) {
                recentCards = (
                    from recentCard in settingsData.Element("recentCards").Elements("card")
                    select AppState.Instance.MelekClient.GetCardByMultiverseId(recentCard.Attribute("multiverseID").Value)
                );
            }

            if (recentCards != null && recentCards.Count() > 0) {
                recentCards = recentCards.Take(5);
            }
            else {
                recentCards = new Card[] { };
            }

            _RecentCards = recentCards.ToList();
        }

        private void RaiseUpdated()
        {
            if (Updated != null) {
                Updated(this, EventArgs.Empty);
            }
        }

        public void LogRecentCard(Card card)
        {
            if (_RecentCards.Contains(card)) {
                _RecentCards.Remove(card);
            }
            else if (_RecentCards.Count > 4) {
                _RecentCards.RemoveAt(0);
            }

            _RecentCards.Add(card);
            Save();
            RaiseUpdated();
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

            RaiseUpdated();
        }
    }
}