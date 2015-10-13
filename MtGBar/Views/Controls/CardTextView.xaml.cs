using System.Windows;
using System.Windows.Controls;

namespace MtGBar.Views.Controls
{
    public partial class CardTextView : UserControl
    {
        public CardTextView()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }
        
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", 
            typeof(string), 
            typeof(CardTextView), 
            new PropertyMetadata(null)
        );
    }
}