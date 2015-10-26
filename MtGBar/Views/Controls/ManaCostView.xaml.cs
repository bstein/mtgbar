using System.Windows;
using System.Windows.Controls;
using Melek;

namespace MtGBar.Views.Controls
{
    public partial class ManaCostView : UserControl
    {
        public ManaCostView()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }
        
        public CardCostCollection Cost
        {
            get { return (CardCostCollection)GetValue(CostProperty); }
            set { SetValue(CostProperty, value); }
        }
        
        public static readonly DependencyProperty CostProperty = DependencyProperty.Register(
            "Cost", 
            typeof(CardCostCollection), 
            typeof(ManaCostView), 
            new PropertyMetadata(null)
        );
    }
}