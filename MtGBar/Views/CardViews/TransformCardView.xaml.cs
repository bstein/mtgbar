using System.Windows;
using System.Windows.Controls;
using Melek.Domain;

namespace MtGBar.Views.CardViews
{
    public partial class TransformCardView : UserControl
    {
        public TransformCardView()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }
        
        public TransformCard Card
        {
            get { return (TransformCard)GetValue(CardProperty); }
            set { SetValue(CardProperty, value); }
        }
        
        public static readonly DependencyProperty CardProperty = DependencyProperty.Register(
            "Card", 
            typeof(TransformCard), 
            typeof(TransformCardView), 
            new PropertyMetadata(null)
        );
        
        public TransformPrinting Printing
        {
            get { return (TransformPrinting)GetValue(PrintingProperty); }
            set { SetValue(PrintingProperty, value); }
        }
        
        public static readonly DependencyProperty PrintingProperty = DependencyProperty.Register(
            "Printing", 
            typeof(TransformPrinting), 
            typeof(TransformCardView), 
            new PropertyMetadata(null)
        );
    }
}