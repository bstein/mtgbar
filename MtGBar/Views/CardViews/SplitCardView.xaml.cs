using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Melek.Domain;

namespace MtGBar.Views.CardViews
{
    public partial class SplitCardView : UserControl
    {
        public SplitCardView()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }
        
        public SplitCard Card
        {
            get { return (SplitCard)GetValue(CardProperty); }
            set { SetValue(CardProperty, value); }
        }
        
        public static readonly DependencyProperty CardProperty = DependencyProperty.Register(
            "Card", 
            typeof(SplitCard), 
            typeof(SplitCardView), 
            new PropertyMetadata(null)
        );
        
        public BitmapImage CardImage
        {
            get { return (BitmapImage)GetValue(CardImageProperty); }
            set { SetValue(CardImageProperty, value); }
        }

        public static readonly DependencyProperty CardImageProperty = DependencyProperty.Register(
            "CardImageProperty", 
            typeof(BitmapImage), 
            typeof(SplitCardView), 
            new PropertyMetadata(null)
        );
        
        public SplitPrinting Printing
        {
            get { return (SplitPrinting)GetValue(PrintingProperty); }
            set { SetValue(PrintingProperty, value); }
        }
        
        public static readonly DependencyProperty PrintingProperty = DependencyProperty.Register(
            "Printing", 
            typeof(SplitPrinting), 
            typeof(SplitCardView), 
            new PropertyMetadata(null)
        );
    }
}