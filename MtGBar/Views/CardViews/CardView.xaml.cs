using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Melek.Domain;

namespace MtGBar.Views.CardViews
{
    public partial class CardView : UserControl
    {
        public CardView()
        {
            InitializeComponent();

            // http://www.codeproject.com/Articles/325911/A-Simple-Pattern-for-Creating-Re-useable-UserContr
            LayoutRoot.DataContext = this;
        }
        
        public Card Card
        {
            get { return (Card)GetValue(CardProperty); }
            set { SetValue(CardProperty, value); }
        }

        public static readonly DependencyProperty CardProperty = DependencyProperty.Register(
            "Card", 
            typeof(Card), 
            typeof(CardView), 
            new PropertyMetadata(null)
        );
        
        public BitmapImage CardImage
        {
            get { return (BitmapImage)GetValue(CardImageProperty); }
            set { SetValue(CardImageProperty, value); }
        }
        
        public static readonly DependencyProperty CardImageProperty = DependencyProperty.Register(
            "CardImage", 
            typeof(BitmapImage), 
            typeof(CardView), 
            new PropertyMetadata(null)
        );
        
        public Printing Printing
        {
            get { return (Printing)GetValue(PrintingProperty); }
            set { SetValue(PrintingProperty, value); }
        }
        
        public static readonly DependencyProperty PrintingProperty = DependencyProperty.Register(
            "Printing", 
            typeof(Printing), 
            typeof(CardView), 
            new PropertyMetadata(null)
        );
    }
}