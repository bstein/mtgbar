using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Bazam.Modules;
using BazamWPF.UIHelpers;
using BazamWPF.ViewModels;
using Melek.Models;
using MtGBar.Infrastructure;
using MtGBar.Infrastructure.Utilities;

namespace MtGBar.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        #region Constants
        private const string PRICE_DEFAULT = "--";
        private const int WINDOW_WIDTH = 800;
        #endregion

        #region Fields
        private string _AmazonLink;
        private string _AmazonPrice = PRICE_DEFAULT;
        private CardViewModel[] _CardMatches;
        private string _CFLink;
        private string _CFPrice = PRICE_DEFAULT;
        private string _DefaultBackground;
        private Dictionary<string, Dictionary<string, string>> _PriceCache = new Dictionary<string, Dictionary<string, string>>();
        private string _SearchTerm;
        private Card _SelectedCard;
        private CardPrinting _SelectedPrinting;
        private BitmapImage _SelectedPrintingImage;
        private CardPrinting _SelectedPrintingTransformsIntoPrinting;
        private Card _SelectedPrintingTransformsIntoCard;
        private bool _ShowPricingData;
        private string _TCGPlayerLink;
        private string _TCGPlayerPrice = PRICE_DEFAULT;
        private string _WatermarkText;
        private int _WindowHeight;
        private int _WindowLeft;
        private int _WindowTop;
        #endregion

        #region Events
        public event EventHandler CardSelected;
        #endregion

        #region Constructor
        public SearchViewModel()
        {
            _DefaultBackground = Path.Combine(FileSystemManager.PackageArtDirectory, "default.jpg");
            ReadSettings();
            ShuffleWatermarkText();

            AppState.Instance.Settings.Updated += (theSettings, haveChanged) => {
                ReadSettings();
            };
        }
        #endregion

        #region Properties
        public string AmazonLink
        {
            get { return _AmazonLink; }
            set
            {
                if (_AmazonLink != value) {
                    _AmazonLink = value;
                    OnPropertyChanged("AmazonLink");
                }
            }
        }

        public string AmazonPrice
        {
            get { return _AmazonPrice; }
            set
            {
                if (_AmazonPrice != value) {
                    _AmazonPrice = value;
                    OnPropertyChanged("AmazonPrice");
                }
            }
        }

        public CardViewModel[] CardMatches
        {
            get { return _CardMatches; }
            set
            {
                _CardMatches = value;
                OnPropertyChanged("CardMatches");
            }
        }

        public string CFLink
        {
            get { return _CFLink; }
            set
            {
                if (_CFLink != value) {
                    _CFLink = value;
                    OnPropertyChanged("CFLink");
                }
            }
        }

        public string CFPrice
        {
            get { return _CFPrice; }
            set
            {
                if (_CFPrice != value) {
                    _CFPrice = value;
                    OnPropertyChanged("CFPrice");
                }
            }
        }

        public string DefaultBackground
        {
            get { return _DefaultBackground; }
        }

        public BitmapImage DefaultCardIcon
        {
            get { return new BitmapImage(new Uri("pack://application:,,,/Assets/card-back.png")); }
        }

        public ICommand FlipCardCommand
        {
            get 
            { 
                return new RelayCommand(
                    (omgHaveAParam) => {
                        // have to retain a reference to the printing to select after we select the card (because selecting the
                        // card manipulates the SelectedPrintingTransformsIntoPrinting property.
                        CardPrinting printingToSelect = SelectedPrintingTransformsIntoPrinting;
                        SelectedCard = SelectedPrintingTransformsIntoCard;
                        SelectedPrinting = printingToSelect;
                    }
                ); 
            }
        }

        public string SearchTerm
        {
            get { return _SearchTerm; }
            set
            {
                if (string.IsNullOrEmpty(_SearchTerm) && !string.IsNullOrEmpty(value)) {
                    ShuffleWatermarkText();
                }

                if (_SearchTerm != value) {
                    _SearchTerm = value;
                    string searchTerm = value.Trim().ToLower();
                    string setCode = string.Empty;
                    Match setCodeMatch = Regex.Match(searchTerm, "^([a-z0-9]{2,3}):");

                    if (setCodeMatch != null && setCodeMatch.Groups.Count == 2) {
                        setCode = setCodeMatch.Groups[1].Value;
                        searchTerm = searchTerm.Replace(setCodeMatch.Groups[0].Value, string.Empty).Trim();
                    }

                    UpdateResults(searchTerm, setCode);
                    OnPropertyChanged("SearchTerm");
                }
            }
        }

        public Card SelectedCard
        {
            get { return _SelectedCard; }
            set
            {
                if (SelectedCard != value) {
                    _SelectedCard = value;

                    // clear the price cache
                    ResetPriceData();
                    _PriceCache.Clear();

                    // changing the selected printing automatically requeries price data
                    if (value == null) {
                        SelectedPrinting = null;
                    }
                    else {
                        SelectedPrinting = value.Printings.OrderByDescending(a => a.Set.Date).First();
                    }

                    OnPropertyChanged("SelectedCard");
                    if (CardSelected != null) {
                        CardSelected(this, EventArgs.Empty);
                    }
                }
            }
        }

        public CardPrinting SelectedPrinting
        {
            get { return _SelectedPrinting; }
            set
            {
                if (_SelectedPrinting != value) {
                    _SelectedPrinting = value;
                    if (value != null) {
                        QueryPriceData();
                        PrintingSelected(value);

                        if (!string.IsNullOrEmpty(value.TransformsToMultiverseID)) {
                            SelectedPrintingTransformsIntoCard = AppState.Instance.MelekDataStore.GetCardByMultiverseID(value.TransformsToMultiverseID);
                            SelectedPrintingTransformsIntoPrinting = SelectedPrintingTransformsIntoCard.Printings.Where(p => p.MultiverseID == value.TransformsToMultiverseID).FirstOrDefault();
                        }
                    }
                    else {
                        SelectedPrintingTransformsIntoCard = null;
                        SelectedPrintingTransformsIntoPrinting = null;
                    }
                }
                OnPropertyChanged("SelectedPrinting");
            }
        }

        public BitmapImage SelectedPrintingImage
        {
            get { return _SelectedPrintingImage; }
            private set
            {
                if (_SelectedPrintingImage != value) {
                    _SelectedPrintingImage = value;
                    OnPropertyChanged("SelectedPrintingImage");
                }
            }
        }

        public CardPrinting SelectedPrintingTransformsIntoPrinting
        {
            get { return _SelectedPrintingTransformsIntoPrinting; }
            set
            {
                _SelectedPrintingTransformsIntoPrinting = value;
                OnPropertyChanged("SelectedPrintingTransformsIntoPrinting");
            }
        }

        public Card SelectedPrintingTransformsIntoCard
        {
            get { return _SelectedPrintingTransformsIntoCard; }
            set
            {
                _SelectedPrintingTransformsIntoCard = value;
                OnPropertyChanged("SelectedPrintingTransformsIntoCard");
            }
        }

        public bool ShowPricingData
        {
            get { return _ShowPricingData; }
            set
            {
                if (_ShowPricingData != value) {
                    _ShowPricingData = value;
                    OnPropertyChanged("ShowPricingData");
                }
            }
        }

        public string TCGPlayerLink
        {
            get { return _TCGPlayerLink; }
            set
            {
                if (_TCGPlayerLink != value) {
                    _TCGPlayerLink = value;
                    OnPropertyChanged("TCGPlayerLink");
                }
            }
        }

        public string TCGPlayerPrice
        {
            get { return _TCGPlayerPrice; }
            set
            {
                if (_TCGPlayerPrice != value) {
                    _TCGPlayerPrice = value;
                    OnPropertyChanged("TCGPlayerPrice");
                }
            }
        }

        public string WatermarkText
        {
            get { return _WatermarkText; }
            set
            {
                if (_WatermarkText != value) {
                    _WatermarkText = value;
                    OnPropertyChanged("WatermarkText");
                }
            }
        }

        public int WindowHeight
        {
            get { return _WindowHeight; }
            set
            {
                if (_WindowHeight != value) {
                    _WindowHeight = value;
                    OnPropertyChanged("WindowHeight");
                }
            }
        }

        public int WindowLeft
        {
            get { return _WindowLeft; }
            set
            {
                if (_WindowLeft != value) {
                    _WindowLeft = value;
                    OnPropertyChanged("WindowLeft");
                }
            }
        }

        public int WindowTop
        {
            get { return _WindowTop; }
            set
            {
                if (_WindowTop != value) {
                    _WindowTop = value;
                    OnPropertyChanged("WindowTop");
                }
            }
        }
        #endregion

        #region Private utility methods
        private string ApplyPriceDefault(string price)
        {
            return price == string.Empty ? PRICE_DEFAULT : price;
        }

        private void CachePrice(string multiverseID, string vendor, string price)
        {
            if (!_PriceCache.ContainsKey(multiverseID)) {
                _PriceCache.Add(multiverseID, new Dictionary<string, string>() { { vendor, price } });
            }
            else {
                _PriceCache[multiverseID][vendor] = price;
            }
        }

        private bool IsPriceCached(string multiverseID, string vendor)
        {
            return _PriceCache.ContainsKey(multiverseID) && _PriceCache[multiverseID].ContainsKey(vendor);
        }

        private void QueryPriceData()
        {
            ResetPriceData();
            if (_ShowPricingData && SelectedPrinting != null && SelectedCard != null) {
                // snag references to these in case they're gone by the time the async functions come back
                string multiverseID = SelectedPrinting.MultiverseID;
                Card selectedCard = SelectedCard;
                Set set = SelectedPrinting.Set;

                BackgroundBuddy.RunAsync(() => {
                    AmazonLink = VendorRelations.GetAmazonLink(selectedCard);

                    string amazonPrice = string.Empty;
                    if (IsPriceCached(multiverseID, "amazon")) {
                        amazonPrice = _PriceCache[multiverseID]["amazon"];
                    }
                    else {
                        amazonPrice = VendorRelations.GetAmazonPrice(selectedCard);
                        CachePrice(multiverseID, "amazon", amazonPrice);
                    }
                    AmazonPrice = ApplyPriceDefault(amazonPrice);
                });
                BackgroundBuddy.RunAsync(() => {
                    CFLink = VendorRelations.GetCFLink(selectedCard.Name, set);

                    string cfPrice = string.Empty;
                    if (IsPriceCached(multiverseID, "cf")) {
                        cfPrice = _PriceCache[multiverseID]["cf"];
                    }
                    else {
                        cfPrice = VendorRelations.GetCFPrice(selectedCard.Name, set);
                        CachePrice(multiverseID, "cf", cfPrice);
                    }
                    CFPrice = ApplyPriceDefault(cfPrice);
                });

                BackgroundBuddy.RunAsync(() => {
                    // set the default tcgplayer link in case the API takes a bit
                    TCGPlayerLink = VendorRelations.GetTCGPlayerLinkDefault(selectedCard.Name, set);
                    string tcgPlayerAPIData = VendorRelations.GetTCGPlayerAPIData(selectedCard.Name, set);
                    string apiLink = VendorRelations.GetTCGPlayerLink(tcgPlayerAPIData);
                    if (!string.IsNullOrEmpty(apiLink)) {
                        TCGPlayerLink = VendorRelations.GetTCGPlayerLink(tcgPlayerAPIData);
                    }

                    string tcgPlayerPrice = string.Empty;
                    if (IsPriceCached(multiverseID, "tcgPlayer")) {
                        tcgPlayerPrice = _PriceCache[multiverseID]["tcgPlayer"];
                    }
                    else {
                        tcgPlayerPrice = VendorRelations.GetTCGPlayerPrice(tcgPlayerAPIData);
                        CachePrice(multiverseID, "tcgPlayer", tcgPlayerPrice);
                    }
                    TCGPlayerPrice = ApplyPriceDefault(tcgPlayerPrice);
                });
            }
        }

        private async void PrintingSelected(CardPrinting printing)
        {
            SelectedPrintingImage = null;

            if (printing.Set.IsPromo && !string.IsNullOrEmpty(printing.Set.MtgImageName)) {
                SelectedPrintingImage = await AppState.Instance.MelekDataStore.GetCardImage(printing.Set, SelectedCard);
            }
            else {
                SelectedPrintingImage = await AppState.Instance.MelekDataStore.GetCardImage(printing);
            }
        }

        private void ReadSettings()
        {
            SetWindowPosition();
            ShowPricingData = AppState.Instance.Settings.ShowPricingData;
        }

        private void ResetPriceData()
        {
            AmazonLink = string.Empty;
            AmazonPrice = PRICE_DEFAULT;
            CFLink = string.Empty;
            CFPrice = PRICE_DEFAULT;
            TCGPlayerLink = string.Empty;
            TCGPlayerPrice = PRICE_DEFAULT;
        }

        private void SetWindowPosition()
        {
            int? displayIndex = AppState.Instance.Settings.DisplayIndex;
            Screen targetScreen = Screen.PrimaryScreen;

            if (displayIndex != null) {
                if (Screen.AllScreens.Count() >= displayIndex) {
                    targetScreen = Screen.AllScreens[displayIndex.Value];
                }
                else {
                    AppState.Instance.Settings.DisplayIndex = null;
                    AppState.Instance.Settings.Save();
                }
            }

            WindowLeft = targetScreen.Bounds.Left + (targetScreen.WorkingArea.Width / 2) - (WINDOW_WIDTH / 2);
            WindowTop = targetScreen.Bounds.Top + (targetScreen.WorkingArea.Height / 2) - (WindowHeight / 2);
        }

        private void ShuffleWatermarkText()
        {
            WatermarkText = "try \"" + AppState.Instance.MelekDataStore.GetRandomCardName() + "\"";
        }

        private async void UpdateResults(string searchTerm, string setCode)
        {
            Card[] results = await Task<Card[]>.Factory.StartNew(() => { return AppState.Instance.MelekDataStore.Search(searchTerm, setCode).Take(5).ToArray(); });
            List<CardViewModel> vms = new List<CardViewModel>();

            foreach (Card card in results) {
                CardViewModel vm = new CardViewModel(card);
                vms.Add(vm);
            }

            // set the CardMatches property so the results get bound asap
            CardMatches = vms.ToArray();
            if (CardMatches.Count() != 1 || CardMatches[0].Card != SelectedCard) {
                SelectedCard = null;
            }
            
            if (CardMatches.Count() == 1) {
                SelectedCard = CardMatches[0].Card;
            }

            // then set up pretty picchurs
            foreach (CardViewModel vm in CardMatches) {
                vm.FullSize = await AppState.Instance.MelekDataStore.GetCardImage(vm.Card.Printings[0]);
                if (vm.FullSize != null) {
                    vm.Thumbnail = new CroppedBitmap(vm.FullSize, new Int32Rect(54, 54, 84, 84));
                }
            }
        }
        #endregion
    }
}