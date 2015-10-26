using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Bazam.Wpf.UIHelpers;
using Bazam.Wpf.ViewModels;
using Melek.Client.Vendors;
using Melek;
using MtGBar.Infrastructure;
using MtGBar.Infrastructure.Utilities;

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
        private ICardViewModel _CardViewModel;
        private string _CFLink;
        private string _CFPrice = PRICE_DEFAULT;
        private string _DefaultBackground;
        private string _GathererLink;
        private string _MagicCardsInfoLink;
        private string _MtgoTradersLink;
        private string _MtgoTradersPrice;
        private Dictionary<string, Dictionary<string, string>> _PriceCache = new Dictionary<string, Dictionary<string, string>>();
        private IReadOnlyList<IPrinting> _Printings;
        private IReadOnlyList<SearchResultViewModel> _SearchResults;
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
            _DefaultBackground = Path.Combine(FileSystemManager.SetArtDirectory, "default.jpg");
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

        public IReadOnlyList<SearchResultViewModel> SearchResults
        {
            get { return _SearchResults; }
            set { ChangeProperty(vm => vm.SearchResults, value); }
        }

        public ICardViewModel CardViewModel
        {
            get { return _CardViewModel; }
            set { ChangeProperty(vm => vm.CardViewModel, value); }
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

        public IReadOnlyList<IPrinting> Printings
        {
            get { return _Printings; }
            set { ChangeProperty<SearchViewModel>(vm => vm.Printings, value); }
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
                        CardViewModel = null;
                        Printings = null;
                        SelectedPrinting = null;
                    }
                    else {
                        Printings = value.Printings.OrderByDescending(p => p.Set.Date).Take(5).ToList();
                        SelectedPrinting = Printings.First();
                        Task t = UpdateSelectedCardViewModel();

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
                        Task t = UpdateSelectedCardViewModel();
                    }
                }
            }
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
            return string.IsNullOrEmpty(price) ? PRICE_DEFAULT : price;
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

        private async void QueryPriceData()
        {
            ResetPriceData();

            if (_ShowPricingData && SelectedPrinting != null && SelectedCard != null) {
                // snag references to these in case they're gone by the time the async functions come back
                string multiverseID = SelectedPrinting.MultiverseId;
                ICard selectedCard = SelectedCard;
                Set set = SelectedPrinting.Set;
                
                // get the gatherer and magiccards.info links, because that's a dreadfully easy chestnut
                GathererLink = await new GathererClient().GetLink(selectedCard, set);
                MagicCardsInfoLink = await new MagicCardsInfoClient().GetLink(selectedCard, set);

                // amazon
                AmazonClient amazonClient = new AmazonClient();
                AmazonLink = await amazonClient.GetLink(selectedCard, set);
                string amazonPrice = null;
                if (IsPriceCached(multiverseID, "amazon")) {
                    amazonPrice = _PriceCache[multiverseID]["amazon"];
                }
                else {
                    amazonPrice = await amazonClient.GetPrice(selectedCard, set);
                    CachePrice(multiverseID, "amazon", amazonPrice);
                }
                AmazonPrice = ApplyPriceDefault(amazonPrice);

                // channel fireball
                ChannelFireballClient cfbClient = new ChannelFireballClient();
                CFLink = await cfbClient.GetLink(selectedCard, set);

                string cfPrice = string.Empty;
                if (IsPriceCached(multiverseID, "cf")) {
                    cfPrice = _PriceCache[multiverseID]["cf"];
                }
                else {
                    cfPrice = await cfbClient.GetPrice(selectedCard, set);
                    CachePrice(multiverseID, "cf", cfPrice);
                }
                CFPrice = ApplyPriceDefault(cfPrice);

                // mtgotraders.com
                MtgoTradersClient mtgoTradersClient = new MtgoTradersClient();
                MtgoTradersLink = await mtgoTradersClient.GetLink(selectedCard, set);

                string mtgoTradersPrice = string.Empty;
                if (IsPriceCached(multiverseID, "mtgotraders")) {
                    mtgoTradersPrice = _PriceCache[multiverseID]["mtgotraders"];
                }
                else {
                    mtgoTradersPrice = await mtgoTradersClient.GetPrice(selectedCard, set);
                    CachePrice(multiverseID, "mtgotradrers", mtgoTradersPrice);
                }
                MtgoTradersPrice = ApplyPriceDefault(mtgoTradersPrice);

                // tcgplayer
                // set the default tcgplayer link in case the API takes a bit
                TcgPlayerClient tcgClient = new TcgPlayerClient();
                TCGPlayerLink = await tcgClient.GetLink(selectedCard, set);
                
                string tcgPlayerPrice = string.Empty;
                if (IsPriceCached(multiverseID, "tcgPlayer")) {
                    tcgPlayerPrice = _PriceCache[multiverseID]["tcgPlayer"];
                }
                else {
                    tcgPlayerPrice = await tcgClient.GetPrice(selectedCard, set);
                    CachePrice(multiverseID, "tcgPlayer", tcgPlayerPrice);
                }
                TCGPlayerPrice = ApplyPriceDefault(tcgPlayerPrice);
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
            WatermarkText = @"try """ + AppState.Instance.MelekClient.GetRandomCardName() + @"""";
        }

        private async void UpdateResults(string searchTerm)
        {
            ICard[] results = await Task<ICard[]>.Factory.StartNew(() => { 
                return AppState.Instance.MelekClient.Search(searchTerm).Take(5).ToArray(); 
            });
            List<SearchResultViewModel> vms = new List<SearchResultViewModel>();

            foreach (ICard card in results) {
                SearchResultViewModel vm = new SearchResultViewModel() {
                    Card = card
                };
                vms.Add(vm);
            }

            // set the CardMatches property so the results get bound asap
            SearchResults = vms;
            if (SearchResults.Count() != 1 || SearchResults[0].Card != SelectedCard) {
                SelectedCard = null;
            }
            
            if (SearchResults.Count() == 1) {
                SelectedCard = SearchResults[0].Card;
            }

            // then set up pretty picchurs
            foreach (SearchResultViewModel vm in SearchResults) {
                vm.FullSize = await ImageFactory.FromUri(await AppState.Instance.MelekClient.GetImageUri(vm.Card.GetLastPrinting()));
                if (vm.FullSize != null) {
                    vm.Thumbnail = new CroppedBitmap(vm.FullSize, new Int32Rect(65, 50, 96, 96));
                }
            }
        }

        private async Task UpdateSelectedCardViewModel()
        {
            await Task.Factory.StartNew(async () => {
                CardViewModel = await CardViewModelFactory.GetCardViewModel(SelectedCard, SelectedPrinting, AppState.Instance.MelekClient);
            });
        }
        #endregion
    }
}