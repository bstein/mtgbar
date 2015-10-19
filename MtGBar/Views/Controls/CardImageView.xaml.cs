using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MtGBar.Views.Controls
{
    public partial class CardImageView : UserControl
    {
        public CardImageView()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }
        
        public BitmapImage ImageSource
        {
            get { return (BitmapImage)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
        
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
            "ImageSource", 
            typeof(BitmapImage), 
            typeof(CardImageView), 
            new PropertyMetadata(null)
        );
    }
}