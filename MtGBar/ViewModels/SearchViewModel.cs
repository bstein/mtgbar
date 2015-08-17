using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Bazam.Wpf.UIHelpers;
using Bazam.Wpf.ViewModels;
using Melek.Domain;
using MtGBar.Infrastructure;
using MtGBar.Infrastructure.Utilities;
using MtGBar.Infrastructure.Utilities.VendorRelations;

namespace MtGBar.ViewModels
{
    public class SearchViewModel : ViewModelBase<SearchViewModel>
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
        private string _GathererLink;
        private string _MagicCardsInfoLink;
        private string _MtgoTradersLink;
        private string _MtgoTradersPrice;
        private Dictionary<string, Dictionary<string, string>> _PriceCache = new Dictionary<string, Dictionary<string, string>>();
        private string _SearchTerm;
        private ICard _SelectedCard;
        private IPrinting _SelectedPrinting;
        private BitmapImage _SelectedPrintingImage;
        private IPrinting _SelectedPrintingTransformsIntoPrinting;
        private ICard _SelectedPrintingTransformsIntoCard;
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
            set { ChangeProperty(vm => vm.AmazonLink, value); }
        }

        public string AmazonPrice
        {
            get { return _AmazonPrice; }
            set { ChangeProperty(vm => vm.AmazonPrice, value); }
        }

        public CardViewModel[] CardMatches
        {
            get { return _CardMatches; }
            set { ChangeProperty(vm => vm.CardMatches, value); }
        }

        public string CFLink
        {
            get { return _CFLink; }
            set { ChangeProperty(vm => vm.CFLink, value); }
        }

        public string CFPrice
        {
            get { return _CFPrice; }
            set { ChangeProperty(vm => vm.CFPrice, value); }
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
                    () => {
                        // have to retain a reference to the printing to select after we select the card (because selecting the
                        // card manipulates the SelectedPrintingTransformsIntoPrinting property.
                        IPrinting printingToSelect = SelectedPrintingTransformsIntoPrinting;
                        SelectedCard = SelectedPrintingTransformsIntoCard;
                        SelectedPrinting = printingToSelect;
                    }
                ); 
            }
        }

        public string GathererLink
        {
            get { return _GathererLink; }
            set { ChangeProperty(s => s.GathererLink, value); }
        }

        public string MagicCardsInfoLink
        {
            get { return _MagicCardsInfoLink; }
            set { ChangeProperty(s => s.MagicCardsInfoLink, value); }
        }

        public string MtgoTradersLink
        {
            get { return _MtgoTradersLink; }
            set { ChangeProperty(s => s.MtgoTradersLink, value); }
        }

        public string MtgoTradersPrice
        {
            get { return _MtgoTradersPrice; }
            set { ChangeProperty(s => s.MtgoTradersPrice, value); }
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
                    string searchTerm = value.Trim().ToLower();
                    UpdateResults(searchTerm);
                    ChangeProperty(vm => vm.SearchTerm, value);
                }
            }
        }

        public ICard SelectedCard
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

                        // log the fact that this card was selected
                        AppState.Instance.Settings.LogRecentCard(value);
                    }

                    RaisePropertyChanged("SelectedCard");
                    if (CardSelected != null) {
                        CardSelected(this, EventArgs.Empty);
                    }
                }
            }
        }

        public IPrinting SelectedPrinting
        {
            get { return _SelectedPrinting; }
            set
            {
                if (_SelectedPrinting != value) {
                    ChangeProperty(vm => vm.SelectedPrinting, value);
                    if (value != null) {
                        QueryPriceData();
                        PrintingSelected(value);

                        if(value.GetType().IsAssignableFrom(typeof(TransformPrinting))) {
                            TransformPrinting printing = (value as TransformPrinting);
                            SelectedPrintingTransformsIntoCard = AppState.Instance.MelekClient.GetCardByMultiverseId(printing.MultiverseId);
                            SelectedPrintingTransformsIntoPrinting = SelectedPrintingTransformsIntoCard.Printings.Where(p => p.MultiverseID == value.TransformsToMultiverseID).FirstOrDefault();
                        }
                    }
                    else {
                        SelectedPrintingTransformsIntoCard = null;
                        SelectedPrintingTransformsIntoPrinting = null;
                    }
                }
            }
        }

        public BitmapImage SelectedPrintingImage
        {
            get { return _SelectedPrintingImage; }
            private set { ChangeProperty(vm => vm.SelectedPrintingImage, value); }
        }

        public IPrinting SelectedPrintingTransformsIntoPrinting
        {
            get { return _SelectedPrintingTransformsIntoPrinting; }
            set { ChangeProperty(vm => vm.SelectedPrintingTransformsIntoPrinting, value); }
        }

        public ICard SelectedPrintingTransformsIntoCard
        {
            get { return _SelectedPrintingTransformsIntoCard; }
            set { ChangeProperty(vm => vm.SelectedPrintingTransformsIntoCard, value); }
        }

        public bool ShowPricingData
        {
            get { return _ShowPricingData; }
            set { ChangeProperty(vm => vm.ShowPricingData, value); }
        }

        public string TCGPlayerLink
        {
            get { return _TCGPlayerLink; }
            set { ChangeProperty(vm => vm.TCGPlayerLink, value); }
        }

        public string TCGPlayerPrice
        {
            get { return _TCGPlayerPrice; }
            set { ChangeProperty(vm => vm.TCGPlayerPrice, value); }
        }

        public string WatermarkText
        {
            get { return _WatermarkText; }
            set { ChangeProperty(vm => vm.WatermarkText, value); }
        }

        public int WindowHeight
        {
            get { return _WindowHeight; }
            set { ChangeProperty(vm => vm.WindowHeight, value); }
        }

        public int WindowLeft
        {
            get { return _WindowLeft; }
            set { ChangeProperty(vm => vm.WindowLeft, value); }
        }

        public int WindowTop
        {
            get { return _WindowTop; }
            set { ChangeProperty(vm => vm.WindowTop, value); }
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
                string multiverseID = SelectedPrinting.MultiverseId;
                ICard selectedCard = SelectedCard;
                Set set = SelectedPrinting.Set;
                
                // get the gatherer and magiccards.info links, because that's an easy chestnut
                GathererLink = VendorRelationsUtilities.GetGathererLink(SelectedPrinting.MultiverseID);
                MagicCardsInfoLink = VendorRelationsUtilities.GetMagicCardsInfoLink(selectedCard.Name);

                BackgroundBuddy.RunAsync(() => {
                    AmazonLink = VendorRelationsUtilities.GetAmazonLink(selectedCard, set);

                    string amazonPrice = string.Empty;
                    if (IsPriceCached(multiverseID, "amazon")) {
                        amazonPrice = _PriceCache[multiverseID]["amazon"];
                    }
                    else {
                        amazonPrice = VendorRelationsUtilities.GetAmazonPrice(selectedCard, set);
                        CachePrice(multiverseID, "amazon", amazonPrice);
                    }
                    AmazonPrice = ApplyPriceDefault(amazonPrice);
                });

                BackgroundBuddy.RunAsync(() => {
                    CFLink = VendorRelationsUtilities.GetCFLink(selectedCard.Name, set);

                    string cfPrice = string.Empty;
                    if (IsPriceCached(multiverseID, "cf")) {
                        cfPrice = _PriceCache[multiverseID]["cf"];
                    }
                    else {
                        cfPrice = VendorRelationsUtilities.GetCFPrice(selectedCard.Name, set);
                        CachePrice(multiverseID, "cf", cfPrice);
                    }
                    CFPrice = ApplyPriceDefault(cfPrice);
                });

                BackgroundBuddy.RunAsync(() => {
                    MtgoTraders mtgoTradersClient = new MtgoTraders();
                    MtgoTradersLink = mtgoTradersClient.GetLink(selectedCard, set);
                    string mtgoTradersPrice = string.Empty;

                    if (IsPriceCached(multiverseID, "mtgotraders")) {
                        mtgoTradersPrice = _PriceCache[multiverseID]["mtgotraders"];
                    }
                    else {
                        mtgoTradersPrice = mtgoTradersClient.GetPrice(selectedCard, set);
                        CachePrice(multiverseID, "mtgotradrers", mtgoTradersPrice);
                    }
                    MtgoTradersPrice = ApplyPriceDefault(mtgoTradersPrice);
                });

                BackgroundBuddy.RunAsync(() => {
                    // set the default tcgplayer link in case the API takes a bit
                    TCGPlayerLink = VendorRelationsUtilities.GetTCGPlayerLinkDefault(selectedCard.Name, set);
                    string tcgPlayerAPIData = VendorRelationsUtilities.GetTCGPlayerAPIData(selectedCard.Name, set);
                    string apiLink = VendorRelationsUtilities.GetTCGPlayerLink(tcgPlayerAPIData);
                    if (!string.IsNullOrEmpty(apiLink)) {
                        TCGPlayerLink = VendorRelationsUtilities.GetTCGPlayerLink(tcgPlayerAPIData);
                    }

                    string tcgPlayerPrice = string.Empty;
                    if (IsPriceCached(multiverseID, "tcgPlayer")) {
                        tcgPlayerPrice = _PriceCache[multiverseID]["tcgPlayer"];
                    }
                    else {
                        tcgPlayerPrice = VendorRelationsUtilities.GetTCGPlayerPrice(tcgPlayerAPIData);
                        CachePrice(multiverseID, "tcgPlayer", tcgPlayerPrice);
                    }
                    TCGPlayerPrice = ApplyPriceDefault(tcgPlayerPrice);
                });
            }
        }

        private async void PrintingSelected(IPrinting printing)
        {
            // clear existing selection
            SelectedPrintingImage = null;

            // get a cool image uri
            Uri imageUri = await AppState.Instance.MelekClient.GetCardImageUri(printing);

            // select it
            SelectedPrintingImage = await ImageFactory.FromUri(imageUri);
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
            MagicCardsInfoLink = string.Empty;
            MtgoTradersLink = string.Empty;
            MtgoTradersPrice = PRICE_DEFAULT;
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

        private async void UpdateResults(string searchTerm)
        {
            ICard[] results = await Task<ICard[]>.Factory.StartNew(() => { 
                return AppState.Instance.MelekClient.Search(searchTerm).Take(5).ToArray(); 
            });
            List<CardViewModel> vms = new List<CardViewModel>();

            foreach (ICard card in results) {
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
                vm.FullSize = await ImageFactory.FromUri(await AppState.Instance.MelekClient.GetCardImageUri(vm.Card.Printings[0]));
                if (vm.FullSize != null) {
                    vm.Thumbnail = new CroppedBitmap(vm.FullSize, new Int32Rect(65, 50, 96, 96));
                }
            }
        }
        #endregion
    }
}